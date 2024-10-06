using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(CharacterController))]
	public class Character : MonoBehaviour
	{
		void Awake()
		{
			_characterController = GetComponent<CharacterController>();
		}

		void Start()
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		void FixedUpdate()
		{
			// Move character

			var move2 = InputManager.Instance.MovementInput;
			var move3 = new Vector3(move2.x, 0.0f, move2.y);

			var translatedMove = Camera.main.transform.TransformDirection(move3);

			var velocity = translatedMove * _moveSpeed;
			velocity.y = 0.0f;

			_characterController.Move(velocity * Time.deltaTime);

			// Rotate character 

			var targetRotation = Camera.main.transform.eulerAngles.y;
			transform.rotation = Quaternion.Euler(0.0f, targetRotation, 0.0f);

			// TODO: 중력 처리
		}

#if UNITY_EDITOR
		[DrawGizmo(GizmoType.Active)]
		static void DrawForwardGizmo(Character target, GizmoType _)
		{
			// forward
			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(target.transform.position, target.transform.forward * 4.0f);
		}

		[DrawGizmo(GizmoType.Selected | GizmoType.Active)]
		static void DrawMoveInputGizmo(Character target, GizmoType _)
		{
			var moveInput = InputManager.Instance.MovementInput;
			if (moveInput == Vector2.zero)
			{
				return;
			}

			// raw input
			Gizmos.color = Color.magenta;
			Gizmos.DrawRay(target.transform.position, ConvertToMoveDirection(moveInput));
		}
#endif

		static Vector3 ConvertToMoveDirection(in Vector2 input) => (new Vector3(input.x, 0.0f, input.y)).normalized;

		CharacterController _characterController;

		[SerializeField] float _moveSpeed = 10.0f;
	}
}
