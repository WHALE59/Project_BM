using FMODUnity;
using UnityEngine;

namespace BM.Interactables
{
	[RequireComponent(typeof(Animator))]
	public class GirlsChest : InteractableBase
	{
		[Header("Sound Effects")]
		[Space]

		[SerializeField] private EventReference m_lockedSound;
		[SerializeField] private EventReference m_unlockingSound;
		[SerializeField] private EventReference m_openingSound;

		[Space]

		[SerializeField] private VoiceLineSO m_voiceOnLocked;

		private Animator m_animator;
		private bool m_isLocked = true;
		private bool m_isOpened = false;

		public override void StartActivation(ActivateAction _)
		{
			base.StartActivation(_);

			if (m_isLocked)
			{
				// 애니메이션 이벤트로, 애니메이션이 끝날 때까지 상호작용 불허
				m_animator.SetTrigger("Locked");

				if (!m_lockedSound.IsNull)
				{
					RuntimeManager.PlayOneShot(m_lockedSound);
				}

				m_voiceOnLocked.Play();
			}
			else
			{
				// 열려있지 않았을 때에만 열리는 로직을 수행

				if (!m_isOpened)
				{
					// 애니메이션 이벤트로, 애니메이션이 끝날 때까지 상호작용 불허
					m_animator.SetTrigger("Open");

					if (!m_openingSound.IsNull)
					{
						RuntimeManager.PlayOneShot(m_openingSound);
					}

					m_isOpened = true;
				}
			}
		}

		public override void StartUsage(UseAction _0, InteractableSO _1)
		{
			base.StartUsage(_0, _1);

			// GirlsChest의 Use로직은 잠금을 해제하는 것이고,
			// 잠금이 이미 해제되었다면 더 이상 Use 로직을 진행할 필요가 없음.

			if (!m_isLocked)
			{
				return;
			}

			// 애니메이션 이벤트로, 애니메이션이 끝날 때까지 상호작용 불허
			m_animator.SetTrigger("Unlock");

			if (!m_unlockingSound.IsNull)
			{
				RuntimeManager.PlayOneShot(m_unlockingSound);
			}

			m_isLocked = false;
		}

		protected override void Awake()
		{
			m_animator = GetComponent<Animator>();
		}
	}
}