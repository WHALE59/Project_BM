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

		[SerializeField] private EventReference m_footstepEventReference;

		private EventInstance m_footstepEventInstance;

		private LocomotiveAction m_locomotiveAction;

		private void FootstepAudio_LocomotionImpulseGenerated(Vector3 position, float force)
		{
			if (!m_applyFootstepBaseAudio)
			{
				return;
			}

			PlayFootstepAudio(in position, in force);
		}

		private void FootstepAudio_LocomotiveStateChanged(LocomotiveAction.State state)
		{
			// TODO: Tune parameter via locomotive state change
		}

		private void PlayFootstepAudio(in Vector3 position, in float force)
		{
			m_footstepEventInstance.setVolume(m_masterVolume * force);
			m_footstepEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
			m_footstepEventInstance.start();
		}

		private void Awake()
		{
			m_locomotiveAction = GetComponent<LocomotiveAction>();
		}

		private void Start()
		{
			m_footstepEventInstance = RuntimeManager.CreateInstance(m_footstepEventReference);
		}

		private void OnEnable()
		{
			m_locomotiveAction.LocomotionImpulseGenerated += FootstepAudio_LocomotionImpulseGenerated;
			m_locomotiveAction.LocomotiveStateChanged += FootstepAudio_LocomotiveStateChanged;
		}

		private void OnDisable()
		{
			m_locomotiveAction.LocomotionImpulseGenerated -= FootstepAudio_LocomotionImpulseGenerated;
			m_locomotiveAction.LocomotiveStateChanged -= FootstepAudio_LocomotiveStateChanged;
		}

		private void OnDestroy()
		{
			if (m_footstepEventInstance.isValid())
			{
				m_footstepEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
				m_footstepEventInstance.release();
			}
		}
	}
}