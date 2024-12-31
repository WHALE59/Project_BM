using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace BM.Interactables
{
	[CreateAssetMenu(menuName = "BM/SO/Item SO", fileName = "ItemSO_ItemName")]
	public class ItemSO : ScriptableObject
	{
		[Header("Inventory Settings")]

		[SerializeField] private Sprite m_inventoryIcon;

		[Header("Usable Settings")]

		[SerializeField] private bool m_isUsable;
		[SerializeField] private Sprite m_equipmentIcon;
		[SerializeField] private Sprite m_useCrosshairIcon;
		[SerializeField] private List<InteractableBase> m_usedTo;

		[Header("Crafting Settings")]

		[SerializeField] private bool m_isCraftingMaterial;
		[SerializeField] private List<ItemSO> m_isCraftingMaterialOf;
		[SerializeField] private List<ItemSO> m_isCraftedBy;

		[Header("Text Data")]

		[SerializeField] private LocalizedString m_displayName;
		[SerializeField] private LocalizedString m_description;

		public bool IsUsable => m_isUsable;
		public Sprite EquipmentIcon => m_equipmentIcon;
		public Sprite InventoryIcon => m_inventoryIcon;

		public LocalizedString LocalizedDisplayName => m_displayName;

		public bool IsUsedTo(InteractableBase usedable)
		{
			foreach (InteractableBase thisTarget in m_usedTo)
			{
				if (thisTarget == usedable)
				{
					continue;
				}

				return true;
			}

			return false;
		}
	}
}