using System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using static BM.IA_GameInputs;

namespace BM
{
	[CreateAssetMenu(fileName = "InputReaderSO_Default", menuName = "BM/SO/Input Reader SO")]
	public partial class InputReaderSO : ScriptableObject, IGameplayActions
	{
		/// <summary>
		/// <see cref="IA_GameInputs"/>에 정의된 Action Map을 나타내는 열거형
		/// </summary>
		[Flags]
		private enum EActionMap
		{
			None = 0,
			Gameplay = 1 << 0,
			UI = 1 << 1
		}

		[Tooltip("게임 시작 시 적용될 입력의 Action Map")]
		[SerializeField] private EActionMap m_startActionMap = EActionMap.Gameplay;

		[SerializeField] private bool m_crouchInputIsToggle;
		[SerializeField] private bool m_walkInputIsToggle;

		// Locomotion

		/// <remarks>
		/// Move 입력은 Composite 이기 때문에, 예를 들어 Input Control이 WASD 라면 WASD를 하나의 버튼처럼 취급한다.
		/// </remarks>
		public event UnityAction<Vector2> MoveInputEvent = delegate { };

		/// <summary>
		/// Move 입력 액션이 정확히 끝난 시점을 알고 싶으면 구독한다.
		/// </summary>
		public event UnityAction<Vector2> MoveInputCanceled = delegate { };

		public event UnityAction<Vector2> LookInputEvent = delegate { };

		public event UnityAction CrouchInputPerformed = delegate { };
		public event UnityAction CrouchInputCanceled = delegate { };

		public event UnityAction WalkInputPerformed = delegate { };
		public event UnityAction WalkInputCanceled = delegate { };

		// Interaction

		public event UnityAction UseInputPerformed = delegate { };
		public event UnityAction UseInputCanceled = delegate { };

		public event UnityAction CollectOrActivateInputPerformed = delegate { };
		public event UnityAction CollectOrActivateInputCanceled = delegate { };

		public event UnityAction UnequipInputEvent = delegate { };
		public event UnityAction TraverseEquipmentInputEvent = delegate { };

		// Debug

		public event UnityAction ToggleDeveloperInputPerformed = delegate { };
		public event UnityAction ToggleDeveloperInputCanceled = delegate { };

		private IA_GameInputs m_gameInputs;

		private bool m_crouchInputPerformed = false;
		private bool m_walkInputPerformed = false;

		private void OnEnable()
		{
			/// <see cref="IA_GameInputs"/> 인스턴스가 없으면 생성
			if (m_gameInputs == null)
			{

				m_gameInputs = new();

				m_gameInputs.Gameplay.SetCallbacks(this);
			}

			SetActiveActionMap(m_startActionMap);
		}

		private void OnDisable()
		{
			SetActiveActionMap(EActionMap.None);
		}

		/// <summary>
		/// <paramref name="actionMap"/> 플래그로 들어온 Action Map을 활성화한다. 들어오지 않은 액션 맵은 비활성화 한다.
		/// </summary>
		private void SetActiveActionMap(EActionMap actionMap)
		{
			if (actionMap.HasFlag(EActionMap.Gameplay))
			{
				m_gameInputs.Gameplay.Enable();
			}
			else
			{
				m_gameInputs.Gameplay.Disable();
			}

			// TODO : UI Action Map이 설정되면, 처리할 것
		}


		/* Locomotion */

		void IGameplayActions.OnMove(InputAction.CallbackContext context)
		{
			switch (context.phase)
			{
				case InputActionPhase.Performed:
					MoveInputEvent.Invoke(context.ReadValue<Vector2>());
					break;
				case InputActionPhase.Canceled:
					MoveInputEvent.Invoke(context.ReadValue<Vector2>());

					MoveInputCanceled.Invoke(context.ReadValue<Vector2>());
					break;
			}
		}

		void IGameplayActions.OnLook(InputAction.CallbackContext context)
		{
			LookInputEvent.Invoke(context.ReadValue<Vector2>());
		}

		void IGameplayActions.OnCrouch(InputAction.CallbackContext context)
		{
			if (!m_crouchInputIsToggle)
			{
				switch (context.phase)
				{
					case InputActionPhase.Performed:
						CrouchInputPerformed.Invoke();
						break;

					case InputActionPhase.Canceled:
						CrouchInputCanceled.Invoke();
						break;
				}
			}
			else
			{
				if (context.phase == InputActionPhase.Performed)
				{
					if (!m_crouchInputPerformed)
					{
						CrouchInputPerformed.Invoke();
					}
					else
					{
						CrouchInputCanceled.Invoke();
					}

					m_crouchInputPerformed = !m_crouchInputPerformed;
				}
			}
		}

		void IGameplayActions.OnWalk(InputAction.CallbackContext context)
		{
			if (!m_walkInputIsToggle)
			{
				switch (context.phase)
				{
					case InputActionPhase.Performed:
						WalkInputPerformed.Invoke();
						break;

					case InputActionPhase.Canceled:
						WalkInputCanceled.Invoke();
						break;
				}
			}
			else
			{
				if (context.phase == InputActionPhase.Performed)
				{
					if (!m_walkInputPerformed)
					{
						WalkInputPerformed.Invoke();
					}
					else
					{
						WalkInputCanceled.Invoke();
					}

					m_walkInputPerformed = !m_walkInputPerformed;
				}
			}
		}

		/* Interaction */

		void IGameplayActions.OnUse(InputAction.CallbackContext context)
		{
			switch (context.phase)
			{
				case InputActionPhase.Performed:
					UseInputPerformed.Invoke();
					break;
				case InputActionPhase.Canceled:
					UseInputCanceled.Invoke();
					break;
			}
		}

		void IGameplayActions.OnCollectOrActivate(InputAction.CallbackContext context)
		{
			switch (context.phase)
			{
				case InputActionPhase.Performed:
					CollectOrActivateInputPerformed.Invoke();
					break;
				case InputActionPhase.Canceled:
					CollectOrActivateInputCanceled.Invoke();
					break;
			}
		}


		private bool m_unequipCalled = false;

		void IGameplayActions.OnTraverseEquipmentOrUnequip(InputAction.CallbackContext context)
		{
			switch (context.phase)
			{
				case InputActionPhase.Canceled:
					if (!m_unequipCalled)
					{
						TraverseEquipmentInputEvent.Invoke();
					}
					m_unequipCalled = false;
					break;

				case InputActionPhase.Performed:
					UnequipInputEvent.Invoke();
					m_unequipCalled = true;
					break;
			}
		}

		/* Debug */

		void IGameplayActions.OnToggleDeveloperOverlay(InputAction.CallbackContext context)
		{
			switch (context.phase)
			{
				case InputActionPhase.Performed:
					ToggleDeveloperInputPerformed.Invoke();
					break;
				case InputActionPhase.Canceled:
					ToggleDeveloperInputCanceled.Invoke();
					break;
			}
		}
	}
}
