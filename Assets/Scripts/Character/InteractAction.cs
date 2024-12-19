using BM.Interactables;

using System;

using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	public class InteractAction : MonoBehaviour
	{
		[SerializeField] private InputReaderSO m_inputReaderSO;
		[SerializeField] private float m_raycastDistance = 5.0f;
		[SerializeField] private LayerMask m_layerMask = (1 << 6);

		private InteractableBase m_detectedInteractable;

		public Action<InteractableBase> InteractableFound;
		public Action<InteractableBase> InteractableLost;

		private bool m_isHit;
		private RaycastHit m_hitResult;
		private Rigidbody m_hitRigidbodyOnLastFrame;

		private Ray GetCameraRay()
		{
			// Screen mid point
			Vector3 viewportPoint = new(0.5f, 0.5f, 0.0f);

			// NOTE: This can be broken when Camera.main is not gameplay camera
			return Camera.main.ViewportPointToRay(viewportPoint);
		}

		private void StartCollectOrActivate()
		{

		}

		private void FinishCollectOrActivate()
		{

		}

		private void HandleInteractRaycast()
		{
			bool isRaycastHit = Physics.Raycast(GetCameraRay(), out m_hitResult, m_raycastDistance, m_layerMask, QueryTriggerInteraction.Ignore);

			if (!isRaycastHit)
			{
				m_isHit = isRaycastHit;

				if (null != m_detectedInteractable)
				{
					FinishHoveringProcedure(m_detectedInteractable);
					m_detectedInteractable = null;
				}

				m_hitRigidbodyOnLastFrame = null;

				return;
			}

			Rigidbody hitRigidbody = m_hitResult.collider.attachedRigidbody;
			bool hasAttachedRigidbody = null != hitRigidbody;

			m_isHit = isRaycastHit && hasAttachedRigidbody;

			if (m_isHit)
			{
				if (hitRigidbody == m_hitRigidbodyOnLastFrame)
				{
					return;
				}

				if (null != m_detectedInteractable)
				{
					FinishHoveringProcedure(m_detectedInteractable);
					m_detectedInteractable = null;
				}

				if (hitRigidbody.TryGetComponent<InteractableBase>(out m_detectedInteractable))
				{
					StartHoveringProcedure(m_detectedInteractable);
				}

				m_hitRigidbodyOnLastFrame = hitRigidbody;
			}
			else
			{
				if (null != m_detectedInteractable)
				{
					FinishHoveringProcedure(m_detectedInteractable);
					m_detectedInteractable = null;
				}

				m_hitRigidbodyOnLastFrame = null;
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
		/// 호버링 시작 하였을 때의 UI의 변경 등을 여기서 처리
		/// </summary>
		private void StartHovering(InteractableBase interactable)
		{
		}

		/// <summary>
		/// 호버링 종료 하였을 때의 UI의 변경 등을 여기서 처리
		/// </summary>
		private void FinishHovering(InteractableBase interactable)
		{
		}


		private void Awake()
		{
			m_inputReaderSO.CollectOrActivateInputPerformed += StartCollectOrActivate;
			m_inputReaderSO.CollectOrActivateInputCanceled += FinishCollectOrActivate;
		}

		private void FixedUpdate()
		{
			HandleInteractRaycast();
		}

#if UNITY_EDITOR
		[UnityEditor.DrawGizmo(UnityEditor.GizmoType.Active | UnityEditor.GizmoType.NonSelected)]
		private static void DrawRaycastResult(InteractAction target, UnityEditor.GizmoType _)
		{
			if (null == Camera.main || !Application.isPlaying)
			{
				return;
			}

			if (!target.m_isHit)
			{
				Gizmos.color = Color.white;
			}
			else if (target.m_isHit && null == target.m_detectedInteractable)
			{
				Gizmos.color = Color.yellow;
			}
			else if (target.m_isHit && null != target.m_detectedInteractable)
			{
				Gizmos.color = Color.green;
			}

			// Ray 그리기

			Ray ray = target.GetCameraRay();

			if (!target.m_isHit)
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

			if (target.m_isHit)
			{
				point = target.m_hitResult.point;
			}
			else
			{
				point = ray.origin + ray.direction * target.m_raycastDistance;
			}

			if (target.m_detectedInteractable != null)
			{
				switch (target.m_detectedInteractable.Type)
				{
					case InteractionType.Collectible:
						Gizmos.DrawWireSphere(point, .1f);
						break;
					case InteractionType.Activatable:
						Gizmos.DrawWireCube(point, new(.1f, .1f, .1f));
						break;
				}
			}

			if (target.m_isHit)
			{
				Vector3 normal = target.m_hitResult.normal;
				Gizmos.DrawRay(point, normal * 0.5f);
			}
		}
#endif
	}
}
