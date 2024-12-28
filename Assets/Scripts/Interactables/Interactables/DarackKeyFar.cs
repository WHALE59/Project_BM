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

		}

		public override void StartUsage(InteractAction _0, InteractableSO _1)
		{
			base.StartUsage(_0, _1);
			m_animator.SetTrigger("Move");
		}

		protected override void Awake()
		{
			base.Awake();

			m_animator = GetComponent<Animator>();
		}

		public void OnMovingAnimationEnd()
		{
			InteractableBase darackKeyNear = Instantiate(m_DarackKeyNearPrefab);
			darackKeyNear.transform.SetPositionAndRotation(transform.position, transform.rotation);

			Destroy(gameObject);
		}
	}
}