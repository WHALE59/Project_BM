
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BM
{
	[DisallowMultipleComponent]
	public class CharacterStart : MonoBehaviour
	{
		[SerializeField] private GameObject m_characterPrefab;
		private const string m_persistentGameplaySceneName = "SC_PersistentGameplay";

		private static bool m_isPersistentGameplaySceneLoadAlreadyRequested = false;

		private IEnumerator LoadRoutine(AsyncOperation operation)
		{
			Time.timeScale = 0.0f;

			while (!operation.isDone)
			{
				yield return null;
			}

			Time.timeScale = 1.0f;
		}

		private void Awake()
		{
			// PersistentGameplay 씬이 로드되지 않았으면 로드한다.

			var isPersistentGameplaySceneLoaded = false;

			for (var i = 0; i < SceneManager.sceneCount; ++i)
			{
				isPersistentGameplaySceneLoaded |= SceneManager.GetSceneAt(i).name == m_persistentGameplaySceneName;
			}

			var shouldLoadPersistentGameplayScene = !isPersistentGameplaySceneLoaded && !m_isPersistentGameplaySceneLoadAlreadyRequested;

			if (shouldLoadPersistentGameplayScene)
			{
				m_isPersistentGameplaySceneLoadAlreadyRequested = true;

				var operation = SceneManager.LoadSceneAsync(m_persistentGameplaySceneName, LoadSceneMode.Additive);

				StartCoroutine(LoadRoutine(operation));
			}
		}


#if UNITY_EDITOR
		[DrawGizmo(GizmoType.Active | GizmoType.NonSelected)]
		private static void DrawCharacterBound(CharacterStart target, GizmoType _)
		{
			var characterPrefab = target.m_characterPrefab;

			if (!characterPrefab)
			{
				return;
			}

			if (characterPrefab.TryGetComponent<CharacterController>(out var characterController))
			{
				var radius = characterController.radius;
				var height = characterController.height;
				var center = characterController.center + target.transform.position;

				var halfExtents = new Vector3(radius, height / 2.0f, radius);

				var isCollideWithWorld = Physics.OverlapBox(center, halfExtents, Quaternion.identity, ~(1 << 3)).Length != 0;

				Gizmos.color = isCollideWithWorld ? Color.red : Color.green;
				Gizmos.DrawWireCube(center, 2.0f * halfExtents);

				if (characterPrefab.TryGetComponent<LocomotiveAction>(out var locomotiveAction))
				{
					var start = target.transform.position + locomotiveAction.CameraTargetLocalPosition;
					var direction = target.transform.forward;

					Gizmos.color = Color.yellow;

					Gizmos.DrawRay(start, direction * 1.0f);
				}
			}
		}
#endif
	}
}