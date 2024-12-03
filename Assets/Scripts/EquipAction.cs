using UnityEngine;

namespace BM
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(InteractableDetector))]
	public class EquipAction : MonoBehaviour
	{
		[SerializeField] private InputReaderSO m_inputReader;

		private InteractableDetector m_detector;

		private Collider m_characterCollider;

		private void Awake()
		{
			m_detector = GetComponent<InteractableDetector>();
		}
	}
}