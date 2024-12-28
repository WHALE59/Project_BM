using FMODUnity;
using UnityEngine;

namespace BM.Interactables
{
	[RequireComponent(typeof(Animator))]
	public class Interactable_CardboardPile : InteractableBase
	{
		[Header("Sound Settings")]
		[Space]

		[SerializeField] private EventReference m_soundOnMoving;

		private Animator m_animator;

		public override void StartActivation(InteractAction _)
		{
			base.StartActivation(_);

			m_animator.SetTrigger("Move");

			if (!m_soundOnMoving.IsNull)
			{
				RuntimeManager.PlayOneShot(m_soundOnMoving);
			}
		}

		protected override void Awake()
		{
			m_animator = GetComponent<Animator>();
		}
	}
}