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

		[SerializeField] private EventReference m_jogEventReference;
		[SerializeField] private EventReference m_walkEventReference;
		[SerializeField] private EventReference m_crouchEventReference;

		[SerializeField] private bool m_inCarpet = false;

		private EventInstance m_eventInstance;
		private LocomotiveAction m_locomotiveAction;

		private PARAMETER_DESCRIPTION m_parameterDescription;
		private const string LABEL_3D_MEMORY = "3D_Memory";

		private void FootstepAudio_LocomotionImpulseGenerated(Vector3 position, float force)
		{
			PlayFootstepAudioOnPosition(in position, in force);
		}
		private void FootstepAudio_LocomotiveStateChanged(LocomotiveAction.State state)
		{

		}

		private void PlayFootstepAudioOnPosition(in Vector3 position, in float force)
		{
			if (!m_applyFootstepBaseAudio)
			{
				return;
			}

			if (!m_eventInstance.isValid())
			{
				return;
			}

			if (!m_inCarpet)
			{

				m_eventInstance.setParameterByName(LABEL_3D_MEMORY, 0);
			}
			else
			{
				m_eventInstance.setParameterByName(LABEL_3D_MEMORY, 1);
			}

			m_eventInstance.setVolume(m_masterVolume * force);
			m_eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
			m_eventInstance.start();
		}

		private void Awake()
		{
			m_locomotiveAction = GetComponent<LocomotiveAction>();

			if (m_crouchEventReference.IsNull)
			{
				Debug.LogError($"{name}에 Event Reference가 할당되지 않았습니다.", this);
			}
		}

		private void Start()
		{
			m_eventInstance = RuntimeManager.CreateInstance(m_crouchEventReference);
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
			if (m_eventInstance.isValid())
			{
				m_eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
				m_eventInstance.release();
			}
		}
	}
}