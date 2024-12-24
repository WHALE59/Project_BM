using BM.Interactables;
using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	public class ControlGuideOverlay : MonoBehaviour
	{
		[SerializeField] private InteractAction m_interactAction;

		[Space()]

		[SerializeField] private bool m_isEnabled = true;

		[Space()]

		[SerializeField] private ControlGuideComponent m_useControlGuid;
		[SerializeField] private ControlGuideComponent m_collectControlGuide;
		[SerializeField] private ControlGuideComponent m_activateControlGuide;

		private void ControlGuideOverlay_InteractableFound(InteractableBase interactable)
		{
			if (!m_isEnabled)
			{
				return;
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
				m_collectControlGuide.gameObject.SetActive(true);
				m_activateControlGuide.gameObject.SetActive(false);
			}
			else if (!interactableSO.IsCollectible && interactableSO.IsActivatable)
			{
				m_collectControlGuide.gameObject.SetActive(false);
				m_activateControlGuide.gameObject.SetActive(true);
			}
			else
			{
				Debug.Log("If you see this, you're fucked up...");
			}
		}

		private void ControlGuideOverlay_InteractableLost(InteractableBase interactable)
		{
			if (!m_isEnabled)
			{
				return;
			}

			m_useControlGuid.gameObject.SetActive(false);
			m_collectControlGuide.gameObject.SetActive(false);
			m_activateControlGuide.gameObject.SetActive(false);
		}

		private void OnEnable()
		{
			m_interactAction.InteractableFound += ControlGuideOverlay_InteractableFound;
			m_interactAction.InteractableLost += ControlGuideOverlay_InteractableLost;
		}

		private void OnDisable()
		{
			m_interactAction.InteractableFound -= ControlGuideOverlay_InteractableFound;
			m_interactAction.InteractableLost -= ControlGuideOverlay_InteractableLost;
		}

		private void Awake()
		{
			m_useControlGuid.gameObject.SetActive(false);
			m_collectControlGuide.gameObject.SetActive(false);
			m_activateControlGuide.gameObject.SetActive(false);
		}
	}
}