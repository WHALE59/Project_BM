using UnityEngine;
using UnityEngine.UI;

namespace BM
{
	[DisallowMultipleComponent]
	public class InteractionCrosshair : MonoBehaviour
	{
		[SerializeField] private Image m_crossHairOnNormal;
		[SerializeField] private Image m_crossHairOnCollectible;
		[SerializeField] private Image m_crossHairOnActivatable;

		public void SetDefaultCrosshair()
		{
			m_crossHairOnNormal.gameObject.SetActive(true);

			m_crossHairOnCollectible.gameObject.SetActive(false);
			m_crossHairOnActivatable.gameObject.SetActive(false);
		}

		public void SetCollectibleCrosshair()
		{
			m_crossHairOnCollectible.gameObject.SetActive(true);

			m_crossHairOnNormal.gameObject.SetActive(false);
			m_crossHairOnActivatable.gameObject.SetActive(false);
		}

		public void SetActivatableCrosshair()
		{
			m_crossHairOnActivatable.gameObject.SetActive(true);

			m_crossHairOnNormal.gameObject.SetActive(false);
			m_crossHairOnCollectible.gameObject.SetActive(false);
		}

		public void SetCrosshair(Sprite sprite)
		{
			m_crossHairOnActivatable.gameObject.SetActive(true);

			m_crossHairOnCollectible.gameObject.SetActive(false);
			m_crossHairOnNormal.gameObject.SetActive(false);

			m_crossHairOnActivatable.sprite = sprite;
		}

		private void Start()
		{
			SetDefaultCrosshair();
		}
	}
}