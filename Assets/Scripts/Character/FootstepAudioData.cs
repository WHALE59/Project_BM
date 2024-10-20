using System.Collections.Generic;

using UnityEngine;

namespace BM
{
	[CreateAssetMenu(fileName = "FootstepAudioData_Default", menuName = "BM/Data/Footstep Audio Data")]
	public class FootstepAudioData : ScriptableObject
	{
		[SerializeField] List<AudioClip> _fallback = new();

		public AudioClip GetProperFootstepClip(/* Context, */)
		{
			return _fallback[Random.Range(0, _fallback.Count)];
		}
	}
}