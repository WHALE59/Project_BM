using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

namespace BM.Interactables
{
	public class DarackCurtain : InteractableBase
	{
		[Header("Sound Settings")]

		[SerializeField] private EventReference m_soundOnOpening;
		[SerializeField] private EventReference m_soundOnClosing;

		[Header("Opened & Closed Models")]

		[SerializeField] private GameObject m_modelOnOpened;
		[SerializeField] private List<MeshRenderer> m_hoveringRenderersOnOpened;

		[SerializeField] private List<Collider> m_hoveringCollidersOnOpened = new();

		[Space]

		[SerializeField] private GameObject m_modelOnClosed;
		[SerializeField] private List<MeshRenderer> m_hoveringRenderersOnClosed;
		[SerializeField] private List<Collider> m_hoveringCollidersOnClosed = new();

		private bool m_isOpened = false;

		public void SwitchModelToOpen()
		{
			m_modelOnOpened.SetActive(true);
			m_modelOnClosed.SetActive(false);

			m_hoveringRenderers = m_hoveringRenderersOnOpened;

			m_hoveringColliders = m_hoveringCollidersOnOpened;
			SetActiveHoveringColliders(m_hoveringColliders, true);
			SetActiveHoveringColliders(m_hoveringCollidersOnClosed, false);

			if (IsHovering)
			{
				EnableFresnelEffectOnMeshGroup(m_fresnelEffectSO, m_hoveringRenderersOnOpened);
			}
			else
			{
				DisableFresnelEffectOnMeshGroup(m_hoveringRenderersOnOpened);
			}
		}

		public void SwitchModelToClose()
		{
			m_modelOnOpened.SetActive(false);
			m_modelOnClosed.SetActive(true);

			m_hoveringRenderers = m_hoveringRenderersOnClosed;

			m_hoveringColliders = m_hoveringCollidersOnClosed;
			SetActiveHoveringColliders(m_hoveringColliders, true);
			SetActiveHoveringColliders(m_hoveringCollidersOnOpened, false);

			if (IsHovering)
			{
				EnableFresnelEffectOnMeshGroup(m_fresnelEffectSO, m_hoveringRenderersOnClosed);
			}
			else
			{
				DisableFresnelEffectOnMeshGroup(m_hoveringRenderersOnClosed);
			}
		}

		public override void StartInteraction(InteractAction _)
		{
			base.StartInteraction(_);

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

			TrySetHoveringColliderLayer(m_hoveringCollidersOnOpened);
			TrySetHoveringColliderLayer(m_hoveringCollidersOnClosed);

			TrySetHoveringRendererEffectData(m_fresnelEffectSO, m_hoveringRenderersOnClosed);
			TrySetHoveringRendererEffectData(m_fresnelEffectSO, m_hoveringRenderersOnOpened);
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
				m_hoveringRenderers = m_hoveringRenderersOnOpened;

				m_hoveringColliders = m_hoveringCollidersOnOpened;

				SetActiveHoveringColliders(m_hoveringColliders, true);
				SetActiveHoveringColliders(m_hoveringCollidersOnClosed, false);
			}
			else if (!isStartOpened && isStartClosed)
			{
				m_isOpened = false;
				m_hoveringRenderers = m_hoveringRenderersOnClosed;

				SetActiveHoveringColliders(m_hoveringColliders, true);
				SetActiveHoveringColliders(m_hoveringCollidersOnOpened, false);
			}
			else
			{
				m_isOpened = true;
				m_hoveringRenderers = m_hoveringRenderersOnOpened;

				SetActiveHoveringColliders(m_hoveringColliders, true);
				SetActiveHoveringColliders(m_hoveringCollidersOnClosed, false);

				m_modelOnOpened.SetActive(true);
				m_modelOnClosed.SetActive(false);
			}
		}

	}
}