using UnityEngine;

namespace BM.InteractableObjects
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	public abstract class InteractableBase : MonoBehaviour, IInteractable
	{
		protected Rigidbody m_rigidbody;

		public void StartHovering()
		{
			// Start Hovering Effect
		}

		public void FinishHovering()
		{
			// Finish Hovering Effect 
		}

		protected virtual void Awake()
		{
			m_rigidbody = GetComponent<Rigidbody>();
			m_rigidbody.isKinematic = true;
		}
	}
}