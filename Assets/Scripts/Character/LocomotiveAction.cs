using System;
using System.Collections;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(AudioSource))]
	public class LocomotiveAction : MonoBehaviour
	{
		public event Action<bool, float> LocomotionImpulseGenerated;

		[Header("입력 설정")]
		[Space()]

		[SerializeField] InputReader m_inputReader;

		// TODO : 제대로 된 스테이트 머신이 나오면 상태 관리를 더 세분화 할 것.
		public enum EGaitState
		{
			Jog, Walk
		}

		public enum EStanceState
		{
			Stand, Crouched
		}

		[Header("이동 속도 설정")]
		[Space()]
		[SerializeField] private float m_speedOnStandJog = 6.0f;
		[SerializeField] private float m_speedOnCrouchedJog = 2.0f;
		[SerializeField] private float m_speedOnStandWalk = 4.0f;
		[SerializeField] private float m_speedOnCrouchedWalk = 2.0f;

		[Header("이동 주체의 물리적 특징 설정")]
		[Space()]

		[Tooltip("캐릭터가 가진 질량입니다. 클수록 캐릭터는 중력의 영향을 크게 받습니다.")]
		[SerializeField] private float m_mass = 50.0f;

		[Tooltip("캐릭터에게 중력을 적용할 지에 대한 여부입니다.")]
		[SerializeField] private bool m_applyGravity = true;

		[Header("앉기 설정")]

		[SerializeField] private Transform m_cameraTarget;

		[SerializeField] private float m_crouchDuration = 0.25f;
		[SerializeField] private float m_crouchedRatio = 0.5f;

		// TODO : 아마 이 데이터들을 관리하는 좀 더 합리적인 방법을 찾아야 할 것

		[Header("Locomotion Impulse 설정")]
		[Space()]

		[SerializeField] private float m_impulsePeriodOnStandJog = 0.55f;
		[SerializeField] private float m_impulsePeriodOnStandWalk = 0.7f;
		[SerializeField] private float m_impulsePeriodOnCrouchedJog = 0.8f;
		[SerializeField] private float m_impulsePeriodOnCrouchedWalk = 0.8f;

		[Space()]

		[SerializeField] private float m_impulseForceOnStandJog = 0.55f;
		[SerializeField] private float m_impulseForceOnStandWalk = 0.4f;
		[SerializeField] private float m_impulseForceOnCrouchedJog = 0.3f;
		[SerializeField] private float m_impulseForceOnCrouchedWalk = 0.3f;

		private bool m_isLeftImpulse = false;

		private float m_elapsedTimeAfterLastImpulse;

		private float m_uncrouchedCapsuleHeight;
		private Vector3 m_uncrouchedCapsuleCenter;
		private Vector3 m_normalVcamLocalPosition;

		// TODO : 임시로 자기 자신을 제외한 모든 레이어로 설정
		private readonly int m_crouchCollisionLayerMask = ~(1 << 3);

		/// <summary>
		/// 카메라 방향을 고려하기 이전, 캐릭터 오브젝트의 지역 방향
		/// </summary>
		private Vector3 m_localMoveDirection;

		/// <summary>
		/// 카메라 방향과 캐릭터의 이동 속력까지 고려한, 캐릭터 오브젝트의 월드 방향
		/// </summary>
		private Vector3 m_worldVelocity;

		private bool m_isMoving;
		private bool m_isDesiredToMove;

		private CharacterController m_characterController;
		private EStanceState m_stanceState = EStanceState.Stand;
		private EGaitState m_gaitState = EGaitState.Jog;

#if UNITY_EDITOR
		private bool m_isStuckedWhileCrouching = false;
		private RaycastHit m_crouchHitInfo;
#endif

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

		private float ImpulsePeriod
		{
			get
			{
				if (m_gaitState == EGaitState.Jog)
				{
					if (m_stanceState == EStanceState.Stand)
					{
						return m_impulsePeriodOnStandJog;
					}
					else
					{
						return m_impulsePeriodOnCrouchedJog;
					}
				}
				else
				{
					if (m_stanceState == EStanceState.Stand)
					{
						return m_impulsePeriodOnStandWalk;
					}
					else
					{
						return m_impulsePeriodOnCrouchedWalk;
					}
				}
			}
		}

		private float ImpulseForce
		{
			get
			{
				if (m_gaitState == EGaitState.Jog)
				{
					if (m_stanceState == EStanceState.Stand)
					{
						return m_impulseForceOnStandJog;
					}
					else
					{
						return m_impulseForceOnCrouchedJog;
					}
				}
				else
				{
					if (m_stanceState == EStanceState.Stand)
					{
						return m_impulseForceOnStandWalk;
					}
					else
					{
						return m_impulseForceOnCrouchedWalk;
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

		private void UpdateLocalMoveDirection(Vector2 moveInput)
		{
			m_isDesiredToMove = moveInput != Vector2.zero;
			m_localMoveDirection = new Vector3(moveInput.x, 0.0f, moveInput.y);
		}

		private void GenerateLocomotionImpulse()
		{
			// 지면에 닿아 있지 않거나, 이동 할 의도가 없거나, 움직이지 않고 있다면 Impulse가 생성되어서는 안 됨.

			if (!m_characterController.isGrounded || !m_isDesiredToMove || !m_isMoving)
			{
				return;
			}

			// 현재 프레임에서 Impulse 타이머에 도달했는지 판정 시작

			var currentImpulsePeriod = ImpulsePeriod;
			var currentImpulseForce = ImpulseForce;

			// Impulse 타이머

			if (m_elapsedTimeAfterLastImpulse < currentImpulsePeriod)
			{
				m_elapsedTimeAfterLastImpulse += Time.deltaTime;

				if (m_elapsedTimeAfterLastImpulse >= currentImpulsePeriod)
				{
					// 이벤트 발생!
					LocomotionImpulseGenerated?.Invoke(m_isLeftImpulse, currentImpulseForce);
					m_isLeftImpulse = !m_isLeftImpulse;

					m_elapsedTimeAfterLastImpulse = 0.0f;
				}
			}

			// currentImpulsePeriod 가 매 프레임 변경될 수 있으므로, 한번 더 검사해 주어야 함.

			if (m_elapsedTimeAfterLastImpulse >= currentImpulsePeriod)
			{
				// 이벤트 발생!
				LocomotionImpulseGenerated?.Invoke(m_isLeftImpulse, currentImpulseForce);
				m_isLeftImpulse = !m_isLeftImpulse;

				m_elapsedTimeAfterLastImpulse = 0.0f;
			}
		}

		private void UpdateCameraTargetLocalPosition()
		{
			m_cameraTarget.localPosition =
				m_characterController.center +
				new Vector3(0.0f, (m_characterController.height - m_characterController.radius) / 2.0f, 0.0f);
		}

		private void StartCrouch()
		{
			m_stanceState = EStanceState.Crouched;

			StopAllCoroutines();

#if UNITY_EDITOR
			m_isStuckedWhileCrouching = false;
#endif
			StartCoroutine(CrouchRoutine(isCrouchingUp: false));
		}

		private void FinishCrouch()
		{
			StopAllCoroutines();
			StartCoroutine(CrouchRoutine(isCrouchingUp: true));
		}

		private void StartWalk()
		{
			m_gaitState = EGaitState.Walk;
		}

		private void FinishWalk()
		{
			m_gaitState = EGaitState.Jog;
		}

		private IEnumerator CrouchRoutine(bool isCrouchingUp)
		{
			var startCapsuleHeight = m_characterController.height;
			var startCapsuleCenter = m_characterController.center;

			var crouchedCapsuleHeight = m_uncrouchedCapsuleHeight * m_crouchedRatio;
			var crouchedCapsuleCenter = m_uncrouchedCapsuleCenter - new Vector3(0.0f, m_uncrouchedCapsuleHeight * (1 - m_crouchedRatio) / 2.0f, 0.0f);

			var endCapsuleHeight = isCrouchingUp ? m_uncrouchedCapsuleHeight : crouchedCapsuleHeight;
			var endCapsuleCenter = isCrouchingUp ? m_uncrouchedCapsuleCenter : crouchedCapsuleCenter;

			var elapsedTime = 0.0f;

			// 로컬 메서드 정의

			bool IsStuckedWhileCrouching()
			{
				var isStuckedWhileCrouching =
#if !UNITY_EDITOR
				Physics.Raycast(transform.position, transform.up, m_uncrouchedCapsuleHeight / 2.0f, m_crouchCollisionLayerMask);
#else
							Physics.Raycast(transform.position, transform.up, out m_crouchHitInfo, m_uncrouchedCapsuleHeight / 2.0f, m_crouchCollisionLayerMask);
				m_isStuckedWhileCrouching = isStuckedWhileCrouching;
#endif
				return isStuckedWhileCrouching;
			}

			// 앉기 구분동작 타이머 시작

			while (elapsedTime < m_crouchDuration)
			{
				// 앉았다가 일어서는 중인데, 장애물이 있는 경우 대기

				if (isCrouchingUp)
				{
					while (IsStuckedWhileCrouching())
					{
						yield return new WaitForFixedUpdate();
					}
				}

				elapsedTime += Time.fixedDeltaTime;

				var ratio = Mathf.Clamp01(elapsedTime / m_crouchDuration);

				m_characterController.height = Mathf.Lerp(startCapsuleHeight, endCapsuleHeight, ratio);
				m_characterController.center = Vector3.Lerp(startCapsuleCenter, endCapsuleCenter, ratio);

				// 카메라의 위치 변경

				UpdateCameraTargetLocalPosition();

				yield return new WaitForFixedUpdate();
			}

			// 타이머 끝난 후, 끝 값(Target Value)으로 초기화

			m_characterController.height = endCapsuleHeight;
			m_characterController.center = endCapsuleCenter;

			UpdateCameraTargetLocalPosition();

			// 일어 서는 중 코드가 여기까지 도달해야, 완전히 일어난 상태로 판정

			if (isCrouchingUp)
			{
				m_stanceState = EStanceState.Stand;
			}
		}

		private void Awake()
		{
			m_characterController = GetComponent<CharacterController>();


			m_uncrouchedCapsuleHeight = m_characterController.height;
			m_uncrouchedCapsuleCenter = m_characterController.center;
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (m_characterController)
			{
				m_uncrouchedCapsuleHeight = m_characterController.height;
				m_uncrouchedCapsuleCenter = m_characterController.center;
			}
		}
#endif

		private void OnEnable()
		{
			m_inputReader.MoveInputEvent += UpdateLocalMoveDirection;

			m_inputReader.CrouchInputPerformed += StartCrouch;
			m_inputReader.CrouchInputCanceled += FinishCrouch;

			m_inputReader.WalkInputPerformed += StartWalk;
			m_inputReader.WalkInputCanceled += FinishWalk;
		}

		private void OnDisable()
		{
			m_inputReader.MoveInputEvent -= UpdateLocalMoveDirection;

			m_inputReader.CrouchInputPerformed -= StartCrouch;
			m_inputReader.CrouchInputCanceled -= FinishCrouch;

			m_inputReader.WalkInputPerformed -= StartWalk;
			m_inputReader.WalkInputCanceled -= FinishWalk;
		}

		private void Start()
		{
			UpdateCameraTargetLocalPosition();
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
			m_isMoving = planeVelocity != Vector3.zero;
			m_characterController.Move(m_worldVelocity * Time.fixedDeltaTime);

			// 회전 업데이트

			var targetRotation = Camera.main.transform.eulerAngles.y;
			transform.rotation = Quaternion.Euler(0.0f, targetRotation, 0.0f);
		}

		private void Update()
		{
			GenerateLocomotionImpulse();
		}

#if UNITY_EDITOR
		/// <summary>
		/// 캐릭터의 전방을 그린다.
		/// </summary>
		[DrawGizmo(GizmoType.Active)]
		private static void DrawForwardGizmo(LocomotiveAction target, GizmoType _)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(target.transform.position, target.transform.forward * 1.0f);
		}

		[DrawGizmo(GizmoType.Active | GizmoType.Selected)]
		private static void DrawCrouchStuckedPointGizmo(LocomotiveAction target, GizmoType _)
		{
			if (!target.m_isStuckedWhileCrouching)
			{
				return;
			}

			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(target.m_crouchHitInfo.point, 0.1f);
		}
#endif
	}
}
