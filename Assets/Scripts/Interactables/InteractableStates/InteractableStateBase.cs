using UnityEngine;

namespace BM.Interactables
{
	/// <summary>
	/// 상호작용 오브젝트가 상호작용에 의해 변경되는 상태를 관리 <br/>
	/// 이 클래스를 상속하여 구체적인 상호작용 이벤트를 구현하도록 한다.
	/// </summary>
	[DisallowMultipleComponent]
	public class InteractableStateBase : MonoBehaviour
	{

#if UNITY_EDITOR
		[SerializeField] private bool m_logOnInteraction = false;
#endif

		// Activation

		public virtual void StartActivate(InteractAction interactionSubject, InteractableBase interactionObject)
		{
#if UNITY_EDITOR
			if (m_logOnInteraction)
			{
				Debug.Log($"{name} 활성화 시작");
			}
#endif
		}

		public virtual void FinishActivate(InteractAction interactionSubject, InteractableBase interactionObject)
		{
#if UNITY_EDITOR
			if (m_logOnInteraction)
			{
				Debug.Log($"{name} 활성화 종료");
			}
#endif
		}

		protected virtual void Awake() { }

		protected virtual void Start() { }
	}
}