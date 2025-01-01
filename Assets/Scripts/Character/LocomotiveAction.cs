using System;
using System.Collections;

using UnityEngine;
using System.Diagnostics.CodeAnalysis;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(CharacterController))]
	public class LocomotiveAction : MonoBehaviour
	{
		[SerializeField] private InputReaderSO m_inputReader;
		[SerializeField] private LocomotivePropertySO m_properties;

		[Space()]

		[SerializeField] private Transform m_cameraTarget;

		public enum State
		{
			Idle,
			NormalJog,
			WalkedJog,
			CrouchedJog
		}

		public event Action<State> LocomotiveStateChanged;

		private State m_state;

		/// <summary>
		/// 이동 중 발생하는 "진동"의 시점을 알고 싶으면 구독.
		/// T1: 진동이 발생한 위치, T2: 진동의 세기
		/// </summary>
		private float m_uncrouchedCapsuleHeight;
		private Vector3 m_uncrouchedCapsuleCenter;

		private readonly int m_crouchCollisionLayerMask = ~(1 << 3); // 자기 자신을 제외한 모든 레이어

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

		private bool m_isWalking;
		private bool m_isCrouching;

		private Coroutine m_crouchRoutine;

		private CharacterController m_characterController;

#if UNITY_EDITOR
		[Header("로깅 설정")]
		[Space()]

		[SerializeField] private bool m_logOnStateChange = false;

		private bool m_isStuckWhileCrouching = false;
		private RaycastHit m_crouchHitInfo;
#endif

		private Vector3 WorldMoveDirection
		{
			get
			{
				Quaternion cameraYRotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);
				Vector3 worldMoveDirection = (cameraYRotation * m_localMoveDirection).normalized;

				return worldMoveDirection;
			}
		}

		public Vector3 CameraTargetLocalPosition
		{
			get
			{
				if (!m_characterController)
				{
					m_characterController = GetComponent<CharacterController>();
				}

				Vector3 ret = m_characterController.center + new Vector3(0f, (m_characterController.height - m_characterController.radius) / 2f, 0f);

				return ret;
			}
		}

		private void StateTransitionProcedure()
		{
			State previousState = m_state;
			State targetState = default;

			if (!m_isDesiredToMove)
			{
				targetState = State.Idle;
			}
			else if (!m_isCrouching && !m_isWalking)
			{
				targetState = State.NormalJog;
			}
			else if (!m_isCrouching && m_isWalking)
			{
				targetState = State.WalkedJog;
			}
			else if (m_isCrouching && !m_isWalking)
			{
				targetState = State.CrouchedJog;
			}
			else
			{
				Debug.Log("You're Fucked Up");
			}

			if (targetState == previousState)
			{
				return;
			}

			m_state = targetState;
			LocomotiveStateChanged?.Invoke(m_state);
		}

		private void LocomotiveAction_MoveInputEvent(Vector2 moveInput)
		{
			m_isDesiredToMove = true;

			m_localMoveDirection = new Vector3(moveInput.x, 0f, moveInput.y);

			StateTransitionProcedure();
		}

		private void LocomotiveAction_MoveInputCanceled(Vector2 _)
		{
			m_isDesiredToMove = false;
			StateTransitionProcedure();
		}

		private void LocomotiveAction_CrouchInputPerformed()
		{
			m_isCrouching = true;
			m_isWalking = false;

			StateTransitionProcedure();

			if (null != m_crouchRoutine)
			{
				StopCoroutine(m_crouchRoutine);
				m_crouchRoutine = null;
			}

#if UNITY_EDITOR
			m_isStuckWhileCrouching = false;
#endif
			m_crouchRoutine = StartCoroutine(CrouchRoutine(isCrouchingUp: false));
		}

		private void LocomotiveAction_CrouchInputCanceled()
		{
			if (null != m_crouchRoutine)
			{
				StopCoroutine(m_crouchRoutine);
				m_crouchRoutine = null;
			}

			m_crouchRoutine = StartCoroutine(CrouchRoutine(isCrouchingUp: true));
		}

		private void LocomotiveAction_WalkInputPerformed()
		{
			// 앉는 중에는 걸을 수 없음

			if (m_isCrouching)
			{
				return;
			}

			m_isWalking = true;

			StateTransitionProcedure();
		}

		private void LocomotiveAction_WalkInputCanceled()
		{
			m_isWalking = false;

			StateTransitionProcedure();
		}

		public bool IsGrounded => m_characterController.isGrounded;

		public bool IsDesiredToMove => m_isDesiredToMove;

		public bool IsMoving => m_isMoving;

		private void UpdateCameraTargetLocalPosition()
		{
			m_cameraTarget.localPosition = CameraTargetLocalPosition;
		}

		private IEnumerator CrouchRoutine(bool isCrouchingUp)
		{
			float startCapsuleHeight = m_characterController.height;
			Vector3 startCapsuleCenter = m_characterController.center;

			float crouchedCapsuleHeight = m_uncrouchedCapsuleHeight * m_properties.CrouchRatio;
			Vector3 crouchedCapsuleCenter = m_uncrouchedCapsuleCenter - new Vector3(0f, m_uncrouchedCapsuleHeight * (1 - m_properties.CrouchRatio) / 2f, 0f);

			float endCapsuleHeight = isCrouchingUp ? m_uncrouchedCapsuleHeight : crouchedCapsuleHeight;
			Vector3 endCapsuleCenter = isCrouchingUp ? m_uncrouchedCapsuleCenter : crouchedCapsuleCenter;

			float elapsedTime = 0.0f;

			// 로컬 메서드 정의

			bool IsStuckWhileCrouching()
			{
				bool isStuckWhileCrouching =
#if !UNITY_EDITOR
				Physics.Raycast(transform.position, transform.up, m_uncrouchedCapsuleHeight / 2.0f, m_crouchCollisionLayerMask);
#else
				Physics.Raycast(transform.position, transform.up, out m_crouchHitInfo, m_uncrouchedCapsuleHeight / 2f, m_crouchCollisionLayerMask);
				m_isStuckWhileCrouching = isStuckWhileCrouching;
#endif
				return isStuckWhileCrouching;
			}

			// 앉기 구분동작 타이머 시작

			while (elapsedTime < m_properties.CrouchDuration)
			{
				// 앉았다가 일어서는 중인데, 장애물이 있는 경우 대기

				if (isCrouchingUp)
				{
					while (IsStuckWhileCrouching())
					{
						yield return new WaitForFixedUpdate();
					}
				}

				elapsedTime += Time.fixedDeltaTime;

				float ratio = Mathf.Clamp01(elapsedTime / m_properties.CrouchDuration);

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
				m_isCrouching = false;

				StateTransitionProcedure();
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
			m_inputReader.MoveInputEvent += LocomotiveAction_MoveInputEvent;
			m_inputReader.MoveInputCanceled += LocomotiveAction_MoveInputCanceled;

			m_inputReader.CrouchInputPerformed += LocomotiveAction_CrouchInputPerformed;
			m_inputReader.CrouchInputCanceled += LocomotiveAction_CrouchInputCanceled;

			m_inputReader.WalkInputPerformed += LocomotiveAction_WalkInputPerformed;
			m_inputReader.WalkInputCanceled += LocomotiveAction_WalkInputCanceled;

#if UNITY_EDITOR
			LocomotiveStateChanged += StateLogger;
#endif
		}

		private void OnDisable()
		{
			m_inputReader.MoveInputEvent -= LocomotiveAction_MoveInputEvent;
			m_inputReader.MoveInputCanceled -= LocomotiveAction_MoveInputCanceled;

			m_inputReader.CrouchInputPerformed -= LocomotiveAction_CrouchInputPerformed;
			m_inputReader.CrouchInputCanceled -= LocomotiveAction_CrouchInputCanceled;

			m_inputReader.WalkInputPerformed -= LocomotiveAction_WalkInputPerformed;
			m_inputReader.WalkInputCanceled -= LocomotiveAction_WalkInputCanceled;

#if UNITY_EDITOR
			LocomotiveStateChanged -= StateLogger;
#endif

#if UNITY_EDITOR
#endif
		}

		private void Start()
		{
			m_state = State.Idle;
			LocomotiveStateChanged?.Invoke(m_state);

			UpdateCameraTargetLocalPosition();
		}

		private void FixedUpdate()
		{
			// 현재 프레임의 속도 계산 시작

			m_worldVelocity = WorldMoveDirection * m_properties.GetSpeedByState(m_state);

			if (!m_properties.IgnoreGravity)
			{
				if (m_characterController.isGrounded)
				{
					m_worldVelocity.y = -.1f;
				}
				else
				{
					m_worldVelocity.y += m_properties.Mass * Physics.gravity.y * Time.fixedDeltaTime;
				}
			}
			else
			{
				m_worldVelocity.y = 0f;
			}

			// 현재 프레임의 속도 계산 끝

			Vector3 planeVelocity = new(m_worldVelocity.x, 0f, m_worldVelocity.z);
			m_isMoving = planeVelocity != Vector3.zero;
			m_characterController.Move(m_worldVelocity * Time.fixedDeltaTime);

			// 회전 업데이트

			float targetRotation = Camera.main.transform.eulerAngles.y;
			transform.rotation = Quaternion.Euler(0f, targetRotation, 0f);
		}

#if UNITY_EDITOR

		private void StateLogger(State state)
		{
			if (!m_logOnStateChange)
			{
				return;
			}

			Debug.Log($"{state}로 상태 바뀜");
		}

		/// <summary>
		/// 캐릭터의 전방을 그린다.
		/// </summary>
		[DrawGizmo(GizmoType.Active)]
		[SuppressMessage("Style", "IDE0051")]
		private static void DrawForwardGizmo(LocomotiveAction target, GizmoType _)
		{
			if (!target.m_cameraTarget)
			{
				return;
			}

			if (!target.m_characterController)
			{
				target.m_characterController = target.GetComponent<CharacterController>();
			}

			Vector3 start = target.CameraTargetLocalPosition;
			Vector3 direction = target.transform.forward;

			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(start, direction * 1f);
		}

		[DrawGizmo(GizmoType.Active | GizmoType.Selected)]
		[SuppressMessage("Style", "IDE0051")]
		private static void DrawCrouchingRoutineStuckPointGizmo(LocomotiveAction target, GizmoType _)
		{
			if (!target.m_isStuckWhileCrouching)
			{
				return;
			}

			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(target.m_crouchHitInfo.point, .1f);
		}
#endif
	}
}
