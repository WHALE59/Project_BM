using UnityEngine;

namespace BM.Interactables
{

	public abstract class ActivationEventSO : ScriptableObject
	{

#if UNITY_EDITOR
		[SerializeField] private bool m_logOnActivation = false;
#endif

		public virtual void StartActivation(InteractAction subject, InteractableBase sceneInteractable)
		{
#if UNITY_EDITOR
			if (m_logOnActivation)
			{
				Debug.Log($"{subject.name}의 {sceneInteractable.name} 활성화 이벤트 시작.");
			}
#endif
		}

		public virtual void FinishActivation(InteractAction subject, InteractableBase sceneInteractable)
		{
#if UNITY_EDITOR
			if (m_logOnActivation)
			{
				Debug.Log($"{subject.name}의 {sceneInteractable.name} 활성화 이벤트 종료.");
			}
#endif
		}
	}
}