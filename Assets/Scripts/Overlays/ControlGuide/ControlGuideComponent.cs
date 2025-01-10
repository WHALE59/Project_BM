using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BM
{
	[DisallowMultipleComponent]
	public class ControlGuideComponent : MonoBehaviour
	{
		[Header("입력 아이콘을 그리는 데에 사용될 입력 애셋")]

		[SerializeField] private InputActionReference m_inputActionReference;

		[Header("UI 구성 요소")]

		[SerializeField] private Image m_controlIcon;
		[SerializeField] private TMP_Text m_tmpText;

		public void EnableWithText(string text)
		{
			SetText(text);
			Enable();
		}

		public void Enable()
		{
			gameObject.SetActive(true);
		}

		public void Disable()
		{
			gameObject.SetActive(false);
		}


		public void SetText(string text)
		{
			m_tmpText.text = text;
		}
	}
}