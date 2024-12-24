using FMODUnity;
using UnityEngine;

namespace BM.Interactables
{
	[CreateAssetMenu(menuName = "BM/SO/Activation Events/Cardboard Pile", fileName = "ActivationEventSO_CardboardPile")]
	public class ActivationEventSO_CardboardPile : ActivationEventSO
	{
		[SerializeField] private EventReference m_movingSound;

		public override void StartActivation(InteractAction _, InteractableBase sceneInteractable)
		{
			base.StartActivation(_, sceneInteractable);

			sceneInteractable.DisallowInteraction();

			if (sceneInteractable.TryGetComponent<Animator>(out Animator animator))
			{
				animator.SetTrigger("Move");

				RuntimeManager.PlayOneShot(m_movingSound);
			}
		}

		public override void FinishActivation(InteractAction _0, InteractableBase _1)
		{
			base.FinishActivation(_0, _1);
		}
	}
}