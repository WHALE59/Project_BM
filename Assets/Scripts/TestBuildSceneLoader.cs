#if !UNITY_EDITOR && UNITY_STANDALONE
#define BM_LOAD_BUILD_SCENES_ADDITIVE
#endif

using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace BM
{
	/// <summary>
	/// 빌드를 하고 실행하면 Build Settings에 첫 번째 씬만 로드되는데, 첫 번째 씬에서 필요한 다른 씬을 Additive로 로딩하기 위한 트리거
	/// SC_PersistentGameplay 씬이 BuildSettings의 첫 번째 씬이므로, 이 씬 내의 아무 오브젝트에나 이 스크립트를 붙이면 된다.
	/// </summary>
	[DisallowMultipleComponent]
	public class TestBuildSceneLoader : MonoBehaviour
	{
		[SerializeField] List<string> m_sceneNameToLoad = new() { "SC_SampleScene" };
		[SerializeField] string m_activeSceneName = "SC_SampleScene";

		private void OnSceneLoaded(Scene scene, LoadSceneMode _)
		{
			var sceneToSetActive = SceneManager.GetSceneByName(m_activeSceneName);

			if (sceneToSetActive == scene)
			{
				SceneManager.SetActiveScene(sceneToSetActive);
			}

		}

		private void OnEnable()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		private void OnDestroy()
		{

			SceneManager.sceneLoaded -= OnSceneLoaded;
		}

		private void Awake()
		{
			LoadScenesAdditiveForBuild();
		}

		[Conditional("BM_LOAD_BUILD_SCENES_ADDITIVE")]
		private void LoadScenesAdditiveForBuild()
		{
			foreach (var sceneNameToLoad in m_sceneNameToLoad)
			{
				if (SceneManager.GetSceneByName(sceneNameToLoad).isLoaded)
				{
					continue;
				}

				SceneManager.LoadScene(sceneNameToLoad, LoadSceneMode.Additive);
			}
		}
	}
}
