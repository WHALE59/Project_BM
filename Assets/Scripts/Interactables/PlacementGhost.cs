using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BM.Interactables
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
		private void OnDrawGizmos()
		{
			if (!Application.isPlaying)
			{
				return;
			}

			if (!gameObject.activeSelf)
			{
				return;
			}

			if (m_collidingObjects.Count > 0)
			{
				var label = string.Empty;

				foreach (var collidingObject in m_collidingObjects)
				{
					label += $"- {collidingObject.name}\n";
				}

				var style = new GUIStyle();
				style.normal.textColor = Color.white;

				Handles.Label(transform.position, label, style);
			}
		}
#endif
	}
}