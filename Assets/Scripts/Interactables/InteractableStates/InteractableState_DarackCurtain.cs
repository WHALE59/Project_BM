using FMODUnity;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BM.Interactables
{
	public class InteractableState_DarackCurtain : InteractableStateBase
	{
		[SerializeField] private EventReference m_soundOnOpening;
		[SerializeField] private EventReference m_soundOnClosing;

		[Space]

		[SerializeField] private InteractableModel m_modelOnOpened;
		[SerializeField] private InteractableModel m_modelOnClosed;

		[Space]

		[SerializeField] private bool m_isOpenOnStart = true;

		private bool m_isOpened = false;

		private InteractableBase m_interactableBase;

		public void SwitchModelToOpen()
		{
			m_interactableBase.Model = m_modelOnOpened;

			m_modelOnOpened.gameObject.SetActive(true);
			m_modelOnClosed.gameObject.SetActive(false);

		}

		public void SwitchModelToClose()
		{
			m_interactableBase.Model = m_modelOnClosed;

			m_modelOnOpened.gameObject.SetActive(false);
			m_modelOnClosed.gameObject.SetActive(true);

		}

		public override void StartActivate(InteractAction interactionSubject, InteractableBase interactionObject)
		{
			base.StartActivate(interactionSubject, interactionObject);

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

		protected override void Awake()
		{
			base.Awake();
			m_interactableBase = GetComponent<InteractableBase>();
		}

		protected override void Start()
		{
			base.Start();

			if (m_isOpenOnStart)
			{
				SwitchModelToOpen();
			}
			else
			{
				SwitchModelToClose();
			}
		}
	}
}