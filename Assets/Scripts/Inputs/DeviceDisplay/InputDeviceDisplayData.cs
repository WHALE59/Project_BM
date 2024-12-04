using UnityEngine;

namespace BM
{
	[CreateAssetMenu(fileName = "InputDeviceDisplayData_DeviceName", menuName = "BM/Data/Input Device Display Data")]
	public class InputDeviceDisplayData : ScriptableObject
	{
		public string displayName = "Input Device Name";

		public Sprite ButtonNorth;
		public Sprite ButtonSouth;
		public Sprite ButtonWest;
		public Sprite ButtonEast;

		public Sprite TriggerRightFront;
		public Sprite TriggerRightBack;
		public Sprite TriggerLeftFront;
		public Sprite TriggerLeftBack;

		public Sprite ClickRightStick;
		public Sprite ClickLeftStick;
	}

}