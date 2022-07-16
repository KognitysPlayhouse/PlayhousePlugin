using Exiled.API.Features;

namespace PlayhousePlugin.CustomClass.SCP
{
	public class SCP0492
	{
		public static void MedicalStudentZombie(Player Ply)
		{
			Ply.CustomClassManager().DisposeCustomClass();
			Ply.CustomClassManager().CustomClass = new MedicalStudent(Ply);
		}

		public static void SpeedyZombie(Player Ply)
		{
			Ply.CustomClassManager().DisposeCustomClass();
			Ply.CustomClassManager().CustomClass = new Sprinter(Ply);
		}

		public static void BoomerZombie(Player Ply)
		{
			Ply.CustomClassManager().DisposeCustomClass();
			Ply.CustomClassManager().CustomClass = new Boomer(Ply);
		}

		public static void Overdoser(Player Ply)
		{
			Ply.CustomClassManager().DisposeCustomClass();
			Ply.CustomClassManager().CustomClass = new Overdoser(Ply);
		}

		public static void Overclocker(Player Ply)
		{
			Ply.CustomClassManager().DisposeCustomClass();
			Ply.CustomClassManager().CustomClass = new Overclocker(Ply);
		}
	}
}
