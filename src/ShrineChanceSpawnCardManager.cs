using RoR2;
using ShrineBag.Settings;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ShrineBag {
	/// <summary>
	/// Represents a reduced Shrine of Chance Interactable Spawn Card (IRC) Manager
	/// </summary>
	internal sealed class ShrineChanceSpawnCardManager: MonoBehaviour {
		private record SpawnCardWithOriginalCost(InteractableSpawnCard SpawnCard, int OriginalCost);

		private static readonly string[] SHRINE_CHANCE_ASSET_VARIANT_PATHS = [
			"RoR2/Base/ShrineChance/iscShrineChance.asset",
			"RoR2/Base/ShrineChance/iscShrineChanceSnowy.asset",
			"RoR2/Base/ShrineChance/iscShrineChanceSandy.asset"
		];

		private bool initialized;
		private SpawnCardWithOriginalCost[] spawnCardVariants;

		public bool Initialized => this.initialized;

		public void OnDestroy() {
			if(this.Initialized)
				this.ResetCreditCost();
		}

		/// <summary>
		/// Sets the credit cost of the Shrine of Chance's Spawn Card relative to a factor of its original cost.
		/// </summary>
		/// <param name="configuredFactor">The factor used to compute the final credit cost of the interactable's SpawnCard.</param>
		public void ApplyCreditCostFactor(float configuredFactor) {
			if(!this.initialized)
				this.Initialize();

			float factor = ModSettings.CreditsFactor.Constraint.Clamp(configuredFactor);
			foreach(SpawnCardWithOriginalCost variant in this.spawnCardVariants)
				variant.SpawnCard.directorCreditCost = (int)MathF.Round(variant.OriginalCost * factor);
		}

		/// <summary>
		/// Sets the weight of the Shrine of Chance's Spawn Card relative to a factor of the original weight on the Director's selection.
		/// </summary>
		/// <remarks>Non-reversible (shouldn't matter).</remarks>
		/// <param name="directorWeights">The result of the original function that generates the Director's choices.</param>
		/// <param name="configuredFactor">The factor used to compute the final weight of the interactable's Spawn Card.</param>
		public void ApplyWeightFactor(WeightedSelection<DirectorCard> directorWeights, float configuredFactor) {
			if(!this.initialized)
				this.Initialize();

			int choiceIndex = this.FindShrineChanceCardChoice(directorWeights, out WeightedSelection<DirectorCard>.ChoiceInfo choice);
			float factor = ModSettings.WeightFactor.Constraint.Clamp(configuredFactor);
			directorWeights.ModifyChoiceWeight(choiceIndex, choice.weight * factor);
		}

		/// <summary>
		/// Resets the credit cost of the Shrine of Chance's Spawn Cards to the values perceived during initialization.
		/// </summary>
		internal void ResetCreditCost() {
			if(!this.initialized)
				this.Initialize();

			foreach(SpawnCardWithOriginalCost variant in this.spawnCardVariants)
				variant.SpawnCard.directorCreditCost = variant.OriginalCost;
		}

		/// <summary>
		/// Initializes this manager.
		/// </summary>
		private void Initialize() {
			this.spawnCardVariants = [..
				SHRINE_CHANCE_ASSET_VARIANT_PATHS
					.Select(assetVariantPath => Addressables
						.LoadAssetAsync<InteractableSpawnCard>(assetVariantPath)
						.WaitForCompletion() ?? throw new InvalidOperationException(
							$"Failed to load Shrine of Chance variant InteractableSpawnCard asset from path: {assetVariantPath}."
						))
					.Select(v => new SpawnCardWithOriginalCost(v, v.directorCreditCost))
			];

			this.initialized = true;
		}

		/// <summary>
		/// Shit fuck.
		/// </summary>
		/// <param name="directorWeights"></param>
		/// <param name="choice"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		private int FindShrineChanceCardChoice(WeightedSelection<DirectorCard> directorWeights, out WeightedSelection<DirectorCard>.ChoiceInfo choice) {
			for(int i = 0; i < directorWeights.Count; i++) {
				WeightedSelection<DirectorCard>.ChoiceInfo c = directorWeights.GetChoice(i);
				if(this.spawnCardVariants.Any(sc => c.value.spawnCard == sc.SpawnCard)) {
					choice = c;
					return i;
				}
			}

			throw new InvalidOperationException("Failed to find the ChoiceInfo associated to this manager's Spawn Card within the supplied WeightedSelection." +
				$"\nthis.spawnCardVariants[.SpawnCard]=\n{string.Join("\n", this.spawnCardVariants.Select(v => v.SpawnCard))}");
		}
	}
}
