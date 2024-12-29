#pragma warning disable CS0414

using UnityEngine;

namespace BM.Interactables
{
	[DisallowMultipleComponent]
	public class InteractableBase : MonoBehaviour
	{
		[SerializeField] private InteractableSO m_interactableSO;
		[SerializeField] private InteractableModel m_interactableModel;

		[SerializeField][HideInInspector] private bool m_isCollected = false;

#if UNITY_EDITOR
		[Space]

		[SerializeField] private bool m_logOnHovering = false;
		[SerializeField] private bool m_logOnInteraction = false;

		[Space]
#endif

		private bool m_allowDetection = true;
		protected bool m_allowInteraction = true;

		/// <summary>
		/// <see cref="InteractAction"/>에 의해 감지 자체가 허용 되는지의 여부
		/// </summary>
		public bool IsDetectionAllowed => m_allowDetection;

		/// <summary>
		/// <see cref="InteractAction"/>에 의해 Use 혹은 Activate가 허용 되는지의 여부
		/// </summary>
		/// <remarks>
		/// <c>false</c>를 반환하는 경우, <see cref="InteractAction"/>에 의해 감지는 되지만, 아무런 상호작용을 할 수 없다는 뜻이다. <br/>
		/// </remarks>
		public bool IsInteractionAllowed => m_allowInteraction;

		public InteractableSO InteractableSO => m_interactableSO;

		public InteractableModel Model { get => m_interactableModel; protected set => m_interactableModel = value; }

		public bool IsCollectible => m_interactableSO.IsCollectible;

		public bool IsActivatable => m_interactableSO.IsActivatable;

		public bool IsUsable => m_interactableSO.IsUsable;

		public bool IsUsedable => m_interactableSO.IsUsedable;

		public void AllowInteraction()
		{
			m_allowDetection = true;
		}

		public void DisallowInteraction()
		{
			m_allowDetection = false;
		}

		public void StartHovering()
		{
#if UNITY_EDITOR
			if (m_logOnHovering)
			{
				Debug.Log($"{name}의 호버링 시작");
			}
#endif

			if (null != m_interactableModel)
			{
				m_interactableModel.StartHoveringEffect();
			}
		}

		public void FinishHovering()
		{
#if UNITY_EDITOR
			if (m_logOnHovering)
			{
				Debug.Log($"{name}의 호버링 종료");

			}
#endif

			if (null != m_interactableModel)
			{
				m_interactableModel.FinishHoveringEffect();
			}
		}

		public void SetCollected()
		{
			m_isCollected = true;

			m_interactableModel.gameObject.SetActive(false);
		}

		public virtual void StartActivation(InteractAction interactionSubject)
		{
#if UNITY_EDITOR
			if (m_logOnInteraction)
			{
				Debug.Log($"주체 {interactionSubject}에 의하여 {name} Activate 시작");
			}
#endif
		}

		public virtual void FinishActivation(InteractAction interactionSubject)
		{
#if UNITY_EDITOR
			if (m_logOnInteraction)
			{
				Debug.Log($"주체 {interactionSubject}에 의하여 {name} Activate 종료");
			}
#endif
		}

		public virtual void StartUsage(UseAction user, InteractableSO equipment)
		{
#if UNITY_EDITOR
			if (m_logOnInteraction)
			{
				Debug.Log($"주체 {user.name}의 장비 {equipment.name}에 의하여 {name} Use 시작");
			}
#endif
		}

		public virtual void FinishUsage(InteractAction interactionSubject, InteractableSO equipment)
		{
#if UNITY_EDITOR
			if (m_logOnInteraction)
			{
				Debug.Log($"주체 {interactionSubject.name}의 장비 {equipment.name} 에 의하여 {name} Use 종료");
			}
#endif
		}

		protected virtual void Awake() { }
		protected virtual void Start() { }
	}
}
