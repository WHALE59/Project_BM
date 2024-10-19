using UnityEngine;
using Cinemachine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BM
{
	/// <summary>
	/// 입력으로부터 Vector3형 방향 정보를 받아서, 캐릭터를 해당 방향으로 지정 속도만큼 이동시킨다.
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(InputListener))]
	public class MoveAction : MonoBehaviour
	{
		/// <summary>
		/// 카메라 방향을 고려하기 이전, 캐릭터 오브젝트의 지역 방향
		/// </summary>
		Vector3 _localMoveDirection;

		/// <summary>
		/// 카메라 방향과 캐릭터의 이동 속력까지 고려한, 캐릭터 오브젝트의 월드 방향
		/// </summary>
		Vector3 _worldVelocity;

		bool _isDesiredToMove;

		CharacterController _characterController;
		InputListener _inputListener;

		CrouchAction _crouchAction;
		bool _isCrouching = false;

		WalkAction _walkAction;
		bool _isWalking = false;

		CinemachineBasicMultiChannelPerlin _noiseChannel;
		NoiseSetting _currentNoise;

		[Header("이동 속도 설정")]
		[Space()]
		[SerializeField] float _speedOnJog = 10.0f;
		[SerializeField] float _speedOnCrouchedJog = 5.0f;
		[SerializeField] float _speedOnWalk = 5.0f;
		[SerializeField] float _speedOnCrouchedWalk = 2.5f;

		[Header("이동 주체의 물리적 특징 설정")]
		[Space()]

		[Tooltip("캐릭터가 가진 질량입니다. 클수록 캐릭터는 중력의 영향을 크게 받습니다.")]
		[SerializeField] float _mass = 50.0f;

		[Tooltip("캐릭터에게 중력을 적용할 지에 대한 여부입니다.")]
		[SerializeField] bool _applyGravity = true;

		[Header("이동에 따른 진동 설정")]
		[Space()]

		[Tooltip("Cinemachine 가상 카메라에 적용할 노이즈 세팅입니다.")]
		[SerializeField] NoiseSettings _cinemachineNoiseSettingsToApply;
		[SerializeField] NoiseSetting _noiseOnIdle = new(0.0f, 0.0f);
		[SerializeField] NoiseSetting _noiseOnJog = new(1.0f, 0.2f);
		[SerializeField] NoiseSetting _noiseOnCrouchedJog = new(0.7f, 0.15f);
		[SerializeField] NoiseSetting _noiseOnWalk = new(0.7f, 0.15f);
		[SerializeField] NoiseSetting _noiseOnCrouchedWalk = new(0.5f, 0.1f);

		[System.Serializable]
		struct NoiseSetting
		{
			public NoiseSetting(float amplitude, float frequency)
			{
				this.amplitude = amplitude;
				this.frequency = frequency;
			}

			public float amplitude;
			public float frequency;
		}

		void Awake()
		{
			_characterController = GetComponent<CharacterController>();
			_inputListener = GetComponent<InputListener>();

			_crouchAction = GetComponent<CrouchAction>();
			_walkAction = GetComponent<WalkAction>();

#if UNITY_EDITOR
			if (!_walkAction || !_crouchAction)
			{
				Debug.LogWarning("CrouchAction 혹은 WalkAction을 찾을 수 없습니다.");
			}
#endif
		}

		void Start()
		{
			var cine = Camera.main.gameObject.GetComponent<CinemachineBrain>();
			var vcam = cine.ActiveVirtualCamera as CinemachineVirtualCamera;

			_noiseChannel = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

			if (!_noiseChannel)
			{
				_noiseChannel = vcam.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
			}

			_noiseChannel.m_NoiseProfile = _cinemachineNoiseSettingsToApply;

			ApplyNoiseSetting(_noiseOnIdle);
		}

		void UpdateMovementInput(Vector2 moveInput) => _localMoveDirection = new Vector3(moveInput.x, 0.0f, moveInput.y);

		void OnEnable()
		{
			_inputListener.Moved += UpdateMovementInput;

			_crouchAction.CrouchStateStarted += OnCrouchStateStarted;
			_crouchAction.CrouchStateFinished += OnCrouchStateFinished;

			_walkAction.WalkStateStarted += OnWalkStateStarted;
			_walkAction.WalkStateFinished += OnWalkStateFinished;
		}

		void OnDisable()
		{
			_inputListener.Moved -= UpdateMovementInput;

			_crouchAction.CrouchStateStarted -= OnCrouchStateStarted;
			_crouchAction.CrouchStateFinished -= OnCrouchStateFinished;

			_walkAction.WalkStateStarted -= OnWalkStateStarted;
			_walkAction.WalkStateFinished -= OnWalkStateFinished;
		}

		void OnWalkStateStarted() => _isWalking = true;

		void OnWalkStateFinished() => _isWalking = false;

		void OnCrouchStateStarted() => _isCrouching = true;

		void OnCrouchStateFinished() => _isCrouching = false;

		Vector3 WorldMoveDirection
		{
			get
			{
				var cameraYRotation = Quaternion.Euler(0.0f, Camera.main.transform.eulerAngles.y, 0);
				var worldMoveDirection = (cameraYRotation * _localMoveDirection).normalized;
				return worldMoveDirection;
			}
		}

		void FixedUpdate()
		{
			// 현재 프레임의 속도 계산 시작

			_worldVelocity = WorldMoveDirection * Speed;

			if (_applyGravity)
			{
				if (_characterController.isGrounded)
				{
					_worldVelocity.y = -0.1f;
				}
				else
				{
					_worldVelocity.y += _mass * Physics.gravity.y * Time.fixedDeltaTime;
				}
			}
			else
			{
				_worldVelocity.y = 0.0f;
			}

			// 현재 프레임의 속도 계산 끝

			var planeVelocity = new Vector3(_worldVelocity.x, 0.0f, _worldVelocity.z);
			_isDesiredToMove = planeVelocity != Vector3.zero;
			_characterController.Move(_worldVelocity * Time.fixedDeltaTime);

			// 카메라 노이즈 업데이트

			var noise = _isDesiredToMove ? Noise : _noiseOnIdle;
			ApplyNoiseSetting(noise);

			// 회전 업데이트

			var targetRotation = Camera.main.transform.eulerAngles.y;
			transform.rotation = Quaternion.Euler(0.0f, targetRotation, 0.0f);
		}

		void ApplyNoiseSetting(in NoiseSetting noiseSetting)
		{
			_noiseChannel.m_AmplitudeGain = noiseSetting.amplitude;
			_noiseChannel.m_FrequencyGain = noiseSetting.frequency;
		}

#if UNITY_EDITOR
		/// <summary>
		/// 캐릭터의 전방을 그린다.
		/// </summary>
		[DrawGizmo(GizmoType.Active)]
		static void DrawForwardGizmo(MoveAction target, GizmoType _)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(target.transform.position, target.transform.forward * 1.0f);
		}
#endif
		NoiseSetting Noise
		{
			get
			{
				if (!_isCrouching && !_isWalking)
				{
					return _noiseOnJog;
				}
				else if (!_isCrouching && _isWalking)
				{
					return _noiseOnWalk;
				}
				else if (_isCrouching && !_isWalking)
				{
					return _noiseOnCrouchedJog;
				}
				else // (_isCrouching && !_isWalking)
				{
					return _noiseOnCrouchedWalk;
				}
			}
		}

		float Speed
		{
			get
			{
				if (!_isCrouching && !_isWalking)
				{
					return _speedOnJog;
				}
				else if (!_isCrouching && _isWalking)
				{
					return _speedOnWalk;
				}
				else if (_isCrouching && !_isWalking)
				{
					return _speedOnCrouchedJog;
				}
				else // (_isCrouching && !_isWalking)
				{
					return _speedOnCrouchedWalk;
				}
			}
		}
	}
}
