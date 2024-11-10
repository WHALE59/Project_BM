using System.Linq;

using UnityEditor;
using UnityEngine;

namespace BM.Editors
{
	/// <summary>
	/// 유니티 에디터에서 빌드 버튼 눌렀을 때의 이벤트를 후킹하여 스크립트 추가. <br/>
	/// SC_PersistentGameplay 씬을 빌드 인덱스 0 으로 강제로 만들고 활성화 한다. <br/>
	/// SC_PersistentGameplay 씬이 빌드 설정에 없으면 경고를 띄우고 빌드를 중지한다. <br/>
	/// 추후 게임이 형태를 갖춰가면 변경
	/// </summary>
	/// <remarks>
	/// 로그는 `%LOCALAPPDATA%/Unity/Editor 에서 확인
	/// </remarks>
	[InitializeOnLoad]
	public static class PreBuilder
	{
		private static readonly string m_persistentGamplaySceneName = "SC_PersistentGameplay";

		static PreBuilder()
		{
			BuildPlayerWindow.RegisterBuildPlayerHandler(OnClickingBuild);
		}

		private static void OnClickingBuild(BuildPlayerOptions options)
		{
			Debug.Log("[BM.PreBuilder] 빌드 후킹 프로세스 시작");

			var scenes = EditorBuildSettings.scenes.ToList();
			var persistentSceneIndex = scenes.FindIndex(scene => scene.path.Contains($"{m_persistentGamplaySceneName}"));

			Debug.Log($"{m_persistentGamplaySceneName}의 빌드 인덱스는 {persistentSceneIndex} 입니다.");

			// 상주 씬이 발견되지 않음
			if (persistentSceneIndex < 0)
			{
				var label = $"File > Build Settings 에 {m_persistentGamplaySceneName} 씬이 없습니다. 해당 씬을 빌드 씬에 추가해 주세요.";

				EditorUtility.DisplayDialog("[BM.PerBuilder] 빌드 오류", label, "알겠습니다. (빌드를 중단합니다)");
				Debug.Log(label);

				return;
			}
			// 발견되고, 정상 설정
			else if (persistentSceneIndex == 0)
			{
				Debug.Log($"[BM.PreBuilder] {m_persistentGamplaySceneName} 씬이 이미 빌드 오더 0이었기 때문에, 기본 설정으로 빌드를 시작합니다.");
				BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
			}
			// 발견되지 않았고, 비정상 설정
			else
			{
				var persistentScene = scenes[persistentSceneIndex];

				scenes.RemoveAt(persistentSceneIndex);
				scenes.Insert(0, persistentScene);

				options.scenes = scenes.Select(scene => scene.path).ToArray();

				Debug.Log($"[BM.PreBuilder] {m_persistentGamplaySceneName} 씬이 빌드 오더 0이 아니었기 때문에, 빌드 오더 0으로 만들었습니다.");

				if (!persistentScene.enabled)
				{
					persistentScene.enabled = true;
					Debug.Log($"[BM.PreBuilder] {m_persistentGamplaySceneName} 씬이 빌드 설정에서 활성화 되지 않았기 때문에, 활성화 시킵니다.");
				}

				BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
			}
		}
	}
}