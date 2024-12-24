using Codice.Client.BaseCommands;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

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
		[SerializeField] private EventReference m_collectingSoundEventReference;

		[Space()]

		[SerializeField] private bool m_isActivatable;
		[SerializeField] private Sprite m_activateCrosshairIcon;
		[SerializeField] private ActivationEventSO m_eventOnActivation;

		[Space()]

		[SerializeField] private bool m_isUsableAndEquippable;

		[SerializeField] private Sprite m_equipmentIcon;
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
		public bool IsUsedable => m_isUsedable;

		public Sprite EquipmentIcon => m_equipmentIcon;

		public void StartActivationEvent(InteractAction subject, InteractableBase sceneInteractable)
		{
			if (null == m_eventOnActivation)
			{
#if UNITY_EDITOR
				Debug.Log($"{name}에 활성화 이벤트가 없습니다.");
#endif
				return;
			}

			m_eventOnActivation.StartActivation(subject, sceneInteractable);
		}
		public void FinishActivationEvent(InteractAction subject, InteractableBase sceneInteractable)
		{
			if (null == m_eventOnActivation)
			{
#if UNITY_EDITOR
				Debug.Log($"{name}에 활성화 이벤트가 없습니다.");
#endif
				return;
			}

			m_eventOnActivation.FinishActivation(subject, sceneInteractable);
		}

		public LocalizedString LocalizedDisplayName => m_displayName;

		public void PlayOneShotCollectingSound()
		{
			RuntimeManager.PlayOneShot(m_collectingSoundEventReference);
		}
	}
}