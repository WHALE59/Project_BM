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
		// Character ActionMap Events
		// 참고: delegate {} 로 null이 아니게 만들어 줘야 매 입력받을 때마다 null 체크를 생략할 수 있음.

		// Move
		public event UnityAction<Vector2> Moved = delegate { };
		public event UnityAction<Vector2> Looked = delegate { };

		// Crouch
		public event UnityAction CrouchStarted = delegate { };
		public event UnityAction CrouchFinished = delegate { };
		bool _isCrouchPerformed = false;
		[Tooltip("Crouch 입력을 토글로 받을지에 대한 여부입니다.")]
		[SerializeField] bool _isCrouchToggle = false;

		// Walk
		public event UnityAction WalkStarted = delegate { };
		public event UnityAction WalkFinished = delegate { };
		bool _isWalkPerformed = false;
		[Tooltip("Walk 입력을 토글로 받을지에 대한 여부입니다.")]
		[SerializeField] bool _isWalkToggle = false;

		// Interact
		public event UnityAction InteractStarted = delegate { };
		public event UnityAction<double> InteractHolded = delegate { };
		public event UnityAction InteractFinished = delegate { };

		// Use
		public event UnityAction UseStarted = delegate { };
		public event UnityAction<double> UseHolded = delegate { };
		public event UnityAction UseFinished = delegate { };

#if UNITY_EDITOR
		[Header("Visualize Inputs for Debug")]

		InputVisualizer _inputVisualizer;
		[SerializeField] InputVisualizer _inputVisualizerPrefab;
		[SerializeField] bool _shouldVisuzlieInputs = true;
#endif
		void Awake()
		{
			InstantiateInputVisualizer();
		}

#if UNITY_EDITOR
		void OnValidate()
		{
			_inputVisualizer?.gameObject.SetActive(_shouldVisuzlieInputs);
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
			else if (IsContextHolded(context))
			{
				InteractHolded.Invoke(context.duration);
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
			else if (IsContextHolded(context))
			{
				UseHolded.Invoke(context.duration);
			}
			else if (IsContextCancled(context))
			{
				UseFinished.Invoke();
			}
		}

		public void OnCrouch(InputAction.CallbackContext context)
		{
			if (!_isCrouchToggle)
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
					if (!_isCrouchPerformed)
					{
						CrouchStarted.Invoke();
					}
					else
					{
						CrouchFinished.Invoke();
					}

					_isCrouchPerformed = !_isCrouchPerformed;
				}
			}
		}

		public void OnWalk(InputAction.CallbackContext context)
		{
			if (!_isWalkToggle)
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
					if (!_isWalkPerformed)
					{
						WalkStarted.Invoke();
					}
					else
					{
						WalkFinished.Invoke();
					}

					_isWalkPerformed = !_isWalkPerformed;
				}
			}
		}

		bool IsContextPerformed(in InputAction.CallbackContext context) => context.phase == InputActionPhase.Performed;

		bool IsContextCancled(in InputAction.CallbackContext context) => context.phase == InputActionPhase.Canceled;

		// TODO : 의도한 대로 작동하지 않음
		bool IsContextHolded(in InputAction.CallbackContext context) => InputActionPhase.Performed < context.phase && context.phase < InputActionPhase.Canceled;

		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		void InstantiateInputVisualizer()
		{
#if UNITY_EDITOR
			if (!_inputVisualizerPrefab)
			{
				Debug.LogWarning("입력 시각화 오브젝트가 할당되지 않았습니다.");
				return;
			}

			_inputVisualizer = Instantiate(_inputVisualizerPrefab);
			_inputVisualizer.transform.SetParent(transform);

			if (!_shouldVisuzlieInputs)
			{
				_inputVisualizer.gameObject.SetActive(false);
			}
#endif
		}
	}
}