using Exiled.API.Features;

namespace PlayhousePlugin.CustomClass
{
	public class CI
	{
		public static void MakeChaosDemo(Player player)
		{
			player.CustomClassManager().DisposeCustomClass();
			player.CustomClassManager().CustomClass = new ChaosDemolitionsExpert(player);
		}

		public static void MakeChaosBulldozer(Player player)
		{
			player.CustomClassManager().DisposeCustomClass();
			player.CustomClassManager().CustomClass = new ChaosBulldozer(player);
		}

		public static void MakeChaosHunter(Player player)
		{
			player.CustomClassManager().DisposeCustomClass();
			player.CustomClassManager().CustomClass = new ChaosHunter(player);
		}

		public static void MakeChaosPoisonCarrier(Player player)
		{
			player.CustomClassManager().DisposeCustomClass();
			player.CustomClassManager().CustomClass = new ChaosExterminator(player);
		}
		
		public static void MakeChaosMachinist(Player player)
		{
			player.CustomClassManager().DisposeCustomClass();
			player.CustomClassManager().CustomClass = new ChaosMachinist(player);
		}
		
		public static void MakeChaosHeretic(Player player)
		{
			player.CustomClassManager().DisposeCustomClass();
			player.CustomClassManager().CustomClass = new ChaosHeretic(player);
		}
	}
}
