using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
#endif

namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(BoxCollider))]
	public class FootstepResponseAudioVolume : FootstepAudioVolume
	{
		[Header("발걸음 반응 소리 설정")]
		[Space()]

		[Tooltip("발걸음 반응 소리를 재생할 확률입니다. 1.0 인 경우, 항상 재생합니다.")]
		[SerializeField][Range(0.0f, 1.0f)] private float m_responseProbability = 0.5f;

		[Tooltip("발걸음 반응 소리가 발걸음의 세기에 영향을 받도록 할 지의 여부를 설정합니다.")]
		[SerializeField] private bool m_responseAffectedByFootstepForce = true;

		private Transform m_characterTransform;

		private void OnFootStepped(bool _, float normalizedFootstepForce)
		{
			if (!m_footstepAudioClipSet)
			{
				return;
			}

			var isHit = Random.Range(0.0f, 1.0f) <= Mathf.Clamp01(m_responseProbability);

			if (!isHit)
			{
				return;
			}

			var clip = m_footstepAudioClipSet.PickClip();

			if (!clip)
			{
				return;
			}

			if (m_responseAffectedByFootstepForce)
			{
				AudioSource.PlayClipAtPoint(clip, m_characterTransform.position, normalizedFootstepForce);
			}
			else
			{
				AudioSource.PlayClipAtPoint(clip, m_characterTransform.position, 1.0f);
			}


#if UNITY_EDITOR
			m_hitPositions.Enqueue(m_characterTransform.position);

			if (m_hitPositions.Count > m_maxPositionCache)
			{
				m_hitPositions.Dequeue();
			}
#endif
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!collider.TryGetComponent<LocomotiveActions>(out var locomotiveActions))
			{
				return;
			}

			m_characterTransform = locomotiveActions.transform;
			locomotiveActions.LocomotionImpulseGenerated += OnFootStepped;
		}

		private void OnTriggerExit(Collider collider)
		{
			if (!collider.TryGetComponent<LocomotiveActions>(out var locomotiveActions))
			{
				return;
			}

			locomotiveActions.LocomotionImpulseGenerated -= OnFootStepped;
			m_characterTransform = null;
		}

#if UNITY_EDITOR
		private Queue<Vector3> m_hitPositions = new();

		[SerializeField] private uint m_maxPositionCache = 10;

		private void OnValidate()
		{
			while (m_maxPositionCache < m_hitPositions.Count)
			{
				m_hitPositions.Dequeue();
			}
		}

		protected override void OnDrawGizmos()
		{
			base.OnDrawGizmos();

			// 발소리 반응 소리 위치

			Gizmos.color = Color.red;
			foreach (var position in m_hitPositions)
			{
				Gizmos.DrawWireSphere(position, 0.2f);
			}
		}
#endif
	}
}