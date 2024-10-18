using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	public class HeadPositionSetter : MonoBehaviour
	{
		CharacterController _characterController;
		CrouchAction _crouchAction;

		void Awake()
		{
			// 캐릭터의 루트 오브젝트에 CrouchAction이 존재한다고 가정
			_crouchAction = transform.root.GetComponent<CrouchAction>();

#if UNITY_EDITOR
			if (!_crouchAction)
			{
				Debug.LogWarning($"가상 카메라의 Follow Target 위치를 초기화 할 수 없습니다. {gameObject}의 루트 오브젝트({transform.root.gameObject})에서 CrouchAction 컴포넌트를 찾을 수 없습니다.");
				return;
			}
#endif
		}

		void OnEnable()
		{
			_crouchAction.CapsuleSizeChanged += OnCapsuleSizeChanged;
		}

		void OnDisable()
		{

			_crouchAction.CapsuleSizeChanged -= OnCapsuleSizeChanged;
		}

		void OnCapsuleSizeChanged(Vector3 center, float height, float radius)
		{
			var targetLocalPosition = center;
			targetLocalPosition.y += (height - radius) / 2.0f;

			transform.localPosition = targetLocalPosition;
		}
	}
}
