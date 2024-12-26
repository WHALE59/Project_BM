#pragma warning disable CS0414

using FMODUnity;
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

		private InteractableStateBase m_state;

		public bool IsInteractionAllowed => m_allowInteraction;

		public InteractableSO InteractableSO => m_interactableSO;

		public InteractableModel Model { get => m_interactableModel; set => m_interactableModel = value; }

		public bool IsCollectible => m_interactableSO.IsCollectible;
		public bool IsActivatable => m_interactableSO.IsActivatable;


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

			RuntimeManager.PlayOneShot(m_interactableSO.CollectingSound);

			m_interactableModel.gameObject.SetActive(false);
		}

		public void StartActivation(InteractAction interactAction)
		{
			if (null != m_state)
			{
				m_state.StartActivate(interactAction, this);
			}
		}

		public void FinishActivation(InteractAction interactAction)
		{
			if (null != m_state)
			{
				m_state.FinishActivate(interactAction, this);
			}
		}

		private void Awake()
		{
			m_state = GetComponent<InteractableStateBase>();
		}
	}
}