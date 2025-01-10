using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

namespace BM.Interactables
{
	/// <summary>
	/// Locked <=(Activate)=> Locked Notifying
	/// Locked =(Use DarackKey)=> Openable
	/// Openable =(Activate)=> Opening => Opened
	/// </summary>
	[RequireComponent(typeof(Animator))]
	public class GirlsChest : InteractableBase
	{
		[Tooltip("상자가 잠겼음을 나타낼 때 효과가 적용될 메쉬 그룹과 프레스넬 효과 정보")]
		[SerializeField] private FresnelEffectGroup m_effectGroupOnLockedNotifying;

		[Tooltip("상자가 열리는 중임을 나타낼 때 효과가 적용될 메쉬 그룹과 프레스넬 효과 정보")]
		[SerializeField] private FresnelEffectGroup m_effectGroupOnUnlocking;

		[Tooltip("상자가 상호작용 가능한(열 수 있는) 상태일 때 프레스넬 효과 정보")]
		[SerializeField] private FresnelEffectSO m_effectDataOnOpenable;


		[Header("사운드 효과")]

		[SerializeField] private EventReference m_lockedSound;
		[SerializeField] private EventReference m_unlockingSound;
		[SerializeField] private EventReference m_openingSound;

		[Space]

		[SerializeField] private VoiceLineSO m_voiceOnLocked;

		private Animator m_animator;
		private bool m_isLocked = true;
		private bool m_isOpened = false;

		public void GirlsChest_LockedNotifyingAnimationFinished()
		{
			m_enableHoveringEffect = true;

			m_effectGroupOnLockedNotifying.Disable();

			if (IsHovering)
			{
				m_currentHoveringGroup.EnableEffect();
			}

			AllowInteraction();
		}

		public void GirlsChest_UnlockingAnimationFinished()
		{
			m_enableHoveringEffect = true;
			m_currentHoveringGroup.ChangeEffect(m_effectDataOnOpenable);

			m_effectGroupOnUnlocking.Disable();

			AllowInteraction();
		}

		public override void StartInteract(InteractAction _)
		{
			base.StartInteract(_);

			if (m_isLocked)
			{
				m_animator.SetTrigger("Locked");

				// 지금부터 Locked Notifying 애니메이션이 끝날 때까지 상호작용 불허

				m_enableHoveringEffect = false;
				m_currentHoveringGroup.DisableEffect();
				DisallowInteraction();

				m_effectGroupOnLockedNotifying.Enable();

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

					m_enableHoveringEffect = false;
					m_currentHoveringGroup.DisableEffect();
					DisallowInteraction();


					if (!m_openingSound.IsNull)
					{
						RuntimeManager.PlayOneShot(m_openingSound);
					}

					m_isOpened = true;
				}
			}
		}

		public override void StartUse(UseAction _0, ItemSO _1)
		{
			base.StartUse(_0, _1);

			// GirlsChest의 Use로직은 잠금을 해제하는 것이고,
			// 잠금이 이미 해제되었다면 더 이상 Use 로직을 진행할 필요가 없음.

			if (!m_isLocked)
			{
				return;
			}

			// 애니메이션 이벤트로, 애니메이션이 끝날 때까지 상호작용 불허

			m_animator.SetTrigger("Unlock");

			m_enableHoveringEffect = false;
			m_currentHoveringGroup.DisableEffect();
			DisallowInteraction();

			m_effectGroupOnUnlocking.Enable();

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

			m_effectGroupOnLockedNotifying.Initialize();
			m_effectGroupOnUnlocking.Initialize();
		}
	}
}