using FMODUnity;
using UnityEngine;

namespace BM.Interactables
{
	public class DarackLamp : InteractableBase
	{
		[Header("Sound Settings")]
		[Space]

		[SerializeField] private EventReference m_soundOnManipulation;

		[Header("Appearance Settings")]
		[Space]

		[SerializeField] private Light m_lightSource;
		[SerializeField] private MeshRenderer m_emissiveRenderer;
		[SerializeField] private string m_emissivePropertyName;

		private bool m_isTurnedOn = true;
		private float m_initialLightIntensity;
		private Material m_emissiveMaterial;
		private int m_emissiveID;

		public override void StartActivation(InteractAction _)
		{
			base.StartActivation(_);

			if (m_isTurnedOn)
			{
				m_lightSource.intensity = 0f;
				m_emissiveMaterial.SetFloat(m_emissiveID, 0);

				m_isTurnedOn = false;
			}
			else
			{
				m_lightSource.intensity = m_initialLightIntensity;
				m_emissiveMaterial.SetFloat(m_emissiveID, 1);

				m_isTurnedOn = true;
			}

			if (!m_soundOnManipulation.IsNull)
			{
				RuntimeManager.PlayOneShot(m_soundOnManipulation);
			}
		}

		protected override void Awake()
		{
			base.Start();

			m_initialLightIntensity = m_lightSource.intensity;
			m_emissiveID = Shader.PropertyToID(m_emissivePropertyName);

			foreach (Material material in m_emissiveRenderer.materials)
			{
				if (!material.HasProperty(m_emissiveID))
				{
					continue;
				}

				m_emissiveMaterial = material;
				return;
			}
		}

	}
}