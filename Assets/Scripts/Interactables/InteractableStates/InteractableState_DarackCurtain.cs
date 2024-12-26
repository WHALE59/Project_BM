using FMODUnity;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BM.Interactables
{
	public class InteractableState_DarackCurtain : InteractableStateBase
	{
		[SerializeField] private EventReference m_manipulatingSound;

		[Space]

		[SerializeField] private InteractableModel m_modelOnOpened;
		[SerializeField] private InteractableModel m_modelOnClosed;

		[Space]

		[SerializeField] private bool m_isOpenOnStart = true;

		private bool m_isOpened = false;

		private InteractableBase m_interactableBase;

		public void Open()
		{
			m_isOpened = true;

			m_interactableBase.Model = m_modelOnOpened;

			m_modelOnOpened.gameObject.SetActive(true);
			m_modelOnClosed.gameObject.SetActive(false);
		}

		public void Close()
		{
			m_isOpened = false;

			m_interactableBase.Model = m_modelOnClosed;

			m_modelOnOpened.gameObject.SetActive(false);
			m_modelOnClosed.gameObject.SetActive(true);

		}

		public override void StartActivate(InteractAction interactionSubject, InteractableBase interactionObject)
		{
			base.StartActivate(interactionSubject, interactionObject);

			if (!m_isOpened)
			{
				Open();
			}
			else
			{
				Close();
			}

			if (!m_manipulatingSound.IsNull)
			{
				RuntimeManager.PlayOneShot(m_manipulatingSound);
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
				Open();
			}
			else
			{
				Close();
			}
		}
	}
}