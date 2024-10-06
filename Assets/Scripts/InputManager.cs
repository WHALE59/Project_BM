using UnityEngine;

namespace BM
{

	[DisallowMultipleComponent]
	public class InputManager : MonoBehaviour
	{
		public static InputManager Instance { get; private set; }

		void Awake()
		{
			_inputActions = new();

			Instance ??= this;
		}

		void OnEnable()
		{
			_inputActions.Enable();
		}

		void OnDisable()
		{
			_inputActions.Disable();
		}

		public Vector2 MovementInput => _inputActions is not null ? _inputActions.Character.Move.ReadValue<Vector2>() : Vector2.zero;
		public Vector2 LookInput => _inputActions is not null ? _inputActions.Character.Look.ReadValue<Vector2>() : Vector2.zero;

		IA_InputActions _inputActions;
	}
}