using System;
using System.Collections.Generic;

using UnityEngine;

namespace BM.Interactables
{
	/// <summary>
	/// 특정한 <see cref="InteractableBase"/>의 외형 정보를 관리한다. <br/> 
	/// 여기서 외형이란, 해당 상호작용 개체의 메쉬(Renderer + Filter)와 콜라이더 이다.
	/// </summary>
	/// <remarks>
	/// 이 클래스는 스스로는 아무 것도 하지 않고, 오직 <see cref="InteractableBase"/>의 명령만을 수행한다.
	/// </remarks>
	[DisallowMultipleComponent]
	public class InteractableModel : MonoBehaviour
	{
		[Tooltip("Fresnel 이펙트를 사용하는 머터리얼이 붙어 있는 메쉬 렌더러 오브젝트를 여기에 할당")]
		[SerializeField] private MeshRenderer m_meshRenderer;

		private Material m_fresnelMaterial;

		private void Awake()
		{
			m_fresnelMaterial = m_meshRenderer.material;
		}

		public void StartHoveringEffect()
		{
			m_fresnelMaterial.SetInt("_IsUsedFresnel", 1);
		}

		public void FinishHoveringEffect()
		{
			m_fresnelMaterial.SetInt("_IsUsedFresnel", 0);
		}
	}
}