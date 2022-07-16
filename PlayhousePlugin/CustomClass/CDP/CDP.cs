using Exiled.API.Features;

namespace PlayhousePlugin.CustomClass
{
	public class CDP
	{
		public static void MakeClassDChad(Player player)
		{
			player.CustomClassManager().DisposeCustomClass();
			player.CustomClassManager().CustomClass = new ClassDChad(player);
		}

		public static void MakeClassDJanitor(Player player)
		{
			player.CustomClassManager().DisposeCustomClass();
			player.CustomClassManager().CustomClass = new ClassDJanitor(player);
		}
	}
}
