using System;
using System.Collections.Generic;
using System.Diagnostics;
using Exiled.API.Features;
using MapGeneration;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace PlayhousePlugin.Components
{
	public class LightBlink : MonoBehaviour
	{
		public float minFlickerTimeRange = 0.1f;
		public float maxFlickerTimeRange = 1.5f;
		public float turnOnSpeed = 0.3f;
		public float maxIntensityDecreaseMultiplier = 0.5f;
		public float offset = 0;
		
		private Light _light;
		private float _frequency;

		public static List<LightBlink> lights = new List<LightBlink>();

		private void Awake()
		{
			_light = GetComponent<Light>();
			lights.Add(this);
			
			if(_light == null)
				Destroy(this);
		}

		private void FixedUpdate()
		{
			_frequency -= Time.deltaTime;

			if (_frequency <= 0)
			{
				_frequency = Random.Range(minFlickerTimeRange, maxFlickerTimeRange);

				_light.intensity = Mathf.Max(_light.intensity + offset - Random.Range(0f, 1 * maxIntensityDecreaseMultiplier),
					0);
			}

			_light.intensity = Mathf.Lerp(_light.intensity + offset, 1, turnOnSpeed);
		}
	}
}