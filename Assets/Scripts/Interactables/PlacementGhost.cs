using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BM.InteractableObjects
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(Rigidbody))]
	public class PlacementGhost : MonoBehaviour
	{
		[SerializeField][Range(0.0f, 1.0f)] private float m_ghostAlpha = 0.5f;

		[SerializeField] private Color m_colorOnPossible = Color.green;
		[SerializeField] private Color m_colorOnImpossible = Color.red;

		private Collider m_collider;
		private HashSet<GameObject> m_collidingObjects = new();
		private Material m_material;

		public bool CanBePlaced => m_collidingObjects.Count == 0;

		private void SetImpossibleState()
		{
			var color = new Color(m_colorOnImpossible.r, m_colorOnImpossible.g, m_colorOnImpossible.b, m_ghostAlpha);
			m_material.color = color;
		}

		private void SetPossibleState()
		{
			var color = new Color(m_colorOnPossible.r, m_colorOnPossible.g, m_colorOnPossible.b, m_ghostAlpha);
			m_material.color = color;
		}

		private void UpdatePlacementState()
		{
			if (CanBePlaced)
			{
				SetPossibleState();
			}
			else
			{
				SetImpossibleState();
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			m_collidingObjects.Add(other.gameObject);

			UpdatePlacementState();
		}

		private void OnTriggerExit(Collider other)
		{
			m_collidingObjects.Remove(other.gameObject);

			UpdatePlacementState();
		}

		private void OnEnable()
		{
			m_collidingObjects.Clear();
		}

		private void OnDisable()
		{
			m_collidingObjects.Clear();
		}

		private void Awake()
		{
			m_collider = GetComponent<Collider>();
			m_collider.isTrigger = true;

			m_material = GetComponentInChildren<MeshRenderer>().material;
		}

		private void Start()
		{
			UpdatePlacementState();
		}

#if UNITY_EDITOR

		/// <summary>
		/// 이 PlacementGhost와 충돌한 다른 오브젝트들의 이름을 표시한다.
		/// </summary>
		[DrawGizmo(GizmoType.Active | GizmoType.NonSelected)]
		private static void DrawCollidedObjectName(PlacementGhost target, GizmoType _)
		{

			if (!Application.isPlaying)
			{
				return;
			}

			if (!target.gameObject.activeSelf)
			{
				return;
			}

			if (target.m_collidingObjects.Count > 0)
			{
				string label = string.Empty;

				foreach (GameObject collidingObject in target.m_collidingObjects)
				{
					label += $"- {collidingObject.name}\n";
				}

				GUIStyle style = new();
				style.normal.textColor = Color.white;

				Handles.Label(target.transform.position, label, style);
			}
		}
#endif
	}
}