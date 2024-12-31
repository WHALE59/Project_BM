using FMODUnity;
using UnityEngine;

namespace BM.Interactables
{
	public class DarackLamp : InteractableBase
	{
		[Header("Sound Settings")]

		[SerializeField] private EventReference m_soundOnManipulation;

		[Header("Appearance Settings")]

		[SerializeField] private Light m_lightSource;

		[Tooltip("이미시브 머터리얼이 부착되어 있는 메쉬 렌더러를 할당")]
		[SerializeField] private MeshRenderer m_emissiveRenderer;

		[SerializeField] private string m_emissivePropertyName;

		private bool m_isTurnedOn = true;

		private Material m_emissiveMaterial;
		private int m_emissiveID;

		private float m_initialLightIntensity;
		private float m_initialEmissivePower;

		public override void StartInteraction(InteractAction _)
		{
			base.StartInteraction(_);

			if (m_isTurnedOn)
			{
				m_lightSource.intensity = 0f;
				m_emissiveMaterial.SetFloat(m_emissiveID, 0);

				m_isTurnedOn = false;
			}
			else
			{
				m_lightSource.intensity = m_initialLightIntensity;
				m_emissiveMaterial.SetFloat(m_emissiveID, m_initialEmissivePower);

				m_isTurnedOn = true;
			}

			if (!m_soundOnManipulation.IsNull)
			{
				RuntimeManager.PlayOneShot(m_soundOnManipulation);
			}
		}

		protected override void Awake()
		{
			base.Awake();

			m_emissiveID = Shader.PropertyToID(m_emissivePropertyName);

			foreach (Material material in m_emissiveRenderer.materials)
			{
				if (!material.HasProperty(m_emissiveID))
				{
					continue;
				}

				m_emissiveMaterial = material;
				break;
			}

			m_initialLightIntensity = m_lightSource.intensity;
			m_initialEmissivePower = m_emissiveMaterial.GetFloat(m_emissiveID);
		}
	}
}