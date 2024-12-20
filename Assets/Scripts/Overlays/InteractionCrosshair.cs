using UnityEngine;
using UnityEngine.UI;

namespace BM
{
	[DisallowMultipleComponent]
	public class InteractionCrosshair : MonoBehaviour
	{
		[SerializeField] private Image m_crossHairOnDefault;
		[SerializeField] private Image m_crossHairOnCollectible;
		[SerializeField] private Image m_crossHairOnEtc;

		public void SetDefaultCrosshair()
		{
			m_crossHairOnDefault.gameObject.SetActive(true);

			m_crossHairOnCollectible.gameObject.SetActive(false);
			m_crossHairOnEtc.gameObject.SetActive(false);
		}

		public void SetCollectibleCrosshair()
		{
			m_crossHairOnCollectible.gameObject.SetActive(true);

			m_crossHairOnDefault.gameObject.SetActive(false);
			m_crossHairOnEtc.gameObject.SetActive(false);
		}

		public void SetCrosshair(Sprite sprite)
		{
			m_crossHairOnEtc.gameObject.SetActive(true);

			m_crossHairOnCollectible.gameObject.SetActive(false);
			m_crossHairOnDefault.gameObject.SetActive(false);

			m_crossHairOnEtc.sprite = sprite;
		}

		private void Start()
		{
			SetDefaultCrosshair();
		}
	}
}