using BM.Interactables;
using System.Collections.Generic;
using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	public class Inventory : MonoBehaviour
	{
		[SerializeField] private List<InteractableSO> m_inventory = new();

		public void PutIn(InteractableBase collectible)
		{
			m_inventory.Add(collectible.InteractableSO);
		}
	}
}