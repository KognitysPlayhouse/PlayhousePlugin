using Exiled.API.Features;
using Exiled.API.Features.Items;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PlayhousePlugin
{
	public class Sprays
	{
		public struct Point
		{
			public int X { get; set; }
			public int Y { get; set; }

			public Point(int x, int y)
			{
				X = x;
				Y = y;
			}
		}

		public static List<Point> ArrowDown()
		{
			List<Point> points = new List<Point>();
			using (var reader = new StreamReader("/home/ubuntu/.config/EXILED/Configs/Sprays/arrowDown.csv"))
			{
				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					var values = line.Split(',');

					points.Add(new Point(int.Parse(values[0]), int.Parse(values[1])));
				}
			}
			return points;
		}
		
		public static List<Point> ArrowUp()
		{
			List<Point> points = new List<Point>();
			using (var reader = new StreamReader("/home/ubuntu/.config/EXILED/Configs/Sprays/arrowUp.csv"))
			{
				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					var values = line.Split(',');

					points.Add(new Point(int.Parse(values[0]), int.Parse(values[1])));
				}
			}
			return points;
		}
		
		public static List<Point> ArrowRight()
		{
			List<Point> points = new List<Point>();
			using (var reader = new StreamReader("/home/ubuntu/.config/EXILED/Configs/Sprays/arrowRight.csv"))
			{
				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					var values = line.Split(',');

					points.Add(new Point(int.Parse(values[0]), int.Parse(values[1])));
				}
			}
			return points;
		}
		
		public static List<Point> ArrowLeft()
		{
			List<Point> points = new List<Point>();
			using (var reader = new StreamReader("/home/ubuntu/.config/EXILED/Configs/Sprays/arrowLeft.csv"))
			{
				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					var values = line.Split(',');

					points.Add(new Point(int.Parse(values[0]), int.Parse(values[1])));
				}
			}
			return points;
		}
		
		public static List<Point> TrollFace()
		{
			List<Point> points = new List<Point>();
			using (var reader = new StreamReader("/home/ubuntu/.config/EXILED/Configs/Sprays/trollface.csv"))
			{
				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					var values = line.Split(',');

					points.Add(new Point(int.Parse(values[0]), int.Parse(values[1])));
				}
			}
			return points;
		}
		
		public static List<Point> KLPPog()
		{
			List<Point> points = new List<Point>();
			using (var reader = new StreamReader("/home/ubuntu/.config/EXILED/Configs/Sprays/klpPog.csv"))
			{
				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					var values = line.Split(',');

					points.Add(new Point(int.Parse(values[0]), int.Parse(values[1])));
				}
			}
			return points;
		}
		
		public static List<Point> KLPLauv()
		{
			List<Point> points = new List<Point>();
			using (var reader = new StreamReader("/home/ubuntu/.config/EXILED/Configs/Sprays/klpLauv.csv"))
			{
				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					var values = line.Split(',');

					points.Add(new Point(int.Parse(values[0]), int.Parse(values[1])));
				}
			}
			return points;
		}
		
		public static List<Point> KLPLauv2()
		{
			List<Point> points = new List<Point>();
			using (var reader = new StreamReader("/home/ubuntu/.config/EXILED/Configs/Sprays/klpLauvHS.csv"))
			{
				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					var values = line.Split(',');

					points.Add(new Point(int.Parse(values[0]), int.Parse(values[1])));
				}
			}
			return points;
		}
		
		public static List<Point> Amogus()
		{
			List<Point> points = new List<Point>();
			using (var reader = new StreamReader("/home/ubuntu/.config/EXILED/Configs/Sprays/amogus.csv"))
			{
				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					var values = line.Split(',');

					points.Add(new Point(int.Parse(values[0]), int.Parse(values[1])));
				}
			}
			return points;
		}

		public static List<Point> Hubert()
		{
			List<Point> points = new List<Point>();
			using (var reader = new StreamReader("/home/ubuntu/.config/EXILED/Configs/Sprays/hubertSmall.csv"))
			{
				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					var values = line.Split(',');

					points.Add(new Point(int.Parse(values[0]), int.Parse(values[1])));
				}
			}
			return points;
		}

		public static List<Point> FoundationLogo()
		{
			List<Point> points = new List<Point>();
			using (var reader = new StreamReader("/home/ubuntu/.config/EXILED/Configs/Sprays/foundationLogo.csv"))
			{
				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					var values = line.Split(',');

					points.Add(new Point(int.Parse(values[0]), int.Parse(values[1])));
				}
			}
			return points;
		}

		public static void SprayPattern(Player ply, List<Point> points)
		{
			var firearm = (Firearm)Firearm.Create(ItemType.GunCOM18);
			var afirearm = firearm.Base as InventorySystem.Items.Firearms.AutomaticFirearm;
			var w = new InventorySystem.Items.Firearms.Modules.SingleBulletHitreg(firearm.Base, ply.ReferenceHub);

			var forward1 = ply.CameraTransform.forward;
			var rOriginal = new Ray(ply.ReferenceHub.PlayerCameraReference.position + forward1,
					ply.ReferenceHub.PlayerCameraReference.forward);
			Physics.Raycast(rOriginal, out RaycastHit originalHit, 100);

			/*
			(Item)Item.Create(ItemType.KeycardContainmentEngineer).Spawn(originalHit.point + originalHit.normal);
			(Item)Item.Create(ItemType.KeycardGuard).Spawn(originalHit.point + Vector3.Cross(ply.CameraTransform.transform.right, originalHit.normal));
			(Item)Item.Create(ItemType.KeycardScientist).Spawn(originalHit.point + ply.CameraTransform.right);

			for (double x = 0; x < Math.PI * 2; x += Math.PI / 12)
			{
				var r = new Ray(originalHit.point + originalHit.normal +
					(Vector3.Cross(ply.CameraTransform.transform.right, originalHit.normal) * Convert.ToSingle(Math.Sin(x))) +
					(ply.CameraTransform.right * Convert.ToSingle(Math.Cos(x)))
					, -originalHit.normal);


				Physics.Raycast(r, out RaycastHit hitinfo, 100);
				w.PlaceBullethole(r, hitinfo);
			}*/

			var up = -Vector3.Cross(ply.CameraTransform.transform.right, originalHit.normal) * 0.05f;
			var right = ply.CameraTransform.right * 0.05f;

			foreach (var point in points)
			{
				var r = new Ray(originalHit.point + originalHit.normal +
					(right * point.X) +
					(up * point.Y)
					, -originalHit.normal);


				Physics.Raycast(r, out RaycastHit hitinfo, 100);
				w.PlaceBulletholeDecal(r, hitinfo);
			}
		}
	}
}
