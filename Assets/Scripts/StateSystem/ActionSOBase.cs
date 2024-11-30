using UnityEngine;

namespace BM.StateSystem
{
	public abstract class ActionSOBase : ScriptableObject
	{
		public abstract void Act(StateComponent stateComponent);
	}
}