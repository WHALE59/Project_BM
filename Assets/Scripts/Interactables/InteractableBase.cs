using UnityEngine;

namespace BM.Interactables
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	public abstract class InteractableBase : MonoBehaviour, IInteractable
	{
		private Rigidbody m_rigidbody;

		public void StartHovering()
		{
			// Start Hovering Effect
		}

		public void FinishHovering()
		{
			// Finish Hovering Effect 
		}

		private void Awake()
		{
			m_rigidbody = GetComponent<Rigidbody>();
			m_rigidbody.isKinematic = true;
		}
	}
}