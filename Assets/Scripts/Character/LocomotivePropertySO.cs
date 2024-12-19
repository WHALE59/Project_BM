using System;
using UnityEngine;

namespace BM
{
	[CreateAssetMenu(menuName = "BM/SO/Locomotive Property SO", fileName = "LocomotivePropertySO_Default")]
	public class LocomotivePropertySO : ScriptableObject
	{
		[Serializable]
		public struct CameraNoiseProperty
		{
			[SerializeField] private float m_amplitudeGain;
			[SerializeField] private float m_frequencyGain;

			public float AmplitudeGain => m_amplitudeGain;
			public float FrequencyGain => m_frequencyGain;

			public CameraNoiseProperty(float amplitudeGain, float frequencyGain)
			{
				m_amplitudeGain = amplitudeGain;
				m_frequencyGain = frequencyGain;
			}
		}

		[Serializable]
		public struct LocomotiveProperty
		{
			[SerializeField] private float m_speed;
			[SerializeField] private float m_impulsePeriod;
			[SerializeField] private float m_impulseForce;

			public float Speed => m_speed;
			public float ImpulsePeriod => m_impulsePeriod;
			public float ImpulseForce => m_impulseForce;

			public LocomotiveProperty(float speed, float impulsePeriod, float impulseForce)
			{
				m_speed = speed;
				m_impulsePeriod = impulsePeriod;
				m_impulseForce = impulseForce;
			}
		}

		[Header("Locomotive Properties")]
		[Space()]

		[SerializeField] private LocomotiveProperty m_locomotivePropertyOnNormalJog = new(3f, .7f, .55f);
		[SerializeField] private LocomotiveProperty m_locomotivePropertyOnWalkedJog = new(1.5f, 1f, .3f);
		[SerializeField] private LocomotiveProperty m_locomotivePropertyOnCrouchedJog = new(1.5f, 1f, .2f);

		[Header("Camera Noise Properties")]
		[Space()]

		[SerializeField] private CameraNoiseProperty m_cameraNoisePropertyOnIdle = new(1f, .5f);
		[SerializeField] private CameraNoiseProperty m_cameraNoisePropertyOnNormalJog = new(1f, .5f);
		[SerializeField] private CameraNoiseProperty m_cameraNoisePropertyOnWalkedJog = new(1f, .5f);
		[SerializeField] private CameraNoiseProperty m_cameraNoisePropertyOnCrouchedJog = new(1f, .5f);

		[Header("Crouch Properties")]
		[Space()]

		[SerializeField] private float m_crouchDuration = .25f;
		[SerializeField][Range(.0f, 1f)] private float m_crouchRatio = .5f;

		[Header("Physical Properties")]
		[Space()]

		[SerializeField] private float m_mass = 50f;
		[SerializeField] private bool m_ignoreGravity = false;

		public float Mass => m_mass;

		public bool IgnoreGravity => m_ignoreGravity;

		public float CrouchDuration => m_crouchDuration;

		public float CrouchRatio => m_crouchRatio;

		public float GetSpeedByState(LocomotiveAction.State state) => GetLocomotivePropertyByState(state).Speed;

		public float GetImpulsePeriodByState(LocomotiveAction.State state) => GetLocomotivePropertyByState(state).ImpulsePeriod;

		public float GetImpulseForceByState(LocomotiveAction.State state) => GetLocomotivePropertyByState(state).ImpulseForce;

		public CameraNoiseProperty GetCameraNoisePropertyByState(in LocomotiveAction.State state)
		{
			switch (state)
			{
				default:
				case LocomotiveAction.State.Idle:
					return m_cameraNoisePropertyOnIdle;
				case LocomotiveAction.State.NormalJog:
					return m_cameraNoisePropertyOnNormalJog;
				case LocomotiveAction.State.WalkedJog:
					return m_cameraNoisePropertyOnWalkedJog;
				case LocomotiveAction.State.CrouchedJog:
					return m_cameraNoisePropertyOnCrouchedJog;
			}
		}

		private ref LocomotiveProperty GetLocomotivePropertyByState(in LocomotiveAction.State state)
		{
			switch (state)
			{
				default:
				case LocomotiveAction.State.NormalJog:
					return ref m_locomotivePropertyOnNormalJog;
				case LocomotiveAction.State.WalkedJog:
					return ref m_locomotivePropertyOnWalkedJog;
				case LocomotiveAction.State.CrouchedJog:
					return ref m_locomotivePropertyOnCrouchedJog;
			}
		}
	}
}