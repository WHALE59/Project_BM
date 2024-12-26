using System.Collections.Generic;

using UnityEngine;

namespace BM.Interactables
{
	/// <summary>
	/// 특정한 <see cref="InteractableBase"/>의 외형 정보를 관리한다. <br/> 
	/// 여기서 외형이란, 해당 상호작용 개체의 메쉬(Renderer + Filter)와 콜라이더 이다. <br/>
	/// </summary>
	/// <remarks>
	/// 콜라이더는 이 스크립트가 부착된 오브젝트의 오브젝트 계층 구조에서 Mesh Filter가 존재하는 오브젝트에 부착한다.
	/// </remarks>
	[DisallowMultipleComponent]
	public class InteractableModel : MonoBehaviour
	{
		[Tooltip("Fresnel 이펙트를 사용하는 머터리얼이 붙어 있는 메쉬 렌더러 오브젝트를 여기에 할당")]
		[SerializeField] private List<MeshRenderer> m_meshRenderers;

		private Color m_colorFirst = Color.white;
		private Color m_colorSecond = Color.grey;
		private float m_amount = -.7f;
		private float m_contrast = .01f;
		private float m_range = .37f;



		private List<Material> m_fresnelMaterials = new();
		private const string FRESNEL_PROPERTY_NAME = "_IsUsedFresnel";
		private int m_fresnelID;

		private void SetFresnelEffect(bool enabled)
		{

#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				foreach (MeshRenderer renderer in m_meshRenderers)
				{
					foreach (Material sharedMaterial in renderer.sharedMaterials)
					{
						sharedMaterial.SetInt(Shader.PropertyToID(FRESNEL_PROPERTY_NAME), enabled ? 1 : 0);
					}

					continue;
				}

				return;
			}
#endif

			foreach (Material material in m_fresnelMaterials)
			{
				material.SetInt(m_fresnelID, enabled ? 1 : 0);
			}
		}

		private void Awake()
		{
			// Set layer for interact action

			foreach (MeshRenderer renderer in m_meshRenderers)
			{
				renderer.gameObject.layer = 6;
			}

			m_fresnelID = Shader.PropertyToID(FRESNEL_PROPERTY_NAME);

			// Cache material reference

			foreach (MeshRenderer renderer in m_meshRenderers)
			{
				foreach (Material material in renderer.materials)
				{
					if (!material.HasProperty(m_fresnelID))
					{
						continue;
					}

					material.SetColor("_FresnelColor", m_colorFirst);
					material.SetColor("_FresnelColor2", m_colorSecond);
					material.SetFloat("_FresnelAmount", m_amount);
					material.SetFloat("_FresnelContrast", m_contrast);
					material.SetFloat("_Fresnelrange", m_range);

					m_fresnelMaterials.Add(material);
				}
			}
		}

		public void StartHoveringEffect()
		{
			SetFresnelEffect(enabled: true);
		}

		public void FinishHoveringEffect()
		{
			SetFresnelEffect(enabled: false);
		}
	}
}