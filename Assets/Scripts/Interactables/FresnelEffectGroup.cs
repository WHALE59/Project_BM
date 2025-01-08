using System;
using System.Collections.Generic;
using UnityEngine;

namespace BM.Interactables
{
	[Serializable]
	public class HoveringEffectGroup
	{
		private int m_hoveringDetectionLayer = 6;

		public List<Collider> colliderGroup = new();
		public FresnelEffectGroup fresnelEffectGroup;
		public void Initialize()
		{
			foreach (Collider collider in colliderGroup)
			{
				collider.gameObject.layer = m_hoveringDetectionLayer;
			}

			if (null != fresnelEffectGroup)
			{
				fresnelEffectGroup.Initialize();
			}
		}

		public void EnableEffect() => fresnelEffectGroup.Enable();
		public void DisableEffect() => fresnelEffectGroup.Disable();
		public void ChangeEffect(FresnelEffectSO newEffect) => fresnelEffectGroup.ChangeEffect(newEffect);
	}

	[Serializable]
	public class FresnelEffectGroup
	{
		public static readonly string PNAME_IS_USED_FRESNEL = "_IsUsedFresnel";
		public static readonly string PNAME_FRESNEL_COLOR = "_FresnelColor";
		public static readonly string PNAME_FRESNEL_COLOR2 = "_FresnelColor2";
		public static readonly string PNAME_FRESNEL_AMOUNT = "_FresnelAmount";
		public static readonly string PNAME_FRESNEL_CONTRAST = "_FresnelContrast";
		public static readonly string PNAME_FRESNEL_RANGE = "_Fresnelrange";

		public List<MeshRenderer> meshGroup;
		public FresnelEffectSO effect;

		private List<Material> m_fresnelMaterials = new();

		public void ChangeEffect(FresnelEffectSO newEffect)
		{
			foreach (Material material in m_fresnelMaterials)
			{
				SetFresnelEffectSOData(material, newEffect);
			}
		}

		public void SetFresnelEffectSOData(Material material, FresnelEffectSO effect)
		{
			material.SetColor(Shader.PropertyToID(PNAME_FRESNEL_COLOR), effect.ColorOnFirst);
			material.SetColor(Shader.PropertyToID(PNAME_FRESNEL_COLOR2), effect.ColorOnSecond);
			material.SetFloat(Shader.PropertyToID(PNAME_FRESNEL_AMOUNT), effect.FresnelAmount);
			material.SetFloat(Shader.PropertyToID(PNAME_FRESNEL_CONTRAST), effect.FresnelContrast);
			material.SetFloat(Shader.PropertyToID(PNAME_FRESNEL_RANGE), effect.FresnelRange);
		}

		public void Initialize()
		{
			foreach (MeshRenderer meshRenderer in meshGroup)
			{
				foreach (Material material in meshRenderer.materials)
				{
					if (!material.HasProperty(Shader.PropertyToID(PNAME_IS_USED_FRESNEL)))
					{
						continue;
					}

					//SetFresnelEffectSOData(material, effect);

					m_fresnelMaterials.Add(material);
				}
			}
		}

		public void Enable()
		{
			foreach (Material material in m_fresnelMaterials)
			{
				SetFresnelEffectSOData(material, effect);
				material.SetInt(Shader.PropertyToID(PNAME_IS_USED_FRESNEL), 1);
			}
		}

		public void Disable()
		{
			foreach (Material material in m_fresnelMaterials)
			{
				material.SetInt(Shader.PropertyToID(PNAME_IS_USED_FRESNEL), 0);
			}
		}
	}
}