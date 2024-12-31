using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

namespace BM.Interactables
{
	[RequireComponent(typeof(Animator))]
	public class GirlsChest : InteractableBase
	{
		[SerializeField] private FresnelEffectSO m_effectOnLockedNotify;
		[SerializeField] private FresnelEffectSO m_effectOnUnlocking;
		[SerializeField] private FresnelEffectSO m_effectOnCanOpen;

		[SerializeField] List<MeshRenderer> m_openableRenderers;

		[Header("Sound Effects")]

		[SerializeField] private EventReference m_lockedSound;
		[SerializeField] private EventReference m_unlockingSound;
		[SerializeField] private EventReference m_openingSound;

		[Space]

		[SerializeField] private VoiceLineSO m_voiceOnLocked;

		private Animator m_animator;
		private bool m_isLocked = true;
		private bool m_isOpened = false;

		public void GirlsChest_LockedAnimationStarted()
		{
			m_enableHoveringEffect = false;
			DisallowInteraction();
			EnableFresnelEffectOnMeshGroup(m_effectOnLockedNotify, m_hoveringRenderers);
		}

		public void GirlsChest_LockedAnimationFinished()
		{
			m_enableHoveringEffect = true;
			DisableFresnelEffectOnMeshGroup(m_hoveringRenderers);
			AllowInteraction();
		}

		public void GirlsChest_UnlockingAnimationStarted()
		{
			m_enableHoveringEffect = false;
			DisallowInteraction();

			EnableFresnelEffectOnMeshGroup(m_effectOnUnlocking, m_hoveringRenderers);
		}

		public void GirlsChest_UnlockingAnimationFinished()
		{
			m_enableHoveringEffect = true;

			DisableFresnelEffectOnMeshGroup(m_hoveringRenderers);

			m_hoveringRenderers = m_openableRenderers;
			m_fresnelEffectSO = m_effectOnCanOpen;

			TrySetHoveringRendererEffectData(m_fresnelEffectSO, m_hoveringRenderers);

			AllowInteraction();
		}

		public override void StartInteraction(InteractAction _)
		{
			base.StartInteraction(_);

			if (m_isLocked)
			{
				// 애니메이션 이벤트로, 애니메이션이 끝날 때까지 상호작용 불허
				m_animator.SetTrigger("Locked");

				GirlsChest_LockedAnimationStarted();

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

		public override void StartUsage(UseAction _0, ItemSO _1)
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
			base.Awake();

			m_animator = GetComponent<Animator>();
		}
	}
}