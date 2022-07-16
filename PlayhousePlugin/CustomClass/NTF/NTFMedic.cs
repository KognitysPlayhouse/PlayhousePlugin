using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace PlayhousePlugin.CustomClass
{
    public class NtfMedic : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "NTF Medic";
        public override int AbilitiesNum { get; } = 1;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }

        public override void Escape()
        {
            Ply.Role.Type = RoleType.ChaosRifleman;
            Ply.CustomClassManager().DisposeCustomClass();
            Ply.CustomClassManager().CustomClass = new ChaosMedic(Ply);
        }

        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
            Vector3 pos = ply.Position;
            Timing.CallDelayed(0.1f, () =>
            {
                ply.Position = pos;
                ply.CustomClassManager().CustomClass = new NtfMedic(ply);
            });
        }

        public NtfMedic(Player ply)
        {
            Ply = ply;
            PassiveAbilities = new[]
            {
                Timing.RunCoroutine(EngageHealing()),
                Timing.RunCoroutine(RegenMedkit())
            };
            ActiveAbilities = new AbilityBase[]{ new Abilities.NtfMedicRevive(ply) };
            ply.CustomClassManager().AbilityIndex = 0;
            
            ply.ClearInventory();
            ply.AddItem(ItemType.KeycardNTFLieutenant);
            ply.AddItem(ItemType.GunFSP9);
            ply.AddItem(ItemType.Radio);
            ply.AddItem(ItemType.ArmorCombat);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.Medkit);
            
            ply.Ammo[ItemType.Ammo9x19] = 120;

            Timing.CallDelayed(0.5f, () =>
            {
                ply.MaxHealth = 105;
                ply.Health = 105;
            });

            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, $"{SillySundayNames.NTFMedicSundayNames.PickRandom()}\n(Custom Class)", -1, "mint");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, "Medic\n(Custom Class)", -1, "mint");
            }

            ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=navy>Medic</color>\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            ply.SendConsoleMessage("Name: NTF Medic\n\nDescription: Generate Medkits and heal people with your passive healing aura to victory!\n\nAbility1: Revive (30s Cooldown)\n\nPassive Buffs: Healing aura, Medical Item Generation\nPassive Debuffs: None\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }

        private IEnumerator<float> RegenMedkit()
        {
            var specialItems = new List<ItemType> { ItemType.Adrenaline, ItemType.SCP500, ItemType.SCP207 };

            while (true)
            {
                yield return Timing.WaitForSeconds(40f);

                if (Ply.CustomClassManager().CustomClass.Name != "NTF Medic")
                    break;

                if (Ply.IsCuffed || Ply.CurrentRoom.Type == RoomType.Pocket) continue;
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
                        Ply.AddItem(ItemType.Medkit);
                        Ply.ShowCenterDownHint($"<color=yellow>Medical Item Generated!</color>", 3);
                    }
                }
                else
                {
                    Ply.ShowCenterDownHint(
                        $"<color=yellow>You would have regenerated an item here, but your inventory is full.</color>",
                        4);
                }
            }
        }

        private IEnumerator<float> EngageHealing()
        {
            yield return Timing.WaitForSeconds(2f);
            List<Player> PlayersAlreadyAffected = new List<Player>();
            while (true)
            {
                if (Ply.CustomClassManager().CustomClass.Name != "NTF Medic")
                    break;

                if (!Ply.IsCuffed)
                {
                    foreach (var ply in Player.List.Where(x=> Vector3.Distance(x.Position, Ply.Position) <= 7))
                    {
                        if (ply == Ply) continue;
                        if (ply.Role.Team == Team.MTF || ply.Role.Team == Team.RSC  || ply.IsCuffed)
                        {
                            if (!PlayersAlreadyAffected.Contains(ply))
                            {
                                UtilityMethods.ApplyMedicHeal(ply, 5, true, Ply);
                                UtilityMethods.ApplyMedicHeal(Ply, 1, false, Ply);
                                PlayersAlreadyAffected.Add(ply);
                            }
                        }
                    }
                }
                PlayersAlreadyAffected.Clear();
                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}