using FMODUnity;
using UnityEngine;

namespace BM.Interactables
{
	[RequireComponent(typeof(Animator))]
	public class CardboardPile : InteractableBase
	{
		[Header("Sound Settings")]

		[SerializeField] private EventReference m_soundOnMoving;

		private Animator m_animator;
		private bool m_isMoved = false;

		/// <summary>
		/// 지정된 루트 모션 애니메이션이 재생되어 위치가 이동한다. 한 번 활성화 하였으면, 더이상 활성화 되지 않는다.
		/// 또, 한번 상호작용 한 이후에는 다시 상호작용 하는 것을 금한다.
		/// </summary>
		public override void StartInteract(InteractAction _)
		{
			base.StartInteract(_);

			if (m_isMoved)
			{
				return;
			}

			// 이동 애니메이션 시작 (애니메이션 이벤트로 InteractableBase.DisallowInteraction이 호출된다.)
			m_animator.SetTrigger("Move");

			if (!m_soundOnMoving.IsNull)
			{
				RuntimeManager.PlayOneShot(m_soundOnMoving);
			}

			DisallowInteraction();

			m_isMoved = true;
		}

		protected override void Awake()
		{
			base.Awake();

			m_animator = GetComponent<Animator>();
		}
	}
}