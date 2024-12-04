using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BM
{
	[CreateAssetMenu(fileName = "InputDeviceDisplayConfigurator", menuName = "BM/Data/Input Device Display Configurator")]
	public class InputDeviceDisplayConfigurator : ScriptableObject
	{
		[System.Serializable]
		public struct InputDeviceSet
		{
			public string rawPath;
			public InputDeviceDisplayData displayData;
		}

		public List<InputDeviceSet> inputDeviceSets = new();

		public string GetInputDeviceName(PlayerInput playerInput)
		{
			var currentRawPath = playerInput.devices.FirstOrDefault().ToString();
			string newDisplayName = null;

			foreach (var inputDeviceSet in inputDeviceSets)
			{
				if (inputDeviceSet.rawPath != currentRawPath)
				{
					continue;
				}

				newDisplayName = inputDeviceSet.displayData.displayName;
			}

			return newDisplayName ??= currentRawPath;
		}

		public Sprite GetInputDeviceBindIcon(PlayerInput playerInput, string playerInputDeviceInputBinding)
		{
			var currentRawPath = playerInput.devices.FirstOrDefault().ToString();

			Sprite displayIcon = null;

			foreach (var inputDeviceSet in inputDeviceSets)
			{
				if (inputDeviceSet.rawPath != currentRawPath)
				{
					continue;
				}

				displayIcon = FilterForDeviceInputBiding(inputDeviceSet, playerInputDeviceInputBinding);
			}

			return displayIcon;
		}

		Sprite FilterForDeviceInputBiding(InputDeviceSet target, string inputBinding)
		{
			var data = target.displayData;

			var spriteIcon = inputBinding switch
			{
				"Button North" => data.ButtonNorth,
				"Button South" => data.ButtonSouth,
				"Button West" => data.ButtonWest,
				"Button East" => data.ButtonEast,
				"Right Shoulder" => data.TriggerRightFront,
				"Right Trigger" => data.TriggerRightBack,
				"rightTriggerButton" => data.TriggerRightBack,
				"Left Shoulder" => data.TriggerLeftFront,
				"Left Trigger" => data.TriggerLeftBack,
				"leftTriggerButton" => data.TriggerLeftBack,
				_ => null // TODO
			};

			return spriteIcon;
		}
	}
}