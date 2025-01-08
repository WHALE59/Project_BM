using UnityEngine;

namespace BM.Interactables
{
	[RequireComponent(typeof(Animator))]
	public class DarackKeyFar : InteractableBase
	{
		[SerializeField] private InteractableBase m_DarackKeyNearPrefab;

		private Animator m_animator;

		public override void StartInteract(InteractAction _)
		{
			base.StartInteract(_);

			// TODO: "열쇠가 멀리 있어서 집을 수 없다" 라는 정보를 대사, UI 등으로 플레이어에게 전달
		}

		public override void StartUse(UseAction _0, ItemSO _1)
		{
			base.StartUse(_0, _1);

			DisallowInteraction();
			// 불허

			m_animator.SetTrigger("Move");
		}

		/// <summary>
		/// 이동 애니메이션의 끝 프레임에서 호출되는 애니메이션 이벤트로, 이 스크립트가 부착된 오브젝트를 제거하고, 
		/// Collectible한 Key를 동일한 트랜스폼으로 인스턴스화 한다.
		/// </summary>
		public void DarackKeyFar_MovingAnimationEnd()
		{
			Instantiate(m_DarackKeyNearPrefab, position: transform.position, rotation: transform.rotation);
			Destroy(gameObject);
		}

		protected override void Awake()
		{
			base.Awake();

			m_animator = GetComponent<Animator>();
		}
	}
}