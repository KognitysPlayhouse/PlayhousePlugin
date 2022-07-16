using System.Collections.Generic;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MEC;

namespace PlayhousePlugin.CustomGameMode
{
    public class BreakoutBlitz
    {
        public static int SCPKills = 0;
        public static int ClassDEscapes = 0;
        public static int ScientistEscapes = 0;
        
        public static int RequiredSCPKills = 200;
        public static int RequiredClassDEscapes = 5;
        public static int RequiredScientistEscapes = 5;
        public static List<Pickup> PickupsToNotClear = new List<Pickup>();

        public static IEnumerator<float> ItemAndRagdollClear()
        {
            while (true)
            {
                Log.Info("AeeA");
                yield return Timing.WaitForSeconds(60f);
                foreach(Pickup pickup in Map.Pickups)
                {
                    if(!PickupsToNotClear.Contains(pickup))
                    {
                        pickup.Destroy();
                    }
                }

                foreach (var ragdoll in Map.Ragdolls)
                {
                    ragdoll.Delete();
                }
            }
        }
    }
}