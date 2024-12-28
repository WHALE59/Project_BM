using FMODUnity;
using UnityEngine;

namespace BM.Interactables
{
	[RequireComponent(typeof(Animator))]
	public class CardboardPile : InteractableBase
	{
		[Header("Sound Settings")]
		[Space]

		[SerializeField] private EventReference m_soundOnMoving;

		private Animator m_animator;

		private bool m_isMoved = false;

		public override void StartActivation(InteractAction _)
		{
			base.StartActivation(_);

			if (m_isMoved)
			{
				return;
			}

			m_animator.SetTrigger("Move");

			if (!m_soundOnMoving.IsNull)
			{
				RuntimeManager.PlayOneShot(m_soundOnMoving);
			}

			m_isMoved = true;
		}

		protected override void Awake()
		{
			m_animator = GetComponent<Animator>();
		}
	}
}