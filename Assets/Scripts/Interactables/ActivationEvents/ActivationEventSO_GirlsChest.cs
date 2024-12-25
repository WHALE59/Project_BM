using BM.Interactables;
using FMODUnity;

using UnityEngine;

namespace BM.Interactable
{
	[CreateAssetMenu(menuName = "BM/SO/Activation Events/Girl's Chest", fileName = "ActivationEventSO_GirlsChest")]
	public class ActivationEventSO_GirlsChest : ActivationEventSO
	{
		[SerializeField] private EventReference m_lockedSound;

		private bool m_isLocked = true;

		public override void StartActivation(InteractAction _, InteractableBase sceneInteractable)
		{
			base.StartActivation(_, sceneInteractable);

			if (sceneInteractable.InteractableModel.TryGetComponent<Animator>(out Animator animator))
			{
				if (m_isLocked)
				{
					animator.SetTrigger("Locked");
					RuntimeManager.PlayOneShot(m_lockedSound);
				}
				else if (!m_isLocked)
				{
				}
			}
		}
	}
}