using UnityEngine;

namespace BM
{
	[RequireComponent(typeof(LocomotiveActions))]
	public class FootstepAudio : MonoBehaviour
	{
		[Header("Footstep Audio 설정")]
		[Space()]

		[Tooltip("FootstepBase 오디오 재생 여부입니다.")]
		[SerializeField] private bool m_applyFootstepBaseAudio = true;

		[SerializeField][Range(0.0f, 1.0f)] private float m_footstepBaseAudioMasterVolume = 1.0f;

		[Tooltip("왼발과 오른발의 공간 편향이 어느정도인지 설정합니다.")]
		[SerializeField][Range(0.0f, 1.0f)] private float m_footstepAudioStereoPan = 0.085f;

		[Tooltip("FootstepBase의 소리들을 담고 있는 사전입니다.")]
		[SerializeField] private RandomAudioClipSet m_footstepBaseAudioDataFallback;

		private RandomAudioClipSet m_footstepBaseAudioDataOverride;
		private AudioSource m_footstepAudioBaseSource;

		private LocomotiveActions m_locomotiveActions;

		public RandomAudioClipSet FootstepBaseAudioData
		{
			get
			{
				if (!m_footstepBaseAudioDataOverride)
				{
					return m_footstepBaseAudioDataFallback;
				}

				return m_footstepBaseAudioDataOverride;
			}
			set
			{
				m_footstepBaseAudioDataOverride = value;
			}
		}

		private void OnLocomotiveImpulseGenerated(bool isLeft, float force)
		{
			// 사운드 재생

			var bias = (isLeft ? 1.0f : -1.0f) * m_footstepAudioStereoPan;

			if (m_applyFootstepBaseAudio)
			{
				m_footstepAudioBaseSource.volume = force * m_footstepBaseAudioMasterVolume;
				m_footstepAudioBaseSource.panStereo = bias;

				var data = FootstepBaseAudioData;
				if (data)
				{
					var clip = data.PickClip();

					if (clip)
					{
						m_footstepAudioBaseSource.clip = clip;
						m_footstepAudioBaseSource.Play();
					}
				}
			}
		}

		private void Awake()
		{
			m_locomotiveActions = GetComponent<LocomotiveActions>();
			m_footstepAudioBaseSource = GetComponent<AudioSource>();

#if UNITY_EDITOR
			if (!m_footstepBaseAudioDataFallback)
			{
				Debug.LogWarning("FootstepBaseAudioDataFallback이 할당되지 않았습니다.");
			}
#endif
		}

		private void OnEnable()
		{
			m_locomotiveActions.LocomotionImpulseGenerated += OnLocomotiveImpulseGenerated;
		}

		private void OnDisable()
		{
			m_locomotiveActions.LocomotionImpulseGenerated -= OnLocomotiveImpulseGenerated;
		}
	}
}