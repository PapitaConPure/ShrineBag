using BepInEx;
using BepInEx.Configuration;
using RoR2;
using ShrineBag.Settings;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace ShrineBag {
	[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
	[BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
	public sealed class Plugin: BaseUnityPlugin {
		public const string PluginGuid = "com.papitaconpure.shrinebag";
		public const string PluginName = "Shrine Bag";
		public const string PluginVersion = "1.0.2";

		internal static ConfigEntry<bool> Enabled;
		internal static ConfigEntry<int> RewardCount;
		internal static ConfigEntry<int> FailureCount;
		internal static ConfigEntry<ModSettings.BagsPerShrineOption> BagsPerShrine;
		internal static ConfigEntry<float> CreditsFactor;
		internal static ConfigEntry<bool> TheCouncil;
		internal static ConfigEntry<bool> Megumin;

		private static ShrineChanceSpawnCardManager shrineChanceSpawnCardManager;

		public void Awake() {
			this.RegisterSettings();
			this.TryRegisterRiskOfOptions();

			On.RoR2.ShrineChanceBehavior.AddShrineStack += ShrineChanceBehavior_AddShrineStack;
			On.RoR2.SceneDirector.Start += SceneDirector_Start;
			On.RoR2.VehicleSeat.OnPassengerExit += VehicleSeat_OnPassengerExit;

#if DEBUG
			this.Logger.LogInfo($"{PluginName} {PluginVersion} loaded.");
#endif
		}

		public void OnDestroy() {
			On.RoR2.ShrineChanceBehavior.AddShrineStack -= ShrineChanceBehavior_AddShrineStack;
			On.RoR2.SceneDirector.Start -= SceneDirector_Start;
			On.RoR2.VehicleSeat.OnPassengerExit -= VehicleSeat_OnPassengerExit;
		}

		private static void ShrineChanceBehavior_AddShrineStack(
			On.RoR2.ShrineChanceBehavior.orig_AddShrineStack origAddShrineStack,
			ShrineChanceBehavior self,
			Interactor activator) {
			if(!NetworkServer.active || self == null || !Enabled.Value) {
				origAddShrineStack(self, activator);
				return;
			}

			ShrineChanceBag bag = self.GetComponent<ShrineChanceBag>() ?? self.gameObject.AddComponent<ShrineChanceBag>();

			if(!bag.TryDraw(RewardCount.Value, FailureCount.Value, out bool reward)) {
				//Disable bag after using final entry, before another purchase can happen
				PurchaseInteraction interaction = self.GetComponent<PurchaseInteraction>();
				interaction?.SetAvailable(false);
				return;
			}

			//Keep as vanilla/compatible as possible
			float previousFailureChance = self.failureChance;

			self.failureChance = reward ? -1f : 1f;
			self.maxPurchaseCount = RewardCount.Value;

			try {
				origAddShrineStack(self, activator);
			} finally {
				self.failureChance = previousFailureChance;
			}
		}

		//Could just set the credit cost in Awake, but config change would require a restart
		private static void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start origStart, SceneDirector self) {
			if(!NetworkServer.active || self == null) {
				origStart(self);
				return;
			}

			TheEvilCouncil.Prepare();

			shrineChanceSpawnCardManager ??= new();

			if(Enabled.Value && CreditsFactor.Value != 1f)
				shrineChanceSpawnCardManager.ApplyCreditCostFactor(CreditsFactor.Value);
			else if(shrineChanceSpawnCardManager.Initialized)
				shrineChanceSpawnCardManager.ResetCreditCost();

			origStart(self);
		}

		private static void VehicleSeat_OnPassengerExit(On.RoR2.VehicleSeat.orig_OnPassengerExit origOnPassengerExit, VehicleSeat self, GameObject passenger) {
			origOnPassengerExit(self, passenger);

			if(!NetworkServer.active || self == null)
				return;

			if(TheEvilCouncil.Spawned)
				return;

			if(Run.instance.stageClearCount == 0 && TheCouncil.Value)
				TheEvilCouncil.BeEvil();
		}

		private void RegisterSettings() {
			Enabled       = ModSettings.Enabled.RegisterFor(this);
			RewardCount   = ModSettings.Rewards.RegisterFor(this);
			FailureCount  = ModSettings.Failures.RegisterFor(this);
			BagsPerShrine = ModSettings.BagsPerShrine.RegisterFor(this);
			CreditsFactor = ModSettings.CreditsFactor.RegisterFor(this);

			TheCouncil    = ModSettings.TheCouncil.RegisterFor(this);

			Megumin       = ModSettings.Megumin.RegisterFor(this);
		}

		private void TryRegisterRiskOfOptions() {
			if(!ModSettingsManagerCompat.Enabled)
				return;

			try {
				ModSettingsManagerCompat.SetModDescription(
					"Each Shrine of Chance gets a finite shuffled bag with a configurable number of rewards and failures.");
				ModSettingsManagerCompat.SetModIcon(ModIcon.Load());

				ModSettingsManagerCompat.AddCheckboxOption(Enabled);
				ModSettingsManagerCompat.AddSliderOption(RewardCount);
				ModSettingsManagerCompat.AddSliderOption(FailureCount);
				ModSettingsManagerCompat.AddChoiceOption(BagsPerShrine);
				ModSettingsManagerCompat.AddSliderOption(CreditsFactor, new(FormatString: "x{0:0.00}", Step: 0.01f));

				ModSettingsManagerCompat.AddCheckboxOption(TheCouncil);

				ModSettingsManagerCompat.AddCheckboxOption(Megumin);
			} catch(Exception exception) {
				this.Logger.LogWarning($"Could not register Risk of Options settings: {exception}");
			}
		}
	}
}
