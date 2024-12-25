using FMODUnity;
using UnityEngine;

namespace BM.Interactables
{
	public class InteractableState_DarackLamp : InteractableStateBase
	{
		[Tooltip("램프를 조정할 때 나는 소리")]
		[SerializeField] private EventReference m_manipulatingSound;

		[Space]

		[SerializeField] private Light m_lightSource;
		[SerializeField] private MeshRenderer m_emissiveRenderer;
		[SerializeField] private string m_emissivePropertyName;

		private bool m_isTurnedOn = true;

		private float m_lightIntensity;

		private Material m_emissiveMaterial;

		public override void StartActivate(InteractAction interactionSubject, InteractableBase interactionObject)
		{
			base.StartActivate(interactionSubject, interactionObject);

			if (m_isTurnedOn)
			{
				m_lightSource.intensity = 0;
				m_isTurnedOn = false;

				m_emissiveMaterial.SetFloat(m_emissiveID, 0);
			}
			else
			{
				m_lightSource.intensity = m_lightIntensity;
				m_isTurnedOn = true;

				m_emissiveMaterial.SetFloat(m_emissiveID, 1);
			}

			if (!m_manipulatingSound.IsNull)
			{
				RuntimeManager.PlayOneShot(m_manipulatingSound);
			}
		}

		private int m_emissiveID;

		protected override void Start()
		{
			base.Start();

			m_lightIntensity = m_lightSource.intensity;
			m_emissiveID = Shader.PropertyToID(m_emissivePropertyName);

			foreach (Material material in m_emissiveRenderer.materials)
			{
				if (!material.HasProperty(m_emissiveID))
				{
					continue;
				}

				m_emissiveMaterial = material;
			}
		}
	}
}