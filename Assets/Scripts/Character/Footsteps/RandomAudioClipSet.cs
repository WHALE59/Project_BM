using System.Collections.Generic;

using UnityEngine;

namespace BM
{
	/// <summary>
	/// 여러 오디오 클립 중 하나를 랜덤하게 뽑는다.
	/// </summary>
	[CreateAssetMenu(menuName = "BM/Data/Random Audio Clip Set", fileName = "RandomAudioClipSet_Default")]
	public class RandomAudioClipSet : ScriptableObject
	{
		[Header("오디오 클립 세트 할당")]
		[Space()]

		[Tooltip("여기에 할당된 오디오 클립 중 랜덤으로 하나를 뽑아 재생할 수 있습니다.")]
		[SerializeField] private List<AudioClip> m_clips = new();


		[Header("랜덤 설정")]
		[Space()]

		[SerializeField] private bool m_allowPickingSameClipInARow = false;
		[SerializeField] private uint m_maxPickingTrialCount = 3;

		private AudioClip m_previousPickedClip = null;

		public AudioClip PickClip()
		{
			if (m_clips.Count == 0)
			{
				return null;
			}

			AudioClip picked = null;

			var trialCount = 1u;

			// 뽑아보고 이전 시도와 같은 것을 뽑았으면 시도 횟수를 채울 때 까지 계속 다시 뽑기
			do
			{
				picked = m_clips[Random.Range(0, m_clips.Count)];
				trialCount++;

				if (trialCount == m_maxPickingTrialCount)
				{
					break;
				}

			} while (picked == m_previousPickedClip && !m_allowPickingSameClipInARow);

			m_previousPickedClip = picked;

			return picked;
		}
	}
}