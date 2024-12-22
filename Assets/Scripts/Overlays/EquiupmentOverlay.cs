using BM.Interactables;
using UnityEngine;
using UnityEngine.UI;

namespace BM
{
	[DisallowMultipleComponent]
	public class EquipmentOverlay : MonoBehaviour
	{
		[SerializeField] private InteractAction m_interactAction;

		[Space()]

		[SerializeField] private Image m_equipmentIcon;

		private void EquipmentOverlay_Equipped(InteractableSO interactableSO)
		{
			m_equipmentIcon.gameObject.SetActive(true);
			m_equipmentIcon.sprite = interactableSO.EquipmentIcon;
		}

		private void EquipmentOverlay_Unequipped()
		{
			m_equipmentIcon.sprite = null;
			m_equipmentIcon.gameObject.SetActive(false);
		}

		private void OnEnable()
		{
			m_interactAction.Equipped += EquipmentOverlay_Equipped;
			m_interactAction.Unequipped += EquipmentOverlay_Unequipped;
		}

		private void OnDisable()
		{
			m_interactAction.Equipped -= EquipmentOverlay_Equipped;
			m_interactAction.Unequipped -= EquipmentOverlay_Unequipped;
		}
	}
}