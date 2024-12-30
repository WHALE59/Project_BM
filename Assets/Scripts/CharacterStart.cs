using System.Collections;
using System.Diagnostics.CodeAnalysis;

using UnityEngine;
using UnityEngine.SceneManagement;

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

			bool isPersistentGameplaySceneLoaded = false;

			for (int i = 0; i < SceneManager.sceneCount; ++i)
			{
				isPersistentGameplaySceneLoaded |= SceneManager.GetSceneAt(i).name == m_persistentGameplaySceneName;
			}

			bool shouldLoadPersistentGameplayScene = !isPersistentGameplaySceneLoaded && !m_isPersistentGameplaySceneLoadAlreadyRequested;

			if (shouldLoadPersistentGameplayScene)
			{
				m_isPersistentGameplaySceneLoadAlreadyRequested = true;

				AsyncOperation operation = SceneManager.LoadSceneAsync(m_persistentGameplaySceneName, LoadSceneMode.Additive);

				StartCoroutine(LoadRoutine(operation));
			}
		}

#if UNITY_EDITOR
		[DrawGizmo(GizmoType.Active | GizmoType.NonSelected)]
		[SuppressMessage("Style", "IDE0051", Justification = "Used by Unity's DrawGizmo attribute.")]
		private static void DrawCharacterBound(CharacterStart target, GizmoType _)
		{
			GameObject characterPrefab = target.m_characterPrefab;

			if (!characterPrefab)
			{
				return;
			}

			if (characterPrefab.TryGetComponent(out CharacterController characterController))
			{
				float radius = characterController.radius;
				float height = characterController.height;
				Vector3 center = characterController.center + target.transform.position;

				Vector3 halfExtents = new(radius, height / 2.0f, radius);

				bool isCollideWithWorld = Physics.OverlapBox(center, halfExtents, Quaternion.identity, ~(1 << 3)).Length != 0;

				Gizmos.color = isCollideWithWorld ? Color.red : Color.green;
				Gizmos.DrawWireCube(center, 2.0f * halfExtents);

				if (characterPrefab.TryGetComponent(out LocomotiveAction locomotiveAction))
				{
					Vector3 start = target.transform.position + locomotiveAction.CameraTargetLocalPosition;
					Vector3 direction = target.transform.forward;

					Gizmos.color = Color.yellow;

					Gizmos.DrawRay(start, direction * 1.0f);
				}
			}
		}
#endif
	}
}