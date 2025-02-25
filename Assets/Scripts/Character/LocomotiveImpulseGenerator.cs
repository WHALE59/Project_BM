using System;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
#endif


namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(LocomotiveAction))]
	public class LocomotiveImpulseGenerator : MonoBehaviour
	{
		[SerializeField] private LocomotivePropertySO m_locomotiveProperty;
		[SerializeField] private InputReaderSO m_inputReader;

#if UNITY_EDITOR
		[Space()]

		[SerializeField] private bool m_logOnLocomotiveImpulse = false;
#endif

		public event Action<Vector3, float> LocomotiveImpulseGenerated;

		private LocomotiveAction m_locomotiveAction;

		private float m_elapsedTimeAfterLastImpulse;

		private bool m_hasTriggeredInitialImpulse = false;

		private LocomotiveAction.State m_state;

		private float m_previousInitialImpulseTime;

#if UNITY_EDITOR
		Queue<Tuple<LocomotiveAction.State, Vector3>> m_impulsePositionHistory = new();
#endif

		private void LocomotiveImpulseGenerator_LocomotiveStateChanged(LocomotiveAction.State state)
		{
			m_state = state;
		}

		private void GenerateLocomotionImpulse()
		{
			// 체공 중이거나, 이동 할 의도가 없거나, 움직이지 않고 있다면 Impulse가 생성되어서는 안 됨.

			if (!m_locomotiveAction.IsGrounded)
			{
#if UNITY_EDITOR
				if (m_logOnLocomotiveImpulse)
				{
					Debug.Log("캐릭터가 지면에 닿아있지 않기 때문에 Locomotion Impulse를 발생시키지 않습니다.");
				}
#endif
				return;
			}

			if (!m_locomotiveAction.IsDesiredToMove)
			{
#if UNITY_EDITOR
				if (m_logOnLocomotiveImpulse)
				{
					Debug.Log("캐릭터가 움직이려는 의도가 없기 때문에 Locomotion Impulse를 발생시키지 않습니다.");
				}
#endif
				return;
			}

			if (!m_locomotiveAction.IsMoving)
			{
#if UNITY_EDITOR
				if (m_logOnLocomotiveImpulse)
				{
					Debug.Log("캐릭터가 움직이고 있지 않기 때문에 Locomotion Impulse를 발생시키지 않습니다.");
				}
#endif
				return;
			}


			if (m_state == LocomotiveAction.State.Idle)
			{
#if UNITY_EDITOR
				if (m_logOnLocomotiveImpulse)
				{
					Debug.Log("캐릭터의 Locomotive State가 Idle이기 때문에 Locomotion Impulse를 발생시키지 않습니다.");
				}
#endif
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

#if UNITY_EDITOR
					m_impulsePositionHistory.Enqueue(new(m_state, transform.position));
					if (m_impulsePositionHistory.Count > 20)
					{
						m_impulsePositionHistory.Dequeue();
					}
#endif

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

#if UNITY_EDITOR
					m_impulsePositionHistory.Enqueue(new(m_state, transform.position));
					if (m_impulsePositionHistory.Count > 20)
					{
						m_impulsePositionHistory.Dequeue();
					}
#endif
				}
			}
			else
			{
				m_elapsedTimeAfterLastImpulse = 0f;
			}
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
			LocomotiveImpulseGenerated += Debug_OnLocomotiveImpulseGenerated;
#endif
		}

		private void OnDisable()
		{
			m_inputReader.MoveInputCanceled -= LocomotiveImpulseGenerator_MoveInputCanceled;

			m_locomotiveAction.LocomotiveStateChanged -= LocomotiveImpulseGenerator_LocomotiveStateChanged;

#if UNITY_EDITOR
			LocomotiveImpulseGenerated -= Debug_OnLocomotiveImpulseGenerated;
#endif
		}

		private void Update()
		{
			GenerateLocomotionImpulse();
		}

#if UNITY_EDITOR
		private void Debug_OnLocomotiveImpulseGenerated(Vector3 position, float force)
		{
			if (!m_logOnLocomotiveImpulse)
			{
				return;
			}

			Debug.Log($"Impulse generated: {Time.time:F2}, {position:F2}, {force:F2}");
		}

		[DrawGizmo(GizmoType.Active | GizmoType.Selected)]
		private static void DrawImpulseHistory(LocomotiveImpulseGenerator target, GizmoType _)
		{
			Gizmos.color = Color.magenta;
			foreach ((LocomotiveAction.State state, Vector3 position) in target.m_impulsePositionHistory)
			{
				float radius = 0.2f;

				switch (state)
				{
					case LocomotiveAction.State.Idle:
						radius = 0.1f;
						break;
					case LocomotiveAction.State.NormalJog:
						Gizmos.color = Color.magenta;
						break;
					case LocomotiveAction.State.WalkedJog:
						Gizmos.color = Color.cyan;
						break;
					case LocomotiveAction.State.CrouchedJog:
						Gizmos.color = Color.yellow;
						break;
				}

				Gizmos.DrawWireSphere(position, radius: radius);
			}
		}

#endif

	}
}