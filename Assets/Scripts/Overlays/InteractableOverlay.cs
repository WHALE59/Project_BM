using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BM
{
	/// <summary>
	/// 상호작용 가능한 오브젝트와 상호작용 하는 시나리오에서 표시될 UI 객체들의 행동을 관리한다.
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(CanvasGroup))]
	public class InteractableOverlay : MonoBehaviour
	{
		//[Tooltip("아이콘이 표시될 오브젝트입니다.")]
		//[SerializeField] private Text m_control;

		//[Tooltip("상호작용 가능 객체의 상태 정보가 표시될 오브젝트입니다.")]
		//[SerializeField] private Text m_displayName;
		[SerializeField] private TMP_Text m_interactionName;

		private CanvasGroup m_canvasGroup;

		private void Show(string displayName, string interactionName)
		{
			m_canvasGroup.alpha = 1.0f;

			//m_displayName.text = displayName;
			m_interactionName.text = displayName;
		}

		private void Hide()
		{
			m_canvasGroup.alpha = 0.0f;
		}
		public void StartHoveringUI(IInteractableObject interactableObject)
		{
			Show(interactableObject.Data.displayName, interactableObject.Data.interactionName);
		}

		public void FinishHoveringUI(IInteractableObject interactableObject)
		{
			Hide();
		}

		public void StartInteractUI(IInteractableObject interactableObject)
		{

		}

		public void FinishInteractUI(IInteractableObject interactable)
		{

		}

		private void Awake()
		{
			m_canvasGroup = GetComponent<CanvasGroup>();
		}

		private void Start()
		{
			Hide();
		}

		/// <summary>
		/// 입력 장치가 변경되었을 때, 해당 장치의 Interact 액션에 대응하는 바인딩을 찾는다. 
		/// 그리고, 그 바인딩에 맞는 아이콘을 업데이트한다.
		/// </summary>
		/// <remarks>
		/// 에디터에서 Charcter 프리팹의 PlayerInput 컴포넌트의 Events의 대응하는 항목에 아래 함수를 바인드 해줘야 한다.
		/// </remarks>
		//public void OnControlsChanged(PlayerInput playerInput)
		//{
		//	var interactAction = playerInput.actions.FindAction("Interact");
		//	var controlBindingIndex = interactAction.GetBindingIndexForControl(interactAction.controls[0]);
		//	var currentBindingInput = InputControlPath.ToHumanReadableString
		//	(
		//		path: interactAction.bindings[controlBindingIndex].effectivePath,
		//		options: InputControlPath.HumanReadableStringOptions.OmitDevice
		//	);

		//	var icon = _inputDeviceDisplayConfigurator.GetInputDeviceBindIcon(playerInput, currentBindingInput);

		//	if (!icon)
		//	{
		//		// Keyboard
		//		m_control.enabled = true;
		//		m_control.text = $"({currentBindingInput})";
		//		m_gamePadIcon.enabled = false;
		//	}
		//	else
		//	{
		//		// Gamepad
		//		m_gamePadIcon.enabled = true;
		//		m_gamePadIcon.sprite = icon;
		//		m_control.enabled = false;
		//	}
		//}
	}
}
