using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(BoxCollider))]
	public class FoostepAudioVolume : MonoBehaviour
	{
		[Tooltip("캐릭터가 볼륨 안에 진입했을 때, 재생할 발소리 정보입니다.")]
		[SerializeField] FootstepAudioData _footstepAudioData;

		[Tooltip("캐릭터가 볼륨을 빠져 나갔을 때, 지정한 발소리를 잃을지에 대한 여부입니다.")]
		[SerializeField] bool _doNotLoseDataWhenPlayerGoOutside = false;

		void OnTriggerEnter(Collider other)
		{
			if (!other.TryGetComponent<IFootstepAudioPlayer>(out var footstepAudioPlayer))
			{
				return;
			}

			footstepAudioPlayer.FootstepAudioData = _footstepAudioData;
		}

		void OnTriggerExit(Collider other)
		{
			if (!other.TryGetComponent<IFootstepAudioPlayer>(out var footstepAudioPlayer))
			{
				return;
			}

			if (_doNotLoseDataWhenPlayerGoOutside)
			{
				return;
			}

			footstepAudioPlayer.FootstepAudioData = null;
		}

#if UNITY_EDITOR
		BoxCollider _boxCollider;

		[Header("기즈모 설정")]
		[Space()]

		[SerializeField] Color _gizmoColor = Color.cyan;
		[SerializeField] float _gizmoAlpha = 0.3f;

		void Awake()
		{
			_boxCollider = GetComponent<BoxCollider>();
		}

		void OnDrawGizmos()
		{
			if (!_boxCollider)
			{
				_boxCollider = GetComponent<BoxCollider>();
			}

			// 외곽선

			Gizmos.color = (!_doNotLoseDataWhenPlayerGoOutside ? _gizmoColor : Color.magenta) - new Color(0.0f, 0.0f, 0.0f, 1 - _gizmoAlpha);
			Gizmos.DrawWireCube(transform.position + _boxCollider.center, _boxCollider.bounds.extents * 2);

			// 내부

			Gizmos.DrawCube(transform.position + _boxCollider.center, _boxCollider.bounds.extents * 2);

			// 데이터 이름

			UnityEditor.Handles.Label(transform.position + _boxCollider.center, _footstepAudioData.name);
		}
#endif
	}
}