using UnityEngine;

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
	public class MoveAction : MonoBehaviour
	{
		void Awake()
		{
			_characterController = GetComponent<CharacterController>();
			_crouchAction = GetComponent<CrouchAction>();
		}

		void OnEnable()
		{
			_crouchAction.CrouchActionStarted += OnCrouchActionStarted;
			_crouchAction.CrouchActionFinished += OnCrouchActionFinished;
		}

		void OnDisable()
		{
			_crouchAction.CrouchActionStarted -= OnCrouchActionStarted;
			_crouchAction.CrouchActionFinished -= OnCrouchActionFinished;
		}

		void OnCrouchActionStarted() => _isCrouching = true;

		void OnCrouchActionFinished() => _isCrouching = false;

		public void MoveToInputDirection(in Vector3 inputDirection)
		{
			UpdateHorizontalVelocity(inputDirection);

			if (_applyGravity)
			{
				UpdateVerticalVelocity();
			}
			else
			{
				_velocity.y = 0.0f;
			}

			_characterController.Move(_velocity * Time.deltaTime);

			UpdateRotation();
		}

		void UpdateHorizontalVelocity(in Vector3 localDirection)
		{
			var worldDirection = Camera.main.transform.TransformDirection(localDirection).normalized;

			_velocity = worldDirection * SpeedByStance;
		}

		void UpdateRotation()
		{
			var targetRotation = Camera.main.transform.eulerAngles.y;
			transform.rotation = Quaternion.Euler(0.0f, targetRotation, 0.0f);
		}

		void UpdateVerticalVelocity()
		{
			if (_characterController.isGrounded)
			{
				_velocity.y = 0.0f;
				return;
			}

			_velocity.y += _mass * Physics.gravity.y * Time.deltaTime;
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
		float SpeedByStance => !_isCrouching ? _speedOnStand : _speedOnCrouch;

		Vector3 _velocity;

		CharacterController _characterController;

		CrouchAction _crouchAction;
		bool _isCrouching = false;

		[Tooltip("캐릭터가 서 있을 때의 이동 속도입니다.")]
		[SerializeField] float _speedOnStand = 10.0f;

		[Tooltip("캐릭터가 앉아 있을 때의 이동 속도입니다.")]
		[SerializeField] float _speedOnCrouch = 5.0f;

		[Tooltip("캐릭터가 가진 질량입니다. 클수록 캐릭터는 중력의 영향을 크게 받습니다.")]
		[SerializeField] float _mass = 50.0f;

		[Tooltip("캐릭터에게 중력을 적용할 지에 대한 여부입니다.")]
		[SerializeField] bool _applyGravity = true;
	}
}
