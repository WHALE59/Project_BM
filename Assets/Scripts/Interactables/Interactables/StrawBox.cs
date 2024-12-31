using FMODUnity;
using UnityEngine;

namespace BM.Interactables
{
	public class StrawBox : InteractableBase
	{
		[Header("Object Settings")]

		[Tooltip("Collect 된 경우 비활성화 할 Straw 메쉬들의 루트 오브젝트")]
		[SerializeField] GameObject m_strawParent;

		[Header("Sound Effects")]

		[SerializeField] EventReference m_soundOnCollectingStraw;

		[Header("Collectible Settings")]

		[SerializeField] private ItemSO m_strawItemSO;

		private bool m_isStrawCollected = false;

		public override void StartInteraction(InteractAction interactor)
		{
			base.StartInteraction(interactor);

			if (m_isStrawCollected)
			{
				return;
			}

			if (!m_soundOnCollectingStraw.IsNull)
			{
				RuntimeManager.PlayOneShot(m_soundOnCollectingStraw);
			}

			m_strawParent.SetActive(false);

			m_isStrawCollected = true;

			DisallowInteraction();
			interactor.CollectItem(m_strawItemSO);
		}
	}
}