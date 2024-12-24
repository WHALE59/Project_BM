using UnityEngine;
using FMODUnity;
using BM.Interactables;

namespace BM.Interactable
{

	[CreateAssetMenu(menuName = "BM/SO/Activation Events/Darack Curtain", fileName = "ActivationEventSO_DarackCurtain")]
	public class ActivationEventSO_DarackCurtain : ActivationEventSO
	{
		[SerializeField] private InteractableModel m_openedModel;
		[SerializeField] private InteractableModel m_closedModel;

		[Space()]

		[SerializeField] private EventReference m_openingSound;
		[SerializeField] private EventReference m_closingSound;

		private bool m_isOpened = false;

		public override void StartActivation(InteractAction _, InteractableBase sceneInteractable)
		{
			base.StartActivation(_, sceneInteractable);

			if (!m_isOpened)
			{
				// Open Curtain

				m_isOpened = true;

				Destroy(sceneInteractable.InteractableModel.gameObject);
				sceneInteractable.InteractableModel = Instantiate(m_openedModel, sceneInteractable.transform);

			}
			else if (m_isOpened)
			{
				// Close Curtain

				m_isOpened = false;

				Destroy(sceneInteractable.InteractableModel.gameObject);
				sceneInteractable.InteractableModel = Instantiate(m_closedModel, sceneInteractable.transform);
			}

			sceneInteractable.InteractableModel.StartHoveringEffect();
		}

	}
}