using UnityEngine;

namespace BM
{
	/// <summary>
	/// 단지 커서를 숨기는 일만 한다. (시야 이동 자체는 Virtual Camera 오브젝트를 참고할 것)
	/// </summary>
	[DisallowMultipleComponent]
	public class LookAction : MonoBehaviour
	{
		[SerializeField] private Transform m_characterAppearance;

		private void Awake()
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		private void FixedUpdate()
		{
			m_characterAppearance.forward = Camera.main.transform.forward;
		}
	}
}
