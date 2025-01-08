using BM.Interactables;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace BM
{
	[DisallowMultipleComponent]
	public class ControlGuideOverlay : MonoBehaviour
	{
		[SerializeField] private InteractableDetector m_detector;
		[SerializeField] private UseAction m_useAction;

		[Space()]

		[SerializeField] private bool m_isControlGuideEnabled = true;

		[Space()]

		[SerializeField] private ControlGuideComponent m_controlGuideOnUseAction;
		[SerializeField] private LocalizeStringEvent m_useControlStringEvent;
		[SerializeField] private string m_useSmartStringKey = "equipmentDisplayName";

		[Space()]

		[SerializeField] private ControlGuideComponent m_controlGuideOnInteractAction;
		[SerializeField] private LocalizeStringEvent m_stringEventOnInteractAction;
		[SerializeField] private string m_smartKeyOnInteractAction = "displayName";

		private ItemSO m_equipment;
		private bool m_enabled = false;

		private void ControlGuideOverlay_InteractableFound(InteractableBase interactable)
		{
			if (!m_isControlGuideEnabled)
			{
				return;
			}

			m_enabled = true;

			// 장비 가 있다면 장비 컨트롤 가이드 ON 
			if (null != m_equipment)
			{
				m_controlGuideOnUseAction.gameObject.SetActive(true);
			}

			/*
			 * 무조건 아래 중 하나임 
			 * * 배타적으로 Collectible인 경우
			 * * 배타적으로 Activatable인 경우
			 */

			m_controlGuideOnInteractAction.gameObject.SetActive(true);
			//m_stringEventOnInteractAction.StringReference[m_smartKeyOnInteractAction] = interactable.ControlGuideLocalizedString;
		}

		private void ControlGuideOverlay_InteractableLost(InteractableBase interactable)
		{
			if (!m_isControlGuideEnabled)
			{
				return;
			}

			m_enabled = false;

			m_controlGuideOnUseAction.gameObject.SetActive(false);
			m_controlGuideOnInteractAction.gameObject.SetActive(false);
		}

		private void ControlGuideOverlay_Equipped(ItemSO equipped)
		{
			m_equipment = equipped;

			// TODO: ~를 ~에 로 스마트 스트링 자체를 바꿀 것
			m_useControlStringEvent.StringReference[m_useSmartStringKey] = equipped.LocalizedDisplayName;

			if (m_enabled)
			{
				m_controlGuideOnUseAction.gameObject.SetActive(true);
			}
		}

		private void ControlGuideOverlay_Unequipped(ItemSO unequipped)
		{
			m_equipment = null;

			if (m_enabled)
			{
				m_controlGuideOnUseAction.gameObject.SetActive(false);
			}
		}

		private void Awake()
		{
			m_controlGuideOnUseAction.gameObject.SetActive(false);
			m_controlGuideOnInteractAction.gameObject.SetActive(false);
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