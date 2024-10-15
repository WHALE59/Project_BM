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

			// Move
			_moveAction = GetComponent<MoveAction>();

			// Interact
			_interactAction = GetComponent<InteractAction>();
			var interactInputAction = _inputActions.Character.Interact;
			interactInputAction.performed += OnInteractInputPerformed;
			interactInputAction.canceled += OnInteractInputCancled;

			// Crouch
			_crouchAction = GetComponent<CrouchAction>();
			var crouchInputAction = _inputActions.Character.Crouch;
			if (!_crouchInputIsToggle)
			{
				crouchInputAction.performed += OnCrouchInputPerformed;
				crouchInputAction.canceled += OnCrouchInputCanceled;
			}
			else
			{
				crouchInputAction.performed += OnCrouchInputToggled;
			}

			// Walk
			_walkAction = GetComponent<WalkAction>();
			var walkInputAction = _inputActions.Character.Walk;
			if (!_walkInputIsToggle)
			{
				walkInputAction.performed += OnWalkInputPerformed;
				walkInputAction.canceled += OnWalkInputCanceled;
			}
			else
			{
				walkInputAction.performed += OnWalkInputToggled;
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

		void OnWalkInputPerformed(InputAction.CallbackContext context)
		{
			_walkAction.StartWalk();
		}

		void OnWalkInputCanceled(InputAction.CallbackContext context)
		{
			_walkAction.FinishWalk();
		}

		void OnWalkInputToggled(InputAction.CallbackContext context)
		{
			if (!_isWalkInputPerformed)
			{
				_walkAction.StartWalk();
			}
			else
			{
				_walkAction.FinishWalk();
			}

			_isWalkInputPerformed = !_isWalkInputPerformed;
		}

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

		void OnCrouchInputCanceled(InputAction.CallbackContext context)
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

			// 에디터에서 플레이 중 Toggle 설정 변경을 바로 반영하기 위한 코드

			var crouchInputAsset = _inputActions.Character.Crouch;
			if (!_crouchInputIsToggle)
			{
				crouchInputAsset.performed -= OnCrouchInputToggled;

				crouchInputAsset.performed += OnCrouchInputPerformed;
				crouchInputAsset.canceled += OnCrouchInputCanceled;
			}
			else
			{
				crouchInputAsset.performed -= OnCrouchInputPerformed;
				crouchInputAsset.canceled -= OnCrouchInputCanceled;

				crouchInputAsset.performed += OnCrouchInputToggled;
			}

			var walkInputAsset = _inputActions.Character.Walk;
			if (!_walkInputIsToggle)
			{
				walkInputAsset.performed -= OnWalkInputToggled;

				walkInputAsset.performed += OnWalkInputPerformed;
				walkInputAsset.canceled += OnWalkInputCanceled;
			}
			else
			{
				walkInputAsset.performed -= OnWalkInputPerformed;
				walkInputAsset.canceled -= OnWalkInputCanceled;

				walkInputAsset.performed += OnWalkInputToggled;
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

		// Move
		MoveAction _moveAction;

		// Interact
		InteractAction _interactAction;

		// Crouch
		CrouchAction _crouchAction;

		[Tooltip("Crouch 입력을 토글로 받을지에 대한 여부입니다.")]
		[SerializeField] bool _crouchInputIsToggle = false;

		bool _isCrouchInputPerformed = false;

		// Walk
		WalkAction _walkAction;

		[Tooltip("Walk 입력을 토글로 받을지에 대한 여부입니다.")]
		[SerializeField] bool _walkInputIsToggle = false;

		bool _isWalkInputPerformed = false;

#if UNITY_EDITOR
		[Header("[DEBUG] Visualize Inputs")]

		InputVisualizer _inputVisualizer;
		[SerializeField] InputVisualizer _inputVisualizerPrefab;
		[SerializeField] bool _shouldVisuzlieInputs = true;
#endif
	}
}