#pragma warning disable CS0414

using UnityEngine;

namespace BM.Interactables
{
	[DisallowMultipleComponent]
	public class InteractableBase : MonoBehaviour
	{
		[SerializeField] private InteractableSO m_interactableSO;
		[SerializeField] private InteractableModel m_interactableModel;

		public InteractableSO InteractableSO => m_interactableSO;

		[SerializeField][HideInInspector] private bool m_isCollected = false;

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
	}
}