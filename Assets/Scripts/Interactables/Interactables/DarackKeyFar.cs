using UnityEngine;

namespace BM.Interactables
{
	[RequireComponent(typeof(Animator))]
	public class DarackKeyFar : InteractableBase
	{
		[SerializeField] private InteractableBase m_DarackKeyNearPrefab;

		private Animator m_animator;
		public override void StartActivation(InteractAction _)
		{
			base.StartActivation(_);

			// TODO: "열쇠가 멀리 있어서 집을 수 없다" 라는 정보를 대사, UI 등으로 플레이어에게 전달
		}

		/// <summary>
		/// 애니메이션이 시작되면 <see cref="InteractableBase.DisallowInteraction"/>이 호출되어, 
		/// 이 객체가 파괴되기 전 까지 상호작용을 불허한다.
		/// </summary>
		public override void StartUsage(UseAction _0, InteractableSO _1)
		{
			base.StartUsage(_0, _1);

			m_animator.SetTrigger("Move");
		}

		/// <summary>
		/// 이동 애니메이션의 끝 프레임에서 호출되는 애니메이션 이벤트로, 이 스크립트가 부착된 오브젝트를 제거하고, 
		/// Collectible한 Key를 동일한 트랜스폼으로 인스턴스화 한다.
		/// </summary>
		public void OnMovingAnimationEnd()
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