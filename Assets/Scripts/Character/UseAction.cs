using BM.Interactables;
using System;
using UnityEngine;

namespace BM
{
	/// <remarks>
	/// 장착한 것은 항상 Usable 함을 가정한다.
	/// </remarks>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Inventory))]
	[RequireComponent(typeof(InteractableDetector))]
	public class UseAction : MonoBehaviour
	{
		[Header("Input Settings")]

		[SerializeField] private InputReaderSO m_inputReader;

		[Header("Editor Manual Equipment")]

		[SerializeField] private ItemSO m_equipment;

#if UNITY_EDITOR
		[Header("Debug")]

		[SerializeField] private bool m_logOnUseActionProcedure = false;
#endif

		private InteractableBase m_detectedInteractable;

		public event Action<ItemSO> Equipped;
		public event Action<ItemSO> Unequipped;
		public event Action<ItemSO> Used;

		private InteractableDetector m_detector;
		private Inventory m_inventory;

		private void UseAction_UseInputPerformed()
		{
			if (null == m_detectedInteractable)
			{
#if UNITY_EDITOR
				if (m_logOnUseActionProcedure)
				{
					Debug.Log("Use의 대상이 감지되지 않았습니다.");
				}
#endif
				return;
			}

			if (null == m_equipment) // TODO Usable 에 대해 고려 해야하는지?
			{
#if UNITY_EDITOR
				if (m_logOnUseActionProcedure)
				{
					Debug.Log("아무 것도 장비하고 있지 않습니다.");
				}
#endif
				return;
			}

			if (!m_detectedInteractable.IsUsedable)
			{
#if UNITY_EDITOR
				if (m_logOnUseActionProcedure)
				{
					Debug.Log($"{m_detectedInteractable.name}은 Usedable하지 않습니다.");
				}
#endif
				return;
			}

			if (!m_equipment.IsUsedTo(m_detectedInteractable))
			{
#if UNITY_EDITOR
				if (m_logOnUseActionProcedure)
				{
					Debug.Log($"{m_equipment.name}는 {m_detectedInteractable.name}에 사용할 수 없습니다.");
				}
#endif
				return;
			}

			if (!m_detectedInteractable.IsUsedBy(m_equipment))
			{
#if UNITY_EDITOR
				if (m_logOnUseActionProcedure)
				{
					Debug.Log($"{m_detectedInteractable.name}은 {m_equipment.name}에 의해 사용되지 않습니다.");
				}
#endif
				return;
			}

			// Use 시작

			m_detectedInteractable.StartUse(this, m_equipment);

			Used?.Invoke(m_equipment);
			m_equipment = null;
		}

		private void UseAction_TraverseEquipmentInputEvent()
		{
			ItemSO usable = m_inventory.TryGetNextUsableItem();

			if (null != usable)
			{
				m_equipment = usable;
				Equipped?.Invoke(m_equipment);
			}
			else
			{
				// 아무것도 없어서 장비할 수가 없는 상태
				Unequipped?.Invoke(m_equipment);
				m_equipment = null;
			}
		}

		private void UseAction_UnequipInputEvent()
		{
			if (null == m_equipment)
			{
				return;
			}

			Unequipped?.Invoke(m_equipment);
			m_equipment = null;
		}

		private void UseAction_InteractableFound(InteractableBase found)
		{
			m_detectedInteractable = found;
		}

		private void UseAction_InteractableLost(InteractableBase lost)
		{
			m_detectedInteractable = null;
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (null != m_equipment)
			{
				Equipped?.Invoke(m_equipment);
			}
			else
			{
				Unequipped?.Invoke(m_equipment);
			}
		}
#endif

		private void Awake()
		{
			m_detector = GetComponent<InteractableDetector>();
			m_inventory = GetComponent<Inventory>();
		}

		private void OnEnable()
		{
			m_inputReader.UseInputPerformed += UseAction_UseInputPerformed;

			m_inputReader.UnequipInputEvent += UseAction_UnequipInputEvent;
			m_inputReader.TraverseEquipmentInputEvent += UseAction_TraverseEquipmentInputEvent;

			m_detector.InteractableFound += UseAction_InteractableFound;
			m_detector.InteractableLost += UseAction_InteractableLost;
		}

		private void OnDisable()
		{
			m_inputReader.UseInputPerformed -= UseAction_UseInputPerformed;

			m_inputReader.UnequipInputEvent -= UseAction_UnequipInputEvent;
			m_inputReader.TraverseEquipmentInputEvent -= UseAction_TraverseEquipmentInputEvent;

			m_detector.InteractableFound -= UseAction_InteractableFound;
			m_detector.InteractableLost -= UseAction_InteractableLost;
		}
	}
}