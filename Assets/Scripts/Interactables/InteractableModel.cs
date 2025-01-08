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
		[SerializeField] private bool m_enableHoveringEffect = true;

		[Space]

		[Tooltip("Fresnel 이펙트를 사용하는 머터리얼이 붙어 있는 메쉬 렌더러 오브젝트를 여기에 할당")]
		[SerializeField] private List<MeshRenderer> m_meshRenderers;

		[Space]
		[SerializeField] private FresnelEffectSO m_fresnelEffectSO;

		private List<Material> m_fresnelMaterials = new();
		private InteractableBase m_interactableBase;

		public InteractableBase InteractableBase => m_interactableBase;

		private void SetFresnelEffect(bool enabled)
		{

#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				foreach (MeshRenderer renderer in m_meshRenderers)
				{
					foreach (Material sharedMaterial in renderer.sharedMaterials)
					{
						//m_fresnelEffectSO.TryApplyFresnelDataToMaterial(sharedMaterial);
						//FresnelEffectSO.TrySetFresnelEffectToMaterial(sharedMaterial, enabled);
					}

					continue;
				}

				return;
			}
#endif

			foreach (Material material in m_fresnelMaterials)
			{
				//FresnelEffectSO.TrySetFresnelEffectToMaterial(material, enabled);
			}
		}

		private void Awake()
		{
			m_interactableBase = transform.parent.GetComponent<InteractableBase>();

			// Set layer for interact action

			foreach (MeshRenderer renderer in m_meshRenderers)
			{
				renderer.gameObject.layer = 6;
			}

			// Cache material reference

			foreach (MeshRenderer renderer in m_meshRenderers)
			{
				foreach (Material material in renderer.materials)
				{
					//m_fresnelEffectSO.TryApplyFresnelDataToMaterial(material);
					m_fresnelMaterials.Add(material);
				}
			}
		}

		public void StartHoveringEffect()
		{
			if (!m_enableHoveringEffect)
			{
				return;
			}

			SetFresnelEffect(enabled: true);
		}

		public void FinishHoveringEffect()
		{
			if (!m_enableHoveringEffect)
			{
				return;
			}

			SetFresnelEffect(enabled: false);
		}
	}
}