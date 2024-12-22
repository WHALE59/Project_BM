// TODO: 자동으로 할당해 주는 것 만들 것.

#if false

using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BM.Editors
{
	[CustomEditor(typeof(KeyboardMouseControlIconsSO))]
	internal sealed class KeyboardMouseControlIconsSOEditor : Editor
	{
		private KeyboardMouseControlIconsSO m_target;

		private string m_style = "Retro"; // 기본 스타일
		private string m_basePath = "Assets/Texture/ControlGuide/Keyboard_Mouse/";

		public override void OnInspectorGUI()
		{
			m_target = (KeyboardMouseControlIconsSO)target;

			DrawDefaultInspector();

			// 스타일 선택

			m_style = EditorGUILayout.TextField("Style", m_style);
			m_basePath = EditorGUILayout.TextField("Base Path", m_basePath);

			if (GUILayout.Button("애셋 자동 할당"))
			{
				AssignSprites(m_target);
			}
		}

		private void AssignSprites(KeyboardMouseControlIconsSO data)
		{
			// 경로 존재 확인

			if (!Directory.Exists(m_basePath))
			{
				Debug.LogError($"다음의 경로가 존재하지 않습니다: {m_basePath}");
				return;
			}

			serializedObject.Update();

			SerializedProperty iterator = serializedObject.GetIterator();

			while (iterator.NextVisible(true))
			{
				if (iterator.propertyType == SerializedPropertyType.ObjectReference && iterator.displayName.StartsWith("m_"))
				{
					Debug.Log($"{iterator.displayName}");
				}
			}

			FieldInfo[] spriteFields = typeof(KeyboardMouseControlIconsSO)
				.GetFields(BindingFlags.Public | BindingFlags.Instance)
				.Where(field => field.FieldType == typeof(Sprite))
				.ToArray();

			string[] spriteFiles = Directory.GetFiles(m_basePath, $"*_{m_style}.png");

			foreach (string fileNameWithPath in spriteFiles)
			{
				string pureFileName = Path.GetFileNameWithoutExtension(fileNameWithPath);
				string variableName = ConvertFilenameToVariableName(pureFileName);

				FieldInfo targetSpriteField = spriteFields.FirstOrDefault(field => field.Name == variableName);

				if (null == targetSpriteField)
				{
					Debug.LogWarning($"{pureFileName} 파일에 대한 필드 {variableName}이 존재하지 않습니다.");
					continue;
				}

				Sprite loadedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(fileNameWithPath);

				if (null == loadedSprite)
				{
					Debug.LogWarning($"스프라이트 {fileNameWithPath}를 찾을 수 없습니다.");
					continue;
				}

				Undo.RecordObject(data, $"{variableName}에 할당");
				targetSpriteField.SetValue(data, loadedSprite);
				Debug.Log($"{variableName}에 {fileNameWithPath}를 할당하였습니다.");
			}

			EditorUtility.SetDirty(data);
			AssetDatabase.SaveAssets();
		}

		private string ConvertFilenameToVariableName(string fileName)
		{
			string[] parts = fileName.Split('_');

			if (parts.Length < 3)
			{
				Debug.LogWarning($"다음 파일 이름 형식이 잘못되었습니다: {fileName}");
				return null;
			}

			string keyName = parts[1];
			string keyType = parts[0] switch
			{
				"T" => "m_keyboard",
				"T_Mouse" => "m_mouse",
				_ => null
			};

			if (null == keyType)
			{
				Debug.LogWarning($"다음 파일의 접두어가 잘못되었습니다: {fileName}");
			}

			return $"{keyType}{keyName}";
		}
	}
}

#endif