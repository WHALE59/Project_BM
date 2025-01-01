using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Collider))]
	public class FootstepAudioVolume : MonoBehaviour
	{
		[SerializeField] private FootstepAudio.FloorMaterialType m_materialType;

		private void OnTriggerEnter(Collider collider)
		{
			if (!collider.TryGetComponent(out FootstepAudio footstepAudioPlayer))
			{
				return;
			}

			footstepAudioPlayer.SetFloorMaterial(m_materialType);
		}

		private void OnTriggerExit(Collider collider)
		{
			if (!collider.TryGetComponent(out FootstepAudio footstepAudioPlayer))
			{
				// if code go through this, something went truly wrong...
			}

			footstepAudioPlayer.SetDefaultFloorMaterial();
		}
	}
}