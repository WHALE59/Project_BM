using System;

using UnityEngine;

using Cinemachine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BM
{
	/// <summary>
	/// 입력으로부터 Vector3형 방향 정보를 받아서, 캐릭터를 해당 방향으로 지정 속도만큼 이동시킨다.
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(InputListener))]
	[RequireComponent(typeof(CinemachineImpulseSource))]
	[RequireComponent(typeof(AudioSource))]
	public class MoveAction : MonoBehaviour
	{
		public Action<float> Footstepped;

		// TODO : 제대로 된 스테이트 머신이 나오면 상태 관리를 더 세분화 할 것.
		public enum EGaitState
		{
			Jog, Walk
		}

		public enum EStanceState
		{
			Stand, Crouched
		}

		/// <summary>
		/// 카메라 방향을 고려하기 이전, 캐릭터 오브젝트의 지역 방향
		/// </summary>
		private Vector3 m_localMoveDirection;

		/// <summary>
		/// 카메라 방향과 캐릭터의 이동 속력까지 고려한, 캐릭터 오브젝트의 월드 방향
		/// </summary>
		private Vector3 m_worldVelocity;

		private bool m_isDesiredToMove;

		private CharacterController m_characterController;
		private InputListener m_inputListener;

		private CrouchAction m_crouchAction;
		private EStanceState m_stanceState = EStanceState.Stand;

		private WalkAction m_walkAction;
		private EGaitState m_gaitState = EGaitState.Jog;

		[Header("이동 속도 설정")]
		[Space()]
		[SerializeField] private float m_speedOnStandJog = 7.0f; // 10.0f; 
		[SerializeField] private float m_speedOnCrouchedJog = 2.0f; // 5.0f;
		[SerializeField] private float m_speedOnStandWalk = 4.0f; // 5.0f;
		[SerializeField] private float m_speedOnCrouchedWalk = 2.0f; // 2.5f;

		[Header("이동 주체의 물리적 특징 설정")]
		[Space()]

		[Tooltip("캐릭터가 가진 질량입니다. 클수록 캐릭터는 중력의 영향을 크게 받습니다.")]
		[SerializeField] private float m_mass = 50.0f;

		[Tooltip("캐릭터에게 중력을 적용할 지에 대한 여부입니다.")]
		[SerializeField] private bool m_applyGravity = true;

		// TODO : 아마 이 데이터들을 관리하는 좀 더 합리적인 방법을 찾아야 할 것

		[Header("Footstep 설정")]
		[Space()]

		[Header("Footstep Impulse 설정 - 주기")]
		[Space()]

		[SerializeField] private float m_footstepImpulsePeriodOnStandJog = 0.35f; // 0.25f;
		[SerializeField] private float m_footstepImpulsePeriodOnStandWalk = 0.6f; // 0.4f;
		[SerializeField] private float m_footstepImpulsePeriodOnCrouchedJog = 0.8f; // 0.4f;
		[SerializeField] private float m_footstepImpulsePeriodOnCrouchedWalk = 0.8f; // 0.6f;

		[Header("Footstep Impulse 설정 - 세기")]
		[Space()]

		[SerializeField] private float m_footstepImpulseForceOnStandJog = 1.0f;
		[SerializeField] private float m_footstepImpulseForceOnStandWalk = 0.7f / 0.4f; // 0.4f;
		[SerializeField] private float m_footstepImpulseForceOnCrouchedJog = 0.4f / 0.7f; // 0.4f;
		[SerializeField] private float m_footstepImpulseForceOnCrouchedWalk = 0.3f;

		[Header("Footstep Audio 설정")]
		[Space()]

		[Tooltip("FootstepBase 오디오 재생 여부입니다.")]
		[SerializeField] private bool m_applyFootstepBaseAudio = true;

		[SerializeField][Range(0.0f, 1.0f)] private float m_footstepBaseAudioMasterVolume = 1.0f;

		[Tooltip("왼발과 오른발의 공간 편향이 어느정도인지 설정합니다.")]
		[SerializeField][Range(0.0f, 1.0f)] private float m_footstepAudioStereoPan = 0.085f;

		[Tooltip("FootstepBase의 소리들을 담고 있는 사전입니다.")]
		[SerializeField] private RandomAudioClipSet m_footstepBaseAudioDataFallback;

		private RandomAudioClipSet m_footstepBaseAudioDataOverride;

		[Header("Footstep Camera Shake 설정")]
		[Space()]

		[Tooltip("Footstep에 맞게 카메라를 흔들 지에 대한 여부입니다.")]
		[SerializeField] private bool m_applyFootstepCameraShake = true;

		[SerializeField][Range(0.0f, 1.0f)] private float m_footstepCameraShakeMasterForce = 1.0f;

		[SerializeField] CinemachineImpulseDefinition.ImpulseShapes m_footstepCameraShakeShape = CinemachineImpulseDefinition.ImpulseShapes.Recoil;
		[SerializeField] private float m_footstepCameraShakeDuration = 0.2f;
		[SerializeField] private Vector3 m_footstepCameraShakeDefaultVelocity = new(0.0f, -0.125f, 0.0f);


		private bool m_isLeftFootstep = false;

		private CinemachineImpulseSource m_impulseSource;
		private float m_elapsedTimeAfterLastFootstepImpulse;
		private AudioSource m_footstepAudioBaseSource;

		private float Speed
		{
			get
			{
				if (m_stanceState == EStanceState.Stand)
				{
					if (m_gaitState == EGaitState.Jog)
					{
						return m_speedOnStandJog;
					}
					else if (m_gaitState == EGaitState.Walk)
					{
						return m_speedOnStandWalk;
					}
				}
				else if (m_stanceState == EStanceState.Crouched)
				{
					if (m_gaitState == EGaitState.Jog)
					{
						return m_speedOnCrouchedJog;
					}
					else if (m_gaitState == EGaitState.Walk)
					{
						return m_speedOnCrouchedWalk;
					}
				}
				return m_speedOnStandJog;
			}
		}

		private float FootstepImpulsePeriod
		{
			get
			{
				if (m_gaitState == EGaitState.Jog)
				{
					if (m_stanceState == EStanceState.Stand)
					{
						return m_footstepImpulsePeriodOnStandJog;
					}
					else
					{
						return m_footstepImpulsePeriodOnCrouchedJog;
					}
				}
				else
				{
					if (m_stanceState == EStanceState.Stand)
					{
						return m_footstepImpulsePeriodOnStandWalk;
					}
					else
					{
						return m_footstepImpulsePeriodOnCrouchedWalk;
					}
				}
			}
		}

		private float FootstepImpulseForce
		{
			get
			{
				if (m_gaitState == EGaitState.Jog)
				{
					if (m_stanceState == EStanceState.Stand)
					{
						return m_footstepImpulseForceOnStandJog;
					}
					else
					{
						return m_footstepImpulseForceOnCrouchedJog;
					}
				}
				else
				{
					if (m_stanceState == EStanceState.Stand)
					{
						return m_footstepImpulseForceOnStandWalk;
					}
					else
					{
						return m_footstepImpulseForceOnCrouchedWalk;
					}
				}

			}
		}

		private Vector3 WorldMoveDirection
		{
			get
			{
				var cameraYRotation = Quaternion.Euler(0.0f, Camera.main.transform.eulerAngles.y, 0);
				var worldMoveDirection = (cameraYRotation * m_localMoveDirection).normalized;

				return worldMoveDirection;
			}
		}

		public RandomAudioClipSet FootstepBaseAudioData
		{
			get
			{
				if (!m_footstepBaseAudioDataOverride)
				{
					return m_footstepBaseAudioDataFallback;
				}

				return m_footstepBaseAudioDataOverride;
			}
			set
			{
				m_footstepBaseAudioDataOverride = value;
			}
		}

		private void UpdateMovementInput(Vector2 moveInput) => m_localMoveDirection = new Vector3(moveInput.x, 0.0f, moveInput.y);

		private void OnWalkStateStarted() => m_gaitState = EGaitState.Walk;

		private void OnWalkStateFinished() => m_gaitState = EGaitState.Jog;

		private void OnCrouchStateStarted() => m_stanceState = EStanceState.Crouched;

		private void OnCrouchStateFinished() => m_stanceState = EStanceState.Stand;

		private void GenerateFootstepImpulse()
		{
			// 지면에 닿아 있고, 플레이어가 수평으로 이동하고 있을 때에만 타이머에 따라 Footstep Impulse 생성

			if (!m_characterController.isGrounded)
			{
				return;
			}

			if (!m_isDesiredToMove)
			{
				return;
			}

			/* 
			 * 현재 프레임에서 발소리 타이머에 도달했는지 판정,
			 * 도달 했으면, Footstep Impulse에 대한 Reaction 생성
			*/

			var currentFootstepImpulsePeriod = FootstepImpulsePeriod;
			var currentFootstepImpulseForce = FootstepImpulseForce;

			// 발소리 타이머

			if (m_elapsedTimeAfterLastFootstepImpulse < currentFootstepImpulsePeriod)
			{
				m_elapsedTimeAfterLastFootstepImpulse += Time.deltaTime;

				if (m_elapsedTimeAfterLastFootstepImpulse >= currentFootstepImpulsePeriod)
				{
					GenerateFootstepReaction(currentFootstepImpulseForce);

					m_elapsedTimeAfterLastFootstepImpulse = 0.0f;
				}
			}

			if (m_elapsedTimeAfterLastFootstepImpulse >= currentFootstepImpulsePeriod)
			{
				GenerateFootstepReaction(currentFootstepImpulseForce);

				m_elapsedTimeAfterLastFootstepImpulse = 0.0f;
			}
		}

		private void GenerateFootstepReaction(in float currentFootstepForce)
		{
			// Virtual Camera 가 수신할 수 있는 임펄스 발생

			if (m_applyFootstepCameraShake)
			{
				m_impulseSource.GenerateImpulse(currentFootstepForce * m_footstepCameraShakeMasterForce);
			}

			// 사운드 재생

			var bias = (m_isLeftFootstep ? 1.0f : -1.0f) * m_footstepAudioStereoPan;

			if (m_applyFootstepBaseAudio)
			{
				m_footstepAudioBaseSource.volume = currentFootstepForce * m_footstepBaseAudioMasterVolume;
				m_footstepAudioBaseSource.panStereo = bias;

				var data = FootstepBaseAudioData;
				if (data)
				{
					var clip = data.PickClip();
					if (clip is not null)
					{
						m_footstepAudioBaseSource.clip = clip;
						m_footstepAudioBaseSource.Play();
					}
				}
			}

			Footstepped?.Invoke(currentFootstepForce);

			m_isLeftFootstep = !m_isLeftFootstep;
		}

		private void Awake()
		{
			m_characterController = GetComponent<CharacterController>();

			m_footstepAudioBaseSource = GetComponent<AudioSource>();

			m_impulseSource = GetComponent<CinemachineImpulseSource>();

			m_impulseSource.m_ImpulseDefinition.m_ImpulseShape = m_footstepCameraShakeShape;
			m_impulseSource.m_ImpulseDefinition.m_ImpulseDuration = m_footstepCameraShakeDuration;
			m_impulseSource.m_DefaultVelocity = m_footstepCameraShakeDefaultVelocity;

			m_inputListener = GetComponent<InputListener>();

			m_crouchAction = GetComponent<CrouchAction>();
			m_walkAction = GetComponent<WalkAction>();

#if UNITY_EDITOR
			if (!m_walkAction || !m_crouchAction)
			{
				Debug.LogWarning("CrouchAction 혹은 WalkAction을 찾을 수 없습니다.");
			}

			if (!m_footstepBaseAudioDataFallback)
			{
				Debug.LogWarning("FootstepBaseAudioDataFallback이 할당되지 않았습니다.");
			}
#endif
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (m_impulseSource)
			{
				m_impulseSource.m_ImpulseDefinition.m_ImpulseShape = m_footstepCameraShakeShape;
				m_impulseSource.m_ImpulseDefinition.m_ImpulseDuration = m_footstepCameraShakeDuration;
				m_impulseSource.m_DefaultVelocity = m_footstepCameraShakeDefaultVelocity;
			}
		}
#endif

		private void OnEnable()
		{
			m_inputListener.Moved += UpdateMovementInput;

			m_crouchAction.CrouchStateStarted += OnCrouchStateStarted;
			m_crouchAction.CrouchStateFinished += OnCrouchStateFinished;

			m_walkAction.WalkStateStarted += OnWalkStateStarted;
			m_walkAction.WalkStateFinished += OnWalkStateFinished;
		}

		private void OnDisable()
		{
			m_inputListener.Moved -= UpdateMovementInput;

			m_crouchAction.CrouchStateStarted -= OnCrouchStateStarted;
			m_crouchAction.CrouchStateFinished -= OnCrouchStateFinished;

			m_walkAction.WalkStateStarted -= OnWalkStateStarted;
			m_walkAction.WalkStateFinished -= OnWalkStateFinished;
		}

		private void FixedUpdate()
		{
			// 현재 프레임의 속도 계산 시작

			m_worldVelocity = WorldMoveDirection * Speed;

			if (m_applyGravity)
			{
				if (m_characterController.isGrounded)
				{
					m_worldVelocity.y = -0.1f;
				}
				else
				{
					m_worldVelocity.y += m_mass * Physics.gravity.y * Time.fixedDeltaTime;
				}
			}
			else
			{
				m_worldVelocity.y = 0.0f;
			}

			// 현재 프레임의 속도 계산 끝

			var planeVelocity = new Vector3(m_worldVelocity.x, 0.0f, m_worldVelocity.z);
			m_isDesiredToMove = planeVelocity != Vector3.zero && m_localMoveDirection != Vector3.zero;
			m_characterController.Move(m_worldVelocity * Time.fixedDeltaTime);

			// 회전 업데이트

			var targetRotation = Camera.main.transform.eulerAngles.y;
			transform.rotation = Quaternion.Euler(0.0f, targetRotation, 0.0f);
		}

		private void Update()
		{
			GenerateFootstepImpulse();
		}

#if UNITY_EDITOR
		/// <summary>
		/// 캐릭터의 전방을 그린다.
		/// </summary>
		[DrawGizmo(GizmoType.Active)]
		private static void DrawForwardGizmo(MoveAction target, GizmoType _)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(target.transform.position, target.transform.forward * 1.0f);
		}
#endif

	}
}
