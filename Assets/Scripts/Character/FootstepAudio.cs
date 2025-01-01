using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(LocomotiveAction))]
	[RequireComponent(typeof(LocomotiveImpulseGenerator))]
	public class FootstepAudio : MonoBehaviour
	{
		public enum FloorMaterialType
		{
			Normal,
			Carpet
		}

		[SerializeField] private EventReference m_normalEvent;
		[SerializeField] private EventReference m_carpetEvent;

#if UNITY_EDITOR
		[Space()]

		[SerializeField] private bool m_logOnFMODError = false;
#endif

		private FloorMaterialType m_floorMaterialType = FloorMaterialType.Normal;
		private LocomotiveAction.State m_state;

		private EventInstance m_normalInstance;
		private EventInstance m_carpetInstance;

		private LocomotiveAction m_locomotiveAction;
		private LocomotiveImpulseGenerator m_locomotiveImpulseGenerator;

		private const string PARAMSHEET_FOOTSTEP_NORMAL = "GaitState";
		private const string PARAMSHEET_FOOTSTEP_CARPET = "3D_M_Footsteps_Carpet";

		private PARAMETER_ID m_carpetID;
		private PARAMETER_ID m_normalID;

		private readonly string[] LOCOMOTIVE_STATE_PARAMETER_LABEL = { "Jog", "Walk", "Crouch" };

		public void SetFloorMaterial(FloorMaterialType materialType)
		{
			m_floorMaterialType = materialType;
		}

		public void SetDefaultFloorMaterial()
		{
			m_floorMaterialType = FloorMaterialType.Normal;
		}

		private void FootstepAudio_LocomotiveImpulseGenerated(Vector3 position, float force)
		{
			PlayFootstepAudio(in position, in force);
		}

		private void FootstepAudio_LocomotiveStateChanged(LocomotiveAction.State state)
		{
			m_state = state;

			ref EventInstance instance = ref GetInstanceByFloorMaterial(m_floorMaterialType, out PARAMETER_ID parameterID);
			SetLocomotiveStateParameterLabelByID(in instance, in parameterID, m_state);
		}

		private void PlayFootstepAudio(in Vector3 position, in float _)
		{
			ref EventInstance instance = ref GetInstanceByFloorMaterial(m_floorMaterialType, out PARAMETER_ID _);

			ATTRIBUTES_3D attributes = RuntimeUtils.To3DAttributes(position);

			instance.set3DAttributes(attributes);
			instance.start();

		}

		private ref EventInstance GetInstanceByFloorMaterial(FloorMaterialType floorMaterialType, out PARAMETER_ID parameterID)
		{
			switch (floorMaterialType)
			{
				default:
				case FloorMaterialType.Normal:
					parameterID = m_normalID;
					return ref m_normalInstance;

				case FloorMaterialType.Carpet:
					parameterID = m_carpetID;
					return ref m_carpetInstance;

			}
		}

		private void SetLocomotiveStateParameterLabelByID(in EventInstance instance, in PARAMETER_ID parameterID, LocomotiveAction.State state)
		{
			string label = state switch
			{
				LocomotiveAction.State.NormalJog => LOCOMOTIVE_STATE_PARAMETER_LABEL[0],
				LocomotiveAction.State.WalkedJog => LOCOMOTIVE_STATE_PARAMETER_LABEL[1],
				LocomotiveAction.State.CrouchedJog => LOCOMOTIVE_STATE_PARAMETER_LABEL[2],
				_ => "",
			};

			if (instance.setParameterByIDWithLabel(parameterID, label) != FMOD.RESULT.OK)
			{
#if UNITY_EDITOR
				if (m_logOnFMODError)
				{
					Debug.Log("ID에 해당하는 파라미터에서 라벨을 찾을 수 없습니다.");
				}
#endif
			}
		}

		/// <summary>
		/// 이벤트 인스턴스에서 이름을 기반으로 파라미터의 ID를 찾아 반환하는 헬퍼 함수
		/// </summary>
		private static void GetParameterID(in string parameterName, in EventInstance instance, out PARAMETER_ID parameterID)
		{
			if (instance.getDescription(out EventDescription eventDescription) != FMOD.RESULT.OK)
			{
#if UNITY_EDITOR
				Debug.Log($"{instance}의 Event Description을 찾을 수 없습니다.");
#endif
			}

			if (eventDescription.getParameterDescriptionByName(parameterName, out PARAMETER_DESCRIPTION gaitParameterDescription) != FMOD.RESULT.OK)
			{
#if UNITY_EDITOR
				Debug.Log($"{eventDescription}의 {parameterName} Parameter Description을 찾을 수 없습니다.");
#endif
			}

			parameterID = gaitParameterDescription.id;
		}

		private void Awake()
		{
			m_locomotiveAction = GetComponent<LocomotiveAction>();
			m_locomotiveImpulseGenerator = GetComponent<LocomotiveImpulseGenerator>();

			// FMOD 인스턴스 초기화

			m_normalInstance = RuntimeManager.CreateInstance(m_normalEvent);
			m_carpetInstance = RuntimeManager.CreateInstance(m_carpetEvent);

			m_normalInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
			m_carpetInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));

			// 최적화를 위한 FMOD 파라미터 ID 캐싱

			GetParameterID(PARAMSHEET_FOOTSTEP_NORMAL, in m_normalInstance, out m_normalID);
			GetParameterID(PARAMSHEET_FOOTSTEP_CARPET, in m_carpetInstance, out m_carpetID);
		}

		private void OnEnable()
		{
			m_locomotiveAction.LocomotiveStateChanged += FootstepAudio_LocomotiveStateChanged;
			m_locomotiveImpulseGenerator.LocomotiveImpulseGenerated += FootstepAudio_LocomotiveImpulseGenerated;
		}

		private void OnDisable()
		{
			m_locomotiveAction.LocomotiveStateChanged -= FootstepAudio_LocomotiveStateChanged;
			m_locomotiveImpulseGenerator.LocomotiveImpulseGenerated -= FootstepAudio_LocomotiveImpulseGenerated;
		}

		private void OnDestroy()
		{
			if (m_normalInstance.isValid())
			{
				m_normalInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
				m_normalInstance.release();
			}

			if (m_carpetInstance.isValid())
			{
				m_carpetInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
				m_carpetInstance.release();
			}
		}
	}
}