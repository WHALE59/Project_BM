using System;

using UnityEngine;

namespace BM.Interactables
{
	[CreateAssetMenu(menuName = "BM/SO/Interactable SO", fileName = "InteractableSO_Default")]
	public class InteractableSO : ScriptableObject
	{
		[Tooltip("게임 플레이 중 플레이어에게 표시될 이 아이템의 이름")]
		[SerializeField] private string m_displayName;

		[Tooltip("게임 플레이 중 플레이어에게 표시될 이 아이템의 설명")]
		[SerializeField] private string m_description;

		[Tooltip("이 아이템이 형체를 가져야 한다면, 그 형체 정보를 담고 있는 프리팹")]
		[SerializeField] private InteractableModel m_interactableModelPrefab;

		private Guid m_guid;
	}
}