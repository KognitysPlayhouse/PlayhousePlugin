using Exiled.API.Interfaces;

namespace PlayhousePlugin.Webhooks
{
    public class Translation : ITranslation
    {
        public string UsedCommand { get; set; } = ":keyboard: {0} ({1}) [{2}] used command: {3} {4}";

        public string HasRunClientConsoleCommand { get; set; } =
            ":keyboard: {0} ({1}) [{2}] has run a client-console command: {3} {4}";

        public string WaitingForPlayers { get; set; } = $":hourglass: Waiting for players...";

        public string RoundStarting { get; set; } = ":arrow_forward: Round starting: {0} players in round.";

        public string RoundEnded { get; set; } = ":stop_button: Round ended: {0} - Players online {1}/{2}.";

        public string HasDamagedForWith { get; set; } =
            ":crossed_swords: **{0} ({1}) [{2}]** has damaged **{3} ({4}) [{5}]** for *{6}* with __{7}__.*";

        public string HasKilledWith { get; set; } =
            ":skull_crossbones: **{0} ({1}) [{2}] killed {3} ({4}) [{5}] with {6}.**";

        public string ThrewAGrenade { get; set; } = ":boom: {0} ({1}) [{2}] threw a {3}.";

        public string UsedMedicalItem { get; set; } = ":medical_symbol: {0} ({1}) [{2}] healed with {3}.";

        public string ChangedRole { get; set; } = ":mens: {0} ({1}) [{2}] has been changed to a {3}.";

        public string ChaosInsurgencyHaveSpawned { get; set; } = ":spy: Chaos Insurgency has spawned with {0} players.";

        public string NineTailedFoxHaveSpawned { get; set; } = ":cop: Nine-Tailed Fox has spawned with {0} players.";

        public string HasJoinedTheGame { get; set; } = ":arrow_right: **{0} ({1}) [{2}] has joined the game.**";

        public string HasBeenFreedBy { get; set; } = ":unlock: {0} ({1}) [{2}] has been freed by {3} ({4}) [{5}].";

        public string HasBeenHandcuffedBy { get; set; } =
            ":lock: {0} ({1}) [{2}] has been handcuffed by {3} ({4}) [{5}].";

        public string HasStartedUsingTheIntercom { get; set; } =
            ":loud_sound: {0} ({1}) [{2}] has started using the intercom.";

        public string HasPickedUp { get; set; } = "{0} ({1}) [{2}] has picked up **{3}**.";

        public string HasDropped { get; set; } = "{0} ({1}) [{2}] has dropped **{3}**.";

        public string DecontaminationHasBegun { get; set; } = ":biohazard: **Deconamination has begun.**";

        public string HasEnteredPocketDimension { get; set; } =
            ":door: {0} ({1}) [{2}] has entered the pocket dimension.";

        public string HasEscapedPocketDimension { get; set; } =
            ":high_brightness: {0} ({1}) [{2}] has escaped the pocket dimension.";

        public string HasTriggeredATeslaGate { get; set; } = ":zap: {0} ({1}) [{2}] has triggered a tesla gate.";

        public string Scp914HasProcessedTheFollowingPlayers { get; set; } =
            ":gear: SCP-914 has processed the following players:\n **{0}**\nand items:\n **{1}**";

        public string HasClosedADoor { get; set; } = ":door: {0} ({1}) [{2}] has closed {3} door.";

        public string HasOpenedADoor { get; set; } = ":door: {0} ({1}) [{2}] has opened {3} door.";

        public string Scp914HasBeenActivated { get; set; } =
            ":gear: {0} ({1}) [{2}] has activated SCP-914 on setting {3}.";

        public string Scp914KnobSettingChanged { get; set; } =
            ":gear: {0} ({1}) [{2}] has changed the SCP-914 knob to {3}.";

        public string PlayerCanceledWarhead { get; set; } =
            ":no_entry: **{0} ({1}) [{2}] canceled warhead detonation sequence.**";

        public string CanceledWarhead { get; set; } = ":no_entry: **Warhead detonation sequence canceled.**";

        public string WarheadHasDetonated { get; set; } = ":radioactive: **The Alpha-warhead has detonated.**";

        public string PlayerWarheadStarted { get; set; } =
            ":radioactive: **{0} ({1}) [{2}] started the alpha-warhead countdown, detonation in: {3}.**";

        public string WarheadStarted { get; set; } =
            ":radioactive: **Alpha-warhead countdown initiated, detonation in: {0}.**";

        public string AccessedWarhead { get; set; } =
            ":key: {0} ({1}) [{2}] has accessed the Alpha-warhead detonation button cover.";

        public string CalledElevator { get; set; } = ":elevator: {0} ({1}) [{2}] has called an elevator.";

        public string UsedLocker { get; set; } = "{0} ({1}) [{2}] has opened a locker.";

        public string GeneratorEjected { get; set; } = "{0} ({1}) [{2}] has ejected a tablet from a generator.";

        public string GeneratorFinished { get; set; } =
            "Generator in {0} has finished it's charge up, {1} generators have been activated.";

        public string GeneratorInserted { get; set; } =
            ":calling: {0} ({1}) [{2}] has inserted a tablet into a generator.";

        public string Scp106WasContained { get; set; } = "{0} ({1}) [{2}] has been contained by the Femur Breaker.";

        public string Scp106Teleported { get; set; } = "{0} ({1}) [{2}] has teleported to a portal.";

        public string GainedLevel { get; set; } = "{0} ({1}) [{2}] has gained a level: {3} :arrow_right: {4}.";

        public string LeftServer { get; set; } = ":arrow_left: **{0} ({1}) [{2}] has left the server.**";

        public string ItemChanged { get; set; } =
            "{0} ({1}) [{2}] changed the item in their hand: {2} :arrow_right: {3}.";

        public string DedicatedServer { get; set; } = "Dedicated server";
    }
}