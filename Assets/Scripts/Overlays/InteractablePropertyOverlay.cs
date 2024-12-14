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
		[SerializeField] private InteractableDetector m_interactableDetector;

		[SerializeField] TMP_Text m_equipAndUseText;
		[SerializeField] TMP_Text m_collectText;
		[SerializeField] TMP_Text m_activateText;
		[SerializeField] TMP_Text m_placeText;
		[SerializeField] TMP_Text m_usedText;

		private void SetTextVisibility(bool isEquippable = false, bool isCollectible = false, bool isActivatable = false, bool isPlaceable = false, bool isUsable = false, bool isUsedable = false)
		{
			m_equipAndUseText.gameObject.SetActive(isEquippable);
			m_collectText.gameObject.SetActive(isCollectible);
			m_activateText.gameObject.SetActive(isActivatable);
			m_placeText.gameObject.SetActive(isPlaceable);

			m_usedText.gameObject.SetActive(isUsedable);
		}

		private void InteractablePropertyOverlay_InteractableFound(InteractableBase interactable)
		{
			SetTextVisibility(interactable.IsEquipPlaceable, interactable.IsCollectible, interactable.IsActivatable, false, false, false);
		}

		private void InteractablePropertyOverlay_InteractableLost(InteractableBase interactable)
		{
			SetTextVisibility();
		}

		private void Start()
		{
			SetTextVisibility();
		}

		private void OnEnable()
		{
			m_interactableDetector.InteractableFound += InteractablePropertyOverlay_InteractableFound;
			m_interactableDetector.InteractableLost += InteractablePropertyOverlay_InteractableLost;
		}

		private void OnDisable()
		{
			m_interactableDetector.InteractableFound -= InteractablePropertyOverlay_InteractableFound;
			m_interactableDetector.InteractableLost -= InteractablePropertyOverlay_InteractableLost;
		}
	}
}