using BM.Interactables;
using UnityEditor;
using UnityEngine;

namespace BM.Editors
{
	[CustomEditor(typeof(InteractableModel))]
	internal sealed class InteractableModelEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			InteractableModel model = (InteractableModel)target;

			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("StartFresnelEffect"))
			{
				model.StartHoveringEffect();
			}

			if (GUILayout.Button("FinishFresnelEffect"))
			{
				model.FinishHoveringEffect();
			}

			EditorGUILayout.EndHorizontal();
		}
	}
}