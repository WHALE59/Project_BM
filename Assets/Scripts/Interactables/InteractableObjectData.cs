using UnityEngine;

namespace BM
{
	[CreateAssetMenu(menuName = "BM/Data/Interactable Object Data", fileName = "InteractableObjectData_Name")]
	public class InteractableObjectData : ScriptableObject
	{
		public string displayName = "표시 이름";
		public string interactionName = "상호작용 한다";
	}
}
