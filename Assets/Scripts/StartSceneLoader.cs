// TODO: Core Definition 같은걸 만들어야 할 수도 있음
#if !UNITY_EDITOR && UNITY_STANDALONE
#define BM_BUILDGAME
#endif

using UnityEngine;
using UnityEngine.SceneManagement;

namespace BM
{
	/// <summary>
	/// 빌드를 하고 실행하면 File > Build Settings의 0번 인덱스 씬만 로드되는데, 이 씬은 SC_PersistentGameplay이다. <br/>
	/// 이 씬에 더해 필요한 씬을 로드하는 트리거. 이 스크립트는 SC_PersistentGameplay 내의 오브젝트에 부착되어야 한다.
	/// </summary>
	[DisallowMultipleComponent]
	public class StartSceneLoader : MonoBehaviour
	{
#if BM_BUILDGAME
		private StartSettings m_startSettings;
		private Scene m_toActive;

		private void OnSceneLoaded(Scene scene, LoadSceneMode _)
		{
			if (m_startSettings.StartSceneName == scene.name)
			{
				SceneManager.SetActiveScene(SceneManager.GetSceneByName(m_startSettings.StartSceneName));
			}
		}

		private void LoadStartScene()
		{
			var toLoad = m_startSettings.StartSceneName;

			SceneManager.LoadScene(toLoad, LoadSceneMode.Additive);
		}

		private void Awake()
		{
			if (!m_startSettings)
			{
				m_startSettings = (StartSettings)Resources.Load("StartSettings");
				if (m_startSettings)
				{
					Debug.Log($"[BM.StartSceneLoader] {m_startSettings} 로드 성공");
					m_toActive = SceneManager.GetSceneByName(m_startSettings.StartSceneName);
				}
				else
				{
					Debug.Log($"[BM.StartSceneLoader] {m_startSettings} 로드 실패");
				}
			}

			LoadStartScene();
		}

		private void OnEnable()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		private void OnDestroy()
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}
#endif
	}
}
