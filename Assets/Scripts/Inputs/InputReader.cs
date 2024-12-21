using System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace BM
{
	[CreateAssetMenu(fileName = "InputReader_Default", menuName = "BM/Data/Input Reader")]
	public class InputReader : ScriptableObject, IA_GameInputs.IGameplayActions, IA_GameInputs.IGamePlay_UIActions
	{
		/// <summary>
		/// <see cref="IA_GameInputs"/>에 정의된 Action Map을 나타내는 열거형
		/// </summary>
		[Flags]
		public enum EActionMap
		{
			None = 0,
			Gameplay = 1 << 0,
			Gameplay_UI = 1 << 1
		}

		[Tooltip("게임 시작 시 적용될 입력의 Action Map")]
		[SerializeField] private EActionMap m_startActionMap = EActionMap.Gameplay;

		[SerializeField] private bool m_crouchInputIsToggle;
		[SerializeField] private bool m_walkInputIsToggle;

		// Gameplay
		public event UnityAction<Vector2> MoveInputPerformed = delegate { };
		public event UnityAction<Vector2> MoveInputCancled = delegate { };

		public event UnityAction<Vector2> LookInputEvent = delegate { };

		public event UnityAction CrouchInputPerformed = delegate { };
		public event UnityAction CrouchInputCanceled = delegate { };

		public event UnityAction WalkInputPerformed = delegate { };
		public event UnityAction WalkInputCanceled = delegate { };

		public event UnityAction InteractInputPerformed = delegate { };
		public event UnityAction InteractInputCanceled = delegate { };

		public event UnityAction UseInputPerformed = delegate { };
		public event UnityAction UseInputCanceled = delegate { };

		public event UnityAction ToggleDeveloperInputPerforemed = delegate { };
		public event UnityAction ToggleDeveloperInputCanceled = delegate { };

		public event UnityAction Open_InventoryPerformed = delegate { };
		public event UnityAction Open_InventoryCanceled = delegate { };




		// Gameplay_UI
		public event UnityAction Close_InventoryPerformed = delegate { };
		public event UnityAction Close_InventoryCanceled = delegate { };

		public event UnityAction Next_PagePerformed = delegate { };
		public event UnityAction Next_PageCanceled = delegate { };

		public event UnityAction Previous_PagePerformed = delegate { };
		public event UnityAction Previous_PageCanceled = delegate { };




		private IA_GameInputs m_gameInputs;

		// TODO : Crouch와 Walk에 대한 Toggle을 입력을 변환하는 쪽에서 제어하도록 리팩터링
		private bool m_crouchInputPerformed = false;
		private bool m_walkInputPerformed = false;

		/// <summary>
		/// <paramref name="actionMap"/> 플래그로 들어온 Action Map을 활성화한다. 들어오지 않은 액션 맵은 비활성화 한다.
		/// </summary>
		public void SetActiveActionMap(EActionMap actionMap)
		{
			if (actionMap.HasFlag(EActionMap.Gameplay))
			{
				m_gameInputs.Gameplay.Enable();
			}
			else
			{
				m_gameInputs.Gameplay.Disable();
			}

			if (actionMap.HasFlag(EActionMap.Gameplay_UI))
			{
				m_gameInputs.GamePlay_UI.Enable();
			}
			else
			{
				m_gameInputs.GamePlay_UI.Disable();
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

		void IA_GameInputs.IGameplayActions.OnInteract(InputAction.CallbackContext context)
		{
			switch (context.phase)
			{
				case InputActionPhase.Performed:
					InteractInputPerformed.Invoke();
					break;

				case InputActionPhase.Canceled:
					InteractInputCanceled.Invoke();
					break;
			}
		}

		void IA_GameInputs.IGameplayActions.OnUse(InputAction.CallbackContext context)
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


		void IA_GameInputs.IGameplayActions.OnOpen_Inventory(InputAction.CallbackContext context)
		{
			switch (context.phase)
			{
				case InputActionPhase.Performed:
					Open_InventoryPerformed.Invoke();
					break;
				case InputActionPhase.Canceled:
					Open_InventoryCanceled.Invoke();
					break;
			}
			return;
		}

		void IA_GameInputs.IGamePlay_UIActions.OnClose_Inventory(InputAction.CallbackContext context)
		{
			switch (context.phase)
			{
				case InputActionPhase.Performed:
					Close_InventoryPerformed.Invoke();
					break;
				case InputActionPhase.Canceled:
					Close_InventoryCanceled.Invoke();
					break;
			}
			return;
		}

		void IA_GameInputs.IGamePlay_UIActions.OnNext_Page(UnityEngine.InputSystem.InputAction.CallbackContext context)
		{
			switch (context.phase)
			{
				case InputActionPhase.Performed:
					Next_PagePerformed.Invoke();
					break;
				case InputActionPhase.Canceled:
					Next_PageCanceled.Invoke();
					break;
			}
			return;
		}

		void IA_GameInputs.IGamePlay_UIActions.OnPrevious_Page(UnityEngine.InputSystem.InputAction.CallbackContext context)
		{
			switch (context.phase)
			{
				case InputActionPhase.Performed:
					Previous_PagePerformed.Invoke();
					break;
				case InputActionPhase.Canceled:
					Previous_PageCanceled.Invoke();
					break;
			}
			return;
		}


		private void OnEnable()
		{
			/// <see cref="IA_GameInputs"/> 인스턴스가 없으면 생성
			if (m_gameInputs is null)
			{
				m_gameInputs = new();

				m_gameInputs.Gameplay.SetCallbacks(this);
				m_gameInputs.GamePlay_UI.SetCallbacks(this);
			}

			SetActiveActionMap(m_startActionMap);
		}

		private void OnDisable()
		{
			SetActiveActionMap(EActionMap.None);
		}

	}
}
