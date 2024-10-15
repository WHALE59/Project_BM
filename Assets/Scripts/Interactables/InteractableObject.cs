using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BM
{
	/// <summary>
	/// 모든 상호작용 가능한 오브젝트의 상호작용 내역은, 아래 컴포넌트를 상속하여 구현.
	/// </summary>
	[DisallowMultipleComponent]
	public class InteractableObject : MonoBehaviour, IInteractableObject
	{
		void IInteractableObject.StartHover()
		{
			_isHovering = true;
		}

		void IInteractableObject.Startnteract()
		{
			_isInteracting = true;
		}

		void IInteractableObject.FinishHovering()
		{
			_isHovering = false;
		}

		void IInteractableObject.FinishInteract()
		{
			_isInteracting = false;
		}

#if UNITY_EDITOR
		[DrawGizmo(GizmoType.Active | GizmoType.NotInSelectionHierarchy)]
		static void DrawInteractableObjectGizmo(InteractableObject target, GizmoType _)
		{
			var label = $"* 상호작용 가능 객체: {target.gameObject.name}";

			var style = new GUIStyle();

			style.normal.textColor = Color.white;

			if (target._isHovering)
			{
				style.normal.textColor = Color.cyan;
			}
			if (target._isInteracting)
			{
				style.normal.textColor = Color.magenta;
			}

			label += $"\n* 표시 이름: {target._interactableObjectData.displayName}";
			label += $"\n* 상호작용 이름: {target._interactableObjectData.interactionName}";

			Handles.Label(target.transform.position, label, style);
		}
#endif
		InteractableObjectData IInteractableObject.Data => _interactableObjectData;

		[SerializeField] InteractableObjectData _interactableObjectData;

		protected bool _isHovering = false;
		protected bool _isInteracting = false;

	}
}
