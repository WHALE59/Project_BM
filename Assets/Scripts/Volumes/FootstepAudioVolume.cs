using System.Collections.Generic;

using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	public class FootstepAudioVolume : MonoBehaviour
	{
		[SerializeField] private FootstepAudio.FloorMaterialType m_materialType;

		private Collider m_collider;

		private void Awake()
		{
			m_collider = GetComponent<Collider>();
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!collider.TryGetComponent<FootstepAudio>(out var footstepAudioPlayer))
			{
				return;
			}

			footstepAudioPlayer.SetFloorMaterial(m_materialType);
		}

		private void OnTriggerExit(Collider collider)
		{
			if (!collider.TryGetComponent<FootstepAudio>(out var footstepAudioPlayer))
			{
				// if code go through this, something went truly wrong...
			}

			footstepAudioPlayer.SetDefaultFloorMaterial();
		}
	}
}