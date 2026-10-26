using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using RoR2;
using ShrineBag.Settings;
using ShrineBag.Testing;
using ShrineBag.Utils;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace ShrineBag {
	[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
	[NetworkCompatibility(CompatibilityLevel.NoNeedForSync, VersionStrictness.DifferentModVersionsAreOk)]
	[BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
	public sealed class Plugin: BaseUnityPlugin {
		internal static ConfigEntry<bool> Enabled;
		internal static ConfigEntry<int> RewardCount;
		internal static ConfigEntry<int> FailureCount;
		internal static ConfigEntry<ModSettings.BagGenModeKey> BagGenMode;
		internal static ConfigEntry<int> BagGenCount;
		internal static ConfigEntry<bool> OnlyTrimLast;

		internal static ConfigEntry<float> CreditsFactor;
		internal static ConfigEntry<float> WeightFactor;

		internal static ConfigEntry<bool> CouncilEnabled;
		internal static ConfigEntry<int> CouncilSize;

		internal static ConfigEntry<bool> Megumin;

		private static ShrineChanceSpawnCardManager shrineChanceSpawnCardManager;

		public void Awake() {
			this.RegisterSettings();
			this.TryRegisterRiskOfOptions();

			On.RoR2.ShrineChanceBehavior.AddShrineStack += ShrineChanceBehavior_AddShrineStack;
			On.RoR2.SceneDirector.PopulateScene += SceneDirector_PopulateScene;
			On.RoR2.SceneDirector.GenerateInteractableCardSelection += SceneDirector_GenerateInteractableCardSelection;
			On.RoR2.VehicleSeat.OnPassengerExit += VehicleSeat_OnPassengerExit;

#if DEBUG
			this.Logger.LogInfo($"{PluginInfo.PLUGIN_NAME} {PluginInfo.PLUGIN_VERSION} loaded.");
#endif
		}

		public void OnDestroy() {
			On.RoR2.ShrineChanceBehavior.AddShrineStack -= ShrineChanceBehavior_AddShrineStack;
			On.RoR2.SceneDirector.PopulateScene -= SceneDirector_PopulateScene;
			On.RoR2.SceneDirector.GenerateInteractableCardSelection -= SceneDirector_GenerateInteractableCardSelection;
			On.RoR2.VehicleSeat.OnPassengerExit -= VehicleSeat_OnPassengerExit;

			if(shrineChanceSpawnCardManager?.Initialized == true)
				shrineChanceSpawnCardManager.ResetCreditCost();
		}

		private static void ShrineChanceBehavior_AddShrineStack(
			On.RoR2.ShrineChanceBehavior.orig_AddShrineStack origAddShrineStack,
			ShrineChanceBehavior self,
			Interactor activator) {
			if(!NetworkServer.active || self == null || !Enabled.Value) {
				origAddShrineStack(self, activator);
				return;
			}

			ShrineChanceBagMarket bagMarket = self.GetComponent<ShrineChanceBagMarket>() ?? self.gameObject.AddComponent<ShrineChanceBagMarket>();

			if(!bagMarket.TryGetUsableBag(RewardCount.Value, FailureCount.Value, BagGenMode.Value, BagGenCount.Value, OnlyTrimLast.Value, out ShrineChanceBag bag)
			|| !bag.TryDraw(out bool rewarded)) {
				//Disable after using final entry from final bag, before another purchase can happen
				PurchaseInteraction interaction = self.GetComponent<PurchaseInteraction>();
				interaction?.SetAvailable(false);
				return;
			}

			//Keep as vanilla/compatible as possible
			float previousFailureChance = self.failureChance;

			self.failureChance = rewarded ? -1f : 1f;

			if(rewarded)
				//Offset by +1 because it's set before orig increments successfulPurchaseCount
				self.maxPurchaseCount = self.successfulPurchaseCount + (bagMarket.IsDepleted ? 1 : 2);

			try {
				origAddShrineStack(self, activator);
			} finally {
				self.failureChance = previousFailureChance;
			}
		}

		private static WeightedSelection<DirectorCard> SceneDirector_GenerateInteractableCardSelection(
			On.RoR2.SceneDirector.orig_GenerateInteractableCardSelection origGenerateInteractableCardSelection,
			SceneDirector self) {
			WeightedSelection<DirectorCard> directorWeights = origGenerateInteractableCardSelection(self);

			if(!NetworkServer.active || self == null || !Enabled.Value || WeightFactor.Value.AboutEquals(1f, 0.01f))
				return directorWeights;

			shrineChanceSpawnCardManager ??= new();
			shrineChanceSpawnCardManager.ApplyWeightFactor(directorWeights, WeightFactor.Value);

			return directorWeights;
		}

		private static void SceneDirector_PopulateScene(On.RoR2.SceneDirector.orig_PopulateScene origPopulateScene, SceneDirector self) {
			if(!NetworkServer.active || self == null) {
				origPopulateScene(self);
				return;
			}

			TheEvilCouncil.Prepare();

			shrineChanceSpawnCardManager ??= new();

			if(Enabled.Value && !CreditsFactor.Value.AboutEquals(1f, 0.01f))
				shrineChanceSpawnCardManager.ApplyCreditCostFactor(CreditsFactor.Value);
			else if(shrineChanceSpawnCardManager.Initialized)
				shrineChanceSpawnCardManager.ResetCreditCost();

			origPopulateScene(self);
		}

		private static void VehicleSeat_OnPassengerExit(On.RoR2.VehicleSeat.orig_OnPassengerExit origOnPassengerExit, VehicleSeat self, GameObject passenger) {
			origOnPassengerExit(self, passenger);

			if(!NetworkServer.active || self == null || passenger == null || !Enabled.Value)
				return;

			if(!self.currentPassengerBody.isPlayerControlled || Run.instance.stageClearCount != 0)
				return;

			if(TheEvilCouncil.AlreadySpawned || !CouncilEnabled.Value)
				return;

			TheEvilCouncil.BeEvil(CouncilSize.Value);
		}

		private void RegisterSettings() {
			Enabled      = ModSettings.Enabled.RegisterFor(this);
			RewardCount  = ModSettings.RewardCount.RegisterFor(this);
			FailureCount = ModSettings.FailureCount.RegisterFor(this);
			BagGenMode   = ModSettings.BagGenMode.RegisterFor(this);
			BagGenCount  = ModSettings.BagGenCount.RegisterFor(this);
			OnlyTrimLast = ModSettings.OnlyTrimLastBag.RegisterFor(this);

			CreditsFactor = ModSettings.CreditsFactor.RegisterFor(this);
			WeightFactor  = ModSettings.WeightFactor.RegisterFor(this);

			CouncilEnabled = ModSettings.CouncilEnabled.RegisterFor(this);
			CouncilSize    = ModSettings.CouncilSize.RegisterFor(this);

			Megumin = ModSettings.Megumin.RegisterFor(this);
		}

		private void TryRegisterRiskOfOptions() {
			if(!ModSettingsManagerCompat.Enabled)
				return;

			try {
				ModSettingsManagerCompat.SetModDescription(
					"Makes Shrines of Chance use finite, shuffled bags instead of being fully random!");
				ModSettingsManagerCompat.SetModIcon(ModIcon.Load());

				ModSettingsManagerCompat.AddCheckboxOption(Enabled);
				ModSettingsManagerCompat.AddSliderOption(RewardCount);
				ModSettingsManagerCompat.AddSliderOption(FailureCount);
				ModSettingsManagerCompat.AddChoiceOption(BagGenMode);
				ModSettingsManagerCompat.AddSliderOption(BagGenCount);
				ModSettingsManagerCompat.AddCheckboxOption(OnlyTrimLast);

				ModSettingsManagerCompat.AddSliderOption(CreditsFactor, new(FormatString: "x{0:0.00}", Step: 0.01f));
				ModSettingsManagerCompat.AddSliderOption(WeightFactor, new(FormatString: "x{0:0.00}", Step: 0.01f));

				ModSettingsManagerCompat.AddCheckboxOption(CouncilEnabled);
				ModSettingsManagerCompat.AddSliderOption(CouncilSize);

				ModSettingsManagerCompat.AddCheckboxOption(Megumin);
			} catch(Exception exception) {
				this.Logger.LogWarning($"Could not register Risk of Options settings: {exception}");
			}
		}
	}
}
