using Exiled.API.Features;

namespace PlayhousePlugin.CustomClass
{
	public class RSC
	{
		public static void MakeMajorScientist(Player player)
		{
			player.CustomClassManager().DisposeCustomClass();
			player.CustomClassManager().CustomClass = new MajorScientistJr(player);
		}
	}
}
