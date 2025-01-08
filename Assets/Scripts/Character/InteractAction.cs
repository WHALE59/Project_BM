using BM.Interactables;

using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(InteractableDetector))]
	[RequireComponent(typeof(Inventory))]
	public class InteractAction : MonoBehaviour
	{
		[Header("입력 설정")]

		[SerializeField] private InputReaderSO m_inputReader;

		private InteractableDetector m_detector;
		private Inventory m_inventory;

		public void CollectItem(ItemSO item)
		{
			m_inventory.PutIn(item);
		}

		private void InteractAction_CollectOrActivateInputPerformed()
		{
			InteractableBase detected = m_detector.DetectedInteractable;

			if (null == detected)
			{
				return;
			}

			detected.StartInteract(this);
		}

		private void Awake()
		{
			m_detector = GetComponent<InteractableDetector>();
			m_inventory = GetComponent<Inventory>();
		}

		private void OnEnable()
		{
			m_inputReader.CollectOrActivateInputPerformed += InteractAction_CollectOrActivateInputPerformed;
		}

		private void OnDisable()
		{
			m_inputReader.CollectOrActivateInputPerformed -= InteractAction_CollectOrActivateInputPerformed;
		}
	}
}
