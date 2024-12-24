
using UnityEngine;

namespace BM.Interactables
{
	[CreateAssetMenu(menuName = "BM/SO/Activation Events/CardboardPile", fileName = "ActivationEventSO_CardboardPile")]
	public class ActivationEventSO_CardboardPile : ActivationEventSO
	{
		public override void StartActivation(InteractAction _, InteractableBase sceneInteractable)
		{
			base.StartActivation(_, sceneInteractable);

			sceneInteractable.DisallowInteraction();

			if (sceneInteractable.TryGetComponent<Animator>(out Animator animator))
			{
				animator.SetTrigger("Move");
			}
		}

		public override void FinishActivation(InteractAction _0, InteractableBase _1)
		{
			base.FinishActivation(_0, _1);
		}
	}
}