using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(CanvasGroup))]
	public class DeveloperOverlay : MonoBehaviour
	{
		[SerializeField] private CharacterStartFinder m_character;
		[SerializeField] private InputReaderSO m_inputReader;
		[SerializeField] private VerticalLayoutGroup m_buttonGroup;
		[SerializeField] private Button m_buttonPrefab;
		[SerializeField] private List<string> m_sceneNameToExclude = new() { m_persistentGameplaySceneName };

		private const string m_persistentGameplaySceneName = "SC_PersistentGameplay";

		private bool m_enabled = false;
		private CanvasGroup m_canvasGroup;

		private void MakeSceneLoadButtons()
		{
			if (!m_buttonGroup || !m_buttonPrefab)
			{
				return;
			}

			// 무슨 이유에서든 버튼 그룹이 비어있지 않다면, 하위 요소들을 모두 파괴

			foreach (Transform childTransform in m_buttonGroup.transform)
			{
				Destroy(childTransform.gameObject);
			}

			int buttonIndex = 0;

			for (int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i)
			{
				// 빌드 씬 목록 받아오기

				string scene = SceneUtility.GetScenePathByBuildIndex(i);
				string sceneName = Path.GetFileNameWithoutExtension(scene);

				if (m_sceneNameToExclude.Contains(sceneName))
				{
					continue;
				}

				// 버튼 생성

				Button buttonGameObject = Instantiate
				(
					original: m_buttonPrefab,
					parent: m_buttonGroup.transform
				);

				buttonGameObject.name = $"Button_{buttonIndex:D2}";

				TMP_Text textComponent = buttonGameObject.GetComponentInChildren<TMP_Text>();
				textComponent.text = sceneName;

				// 씬 로드 이벤트 바인딩

				buttonGameObject.onClick.AddListener(() => { OnSceneLoadButtonClicked(sceneName); });

				buttonIndex++;
			}

		}

		/// <summary>
		/// 로드할 씬과 언로드할 씬을 구분하여, AsyncOperation 생성 후 코루틴에 넘겨줘서 대기
		/// </summary>
		private void OnSceneLoadButtonClicked(string loadSceneName)
		{
			m_canvasGroup.interactable = false;

			var unloadSceneNames = new List<string>();

			for (int i = 0; i < SceneManager.sceneCount; ++i)
			{
				Scene loadedScene = SceneManager.GetSceneAt(i);

				if (loadedScene.name == m_persistentGameplaySceneName)
				{
					continue;
				}

				unloadSceneNames.Add(loadedScene.name);
			}

			StartCoroutine(LoadingRoutine(unloadSceneNames, loadSceneName));
		}

		private IEnumerator LoadingRoutine(IReadOnlyCollection<string> toUnloads, string toLoad)
		{
			m_character.gameObject.SetActive(false);

			var unloadOperations = new List<AsyncOperation>();

			// 언로드 작업

			foreach (string toUnload in toUnloads)
			{
				unloadOperations.Add(SceneManager.UnloadSceneAsync(toUnload));
			}

			// 언로드 작업 대기

			while (!unloadOperations.All(operation => operation.isDone))
			{
				yield return null;
			}

			// 로드 작업 시작

			AsyncOperation loadOperation = SceneManager.LoadSceneAsync(toLoad, LoadSceneMode.Additive);

			while (!loadOperation.isDone)
			{
				yield return null;
			}

			// 로드 작업이 완료 됨

			// 오버레이 숨기고
			Hide();
			m_enabled = false;

			// 캐릭터 위치 초기화 및 활성화
			m_character.Initialize();
			m_character.gameObject.SetActive(true);

			// 방금 로드한 씬을 Active Scene으로

			SceneManager.SetActiveScene(SceneManager.GetSceneByName(toLoad));
		}

		private void Show()
		{
			Time.timeScale = 0.0f;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;

			m_canvasGroup.alpha = 1.0f;
			m_canvasGroup.interactable = true;
		}

		private void Hide()
		{
			Time.timeScale = 1.0f;
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;

			m_canvasGroup.alpha = 0.0f;
			m_canvasGroup.interactable = false;
		}

		private void OnToggleInput()
		{
			if (!m_enabled)
			{
				Show();
			}
			else
			{
				Hide();
			}

			m_enabled = !m_enabled;
		}

		private void Awake()
		{
			m_canvasGroup = GetComponent<CanvasGroup>();
			MakeSceneLoadButtons();
		}

		private void Start()
		{
			gameObject.SetActive(true);
			Hide();
		}

		private void OnEnable()
		{
			m_inputReader.ToggleDeveloperInputPerformed += OnToggleInput;
		}

		private void OnDisable()
		{
			m_inputReader.ToggleDeveloperInputPerformed -= OnToggleInput;
		}
	}
}
