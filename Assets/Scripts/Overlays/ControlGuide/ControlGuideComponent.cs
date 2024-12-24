using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BM
{
	[DisallowMultipleComponent]
	public class ControlGuideComponent : MonoBehaviour
	{
		[Header("Input Action Settings")]
		[Space()]

		[SerializeField] private InputActionReference m_inputActionReference;

		[Header("UI Components")]
		[Space()]

		[SerializeField] private Image m_controlIcon;
	}
}