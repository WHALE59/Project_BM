using System.Collections.Generic;

using UnityEngine;

namespace BM.Interactables
{
	public interface IEquippable
	{
		Vector3 EquipmentPosition { get; set; }
		Quaternion EquipmentRotation { get; set; }
		Vector3 SocketPosition { get; }
		Quaternion SocketRotation { get; }
		PlacementGhost Ghost { get; }

		void EnableGhost();
		void DisableGhost();


		IReadOnlyCollection<Collider> Colliders { get; }

		void StartEquipped(InteractAction subject);
		void FinishEqipped(InteractAction subject);
	}
}