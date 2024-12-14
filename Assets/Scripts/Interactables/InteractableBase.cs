using UnityEngine;

namespace BM.Interactables
{
	/// <summary>
	/// 모든 상호작용 가능한 객체는 이 클래스에서 파생되어야 한다.
	/// </summary>
	/// <remarks>
	/// <see cref="InteractableModel"/>과 <see cref="InteractablePlacement"/>는 스스로는 아무것도 하지 않고, 이 클래스의 명령만을 수행한다.
	/// </remarks>
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

		[Header("기타 설정")]
		[Space()]

		[SerializeField] private Material m_materialOnPlacement;

		[Tooltip("메쉬와 머터리얼, 콜라이더가 존재하는 이 상호작용 오브젝트의 프리팹이 들어있는 자식 오브젝트를 할당합니다.")]
		[SerializeField] private InteractableModel m_modelChild;

		private InteractablePlacement m_placement;

		public bool IsEquipPlaceable => m_isEquipAndPlaceable;
		public bool IsCollectible => m_isCollectible;
		public bool IsActivatable => m_isActivatable;
		public bool IsUsable => m_isUsable;
		public bool IsUsedable => m_isUsedable;

		private void Awake()
		{
			m_modelChild.InitializeMeshElements();
			m_modelChild.InitializeColliders();

			m_modelChild.gameObject.layer = 6;

			m_placement = GeneratePlacementChild(m_modelChild, m_materialOnPlacement);
			m_placement.Disable();

			m_placement.gameObject.layer = 7;
		}

		/// <summary>
		/// <paramref name="model"/>과 <paramref name="material"/>를 기반으로 하여 Placement시 사용될 홀로그램 모델을 자식 오브젝트로 생성한다.
		/// </summary>
		private InteractablePlacement GeneratePlacementChild(InteractableModel model, Material material)
		{
			GameObject rootPlacementObject = new($"{model.gameObject.name}");

			rootPlacementObject.name = rootPlacementObject.name.Replace("Model", "Placement"); // TODO: 빌드 버전에는 이름이 필요하지는 않음
			rootPlacementObject.name += "(Generated)";

			InteractablePlacement placement = rootPlacementObject.AddComponent<InteractablePlacement>();

			placement.GenerateVisualChildren(model.MeshDatas, material);
			placement.GenerateTriggerChildren(model.Colliders);

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