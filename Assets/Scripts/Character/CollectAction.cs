using BM.Interactables;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(InteractableDetector))]
	[RequireComponent(typeof(UseAction))]
	public class CollectAction : MonoBehaviour
	{
		[Header("Input Settings")]
		[Space]

		[SerializeField] private InputReaderSO m_inputReaderSO;

		[Header("Sound Settings")]
		[Space]

		[SerializeField] private EventReference m_defaultSoundOnCollect;

		[Header("Editor Manual Inventory")]
		[Space]

		[SerializeField] private List<InteractableSO> m_collectedSOs = new();


		private InteractableDetector m_detector;
		private UseAction m_useAction;

		private int m_currentIndex = -1;

		public void PutIn(InteractableBase collectible)
		{
			m_collectedSOs.Add(collectible.InteractableSO);
		}

		/// <summary>
		/// 인덱스를 기반으로 인벤토리에 존재하는 다음 사용 가능한 아이템을 반환한다. 만일 인벤토리 리스트의 끝에 도달했거나, 인벤토리가 비어 있으면 널을 반환한다.
		/// </summary>
		public InteractableSO TryGetNextUsableItem()
		{
			if (m_collectedSOs.Count == 0)
			{
				return null;
			}

			while (true)
			{
				m_currentIndex++;

				if (m_currentIndex >= m_collectedSOs.Count)
				{
					m_currentIndex = -1;
					return null;
				}

				if (m_collectedSOs[m_currentIndex].IsUsable)
				{
					return m_collectedSOs[m_currentIndex];
				}
			}
		}

		private void CollectAction_Unequipped(InteractableSO unequipped)
		{
			if (m_currentIndex == -1)
			{
				return;
			}

			--m_currentIndex;
		}

		private void CollectAction_Used(InteractableSO used)
		{
			m_collectedSOs.Remove(used);

			--m_currentIndex;
		}

		private void CollectAction_CollectOrActivateInputPerformed()
		{
			InteractableBase detected = m_detector.DetectedInteractable;

			if (null == detected)
			{
				return;
			}

			if (detected.IsCollectible && !detected.IsActivatable)
			{
				m_collectedSOs.Add(detected.InteractableSO);

				EventReference soundOnCollect = detected.InteractableSO.SoundOnCollectingOverride;

				if (soundOnCollect.IsNull)
				{
					RuntimeManager.PlayOneShot(m_defaultSoundOnCollect);
				}
				else
				{
					RuntimeManager.PlayOneShot(soundOnCollect);
				}

				detected.SetCollected();

				m_detector.DetectedInteractableGone();
			}
		}

		private void CollectAction_CollectOrActivateInputCanceled()
		{

		}

		private void Awake()
		{
			m_detector = GetComponent<InteractableDetector>();
			m_useAction = GetComponent<UseAction>();
		}

		private void OnEnable()
		{
			m_inputReaderSO.CollectOrActivateInputPerformed += CollectAction_CollectOrActivateInputPerformed;
			m_inputReaderSO.CollectOrActivateInputCanceled += CollectAction_CollectOrActivateInputCanceled;

			m_useAction.Unequipped += CollectAction_Unequipped;
			m_useAction.Used += CollectAction_Used;
		}

		private void OnDisable()
		{
			m_inputReaderSO.CollectOrActivateInputPerformed -= CollectAction_CollectOrActivateInputPerformed;
			m_inputReaderSO.CollectOrActivateInputCanceled -= CollectAction_CollectOrActivateInputCanceled;

			m_useAction.Unequipped -= CollectAction_Unequipped;
			m_useAction.Used -= CollectAction_Used;
		}
	}
}