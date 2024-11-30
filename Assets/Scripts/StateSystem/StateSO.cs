using UnityEngine;

namespace BM.StateSystem
{
	[CreateAssetMenu(menuName = "BM/State System/StateSO", fileName = "StateSO_Default")]
	public class StateSO : ScriptableObject
	{
		[SerializeField] ActionSOBase[] m_entryActions;
		[SerializeField] ActionSOBase[] m_exitActions;
		[SerializeField] ActionSOBase[] m_physicsActions;
		[SerializeField] ActionSOBase[] m_stateActions;

		public void Begin(StateComponent stateComponent)
		{
			foreach (ActionSOBase action in m_entryActions)
			{
				// TODO: null 이면 그냥 넘기기
				if (null != action)
				{
					action.Act(stateComponent);
				}
				else
				{
					Debug.LogError($"[BM.StateSystem] {name}의 Entry Action 목록에 null 항목이 있습니다.", this);
				}
			}
		}

		public void End(StateComponent stateComponent)
		{
			foreach (ActionSOBase action in m_exitActions)
			{
				if (null != action)
				{
					action.Act(stateComponent);
				}
				else
				{
					Debug.LogError($"[BM.StateSystem] {name}의 Exit Action 목록에 null 항목이 있습니다.", this);
				}
			}
		}

		public void UpdatePhysics(StateComponent stateComponent)
		{
			foreach (ActionSOBase action in m_physicsActions)
			{
				if (null != action)
				{

					action.Act(stateComponent);
				}
				else
				{
					Debug.LogError($"[BM.StateSystem] {name}의 Physics Action 목록에 null 항목이 있습니다.", this);
				}
			}

		}

		public void UpdateState(StateComponent stateComponent)
		{
			foreach (ActionSOBase action in m_stateActions)
			{
				if (null != action)
				{
					action.Act(stateComponent);
				}
				else
				{
					Debug.LogError($"[BM.StateSystem] {name}의 State Action 목록에 null 항목이 있습니다.", this);
				}
			}
		}

	}
}