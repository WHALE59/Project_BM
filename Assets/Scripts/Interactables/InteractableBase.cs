#pragma warning disable CS0414

using System;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace BM.Interactables
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	public class InteractableBase : MonoBehaviour
	{
		[Header("외형 오브젝트의 루트 설정")]

		[SerializeField] protected GameObject m_rootAppearance;

		[Header("기본 호버링 이펙트 설정")]

		[SerializeField] protected bool m_enableHoveringEffect = true;
		[Tooltip("호버링이 감지되는 영역 콜라이더와, 호버링이 시작되었을 때에 적용될 프레스넬 효과와 메쉬 그룹에 관한 데이터. 할당하지 않고 상속 쪽에서 처리할 수도 있음")]
		[SerializeField] protected HoveringEffectGroup m_defaultHoveringEffectGroup;

		protected HoveringEffectGroup m_currentHoveringGroup;

		[Header("Usedable 설정")]

		[SerializeField] private bool m_isUsedable = false;
		[SerializeField] private List<ItemSO> m_usedBy = new();

		[Header("UI 설정")]

		[SerializeField] private Sprite m_interactionCrosshair;

		[Tooltip("이 상호작용 오브젝트가 컨트롤 가이드 UI에 표시할 상호작용 행동의 이름을 나타내는 LocalizedString")]
		[SerializeField] private LocalizedString m_controlGuideAction;

		[Tooltip("이 상호작용 오브젝트가 컨트롤 가이드 UI에 표시될 때 그 표시 이름을 나타내는 LocalizedString")]
		[SerializeField] private LocalizedString m_controlGuideDisplayName;

#if UNITY_EDITOR
		[Header("Debug")]

		[SerializeField] private bool m_logOnHovering = false;
		[SerializeField] private bool m_logOnInteract = false;
		[SerializeField] private bool m_logOnUse = false;

		[Space]
#endif
		private bool m_allowDetection = true;
		protected bool m_allowInteraction = true;
		private bool m_isHovering = false;

		protected bool IsHovering => m_isHovering;

		public bool IsUsedable => m_isUsedable;
		public Sprite InteractionCrosshair => m_interactionCrosshair;

		/// <summary>
		/// <see cref="InteractableDetector"/>에 의해 감지 자체가 허용 되는지의 여부
		/// </summary>
		public bool IsDetectionAllowed => m_allowDetection;

		/// <summary>
		/// <see cref="InteractableDetector"/>에 의해 Use 혹은 Activate가 허용 되는지의 여부
		/// </summary>
		/// <remarks>
		/// <c>false</c>를 반환하는 경우, <see cref="InteractableDetector"/>에 의해 감지는 되지만, 아무런 상호작용을 할 수 없다는 뜻이다. <br/>
		/// </remarks>
		public bool IsInteractionAllowed => m_allowInteraction;

		public bool IsUsedBy(ItemSO usable)
		{
			foreach (ItemSO thisTarget in m_usedBy)
			{
				if (thisTarget != usable)
				{
					continue;
				}

				return true;
			}

			return false;
		}

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

			m_isHovering = true;

			if (!m_enableHoveringEffect)
			{
				return;
			}

			// 호버링 시각 효과(프레스넬 효과) 시작

			if (null != m_currentHoveringGroup)
			{
				m_currentHoveringGroup.EnableEffect();
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

			m_isHovering = false;

			// 호버링 시각 효과(프레스넬 효과) 끝

			if (!m_enableHoveringEffect)
			{
				return;
			}

			if (null != m_currentHoveringGroup)
			{
				m_currentHoveringGroup.DisableEffect();
			}
		}

		// TODO: Optimization Problem

		/// <summary>
		/// 캐릭터가 상호작용(키보드 F)을 수행할 때 호출
		/// </summary>
		public virtual void StartInteract(InteractAction interactor)
		{
#if UNITY_EDITOR
			if (m_logOnInteract)
			{
				Debug.Log($"주체 {interactor}에 의하여 {name} Activate 시작");
			}
#endif
		}

		/// <summary>
		/// 캐릭터가 사용 (마우스 LMB)을 수행할 때 호출
		/// </summary>
		public virtual void StartUse(UseAction user, ItemSO equipment)
		{
#if UNITY_EDITOR
			if (m_logOnUse)
			{
				Debug.Log($"주체 {user.name}의 장비 {equipment.name}에 의하여 {name} Use 시작");
			}
#endif
		}

		/// <summary>
		/// Rigidbody의 Kinematic 설정과 <see cref="HoveringEffectGroup"/>의 초기화를 진행함. 
		/// </summary>
		/// <remarks>
		/// 이 클래스를 상속하는 경우 베이스 콜을 반드시 잊지 말것!
		/// </remarks>
		protected virtual void Awake()
		{
			GetComponent<Rigidbody>().isKinematic = true;

			if (null != m_defaultHoveringEffectGroup)
			{
				m_defaultHoveringEffectGroup.Initialize();
			}

			m_currentHoveringGroup = m_defaultHoveringEffectGroup;
		}

		protected virtual void OnEnable() { }
		protected virtual void Start() { }
		protected virtual void OnDisable() { }
	}
}
