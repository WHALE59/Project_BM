using UnityEngine;

namespace BM.Interactables
{
	[CreateAssetMenu(menuName = "BM/SO/Activation Events/Darack Lamp", fileName = "ActivationEventSO_DarackLamp")]
	public class ActivationEventSO_DarackLamp : ActivationEventSO
	{
		private bool m_isTurnedOn = true;
		private float m_intensity;

		public override void StartActivation(InteractAction _, InteractableBase sceneInteractable)
		{
			Light lightSource = sceneInteractable.GetComponentInChildren<Light>();

			if (m_isTurnedOn)
			{
				m_intensity = lightSource.intensity;
				lightSource.intensity = 0;
				m_isTurnedOn = false;
			}
			else
			{
				lightSource.intensity = m_intensity;
				m_isTurnedOn = true;
			}
		}
	}
}