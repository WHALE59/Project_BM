using FMODUnity;
using UnityEngine;

namespace BM.Interactables
{
	[RequireComponent(typeof(Animator))]
	public class Interactable_DarackKey : InteractableBase
	{
		private bool m_isReachable = false;

		private Animator m_animator;
		public override void StartActivation(InteractAction _)
		{
			base.StartActivation(_);

			if (!m_isReachable)
			{
				return;
			}
		}

		public override void StartUsage(InteractAction _0, InteractableSO _1)
		{
			base.StartUsage(_0, _1);

			m_isReachable = true;
			m_animator.SetTrigger("Move");
		}

		protected override void Awake()
		{
			base.Awake();
			m_animator = GetComponent<Animator>();
		}
	}
}