using UnityEngine;

namespace BM.Interactables
{
	[CreateAssetMenu(fileName = "FresnelEffectSO_Default", menuName = "BM/SO/Fresnel Effect SO")]
	public class FresnelEffectSO : ScriptableObject
	{
		public Color ColorOnFirst = Color.white;
		public Color ColorOnSecond = Color.grey;

		public float FresnelAmount = .7f;
		public float FresnelContrast = .01f;
		public float FresnelRange = .37f;

		public static readonly string PNAME_IS_USED_FRESNEL = "_IsUsedFresnel";
		public static readonly string PNAME_FRESNEL_COLOR = "_FresnelColor";
		public static readonly string PNAME_FRESNEL_COLOR2 = "_FresnelColor2";
		public static readonly string PNAME_FRESNEL_AMOUNT = "_FresnelAmount";
		public static readonly string PNAME_FRESNEL_CONTRAST = "_FresnelContrast";
		public static readonly string PNAME_FRESNEL_RANGE = "_Fresnelrange";

		public static bool TrySetFresnelEffectToMaterial(Material material, bool enabled)
		{
			if (!material.HasProperty(Shader.PropertyToID(PNAME_IS_USED_FRESNEL)))
			{
				return false;
			}

			material.SetInt(Shader.PropertyToID(PNAME_IS_USED_FRESNEL), enabled ? 1 : 0);
			return true;
		}

		// TODO: Optimization of ID (ID should be cached, and static?)
		public bool TryApplyFresnelDataToMaterial(Material material)
		{
			if (!material.HasProperty(Shader.PropertyToID(PNAME_IS_USED_FRESNEL)))
			{
				return false;
			}

			material.SetColor(Shader.PropertyToID(PNAME_FRESNEL_COLOR), ColorOnFirst);
			material.SetColor(Shader.PropertyToID(PNAME_FRESNEL_COLOR2), ColorOnSecond);
			material.SetFloat(Shader.PropertyToID(PNAME_FRESNEL_AMOUNT), FresnelAmount);
			material.SetFloat(Shader.PropertyToID(PNAME_FRESNEL_CONTRAST), FresnelContrast);
			material.SetFloat(Shader.PropertyToID(PNAME_FRESNEL_RANGE), FresnelRange);

			return true;
		}
	}
}