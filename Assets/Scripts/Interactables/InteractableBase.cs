using System;
using UnityEngine;

namespace BM.Interactables
{
	[Serializable]
	public enum InteractionType
	{
		Collectible,
		Activatable
	}

	[DisallowMultipleComponent]
	public class InteractableBase : MonoBehaviour
	{
		[SerializeField] private InteractionType m_interactionType = InteractionType.Collectible;
		[SerializeField] private InteractableSO m_interactableSO;
		[SerializeField] private InteractableModel m_interactableModel;

		public InteractionType Type => m_interactionType;
		private bool m_isCollected = false;

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
	}
}