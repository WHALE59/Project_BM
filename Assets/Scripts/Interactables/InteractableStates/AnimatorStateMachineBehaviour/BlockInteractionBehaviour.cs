using BM.Interactables;
using UnityEngine;

namespace BM
{
	public class BlockInteractionBehaviour : StateMachineBehaviour
	{

#if UNITY_EDITOR
		[SerializeField] private bool m_logOnStateTransition = false;
#endif

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (animator.TryGetComponent<InteractableModel>(out var model))
			{
				if (null != model)
				{
					model.InteractableBase.DisallowInteraction();

#if UNITY_EDITOR
					if (m_logOnStateTransition)
					{
						Debug.Log($"{model.name}의 애니메이션 상태 {stateInfo.shortNameHash}에 진입하여 상호작용이 비활성화 되었습니다.");
					}
#endif
				}
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (animator.TryGetComponent<InteractableModel>(out var model))
			{
				if (null != model)
				{
					model.InteractableBase.AllowInteraction();

#if UNITY_EDITOR
					if (m_logOnStateTransition)
					{
						Debug.Log($"{model.name}의 애니메이션 상태 {stateInfo.shortNameHash}에서 탈출하여 상호작용이 활성화 되었습니다.");
					}
#endif
				}
			}
		}
	}
}
