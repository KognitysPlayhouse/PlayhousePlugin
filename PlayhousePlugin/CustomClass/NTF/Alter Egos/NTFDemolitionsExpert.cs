using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace PlayhousePlugin.CustomClass
{
    public class NTFDemolitionsExpert : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "NTF Demolitions Expert";
        public override int AbilitiesNum { get; } = 0;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }

        public override void Escape()
        {
            Ply.Role.Type = RoleType.ChaosRifleman;
            Ply.CustomClassManager().DisposeCustomClass();
            Ply.CustomClassManager().CustomClass = new ChaosDemolitionsExpert(Ply);
        }

        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
            Vector3 pos = ply.Position;
            Timing.CallDelayed(0.1f, () =>
            {
                ply.Position = pos;
                ply.CustomClassManager().CustomClass = new NTFDemolitionsExpert(ply);
            });
        }
        
        public NTFDemolitionsExpert(Player ply)
        {
            Ply = ply;
            ActiveAbilities = new AbilityBase[] { };
            PassiveAbilities = new[]
            {
                Timing.RunCoroutine(RegenExplosive())
            };

            ply.ClearInventory();
            ply.AddItem(ItemType.KeycardNTFLieutenant);
            ply.AddItem(ItemType.GunCrossvec);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.Radio);
            ply.AddItem(ItemType.ArmorCombat);
            ply.AddItem(ItemType.GrenadeHE);
            ply.AddItem(ItemType.GrenadeHE);
            ply.AddItem(ItemType.GrenadeHE);
           
            ply.Ammo[ItemType.Ammo9x19] = 120;
            
            Timing.CallDelayed(1.8f, () =>
            {
                ply.Health = 150;
                ply.MaxHealth = 150;
            });

            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, $"{SillySundayNames.ChaosDemolitionsExpertSundayNames.PickRandom()}\n(Custom Class)", -1, "mint");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, "Demolitions Expert\n(Custom Class)", -1, "mint");
            }
            
            ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=navy>Demolitions Expert</color>\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            ply.SendConsoleMessage("Name: NTF Demolitions Expert\n\nDescription: You still have a bit of Chaos inside of you, use that to your advantage and blow shit up.\n\nPassive Buffs: 150 Max HP, Explosive Item Regen every minute\nPassive Debuffs: None\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
        
        private IEnumerator<float> RegenExplosive()
        {
            var specialItems = new List<ItemType> { ItemType.GrenadeFlash, ItemType.SCP018 , ItemType.SCP2176};

            while (true)
            {
                yield return Timing.WaitForSeconds(60f);

                if (Ply.CustomClassManager().CustomClass.Name != "NTF Demolitions Expert")
                {
                    break;
                }

                if (Ply.IsCuffed) continue;
                if (Ply.Inventory.UserInventory.Items.Count != 8)
                {
                    var chance = EventHandler.random.Next(0, 100);

                    if (chance <= 20)
                    {
                        Ply.AddItem(specialItems[EventHandler.random.Next(specialItems.Count)]);
                        Ply.ShowCenterDownHint($"<color=yellow>Special Item Generated!</color>", 3);
                    }
                    else
                    {
                        Ply.AddItem(ItemType.GrenadeHE);
                        Ply.ShowCenterDownHint($"<color=yellow>Explosive Item Generated!</color>", 3);
                    }
                }
                else
                {
                    Ply.ShowCenterDownHint($"<color=yellow>You would have regenerated an item here, but your inventory is full.</color>", 4);
                }
            }
        }
    }
}