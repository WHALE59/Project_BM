using FMODUnity;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Utilities;

namespace BM.Interactables
{
	public class Interactable_DarackCurtain : InteractableBase
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