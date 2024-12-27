using System.Collections;

using Cinemachine;
using UnityEngine;

namespace BM
{

	[DisallowMultipleComponent]
	[RequireComponent(typeof(LocomotiveAction))]
	public class CameraNoiseHandler : MonoBehaviour
	{
		[SerializeField] private CinemachineBasicMultiChannelPerlin m_noiseTarget;
		[SerializeField] private LocomotivePropertySO m_locomotiveProperty;

		[Header("Transition Settings")]
		[Space()]

		[SerializeField] private float m_duration = .4f;

		private LocomotiveAction m_locomotiveAction;
		private Coroutine m_transitionRoutine;

		private void CameraNoiseHandler_LocomotiveStateChanged(LocomotiveAction.State state)
		{
			float amplitudeGainInState = m_locomotiveProperty.GetCameraNoisePropertyByState(state).AmplitudeGain;
			float frequencyGainInState = m_locomotiveProperty.GetCameraNoisePropertyByState(state).FrequencyGain;

			if (null != m_transitionRoutine)
			{
				StopCoroutine(m_transitionRoutine);
				m_transitionRoutine = null;
			}

			m_transitionRoutine = StartCoroutine(TransitionRoutine(amplitudeGainInState, frequencyGainInState));

		}

		private IEnumerator TransitionRoutine(float targetAmplitudeGain, float targetFrequencyGain)
		{
			float elapsedTime = 0f;

			float startAmp = m_noiseTarget.m_AmplitudeGain;
			float startFreq = m_noiseTarget.m_FrequencyGain;

			while (elapsedTime < m_duration)
			{
				elapsedTime += Time.deltaTime;
				float ratio = Mathf.Clamp01(elapsedTime / m_duration);

				m_noiseTarget.m_AmplitudeGain = Mathf.Lerp(startAmp, targetAmplitudeGain, ratio);
				m_noiseTarget.m_FrequencyGain = Mathf.Lerp(startFreq, targetFrequencyGain, ratio);

				yield return null;
			}

			m_noiseTarget.m_AmplitudeGain = targetAmplitudeGain;
			m_noiseTarget.m_FrequencyGain = targetFrequencyGain;

			yield break;
		}

		private void Awake()
		{
			m_locomotiveAction = GetComponent<LocomotiveAction>();
		}

		private void OnEnable()
		{
			m_locomotiveAction.LocomotiveStateChanged += CameraNoiseHandler_LocomotiveStateChanged;
		}

		private void OnDisable()
		{
			m_locomotiveAction.LocomotiveStateChanged -= CameraNoiseHandler_LocomotiveStateChanged;
		}
	}
}