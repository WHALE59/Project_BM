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
			// 입력 애셋을 실제로 생성함
			_inputActions = new();

			// 각 입력을 캐릭터 행동 컴포넌트와 바인딩

			_moveAction = GetComponent<MoveAction>();

			_interactAction = GetComponent<InteractAction>();
			var interactInputAsset = _inputActions.Character.Interact;
			interactInputAsset.performed += OnInteractInputPerformed;
			interactInputAsset.canceled += OnInteractInputCancled;

			_crouchAction = GetComponent<CrouchAction>();
			var crouchInputAsset = _inputActions.Character.Crouch;
			if (!_crouchInputIsToggle)
			{
				crouchInputAsset.performed += OnCrouchInputPerformed;
				crouchInputAsset.canceled += OnCrouchInputCancled;
			}
			else
			{
				crouchInputAsset.performed += OnCrouchInputToggled;
			}

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

		// 구독 해제와 일관성 유지를 위해 래핑함

		void OnInteractInputPerformed(InputAction.CallbackContext context)
		{
			_interactAction.StartInteraction();
		}

		void OnInteractInputCancled(InputAction.CallbackContext context)
		{
			_interactAction.FinishInteraction();
		}

		void OnCrouchInputPerformed(InputAction.CallbackContext context)
		{
			_crouchAction.StartCrouch();
		}

		void OnCrouchInputToggled(InputAction.CallbackContext context)
		{
			if (!_isCrouchInputPerformed)
			{
				_crouchAction.StartCrouch();
			}
			else
			{
				_crouchAction.FinishCrouch();
			}

			_isCrouchInputPerformed = !_isCrouchInputPerformed;
		}

		void OnCrouchInputCancled(InputAction.CallbackContext context)
		{
			_crouchAction.FinishCrouch();
		}

		void OnEnable()
		{
			_inputActions.Enable();
		}

		void OnDisable()
		{
			_inputActions.Disable();
		}

#if UNITY_EDITOR
		void OnValidate()
		{
			_inputVisualizer?.gameObject.SetActive(_shouldVisuzlieInputs);

			if (_inputActions is null)
			{
				return;
			}

			// 에디터에서 플레이 중 _crouchInputIsToggle 체크시, 바로 반영 하기 위한 코드
			var crouchInputAsset = _inputActions.Character.Crouch;
			if (!_crouchInputIsToggle)
			{
				crouchInputAsset.performed -= OnCrouchInputToggled;

				crouchInputAsset.performed += OnCrouchInputPerformed;
				crouchInputAsset.canceled += OnCrouchInputCancled;
			}
			else
			{
				crouchInputAsset.performed -= OnCrouchInputPerformed;
				crouchInputAsset.canceled -= OnCrouchInputCancled;

				crouchInputAsset.performed += OnCrouchInputToggled;
			}

		}
#endif

		private void FixedUpdate()
		{
			if (!_moveAction)
			{
				return;
			}

			// 이동 입력은 지속적으로 폴링해야 하므로 어쩔 수 없이 FixedUpdate에 배치

			var move2 = _inputActions.Character.Move.ReadValue<Vector2>();
			var move3 = new Vector3(move2.x, 0.0f, move2.y);

			_moveAction.MoveToInputDirection(move3);
		}

		IA_InputActions _inputActions;

		MoveAction _moveAction;

		InteractAction _interactAction;

		CrouchAction _crouchAction;

		[Tooltip("Crouch 입력을 토글로 받을지에 대한 여부입니다.")]
		[SerializeField] bool _crouchInputIsToggle = false;

		bool _isCrouchInputPerformed = false;

#if UNITY_EDITOR
		[Header("[DEBUG] Visualize Inputs")]

		InputVisualizer _inputVisualizer;
		[SerializeField] InputVisualizer _inputVisualizerPrefab;
		[SerializeField] bool _shouldVisuzlieInputs = true;
#endif
	}
}