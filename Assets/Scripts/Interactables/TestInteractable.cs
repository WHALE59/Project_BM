
using System.Collections.Generic;
using UnityEngine;

namespace BM.InteractableObjects
{
	[RequireComponent(typeof(Rigidbody))]
	public class TestInteractable : InteractableBase, ICollectible, IActivatable, IUsable, IUsedable<TestInteractable>
	{
		[SerializeField] private Transform m_socketOnEquipped;
		[SerializeField] private PlacementGhost m_placementGhostPrefab;

		private Collider[] m_colliders;

		private PlacementGhost m_placementGhostInstance;

		Vector3 IEquippable.EquipmentPosition { get => m_rigidbody.position; set => m_rigidbody.MovePosition(value); }

		Quaternion IEquippable.EquipmentRotation { get => m_rigidbody.rotation; set => m_rigidbody.MoveRotation(value); }

		Vector3 IEquippable.SocketPosition => m_socketOnEquipped.position;

		Quaternion IEquippable.SocketRotation => m_socketOnEquipped.rotation;

		IReadOnlyCollection<Collider> IEquippable.Colliders => m_colliders;

		PlacementGhost IEquippable.Ghost => m_placementGhostInstance;

		void ICollectible.Enable()
		{
			gameObject.SetActive(true);
		}
		void ICollectible.Disable()
		{
			gameObject.SetActive(false);
		}

		void IActivatable.StartActivate() { }

		void IActivatable.EndActivate() { }

		void IEquippable.FinishEquipped(InteractAction subject) { }


		void IEquippable.StartEquipped(InteractAction subject)
		{
		}

		void IEquippable.EnableGhost()
		{
			m_placementGhostInstance.gameObject.SetActive(true);
		}

		void IEquippable.DisableGhost()
		{
			m_placementGhostInstance.gameObject.SetActive(false);
		}

		protected override void Awake()
		{
			base.Awake();

			m_colliders = GetComponentsInChildren<Collider>();

			m_placementGhostInstance = Instantiate(m_placementGhostPrefab, parent: transform);
			((IEquippable)this).DisableGhost();
		}
	}
}