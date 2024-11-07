using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BM
{
	/// <summary>
	/// 상호작용 가능한 오브젝트와 상호작용 하는 시나리오에서 표시될 UI 객체들의 행동을 관리한다.
	/// </summary>
	public class InteractableOverlay : MonoBehaviour
	{
		[Header("Interactable Control Icons")]

		[Tooltip("각 게임패드에 맞는 아이콘을 저장하고 있는 데이터입니다.")]
		[SerializeField] InputDeviceDisplayConfigurator _inputDeviceDisplayConfigurator;

		[Tooltip("게임패드를 사용할 때, 아이콘이 표시될 오브젝트입니다.")]
		[SerializeField] Image _gamepadIcon;

		[Tooltip("키보드를 사용할 때, 아이콘이 표시될 오브젝트입니다.")]
		[SerializeField] Text _keyboardIcon;

		[Tooltip("상호작용 가능 객체의 상태 정보가 표시될 오브젝트입니다.")]
		[SerializeField] Text _displayName;
		[SerializeField] Text _interactionName;

		void Awake()
		{
#if UNITY_EDITOR
			if (!_inputDeviceDisplayConfigurator)
			{
				Debug.LogWarning("InteractableUI에서 Input Device Display Configurator 오브젝트가 할당되지 않았습니다.");
			}
			if (!_gamepadIcon || !_keyboardIcon)
			{
				Debug.LogWarning("InteractableUI에서 Keyboard & Icon 오브젝트가 할당되지 않았습니다.");
			}
#endif

			SetActive(false);
		}

		void SetActive(bool active)
		{
			_keyboardIcon.gameObject.SetActive(active);
			_gamepadIcon.gameObject.SetActive(active);
			_displayName.gameObject.SetActive(active);
			_interactionName.gameObject.SetActive(active);
		}

		public void StartHoveringUI(IInteractableObject interactableObject)
		{
			SetActive(true);

			_displayName.text = interactableObject.Data.displayName;
			_interactionName.text = interactableObject.Data.interactionName;

		}

		public void FinishHoveringUI(IInteractableObject interactableObject)
		{
			SetActive(false);
		}

		public void StartInteractUI(IInteractableObject interactableObject)
		{

		}

		public void FinishInteractUI(IInteractableObject interactable)
		{

		}

		/// <summary>
		/// 입력 장치가 변경되었을 때, 해당 장치의 Interact 액션에 대응하는 바인딩을 찾는다. 
		/// 그리고, 그 바인딩에 맞는 아이콘을 업데이트한다.
		/// </summary>
		/// <remarks>
		/// 에디터에서 Charcter 프리팹의 PlayerInput 컴포넌트의 Events의 대응하는 항목에 아래 함수를 바인드 해줘야 한다.
		/// </remarks>
		public void OnControlsChanged(PlayerInput playerInput)
		{
			var interactAction = playerInput.actions.FindAction("Interact");
			var controlBindingIndex = interactAction.GetBindingIndexForControl(interactAction.controls[0]);
			var currentBindingInput = InputControlPath.ToHumanReadableString
			(
				path: interactAction.bindings[controlBindingIndex].effectivePath,
				options: InputControlPath.HumanReadableStringOptions.OmitDevice
			);

			var icon = _inputDeviceDisplayConfigurator.GetInputDeviceBindIcon(playerInput, currentBindingInput);

			if (!icon)
			{
				// Keyboard
				_keyboardIcon.enabled = true;
				_keyboardIcon.text = $"({currentBindingInput})";
				_gamepadIcon.enabled = false;
			}
			else
			{
				// Gamepad
				_gamepadIcon.enabled = true;
				_gamepadIcon.sprite = icon;
				_keyboardIcon.enabled = false;
			}
		}
	}
}
