using BM.Interactables;
using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(InteractableDetector))]
	public class ActivateAction : MonoBehaviour
	{
		[Header("Input Settings")]
		[Space]

		[SerializeField] private InputReaderSO m_inputReader;

		private InteractableDetector m_detector;

		private void ActivateAction_CollectOrActivateInputPerformed()
		{
			InteractableBase detected = m_detector.DetectedInteractable;

			if (null == detected)
			{
				return;
			}

			if (detected.IsActivatable && !detected.IsCollectible)
			{
				detected.StartActivation(this);
			}
		}

		private void ActivateAction_CollectOrActivateInputCanceled()
		{
		}

		private void Awake()
		{
			m_detector = GetComponent<InteractableDetector>();
		}

		private void OnEnable()
		{
			m_inputReader.CollectOrActivateInputPerformed += ActivateAction_CollectOrActivateInputPerformed;
			m_inputReader.CollectOrActivateInputCanceled += ActivateAction_CollectOrActivateInputCanceled;
		}

		private void OnDisable()
		{
			m_inputReader.CollectOrActivateInputPerformed -= ActivateAction_CollectOrActivateInputPerformed;
			m_inputReader.CollectOrActivateInputCanceled -= ActivateAction_CollectOrActivateInputCanceled;
		}
	}
}
