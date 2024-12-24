using System.Collections;
using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	public class FlickeringLightBulb : MonoBehaviour
	{
		[SerializeField] private Light m_lightSource;

		[Header("Waving Light Settings")]
		[Space()]

		[SerializeField] private float m_wavingPeriod;

		[Space()]
		[SerializeField] private float m_minWavingIntensity;
		[SerializeField] private float m_maxWavingIntensity;

		[Header("Flickering Light Settings")]
		[Space()]

		[SerializeField] private float m_minFlickeringInterval;
		[SerializeField] private float m_maxFlickeringInterval;

		[Space()]

		[SerializeField] private float m_minLightIntensity;
		[SerializeField] private float m_maxLightIntensity;

		private float m_wavingElapsedTime = 0f;
		private bool m_wavingUp = true;

		private void Start()
		{
			m_lightSource.intensity = m_minWavingIntensity;
		}

		private void Update()
		{
		}
	}
}
