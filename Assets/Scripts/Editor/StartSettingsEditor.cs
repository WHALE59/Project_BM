using System;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BM.Editors
{
	[CustomEditor(typeof(StartSettings))]
	internal sealed class StartSettingsEditor : Editor
	{
		private SerializedProperty m_startSceneProperty;

		private void OnEnable()
		{
			m_startSceneProperty = serializedObject.FindProperty("m_startScene");
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			serializedObject.Update();

			var sceneCount = SceneManager.sceneCountInBuildSettings;
			string[] buildSceneNames = new string[sceneCount];

			for (var i = 0; i < sceneCount; ++i)
			{
				var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
				buildSceneNames[i] = Path.GetFileNameWithoutExtension(scenePath);
			}

			buildSceneNames = Array.FindAll(buildSceneNames, (sceneName) => sceneName != "SC_PersistentGameplay");

			var selectedSceneIndex = Mathf.Max(0, Array.IndexOf(buildSceneNames, m_startSceneProperty.stringValue));

			selectedSceneIndex = EditorGUILayout.Popup("시작 씬", selectedSceneIndex, buildSceneNames);
			m_startSceneProperty.stringValue = buildSceneNames[selectedSceneIndex];

			serializedObject.ApplyModifiedProperties();
		}
	}

	internal static class StartSettingsMenu
	{
		[MenuItem("BM/시작 설정")]
		private static void OpenStartSettings()
		{
			Selection.activeObject = Resources.Load<StartSettings>("StartSettings");
		}
	}
}