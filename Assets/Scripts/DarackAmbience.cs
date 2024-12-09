using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	public class DarackAmbience : MonoBehaviour
	{
		[SerializeField] private EventReference m_eventReference;
		private EventInstance m_instance;

		private void Awake()
		{
			m_instance = RuntimeManager.CreateInstance(m_eventReference);
		}

		private void Start()
		{

		}

		private void Update()
		{

		}
	}
}
