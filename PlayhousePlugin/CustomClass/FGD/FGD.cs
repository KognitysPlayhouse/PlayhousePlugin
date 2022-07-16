using Exiled.API.Features;

namespace PlayhousePlugin.CustomClass
{
	public class FGD
	{
		public static void MakeGuardManager(Player player)
		{
			player.CustomClassManager().DisposeCustomClass();
			player.CustomClassManager().CustomClass = new GuardManager(player);
		}

		public static void MakeSeniorGuard(Player player)
		{
			player.CustomClassManager().DisposeCustomClass();
			player.CustomClassManager().CustomClass = new SeniorGuard(player);
		}
	}
}
