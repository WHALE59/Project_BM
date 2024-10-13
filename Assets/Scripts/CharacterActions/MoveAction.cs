using UnityEngine;
using UnityEngine.InputSystem;

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
		}

		public void MoveToInputDirection(in Vector3 inputDirection)
		{
			UpdateHorizontalVelocity(inputDirection);
			UpdateVerticalVelocity();

			_characterController.Move(_velocity * Time.deltaTime);

			UpdateRotation();
		}

		void UpdateHorizontalVelocity(in Vector3 localDirection)
		{
			var worldDirection = Camera.main.transform.TransformDirection(localDirection);

			_velocity = worldDirection * _speed;
			_velocity.y = 0.0f;

			_characterController.Move(_velocity * Time.deltaTime);
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
				return;
			}

			_velocity.y += Physics.gravity.y * Time.deltaTime;
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

		Vector3 _velocity;

		InputAction _inputAction;
		CharacterController _characterController;

		[SerializeField] float _speed = 10.0f;
	}
}
