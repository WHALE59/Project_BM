using BM.Interactables;
using FMODUnity;
using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BM
{
	[DisallowMultipleComponent]
	public class InteractableDetector : MonoBehaviour
	{
		[Header("감지 설정")]

		[SerializeField] private float m_raycastDistance = 2.5f;
		[SerializeField] private LayerMask m_layerMask = (1 << 6);

		[Header("감지 사운드 설정")]

		[SerializeField] private bool m_enableHoveringSound = true;
		[SerializeField] private EventReference m_soundOnHovering;
		[SerializeField] private EventReference m_defaultSoundOnCollecting;

#if UNITY_EDITOR
		[Header("Debug")]

		[SerializeField] private bool m_logOnInteractableFoundAndLost;
#endif

		private bool m_isValidInteractionHit;
		private RaycastHit m_hitResult;

		private InteractableBase m_detectedInteractable;

		/// <summary>
		/// 캐릭터가 어떤 대상에 호버링한 시점과 그 대상을 알고 싶으면 구독
		/// </summary>
		public event Action<InteractableBase> InteractableFound;

		/// <summary>
		/// 캐릭터가 어떤 대상에 호버링을 중지한 시점과 그 대상을 알고 싶으면 구독
		/// </summary>
		public event Action<InteractableBase> InteractableLost;

		/// <summary>
		/// 외부에서 감지된 상호작용 오브젝트가 있는 지 질의할 때에 사용.
		/// </summary>
		/// <remarks> 
		/// 감지된 상호작용 오브젝트가 존재하지 않으면 <c>null</c>을 반환함을 주의
		/// </remarks>
		public InteractableBase DetectedInteractable => m_detectedInteractable;

		private Ray GetCameraRay()
		{
			// Screen mid point
			Vector3 viewportPoint = new(.5f, .5f, .0f);

			// NOTE: This can be broken when Camera.main is not gameplay camera
			return Camera.main.ViewportPointToRay(viewportPoint);
		}

		private void HandleInteractRaycast()
		{
			InteractableBase newlyDetectedInteractable = null;

			bool isRaycastHit = Physics.Raycast
			(
				ray: GetCameraRay(),
				hitInfo: out m_hitResult,
				maxDistance: m_raycastDistance,
				layerMask: m_layerMask,
				queryTriggerInteraction: QueryTriggerInteraction.Ignore
			);

			m_isValidInteractionHit = isRaycastHit;

			if (isRaycastHit)
			{
				Rigidbody hitRigidbody = m_hitResult.collider.attachedRigidbody;
				bool hasAttachedRigidbody = null != hitRigidbody;

				m_isValidInteractionHit &= hasAttachedRigidbody;

				if (hasAttachedRigidbody)
				{
					bool hasAttachedInteractable = hitRigidbody.TryGetComponent(out newlyDetectedInteractable);

					m_isValidInteractionHit &= hasAttachedInteractable;

					if (hasAttachedInteractable)
					{
						m_isValidInteractionHit &= newlyDetectedInteractable.IsDetectionAllowed;
					}
				}
			}

			// 뭔가 상호작용 가능한 것이 실제로 검출 되었음
			if (m_isValidInteractionHit)
			{
				// 새로 검출된 것이 이전 프레임에 검출 된 것과 탑 레벨에서 다른 것
				if (m_detectedInteractable != newlyDetectedInteractable)
				{
					// 이전 프레임에 검출된 것이 null 이 아니라면, 이전 프레임에 호버링 절차를 종료 (TODO: "했다면, 종료?")
					if (null != m_detectedInteractable)
					{
						FinishHoveringProcedure(m_detectedInteractable);
						m_detectedInteractable = null;
					}

					StartHoveringProcedure(newlyDetectedInteractable);
					m_detectedInteractable = newlyDetectedInteractable;
				}
				// 새로 검출된 것이 이전 프레임에 검출된 것과 탑 레벨에서는 같은 것
				else
				{
				}
			}
			// 뭔가 상호작용 가능한 것이 하나도 검출 되지 않았음
			else
			{
				if (null != m_detectedInteractable)
				{
					FinishHoveringProcedure(m_detectedInteractable);

					m_detectedInteractable = null;
				}
			}
		}

		/// <summary>
		/// <paramref name="interactable"/>로 호버링 시작 했을 때, 수행되어야 하는 절차
		/// </summary>
		private void StartHoveringProcedure(InteractableBase interactable)
		{
			StartHovering(interactable);
			interactable.StartHovering();
			InteractableFound?.Invoke(interactable);
		}

		/// <summary>
		/// <paramref name="interactable"/>에서 호버링 종료 했을 때, 수행되어야 하는 절차
		/// </summary>
		private void FinishHoveringProcedure(InteractableBase interactable)
		{
			FinishHovering(interactable);
			interactable.FinishHovering();
			InteractableLost?.Invoke(interactable);
		}

		/// <summary>
		/// 호버링을 시작 하였을 때에, 캐릭터 쪽에서 처리하여야 하는 로직을 여기에 작성.
		/// </summary>
		private void StartHovering(InteractableBase _)
		{
			if (m_enableHoveringSound)
			{
				RuntimeManager.PlayOneShot(m_soundOnHovering);
			}
		}

		/// <summary>
		/// 호버링을 종료 하였을 때에, 캐릭터 쪽에서 처리하여야 하는 로직을 여기에 작성.
		/// </summary>
		private void FinishHovering(InteractableBase _)
		{
		}

		private void OnEnable()
		{
#if UNITY_EDITOR
			InteractableFound += Debug_OnInteractableFound;
			InteractableLost += Debug_OnInteractableLost;
#endif
		}

		private void OnDisable()
		{
#if UNITY_EDITOR
			InteractableFound -= Debug_OnInteractableFound;
			InteractableLost -= Debug_OnInteractableLost;
#endif
		}

		private void FixedUpdate()
		{
			HandleInteractRaycast();
		}

#if UNITY_EDITOR

		private void Debug_OnInteractableFound(InteractableBase found)
		{
			if (m_logOnInteractableFoundAndLost)
			{
				Debug.Log($"{found.name} 탐지");
			}

		}

		private void Debug_OnInteractableLost(InteractableBase lost)
		{
			if (m_logOnInteractableFoundAndLost)
			{
				Debug.Log($"{lost.name} 소실");
			}

		}

		[DrawGizmo(GizmoType.Active | GizmoType.NonSelected)]
		private static void DrawRaycastResult(InteractableDetector target, GizmoType _)
		{
			if (null == Camera.main || !Application.isPlaying)
			{
				return;
			}

			Gizmos.color = target.m_isValidInteractionHit ? Color.green : Color.white;

			// Ray 그리기

			Ray ray = target.GetCameraRay();

			if (!target.m_isValidInteractionHit)
			{
				Gizmos.DrawRay(ray.origin, ray.direction * target.m_raycastDistance);
			}
			else
			{
				Vector3 firstStart = ray.origin;
				Vector3 firstEnd = target.m_hitResult.point;

				Gizmos.DrawLine(firstStart, firstEnd);

				Color previousGizmoColor = Gizmos.color;
				Gizmos.color = Color.white;

				Vector3 secondStart = firstEnd;
				Vector3 secondEnd = firstStart + ray.direction * target.m_raycastDistance;

				Gizmos.DrawLine(secondStart, secondEnd);

				Gizmos.color = previousGizmoColor;
			}

			// Raycast Result 그리기

			Vector3 point;

			if (target.m_isValidInteractionHit)
			{
				point = target.m_hitResult.point;
			}
			else
			{
				point = ray.origin + ray.direction * target.m_raycastDistance;
			}

			if (null != target.m_detectedInteractable)
			{
				Gizmos.DrawWireSphere(point, .1f);
			}

			if (target.m_isValidInteractionHit)
			{
				Vector3 normal = target.m_hitResult.normal;
				Gizmos.DrawRay(point, normal * 0.5f);
			}
		}
#endif
	}
}
