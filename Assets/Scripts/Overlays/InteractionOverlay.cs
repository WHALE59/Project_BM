using BM.Interactables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(CanvasGroup))]
	public class InteractionOverlay : MonoBehaviour
	{
		[SerializeField] private InteractableDetector m_detector;

		[SerializeField] private InteractionCrosshair m_interactionCrosshair;
		private void InteractionOverlay_InteractableFound(InteractableBase interactable)
		{
			// 호버링 상태 돌입

			InteractableSO interactableSO = interactable.InteractableSO;

			if (null == interactableSO)
			{
				return;
			}

			// TODO: Usable has highest priority

			if (interactableSO.IsCollectible)
			{
				m_interactionCrosshair.SetCollectibleCrosshair();
			}
			else if (interactableSO.IsActivatable)
			{
				// m_interactionCrosshair.SetCrosshair(interactableSO)
				m_interactionCrosshair.SetActivatableCrosshair();
			}
		}

		private void InteractionOverlay_InteractableLost(InteractableBase interactable)
		{
			// 호버링 상태 해제

			InteractableSO interactableSO = interactable.InteractableSO;

			if (null == interactableSO)
			{
				return;
			}

			m_interactionCrosshair.SetDefaultCrosshair();
		}

		private void OnEnable()
		{
			if (null != m_detector)
			{
				m_detector.InteractableFound += InteractionOverlay_InteractableFound;
				m_detector.InteractableLost += InteractionOverlay_InteractableLost;
			}
		}

		private void OnDisable()
		{
			if (null != m_detector)
			{
				m_detector.InteractableFound -= InteractionOverlay_InteractableFound;
				m_detector.InteractableLost -= InteractionOverlay_InteractableLost;
			}
		}
	}
}