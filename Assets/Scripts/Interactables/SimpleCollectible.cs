using FMODUnity;
using UnityEngine;

namespace BM.Interactables
{
	public class SimpleCollectible : InteractableBase
	{
		[Header("Collectible Info")]
		[SerializeField] private ItemSO m_itemToCollect;

		[Header("Sound Settings")]

		[SerializeField] private EventReference m_soundOnCollect;


		public override void StartInteract(InteractAction collector)
		{
			base.StartInteract(collector);

			collector.CollectItem(m_itemToCollect);
			m_rootAppearance.SetActive(false);

			if (!m_soundOnCollect.IsNull)
			{
				RuntimeManager.PlayOneShot(m_soundOnCollect);
			}
		}
	}
}