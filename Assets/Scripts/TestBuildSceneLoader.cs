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
		void Awake()
		{
			LoadScenesAdditiveForBuild();
		}

#if UNITY_EDITOR
		void Reset()
		{
			_sceneNamesToLoad.Clear();
			_sceneNamesToLoad.Add("SC_SampleScene");
		}
#endif

		[Conditional("BM_LOAD_BUILD_SCENES_ADDITIVE")]
		void LoadScenesAdditiveForBuild()
		{
			foreach (var sceneNameToLoad in _sceneNamesToLoad)
			{
				if (SceneManager.GetSceneByName(sceneNameToLoad).isLoaded)
				{
					continue;
				}

				SceneManager.LoadScene(sceneNameToLoad, LoadSceneMode.Additive);
			}
		}

		[SerializeField] List<string> _sceneNamesToLoad = new();
	}
}
