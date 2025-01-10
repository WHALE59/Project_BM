using BM.Interactables;
using TMPro;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace BM
{
	[DisallowMultipleComponent]
	public class ControlGuideOverlay : MonoBehaviour
	{
		[SerializeField] private bool m_isControlGuideEnabled = true;

		[Header("외부 레퍼런스")]

		[Space]

		[SerializeField] private InteractableDetector m_detector;
		[SerializeField] private UseAction m_useAction;

		[Header("UseAction 설정")]

		[SerializeField] private ControlGuideComponent m_controlGuideOnUseAction;
		[SerializeField] private LocalizedString m_useActionLocalizedString;

		[Header("InteractAction 설정")]

		[SerializeField] private ControlGuideComponent m_controlGuideOnInteractAction;
		[SerializeField] private TMP_Text m_interactActionControlGuideText;

#if UNITY_EDITOR
		[Header("Debug")]

		[SerializeField] private bool m_logOnControlGuideLocalized = false;
#endif

		private ItemSO m_characterEquipment;
		private bool m_enabled = false;

		private async void ControlGuideOverlay_InteractableFound(InteractableBase interactable)
		{
			if (!m_isControlGuideEnabled)
			{
				return;
			}

			m_enabled = true;

			// (1) 장비하고 있는 것이 있다면 UseAction Control Guide도 On
			// (장비 하면서 미리 Text는 Set 되어있어야 함)

			if (null != m_characterEquipment)
			{
				m_controlGuideOnUseAction.Enable();
			}

			// (2) 감지된 interactable의 InteractAction Control Guide 구성

			// TODO: 그냥 프로퍼티 자체를 async로 만들어 보는 것은 어떤지?

			string interactText = await interactable.ControlGuideLocalizedString.GetLocalizedStringAsync().Task;

#if UNITY_EDITOR
			if (m_logOnControlGuideLocalized)
			{
				Debug.Log($"{interactable}에 대한 GetLocalizedStringAsync의 결과로 {interactText} 반환");
			}
#endif

			m_controlGuideOnInteractAction.EnableWithText(interactText);

		}

		private void ControlGuideOverlay_InteractableLost(InteractableBase interactable)
		{
			if (!m_isControlGuideEnabled)
			{
				return;
			}

			m_enabled = false;

			m_controlGuideOnUseAction.Disable();
			m_controlGuideOnInteractAction.Disable();
		}

		private async void ControlGuideOverlay_Equipped(ItemSO equipped)
		{
			// 장비를 우선 캐싱

			m_characterEquipment = equipped;

			// TODO: ContainsKey 없이도 작동하는지 확인
			// 장비하고 있는 것으로 Use Action Control Guide 구성

			if (!m_useActionLocalizedString.ContainsKey("equipmentDisplayName"))
			{
				m_useActionLocalizedString.Add("equipmentDisplayName", m_characterEquipment.LocalizedInventoryDisplayName);
			}
			else
			{
				m_useActionLocalizedString["equipmentDisplayName"] = m_characterEquipment.LocalizedInventoryDisplayName;
			}

			string useActionText = await m_useActionLocalizedString.GetLocalizedStringAsync().Task;

#if UNITY_EDITOR
			if (m_logOnControlGuideLocalized)
			{
				Debug.Log($"장비 {equipped}에 대한 GetLocalizedStringAsync의 결과로 {useActionText} 반환");
			}
#endif
			m_controlGuideOnUseAction.SetText(useActionText);

			// 지금 오버레이가 활성화된 상태였으면 지금 활성화도 해야함 (예: 호버링 하던 중 장비)
			if (m_enabled)
			{
				m_controlGuideOnUseAction.Enable();
			}
		}

		private void ControlGuideOverlay_Unequipped(ItemSO unequipped)
		{
			m_characterEquipment = null;

			if (m_enabled)
			{
				m_controlGuideOnUseAction.Disable();
			}
		}

		private void Awake()
		{
			m_controlGuideOnUseAction.Disable();
			m_controlGuideOnInteractAction.Disable();
		}
		private void OnEnable()
		{
			m_detector.InteractableFound += ControlGuideOverlay_InteractableFound;
			m_detector.InteractableLost += ControlGuideOverlay_InteractableLost;

			m_useAction.Equipped += ControlGuideOverlay_Equipped;
			m_useAction.Unequipped += ControlGuideOverlay_Unequipped;
			m_useAction.Used += ControlGuideOverlay_Unequipped;
		}

		private void OnDisable()
		{
			m_detector.InteractableFound -= ControlGuideOverlay_InteractableFound;
			m_detector.InteractableLost -= ControlGuideOverlay_InteractableLost;

			m_useAction.Equipped -= ControlGuideOverlay_Equipped;
			m_useAction.Unequipped -= ControlGuideOverlay_Unequipped;
			m_useAction.Used -= ControlGuideOverlay_Unequipped;
		}
	}
}