using System;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Respawning;

namespace PlayhousePlugin.Webhooks
{
    public class EventHandlers
    {
        public void OnWaitingForPlayers()
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {PlayhousePlugin.Singleton.Translation.WaitingForPlayers}");
        }

        public void OnDecontaminating(DecontaminatingEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {PlayhousePlugin.Singleton.Translation.DecontaminationHasBegun}");
        }

        public void OnGeneratorActivated(GeneratorActivatedEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.GeneratorFinished, ev.Generator.Room.Name, Generator.List.Count(x=>x.IsEngaged))}");
        }

        public void OnStartingWarhead(StartingEventArgs ev)
        {
            var vars = ev.Player == null
                ? new object[] {Warhead.DetonationTimer}
                : new object[] {ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type, Warhead.DetonationTimer};
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(ev.Player == null ? PlayhousePlugin.Singleton.Translation.WarheadStarted : PlayhousePlugin.Singleton.Translation.PlayerWarheadStarted, vars)}");
        }

        public void OnStoppingWarhead(StoppingEventArgs ev)
        {
            var vars = ev.Player == null
                ? Array.Empty<object>()
                : new object[] {ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type};
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(ev.Player == null ? PlayhousePlugin.Singleton.Translation.CanceledWarhead : PlayhousePlugin.Singleton.Translation.PlayerCanceledWarhead, vars)}");
        }

        public void OnWarheadDetonated()
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {PlayhousePlugin.Singleton.Translation.WarheadHasDetonated}");
        }
        
        /*
        public void OnUpgradingItems(UpgradingItemEventArgs ev)
        {
            {
                StringBuilder players = StringBuilderPool.Shared.Rent();
                StringBuilder items = StringBuilderPool.Shared.Rent();

                foreach (Player player in ev.Players)
                    players.Append(player.Nickname).Append(" (").Append(player.UserId).Append(") [").Append(player.Role.Type)
                        .Append(']').AppendLine();

                foreach (Pickup item in ev.)
                    items.Append(item.ItemId.ToString()).AppendLine();

                PlayhousePlugin.Singleton.GameLogsQueue.Add(
                    $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.Scp914HasProcessedTheFollowingPlayers, players, items)}");
                StringBuilderPool.Shared.Return(players);
                StringBuilderPool.Shared.Return(items);
            }
        }*/

        public void OnRoundStarted()
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.RoundStarting, Player.Dictionary.Count)}");
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.RoundEnded, ev.LeadingTeam, Player.Dictionary.Count, CustomNetworkManager.slots)}");
        }

        public void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(ev.NextKnownTeam == SpawnableTeamType.ChaosInsurgency ? PlayhousePlugin.Singleton.Translation.ChaosInsurgencyHaveSpawned : PlayhousePlugin.Singleton.Translation.NineTailedFoxHaveSpawned, ev.Players.Count)}");
        }

        public void OnChangingScp914KnobSetting(ChangingKnobSettingEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.Scp914KnobSettingChanged, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type, ev.KnobSetting)}");
        }

        public void OnUsedMedicalItem(UsedItemEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.UsedMedicalItem, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type, ev.Item)}");
        }

        public void OnInteractingTesla(InteractingTeslaEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.HasTriggeredATeslaGate, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type)}");
        }

        public void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.HasPickedUp, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type, ev.Pickup.Type)}");
        }

        public void OnInsertingGeneratorTablet(ActivatingGeneratorEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.GeneratorInserted, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type)}");
        }

        public void OnEjectingGeneratorTablet(StoppingGeneratorEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.GeneratorEjected, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type)}");
        }

        public void OnGainingLevel(GainingLevelEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.GainedLevel, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type, ev.Player.ReferenceHub.scp079PlayerScript._curLvl, ev.NewLevel)}");
        }

        public void OnEscapingPocketDimension(EscapingPocketDimensionEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.HasEscapedPocketDimension, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type)}");
        }

        public void OnEnteringPocketDimension(EnteringPocketDimensionEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.HasEnteredPocketDimension, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type)}");
        }

        public void OnActivatingWarheadPanel(ActivatingWarheadPanelEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.AccessedWarhead, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type)}");
        }

        public void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.HasTriggeredATeslaGate, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type)}");
        }

        public void OnThrowingGrenade(ThrowingItemEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.ThrewAGrenade, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type, ev.Item.Type)}");
        }

        public void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker != null && ev.Target != null && ev.Amount != 0)
                PlayhousePlugin.Singleton.PvPLogsQueue.Add(
                    $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.HasDamagedForWith, ev.Attacker.Nickname, ev.Attacker.UserId, ev.Attacker.Role.Type, ev.Target.Nickname, ev.Target.UserId, ev.Target.Role.Type, ev.Amount, ev.Handler.Base.ServerLogsText)}");
        }

        public void OnDying(DiedEventArgs ev)
        {
            if (ev.Killer != null && ev.Target != null)
                PlayhousePlugin.Singleton.PvPLogsQueue.Add(
                    $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.HasKilledWith, ev.Killer.Nickname, ev.Killer.UserId, ev.Killer.Role.Type, ev.Target.Nickname, ev.Target.UserId, ev.Target.Role.Type, ev.Handler.Base.ServerLogsText)}");
        }

        public void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(ev.Door.IsOpen ? PlayhousePlugin.Singleton.Translation.HasClosedADoor : PlayhousePlugin.Singleton.Translation.HasOpenedADoor, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type, ev.Door.Nametag)}");
        }

        public void OnInteractingElevator(InteractingElevatorEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.CalledElevator, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type)}");
        }

        public void OnInteractingLocker(InteractingLockerEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.UsedLocker, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type)}");
        }

        public void OnIntercomSpeaking(IntercomSpeakingEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.HasStartedUsingTheIntercom, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type)}");
        }

        public void OnHandcuffing(HandcuffingEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.HasBeenHandcuffedBy, ev.Target.Nickname, ev.Target.UserId, ev.Target.Role.Type, ev.Cuffer.Nickname, ev.Cuffer.UserId, ev.Cuffer.Role.Type)}");
        }

        public void OnRemovingHandcuffs(RemovingHandcuffsEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.HasBeenFreedBy, ev.Target.Nickname, ev.Target.UserId, ev.Target.Role.Type, ev.Cuffer.Nickname, ev.Cuffer.UserId, ev.Cuffer.Role.Type)}");
        }

        public void OnTeleporting(TeleportingEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.Scp106Teleported, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type)}");
        }

        public void OnItemDropped(DroppingItemEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.HasDropped, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type, ev.Item.Type)}");
        }

        public void OnVerified(VerifiedEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.HasJoinedTheGame, ev.Player.Nickname, ev.Player.UserId, "REDACTED")}");
        }

        public void OnDestroying(DestroyingEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.LeftServer, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type)}");
        }

        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.ChangedRole, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type, ev.NewRole)}");
        }

        public void OnChangingItem(ChangingItemEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.ItemChanged, ev.Player.Nickname, ev.Player.UserId, ev.Player.CurrentItem?.Type ?? ItemType.None , ev.NewItem?.Type ?? ItemType.None)}"
            );
        }

        public void OnActivatingScp914(ActivatingEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.Scp914HasBeenActivated, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type, Exiled.API.Features.Scp914.KnobStatus)}");
        }

        public void OnContaining(ContainingEventArgs ev)
        {
            PlayhousePlugin.Singleton.GameLogsQueue.Add(
                $"{Date} {string.Format(PlayhousePlugin.Singleton.Translation.Scp106WasContained, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role.Type)}");
        }

        private string Date => $"[{DateTime.Now:HH:mm:ss}]";
    }
}