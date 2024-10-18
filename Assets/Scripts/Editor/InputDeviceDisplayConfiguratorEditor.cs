using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BM.Editors
{
	[CustomEditor(typeof(InputDeviceDisplayConfigurator))]
	public class InputDeviceDisplayConfiguratorEditor : Editor
	{
		ReorderableList inputDeviceSets;

		void OnEnable()
		{
			DrawDeviceSets();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.LabelField("Device Sets", EditorStyles.boldLabel);
			inputDeviceSets.DoLayoutList();

			serializedObject.ApplyModifiedProperties();
		}

		void DrawDeviceSets()
		{
			inputDeviceSets = new
			(
				serializedObject,
				serializedObject.FindProperty("inputDeviceSets"),
				draggable: true,
				displayHeader: true,
				displayAddButton: true,
				displayRemoveButton: true
			);

			inputDeviceSets.drawHeaderCallback = (Rect rect) =>
			{
				EditorGUI.LabelField(CalculateColumn(rect, 1, 15, 0), "Raw Path Name");
				EditorGUI.LabelField(CalculateColumn(rect, 2, 15, 0), "Device Display Settings");
			};

			inputDeviceSets.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
			{
				var inputDeviceSet = inputDeviceSets.serializedProperty.GetArrayElementAtIndex(index);

				rect.y += 2;

				EditorGUI.PropertyField(CalculateColumn(rect, 1, 0, 0), inputDeviceSet.FindPropertyRelative("rawPath"), GUIContent.none);
				EditorGUI.PropertyField(CalculateColumn(rect, 2, 10, 10), inputDeviceSet.FindPropertyRelative("displayData"), GUIContent.none);
			};
		}

		Rect CalculateColumn(Rect rect, int columnNumber, float xPadding, float xWidth)
		{
			var xPosition = rect.x + columnNumber switch
			{
				1 => xPadding,
				2 => rect.width / 2.0f + xPadding,
				_ => 0
			};

			return new Rect(xPosition, rect.y, rect.width / 2.0f - xWidth, EditorGUIUtility.singleLineHeight);
		}
	}
}
