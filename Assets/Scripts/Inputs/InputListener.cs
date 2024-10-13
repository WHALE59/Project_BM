using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEngine.InputSystem.Samples;
#endif

namespace BM
{

	/// <summary>
	/// 입력 애셋을 생성하고, 입력과 각 행동 컴포넌트를 연결한다.
	/// </summary>
	[DisallowMultipleComponent]
	public class InputListener : MonoBehaviour
	{
		void Awake()
		{
			_inputActions = new();

			_moveAction = GetComponent<MoveAction>();

			_interactAction = GetComponent<InteractAction>();
			var interactInputAsset = _inputActions.Character.Interact;

			interactInputAsset.performed += (InputAction.CallbackContext _) => { _interactAction.BeginInteraction(); };
			interactInputAsset.canceled += (InputAction.CallbackContext _) => { _interactAction.EndInteraction(); };

#if UNITY_EDITOR
			// Input Visualizer를 일단 생성 후 설정에 따라 활성화/비활성화

			if (!_inputVisualizerPrefab)
			{
				return;
			}

			_inputVisualizer = Instantiate(_inputVisualizerPrefab);
			_inputVisualizer.transform.SetParent(transform);

			if (!_shouldVisuzlieInputs)
			{
				_inputVisualizer.gameObject.SetActive(false);
			}
#endif
		}

		void OnEnable()
		{
			_inputActions.Enable();
		}

#if UNITY_EDITOR
		void OnValidate()
		{
			_inputVisualizer?.gameObject.SetActive(_shouldVisuzlieInputs);
		}
#endif

		void OnDisable()
		{
			_inputActions.Disable();
		}

		void FixedUpdate()
		{
			if (!_moveAction)
			{
				return;
			}

			var move2 = _inputActions.Character.Move.ReadValue<Vector2>();
			var move3 = new Vector3(move2.x, 0.0f, move2.y);

			_moveAction.MoveToInputDirection(move3);
		}

		IA_InputActions _inputActions;

		MoveAction _moveAction;
		InteractAction _interactAction;

#if UNITY_EDITOR
		[Header("[DEBUG] Visualize Inputs")]

		InputVisualizer _inputVisualizer;
		[SerializeField] InputVisualizer _inputVisualizerPrefab;
		[SerializeField] bool _shouldVisuzlieInputs = true;
#endif
	}
}