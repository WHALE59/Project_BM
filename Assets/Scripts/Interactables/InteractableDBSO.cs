using System.Collections.Generic;

using UnityEngine;

namespace BM.Interactables
{
	[CreateAssetMenu(menuName = "BM/SO/Interactable DB SO", fileName = "InteractableDBSO_Default")]
	public class InteractableDBSO : ScriptableObject
	{
		[SerializeField] private List<InteractableSO> m_interactableScriptableObjects;
	}
}