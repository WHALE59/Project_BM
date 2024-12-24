#pragma warning disable CS0414

using UnityEngine;

namespace BM.Interactables
{
	[DisallowMultipleComponent]
	public class InteractableBase : MonoBehaviour
	{
		[SerializeField] private InteractableSO m_interactableSO;
		[SerializeField] private InteractableModel m_interactableModel;

		[SerializeField][HideInInspector] private bool m_isCollected = false;

		private bool m_allowInteraction = true;

		public bool IsInteractionAllowed => m_allowInteraction;

		public InteractableSO InteractableSO => m_interactableSO;

		public void DisallowInteraction()
		{
			m_allowInteraction = false;
		}

		public void StartHovering()
		{
			if (null != m_interactableModel)
			{
				m_interactableModel.StartHoveringEffect();
			}
		}

		public void FinishHovering()
		{
			if (null != m_interactableModel)
			{
				m_interactableModel.FinishHoveringEffect();
			}
		}

		public void SetCollected()
		{
			m_isCollected = true;

			m_interactableModel.gameObject.SetActive(false);
		}

		public void StartActivation(InteractAction interactAction)
		{
			m_interactableSO.StartActivationEvent(interactAction, this);
		}

		public void FinishActivation(InteractAction interactAction)
		{
			m_interactableSO.FinishActivationEvent(interactAction, this);
		}
	}
}