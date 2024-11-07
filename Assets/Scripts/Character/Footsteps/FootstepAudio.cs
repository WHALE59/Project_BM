using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(LocomotiveAction))]
	public class FootstepAudio : MonoBehaviour
	{
		[SerializeField] private bool m_applyFootstepBaseAudio = true;
		[SerializeField][Range(0.0f, 1.0f)] private float m_masterVolume = 1.0f;

		[SerializeField] private EventReference m_eventReference;

		private EventInstance m_eventInstance;
		private LocomotiveAction m_locomotiveAction;

		private void PlayFootstepAudioOnPosition(Vector3 position, float _)
		{
			if (!m_applyFootstepBaseAudio)
			{
				return;
			}

			if (!m_eventInstance.isValid())
			{
				return;
			}

			m_eventInstance.setVolume(m_masterVolume);
			m_eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
			m_eventInstance.start();
		}

		private void Awake()
		{
			m_locomotiveAction = GetComponent<LocomotiveAction>();
		}

		private void Start()
		{
			m_eventInstance = RuntimeManager.CreateInstance(m_eventReference);
		}

		private void OnEnable()
		{
			m_locomotiveAction.LocomotionImpulseGenerated += PlayFootstepAudioOnPosition;
		}

		private void OnDisable()
		{
			m_locomotiveAction.LocomotionImpulseGenerated -= PlayFootstepAudioOnPosition;
		}

		private void OnDestroy()
		{
			if (m_eventInstance.isValid())
			{
				m_eventInstance.release();
			}
		}
	}
}