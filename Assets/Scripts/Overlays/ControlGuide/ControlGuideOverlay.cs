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

		[SerializeField] private ControlGuideComponent m_useControlGuide;
		[SerializeField] private LocalizeStringEvent m_useControlStringEvent;
		[SerializeField] private string m_useSmartStringKey = "equipmentDisplayName";

		[Space()]

		[SerializeField] private ControlGuideComponent m_collectControlGuide;
		[SerializeField] private LocalizeStringEvent m_collectControlStringEvent;
		[SerializeField] private string m_collectSmartStringKey = "displayName";

		[Space()]

		[SerializeField] private ControlGuideComponent m_activateControlGuide;
		[SerializeField] private LocalizeStringEvent m_activateControlStringEvent;
		[SerializeField] private string m_activateSmartStringKey = "displayName";

		private InteractableSO m_equipment;
		private bool m_enabled = false;

		private void ControlGuideOverlay_InteractableFound(InteractableBase interactable)
		{
			if (!m_isControlGuideEnabled)
			{
				return;
			}

			// 장비 가 있다면 장비 컨트롤 가이드 ON 
			m_enabled = true;

			if (null != m_equipment)
			{
				m_useControlGuide.gameObject.SetActive(true);
			}

			InteractableSO interactableSO = interactable.InteractableSO;

			if (null == interactableSO)
			{
				return;
			}

			/*
			 * 무조건 아래 중 하나임 
			 * * 배타적으로 Collectible인 경우
			 * * 배타적으로 Activatable인 경우
			 */

			if (interactableSO.IsCollectible && !interactableSO.IsActivatable)
			{
				m_collectControlStringEvent.StringReference[m_collectSmartStringKey] = interactableSO.LocalizedDisplayName;

				m_collectControlGuide.gameObject.SetActive(true);
				m_activateControlGuide.gameObject.SetActive(false);
			}
			else if (!interactableSO.IsCollectible && interactableSO.IsActivatable)
			{
				m_activateControlStringEvent.StringReference[m_activateSmartStringKey] = interactableSO.LocalizedDisplayName;

				m_collectControlGuide.gameObject.SetActive(false);
				m_activateControlGuide.gameObject.SetActive(true);
			}
			else
			{
				Debug.Log("If you see this, you're SO fucked up...");
			}
		}

		private void ControlGuideOverlay_InteractableLost(InteractableBase interactable)
		{
			if (!m_isControlGuideEnabled)
			{
				return;
			}

			m_enabled = false;

			m_useControlGuide.gameObject.SetActive(false);
			m_collectControlGuide.gameObject.SetActive(false);
			m_activateControlGuide.gameObject.SetActive(false);
		}

		private void ControlGuideOverlay_Equipped(InteractableSO equipped)
		{
			m_equipment = equipped;

			// TODO: ~를 ~에 로 스마트 스트링 자체를 바꿀 것
			m_useControlStringEvent.StringReference[m_useSmartStringKey] = equipped.LocalizedDisplayName;

			if (m_enabled)
			{
				m_useControlGuide.gameObject.SetActive(true);
			}
		}

		private void ControlGuideOverlay_Unequipped(InteractableSO unequipped)
		{
			m_equipment = null;

			if (m_enabled)
			{
				m_useControlGuide.gameObject.SetActive(false);
			}
		}

		private void Awake()
		{
			m_useControlGuide.gameObject.SetActive(false);
			m_collectControlGuide.gameObject.SetActive(false);
			m_activateControlGuide.gameObject.SetActive(false);
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