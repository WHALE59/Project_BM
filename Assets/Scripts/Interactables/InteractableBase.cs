using UnityEngine;

namespace BM.Interactables
{
	/// <summary>
	/// 모든 상호작용 가능한 객체는 이 클래스에서 파생되어야 한다.
	/// </summary>
	[SelectionBase]
	[DisallowMultipleComponent]
	public class InteractableBase : MonoBehaviour
	{
		[Header("상호작용 속성 설정")]
		[Space()]

		[Tooltip("이 상호작용 객체는 손에 들 수 있고, 레벨에 배치할 수 있습니다.")]
		[SerializeField] private bool m_isEquipAndPlaceable;
		[Tooltip("이 상호작용 객체는 인벤토리에 넣을 수 있다.")]
		[SerializeField] private bool m_isCollectible;

		[Tooltip("이 상호작용 객체는 고유의 활성화 기능이 있다.")]
		[SerializeField] private bool m_isActivatable;

		[Tooltip("이 상호작용 객체는 다른 상호작용 대상을 트리거하는 주체가 될 수 있다. 예) 자물쇠 A를 여는 열쇠 A")]
		[SerializeField] private bool m_isUsable;
		[Tooltip("이 상호작용 객체는 다른 상호작용 대상에 의해 트리거 되는 객체가 될 수 있다. 예) 열쇠 A에 의해 열리는 자물쇠 A")]
		[SerializeField] private bool m_isUsedable;

		[SerializeField] private Material m_materialOnPlacement;

		[Tooltip("메쉬와 머터리얼, 콜라이더가 존재하는 이 상호작용 오브젝트의 프리팹이 들어있는 자식 오브젝트를 할당합니다.")]
		[SerializeField] private InteractiveModel m_modelChild;

		private InteractablePlacement m_placement;

		public bool IsEquipAndPlaceable => m_isEquipAndPlaceable;
		public bool IsCollectible => m_isCollectible;
		public bool IsActivatable => m_isActivatable;
		public bool IsUsable => m_isUsable;
		public bool IsUsedable => m_isUsedable;

		private void Start()
		{
			m_placement = CreatePlacement();
			m_placement.Disable();
		}

		private InteractablePlacement CreatePlacement()
		{
			GameObject rootObject = new($"{name}_Placement");

			InteractablePlacement placement = rootObject.AddComponent<InteractablePlacement>();

			placement.GenerateVisualChildren(m_modelChild.MeshDatas, m_materialOnPlacement);
			placement.GenerateTriggerChildren(m_modelChild.Colliders);

			placement.transform.SetPositionAndRotation(transform.position, transform.rotation);
			placement.transform.SetParent(transform);

			return placement;
		}

		public void StartHovering()
		{
			// Start Hovering Effect
		}

		public void FinishHovering()
		{
			// Finish Hovering Effect 
		}
	}
}