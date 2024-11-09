using UnityEngine;

namespace BM
{
	[RequireComponent(typeof(BoxCollider))]
	public abstract class FootstepAudioVolume : MonoBehaviour
	{
		[Header("오디오 설정")]
		[Space()]

		[Tooltip("캐릭터가 볼륨 안에 진입했을 때, 재생할 오디오 클립 정보입니다.")]
		[SerializeField] protected RandomAudioClipSet m_footstepAudioClipSet;

#if UNITY_EDITOR
		private BoxCollider m_boxCollider;

		[Header("기즈모 설정")]
		[Space()]

		[SerializeField] private Color m_gizmoColor = Color.cyan;
		[SerializeField] private float m_gizmoAlpha = 0.02f;

		private void Awake()
		{
			m_boxCollider = GetComponent<BoxCollider>();
		}
		protected virtual void OnDrawGizmos()
		{
			if (!m_boxCollider)
			{
				m_boxCollider = GetComponent<BoxCollider>();
			}

			// 외곽선

			Gizmos.color = m_gizmoColor - new Color(0.0f, 0.0f, 0.0f, 1 - m_gizmoAlpha);
			Gizmos.DrawWireCube(transform.position + m_boxCollider.center, m_boxCollider.bounds.extents * 2);

			// 내부

			Gizmos.DrawCube(transform.position + m_boxCollider.center, m_boxCollider.bounds.extents * 2);

			// 데이터 이름

			if (m_footstepAudioClipSet)
			{
				UnityEditor.Handles.Label(transform.position + m_boxCollider.center, m_footstepAudioClipSet.name);
			}
			else
			{
				UnityEditor.Handles.Label(transform.position + m_boxCollider.center, "No Footstep Response Data Allocated");
			}
		}
#endif
	}
}