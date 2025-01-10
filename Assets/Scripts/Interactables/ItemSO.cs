using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace BM.Interactables
{
	[CreateAssetMenu(menuName = "BM/SO/Item SO", fileName = "ItemSO_ItemName")]
	public class ItemSO : ScriptableObject
	{
		[Header("인벤토리 설정")]

		[SerializeField] private Sprite m_inventoryIcon;

		[Space]

		[SerializeField] private LocalizedString m_inventoryDisplayName;
		[SerializeField] private LocalizedString m_inventoryDescription;

		[Header("Usable 인 경우 설정")]

		[SerializeField] private bool m_isUsable;

		[SerializeField] private Sprite m_equipmentIcon;
		[SerializeField] private Sprite m_useCrosshairIcon;

		[Tooltip("이 아이템의 사용처. 예를 들어 이 아이템이 열쇠라면, 이 목록에는 열쇠로 열 수 있는 상자(들)이 할당되어야 함")]
		[SerializeField] private List<InteractableBase> m_usedTo;

		public bool IsUsable => m_isUsable;
		public Sprite EquipmentIcon => m_equipmentIcon;
		public Sprite InventoryIcon => m_inventoryIcon;

		public LocalizedString LocalizedInventoryDisplayName => m_inventoryDisplayName;

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