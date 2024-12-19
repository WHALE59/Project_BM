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
			[Serializable]
			public struct CameraNoiseProperty
			{
				[SerializeField] private float m_amplitudeGain;
				[SerializeField] private float m_frequencyGain;

				public CameraNoiseProperty(float amplitudeGain, float frequencyGain)
				{
					m_amplitudeGain = amplitudeGain;
					m_frequencyGain = frequencyGain;
				}
			}

			[SerializeField] private float m_speed;
			[SerializeField] private float m_impulsePeriod;
			[SerializeField] private float m_impulseForce;
			[SerializeField] private CameraNoiseProperty m_cameraNoise;

			public float Speed => m_speed;
			public float ImpulsePeriod => m_impulsePeriod;
			public float ImpulseForce => m_impulseForce;
			public CameraNoiseProperty CameraNoise => m_cameraNoise;

			public LocomotiveProperty(float speed, float impulsePeriod, float impulseForce, float amplitudeGain, float frequencyGain)
			{
				m_speed = speed;
				m_impulsePeriod = impulsePeriod;
				m_impulseForce = impulseForce;

				m_cameraNoise = new CameraNoiseProperty(amplitudeGain, frequencyGain);
			}
		}

		[SerializeField] private LocomotiveProperty m_propertyOnJog = new(6f, .55f, .55f, 1f, .5f);
		[SerializeField] private LocomotiveProperty m_propertyOnWalk = new(4f, .7f, .4f, 1f, .5f);
		[SerializeField] private LocomotiveProperty m_propertyOnCrouch = new(2f, .8f, .3f, 1f, .5f);

		[Space()]

		[SerializeField] private float m_crouchDuration = .25f;
		[SerializeField][Range(.0f, 1f)] private float m_crouchRatio = .5f;

		[Space()]

		[SerializeField] private float m_mass = 50f;
		[SerializeField] private bool m_ignoreGravity = false;

		public float Mass => m_mass;

		public bool IgnoreGravity => m_ignoreGravity;

		public float CrouchDuration => m_crouchDuration;

		public float CrouchRatio => m_crouchRatio;

		public float GetSpeedByState(LocomotiveAction.State state) => this[state].Speed;

		public float GetImpulsePeriodByState(LocomotiveAction.State state) => this[state].ImpulsePeriod;

		public float GetImpulseForceByState(LocomotiveAction.State state) => this[state].ImpulseForce;

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