using System;
using UnityEngine;

namespace BM.StateSystem
{
	public class StateComponent : MonoBehaviour
	{
		[SerializeField] private StateMachineSO m_stateMachine;

		private StateSO m_currentState;

		public StateSO CurrentSTate => m_currentState;

		/// <summary>
		/// T1: 이전 상태, T2: 천이한 상태
		/// </summary>
		public event Action<StateSO, StateSO> StateChanged;

		public void CheckTransitions()
		{
			StateSO nextState = m_stateMachine.CheckTransitions(this, m_currentState);

			if (nextState == m_stateMachine.EmptyState)
			{
				return;
			}

			m_currentState.End(this);
			StateSO previousState = m_currentState;

			m_currentState = nextState;
			m_currentState.Begin(this);

			StateChanged?.Invoke(previousState, nextState);
		}

		private void Start()
		{
			if (null == m_stateMachine.InitialState)
			{
				Debug.LogError("널이지롱~", this);
				return;
			}

			m_currentState = m_stateMachine.InitialState;
			m_currentState.Begin(this);
		}

		private void FixedUpdate()
		{
			if (null == m_currentState)
			{
				return;
			}

			m_currentState.UpdatePhysics(this);
		}

		private void Update()
		{
			if (null == m_currentState)
			{
				return;
			}

			m_currentState.UpdateState(this);
		}

		private void LateUpdate()
		{
			if (null == m_currentState)
			{
				return;
			}

			CheckTransitions();
		}

	}
}