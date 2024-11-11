using UnityEngine;

namespace BM.Objects
{
	[DisallowMultipleComponent]
	public abstract class BMObjectBase : MonoBehaviour, IRaycastable
	{
		public void FinishHovering() { }
		public void StartHovering() { }
	}
}