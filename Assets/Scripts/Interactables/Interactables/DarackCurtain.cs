using FMODUnity;
using UnityEngine;

namespace BM.Interactables
{
	public class DarackCurtain : InteractableBase
	{
		[Header("Sound Settings")]
		[Space]

		[SerializeField] private EventReference m_soundOnOpening;
		[SerializeField] private EventReference m_soundOnClosing;

		[Space]

		[SerializeField] private InteractableModel m_modelOnOpened;
		[SerializeField] private InteractableModel m_modelOnClosed;

		private bool m_isOpened = false;

		public void SwitchModelToOpen()
		{
			Model = m_modelOnOpened;

			m_modelOnOpened.gameObject.SetActive(true);
			m_modelOnClosed.gameObject.SetActive(false);
		}

		public void SwitchModelToClose()
		{
			Model = m_modelOnClosed;

			m_modelOnOpened.gameObject.SetActive(false);
			m_modelOnClosed.gameObject.SetActive(true);
		}

		public override void StartActivation(InteractAction _)
		{
			base.StartActivation(_);

			if (!m_isOpened)
			{
				SwitchModelToOpen();

				m_isOpened = true;

				if (!m_soundOnOpening.IsNull)
				{
					RuntimeManager.PlayOneShot(m_soundOnOpening);
				}
			}
			else
			{
				SwitchModelToClose();
				m_isOpened = false;

				if (!m_soundOnClosing.IsNull)
				{
					RuntimeManager.PlayOneShot(m_soundOnClosing);
				}
			}
		}

		/// <summary>
		/// 에디터에서 <see cref="m_modelOnClosed"/>와 <see cref="m_modelOnOpened"/> 둘 중 하나만 활성화 되어있어야 하며, 해당 상태를 기준으로 <see cref="m_isOpened"/>와 <see cref="Model"/>을 설정한다.
		/// </summary>
		/// <remarks>
		/// 만일 둘이 동시에 활성화 되어 있는 경우, <see cref="m_modelOnOpened"/>가 활성화 된 것을 시작 상태로 설정한다..
		/// </remarks>
		protected override void Start()
		{
			bool isStartOpened = m_modelOnOpened.gameObject.activeSelf;
			bool isStartClosed = m_modelOnClosed.gameObject.activeSelf;

			if (isStartOpened && !isStartClosed)
			{
				m_isOpened = true;

				Model = m_modelOnOpened;
			}
			else if (!isStartOpened && isStartClosed)
			{
				m_isOpened = false;

				Model = m_modelOnClosed;
			}
			else
			{
				m_isOpened = true;

				m_modelOnOpened.gameObject.SetActive(true);
				m_modelOnClosed.gameObject.SetActive(false);

				Model = m_modelOnOpened;
			}
		}
	}
}