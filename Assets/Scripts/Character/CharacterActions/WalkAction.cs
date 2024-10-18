using System;

using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(InputListener))]
	public class WalkAction : MonoBehaviour
	{
		public Action WalkStateStarted;
		public Action WalkStateFinished;

		InputListener _inputListener;

		void Awake()
		{
			_inputListener = GetComponent<InputListener>();
		}

		void OnEnable()
		{
			_inputListener.WalkStarted += StartWalk;
			_inputListener.WalkFinished += FinishWalk;
		}

		void OnDisable()
		{
			_inputListener.WalkStarted -= StartWalk;
			_inputListener.WalkFinished -= FinishWalk;
		}

		void StartWalk()
		{
			WalkStateStarted?.Invoke();
		}

		void FinishWalk()
		{
			WalkStateFinished?.Invoke();
		}
	}
}
