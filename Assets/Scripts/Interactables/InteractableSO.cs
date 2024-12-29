using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace BM.Interactables
{
	[CreateAssetMenu(menuName = "BM/SO/Interactable SO", fileName = "InteractableSO_Default")]
	public class InteractableSO : ScriptableObject
	{
		[Header("Collectible Settings")]
		[Space]

		[SerializeField] private bool m_isCollectible;
		[SerializeField] private EventReference m_soundOnCollectingOverride;
		[SerializeField] private Sprite m_inventoryIcon;

		// InventoryModel;

		[Header("Activatable Settings")]
		[Space]

		[SerializeField] private bool m_isActivatable;
		[SerializeField] private Sprite m_crosshairOnActivationOverride;

		[Header("Usable Settings")]
		[Space]

		[SerializeField] private bool m_isUsable;
		[SerializeField] private Sprite m_equipmentIcon;
		[SerializeField] private Sprite m_useCrosshairIcon;
		[SerializeField] private List<InteractableSO> m_isUsedTo;

		[Header("Usedable Settings")]
		[Space]

		[SerializeField] private bool m_isUsedable;
		[SerializeField] private List<InteractableSO> m_isUsedBy;

		[Header("Crafting Settings")]
		[Space]

		[SerializeField] private bool m_isCraftingMaterial;
		[SerializeField] private List<InteractableSO> m_isCraftingMaterialOf;
		[SerializeField] private List<InteractableSO> m_isCraftedBy;

		[Header("Text Data")]
		[Space]

		[SerializeField] private LocalizedString m_displayName;
		[SerializeField] private LocalizedString m_description;


		/// <remarks>
		/// 게임 로직상 Collectible과 Activatable은 동시에 가질 수 있는 속성이 아니지만, 이 프로퍼티의 반환값이 참이라고 해서, <see cref="IsActivatable"/>의 반환값이 거짓임이 보장되지는 않는다.
		/// </remarks>
		public bool IsCollectible => m_isCollectible;

		/// <remarks>
		/// 게임 로직상 Collectible과 Activatable은 동시에 가질 수 있는 속성이 아니지만, 이 프로퍼티의 반환값이 참이라고 해서, <see cref="IsCollectible"/>의 반환값이 거짓임이 보장되지는 않는다.
		/// </remarks>
		public bool IsActivatable => m_isActivatable;

		public bool IsUsable => m_isUsable;
		public bool IsUsedable => m_isUsedable;
		public Sprite EquipmentIcon => m_equipmentIcon;
		public LocalizedString LocalizedDisplayName => m_displayName;

		public bool IsUsedTo(InteractableSO target)
		{
			foreach (InteractableSO thisTarget in m_isUsedTo)
			{
				if (thisTarget != target)
				{
					continue;
				}

				return true;
			}

			return false;
		}

		public bool IsUsedBy(InteractableSO target)
		{
			foreach (InteractableSO thisTarget in m_isUsedBy)
			{
				if (thisTarget != target)
				{
					continue;
				}

				return true;
			}

			return false;
		}

		public EventReference SoundOnCollectingOverride => m_soundOnCollectingOverride;
		public Sprite CrosshairOnActivationOverride => m_crosshairOnActivationOverride;
	}
}