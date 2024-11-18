using System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace BM
{
	[CreateAssetMenu(fileName = "InputReader_Default", menuName = "BM/Data/Input Reader")]
	public class InputReader : ScriptableObject, IA_GameInputs.IGameplayActions
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

		public event UnityAction<Vector2> MoveInputPerformed = delegate { };
		public event UnityAction<Vector2> MoveInputCancled = delegate { };

		public event UnityAction<Vector2> LookInputEvent = delegate { };

		public event UnityAction CrouchInputPerformed = delegate { };
		public event UnityAction CrouchInputCanceled = delegate { };

		public event UnityAction WalkInputPerformed = delegate { };
		public event UnityAction WalkInputCanceled = delegate { };

		// Interaction 

		public event UnityAction EquipInputTriggered = delegate { };

		public event UnityAction UseInputStarted = delegate { };
		public event UnityAction UseInputFinished = delegate { };

		public event UnityAction TogglePlaceModeInputTriggered = delegate { };

		public event UnityAction PlaceInputTriggered = delegate { };

		public event UnityAction PushPopInventoryInputTriggered = delegate { };
		public event UnityAction PopInventoryInputTriggered = delegate { };

		// Collect 는 'Hold' 인터랙션 속성을 가지고 있음
		public event UnityAction CollectHoldInputStarted = delegate { };
		public event UnityAction CollectHoldInputTriggered = delegate { };

		public event UnityAction ActivateInputStarted = delegate { };
		public event UnityAction ActivateInputFinished = delegate { };

		// Debug

		public event UnityAction ToggleDeveloperInputPerforemed = delegate { };
		public event UnityAction ToggleDeveloperInputCanceled = delegate { };

		private IA_GameInputs m_gameInputs;

		// TODO : Crouch와 Walk에 대한 Toggle을 입력을 변환하는 쪽에서 제어하도록 리팩터링
		private bool m_crouchInputPerformed = false;
		private bool m_walkInputPerformed = false;

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

		void IA_GameInputs.IGameplayActions.OnMove(InputAction.CallbackContext context)
		{
			MoveInputPerformed.Invoke(context.ReadValue<Vector2>());

			if (context.phase == InputActionPhase.Canceled)
			{
				MoveInputCancled.Invoke(context.ReadValue<Vector2>());
			}
		}

		void IA_GameInputs.IGameplayActions.OnLook(InputAction.CallbackContext context)
		{
			LookInputEvent.Invoke(context.ReadValue<Vector2>());
		}

		void IA_GameInputs.IGameplayActions.OnCrouch(InputAction.CallbackContext context)
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

		void IA_GameInputs.IGameplayActions.OnWalk(InputAction.CallbackContext context)
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

		// Interaction

		void IA_GameInputs.IGameplayActions.OnEquip(InputAction.CallbackContext context)
		{
			if (context.phase == InputActionPhase.Performed)
			{
				EquipInputTriggered.Invoke();
			}
		}

		void IA_GameInputs.IGameplayActions.OnUse(InputAction.CallbackContext context)
		{
			switch (context.phase)
			{
				case InputActionPhase.Performed:
					UseInputStarted.Invoke();
					break;
				case InputActionPhase.Canceled:
					UseInputFinished.Invoke();
					break;
			}
		}

		void IA_GameInputs.IGameplayActions.OnTogglePlaceMode(InputAction.CallbackContext context)
		{
			if (context.phase == InputActionPhase.Performed)
			{
				TogglePlaceModeInputTriggered.Invoke();
			}
		}

		void IA_GameInputs.IGameplayActions.OnPlace(InputAction.CallbackContext context)
		{
			if (context.phase == InputActionPhase.Performed)
			{
				PlaceInputTriggered.Invoke();
			}
		}

		void IA_GameInputs.IGameplayActions.OnCollect(InputAction.CallbackContext context)
		{
			switch (context.phase)
			{
				case InputActionPhase.Started:
					CollectHoldInputStarted.Invoke();
					break;
				case InputActionPhase.Performed:
					CollectHoldInputTriggered.Invoke();
					break;
			}
		}

		void IA_GameInputs.IGameplayActions.OnActivate(InputAction.CallbackContext context)
		{
			switch (context.phase)
			{
				case InputActionPhase.Performed:
					ActivateInputStarted.Invoke();
					break;

				case InputActionPhase.Canceled:
					ActivateInputFinished.Invoke();
					break;
			}
		}

		void IA_GameInputs.IGameplayActions.OnPushPopInventory(InputAction.CallbackContext context)
		{
			if (context.phase == InputActionPhase.Performed)
			{
				PushPopInventoryInputTriggered.Invoke();
			}
		}

		// Cheats

		void IA_GameInputs.IGameplayActions.OnToggleDeveloperOverlay(InputAction.CallbackContext context)
		{
			switch (context.phase)
			{
				case InputActionPhase.Performed:
					ToggleDeveloperInputPerforemed.Invoke();
					break;
				case InputActionPhase.Canceled:
					ToggleDeveloperInputCanceled.Invoke();
					break;
			}
		}

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

	}
}
