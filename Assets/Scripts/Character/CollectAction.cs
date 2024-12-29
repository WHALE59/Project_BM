using BM.Interactables;
using System.Collections.Generic;
using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(UseAction))]
	public class CollectAction : MonoBehaviour
	{
		[SerializeField] private List<InteractableSO> m_collectedSOs = new();

		private UseAction m_useAction;

		public void PutIn(InteractableBase collectible)
		{
			m_collectedSOs.Add(collectible.InteractableSO);
		}

		private int m_currentIndex = -1;

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

		private void Awake()
		{
			m_useAction = GetComponent<UseAction>();
		}

		private void OnEnable()
		{
			m_useAction.Unequipped += CollectAction_Unequipped;
			m_useAction.Used += CollectAction_Used;
		}

		private void OnDisable()
		{
			m_useAction.Unequipped -= CollectAction_Unequipped;
			m_useAction.Used -= CollectAction_Used;
		}
	}
}