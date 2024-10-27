using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;


#if UNITY_EDITOR
using UnityEngine.InputSystem.Samples;
#endif

namespace BM
{

	/// <summary>
	/// 입력 애셋을 생성하고, 입력과 각 행동 컴포넌트를 연결한다.
	/// </summary>
	[DisallowMultipleComponent]
	public class InputListener : MonoBehaviour
	{
		public event UnityAction<Vector2> Moved = delegate { };
		public event UnityAction<Vector2> Looked = delegate { };

		public event UnityAction CrouchStarted = delegate { };
		public event UnityAction CrouchFinished = delegate { };

		public event UnityAction WalkStarted = delegate { };
		public event UnityAction WalkFinished = delegate { };

		public event UnityAction InteractStarted = delegate { };
		public event UnityAction InteractFinished = delegate { };

		public event UnityAction UseStarted = delegate { };
		public event UnityAction UseFinished = delegate { };

		private bool m_isCrouchPerformed = false;
		private bool m_isWalkPerformed = false;

		[Tooltip("Crouch 입력을 토글로 받을지에 대한 여부입니다.")]
		[SerializeField] private bool m_isCrouchToggle = false;

		[Tooltip("Walk 입력을 토글로 받을지에 대한 여부입니다.")]
		[SerializeField] private bool m_isWalkToggle = false;

		private bool IsContextPerformed(in InputAction.CallbackContext context) => context.phase == InputActionPhase.Performed;

		private bool IsContextCancled(in InputAction.CallbackContext context) => context.phase == InputActionPhase.Canceled;

#if UNITY_EDITOR
		[Header("Visualize Inputs for Debug")]

		private InputVisualizer m_inputVisualizer;
		[SerializeField] private InputVisualizer m_inputVisualizerPrefab;
		[SerializeField] private bool m_shouldVisuzlieInputs = true;
#endif
		private void Awake()
		{
			InstantiateInputVisualizer();
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			m_inputVisualizer?.gameObject.SetActive(m_shouldVisuzlieInputs);
		}
#endif

		public void OnMove(InputAction.CallbackContext context)
		{
			Moved.Invoke(context.ReadValue<Vector2>());
		}

		public void OnLook(InputAction.CallbackContext context)
		{
			Looked.Invoke(context.ReadValue<Vector2>());
		}

		public void OnInteract(InputAction.CallbackContext context)
		{
			if (IsContextPerformed(context))
			{
				InteractStarted.Invoke();
			}
			else if (IsContextCancled(context))
			{
				InteractFinished.Invoke();
			}
		}

		public void OnUse(InputAction.CallbackContext context)
		{
			if (IsContextPerformed(context))
			{
				UseStarted.Invoke();
			}
			else if (IsContextCancled(context))
			{
				UseFinished.Invoke();
			}
		}

		public void OnCrouch(InputAction.CallbackContext context)
		{
			if (!m_isCrouchToggle)
			{
				if (IsContextPerformed(context))
				{
					CrouchStarted.Invoke();
				}
				else if (IsContextCancled(context))
				{
					CrouchFinished.Invoke();
				}
			}
			else
			{
				if (IsContextPerformed(context))
				{
					if (!m_isCrouchPerformed)
					{
						CrouchStarted.Invoke();
					}
					else
					{
						CrouchFinished.Invoke();
					}

					m_isCrouchPerformed = !m_isCrouchPerformed;
				}
			}
		}

		public void OnWalk(InputAction.CallbackContext context)
		{
			if (!m_isWalkToggle)
			{
				if (IsContextPerformed(context))
				{
					WalkStarted.Invoke();
				}
				else if (IsContextCancled(context))
				{
					WalkFinished.Invoke();
				}
			}
			else
			{
				if (IsContextPerformed(context))
				{
					if (!m_isWalkPerformed)
					{
						WalkStarted.Invoke();
					}
					else
					{
						WalkFinished.Invoke();
					}

					m_isWalkPerformed = !m_isWalkPerformed;
				}
			}
		}


		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		void InstantiateInputVisualizer()
		{
#if UNITY_EDITOR
			if (!m_inputVisualizerPrefab)
			{
				Debug.LogWarning("입력 시각화 오브젝트가 할당되지 않았습니다.");
				return;
			}

			m_inputVisualizer = Instantiate(m_inputVisualizerPrefab);
			m_inputVisualizer.transform.SetParent(transform);

			if (!m_shouldVisuzlieInputs)
			{
				m_inputVisualizer.gameObject.SetActive(false);
			}
#endif
		}
	}
}