using BM.Interactables;

using System.Collections.Generic;

using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	public class NewInteractAction : MonoBehaviour
	{
		[SerializeField] private InputReaderSO m_inputReader;
		[SerializeField] private float m_raycastDistance = 5.0f;

		[SerializeField] private LayerMask m_nonPlaceStateLayerMask = (1 << 6);
		[SerializeField] private LayerMask m_placeStateLayerMask = (1 << 6) | (0 << 1);

		private Collider m_characterCollider;

		private InteractableBase m_detectedInteractable;

		private InteractableBase m_equippedInteractable;
		private Stack<InteractableBase> m_temporaryInventory = new();

		private bool m_isHit;
		private RaycastHit m_hitResult;
		private Rigidbody m_hitRigidbodyOnLastFrame;

		private void FixedUpdate()
		{
			HandleInteractRaycast();
		}

		private void HandleInteractRaycast()
		{
			bool isRaycastHit = Physics.Raycast(GetCameraRay(), out m_hitResult, m_raycastDistance, m_nonPlaceStateLayerMask, QueryTriggerInteraction.Ignore);

			if (!isRaycastHit)
			{
				m_isHit = isRaycastHit;
				m_detectedInteractable = null;
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
					m_detectedInteractable.FinishHovering();
				}

				if (hitRigidbody.TryGetComponent<InteractableBase>(out m_detectedInteractable))
				{
					m_detectedInteractable.StartHovering();
				}

				m_hitRigidbodyOnLastFrame = hitRigidbody;
			}
			else
			{
				if (null != m_detectedInteractable)
				{
					m_detectedInteractable.FinishHovering();
				}

				m_detectedInteractable = null;
				m_hitRigidbodyOnLastFrame = null;
			}
		}

		private Ray GetCameraRay()
		{
			// Screen mid point
			Vector3 viewportPoint = new(0.5f, 0.5f, 0.0f);

			// NOTE: This can be broken when Camera.main is not gameplay camera
			return Camera.main.ViewportPointToRay(viewportPoint);
		}

#if UNITY_EDITOR
		[UnityEditor.DrawGizmo(UnityEditor.GizmoType.Active | UnityEditor.GizmoType.NonSelected)]
		private static void DrawRaycastResult(NewInteractAction target, UnityEditor.GizmoType _)
		{
			if (null == Camera.main || !Application.isPlaying)
			{
				return;
			}

			if (target.m_isHit)
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

			Gizmos.DrawWireSphere(point, 0.1f);

			if (target.m_isHit)
			{
				Vector3 normal = target.m_hitResult.normal;
				Gizmos.DrawRay(point, normal * 0.5f);
			}
		}
#endif
	}
}
