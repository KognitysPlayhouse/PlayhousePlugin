using Exiled.API.Features;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlayhousePlugin.Components
{
	public class RainbowLightController : MonoBehaviour
	{
		public static readonly List<RainbowLightController> Instances = new List<RainbowLightController>();
		public float _saturation = 1f;
		public float _hueShiftSpeed = 0.2f;
		public float _value = 1f;

		private Room _room;
		public Room room
		{
			get
			{
				if (_room == null)
					_room = Room.List.Where(x => x.gameObject == gameObject).FirstOrDefault();
				return _room;
			}
		}

		private void Awake()
		{
			Instances.Add(this);
		}

		private void OnDestroy()
		{
			Instances.Remove(this);
		}

		private void Update()
		{
			float amountToShift = _hueShiftSpeed * Time.deltaTime;
			Color newColor = ShiftHueBy(room.Color, amountToShift);
			room.Color = newColor;
		}

		private Color ShiftHueBy(Color color, float amount)
		{
			// convert from RGB to HSV
			Color.RGBToHSV(color, out float hue, out float sat, out float val);

			// shift hue by amount
			hue += amount;
			sat = _saturation;
			val = _value;

			// convert back to RGB and return the color
			return Color.HSVToRGB(hue, sat, val);
		}
	}
}
