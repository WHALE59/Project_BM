
using BM.Interactables;
using FMODUnity;
using System;
using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	public class InteractableDetector : MonoBehaviour
	{
		[Header("Detection Settings")]
		[Space]

		[SerializeField] private float m_raycastDistance = 5.0f;
		[SerializeField] private LayerMask m_layerMask = (1 << 6);

		[Header("Sound Settings")]
		[Space]

		[SerializeField] private bool m_enableHoveringSound = true;
		[SerializeField] private EventReference m_soundOnHovering;
		[SerializeField] private EventReference m_defaultSoundOnCollecting;

#if UNITY_EDITOR
		[Header("Debug")]
		[Space]

		[SerializeField] private bool m_logOnInteractableFoundAndLost;
#endif


		public event Action<InteractableBase> InteractableFound;
		public event Action<InteractableBase> InteractableLost;

		private bool m_isValidInteractionHit;
		private RaycastHit m_hitResult;
		private InteractableModel m_hitModelOnLastFrame;

		private InteractableBase m_detectedInteractable;

		public InteractableBase DetectedInteractable => m_detectedInteractable;

		private Ray GetCameraRay()
		{
			// Screen mid point
			Vector3 viewportPoint = new(.5f, .5f, .0f);

			// NOTE: This can be broken when Camera.main is not gameplay camera
			return Camera.main.ViewportPointToRay(viewportPoint);
		}

		public void DetectedInteractableGone()
		{
			InteractableLost?.Invoke(m_detectedInteractable);
			m_detectedInteractable = null;
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
					bool hasAttachedInteractable = hitRigidbody.TryGetComponent<InteractableBase>(out newlyDetectedInteractable);

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
					InteractableModel newlyDetectedModel = newlyDetectedInteractable.Model;

					// 내부의 모델이 변하였다
					if (m_hitModelOnLastFrame != newlyDetectedModel)
					{
						if (null != m_hitModelOnLastFrame)
						{
							m_hitModelOnLastFrame.FinishHoveringEffect();
						}

						newlyDetectedModel.StartHoveringEffect();
					}
					// 내부의 모델도 변치 않았다
					else
					{
						// 아무 것도 하지 않아도 됨
					}

					m_hitModelOnLastFrame = newlyDetectedModel;
				}
			}
			// 뭔가 상호작용 가능한 것이 하나도 검출 되지 않았음
			else
			{
				if (null != m_detectedInteractable)
				{
					FinishHoveringProcedure(m_detectedInteractable);

					m_detectedInteractable = null;
					m_hitModelOnLastFrame = null;
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
		private void StartHovering(InteractableBase interactable)
		{
			if (m_enableHoveringSound)
			{
				RuntimeManager.PlayOneShot(m_soundOnHovering);
			}
		}

		/// <summary>
		/// 호버링을 종료 하였을 때에, 캐릭터 쪽에서 처리하여야 하는 로직을 여기에 작성.
		/// </summary>
		private void FinishHovering(InteractableBase interactable)
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

		[UnityEditor.DrawGizmo(UnityEditor.GizmoType.Active | UnityEditor.GizmoType.NonSelected)]
		private static void DrawRaycastResult(InteractableDetector target, UnityEditor.GizmoType _)
		{
			if (null == Camera.main || !Application.isPlaying)
			{
				return;
			}

			if (!target.m_isValidInteractionHit)
			{
				Gizmos.color = Color.white;
			}
			else if (target.m_isValidInteractionHit && null == target.m_detectedInteractable)
			{
				Gizmos.color = Color.yellow;
			}
			else if (target.m_isValidInteractionHit && null != target.m_detectedInteractable)
			{
				Gizmos.color = Color.green;
			}

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

			Vector3 point = default;

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
				InteractableSO interactableSO = target.m_detectedInteractable.InteractableSO;

				if (null != interactableSO)
				{

					if (interactableSO.IsCollectible && !interactableSO.IsActivatable)
					{
						Gizmos.DrawWireSphere(point, .1f);
					}
					else if (interactableSO.IsActivatable && !interactableSO.IsCollectible)
					{
						Gizmos.DrawWireCube(point, new(.1f, .1f, .1f));
					}
				}
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
