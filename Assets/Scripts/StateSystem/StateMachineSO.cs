using System.Collections.Generic;

using UnityEngine;

namespace BM.StateSystem
{
	[CreateAssetMenu(menuName = "BM/State System/State Machine", fileName = "StateMachineSO_Default")]
	public class StateMachineSO : ScriptableObject
	{
		[SerializeField] private StateSO m_initialState;
		[SerializeField] private StateSO m_emptyState;
		[SerializeField] private List<Transition> m_transitions;

		public StateSO InitialState => m_initialState;
		public StateSO EmptyState => m_emptyState;

		/// <summary>
		/// <paramref name="currentState"/>에서 가능한 천이를 <see cref="m_transitions"/>에서 탐색하여, <paramref name="stateComponent"/>의 맥락에 따라 조건을 검사하여 천이가 가능하다면 천이한다.
		/// </summary>
		public StateSO CheckTransitions(StateComponent stateComponent, StateSO currentState)
		{
			foreach (Transition transition in m_transitions)
			{
				// 천이가 현재 상태 머신의 상태에서 출발하지 않으면 무시
				if (transition.m_originState != currentState)
				{
					continue;
				}

				// 천이가 조건을 아예 가지고 있지 않으면 무시
				if (null == transition.m_condition)
				{
					continue;
				}

				// 천이 조건이 참일 때
				if (transition.m_condition.Verify(stateComponent))
				{
					if (transition.m_stateOnTrue == m_emptyState)
					{
						return m_emptyState;
					}

					// 조건 만족 시킬 때, 전이할 상태가 존재하는가?
					if (null != transition.m_stateOnTrue)
					{
						// 존재 한다면 해당 상태를 반환
						return transition.m_stateOnTrue;
					}
					else
					{
						Debug.LogError($"[BM.StateSystem] {name}의 천이 목록에 할당되지 않은 상태가 존재합니다.", this);
					}
				}
				// 천이 조건이 거짓일 때
				else
				{
					if (transition.m_stateOnFalse == m_emptyState)
					{
						return m_emptyState;
					}

					if (null != transition.m_stateOnFalse)
					{
						return transition.m_stateOnFalse;
					}
					else
					{
						Debug.LogError($"[BM.StateSystem] {name}의 천이 목록에 할당되지 않은 상태가 존재합니다.", this);
					}
				}
			}

			return m_emptyState;
		}
	}
}
