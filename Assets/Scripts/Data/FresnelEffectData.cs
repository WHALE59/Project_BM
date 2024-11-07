using UnityEngine;

namespace BM
{
	[CreateAssetMenu(menuName = "BM/Data/Fresnel Effect Data", fileName = "FresnelEffectData_Default")]
	public class FresnelEffectData : ScriptableObject
	{
		public Color m_primaryColor;
		public Color m_secondaryColor;
		[Range(-1.0f, 1.0f)] public float m_amount;
		[Range(0.01f, 10.0f)] public float m_contrast;
		[Range(0.01f, 10.0f)] public float m_range;
		[Range(0.01f, 10.0f)] public float m_timeSpeed;

		public static void ApplySettingsToMaterial(Material material, FresnelEffectData data)
		{
			material.SetColor("_FresnelColor", data.m_primaryColor);
			material.SetColor("_FresnelColor2", data.m_secondaryColor);
			material.SetFloat("_FresnelAmount", data.m_amount);
			material.SetFloat("_FresnelContrast", data.m_contrast);
			material.SetFloat("_FresnelRange", data.m_range);
			material.SetFloat("_TimeSpeed", data.m_timeSpeed);
		}
	}
}