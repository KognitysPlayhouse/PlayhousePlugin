using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace PlayhousePlugin.CustomClass
{
    public class ChaosHeavy : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "Chaos Heavy";
        public override int AbilitiesNum { get; } = 0;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }
        
        public override void Dispose()
        {
            Ply.Scale = Vector3.one;
            base.Dispose();
        }

        public override void Escape()
        {
            Ply.Role.Type = RoleType.NtfPrivate;
            Ply.CustomClassManager().DisposeCustomClass();
            Ply.CustomClassManager().CustomClass = new NtfHeavy(Ply);
        }

        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
            Vector3 pos = ply.Position;
            Timing.CallDelayed(0.1f, () =>
            {
                ply.Position = pos;
                ply.CustomClassManager().CustomClass = new ChaosHeavy(ply);
            });
        }
        
        public ChaosHeavy(Player ply)
        {
            Ply = ply;
            PassiveAbilities = new[]
            {
                Timing.RunCoroutine(EngageAmmo())
            };
            ActiveAbilities = new AbilityBase[] { };

            ply.ClearInventory();
            ply.AddItem(ItemType.GunLogicer);
            ply.AddItem(ItemType.GunAK);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.Adrenaline);
            ply.AddItem(ItemType.KeycardChaosInsurgency);
            ply.AddItem(ItemType.ArmorHeavy);
            
            ply.Ammo[ItemType.Ammo762x39] = 200;

            Timing.CallDelayed(0.25f, () =>
            {
                ply.MaxHealth = 200;
                ply.Health = 200;
            });

            ply.Scale = new Vector3((float)1.1, (float)1.1, (float)1.1);

            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, $"{SillySundayNames.NTFHeavySundayNames.PickRandom()}\n(Custom Class)", -1, "army_green");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, "Chaos Heavy\n(Custom Class)", -1, "army_green");
            }

            ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=green>Chaos Heavy!</color>\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            ply.SendConsoleMessage("Name: Chaos Heavy\n\nDescription: Bigger model, More HP, More Ammo and a better selection of guns makes the Chaos Heavy objectively inferior to Chaos Bulldozer! Who detained you???\n\nPassive Buffs: 200 Max HP, Ammunition providing aura\nPassive Debuffs: Large Hitbox\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }

        private IEnumerator<float> EngageAmmo()
        {
            yield return Timing.WaitForSeconds(2f);
            List<Player> PlayersAlreadyAffected = new List<Player>();
            while (true)
            {
                PlayersAlreadyAffected.Clear();
                if (Ply.CustomClassManager().CustomClass.Name != "Chaos Heavy")
                    break;

                if (!Ply.IsCuffed)
                {
                    foreach (var ply in Player.List.Where(x=> Vector3.Distance(x.Position, Ply.Position) <= 7))
                    {
                        if (ply == Ply) continue;
                        if ((ply.Role.Team == Team.CHI || ply.Role.Team == Team.CDP ) && !ply.IsCuffed)
                        {
                            if (!PlayersAlreadyAffected.Contains(ply))
                            {
                                UtilityMethods.ApplyAmmoRegen(ply, 20, true, Ply);
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