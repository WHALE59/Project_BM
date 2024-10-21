using System.Collections.Generic;

using UnityEngine;

namespace BM
{
	[CreateAssetMenu(fileName = "FootstepAudioData_Default", menuName = "BM/Data/Footstep Audio Data")]
	public class FootstepAudioData : ScriptableObject
	{
		[SerializeField] List<AudioClip> _data = new();

		public AudioClip GetProperFootstepClip(/* Context, */)
		{
			return _data[Random.Range(0, _data.Count)];
		}
	}
}