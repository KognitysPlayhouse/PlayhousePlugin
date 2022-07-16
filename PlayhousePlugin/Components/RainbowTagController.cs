using System.Collections.Generic;
using UnityEngine;

namespace PlayhousePlugin
{
	public class RainbowTagController : MonoBehaviour
	{
		private ServerRoles Roles;
		private string OriginalColor;

		private int Position = 0;
		private float NextCycle = 0f;

		public static List<string> Colors = new List<string>
		{
			"pink",
			"red",
			"brown",
			"silver",
			"light_green",
			"crimson",
			"cyan",
			"aqua",
			"deep_pink",
			"tomato",
			"yellow",
			"magenta",
			"blue_green",
			"orange",
			"lime",
			"green",
			"emerald",
			"carmine",
			"nickel",
			"mint",
			"army_green",
			"pumpkin"
		};

		public static float Interval { get; set; } = PlayhousePlugin.PlayhousePluginRef.Config.TagInterval;

		public void Awake()
		{
			Roles = GetComponent<ServerRoles>();
			NextCycle = Time.time;
			OriginalColor = Roles.Network_myColor;
		}

		public void OnDestroy()
		{
			Roles.Network_myColor = OriginalColor;
		}

		public void Update()
		{
			if (Time.time >= NextCycle)
			{
				NextCycle += Interval;
				Roles.Network_myColor = Colors[Position];

				if (++Position >= Colors.Count)
					Position = 0;
			}
		}
	}
}
