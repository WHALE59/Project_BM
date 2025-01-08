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
	}
}