using UnityEngine;
using Cinemachine;

using ImpulseShapes = Cinemachine.CinemachineImpulseDefinition.ImpulseShapes;

namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(LocomotiveImpulseGenerator))]
	[RequireComponent(typeof(CinemachineImpulseSource))]
	public class FootstepCameraShake : MonoBehaviour
	{
		[SerializeField] private bool m_applyShake = true;
		[SerializeField][Range(0.0f, 1.0f)] private float m_masterForce = .55f;

		[Space()]

		[SerializeField] private ImpulseShapes m_shakeShape = ImpulseShapes.Bump;
		[SerializeField] private float m_shakeDuration = 0.25f;
		[SerializeField] private Vector3 m_shakeVelocity = new(0f, -.125f, 0f);

		private LocomotiveImpulseGenerator m_locomotiveImpulseGenerator;
		private CinemachineImpulseSource m_impulseSource;

		private void FootstepCameraShake_LocomotiveImpulseGenerated(Vector3 _, float force)
		{
			if (m_applyShake)
			{
				m_impulseSource.GenerateImpulse(force * m_masterForce);
			}
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (m_impulseSource)
			{
				m_impulseSource.m_ImpulseDefinition.m_ImpulseShape = m_shakeShape;
				m_impulseSource.m_ImpulseDefinition.m_ImpulseDuration = m_shakeDuration;
				m_impulseSource.m_DefaultVelocity = m_shakeVelocity;
			}
		}
#endif

		private void Awake()
		{
			m_locomotiveImpulseGenerator = GetComponent<LocomotiveImpulseGenerator>();
			m_impulseSource = GetComponent<CinemachineImpulseSource>();

		}

		private void OnEnable()
		{
			m_locomotiveImpulseGenerator.LocomotiveImpulseGenerated += FootstepCameraShake_LocomotiveImpulseGenerated;
		}

		private void Start()
		{
			m_impulseSource.m_ImpulseDefinition.m_ImpulseShape = m_shakeShape;
			m_impulseSource.m_ImpulseDefinition.m_ImpulseDuration = m_shakeDuration;
			m_impulseSource.m_DefaultVelocity = m_shakeVelocity;
		}

		private void OnDisable()
		{
			m_locomotiveImpulseGenerator.LocomotiveImpulseGenerated -= FootstepCameraShake_LocomotiveImpulseGenerated;
		}
	}
}