using UnityEngine;

namespace BM
{
	public class FootstepBaseAudioVolume : FootstepAudioVolume
	{
		[SerializeField] private bool m_overrideWhenTriggerExit = false;

		private void OnTriggerEnter(Collider collider)
		{
			if (!m_footstepAudioClipSet)
			{
				return;
			}

			if (!collider.TryGetComponent<FootstepAudio>(out var footstepAudio))
			{
				return;
			}

			footstepAudio.FootstepBaseAudioData = m_footstepAudioClipSet;
		}

		private void OnTriggerExit(Collider collider)
		{
			if (!m_footstepAudioClipSet)
			{
				return;
			}

			if (m_overrideWhenTriggerExit)
			{
				return;
			}

			if (!collider.TryGetComponent<FootstepAudio>(out var footstepAudio))
			{
				return;
			}

			footstepAudio.FootstepBaseAudioData = null;
		}
	}
}