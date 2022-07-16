using Exiled.API.Features;

namespace PlayhousePlugin.CustomClass
{
	public static class Ntf
	{
		public static void MakeNtfHeavy(Player player)
		{
			player.CustomClassManager().DisposeCustomClass();
			player.CustomClassManager().CustomClass = new NtfHeavy(player);
		}

		public static void MakeNtfEngineer(Player player)
		{
			player.CustomClassManager().DisposeCustomClass();
			player.CustomClassManager().CustomClass = new NtfEngineer(player);
		}

		public static void MakeNtfMedic(Player player)
		{
			player.CustomClassManager().DisposeCustomClass();
			player.CustomClassManager().CustomClass = new NtfMedic(player);
		}

		public static void MakeNtfScout(Player player)
		{
			player.CustomClassManager().DisposeCustomClass();
			player.CustomClassManager().CustomClass = new NtfScout(player);
		}

		public static void MakeNtfDemo(Player player)
		{
			player.CustomClassManager().DisposeCustomClass();
			player.CustomClassManager().CustomClass = new NtfDemoman(player);
		}

		public static void MakeNtfContainmentSpecialist(Player player)
		{
			player.CustomClassManager().DisposeCustomClass();
			player.CustomClassManager().CustomClass = new NtfContainmentSpecialist(player);
		}
	}
}
