#pragma warning disable CS0414

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace BM.Interactables
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	public class InteractableBase : MonoBehaviour
	{
		[Header("Root Appearance")]

		[SerializeField] protected GameObject m_rootAppearance;

		[Header("Hovering Settings")]

		[SerializeField] protected List<Collider> m_hoveringColliders = new();

		[Header("Hovering Effect Settings")]

		[SerializeField] protected bool m_enableHoveringEffect = true;
		[SerializeField] protected List<MeshRenderer> m_hoveringRenderers = new();
		[SerializeField] protected FresnelEffectSO m_fresnelEffectSO;
		[SerializeField] private Sprite m_interactionCrosshair;

		[Header("Usage Settings")]

		[SerializeField] private bool m_isUsedable = false;
		[SerializeField] private List<ItemSO> m_usedBy = new();

		[Header("Control Guide Settings")]

		[SerializeField] private LocalizedString m_controlGuideLocalizedString;

#if UNITY_EDITOR
		[Header("Debug")]

		[SerializeField] private bool m_logOnHovering = false;
		[SerializeField] private bool m_logOnInteraction = false;

		[Space]
#endif

		private int m_detectionLayer = 6;

		private bool m_allowDetection = true;
		protected bool m_allowInteraction = true;
		private bool m_isHovering = false;

		protected bool IsHovering => m_isHovering;
		protected int DetectionLayer => m_detectionLayer;

		public bool IsUsedable => m_isUsedable;
		public LocalizedString ControlGuideLocalizedString => m_controlGuideLocalizedString;
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

			StartHoveringEffect();
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

			FinishHoveringEffect();
		}

		public virtual void StartHoveringEffect()
		{
			if (!m_enableHoveringEffect)
			{
				return;
			}

			// TODO: This can be problem
			EnableFresnelEffectOnMeshGroup(m_fresnelEffectSO, m_hoveringRenderers);
		}

		public virtual void FinishHoveringEffect()
		{
			if (!m_enableHoveringEffect)
			{
				return;
			}

			DisableFresnelEffectOnMeshGroup(m_hoveringRenderers);
		}

		protected void SetActiveHoveringColliders(List<Collider> colliders, bool enabled)
		{
			foreach (Collider collider in colliders)
			{
				collider.enabled = enabled;
			}
		}

		// TODO: Optimization Problem

		protected void EnableFresnelEffectOnMeshGroup(FresnelEffectSO effect, List<MeshRenderer> meshGroup)
		{
			foreach (MeshRenderer renderer in meshGroup)
			{
				foreach (Material material in renderer.materials)
				{
					effect.TryApplyFresnelDataToMaterial(material);
					FresnelEffectSO.TrySetFresnelEffectToMaterial(material, enabled);
				}
			}
		}

		protected void DisableFresnelEffectOnMeshGroup(List<MeshRenderer> meshGroup)
		{
			foreach (MeshRenderer renderer in meshGroup)
			{
				foreach (Material material in renderer.materials)
				{
					FresnelEffectSO.TrySetFresnelEffectToMaterial(material, false);
				}
			}
		}

		public virtual void StartInteraction(InteractAction interactor)
		{
#if UNITY_EDITOR
			if (m_logOnInteraction)
			{
				Debug.Log($"주체 {interactor}에 의하여 {name} Activate 시작");
			}
#endif
		}

		public virtual void StartUsage(UseAction user, ItemSO equipment)
		{
#if UNITY_EDITOR
			if (m_logOnInteraction)
			{
				Debug.Log($"주체 {user.name}의 장비 {equipment.name}에 의하여 {name} Use 시작");
			}
#endif
		}

		protected void TrySetHoveringColliderLayer(List<Collider> colliders)
		{
			if (null == colliders)
			{
				return;
			}

			foreach (Collider collider in colliders)
			{
				collider.gameObject.layer = m_detectionLayer;
			}
		}

		protected void TrySetHoveringRendererEffectData(FresnelEffectSO effect, List<MeshRenderer> renderers)
		{
			if (null == renderers)
			{
				return;
			}

			HashSet<Material> fresnelMaterials = new();

			foreach (MeshRenderer renderer in renderers)
			{
				foreach (Material material in renderer.materials)
				{
					if (fresnelMaterials.Contains(material))
					{
						continue;
					}

					if (effect.TryApplyFresnelDataToMaterial(material))
					{
						fresnelMaterials.Add(material);
					}
				}
			}
		}

		/// <summary>
		/// Rigidbody의 Kinematic 설정과 초기 설정된 인터랙션 Collider와 호버링 머터리얼 설정을 진행함. 만일 상속하는 경우 반드시 베이스 콜을 해 주어야 함.
		/// </summary>
		protected virtual void Awake()
		{
			GetComponent<Rigidbody>().isKinematic = true;

			// initialize Models.

			TrySetHoveringColliderLayer(m_hoveringColliders);
			TrySetHoveringRendererEffectData(m_fresnelEffectSO, m_hoveringRenderers);
		}

		protected virtual void OnEnable() { }
		protected virtual void Start() { }
		protected virtual void OnDisable() { }
	}
}
