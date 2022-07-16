namespace PlayhousePlugin
{
    public class SillySundayInfectionController
    {
        public static bool InfectionEnabled = false;
        public static RoleType InfectedRole = RoleType.None;
        
        public static void ResetToDefaults()
        {
            InfectionEnabled = false;
            InfectedRole = RoleType.None;
        }
    }
}