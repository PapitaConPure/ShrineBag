using RoR2;
using ShrineBag.Settings;
using System;
using UnityEngine.AddressableAssets;

namespace ShrineBag {
	/// <summary>
	/// Represents a reduced Shrine of Chance Interactable Spawn Card (IRC) Manager
	/// </summary>
	internal sealed class ShrineChanceSpawnCardManager {
		private bool initialized;
		private int originalCreditCost;
		private InteractableSpawnCard spawnCard;

		public bool Initialized => this.initialized;

		/// <summary>
		/// Sets the credit cost of the Shrine of Chance's Spawn Card relative to a factor of its original cost.
		/// </summary>
		/// <param name="configuredFactor">The factor used to compute the final credit cost of the interactable's SpawnCard.</param>
		public void ApplyCreditCostFactor(float configuredFactor) {
			if(!this.initialized)
				this.Initialize();

			float factor = ModSettings.CreditsFactor.Constraint.Clamp(configuredFactor);
			this.spawnCard.directorCreditCost = (int)MathF.Round(this.originalCreditCost * factor);
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

			int choiceIndex = this.FindCardChoice(directorWeights, out WeightedSelection<DirectorCard>.ChoiceInfo choice);
			float factor = ModSettings.WeightFactor.Constraint.Clamp(configuredFactor);
			directorWeights.ModifyChoiceWeight(choiceIndex, choice.weight * factor);
		}

		/// <summary>
		/// Resets the credit cost of the Shrine of Chance's Spawn Card to the originally perceived value.
		/// </summary>
		internal void ResetCreditCost() {
			if(!this.initialized)
				this.Initialize();

			this.spawnCard.directorCreditCost = this.originalCreditCost;
		}

		/// <summary>
		/// Initializes this manager.
		/// </summary>
		private void Initialize() {
			InteractableSpawnCard spawnCard = Addressables
				.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineChance/iscShrineChance.asset")
				.WaitForCompletion() ?? throw new InvalidOperationException(
					"Failed to load Shrine of Chance InteractableSpawnCard."
				);

			this.originalCreditCost = spawnCard.directorCreditCost;
			this.spawnCard = spawnCard;
			this.initialized = true;
		}

		/// <summary>
		/// Shit fuck.
		/// </summary>
		/// <param name="directorWeights"></param>
		/// <param name="choice"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		private int FindCardChoice(WeightedSelection<DirectorCard> directorWeights, out WeightedSelection<DirectorCard>.ChoiceInfo choice) {
			for(int i = 0; i < directorWeights.Count; i++) {
				WeightedSelection<DirectorCard>.ChoiceInfo c = directorWeights.GetChoice(i);
				if(c.value.spawnCard == this.spawnCard) {
					choice = c;
					return i;
				}
			}

			throw new InvalidOperationException("Failed to find the ChoiceInfo associated to this manager's Spawn Card within the supplied WeightedSelection.");
		}
	}
}
