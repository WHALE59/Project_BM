using FMOD.Studio;
using FMODUnity;
using UnityEngine;

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

		private FloorMaterialType m_floorMaterialType = FloorMaterialType.Normal;

		private EventInstance m_normalInstance;
		private EventInstance m_carpetInstance;

		private LocomotiveAction m_locomotiveAction;
		private LocomotiveImpulseGenerator m_locomotiveImpulseGenerator;

		private const string PARAMSHEET_FOOTSTEP_NORMAL = "3D_M_Footsteps_Carpet";
		private const string PARAMSHEET_FOOTSTEP_CARPET = "3D_M_Footstep_Darack";

		// TODO: 이것 보다 더 합리적인 관리 법은 없는 것?
		private readonly string[] LABEL = { "Jog", "Walk", "Crouch" };

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
			// TODO: Tune parameter via locomotive state change
			switch (state)
			{
				case LocomotiveAction.State.Idle:
				case LocomotiveAction.State.NormalJog:

					switch (m_floorMaterialType)
					{
						default:
						case FloorMaterialType.Normal:
							m_normalInstance.setParameterByNameWithLabel(PARAMSHEET_FOOTSTEP_NORMAL, LABEL[0]);
							break;
						case FloorMaterialType.Carpet:
							break;
					}
					break;

				case LocomotiveAction.State.WalkedJog:
					m_normalInstance.setParameterByNameWithLabel(PARAMSHEET_FOOTSTEP_NORMAL, LABEL[1]);
					break;
				case LocomotiveAction.State.CrouchedJog:
					m_normalInstance.setParameterByNameWithLabel(PARAMSHEET_FOOTSTEP_NORMAL, LABEL[2]);
					break;
			}
		}

		private void PlayFootstepAudio(in Vector3 position, in float force)
		{

			var attributes = RuntimeUtils.To3DAttributes(position);

			switch (m_floorMaterialType)
			{
				default:
				case FloorMaterialType.Normal:
					m_normalInstance.set3DAttributes(attributes);
					m_normalInstance.start();
					break;
				case FloorMaterialType.Carpet:
					m_carpetInstance.set3DAttributes(attributes);
					m_carpetInstance.start();
					break;
			}
		}

		private void Awake()
		{
			m_locomotiveAction = GetComponent<LocomotiveAction>();
			m_locomotiveImpulseGenerator = GetComponent<LocomotiveImpulseGenerator>();

			m_normalInstance = RuntimeManager.CreateInstance(m_normalEvent);
			m_carpetInstance = RuntimeManager.CreateInstance(m_carpetEvent);

			// 이게 없으면 경고가 뜹니다.
			m_normalInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
			m_carpetInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
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