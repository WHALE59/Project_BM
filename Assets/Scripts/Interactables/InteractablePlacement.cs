using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BM.Interactables
{
	[DisallowMultipleComponent]

	/// <remarks>
	/// 이 클래스는 에디터에서 부착할 용도로 만들어지지 않았고, <see cref="InteractableBase"/>에 의해 런타임에 인스턴싱 된다.
	/// </remarks>
	public class InteractablePlacement : MonoBehaviour
	{
		private List<Material> m_materials = new();
		private HashSet<GameObject> m_triggeringObject = new();

		public bool CanBePlaced => m_triggeringObject.Count == 0;

		private void OnTriggerEnter(Collider other)
		{
			m_triggeringObject.Add(other.gameObject);

			UpdateMaterial();
		}

		private void OnTriggerExit(Collider other)
		{
			m_triggeringObject.Remove(other.gameObject);

			UpdateMaterial();
		}

		public void GenerateVisualChildren(IReadOnlyCollection<InteractableModel.MeshElement> visualData, Material materialOnPlacement)
		{
			for (int i = 0; i < visualData.Count; ++i)
			{
				GameObject child = new($"Visual_{i:D2}");

				MeshFilter filter = child.AddComponent<MeshFilter>();
				MeshRenderer renderer = child.AddComponent<MeshRenderer>();

				filter.sharedMesh = visualData.ElementAt(i).Filter.sharedMesh;
				renderer.sharedMaterial = materialOnPlacement;

				child.transform.SetParent(transform);

				m_materials.Add(renderer.material);
			}
		}

		public void GenerateTriggerChildren(IReadOnlyCollection<Collider> colliders)
		{
			GameObject child = new("Trigger");
			child.transform.SetParent(transform);

			foreach (Collider collider in colliders)
			{
				if (collider is BoxCollider boxCollider)
				{
					BoxCollider targetCollider = child.AddComponent<BoxCollider>();

					targetCollider.center = boxCollider.center;
					targetCollider.size = boxCollider.size;

					targetCollider.isTrigger = true;
				}
				else if (collider is SphereCollider sphereCollider)
				{
					SphereCollider targetCollider = child.AddComponent<SphereCollider>();

					targetCollider.center = sphereCollider.center;
					targetCollider.radius = sphereCollider.radius;

					targetCollider.isTrigger = true;
				}
				else if (collider is CapsuleCollider capsuleCollider)
				{
					CapsuleCollider targetCollider = child.AddComponent<CapsuleCollider>();

					targetCollider.center = capsuleCollider.center;
					targetCollider.height = capsuleCollider.height;
					targetCollider.radius = capsuleCollider.radius;
					targetCollider.direction = capsuleCollider.direction;

					targetCollider.isTrigger = true;
				}

				else if (collider is MeshCollider meshCollider)
				{
					MeshCollider targetCollider = child.AddComponent<MeshCollider>();

					targetCollider.sharedMesh = meshCollider.sharedMesh;
					targetCollider.convex = meshCollider.convex;
					targetCollider.cookingOptions = meshCollider.cookingOptions;

					targetCollider.isTrigger = true;
				}
			}
		}

		public void Enable()
		{
			gameObject.SetActive(true);
		}

		public void Disable()
		{
			gameObject.SetActive(false);
		}

		private void UpdateMaterial()
		{
			if (CanBePlaced)
			{
				foreach (Material material in m_materials)
				{
					material.color = Color.green;
				}
			}
			else
			{
				foreach (Material material in m_materials)
				{
					material.color = Color.red;
				}
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// 다른 오브젝트와 겹쳤을 때, 다른 오브젝트의 이름을 표시한다.
		/// </summary>
		[UnityEditor.DrawGizmo(UnityEditor.GizmoType.Active | UnityEditor.GizmoType.NonSelected)]
		private static void DrawTriggeredObjectName(InteractablePlacement target, UnityEditor.GizmoType _)
		{
			if (!Application.isPlaying)
			{
				return;
			}

			if (!target.gameObject.activeSelf)
			{
				return;
			}

			if (target.m_triggeringObject.Count > 0)
			{
				string label = string.Empty;

				foreach (GameObject collidingObject in target.m_triggeringObject)
				{
					label += $"- {collidingObject.name}\n";
				}

				GUIStyle style = new();
				style.normal.textColor = Color.white;

				UnityEditor.Handles.Label(target.transform.position, label, style);
			}
		}
#endif

	}
}