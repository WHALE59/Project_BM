using System;

namespace BM.StateSystem
{
	[Serializable]
	public struct Transition
	{
		public StateSO m_originState;
		public ConditionSO m_condition;
		public StateSO m_stateOnTrue;
		public StateSO m_stateOnFalse;
	}
}