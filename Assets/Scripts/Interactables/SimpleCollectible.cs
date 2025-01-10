using FMODUnity;
using UnityEngine;

namespace BM.Interactables
{
	/// <summary>
	/// 상호작용의 내용이 인벤토리에 추가되는 것외에는 기능이 없는 경우 사용
	/// </summary>
	public class SimpleCollectible : InteractableBase
	{
		[Header("인벤토리에 넣을 아이템 정보")]

		[SerializeField] private ItemSO m_itemToCollect;

		[Header("사운드 설정")]

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