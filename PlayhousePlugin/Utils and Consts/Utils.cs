using System;
using System.Collections.Generic;
using AdminToys;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Mirror;
using UnityEngine;

namespace PlayhousePlugin
{
	public class Pair
	{
		public int First { get; set; }
		public int Second { get; set; }

		public Pair(int first, int second)
		{
			First = first;
			Second = second;
		}
	}

	public class Tip
	{
		public int Time { get; }
		public string Message { get; }
		
		public Tip(string message, int time)
		{
			Time = time;
			Message = message + "\n";
		}
	}

	public class Utils
	{
		public static List<string> BlacklistedURLs = new List<string>
		{
			"hellcase.org",
			"hellcase.com",
			"velk.ca",
			"pvpro.com",
			"banditcamp.com",
			"bandit.camp",
			"rustchance.com",
			"flx.gg",
		};

		public static List<string> WhitelistedIDs = new List<string>
		{
			""
		};

		public enum Ability
		{
			None = -1,
			Ability1 = 0,
			Ability2 = 1,
			Ability3 = 2,
			Ability4 = 3,
		}

		public enum ObjectiveStates
		{
			Disabled,
			Activating,
			Contested,
			Decaying,
			Enabled
		}

		public static List<RoleType> SCPRoles = new List<RoleType>()
		{
			RoleType.Scp049,
			RoleType.Scp096,
			RoleType.Scp106,
			RoleType.Scp173,
			RoleType.Scp93953,
			RoleType.Scp93989
		};
		
		public class PosRot
		{
			public Vector3 Pos;
			public Vector3 Rot;

			public PosRot(Vector3 pos, Vector3 rot)
			{
				Pos = pos;
				Rot = rot;
			}
		}

		public static Dictionary<RoomType, List<PosRot>> ObjectivePointLocations = new Dictionary<RoomType, List<PosRot>>()
		{
			{RoomType.HczEzCheckpoint, new List<PosRot>{new PosRot(new Vector3(-5.9f, 2.1f, 9.6f), new Vector3(0,180,0)),
				new PosRot(new Vector3(6.5f, 1.8f, 6.1f), new Vector3(0,270,0))}}, // Heavy's side
			{RoomType.EzConference, new List<PosRot>{new PosRot(new Vector3(-4.2f, 1.9f, -1.8f), new Vector3(0,90,0))}},
			{RoomType.EzIntercom, new List<PosRot>{new PosRot(new Vector3(-0.1f, 2, 2.3f), new Vector3(0,180,0))}},
			{RoomType.HczHid, new List<PosRot>{new PosRot(new Vector3(8.0f, 2.1f, -8.7f), new Vector3(0, 0, 0))}},
			{RoomType.Hcz106, new List<PosRot>{new PosRot(new Vector3(20, 2.4f, 4.7f), new Vector3(0,180,0))}},
			{RoomType.HczTCross, new List<PosRot>{new PosRot(new Vector3(3.0f, 2.2f, -4.9f), new Vector3(0,270,0))}},
			{RoomType.Hcz049, new List<PosRot>{new PosRot(new Vector3(-8.8f, 2.2f, 2.9f), new Vector3(0,180,0))}},
			{RoomType.HczChkpB, new List<PosRot>{new PosRot(new Vector3(3.5f, 1.6f, -0.3f), new Vector3(0,270,0))}},
			{RoomType.HczChkpA, new List<PosRot>{new PosRot(new Vector3(3.5f, 1.6f, -0.3f), new Vector3(0,270,0))}},
			{RoomType.HczArmory, new List<PosRot>{new PosRot(new Vector3(-0.5f, 2.2f, 2.2f), new Vector3(0, 270, 0))}},
			{RoomType.Hcz079, new List<PosRot>{new PosRot(new Vector3(9.5f, -2.6f, 2.2f), new Vector3(0,180,0))}},
			{RoomType.HczServers, new List<PosRot>{new PosRot(new Vector3(-5.6f, 1.8f, 4.2f), new Vector3(0,270,0)),
				new PosRot(new Vector3(-2.1f, -6.1f, 5.9f), new Vector3(0,180,0))}}, // Top
			{RoomType.EzUpstairsPcs, new List<PosRot>{new PosRot(new Vector3(10.4f, 5.6f, 4.0f), new Vector3(0,270,0)),
				new PosRot(new Vector3(5.9f, 1.8f, 5.7f), new Vector3(0,270,0))}},
			{RoomType.EzCrossing, new List<PosRot>{new PosRot(new Vector3(2.9f, 1.9f, -2.9f), new Vector3(0,315,0))}},
			{RoomType.EzCurve, new List<PosRot>{new PosRot(new Vector3(3.5f, 1.7f, 3.6f), new Vector3(0,225,0))}},
			{RoomType.HczCurve, new List<PosRot>{new PosRot(new Vector3(-3.5f, 1.7f, -0.1f), new Vector3(0,90,0))}},
			{RoomType.HczStraight, new List<PosRot>{new PosRot(new Vector3(-2.2f, 1.9f, 3.0f), new Vector3(0,180,0))}},
			//{"", new PosRot(new Vector3(), new Vector3())},
		};

		public static Dictionary<RoomType, List<Vector3>> EZRecyclingBinLocations = new Dictionary<RoomType, List<Vector3>>()
		{
			{ RoomType.EzUpstairsPcs, new List<Vector3>{new Vector3(-6.8f, 0, 9.4f) }},
			{ RoomType.EzCurve, new List<Vector3>{new Vector3(-3.2f, 0, 5), new Vector3(4.8f, 0, -3.0f)}},
			{ RoomType.EzConference, new List<Vector3>{new Vector3(-3.6f, 0, 2.0f)} },
		};

		public static Dictionary<RoomType, List<Vector3>> GateRecyclingBinLocations =
			new Dictionary<RoomType, List<Vector3>>()
			{
				{RoomType.EzGateB, new List<Vector3> {new Vector3(5.1f, 0, 1.2f), new Vector3(5.1f, 0, -9.9f)}},
				{RoomType.EzGateA, new List<Vector3> {new Vector3(5.7f, 0, -9.8f)}},
			};
		
		public static Dictionary<RoomType, List<Vector3>> HCZRecyclingBinLocations = new Dictionary<RoomType, List<Vector3>>()
		{
			{ RoomType.HczCurve, new List<Vector3>{new Vector3(-2.9f, 0, 3.0f) }},
			{ RoomType.HczCrossing, new List<Vector3>{new Vector3(-2.7f, 0, 2.7f), new Vector3(2.7f, 0, 2.7f), new Vector3(2.7f, 0, -2.7f), new Vector3(-2.7f, 0, -2.7f)}},
		};
		
		public static Dictionary<RoomType, List<Vector3>> LCZRecyclingBinLocations = new Dictionary<RoomType, List<Vector3>>()
		{
			{ RoomType.Lcz914, new List<Vector3>{new Vector3(-2.2f, 0, -9.5f), new Vector3(-9.3f, 0, -3.6f) }},
			{ RoomType.LczAirlock, new List<Vector3>{new Vector3(0f, 0, 1.6f)}},
			{ RoomType.LczCafe, new List<Vector3>{new Vector3(-6.7f, 0.0f, 6.1f)}},
			{ RoomType.Lcz330, new List<Vector3>{new Vector3(-2.9f, 0.0f, 0.0f)}},
		};

		public static Dictionary<RoomType, List<PosRot>> LCZVendingLocations = new Dictionary<RoomType, List<PosRot>>
		{
			{RoomType.LczCafe, new List<PosRot>{new PosRot(new Vector3(-7.0f, 0.0f, -6.7f), new Vector3(0, 0, 0))}},
			{RoomType.LczToilets, new List<PosRot>{new PosRot(new Vector3(5.7f, 0.0f, -1.6f), new Vector3(0, 0, 0))}},
			{RoomType.Lcz330, new List<PosRot>{new PosRot(new Vector3(-7.8f, 0.0f, 4.3f), new Vector3(0, 180, 0))}},
		};
		
		public static Dictionary<RoomType, List<PosRot>> EZVendingLocations = new Dictionary<RoomType, List<PosRot>>
		{
			{RoomType.EzPcs, new List<PosRot>{new PosRot(new Vector3(3.7f, 0.0f, 9.6f), new Vector3(0, 180, 0))}},
			{RoomType.EzUpstairsPcs, new List<PosRot>{new PosRot(new Vector3(10.1f, 3.9f, 1.3f), new Vector3(0, 270, 0)),
				new PosRot(new Vector3(-3.5f, 0, 9.9f), new Vector3(0, 180, 0))}},
			{RoomType.EzStraight, new List<PosRot>{new PosRot(new Vector3(-1.8f, 0.0f, 0.8f), new Vector3(0, 90, 0))}},
		};

		public static Dictionary<int, string> ServerPort = new Dictionary<int, string>
		{
			{7777, "#1"},
			{8888, "#2"},
			{8889, "#3"},
			{7778, "Dev Server"},
			{8899, "Breakout Blitz"}
		};

		public static List<Tip> Tips = new List<Tip>
		{
			new Tip("Tip: Do you have your ability binds set up?\n" +
			        "If not you can do so by:\nPressing Escape -> Server Info -> Then watching the YouTube video on how to do it.\n<color=orange>If you do it I'll give you a cookie :]</color>\n\n\n", 10),
			new Tip("Tip: MTF are strong together, Chaos work alone.", 5),
			new Tip("Tip: You can detain and escort MTF or Chaos to convert them to your own faction.", 5),
			new Tip("Tip: Healing classes keep your team alive, so don't let them die.", 5),
			new Tip("Tip: SCP-207 is best used on Scouts or Heavies.", 5),
			new Tip("Tip: Burn your targets as NTF Containment Specialist by using your Revolver.", 5),
			new Tip("Tip: Are you hydrated?", 5),
			new Tip("Tip: If you are MTF or Guards capture the decontamination terminals, they will be helpful later in the round.", 7),
			new Tip("Tip: This is a heavily modded server, if you have any questions join the discord or ask in the chat!", 7),
			new Tip("Tip: Radios have infinite battery, set it to Ultra range for most utility.", 5),
			new Tip("Tip: SCP-079 grows in power the higher his tier, shut him down before he tips the scales.", 7),
			new Tip("Tip: Consider applying for staff! We are always hiring, join the discord for more info!", 7),
			new Tip("Tip: You can get vanity server badges for certain milestones, join the discord for more info!", 7),
			new Tip("Tip: SCP-096 will not add you as a target if you are invisible, but will still enter rage.", 7),
			new Tip("Tip: If the lights flicker there is an SCP-096 in the round", 8),
			new Tip("Tip: 4 Zombies can break open any door.", 5),
			new Tip("Tip: Adrenaline gives you a speed boost for a short period of time.", 5),
			new Tip("Tip: There are Sergeant Cards scattered throughout heavy. Find them in 939 stairs, 049's Room, Nuke Armory and 106's 2nd Door", 7),
			new Tip("Tip: See a rule breaker? Clip it and send information to the #player-reporting channel on the discord!", 7),
			new Tip("Tip: Drinking water can keep you hydrated.", 5),
			new Tip("Tip: Medkits and SCP-500 can prevent the spread of zombie infections.", 6),
			new Tip("Tip: If SCP-939 is in your current zone, she hears you if you interact with certain doors or elevators.", 8),
			new Tip("Tip: You can kill or vaporize yourself! Use: \".kill\" or \".vaporize\"", 6),
			new Tip("Tip: Consider donating, your donations are only used to pay the server bills and nothing else.", 7),
			new Tip("Tip: As an NTF Engineer your Dispenser and Speedpad are best placed in areas of high foot traffic.", 6),
			new Tip("Tip: Tip: Tip: Tip: Tip: Tip: Did you get all that?", 5),
			new Tip("Tip: Sundays is when we throw out balance out the window. Funi zombi explod :D", 6),
			new Tip("Tip: Always try to detain Class D but be ready if they have any surprises.", 6),
			new Tip("Tip: Do you have any plugin/balance/custom class or other suggestions? Join the discord and suggest them!", 7),
			new Tip("Tip: Fortnite pretty cringe innit bruv?", 5),
			new Tip("Tip: If you experience frame drops go to your main menu and change your blood and ragdoll settings!", 6),
			new Tip("Tip: Recycle facility trash for your financial gain.", 5),
			new Tip("Tip: Cheese.", 5),
			new Tip("Tip: Are you familiar with our rules? If not you can do so by Pressing Escape -> Server Info", 5),
			//new Tip("Tip: ", 5),
		};

		public static Dictionary<ItemType, ArmourAmmo> ArmourAmmoLimits = new Dictionary<ItemType, ArmourAmmo>
		{
			{ItemType.None, new ArmourAmmo(ItemType.None, 30, 40, 40, 18, 14)},
			{ItemType.ArmorLight, new ArmourAmmo(ItemType.ArmorLight, 60, 40, 40, 18, 14)},
			{ItemType.ArmorCombat, new ArmourAmmo(ItemType.ArmorCombat, 160, 120, 120, 48, 54)},
			{ItemType.ArmorHeavy, new ArmourAmmo(ItemType.ArmorHeavy, 200, 200, 200, 68, 74)},
		};

		private static PrimitiveObjectToy primitiveBaseObject = null;
		public static PrimitiveObjectToy PrimitiveBaseObject
		{
			get
			{
				if (primitiveBaseObject == null)
				{
					foreach (var gameObject in NetworkClient.prefabs.Values)
					{
						if (gameObject.TryGetComponent<PrimitiveObjectToy>(out var component))
							primitiveBaseObject = component;
					}
				}

				return primitiveBaseObject;
			}
		}
		
		private static LightSourceToy lightSourceBaseObject = null;

		public static LightSourceToy LightSourceBaseObject
		{
			get
			{
				if (lightSourceBaseObject == null)
				{
					foreach (var gameObject in NetworkClient.prefabs.Values)
					{
						if (gameObject.TryGetComponent<LightSourceToy>(out var component))
							lightSourceBaseObject = component;
					}
				}

				return lightSourceBaseObject;
			}
		}
	}

	public class ArmourAmmo
	{
		public ItemType Armour { get; }
		public int Ammo9 { get; }
		public int Ammo556 { get; }
		public int Ammo762 { get; }
		public int Ammo44 { get; }
		public int Ammo12Gauge { get; }
		public Dictionary<ItemType, int> LimitDictionary { get; }

		public ArmourAmmo(ItemType armour, int ammo9, int ammo556, int ammo762, int ammo44, int ammo12Gauge)
		{
			Armour = armour;
			Ammo9 = ammo9;
			Ammo556 = ammo556;
			Ammo762 = ammo762;
			Ammo44 = ammo44;
			Ammo12Gauge = ammo12Gauge;
			LimitDictionary = new Dictionary<ItemType, int>
			{
				{ItemType.Ammo9x19, ammo9},
				{ItemType.Ammo556x45, ammo556},
				{ItemType.Ammo762x39, ammo762},
				{ItemType.Ammo44cal, ammo44},
				{ItemType.Ammo12gauge, ammo12Gauge},
			};
		}
	}
	
	public class Jailed
	{
		public string Userid;
		public string Name;
		public List<Item> Items;
		public RoleType Role;
		public Vector3 Position;
		public float Health;
		public Dictionary<AmmoType, ushort> Ammo;
		public bool CurrentRound;
		public byte SCP207Intensity;
	}
	
	public class Donator
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public int DonatorNum { get; set; }
		public bool IsBooster { get; set; }
		public string Preference { get; set; }

		public static List<Donator> Donators = new List<Donator>();

		public static Donator GetDonator(string RawUserId, out Donator donator)
		{
			donator = null;

			foreach (Donator dn in Donators)
			{
				if (dn.UserId == RawUserId)
				{
					donator = dn;
					break;
				}
			}

			return donator;
		}

		public static Donator GetDonator(Player ply, out Donator donator)
		{
			donator = null;

			foreach(Donator dn in Donators)
			{
				if(dn.UserId == ply.RawUserId)
				{
					donator = dn;
					break;
				}
			}

			return donator;
		}
	}
}
