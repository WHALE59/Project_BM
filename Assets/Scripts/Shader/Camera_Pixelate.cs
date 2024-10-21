using UnityEngine;
using UnityEngine.UI;

namespace BM
{
	public enum ePixelScreenMode { Resize, Scale }
	[System.Serializable]
	public struct ScreenSize
	{
		public int iWidth;
		public int iHeight;
	}

	public class Camera_Pixelate : MonoBehaviour
	{

		void Start()
		{
			Initialize();
		}

		void Initialize()
		{
			// m_cRender_Camera가 비어있을 시 카메라 컴포넌트 경로선언
			if (!m_cRender_Camera) { m_cRender_Camera = GetComponent<Camera>(); }
			// 현재 스크린의 사이즈 정보
			m_iScreenWidth = Screen.width;
			m_iScreenHeight = Screen.height;

			// 모든 값을 1 이하로 초기화
			//if (1 > m_iScreenScaleFactor) { m_iScreenScaleFactor = 1; }
			//if (1 > m_stTargetSize.iWidth) { m_stTargetSize.iWidth = 1; }
			//if (1 > m_stTargetSize.iHeight) { m_stTargetSize.iHeight = 1; }

			// 후처리용 이미지의 사이즈 정의
			int iWidth = m_eMode == ePixelScreenMode.Resize ? m_stTargetSize.iWidth : m_iScreenWidth / (int)m_iScreenScaleFactor;
			int iHeight = m_eMode == ePixelScreenMode.Resize ? m_stTargetSize.iHeight : m_iScreenHeight / (int)m_iScreenScaleFactor;

			// 이미지 생성
			m_cRenderTexture = new RenderTexture(iWidth, iHeight, 24)
			{
				filterMode = FilterMode.Point,
				antiAliasing = 1
			};

			// 이미지 적용
			m_cRender_Camera.targetTexture = m_cRenderTexture;
			m_cDisplay.texture = m_cRenderTexture;
		}

		bool CheckScreenResize()
		{
			return Screen.width != m_iScreenWidth || Screen.height != m_iScreenHeight;
		}

		[Header("화면 스케일링 설정")]
		public ePixelScreenMode m_eMode;
		public ScreenSize m_stTargetSize = new ScreenSize { iWidth = Screen.width, iHeight = Screen.height };
		public uint m_iScreenScaleFactor = 1;

		private Camera m_cRender_Camera;
		private RenderTexture m_cRenderTexture;
		private int m_iScreenWidth;
		private int m_iScreenHeight;

		[Header("디스플레이")]
		public RawImage m_cDisplay;

	}

}
