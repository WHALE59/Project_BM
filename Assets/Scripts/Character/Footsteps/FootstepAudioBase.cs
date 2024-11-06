using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace BM
{
	// TODO: FMOD Studio Integration
	[RequireComponent(typeof(LocomotiveAction))]
	public class FootstepAudioBase : MonoBehaviour
	{
		[Header("사운드 설정")]
		[Space()]

		[Tooltip("FootstepBase 오디오 재생 여부입니다.")]
		[SerializeField] private bool m_applyFootstepBaseAudio = true;

		[SerializeField][Range(0.0f, 1.0f)] private float m_masterVolume = 1.0f;

		[Tooltip("왼발과 오른발의 공간 편향이 어느정도인지 설정합니다.")]
		[SerializeField][Range(0.0f, 1.0f)] private float m_stereoPan = 0.085f;

		[Tooltip("FootstepBase의 소리들을 담고 있는 사전입니다.")]
		[SerializeField] private RandomAudioClipSet m_footstepBaseAudioDataFallback;

		[SerializeField] private EventReference m_eventReference;
		private EventInstance m_eventInstance;

		private LocomotiveAction m_locomotiveAction;
		private RandomAudioClipSet m_footstepBaseAudioDataOverride;

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

		private void PlayFMODEvent(bool _0, float force)
		{
			m_eventInstance.setVolume(force);
			m_eventInstance.setPitch(Random.Range(0.5f, 1.3f));
			m_eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

			m_eventInstance.start();

			Debug.Log(m_eventInstance);
		}

		private void OnLocomotiveImpulseGenerated(bool isLeft, float force)
		{
			var panning = (isLeft ? 1.0f : -1.0f) * m_stereoPan;

			if (m_applyFootstepBaseAudio)
			{
				//m_footstepAudioBaseSource.volume = force * m_masterVolume;
				//m_footstepAudioBaseSource.panStereo = panning;

				var data = FootstepBaseAudioData;

				if (data)
				{
					var clip = data.PickClip();

					if (clip)
					{
						//m_footstepAudioBaseSource.clip = clip;
						//m_footstepAudioBaseSource.Play();
					}
				}
			}
		}

		private void Awake()
		{
			m_locomotiveAction = GetComponent<LocomotiveAction>();
			//m_footstepAudioBaseSource = GetComponent<AudioSource>();

#if UNITY_EDITOR
			if (!m_footstepBaseAudioDataFallback)
			{
				Debug.LogWarning("FootstepBaseAudioDataFallback이 할당되지 않았습니다.");
			}
#endif
		}

		private void Start()
		{
			m_eventInstance = RuntimeManager.CreateInstance(m_eventReference);
		}

		private void OnEnable()
		{
			m_locomotiveAction.LocomotionImpulseGenerated += PlayFMODEvent;
		}

		private void OnDisable()
		{
			m_locomotiveAction.LocomotionImpulseGenerated -= PlayFMODEvent;

		}

		private void OnDestroy()
		{
			m_eventInstance.release();
		}

	}
}