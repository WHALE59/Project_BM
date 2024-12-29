using System;
using System.Collections.Generic;

using UnityEngine;

namespace BM.Interactables
{
	[CreateAssetMenu(menuName = "BM/SO/Interactable SO", fileName = "InteractableSO_Default")]
	public class InteractableSO : ScriptableObject
	{
		[Tooltip("이 아이템이 형체를 가져야 한다면, 그 형체 정보를 담고 있는 프리팹")]
		[SerializeField] private InteractableModel m_interactableModelPrefab;

		[Header("사용성")]

		[Space()]

		[SerializeField] private bool m_isCollectible;

		[Space()]

		[SerializeField] private bool m_isActivatable;
		[SerializeField] private Sprite m_activateCrosshairIcon;

		[Space()]

		[SerializeField] private bool m_isUsableAndEquippable;

		[SerializeField] private Sprite m_useAndEquipIcon;
		[SerializeField] private Sprite m_useCrosshairIcon;
		[SerializeField] private List<InteractableSO> m_isUsedTo;

		[Space()]

		[SerializeField] private bool m_isUsedable;
		[SerializeField] private List<InteractableSO> m_isUsedBy;

		[Header("조합 설정")]
		[Space()]

		[SerializeField] private bool m_isCraftingMaterial;

		[SerializeField] private List<InteractableSO> m_isCraftingMaterialOf;
		[SerializeField] private List<InteractableSO> m_isCraftedBy;

		[Header("아이템 설명")]
		[Space()]

		[SerializeField] private string m_displayNameKey;

		[SerializeField] private string m_descriptionKey;

		public bool IsCollectible => m_isCollectible;
		public bool IsActivatable => m_isActivatable;
		public InteractableModel InteractableModelPrefab => m_interactableModelPrefab;
	}
}