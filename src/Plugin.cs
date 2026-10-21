using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine.Networking;

namespace ShrineBag {
	[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
	[BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
	public sealed class Plugin: BaseUnityPlugin {
		public const string PluginGuid = "com.papitaconpure.shrinebag";
		public const string PluginName = "Shrine Bag";
		public const string PluginVersion = "1.0.1";

		internal static ConfigEntry<bool> Enabled;
		internal static ConfigEntry<int> RewardCount;
		internal static ConfigEntry<int> FailureCount;

		private void Awake() {
			this.RegisterSettings();
			this.TryRegisterRiskOfOptions();

			On.RoR2.ShrineChanceBehavior.AddShrineStack += OnAddShrineStack;

#if DEBUG
			this.Logger.LogInfo($"{PluginName} {PluginVersion} loaded.");
#endif
		}

		private void OnDestroy() {
			On.RoR2.ShrineChanceBehavior.AddShrineStack -= OnAddShrineStack;
		}

		private static void OnAddShrineStack(
			On.RoR2.ShrineChanceBehavior.orig_AddShrineStack origAddShrineStack,
			ShrineChanceBehavior self,
			Interactor activator) {

			if(!NetworkServer.active || !Enabled.Value || self == null) {
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

			//Keep as vanilla-compatible as possible
			float previousFailureChance = self.failureChance;

			self.failureChance = reward ? -1f : 1f;
			self.maxPurchaseCount = RewardCount.Value;

			try {
				origAddShrineStack(self, activator);
			} finally {
				self.failureChance = previousFailureChance;
			}
		}

		private void RegisterSettings() {
			Enabled = this.Config.Bind(
				"General",
				ModSettings.Enabled.Name,
				ModSettings.Enabled.Default,
				ModSettings.Enabled.Description);

			RewardCount = this.Config.Bind(
				"General",
				ModSettings.Rewards.Name,
				ModSettings.Rewards.Default,
				new ConfigDescription(
					ModSettings.Rewards.Description,
					ModSettings.Rewards.Constraint.AcceptableValue));

			FailureCount = this.Config.Bind(
				"General",
				ModSettings.Failures.Name,
				ModSettings.Failures.Default,
				new ConfigDescription(
					ModSettings.Failures.Description,
					ModSettings.Failures.Constraint.AcceptableValue));
		}

		private void TryRegisterRiskOfOptions() {
			if(!ModSettingsManagerCompat.Enabled)
				return;

			try {
				ModSettingsManagerCompat.SetModDescription(
					"Each Shrine of Chance gets a finite shuffled bag with a configurable number of rewards and failures.");
				ModSettingsManagerCompat.SetModIcon(ModIcon.Load());

				ModSettingsManagerCompat.AddCheckboxOption(Enabled);

				ModSettingsManagerCompat.AddSliderOption(RewardCount,
					ModSettings.Rewards.Constraint as ModSettings.RangeEntryConstraint<int>);

				ModSettingsManagerCompat.AddSliderOption(FailureCount,
					ModSettings.Failures.Constraint as ModSettings.RangeEntryConstraint<int>);
			} catch(System.Exception exception) {
				this.Logger.LogWarning($"Could not register Risk of Options settings: {exception}");
			}
		}
	}
}
