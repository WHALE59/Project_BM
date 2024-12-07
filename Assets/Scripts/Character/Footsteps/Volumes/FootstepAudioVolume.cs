using System.Collections.Generic;

using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	public class FootstepAudioVolume : MonoBehaviour
	{
		private HashSet<FootstepAudio> m_triggeredFootstepAudioPlayers = new();

		private void OnTriggerEnter(Collider collider)
		{
			if (!collider.TryGetComponent<FootstepAudio>(out var footstepAudioPlayer))
			{
				return;
			}

			m_triggeredFootstepAudioPlayers.Add(footstepAudioPlayer);
		}

		private void OnTriggerExit(Collider collider)
		{
			if (!collider.TryGetComponent<FootstepAudio>(out var footstepAudioPlayer))
			{
				// Something went truly wrong...
			}

			m_triggeredFootstepAudioPlayers.Remove(footstepAudioPlayer);
		}
	}
}