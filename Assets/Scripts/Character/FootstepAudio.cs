using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(LocomotiveAction))]
	[RequireComponent(typeof(LocomotiveImpulseGenerator))]
	public class FootstepAudio : MonoBehaviour
	{
		[SerializeField] private bool m_applyFootstepBaseAudio = true;
		[SerializeField] private bool m_volumeAffectedByForce = false;
		[SerializeField][Range(0.0f, 1.0f)] private float m_masterVolume = 1.0f;

		[SerializeField] private EventReference m_footstepEventReference;

		private EventInstance m_footstepEventInstance;

		private LocomotiveAction m_locomotiveAction;
		private LocomotiveImpulseGenerator m_locomotiveImpulseGenerator;

		private readonly string PARAM_FOOTSTEP = "3D_M_Footsteps_DarackWood";
		// TODO: 이것 보다 더 합리적인 관리 법은 없는 것?
		private readonly string[] LABEL = { "Jog", "Walk", "Crouch" };

		private void FootstepAudio_LocomotiveImpulseGenerated(Vector3 position, float force)
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
			switch (state)
			{
				case LocomotiveAction.State.Idle:
				case LocomotiveAction.State.NormalJog:
					m_footstepEventInstance.setParameterByNameWithLabel(PARAM_FOOTSTEP, LABEL[0]);
					break;
				case LocomotiveAction.State.WalkedJog:
					m_footstepEventInstance.setParameterByNameWithLabel(PARAM_FOOTSTEP, LABEL[1]);
					break;
				case LocomotiveAction.State.CrouchedJog:
					m_footstepEventInstance.setParameterByNameWithLabel(PARAM_FOOTSTEP, LABEL[2]);
					break;
			}
		}

		private void PlayFootstepAudio(in Vector3 position, in float force)
		{
			if (m_volumeAffectedByForce)
			{
				m_footstepEventInstance.setVolume(m_masterVolume * force);
			}
			else
			{
				m_footstepEventInstance.setVolume(m_masterVolume);
			}

			m_footstepEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
			m_footstepEventInstance.start();
		}

		private void Awake()
		{
			m_locomotiveAction = GetComponent<LocomotiveAction>();
			m_locomotiveImpulseGenerator = GetComponent<LocomotiveImpulseGenerator>();

			m_footstepEventInstance = RuntimeManager.CreateInstance(m_footstepEventReference);

			// 이게 없으면 경고가 뜹니다.
			m_footstepEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
		}

		private void OnEnable()
		{
			m_locomotiveAction.LocomotiveStateChanged += FootstepAudio_LocomotiveStateChanged;
			m_locomotiveImpulseGenerator.LocomotiveImpulseGenerated += FootstepAudio_LocomotiveImpulseGenerated;
		}

		private void OnDisable()
		{
			m_locomotiveAction.LocomotiveStateChanged -= FootstepAudio_LocomotiveStateChanged;
			m_locomotiveImpulseGenerator.LocomotiveImpulseGenerated -= FootstepAudio_LocomotiveImpulseGenerated;
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