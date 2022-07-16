using System;
using System.Linq;
using Exiled.API.Features;
using MEC;
using System.Collections.Generic;
using System.Text;
using Exiled.Events.EventArgs;
using HarmonyLib;
using PlayhousePlugin.Webhooks;
using UnityEngine.Networking;


namespace PlayhousePlugin
{

	public class PlayhousePlugin : Plugin<Config, Translation>
	{
		public static PlayhousePlugin PlayhousePluginRef { get; private set; }
		public override string Name => nameof(PlayhousePlugin);
		public override string Author => "Kognity";
		
		public static PlayhousePlugin Singleton;
		
		public EventHandler Handler;
		private Webhooks.EventHandlers WebhookEventHandlers;
		
		public List<string> GameLogsQueue = new List<string>();
		public List<string> PvPLogsQueue = new List<string>();

		private List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();
		
		public int PatchesCounter { get; private set; }
		public Harmony Harmony { get; private set; }
		//public static readonly DamageTypes.DamageType Hemorrhage = new DamageTypes.DamageType("Hemorrhage", false, false, -1);
		//public static readonly DamageTypes.DamageType Scp018 = new DamageTypes.DamageType("SCP-018", false, true, -1);

		public PlayhousePlugin()
		{
			PlayhousePluginRef = this;
		}

		public override void OnEnabled()
		{
			if (!PlayhousePluginRef.Config.IsEnabled)
			{
				Log.Info("Playhouse Plugin Disabled");
				return;
			}

			if (PlayhousePluginRef.Config.UseCustomSequence)
				RainbowTagController.Colors = PlayhousePluginRef.Config.CustomSequence;

			//AddDamageType(Hemorrhage);
			//AddDamageType(Scp018);
			
			Singleton = this;
			
			Handler = new EventHandler(this);
			Handler.ConnectToServer();
			Handler.SetUpWebsockets();
			Exiled.Events.Handlers.Server.EndingRound += Handler.RoundEnding;
			Exiled.Events.Handlers.Player.Destroying += Handler.OnDestroying;
			Exiled.Events.Handlers.Server.WaitingForPlayers += Handler.WaitingForPlayers;
			Exiled.Events.Handlers.Player.Verified += Handler.VerifiedPlayer;
			Exiled.Events.Handlers.Player.Joined += Handler.PlayerJoined;
			Exiled.Events.Handlers.Server.RoundStarted += Handler.OnRoundStart;
			Exiled.Events.Handlers.Server.RespawningTeam += Handler.OnRespawn;
			Exiled.Events.Handlers.Player.Died += Handler.OnDied;
			Exiled.Events.Handlers.Player.Dying += Handler.OnDying;
			Exiled.Events.Handlers.Player.Escaping += Handler.OnEscape;
			Exiled.Events.Handlers.Player.PickingUpItem += Handler.OnItemPickup;
			Exiled.Events.Handlers.Player.DroppingAmmo += Handler.OnDroppingAmmo;
			Exiled.Events.Handlers.Player.Hurting += Handler.OnHurting;
			Exiled.Events.Handlers.Player.Shooting += Handler.OnShooting;
			Exiled.Events.Handlers.Player.ChangingMuteStatus += Handler.OnMute;
			Exiled.Events.Handlers.Player.ChangingIntercomMuteStatus += Handler.OnIntercomMute;
			Exiled.Events.Handlers.Player.UsedItem += Handler.OnMedicalItemDequipped;
			Exiled.Events.Handlers.Player.ChangingItem += Handler.ChangingItem;
			Exiled.Events.Handlers.Player.EnteringPocketDimension += Handler.OnEnterPocketDimension;
			Exiled.Events.Handlers.Scp096.AddingTarget += Handler.Add096Target;
			Exiled.Events.Handlers.Scp096.Enraging += Handler.OnRaging;
			Exiled.Events.Handlers.Player.Spawning += Handler.OnSpawning;
			Exiled.Events.Handlers.Player.DroppingItem += Handler.OnDropItem;
			Exiled.Events.Handlers.Player.Banning += Handler.OnBan;
			Exiled.Events.Handlers.Player.ReloadingWeapon += Handler.OnReloading;
			Exiled.Events.Handlers.Player.EnteringFemurBreaker += Handler.FemurBreaker;
			//Exiled.Events.Handlers.Player.ReceivingEffect += Handler.OnEffect;
			Exiled.Events.Handlers.Scp106.Containing += Handler.On106Contain;
			Exiled.Events.Handlers.Scp049.FinishingRecall += Handler.OnRevive;
			Exiled.Events.Handlers.Player.SpawningRagdoll += Handler.RagdollSpawning;
			Exiled.Events.Handlers.Player.ChangingRole += Handler.OnSetClass;
			Exiled.Events.Handlers.Player.InteractingDoor += Handler.DoorInteraction;
			Exiled.Events.Handlers.Player.InteractingElevator += Handler.ElevatorInteraction;
			Exiled.Events.Handlers.Player.RemovingHandcuffs += Handler.OnRemoveHandcuffs;
			Exiled.Events.Handlers.Scp096.CalmingDown += Handler.Nevercalmdown;
			Exiled.Events.Handlers.Server.RestartingRound += Handler.RoundRestart;
			Exiled.Events.Handlers.Player.UsingRadioBattery += Handler.OnUsingRadioBattery;
			Exiled.Events.Handlers.Map.Decontaminating += Handler.OnDecontaminating;
			Exiled.Events.Handlers.Warhead.Detonated += Handler.OnWarheadDetonated;
			Exiled.Events.Handlers.Scp914.Activating += Handler.OnActivating914;
			Exiled.Events.Handlers.Player.FlippingCoin += Handler.OnCoinFlip;
			Exiled.Events.Handlers.Player.PreAuthenticating += Handler.OnPreAuthenticating;
			Exiled.Events.Handlers.Player.ProcessingHotkey += Handler.OnHotkey;
			Exiled.Events.Handlers.Map.ExplodingGrenade += Handler.OnExplodingGrenade;
			Exiled.Events.Handlers.Warhead.Starting += Handler.OnStartingWarhead;
			Exiled.Events.Handlers.Warhead.Stopping += Handler.OnStoppingWarhead;
			Exiled.Events.Handlers.Player.Handcuffing += Handler.OnHandcuff;

			try
			{
				Harmony = new Harmony($"com.kognity.playhouseplugin.{PatchesCounter++}");
				Harmony.PatchAll();
				Log.Info("Patched everything boss!");
			}
			catch (Exception e)
			{
				Log.Error($"Patching failed!, " + e);
			}
			
			 Singleton = this;
			 /*
            WebhookEventHandlers = new Webhooks.EventHandlers();

            Exiled.Events.Handlers.Map.Decontaminating += WebhookEventHandlers.OnDecontaminating;
            Exiled.Events.Handlers.Map.GeneratorActivated += WebhookEventHandlers.OnGeneratorActivated;
            Exiled.Events.Handlers.Warhead.Starting += WebhookEventHandlers.OnStartingWarhead;
            Exiled.Events.Handlers.Warhead.Stopping += WebhookEventHandlers.OnStoppingWarhead;
            Exiled.Events.Handlers.Warhead.Detonated += WebhookEventHandlers.OnWarheadDetonated;
            //Exiled.Events.Handlers.Scp914.UpgradingItem += WebhookEventHandlers.OnUpgradingItem;
            //Exiled.Events.Handlers.Server.SendingRemoteAdminCommand += WebhookEventHandlers.OnSendingRemoteAdminCommand;
            Exiled.Events.Handlers.Server.WaitingForPlayers += WebhookEventHandlers.OnWaitingForPlayers;
            //Exiled.Events.Handlers.Server.SendingConsoleCommand += WebhookEventHandlers.OnSendingConsoleCommand;
            Exiled.Events.Handlers.Server.RoundStarted += WebhookEventHandlers.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += WebhookEventHandlers.OnRoundEnded;
            Exiled.Events.Handlers.Server.RespawningTeam += WebhookEventHandlers.OnRespawningTeam;
            Exiled.Events.Handlers.Scp914.ChangingKnobSetting += WebhookEventHandlers.OnChangingScp914KnobSetting;
            Exiled.Events.Handlers.Player.ItemUsed += WebhookEventHandlers.OnUsedMedicalItem;
            Exiled.Events.Handlers.Scp079.InteractingTesla += WebhookEventHandlers.OnInteractingTesla;
            Exiled.Events.Handlers.Player.PickingUpItem += WebhookEventHandlers.OnPickingUpItem;
            Exiled.Events.Handlers.Player.ActivatingGenerator += WebhookEventHandlers.OnInsertingGeneratorTablet;
            Exiled.Events.Handlers.Player.StoppingGenerator += WebhookEventHandlers.OnEjectingGeneratorTablet;
            Exiled.Events.Handlers.Scp079.GainingLevel += WebhookEventHandlers.OnGainingLevel;
            Exiled.Events.Handlers.Player.EscapingPocketDimension += WebhookEventHandlers.OnEscapingPocketDimension;
            Exiled.Events.Handlers.Player.EnteringPocketDimension += WebhookEventHandlers.OnEnteringPocketDimension;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel += WebhookEventHandlers.OnActivatingWarheadPanel;
            Exiled.Events.Handlers.Player.TriggeringTesla += WebhookEventHandlers.OnTriggeringTesla;
            Exiled.Events.Handlers.Player.ThrowingItem += WebhookEventHandlers.OnThrowingGrenade;
            Exiled.Events.Handlers.Player.Hurting += WebhookEventHandlers.OnHurting;
            Exiled.Events.Handlers.Player.Died += WebhookEventHandlers.OnDying;
            Exiled.Events.Handlers.Player.InteractingDoor += WebhookEventHandlers.OnInteractingDoor;
            Exiled.Events.Handlers.Player.InteractingElevator += WebhookEventHandlers.OnInteractingElevator;
            Exiled.Events.Handlers.Player.InteractingLocker += WebhookEventHandlers.OnInteractingLocker;
            Exiled.Events.Handlers.Player.IntercomSpeaking += WebhookEventHandlers.OnIntercomSpeaking;
            Exiled.Events.Handlers.Player.Handcuffing += WebhookEventHandlers.OnHandcuffing;
            Exiled.Events.Handlers.Player.RemovingHandcuffs += WebhookEventHandlers.OnRemovingHandcuffs;
            Exiled.Events.Handlers.Scp106.Teleporting += WebhookEventHandlers.OnTeleporting;
            Exiled.Events.Handlers.Player.DroppingItem += WebhookEventHandlers.OnItemDropped;
            Exiled.Events.Handlers.Player.Verified += WebhookEventHandlers.OnVerified;
            Exiled.Events.Handlers.Player.Destroying += WebhookEventHandlers.OnDestroying;
            Exiled.Events.Handlers.Player.ChangingRole += WebhookEventHandlers.OnChangingRole;
            Exiled.Events.Handlers.Player.ChangingItem += WebhookEventHandlers.OnChangingItem;
            Exiled.Events.Handlers.Scp914.Activating += WebhookEventHandlers.OnActivatingScp914;
            Exiled.Events.Handlers.Scp106.Containing += WebhookEventHandlers.OnContaining;

            Coroutines.Add(Timing.RunCoroutine(QueueSender(2f)));*/
		}

		public override void OnDisabled()
		{
			Exiled.Events.Handlers.Server.EndingRound -= Handler.RoundEnding;
			Exiled.Events.Handlers.Player.Destroying -= Handler.OnDestroying;
			Exiled.Events.Handlers.Server.WaitingForPlayers -= Handler.WaitingForPlayers;
			Exiled.Events.Handlers.Player.Verified -= Handler.VerifiedPlayer;
			Exiled.Events.Handlers.Server.RoundStarted -= Handler.OnRoundStart;
			Exiled.Events.Handlers.Server.RespawningTeam -= Handler.OnRespawn;
			Exiled.Events.Handlers.Player.Died -= Handler.OnDied;
			Exiled.Events.Handlers.Player.Dying -= Handler.OnDying;
			Exiled.Events.Handlers.Player.Escaping -= Handler.OnEscape;
			Exiled.Events.Handlers.Player.PickingUpItem -= Handler.OnItemPickup;
			Exiled.Events.Handlers.Player.DroppingAmmo -= Handler.OnDroppingAmmo;
			Exiled.Events.Handlers.Player.Hurting -= Handler.OnHurting;
			Exiled.Events.Handlers.Player.Shooting -= Handler.OnShooting;
			Exiled.Events.Handlers.Player.ChangingMuteStatus -= Handler.OnMute;
			Exiled.Events.Handlers.Player.ChangingIntercomMuteStatus -= Handler.OnIntercomMute;
			Exiled.Events.Handlers.Player.UsedItem -= Handler.OnMedicalItemDequipped;
			Exiled.Events.Handlers.Player.ChangingItem -= Handler.ChangingItem;
			Exiled.Events.Handlers.Player.EnteringPocketDimension -= Handler.OnEnterPocketDimension;
			Exiled.Events.Handlers.Scp096.AddingTarget -= Handler.Add096Target;
			Exiled.Events.Handlers.Scp096.Enraging -= Handler.OnRaging;
			Exiled.Events.Handlers.Player.Spawning -= Handler.OnSpawning;
			Exiled.Events.Handlers.Player.DroppingItem -= Handler.OnDropItem;
			Exiled.Events.Handlers.Player.DroppingAmmo -= Handler.OnDroppingAmmo;
			Exiled.Events.Handlers.Player.Banning -= Handler.OnBan;
			Exiled.Events.Handlers.Player.ReloadingWeapon -= Handler.OnReloading;
			Exiled.Events.Handlers.Player.EnteringFemurBreaker -= Handler.FemurBreaker;
			//Exiled.Events.Handlers.Player.ReceivingEffect -= Handler.OnEffect;
			Exiled.Events.Handlers.Scp106.Containing -= Handler.On106Contain;
			Exiled.Events.Handlers.Scp049.FinishingRecall -= Handler.OnRevive;
			Exiled.Events.Handlers.Player.SpawningRagdoll -= Handler.RagdollSpawning;
			Exiled.Events.Handlers.Player.ChangingRole -= Handler.OnSetClass;
			Exiled.Events.Handlers.Player.InteractingDoor -= Handler.DoorInteraction;
			Exiled.Events.Handlers.Player.InteractingElevator -= Handler.ElevatorInteraction;
			Exiled.Events.Handlers.Scp096.CalmingDown -= Handler.Nevercalmdown;
			Exiled.Events.Handlers.Server.RestartingRound -= Handler.RoundRestart;
			Exiled.Events.Handlers.Player.UsingRadioBattery -= Handler.OnUsingRadioBattery;
			Exiled.Events.Handlers.Map.Decontaminating -= Handler.OnDecontaminating;
			Exiled.Events.Handlers.Warhead.Detonated -= Handler.OnWarheadDetonated;
			Exiled.Events.Handlers.Player.FlippingCoin -= Handler.OnCoinFlip;
			Exiled.Events.Handlers.Player.PreAuthenticating -= Handler.OnPreAuthenticating;
			Exiled.Events.Handlers.Player.ProcessingHotkey -= Handler.OnHotkey;
			Exiled.Events.Handlers.Map.ExplodingGrenade -= Handler.OnExplodingGrenade;
			Exiled.Events.Handlers.Warhead.Starting -= Handler.OnStartingWarhead;
			Exiled.Events.Handlers.Warhead.Stopping -= Handler.OnStoppingWarhead;
			Exiled.Events.Handlers.Player.Handcuffing -= Handler.OnHandcuff;
			Handler = null;
			Harmony.UnpatchAll();
			
			/*
			Exiled.Events.Handlers.Map.Decontaminating -= WebhookEventHandlers.OnDecontaminating;
            Exiled.Events.Handlers.Map.GeneratorActivated -= WebhookEventHandlers.OnGeneratorActivated;
            Exiled.Events.Handlers.Warhead.Starting -= WebhookEventHandlers.OnStartingWarhead;
            Exiled.Events.Handlers.Warhead.Stopping -= WebhookEventHandlers.OnStoppingWarhead;
            Exiled.Events.Handlers.Warhead.Detonated -= WebhookEventHandlers.OnWarheadDetonated;
            //Exiled.Events.Handlers.Scp914.UpgradingItem -= WebhookEventHandlers.OnUpgradingItem;
            //Exiled.Events.Handlers.Server.SendingRemoteAdminCommand -= WebhookEventHandlers.OnSendingRemoteAdminCommand;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= WebhookEventHandlers.OnWaitingForPlayers;
            //Exiled.Events.Handlers.Server.SendingConsoleCommand -= WebhookEventHandlers.OnSendingConsoleCommand;
            Exiled.Events.Handlers.Server.RoundStarted -= WebhookEventHandlers.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= WebhookEventHandlers.OnRoundEnded;
            Exiled.Events.Handlers.Server.RespawningTeam -= WebhookEventHandlers.OnRespawningTeam;
            Exiled.Events.Handlers.Scp914.ChangingKnobSetting -= WebhookEventHandlers.OnChangingScp914KnobSetting;
            Exiled.Events.Handlers.Player.ItemUsed -= WebhookEventHandlers.OnUsedMedicalItem;
            Exiled.Events.Handlers.Scp079.InteractingTesla -= WebhookEventHandlers.OnInteractingTesla;
            Exiled.Events.Handlers.Player.PickingUpItem -= WebhookEventHandlers.OnPickingUpItem;
            Exiled.Events.Handlers.Player.ActivatingGenerator -= WebhookEventHandlers.OnInsertingGeneratorTablet;
            Exiled.Events.Handlers.Player.StoppingGenerator -= WebhookEventHandlers.OnEjectingGeneratorTablet;
            Exiled.Events.Handlers.Scp079.GainingLevel -= WebhookEventHandlers.OnGainingLevel;
            Exiled.Events.Handlers.Player.EscapingPocketDimension -= WebhookEventHandlers.OnEscapingPocketDimension;
            Exiled.Events.Handlers.Player.EnteringPocketDimension -= WebhookEventHandlers.OnEnteringPocketDimension;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel -= WebhookEventHandlers.OnActivatingWarheadPanel;
            Exiled.Events.Handlers.Player.TriggeringTesla -= WebhookEventHandlers.OnTriggeringTesla;
            Exiled.Events.Handlers.Player.ThrowingItem -= WebhookEventHandlers.OnThrowingGrenade;
            Exiled.Events.Handlers.Player.Hurting -= WebhookEventHandlers.OnHurting;
            Exiled.Events.Handlers.Player.Died -= WebhookEventHandlers.OnDying;
            Exiled.Events.Handlers.Player.InteractingDoor -= WebhookEventHandlers.OnInteractingDoor;
            Exiled.Events.Handlers.Player.InteractingElevator -= WebhookEventHandlers.OnInteractingElevator;
            Exiled.Events.Handlers.Player.InteractingLocker -= WebhookEventHandlers.OnInteractingLocker;
            Exiled.Events.Handlers.Player.IntercomSpeaking -= WebhookEventHandlers.OnIntercomSpeaking;
            Exiled.Events.Handlers.Player.Handcuffing -= WebhookEventHandlers.OnHandcuffing;
            Exiled.Events.Handlers.Player.RemovingHandcuffs -= WebhookEventHandlers.OnRemovingHandcuffs;
            Exiled.Events.Handlers.Scp106.Teleporting -= WebhookEventHandlers.OnTeleporting;
            Exiled.Events.Handlers.Player.DroppingItem -= WebhookEventHandlers.OnItemDropped;
            Exiled.Events.Handlers.Player.Verified -= WebhookEventHandlers.OnVerified;
            Exiled.Events.Handlers.Player.Destroying -= WebhookEventHandlers.OnDestroying;
            Exiled.Events.Handlers.Player.ChangingRole -= WebhookEventHandlers.OnChangingRole;
            Exiled.Events.Handlers.Player.ChangingItem -= WebhookEventHandlers.OnChangingItem;
            Exiled.Events.Handlers.Scp914.Activating -= WebhookEventHandlers.OnActivatingScp914;
            Exiled.Events.Handlers.Scp106.Containing -= WebhookEventHandlers.OnContaining;
            
            WebhookEventHandlers = null;

            foreach (var coroutine in Coroutines)
	            Timing.KillCoroutines(coroutine);*/
		}

		public override void OnReloaded() { }
	}
}
