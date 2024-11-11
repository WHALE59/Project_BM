using BM.Objects;

using UnityEngine;
using UnityEngine.Events;

namespace BM
{
	[DisallowMultipleComponent]
	public class CameraRaycast : MonoBehaviour
	{
		public UnityAction<IRaycastable> HoveringStarted;
		public UnityAction<IRaycastable> HoveringFinished;

		[Header("레이 캐스트 설정")]
		[Space()]

		[SerializeField] private float m_maxDistance = 5.0f;
		[SerializeField] private LayerMask m_layerMask = (1 << 0);

		private GameObject m_hitRootOnLastFrame;
		private IRaycastable m_raycastable;

		private Ray CameraRay => new(Camera.main.transform.position, Camera.main.transform.forward);

#if UNITY_EDITOR
		private bool m_isHit;
		private RaycastHit m_hitResultInLastFrame;
#endif

		private bool Raycast(out RaycastHit hitResult)
		{
			var isHit = Physics.Raycast(CameraRay, out hitResult, m_maxDistance, m_layerMask, QueryTriggerInteraction.Ignore);

#if UNITY_EDITOR
			m_hitResultInLastFrame = hitResult;
			m_isHit = isHit;
#endif

			return isHit;
		}

		private void HandleRaycast()
		{
			// 뭔가가 검출 되었음
			if (Raycast(out var hitResult))
			{
				var hitRoot = hitResult.transform.root.gameObject;

				// 지금 Raycast 결과가 이전거랑 같은 거면, 딱히 뭐 할 필요 없음
				if (hitRoot == m_hitRootOnLastFrame)
				{
					return;
				}

				// 이전에 (하고 있었다면) 호버링 상태 해제
				if (m_raycastable != null)
				{
					m_raycastable.FinishHovering();
					HoveringFinished?.Invoke(m_raycastable);
				}

				// 지금 Raycast 타겟인지 확인
				if (hitRoot.TryGetComponent<IRaycastable>(out m_raycastable))
				{
					// 맞으면, 해당 오브젝트로 호버링 상태 돌입
					m_raycastable.StartHovering();
					HoveringStarted?.Invoke(m_raycastable);
				}

				m_hitRootOnLastFrame = hitRoot;
			}
			// 암것도 안 검출되었으면, 
			else
			{
				if (m_raycastable != null)
				{
					m_raycastable.FinishHovering();
					HoveringFinished?.Invoke(m_raycastable);
				}

				m_raycastable = null;
				m_hitRootOnLastFrame = null;
			}

		}

		private void Awake()
		{
			HoveringStarted += (IRaycastable target) =>
			{
				var bmObject = target as BMObjectBase;

				if (!bmObject)
				{
					return;
				}

				Debug.Log($"Started: {bmObject.gameObject.name}");
			};
			HoveringFinished += (IRaycastable target) =>
			{
				var bmObject = target as BMObjectBase;

				if (!bmObject)
				{
					return;
				}

				Debug.Log($"Ended: {bmObject.gameObject.name}");
			};
		}

		private void FixedUpdate()
		{
			HandleRaycast();
		}

#if UNITY_EDITOR
		[Header("기즈모 설정")]
		[Space()]

		[SerializeField] private Color m_colorOnNoResult = Color.white;
		[SerializeField] private Color m_colorOnNotRaycastable = Color.yellow;
		[SerializeField] private Color m_colorOnRaycastable = Color.green;

		[Space()]
		[SerializeField] private float m_resultSphereSize = 0.1f;
		[SerializeField] private float m_resultNormalLength = 0.5f;

		[Space()]
		[SerializeField] private Color m_colorOnCollectible = Color.magenta - new Color(0.0f, 0.0f, 0.0f, 0.7f);
		[SerializeField] private Color m_colorOnHoldable = Color.red - new Color(0.0f, 0.0f, 0.0f, 0.7f);
		[SerializeField] private Color m_colorOnInteractable = Color.cyan - new Color(0.0f, 0.0f, 0.0f, 0.7f);

		private void OnDrawGizmosSelected()
		{
			if (!Camera.main)
			{
				return;
			}

			// 마스터 색상 설정

			if (!m_isHit)
			{
				Gizmos.color = m_colorOnNoResult;
			}
			else if (m_isHit && m_raycastable == null)
			{
				Gizmos.color = m_colorOnNotRaycastable;
			}
			else if (m_isHit && m_raycastable != null)
			{
				Gizmos.color = m_colorOnRaycastable;
			}

			// (1) Ray 그리기

			// 아무것도 안
			var ray = CameraRay;
			if (!m_isHit)
			{
				Gizmos.DrawRay(ray.origin, ray.direction * m_maxDistance);
			}
			else
			{
				var firstStart = ray.origin;
				var firstEnd = m_hitResultInLastFrame.point;

				Gizmos.DrawLine(firstStart, firstEnd);

				var previousGizmoColor = Gizmos.color;
				Gizmos.color = m_colorOnNoResult;

				var secondStart = firstEnd;
				var secondEnd = firstStart + ray.direction * m_maxDistance;
				Gizmos.DrawLine(secondStart, secondEnd);

				Gizmos.color = previousGizmoColor;
			}

			// (2) Raycast Result 그리기

			var point = m_hitResultInLastFrame.point;
			var normal = m_hitResultInLastFrame.normal;

			Gizmos.DrawWireSphere(point, m_resultSphereSize);
			Gizmos.DrawRay(point, normal * m_resultNormalLength);

			// (3) 대상 검출 후 bound 그리기

			if (m_raycastable is CollectibleObject)
			{
				Gizmos.color = m_colorOnCollectible;
			}
			else if (m_raycastable is HoldableObject)
			{
				Gizmos.color = m_colorOnHoldable;
			}
			else if (m_raycastable is BM.Objects.InteractableObject)
			{
				Gizmos.color = m_colorOnInteractable;
			}

			var bmObject = m_raycastable as BMObjectBase;

			if (!bmObject)
			{
				return;
			}

			var collider = bmObject.GetComponentInChildren<Collider>();

			if (collider)
			{
				Gizmos.DrawCube(collider.bounds.center, collider.bounds.extents * 2.0f);
			}
		}
#endif
	}

}