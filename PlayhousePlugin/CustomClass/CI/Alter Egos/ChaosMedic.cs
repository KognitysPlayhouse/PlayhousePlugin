using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace PlayhousePlugin.CustomClass
{
    public class ChaosMedic : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "Chaos Medic";
        public override int AbilitiesNum { get; } = 1;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }
        
        public override void Escape()
        {
            Ply.Role.Type = RoleType.NtfPrivate;
            Ply.CustomClassManager().DisposeCustomClass();
            Ply.CustomClassManager().CustomClass = new NtfMedic(Ply);
        }

        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
            Vector3 pos = ply.Position;
            Timing.CallDelayed(0.1f, () =>
            {
                ply.Position = pos;
                ply.CustomClassManager().CustomClass = new ChaosMedic(ply);
            });
        }

        public ChaosMedic(Player ply)
        {
            Ply = ply;
            PassiveAbilities = new[]
            {
                Timing.RunCoroutine(EngageHealing()),
                Timing.RunCoroutine(RegenMedkit())
            };
            ActiveAbilities = new AbilityBase[]{ new Abilities.ChaosMedicRevive(ply) };
            ply.CustomClassManager().AbilityIndex = 0;
            
            ply.ClearInventory();
            ply.AddItem(ItemType.KeycardChaosInsurgency);
            ply.AddItem(ItemType.GunAK);
            ply.AddItem(ItemType.Adrenaline);
            ply.AddItem(ItemType.ArmorCombat);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.Medkit);
            
            ply.Ammo[ItemType.Ammo762x39] = 120;

            Timing.CallDelayed(0.5f, () =>
            {
                ply.MaxHealth = 105;
                ply.Health = 105;
            });

            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, $"{SillySundayNames.NTFMedicSundayNames.PickRandom()}\n(Custom Class)", -1, "army_green");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, "Chaos Medic\n(Custom Class)", -1, "army_green");
            }

            ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=army_green>Chaos Medic</color>\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            ply.SendConsoleMessage("Name: Chaos Medic\n\nDescription: TLDR you're a green NTF Medic, kill your ex-coworkers lmao.\n\nAbility1: Revive (30s Cooldown)\n\nPassive Buffs: Healing aura, Medical Item Generation\nPassive Debuffs: None\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
        
        private IEnumerator<float> RegenMedkit()
        {
            var specialItems = new List<ItemType> { ItemType.Adrenaline, ItemType.SCP500, ItemType.SCP207 };

            while (true)
            {
                yield return Timing.WaitForSeconds(40f);

                if (Ply.CustomClassManager().CustomClass.Name != "Chaos Medic")
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
                if (Ply.CustomClassManager().CustomClass.Name != "Chaos Medic")
                    break;

                if (!Ply.IsCuffed)
                {
                    
                    foreach (var ply in Player.List.Where(x=> Vector3.Distance(x.Position, Ply.Position) <= 7))
                    {
                        if (ply == Ply) continue;
                        if (ply.Role.Team == Team.CHI || ply.Role.Team == Team.CDP  || ply.IsCuffed)
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
                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}