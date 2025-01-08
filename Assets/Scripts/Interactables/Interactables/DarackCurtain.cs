using FMODUnity;
using UnityEngine;

namespace BM.Interactables
{
	public class DarackCurtain : InteractableBase
	{
		[Header("사운드 설정")]

		[SerializeField] private EventReference m_soundOnOpening;
		[SerializeField] private EventReference m_soundOnClosing;

		[Header("Opened 상태")]

		[SerializeField] private GameObject m_modelOnOpened;
		[SerializeField] private HoveringEffectGroup m_hoveringEffectGroupOnOpened;

		[Header("Closed 상태")]

		[SerializeField] private GameObject m_modelOnClosed;
		[SerializeField] private HoveringEffectGroup m_hoveringEffectGroupOnClosed;

		private bool m_isOpened = false;

		public override void StartInteract(InteractAction _)
		{
			base.StartInteract(_);

			if (!m_isOpened)
			{
				// Switch to Open

				m_modelOnClosed.SetActive(false);

				m_modelOnOpened.SetActive(true);
				m_currentHoveringGroup = m_hoveringEffectGroupOnOpened;

				m_isOpened = true;

				if (!m_soundOnOpening.IsNull)
				{
					RuntimeManager.PlayOneShot(m_soundOnOpening);
				}
			}
			else
			{
				// Switch to Close

				m_modelOnOpened.SetActive(false);

				m_modelOnClosed.SetActive(true);
				m_currentHoveringGroup = m_hoveringEffectGroupOnClosed;

				m_isOpened = false;

				if (!m_soundOnClosing.IsNull)
				{
					RuntimeManager.PlayOneShot(m_soundOnClosing);
				}
			}

			if (IsHovering)
			{
				m_currentHoveringGroup.EnableEffect();
			}
			else
			{
				m_currentHoveringGroup.DisableEffect();
			}
		}

		protected override void Awake()
		{
			base.Awake();

			m_hoveringEffectGroupOnOpened.Initialize();
			m_hoveringEffectGroupOnClosed.Initialize();
		}

		/// <summary>
		/// 에디터에서 <see cref="m_modelOnClosed"/>와 <see cref="m_modelOnOpened"/> 둘 중 하나만 활성화 되어있어야 하며, 해당 상태를 기준으로 <see cref="m_isOpened"/>와 <see cref="Model"/>을 설정한다.
		/// </summary>
		/// <remarks>
		/// 만일 둘이 동시에 활성화 되어 있는 경우, <see cref="m_modelOnOpened"/>가 활성화 된 것을 시작 상태로 설정한다..
		/// </remarks>
		protected override void Start()
		{
			base.Start();

			bool isStartOpened = m_modelOnOpened.activeSelf;
			bool isStartClosed = m_modelOnClosed.activeSelf;

			if (isStartOpened && !isStartClosed)
			{
				m_isOpened = true;

				m_currentHoveringGroup = m_hoveringEffectGroupOnOpened;
			}
			else if (!isStartOpened && isStartClosed)
			{
				m_isOpened = false;

				m_currentHoveringGroup = m_hoveringEffectGroupOnClosed;
			}
			else
			{
				m_isOpened = true;

				m_currentHoveringGroup = m_hoveringEffectGroupOnOpened;

				m_modelOnOpened.SetActive(true);
				m_modelOnClosed.SetActive(false);
			}
		}

	}
}