using FMODUnity;
using UnityEngine;

namespace BM.Interactables
{
	public class InteractableState_GirlsChest : InteractableStateBase
	{
		[Tooltip("상자가 잠겼다는 애니메이션과 함께 재생될 자물쇠가 달그락 거리는 사운드")]
		[SerializeField] private EventReference m_lockedSound;

		[SerializeField] private Animator m_animator;

		private bool m_isLocked = true;

		public override void StartActivate(InteractAction _, InteractableBase interactionObject)
		{
			base.StartActivate(_, interactionObject);

			if (m_isLocked)
			{
				if (null != m_animator)
				{
					m_animator.SetTrigger("Locked");
				}

				if (!m_lockedSound.IsNull)
				{
					RuntimeManager.PlayOneShot(m_lockedSound);
				}
			}
			else
			{
				// TODO 열리는 로직
			}
		}
	}
}