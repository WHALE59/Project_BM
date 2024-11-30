using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace BM.StateSystem
{
	public abstract class ConditionSO : ScriptableObject
	{
		public abstract bool Verify(StateComponent stateComponent);
	}
}