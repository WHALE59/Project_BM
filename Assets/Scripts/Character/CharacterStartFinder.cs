using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(CharacterController))]
	public class CharacterStartFinder : MonoBehaviour
	{
		private List<CharacterStart> m_characterStarts = new();
		private CharacterController m_characterController;

		public void FindCharacterStartInAllLoadedScene()
		{
			m_characterStarts.Clear();

			for (int i = 0; i < SceneManager.sceneCount; ++i)
			{
				Scene loadedScene = SceneManager.GetSceneAt(i);

				// 이 스크립트가 부착된 오브젝트는 반드시 SC_PersistentGameplay 씬에 존재한다고 가정
				if (gameObject.scene == loadedScene)
				{
					continue;
				}

				foreach (GameObject rootGameObject in loadedScene.GetRootGameObjects())
				{
					if (rootGameObject.TryGetComponent(out CharacterStart characterStart))
					{
						m_characterStarts.Add(characterStart);
					}
				}
			}
		}

		public void MoveCharacterToProperStart()
		{

			CharacterStart properStart = null;
			Vector3 properPosition;
			Quaternion properRotation;

			if (m_characterStarts.Count <= 0)
			{
#if UNITY_EDITOR
				Debug.LogWarning("어떤 Character Start도 발견되지 않았습니다.");
#endif
				properPosition = Vector3.zero;
				properRotation = Quaternion.identity;
			}
			else
			{
#if UNITY_EDITOR
				if (m_characterStarts.Count > 1)
				{
					Debug.LogWarning($"Character Start가 하나보다 많습니다. {m_characterStarts.First().gameObject.scene.name}의 {m_characterStarts.First()}를 시작 지점으로 사용합니다.");
				}
#endif

				properStart = m_characterStarts.First();
				properStart.transform.GetPositionAndRotation(out properPosition, out properRotation);
			}

			m_characterController.gameObject.SetActive(false);
			m_characterController.enabled = false;

			transform.SetPositionAndRotation(properPosition, properRotation);

			m_characterController.enabled = true;
			m_characterController.gameObject.SetActive(true);

			if (properStart)
			{
				Destroy(properStart.gameObject);
			}
		}

		public void Initialize()
		{
			FindCharacterStartInAllLoadedScene();
			MoveCharacterToProperStart();
		}

		private void Awake()
		{
			m_characterController = GetComponent<CharacterController>();
		}

		private void Start()
		{
			Initialize();
		}

	}
}