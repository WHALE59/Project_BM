using System;

using UnityEngine;


namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(LocomotiveAction))]
	public class LocomotiveImpulseGenerator : MonoBehaviour
	{
		[SerializeField] private LocomotivePropertySO m_locomotiveProperty;
		[SerializeField] private InputReaderSO m_inputReader;

		private LocomotiveAction m_locomotiveAction;

		public event Action<Vector3, float> LocomotiveImpulseGenerated;

		private float m_elapsedTimeAfterLastImpulse;

		private bool m_hasTriggeredInitialImpulse = false;

		private LocomotiveAction.State m_state;

		private float m_previousInitialImpulseTime;

		[SerializeField] private bool m_logOnLocomotiveImpulse = false;


		private void LocomotiveImpulseGenerator_LocomotiveStateChanged(LocomotiveAction.State state)
		{
			m_state = state;
		}

		private void GenerateLocomotionImpulse()
		{
			// 체공 중이거나, 이동 할 의도가 없거나, 움직이지 않고 있다면 Impulse가 생성되어서는 안 됨.
			if (!m_locomotiveAction.IsGrounded || !m_locomotiveAction.IsDesiredToMove || !m_locomotiveAction.IsMoving)
			{
				return;
			}

			if (m_state == LocomotiveAction.State.Idle)
			{
				return;
			}

			float currentImpulsePeriod = m_locomotiveProperty.GetImpulsePeriodByState(m_state);
			float currentImpulseForce = m_locomotiveProperty.GetImpulseForceByState(m_state);

			if (!m_hasTriggeredInitialImpulse)
			{
				float elapsedTimeAfterLastInitialImpulse = Time.time - m_previousInitialImpulseTime;

				if (elapsedTimeAfterLastInitialImpulse >= currentImpulsePeriod)
				{
#if UNITY_EDITOR
					if (m_logOnLocomotiveImpulse)
					{
						Debug.Log("Initial impulse generated");
					}
#endif

					m_previousInitialImpulseTime = Time.time;
					m_hasTriggeredInitialImpulse = true;

					LocomotiveImpulseGenerated?.Invoke(transform.position, currentImpulseForce);
					m_elapsedTimeAfterLastImpulse = 0f;
				}
				else
				{
#if UNITY_EDITOR
					if (m_logOnLocomotiveImpulse)
					{
						Debug.Log($"Initial impulse blocked {elapsedTimeAfterLastInitialImpulse:F2} < {currentImpulsePeriod:F2}");
					}
#endif
				}

				return;
			}

			// Impulse 타이머

			if (m_elapsedTimeAfterLastImpulse < currentImpulsePeriod)

			{
				m_elapsedTimeAfterLastImpulse += Time.deltaTime;

				if (m_elapsedTimeAfterLastImpulse >= currentImpulsePeriod)
				{
					// 이벤트 발생 및 타이머 초기화
					LocomotiveImpulseGenerated?.Invoke(transform.position, currentImpulseForce);
					m_elapsedTimeAfterLastImpulse = 0f;
				}
			}

			//// currentImpulsePeriod가 매 프레임 변경될 수 있어서, 한번 더 검사해 주어야 함.

			//if (m_elapsedTimeAfterLastImpulse >= currentImpulsePeriod)
			//{
			//	LocomotiveImpulseGenerated?.Invoke(transform.position, currentImpulseForce);
			//	m_elapsedTimeAfterLastImpulse = 0f;
			//}
		}

		private void LocomotiveImpulseGenerator_MoveInputCanceled(Vector2 _)
		{
			m_hasTriggeredInitialImpulse = false;

#if UNITY_EDITOR
			if (m_logOnLocomotiveImpulse)
			{
				Debug.Log("Reset initial impulse");
			}
#endif
		}

		private void Awake()
		{
			m_locomotiveAction = GetComponent<LocomotiveAction>();
		}

		private void OnEnable()
		{
			m_inputReader.MoveInputCanceled += LocomotiveImpulseGenerator_MoveInputCanceled;

			m_locomotiveAction.LocomotiveStateChanged += LocomotiveImpulseGenerator_LocomotiveStateChanged;

#if UNITY_EDITOR
			LocomotiveImpulseGenerated += OnLocomotiveImpulseGenerated;
#endif
		}

		private void OnDisable()
		{
			m_inputReader.MoveInputCanceled -= LocomotiveImpulseGenerator_MoveInputCanceled;

			m_locomotiveAction.LocomotiveStateChanged -= LocomotiveImpulseGenerator_LocomotiveStateChanged;

#if UNITY_EDITOR
			LocomotiveImpulseGenerated -= OnLocomotiveImpulseGenerated;
#endif
		}

		private void Update()
		{
			GenerateLocomotionImpulse();
		}

#if UNITY_EDITOR
		private void OnLocomotiveImpulseGenerated(Vector3 position, float force)
		{
			if (!m_logOnLocomotiveImpulse)
			{
				return;
			}

			Debug.Log($"Impulse generated: {Time.time:F2}, {position:F2}, {force:F2}");
		}
#endif

	}
}