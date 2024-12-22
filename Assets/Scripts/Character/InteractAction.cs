
using BM.Interactables;
using FMODUnity;
using System;
using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Inventory))]
	public class InteractAction : MonoBehaviour
	{
		[SerializeField] private InputReaderSO m_inputReaderSO;
		[SerializeField] private float m_raycastDistance = 5.0f;
		[SerializeField] private LayerMask m_layerMask = (1 << 6);

		[Space()]

		[SerializeField] private bool m_enableHoveringSound = true;
		[SerializeField] private EventReference m_hoveringSoundEventReference;

		[SerializeField] private InteractableSO m_equipment;

		private InteractableBase m_detectedInteractable;

		public event Action<InteractableBase> InteractableFound;
		public event Action<InteractableBase> InteractableLost;

		public event Action<InteractableSO> Equipped;
		public event Action Unequipped;

		private bool m_isHit;
		private RaycastHit m_hitResult;
		private Rigidbody m_hitRigidbodyOnLastFrame;

		private Inventory m_inventory;

		public InteractableSO Equipment => m_equipment;

		private Ray GetCameraRay()
		{
			// Screen mid point
			Vector3 viewportPoint = new(0.5f, 0.5f, 0.0f);

			// NOTE: This can be broken when Camera.main is not gameplay camera
			return Camera.main.ViewportPointToRay(viewportPoint);
		}

		private void StartCollectOrActivate()
		{
			if (null == m_detectedInteractable)
			{
				return;
			}

			InteractableSO interactableSO = m_detectedInteractable.InteractableSO;

			if (null == interactableSO)
			{
				return;
			}

			if (interactableSO.IsCollectible)
			{
				InteractableLost?.Invoke(m_detectedInteractable);

				// 씬에 배치된 Interactable의 수납 처리

				m_detectedInteractable.SetCollected();
				m_detectedInteractable = null;

				// 효과음 재생

				interactableSO.PlayOneShotCollectingSound();

				// 인벤토리 수납

				m_inventory.PutIn(interactableSO);
			}
			else if (interactableSO.IsActivatable)
			{
			}
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
			if (m_enableHoveringSound)
			{
				RuntimeManager.PlayOneShot(m_hoveringSoundEventReference);
			}
		}

		/// <summary>
		/// 호버링 종료 하였을 때의 UI의 변경 등을 여기서 처리
		/// </summary>
		private void FinishHovering(InteractableBase interactable)
		{
		}

		private void OnValidate()
		{
			if (null != m_equipment)
			{
				Equipped?.Invoke(m_equipment);
			}
			else
			{
				Unequipped?.Invoke();
			}
		}

		private void Awake()
		{
			m_inventory = GetComponent<Inventory>();

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

			if (target.m_isHit)
			{
				Vector3 normal = target.m_hitResult.normal;
				Gizmos.DrawRay(point, normal * 0.5f);
			}
		}
#endif
	}
}
