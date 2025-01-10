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
}
