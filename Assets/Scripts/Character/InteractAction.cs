using BM.InteractableObjects;

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BM
{
	[DisallowMultipleComponent]
	public class InteractAction : MonoBehaviour
	{
		[SerializeField] private InputReaderSO m_inputReader;
		[SerializeField] private Transform m_equipSocket;

		[SerializeField] private float m_raycastDistance = 5.0f;

		[SerializeField] private LayerMask m_nonPlaceStateLayerMask = (1 << 6);
		[SerializeField] private LayerMask m_placeStateLayerMask = ~(1 << 6) | (0 << 1);

		private Collider m_characterCollider;

		private IInteractable m_hoveringInteractable;
		private IEquippable m_hoveringEquippable;
		private ICollectible m_hoveringCollectible;
		private IActivatable m_hoveringActivatable;
		private IUsable m_hoveringUsable;

		private IEquippable m_equipment;
		private Stack<ICollectible> m_inventory = new();

		private PlacementGhost m_ghost;

		private GameObject m_hitGameObjectOnLastFrame;

		private bool m_isHit;
		private RaycastHit m_hitResult;

		private Ray CameraRay => Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));

		private void UpdateInteractable(IInteractable interactable)
		{
			m_hoveringEquippable = interactable as IEquippable;
			m_hoveringCollectible = interactable as ICollectible;
			m_hoveringActivatable = interactable as IActivatable;
			m_hoveringUsable = interactable as IUsable;
		}

		private void ResetInteractable()
		{
			m_hoveringEquippable = null;
			m_hoveringCollectible = null;
			m_hoveringActivatable = null;
			m_hoveringUsable = null;
		}

		/// <summary>
		/// 매 고정 프레임 수행되면서 조준자에 <see cref="InteractableBase"/>이 있다면 참조를 <see cref="m_hoveringInteractable"/>에 업데이트 함. <br/>
		/// 없다면 <see cref="m_hoveringInteractable"/>을 <c>null</c>로 만듦
		/// </summary>
		private void HandleInteractRaycast()
		{
			if (m_ghost == null)
			{
				m_isHit = Physics.Raycast(CameraRay, out m_hitResult, m_raycastDistance, m_nonPlaceStateLayerMask, QueryTriggerInteraction.Ignore);
			}
			else
			{
				m_isHit = Physics.Raycast(CameraRay, out m_hitResult, m_raycastDistance, m_placeStateLayerMask, QueryTriggerInteraction.Ignore);
			}

			// 뭔가가 검출 되었음
			if (m_isHit)
			{
				GameObject hitGameObject = m_hitResult.transform.gameObject;

				// 지금 Raycast 결과가 이전거랑 같은 거면, 딱히 뭐 할 필요 없음
				if (hitGameObject == m_hitGameObjectOnLastFrame)
				{
					return;
				}

				// 일단 이전 프레임과는 다른 것이 검출된 상황
				// (이전에 하고 있었다면) 호버링 해제
				if (m_hoveringInteractable != null)
				{
					m_hoveringInteractable.FinishHovering();

				}

				// 지금 검출된 대상이 상호작용 가능한 타겟인지 확인
				if (hitGameObject.TryGetComponent<IInteractable>(out m_hoveringInteractable))
				{
					// 맞으면, 해당 상호작용 가능 오브젝트로 호버링 상태 돌입
					m_hoveringInteractable.StartHovering();

					UpdateInteractable(m_hoveringInteractable);
				}

				m_hitGameObjectOnLastFrame = hitGameObject;
			}
			// 아무것도 검출되지 않았음
			else
			{
				// 일단 현재 아무것도 검출되지 않은 상황
				// (이전에 하고 있었다면) 호버링 해제
				if (m_hoveringInteractable != null)
				{
					m_hoveringInteractable.FinishHovering();
				}

				// 정리
				m_hoveringInteractable = null;
				m_hitGameObjectOnLastFrame = null;

				ResetInteractable();
			}
		}

		private Coroutine m_equipRoutine;

		private IEnumerator EquipmentAttachingRoutine(IEquippable equippable)
		{

			while (true)
			{
				Vector3 positionOffset = equippable.EquipmentPosition - equippable.SocketPosition;
				Vector3 targetPosition = m_equipSocket.position + positionOffset;

				Quaternion rotationOffset = Quaternion.Inverse(equippable.EquipmentRotation) * equippable.SocketRotation;
				Quaternion targetRotation = m_equipSocket.rotation * rotationOffset;

				equippable.EquipmentPosition = targetPosition;
				equippable.EquipmentRotation = targetRotation;

				yield return new WaitForFixedUpdate();
			}
		}

		private void SetIgnoreCollisionWithEquipment(IEquippable equippable, bool ignore)
		{
			foreach (Collider collider in equippable.Colliders)
			{
				Physics.IgnoreCollision(m_characterCollider, collider, ignore: ignore);
				collider.isTrigger = ignore;
			}
		}

		/// <summary>
		/// 아무것도 쥐고 있지 않을 때, 쥘 수 있다면 쥔다. (bare hand => hand)
		/// </summary>
		private void Equip()
		{
			if (m_equipment != null) // == not bare hand
			{
				return;
			}

			if (m_hoveringEquippable == null) // == not hovering
			{
				// Equip 실패 로직

				return;
			}

			EquipInner(m_hoveringEquippable);

			ResetInteractable();
		}

		private void EquipInner(IEquippable toEquip)
		{
			m_equipment = toEquip;
			// Equip 로직

			SetIgnoreCollisionWithEquipment(m_equipment, ignore: true);

			m_equipRoutine = StartCoroutine(EquipmentAttachingRoutine(m_equipment));
		}

		private void Collect()
		{
			if (m_hoveringCollectible == null)
			{
				return;
			}

			// Collect 로직
			CollectInner(m_hoveringCollectible);

			ResetInteractable();
		}

		private void CollectInner(ICollectible toCollect)
		{
			toCollect.Disable();
			m_inventory.Push(toCollect);
		}

		private void Activate() { }

		private void PushPopInventory()
		{
			// 손이 비어있으면 인벤토리에 가장 최근에 들어갔던 아이템을 Pop
			if (m_equipment == null)
			{
				if (m_inventory.Count == 0)
				{
					return;
				}

				ICollectible recentCollectible = m_inventory.Pop();

				recentCollectible.Enable();
				EquipInner(recentCollectible);
			}
			// 손이 비어있지 않으면 손에 있는 아이템을 인벤토리로 Push
			else
			{
				if (m_equipment is ICollectible toCollect)
				{
					StopCoroutine(m_equipRoutine);
					m_equipRoutine = null;

					toCollect.Disable();
					CollectInner(toCollect);

					m_equipment = null;
				}
			}
		}

		private void Use() { }

		private void Place()
		{
			if (m_ghost == null)
			{
				return;
			}

			if (!m_ghost.CanBePlaced)
			{
				return;
			}

			StopCoroutine(m_equipRoutine);
			m_equipRoutine = null;

			StopCoroutine(m_ghostTrackingRoutine);
			m_ghostTrackingRoutine = null;

			m_equipment.EquipmentPosition = m_ghost.transform.position;
			m_equipment.EquipmentRotation = m_ghost.transform.rotation;

			m_equipment.DisableGhost();

			SetIgnoreCollisionWithEquipment(m_equipment, ignore: false);

			m_equipment = null;
			m_ghost = null;
		}

		private Coroutine m_ghostTrackingRoutine;

		private IEnumerator GhostTrackingRoutine()
		{
			while (true)
			{
				if (m_isHit)
				{
					m_equipment.EnableGhost();

					Vector3 position = m_hitResult.point;
					Vector3 up = m_hitResult.normal;
					Vector3 forward = transform.forward;

					m_ghost.transform.position = position;
					m_ghost.transform.up = up;
					m_ghost.transform.forward = forward;
				}
				else
				{
					m_equipment.DisableGhost();
				}

				yield return new WaitForFixedUpdate();
			}
		}

		private void TogglePlaceMode()
		{
			if (m_equipment == null)
			{
				return;
			}

			if (m_ghost == null)
			{
				m_ghost = m_equipment.Ghost;
				m_ghostTrackingRoutine = StartCoroutine(GhostTrackingRoutine());
			}
			else
			{
				m_equipment.DisableGhost();
				StopCoroutine(m_ghostTrackingRoutine);
				m_ghost = null;
			}
		}

		private void Awake()
		{
			m_characterCollider = GetComponent<Collider>();
		}

		private void OnEnable()
		{
			m_inputReader.EquipInputTriggered += Equip;
			m_inputReader.PlaceInputTriggered += Place;
			m_inputReader.CollectHoldInputTriggered += Collect;
			m_inputReader.TogglePlaceModeInputTriggered += TogglePlaceMode;

			m_inputReader.PushPopInventoryInputTriggered += PushPopInventory;
		}

		private void OnDisable()
		{
			m_inputReader.EquipInputTriggered -= Equip;
			m_inputReader.PlaceInputTriggered -= Place;
			m_inputReader.CollectHoldInputTriggered -= Collect;
			m_inputReader.TogglePlaceModeInputTriggered -= TogglePlaceMode;

			m_inputReader.PushPopInventoryInputTriggered -= PushPopInventory;
		}

		private void FixedUpdate()
		{
			HandleInteractRaycast();
		}

#if UNITY_EDITOR

		[Header("기즈모 설정")]
		[Space()]

		[SerializeField] private bool m_drawRayAndResult = true;
		[SerializeField] private bool m_drawGrabSocket = true;

		/// <summary>
		/// 캐릭터 레이캐스팅의 결과를 나타내는 Gizmo를 그린다.
		/// </summary>
		[DrawGizmo(GizmoType.Active | GizmoType.NonSelected)]
		private static void DrawRayAndResult(InteractAction target, GizmoType _)
		{
			if (!Camera.main || !Application.isPlaying)
			{
				return;
			}

			if (!target.m_drawRayAndResult)
			{
				return;
			}

			// 마스터 색상 설정

			if (!target.m_isHit)
			{
				Gizmos.color = Color.white;
			}
			else if (target.m_isHit && target.m_hoveringInteractable == null)
			{
				Gizmos.color = Color.yellow;
			}
			else if (target.m_isHit && target.m_hoveringInteractable != null)
			{
				Gizmos.color = Color.green;
			}

			// (1) Ray 그리기

			Ray ray = target.CameraRay;

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

			// (2) Raycast Result 그리기

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

		/// <summary>
		/// <see cref="m_equipSocket"/>의 위치를 나타내는 Gizmo를 그린다.
		/// </summary>
		[DrawGizmo(GizmoType.Active | GizmoType.NonSelected)]
		private static void DrawGrabSocketTransform(InteractAction target, GizmoType _)
		{
			if (target.m_equipSocket == null)
			{
				return;
			}

			if (!target.m_drawGrabSocket)
			{
				return;
			}

			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(target.m_equipSocket.position, target.m_equipSocket.forward * 0.2f);

			Gizmos.color = Color.green;
			Gizmos.DrawRay(target.m_equipSocket.position, target.m_equipSocket.up * 0.2f);

			Gizmos.color = Color.red;
			Gizmos.DrawRay(target.m_equipSocket.position, target.m_equipSocket.right * 0.2f);
		}
#endif
	}
}