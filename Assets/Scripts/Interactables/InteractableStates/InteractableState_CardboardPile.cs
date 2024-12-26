using FMODUnity;
using UnityEngine;

namespace BM.Interactables
{
	public class InteractableState_CardboardPile : InteractableStateBase
	{
		[Tooltip("종이 상자 더미가 이동할 때 바닥과 마찰하며 내는 소리를 표현하를 FMOD 이벤트")]
		[SerializeField] private EventReference m_movingSound;

		[SerializeField] private Animator m_animator;

		public override void StartActivate(InteractAction _, InteractableBase interactionObject)
		{
			base.StartActivate(_, interactionObject);

			// 이동 애니메이션을 재생

			if (null != m_animator)
			{
				m_animator.SetTrigger("Move");
			}

			// 이동 사운드를 재생

			if (!m_movingSound.IsNull)
			{
				RuntimeManager.PlayOneShot(m_movingSound);
			}
		}
	}
}