using BM.Interactables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(CanvasGroup))]
	public class InteractablePropertyOverlay : MonoBehaviour
	{
		[SerializeField] private InteractAction m_interactAction;

		private void InteractablePropertyOverlay_InteractableFound(InteractableBase interactable)
		{
			// 호버링 상태 돌입
		}

		private void InteractablePropertyOverlay_InteractableLost(InteractableBase interactable)
		{
			// 호버링 상태 해제
		}

		private void OnEnable()
		{
			m_interactAction.InteractableFound += InteractablePropertyOverlay_InteractableFound;
			m_interactAction.InteractableLost += InteractablePropertyOverlay_InteractableLost;
		}

		private void OnDisable()
		{
			m_interactAction.InteractableFound -= InteractablePropertyOverlay_InteractableFound;
			m_interactAction.InteractableLost -= InteractablePropertyOverlay_InteractableLost;
		}
	}
}