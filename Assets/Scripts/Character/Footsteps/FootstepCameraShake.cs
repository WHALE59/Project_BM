using Unity.Cinemachine;
using UnityEngine;

using ImpulseShapes = Unity.Cinemachine.CinemachineImpulseDefinition.ImpulseShapes;

namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(LocomotiveAction))]
	[RequireComponent(typeof(CinemachineImpulseSource))]
	public class FootstepCameraShake : MonoBehaviour
	{
		[SerializeField] private bool m_applyShake = true;
		[SerializeField][Range(0.0f, 1.0f)] private float m_masterForce = 0.55f;

		[Space()]

		[SerializeField] private ImpulseShapes m_shakeShape = ImpulseShapes.Bump;
		[SerializeField] private float m_shakeDuration = 0.25f;
		[SerializeField] private Vector3 m_shakeVelocity = new(0.0f, -0.125f, 0.0f);

		private LocomotiveAction m_locomotiveAction;
		private CinemachineImpulseSource m_impulseSource;

		private void OnLocomotiveImpulseGenerated(Vector3 _, float force)
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
				m_impulseSource.ImpulseDefinition.ImpulseShape = m_shakeShape;
				m_impulseSource.ImpulseDefinition.ImpulseDuration = m_shakeDuration;
				m_impulseSource.DefaultVelocity = m_shakeVelocity;
			}
		}
#endif

		private void Awake()
		{
			m_locomotiveAction = GetComponent<LocomotiveAction>();
			m_impulseSource = GetComponent<CinemachineImpulseSource>();

		}

		private void OnEnable()
		{
			m_locomotiveAction.LocomotionImpulseGenerated += OnLocomotiveImpulseGenerated;
		}

		private void Start()
		{
			m_impulseSource.ImpulseDefinition.ImpulseShape = m_shakeShape;
			m_impulseSource.ImpulseDefinition.ImpulseDuration = m_shakeDuration;
			m_impulseSource.DefaultVelocity = m_shakeVelocity;
		}

		private void OnDisable()
		{
			m_locomotiveAction.LocomotionImpulseGenerated -= OnLocomotiveImpulseGenerated;
		}
	}
}