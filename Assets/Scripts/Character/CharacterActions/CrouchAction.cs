using System;
using System.Collections;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BM
{
	/// <summary>
	/// 캐릭터의 앉기 동작을 수행한다. 앉기의 구분동작에 맞게 캡슐의 크기를 바꾼다.
	/// 캡슐 크기 변화하면 이벤트를 발생시켜, 크기 변화 정보가 필요한 구독자들에게 변화 사실을 알린다.
	/// 앉기 시작 / 끝 이벤트를 발생시켜, 이 정보가 필요한 구독자들에게 알린다.
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(CharacterController))]
	public class CrouchAction : MonoBehaviour
	{
		public event Action<Vector3, float, float> CapsuleSizeChanged;

		// 아직 State 머신 같은게 없으므로, 일단은 임시로 이렇게 처리하는 것으로 합시다!
		public event Action CrouchActionStarted;
		public event Action CrouchActionFinished;

		void Awake()
		{
			_characterController = GetComponent<CharacterController>();

			_normalCapsuleHeight = _characterController.height;
			_normalCapsuleCenter = _characterController.center;
		}

		void OnValidate()
		{
			if (_characterController)
			{
				_normalCapsuleHeight = _characterController.height;
				_normalCapsuleCenter = _characterController.center;
			}
		}

		void Start()
		{
			CapsuleSizeChanged?.Invoke(_characterController.center, _characterController.height, _characterController.radius);
		}

		public void StartCrouch()
		{
			CrouchActionStarted?.Invoke();

			StopAllCoroutines();

#if UNITY_EDITOR
			_isStucked = false;
#endif

			StartCoroutine(CrouchRoutine(up: false));
		}

		public void FinishCrouch()
		{
			CrouchActionFinished?.Invoke();

			StopAllCoroutines();

			StartCoroutine(CrouchRoutine(up: true));
		}

		/// <param name="up">
		/// 일어 서 있다가 앉는 루틴이면 false, 앉아 있다가 일어서는 루틴이면 true
		/// </param>
		IEnumerator CrouchRoutine(bool up = false)
		{
			// 초기화
			var startCapsuleHeight = _characterController.height;
			var startCapsuleCenter = _characterController.center;

			var endCapsuleHeight = up ? _normalCapsuleHeight : CrouchedCapsuleHeight;
			var endCapsuleCenter = up ? _normalCapsuleCenter : CrouchedCapsuleCenter;

			var elapsedTime = 0.0f;

			// 앉기 구분동작 타이머
			while (elapsedTime < _crouchSpeed)
			{
				// 지금 앉았다가 일어서는 중인데, 일어서는 경로에 장애물이 있는지 체크.
				while (up && IsStucked)
				{
					yield return new WaitForFixedUpdate();
				}

				elapsedTime += Time.deltaTime;

				var ratio = elapsedTime / _crouchSpeed;

				_characterController.height = Mathf.Lerp(startCapsuleHeight, endCapsuleHeight, ratio);
				_characterController.center = Vector3.Lerp(startCapsuleCenter, endCapsuleCenter, ratio);

				CapsuleSizeChanged?.Invoke(_characterController.center, _characterController.height, _characterController.radius);

				yield return new WaitForFixedUpdate();
			}

			_characterController.height = endCapsuleHeight;
			_characterController.center = endCapsuleCenter;

			CapsuleSizeChanged?.Invoke(_characterController.center, _characterController.height, _characterController.radius);
		}

#if UNITY_EDITOR
		/// <summary>
		/// 일어서다가 천장에 부딪히면, 부딪힌 지점을 표시한다.
		/// </summary>
		[DrawGizmo(GizmoType.Active | GizmoType.Selected)]
		static void DrawStuckedGizmo(CrouchAction target, GizmoType _)
		{
			if (!target._isStucked)
			{
				return;
			}

			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(target._hitInfo.point, 0.1f);
		}

#endif
		float CrouchedCapsuleHeight => _normalCapsuleHeight * _crouchedRatio;
		Vector3 CrouchedCapsuleCenter
		{
			get
			{
				var netYPosOffset = _normalCapsuleHeight * (1 - _crouchedRatio) / 2.0f;
				return _normalCapsuleCenter - new Vector3(0.0f, netYPosOffset, 0.0f);
			}
		}

		CharacterController _characterController;

		float _normalCapsuleHeight;
		Vector3 _normalCapsuleCenter;
		Vector3 _normalVcamLocalPosition;

		// TODO: 현재 임시로 "캐릭터(자기 자신)를 제외한 모든 레이어"로 설정해 두었음
		int _layerMask = ~(1 << 3);

#if UNITY_EDITOR
		bool _isStucked = false;
#endif

		bool IsStucked
		{
			get
			{
				return
#if UNITY_EDITOR
				_isStucked =
#endif
				Physics.Raycast(
							transform.position,
							transform.up,
#if UNITY_EDITOR
							out _hitInfo,
#endif
							_normalCapsuleHeight / 2.0f,
							_layerMask
				);
			}
		}

#if UNITY_EDITOR
		RaycastHit _hitInfo;
#endif

		[Tooltip("앉기 동작을 수행하는 총 시간(초)")]
		[SerializeField] float _crouchSpeed = 0.25f;

		[Tooltip("서 있는 키에 대한, 앉은 키의 비율")]
		[SerializeField][Range(0.0f, 1.0f)] float _crouchedRatio = 0.5f;
	}
}
