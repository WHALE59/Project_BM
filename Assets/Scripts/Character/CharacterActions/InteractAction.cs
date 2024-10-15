using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BM
{
	/// <summary>
	/// 캐릭터가 상호작용 가능한 오브젝트를 검출하고 그들과 통신하는 것을 관장하는 컴포넌트
	/// </summary>
	[DisallowMultipleComponent]
	public class InteractAction : MonoBehaviour
	{
		/// <summary>
		/// 매 고정 프레임마다 레이캐스트를 진행하여 오브젝트를 검출
		/// </summary>
		void FixedUpdate()
		{
			/*
			 * 호버링 기능 덕분에 단순 레이캐스트로 구현할 수 없었고, 이전 프레임에 무엇을 검출했는지 추적하는 코드가 필요했음.
			 */

			var ray = new Ray
			(
				origin: Camera.main.transform.position,
				direction: Camera.main.transform.forward
			);

			var isThereHitResultInCurrentFrame = Physics.Raycast(ray, out var resultInCurrentFrame, _maxDistance, _layerMask);

			if (isThereHitResultInCurrentFrame)
			{
				// 이번에 뭔가 검출 되었다면, 그 뭔가가 이전거랑 같은 대상인가?
				// 만일 다른 대상이면:
				//   * 이전에 호버링 하고 있었다면 호버링 종료
				//   * 이전에 상호작용 하고 있었다면 상호작용 종료

				var rootGameObjectInCurrentFrame = resultInCurrentFrame.transform.root.gameObject;

				// 이전 프레임과는 다른 것이 검출되었다면:
				if (rootGameObjectInCurrentFrame != _rootGameObjectInLastFrame)
				{
					// 이전 프레임 처리
					ResetPreviousInteractionState();

					// 현재 프레임 처리
					//   * 검출된 오브젝트가 상호작용 가능하다면, 호버링 처리하고 참조를 업데이트함
					//   * 상호작용 가능하지 않다면, 상태를 초기화

					var isThereInterface = rootGameObjectInCurrentFrame.TryGetComponent<IInteractableObject>(out var interactableObject);

					if (isThereInterface)
					{
						_interactableObject = interactableObject;
						_isHoveringInLastFrame = true;
						_isThereHitResultInLastFrame = true;
						_rootGameObjectInLastFrame = rootGameObjectInCurrentFrame;

						_interactableObject.StartHover();
					}
					else
					{
						_interactableObject = null;
						_isHoveringInLastFrame = false;
						_isThereHitResultInLastFrame = true;
						_rootGameObjectInLastFrame = rootGameObjectInCurrentFrame;
					}
				}
				else
				{
					// 같은 것이 검출되었다 ? ->  아무 것도 할 필요가 없다.
				}
			}
			else
			{
				// 이번에 아무것도 검출되지 않았다면,
				// 이전에 뭔가 상호작용 가능하던게 검출되었었나?
				// 만일 검출 되었으면:
				//   이전에 호버링 하고 있었다면 호버링 종료
				//   이전에 상호작용 하고 있었다면 상호작용 종료

				ResetPreviousInteractionState();

				// 현재 프레임 처리
				_interactableObject = null;
				_isThereHitResultInLastFrame = false;
				_isHoveringInLastFrame = false;
				_isInteractingInLastFrame = false;
				_rootGameObjectInLastFrame = null;
			}

#if UNITY_EDITOR
			_hitResultInLastFrame = resultInCurrentFrame;
#endif
		}

		void ResetPreviousInteractionState()
		{
			if (_isHoveringInLastFrame)
			{
				_interactableObject?.FinishHovering();
			}

			if (_isInteractingInLastFrame)
			{
				_interactableObject?.FinishInteract();
			}
		}

		public void StartInteraction()
		{
			if (_isInteractingInLastFrame)
			{
				return;
			}

			if (!_isHoveringInLastFrame)
			{
				return;
			}

			if (!_isThereHitResultInLastFrame)
			{
				return;
			}

			_isInteractingInLastFrame = true;
			_interactableObject?.Startnteract();
		}

		public void FinishInteraction()
		{
			_isInteractingInLastFrame = false;
			_interactableObject?.FinishInteract();
		}

#if UNITY_EDITOR

		/// <summary>
		/// Raycast 경로를 그린다.
		/// </summary>
		[DrawGizmo(GizmoType.Active | GizmoType.Selected)]
		static void DrawInteractionRayGizmo(InteractAction target, GizmoType _)
		{
			if (!target._isThereHitResultInLastFrame)
			{
				Gizmos.color = Color.white;
			}
			else if (target._isThereHitResultInLastFrame && !target._isHoveringInLastFrame)
			{
				Gizmos.color = Color.yellow;
			}
			else if (target._isThereHitResultInLastFrame && target._isHoveringInLastFrame)
			{
				Gizmos.color = Color.green;
			}

			Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * target._maxDistance);
		}

		/// <summary>
		/// Raycast 결과 중, InteractionTarget으로 검출된 것에 검출 결과를 그린다.
		/// </summary>
		[DrawGizmo(GizmoType.Active | GizmoType.Selected)]
		static void DrawRaycastResult(InteractAction target, GizmoType _)
		{
			if (!target._isThereHitResultInLastFrame)
			{
				return;
			}

			Gizmos.color = target._isHoveringInLastFrame ? Color.green : Color.yellow;

			Gizmos.DrawWireSphere(target._hitResultInLastFrame.point, 0.1f);
			Gizmos.DrawRay(target._hitResultInLastFrame.point, target._hitResultInLastFrame.normal * 0.5f);
		}


		[DrawGizmo(GizmoType.Active | GizmoType.Selected)]
		static void DrawInteractionStatus(InteractAction target, GizmoType _)
		{
			var style = new GUIStyle();

			var text = $"* 검출 객체: {(target._isThereHitResultInLastFrame ? target._hitResultInLastFrame.transform.name : "없음")}";
			text += $"\n* 호버링: {target._isHoveringInLastFrame}";
			text += $"\n* 상호작용: {target._isInteractingInLastFrame}";

			var labelPivot = target._hitResultInLastFrame.point;

			if (!target._isThereHitResultInLastFrame)
			{
				style.normal.textColor = Color.white;
				labelPivot = Camera.main.transform.position + Camera.main.transform.forward * target._maxDistance;
			}
			else if (!target._isHoveringInLastFrame)
			{
				style.normal.textColor = Color.yellow;
			}
			else
			{
				style.normal.textColor = Color.green;
			}

			Handles.Label(labelPivot, text, style);
		}
#endif

		[SerializeField] float _maxDistance = 10.0f;

		int _layerMask = ~(1 << 3);

		const string _interactionTargetTagName = "InteractionTarget";

		bool _isThereHitResultInLastFrame = false;
#if UNITY_EDITOR
		RaycastHit _hitResultInLastFrame;
#endif

		GameObject _rootGameObjectInLastFrame;
		bool _isHoveringInLastFrame = false;
		bool _isInteractingInLastFrame = false;

		IInteractableObject _interactableObject;
	}
}
