// TODO: Core Definition 같은걸 만들어야 할 수도 있음
#if !UNITY_EDITOR && UNITY_STANDALONE
#define BM_BUILDGAME
#endif

#pragma warning disable CS0414

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace BM
{
	/// <summary>
	/// 빌드를 하고 실행하면 File > Build Settings의 0번 인덱스 씬만 로드되는데, 이 씬은 SC_PersistentGameplay이다. <br/>
	/// 이 씬에 더해 필요한 씬을 로드하는 트리거. 이 스크립트는 SC_PersistentGameplay 내의 오브젝트에 부착되어야 한다.
	/// </summary>
	[DisallowMultipleComponent]
	public class TestBuildSceneLoader : MonoBehaviour
	{
		[SerializeField] List<string> m_sceneNameToLoad = new() { "SC_SampleScene" };
		[SerializeField] string m_activeSceneName = "SC_SampleScene";

#if BM_BUILDGAME
		private void OnSceneLoaded(Scene scene, LoadSceneMode _)
		{

			Debug.Log($"- [BM]: 씬 {scene.name}을 로드하였습니다.");

			var sceneToSetActive = SceneManager.GetSceneByName(m_activeSceneName);

			if (sceneToSetActive == scene)
			{
				Debug.Log($"- [BM]: 씬 {sceneToSetActive.name}을 활성 씬으로 바꾸었습니다.");
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
#endif
	}
}

#pragma warning restore CS0414
