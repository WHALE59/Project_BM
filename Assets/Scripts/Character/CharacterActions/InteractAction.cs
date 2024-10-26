using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BM
{
	/// <summary>
	/// 캐릭터가 상호작용 가능한 오브젝트를 검출하고 그들과 통신하는 것을 관장하는 컴포넌트
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(InputListener))]
	public class InteractAction : MonoBehaviour
	{
		[Tooltip("상호작용이 가능한 최대 거리를 설정합니다.")]
		[SerializeField] private float m_maxDistance = 10.0f;

		[SerializeField] private UIInteractableBehaviour m_interactableUI;

		private InputListener m_inputListener;

		private const int m_layerMask = ~(1 << 3); // 자기 자신에 대한 레이캐스트 검출을 무시 (임시)

		private bool m_isHit = false;
		private GameObject m_hitRootGameObject;

		private bool m_isInteracting = false;

		private IInteractableObject m_hitInteractableObject;

#if UNITY_EDITOR
		private RaycastHit m_hitResultInLastFrame;
#endif

		private void StartInteract()
		{
			if (m_isInteracting || !m_isHit || m_hitInteractableObject is null)
			{
				return;
			}

			m_isInteracting = true;

			m_interactableUI.StartInteractUI(m_hitInteractableObject);
			m_hitInteractableObject.StartInteract();
		}

		private void FinishInteract()
		{
			if (m_isInteracting && m_hitInteractableObject is not null)
			{
				m_isInteracting = false;
				m_interactableUI.FinishInteractUI(m_hitInteractableObject);
				m_hitInteractableObject.FinishInteract();
			}
		}

		private void Awake()
		{
			m_inputListener = GetComponent<InputListener>();

#if UNITY_EDITOR
			if (!m_interactableUI)
			{
				Debug.LogWarning("InteractAction에 InteractableUI 오브젝트가 할당되지 않았습니다.");
			}
#endif
		}
		private void OnEnable()
		{
			m_inputListener.InteractStarted += StartInteract;
			m_inputListener.InteractFinished += FinishInteract;
		}

		private void OnDisable()
		{
			m_inputListener.InteractStarted -= StartInteract;
			m_inputListener.InteractFinished -= FinishInteract;
		}

		/// <summary>
		/// 매 고정 프레임마다 레이캐스트를 진행하여 오브젝트를 검출
		/// </summary>
		private void FixedUpdate()
		{
			var ray = new Ray
			(
				origin: Camera.main.transform.position,
				direction: Camera.main.transform.forward
			);

			m_isHit = Physics.Raycast
			(
				ray: ray,
				hitInfo: out var resultInCurrentFrame,
				maxDistance: m_maxDistance,
				layerMask: m_layerMask,
				queryTriggerInteraction: QueryTriggerInteraction.Ignore
			);

			if (m_isHit)
			{
				var hitRootGameObject = resultInCurrentFrame.transform.root.gameObject;

				if (hitRootGameObject != m_hitRootGameObject)
				{
					// 이전에 상호작용 가능 오브젝트를 조준 하고 있었으면, 조준 해제
					if (m_hitInteractableObject is not null)
					{
						// 심지어 상호작용 까지 하고 있었던 경우, 상호작용 해제
						if (m_isInteracting)
						{
							m_hitInteractableObject.FinishInteract();
							m_interactableUI.FinishInteractUI(m_hitInteractableObject);

							m_isInteracting = false;
						}

						// 호버링 해제
						m_hitInteractableObject.FinishHovering();
						m_interactableUI.FinishHoveringUI(m_hitInteractableObject);

						m_hitInteractableObject = null;
					}

					// 검출된게 상호작용 가능한지 확인
					if (hitRootGameObject.TryGetComponent<IInteractableObject>(out m_hitInteractableObject))
					{
						m_hitInteractableObject.StartHover();
						m_interactableUI.StartHoveringUI(m_hitInteractableObject);
					}

					// 검출된 것 업데이트
					m_hitRootGameObject = hitRootGameObject;
				}
				else { } // 이전 프레임이랑 같은 것이 검출된 경우 암것도 할 필요가 없음
			}
			else
			{
				// 암것도 안 검출된 경우 이전 프레임에서 하던 짓이 있다면 올 스톱.
				if (m_hitInteractableObject is not null)
				{
					if (m_isInteracting)
					{
						m_hitInteractableObject.FinishInteract();
						m_interactableUI.FinishInteractUI(m_hitInteractableObject);
						m_isInteracting = false;
					}

					m_hitInteractableObject.FinishHovering();
					m_interactableUI.FinishHoveringUI(m_hitInteractableObject);
					m_hitInteractableObject = null;
				}

				m_hitRootGameObject = null;
			}

#if UNITY_EDITOR
			m_hitResultInLastFrame = resultInCurrentFrame;
#endif
		}


#if UNITY_EDITOR
		/// <summary>
		/// Interaction Raycast 경로를 그린다.
		/// </summary>
		[DrawGizmo(GizmoType.Active | GizmoType.Selected)]
		private static void DrawRaycast(InteractAction target, GizmoType _)
		{
			if (!target.m_isHit)
			{
				Gizmos.color = Color.white;
			}
			else if (target.m_isHit && target.m_hitInteractableObject is null)
			{
				Gizmos.color = Color.yellow;
			}
			else if (target.m_isHit && target.m_hitInteractableObject is not null)
			{
				Gizmos.color = Color.green;
			}

			Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * target.m_maxDistance);
		}

		/// <summary>
		/// Raycast 결과 중, InteractionTarget으로 검출된 것에 검출 결과를 그린다.
		/// </summary>
		[DrawGizmo(GizmoType.Active | GizmoType.Selected)]
		private static void DrawRaycastResult(InteractAction target, GizmoType _)
		{
			if (!target.m_isHit)
			{
				Gizmos.color = Color.white;
				Gizmos.DrawWireSphere(Camera.main.transform.position + Camera.main.transform.forward * target.m_maxDistance, 0.1f);
			}
			else if (target.m_isHit)
			{
				if (target.m_isInteracting)
				{
					Gizmos.color = Color.magenta;
				}
				else
				{
					if (target.m_hitInteractableObject is null)
					{
						Gizmos.color = Color.yellow;
					}
					else
					{
						Gizmos.color = Color.green;
					}
				}

				Gizmos.DrawWireSphere(target.m_hitResultInLastFrame.point, 0.1f);
				Gizmos.DrawRay(target.m_hitResultInLastFrame.point, target.m_hitResultInLastFrame.normal * 0.5f);
			}
		}
#endif
	}
}
