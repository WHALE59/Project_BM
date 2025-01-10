using BM.Interactables;
using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(CanvasGroup))]
	public class InteractionOverlay : MonoBehaviour
	{
		[Header("External Reference")]

		[SerializeField] private InteractableDetector m_detector;
		[SerializeField] private InteractionCrosshair m_interactionCrosshair;

		private void InteractionOverlay_InteractableFound(InteractableBase found)
		{
			m_interactionCrosshair.SetCrosshair(found.InteractionCrosshair);
		}

		private void InteractionOverlay_InteractableLost(InteractableBase lost)
		{
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