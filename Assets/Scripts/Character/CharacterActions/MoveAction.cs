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
	public class MoveAction : MonoBehaviour, IFootstepAudioPlayer
	{
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
		Vector3 _localMoveDirection;

		/// <summary>
		/// 카메라 방향과 캐릭터의 이동 속력까지 고려한, 캐릭터 오브젝트의 월드 방향
		/// </summary>
		Vector3 _worldVelocity;

		bool _isDesiredToMove;

		CharacterController _characterController;
		InputListener _inputListener;

		CrouchAction _crouchAction;
		EStanceState _stanceState = EStanceState.Stand;

		WalkAction _walkAction;
		EGaitState _gaitState = EGaitState.Jog;

		[Header("이동 속도 설정")]
		[Space()]
		[SerializeField] float _speedOnStandJog = 10.0f;
		[SerializeField] float _speedOnCrouchedJog = 5.0f;
		[SerializeField] float _speedOnStandWalk = 5.0f;
		[SerializeField] float _speedOnCrouchedWalk = 2.5f;

		[Header("이동 주체의 물리적 특징 설정")]
		[Space()]

		[Tooltip("캐릭터가 가진 질량입니다. 클수록 캐릭터는 중력의 영향을 크게 받습니다.")]
		[SerializeField] float _mass = 50.0f;

		[Tooltip("캐릭터에게 중력을 적용할 지에 대한 여부입니다.")]
		[SerializeField] bool _applyGravity = true;

		// TODO : 아마 이 데이터들을 관리하는 좀 더 합리적인 방법을 찾아야 할 것

		[Header("Footstep 설정")]
		[Space()]

		[Header("Footstep Impulse 설정 - 주기")]
		[Space()]

		[SerializeField] float _footstepImpulsePeriodOnStandJog = 0.25f;
		[SerializeField] float _footstepImpulsePeriodOnStandWalk = 0.4f;
		[SerializeField] float _footstepImpulsePeriodOnCrouchedJog = 0.4f;
		[SerializeField] float _footstepImpulsePeriodOnCrouchedWalk = 0.6f;

		[Header("Footstep Impulse 설정 - 세기")]
		[Space()]

		[SerializeField] float _footstepImpulseForceOnStandJog = 1.0f;
		[SerializeField] float _footstepImpulseForceOnStandWalk = 0.4f;
		[SerializeField] float _footstepImpulseForceOnCrouchedJog = 0.4f;
		[SerializeField] float _footstepImpulseForceOnCrouchedWalk = 0.3f;

		[Header("Footstep Audio 설정")]
		[Space()]

		[Tooltip("Footstep에 맞게 소리를 낼 지에 대한 여부입니다.")]
		[SerializeField] bool _applyFootstepSound = true;

		[SerializeField][Range(0.0f, 1.0f)] float _footstepAudioMasterVolume = 1.0f;

		[Tooltip("왼발과 오른발의 공간 편향이 어느정도인지 설정합니다.")]
		[SerializeField][Range(0.0f, 1.0f)] float _footstepAudioStereoPan = 0.3f;

		[Tooltip("기본 발소리의 소리들을 담고 있는 사전입니다.")]
		[SerializeField] FootstepAudioData _footstepAudioDataFallback;

		[Header("Footstep Camera Shake 설정")]
		[Space()]

		[Tooltip("Footstep에 맞게 카메라를 흔들 지에 대한 여부입니다.")]
		[SerializeField] bool _applyFootstepCameraShake = true;

		[SerializeField][Range(0.0f, 1.0f)] float _footstepCameraShakeMasterForce = 1.0f;

		[SerializeField] CinemachineImpulseDefinition.ImpulseShapes _footstepCameraShakeShape = CinemachineImpulseDefinition.ImpulseShapes.Recoil;
		[SerializeField] float _footstepCameraShakeDuration = 0.2f;
		[SerializeField] Vector3 _footstepCameraShakeDefaultVelocity = new(0.0f, -0.125f, 0.0f);

		/// <summary>
		/// 특정 볼륨 내에서 오버라이드되는 발소리
		/// </summary>
		FootstepAudioData _footstepAudioData;

		bool _isLeftFootstep = false;

		CinemachineImpulseSource _impulseSource;
		float _elapsedTimeAfterLastFootstepImpulse;
		AudioSource _audioSource;

		float Speed
		{
			get
			{
				if (_stanceState == EStanceState.Stand)
				{
					if (_gaitState == EGaitState.Jog)
					{
						return _speedOnStandJog;
					}
					else if (_gaitState == EGaitState.Walk)
					{
						return _speedOnStandWalk;
					}
				}
				else if (_stanceState == EStanceState.Crouched)
				{
					if (_gaitState == EGaitState.Jog)
					{
						return _speedOnCrouchedJog;
					}
					else if (_gaitState == EGaitState.Walk)
					{
						return _speedOnCrouchedWalk;
					}
				}
				return _speedOnStandJog;
			}
		}

		float FootstepImpulsePeriod
		{
			get
			{
				if (_gaitState == EGaitState.Jog)
				{
					if (_stanceState == EStanceState.Stand)
					{
						return _footstepImpulsePeriodOnStandJog;
					}
					else
					{
						return _footstepImpulsePeriodOnCrouchedJog;
					}
				}
				else
				{
					if (_stanceState == EStanceState.Stand)
					{
						return _footstepImpulsePeriodOnStandWalk;
					}
					else
					{
						return _footstepImpulsePeriodOnCrouchedWalk;
					}
				}
			}
		}

		float FootstepImpulseForce
		{
			get
			{
				if (_gaitState == EGaitState.Jog)
				{
					if (_stanceState == EStanceState.Stand)
					{
						return _footstepImpulseForceOnStandJog;
					}
					else
					{
						return _footstepImpulseForceOnCrouchedJog;
					}
				}
				else
				{
					if (_stanceState == EStanceState.Stand)
					{
						return _footstepImpulseForceOnStandWalk;
					}
					else
					{
						return _footstepImpulseForceOnCrouchedWalk;
					}
				}

			}
		}

		Vector3 WorldMoveDirection
		{
			get
			{
				var cameraYRotation = Quaternion.Euler(0.0f, Camera.main.transform.eulerAngles.y, 0);
				var worldMoveDirection = (cameraYRotation * _localMoveDirection).normalized;

				return worldMoveDirection;
			}
		}

		FootstepAudioData IFootstepAudioPlayer.FootstepAudioData
		{
			get
			{
				if (!_footstepAudioData)
				{
					return _footstepAudioDataFallback;
				}
				return _footstepAudioData;
			}
			set
			{
				_footstepAudioData = value;
			}
		}

		void UpdateMovementInput(Vector2 moveInput) => _localMoveDirection = new Vector3(moveInput.x, 0.0f, moveInput.y);

		void OnWalkStateStarted() => _gaitState = EGaitState.Walk;

		void OnWalkStateFinished() => _gaitState = EGaitState.Jog;

		void OnCrouchStateStarted() => _stanceState = EStanceState.Crouched;

		void OnCrouchStateFinished() => _stanceState = EStanceState.Stand;

		void GenerateFootstepImpulse()
		{
			// 지면에 닿아 있고, 플레이어가 수평으로 이동하고 있을 때에만 타이머에 따라 Footstep Impulse 생성

			if (!_characterController.isGrounded)
			{
				return;
			}

			if (!_isDesiredToMove)
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

			if (_elapsedTimeAfterLastFootstepImpulse < currentFootstepImpulsePeriod)
			{
				_elapsedTimeAfterLastFootstepImpulse += Time.deltaTime;

				if (_elapsedTimeAfterLastFootstepImpulse >= currentFootstepImpulsePeriod)
				{
					GenerateFootstepReaction(currentFootstepImpulseForce);

					_elapsedTimeAfterLastFootstepImpulse = 0.0f;
				}
			}

			if (_elapsedTimeAfterLastFootstepImpulse >= currentFootstepImpulsePeriod)
			{
				GenerateFootstepReaction(currentFootstepImpulseForce);

				_elapsedTimeAfterLastFootstepImpulse = 0.0f;
			}
		}

		void GenerateFootstepReaction(in float currentFootstepForce)
		{
			// Virtual Camera 가 수신할 수 있는 임펄스 발생

			if (_applyFootstepCameraShake)
			{
				_impulseSource.GenerateImpulse(currentFootstepForce * _footstepCameraShakeMasterForce);
			}

			// 사운드 재생

			if (_applyFootstepSound)
			{
				var footstepAudioPlayer = (IFootstepAudioPlayer)this;

				_audioSource.clip = footstepAudioPlayer.FootstepAudioData.GetProperFootstepClip();
				_audioSource.volume = currentFootstepForce * _footstepAudioMasterVolume;

				var bias = (_isLeftFootstep ? 1.0f : -1.0f) * _footstepAudioStereoPan;
				_audioSource.panStereo = bias;

				_isLeftFootstep = !_isLeftFootstep;

				_audioSource.Play();
			}
		}

		void Awake()
		{
			_characterController = GetComponent<CharacterController>();
			_inputListener = GetComponent<InputListener>();

			_crouchAction = GetComponent<CrouchAction>();
			_walkAction = GetComponent<WalkAction>();

			_impulseSource = GetComponent<CinemachineImpulseSource>();

			_impulseSource.m_ImpulseDefinition.m_ImpulseShape = _footstepCameraShakeShape;
			_impulseSource.m_ImpulseDefinition.m_ImpulseDuration = _footstepCameraShakeDuration;
			_impulseSource.m_DefaultVelocity = _footstepCameraShakeDefaultVelocity;

			_audioSource = GetComponent<AudioSource>();

#if UNITY_EDITOR
			if (!_walkAction || !_crouchAction)
			{
				Debug.LogWarning("CrouchAction 혹은 WalkAction을 찾을 수 없습니다.");
			}

			if (!_footstepAudioDataFallback)
			{
				Debug.LogWarning("FootstepAudioDataFallback이 할당되지 않았습니다.");
			}
#endif
		}

#if UNITY_EDITOR
		void OnValidate()
		{
			if (_impulseSource)
			{
				_impulseSource.m_ImpulseDefinition.m_ImpulseShape = _footstepCameraShakeShape;
				_impulseSource.m_ImpulseDefinition.m_ImpulseDuration = _footstepCameraShakeDuration;
				_impulseSource.m_DefaultVelocity = _footstepCameraShakeDefaultVelocity;
			}
		}
#endif

		void OnEnable()
		{
			_inputListener.Moved += UpdateMovementInput;

			_crouchAction.CrouchStateStarted += OnCrouchStateStarted;
			_crouchAction.CrouchStateFinished += OnCrouchStateFinished;

			_walkAction.WalkStateStarted += OnWalkStateStarted;
			_walkAction.WalkStateFinished += OnWalkStateFinished;
		}

		void OnDisable()
		{
			_inputListener.Moved -= UpdateMovementInput;

			_crouchAction.CrouchStateStarted -= OnCrouchStateStarted;
			_crouchAction.CrouchStateFinished -= OnCrouchStateFinished;

			_walkAction.WalkStateStarted -= OnWalkStateStarted;
			_walkAction.WalkStateFinished -= OnWalkStateFinished;
		}

		void FixedUpdate()
		{
			// 현재 프레임의 속도 계산 시작

			_worldVelocity = WorldMoveDirection * Speed;

			if (_applyGravity)
			{
				if (_characterController.isGrounded)
				{
					_worldVelocity.y = -0.1f;
				}
				else
				{
					_worldVelocity.y += _mass * Physics.gravity.y * Time.fixedDeltaTime;
				}
			}
			else
			{
				_worldVelocity.y = 0.0f;
			}

			// 현재 프레임의 속도 계산 끝

			var planeVelocity = new Vector3(_worldVelocity.x, 0.0f, _worldVelocity.z);
			_isDesiredToMove = planeVelocity != Vector3.zero && _localMoveDirection != Vector3.zero;
			_characterController.Move(_worldVelocity * Time.fixedDeltaTime);


			// 회전 업데이트

			var targetRotation = Camera.main.transform.eulerAngles.y;
			transform.rotation = Quaternion.Euler(0.0f, targetRotation, 0.0f);
		}

		void Update()
		{
			GenerateFootstepImpulse();
		}

#if UNITY_EDITOR
		/// <summary>
		/// 캐릭터의 전방을 그린다.
		/// </summary>
		[DrawGizmo(GizmoType.Active)]
		static void DrawForwardGizmo(MoveAction target, GizmoType _)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(target.transform.position, target.transform.forward * 1.0f);
		}
#endif

	}
}
