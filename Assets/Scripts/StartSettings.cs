using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace BM
{
	[CreateAssetMenu(fileName = "StartSettings", menuName = "BM/Settings/Start Settings")]
	public class StartSettings : ScriptableObject
	{
		[SerializeField][HideInInspector] private string m_startScene = "SC_Darack";
		public string StartSceneName => m_startScene;
	}
}
