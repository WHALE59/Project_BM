using UnityEngine;

#if UNITY_EDITOR
#endif

namespace BM
{
	/// <summary>
	/// 모든 상호작용 가능한 오브젝트의 상호작용 내역은, 아래 컴포넌트를 상속하여 구현.
	/// </summary>
	[DisallowMultipleComponent]
	public class InteractableObject : MonoBehaviour, IInteractableObject
	{
		[SerializeField] private InteractableObjectData m_interactableObjectData;

		private Material m_propMaterial;

		InteractableObjectData IInteractableObject.Data => m_interactableObjectData;

		void IInteractableObject.StartHover()
		{
			SetFresnel(true);
		}

		void IInteractableObject.StartInteract()
		{
		}

		void IInteractableObject.FinishHovering()
		{
			SetFresnel(false);
		}

		void IInteractableObject.FinishInteract()
		{
		}

		private void SetFresnel(bool on)
		{
			if (on)
			{
				m_propMaterial.SetInt("_IsUsedFresnel", 1);
			}
			else
			{
				m_propMaterial.SetInt("_IsUsedFresnel", 0);
			}
		}

		private void Awake()
		{
			var meshRenderer = GetComponentInChildren<MeshRenderer>();
			if (meshRenderer)
			{
				m_propMaterial = meshRenderer.material;
			}

			//FresnelEffectData.ApplySettingsToMaterial(m_propMaterial, m_fresnelEffectData);
		}
	}
}
