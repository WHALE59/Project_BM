using BM.Interactables;
using UnityEngine;
using UnityEngine.UI;

namespace BM
{
	[DisallowMultipleComponent]
	public class EquipmentOverlay : MonoBehaviour
	{
		[SerializeField] private UseAction m_useAction;

		[Space()]

		[SerializeField] private Image m_equipmentIcon;

		private void EquipmentOverlay_Equipped(ItemSO equipped)
		{
			m_equipmentIcon.gameObject.SetActive(true);
			m_equipmentIcon.sprite = equipped.EquipmentIcon;
		}

		private void EquipmentOverlay_Unequipped(ItemSO unequipped)
		{
			m_equipmentIcon.sprite = null;
			m_equipmentIcon.gameObject.SetActive(false);
		}

		private void EquipmentOverlay_Used(ItemSO equipped)
		{
			m_equipmentIcon.sprite = null;
			m_equipmentIcon.gameObject.SetActive(false);
		}

		private void OnEnable()
		{
			m_useAction.Equipped += EquipmentOverlay_Equipped;
			m_useAction.Unequipped += EquipmentOverlay_Unequipped;

			m_useAction.Used += EquipmentOverlay_Used;
		}

		private void OnDisable()
		{
			m_useAction.Equipped -= EquipmentOverlay_Equipped;
			m_useAction.Unequipped -= EquipmentOverlay_Unequipped;

			m_useAction.Used -= EquipmentOverlay_Used;
		}
	}
}