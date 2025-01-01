using FMODUnity;
using UnityEngine;
using UnityEngine.Localization;

namespace BM
{
	[CreateAssetMenu(menuName = "BM/SO/Voice Line SO", fileName = "VoiceLineSO_Default")]
	public class VoiceLineSO : ScriptableObject
	{
		[SerializeField] private EventReference m_voiceSound;
		[SerializeField] private LocalizedString m_subtitle;

		public void Play()
		{
			RuntimeManager.PlayOneShot(m_voiceSound);
		}
	}
}