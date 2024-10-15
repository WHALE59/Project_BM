using System;

using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	public class WalkAction : MonoBehaviour
	{
		public Action WalkStarted;
		public Action WalkFinished;

		public void StartWalk()
		{
			WalkStarted?.Invoke();
		}

		public void FinishWalk()
		{
			WalkFinished?.Invoke();
		}
	}
}
