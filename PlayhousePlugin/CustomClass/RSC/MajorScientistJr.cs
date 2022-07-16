using Exiled.API.Features;
using MEC;

namespace PlayhousePlugin.CustomClass
{
    public class MajorScientistJr : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "Major Scientist Jr.";
        public override int AbilitiesNum { get; } = 0;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }
        
        public override void Replace(Player ply)
        {
            throw new System.NotImplementedException();
        }

        public MajorScientistJr(Player ply)
        {
            Ply = ply;
            PassiveAbilities = new CoroutineHandle[] { };
            ActiveAbilities = new AbilityBase[] { };
            
            PlayhousePlugin.PlayhousePluginRef.Handler.CheckandGive05(ply);
            ply.ClearInventory();
            ply.AddItem(ItemType.KeycardResearchCoordinator);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.SCP207);
            ply.AddItem(ItemType.SCP500);
            ply.AddItem(ItemType.Flashlight);

            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, $"{SillySundayNames.MajorScientistSundayNames.PickRandom()}\n(Custom Class)", -1, "mint");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, "Major Scientist Jr.\n(Custom Class)", -1, "mint");
            }

            ply.Broadcast(10, "<size=60><b><i>You have spawned as a <color=yellow>Major Scientist Jr.</color></i></b></size>");
            ply.SendConsoleMessage("Name: Major Scientist Jr.\n\nDescription: Help your fellow scientists get access to Heavy Containment faster with your higher access card!\n\nPassive Buffs: Better Keycard, SCP-207 and SCP-500 on spawn\nPassive Debuffs: None\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
    }
}