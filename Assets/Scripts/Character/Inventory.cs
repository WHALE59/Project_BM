using BM.Interactables;
using System.Collections.Generic;

using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	public class Inventory : MonoBehaviour
	{
		[SerializeField] private List<InteractableSO> m_inventory = new();

		public void PutIn(InteractableSO interactable)
		{
			m_inventory.Add(interactable);
		}
	}
}