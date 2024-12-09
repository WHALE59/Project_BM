using System;
using UnityEngine;

namespace BM
{
	[CreateAssetMenu(menuName = "BM/SO/Locomotive Property SO", fileName = "LocomotivePropertySO_Default")]
	public class LocomotivePropertySO : ScriptableObject
	{
		[Serializable]
		public struct LocomotiveProperty
		{
			public float Speed;
			public float ImpulsePeriod;
			public float ImpulseForce;
			public LocomotiveProperty(float speed, float impulsePeriod, float impulseForce)
			{
				Speed = speed;
				ImpulsePeriod = impulsePeriod;
				ImpulseForce = impulseForce;
			}
		}

		[SerializeField] private LocomotiveProperty m_propertyOnJog = new(6f, .55f, .55f);
		[SerializeField] private LocomotiveProperty m_propertyOnWalk = new(4f, .7f, .4f);
		[SerializeField] private LocomotiveProperty m_propertyOnCrouch = new(2f, .8f, .3f);

		[Space()]

		[SerializeField] private float m_crouchDuration = .25f;
		[SerializeField][Range(.0f, 1f)] private float m_crouchRatio = .5f;

		[Space()]

		[SerializeField] private float m_mass = 50f;
		[SerializeField] private bool m_ignoreGravity = false;

		public float GetMass() => m_mass;

		public bool GetIgnoreGravity() => m_ignoreGravity;

		public float GetCrouchDuration() => m_crouchDuration;

		public float GetCrouchRatio() => m_crouchRatio;

		public float GetSpeed(LocomotiveAction.State state) => this[state].Speed;

		public float GetImpulsePeriod(LocomotiveAction.State state) => this[state].ImpulsePeriod;

		public float GetImpulseForce(LocomotiveAction.State state) => this[state].ImpulseForce;

		private ref LocomotiveProperty this[in LocomotiveAction.State state]
		{
			get
			{
				switch (state)
				{
					default:
					case LocomotiveAction.State.Jog:
						return ref m_propertyOnJog;
					case LocomotiveAction.State.Walk:
						return ref m_propertyOnWalk;
					case LocomotiveAction.State.Crouch:
						return ref m_propertyOnCrouch;
				}
			}
		}

	}
}