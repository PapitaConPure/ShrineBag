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
		public ShrineChanceSpawnCardManager ApplyCreditCostFactor(float configuredFactor) {
			if(!this.initialized)
				this.Initialize();

			float factor = ModSettings.CreditsFactor.Constraint.Clamp(configuredFactor);
			this.spawnCard.directorCreditCost = (int)MathF.Round(this.originalCreditCost * factor);

			return this;
		}

		/// <summary>
		/// Sets the weight of the Shrine of Chance's Spawn Card relative to a factor of its original cost.
		/// </summary>
		/// <param name="configuredFactor">The factor used to compute the final weight of the interactable's Spawn Card.</param>
		public ShrineChanceSpawnCardManager ApplyWeightFactor(float configuredFactor) {
			if(!this.initialized)
				this.Initialize();

			float factor = ModSettings.WeightFactor.Constraint.Clamp(configuredFactor);
			this.spawnCard.directorCreditCost = (int)MathF.Round(this.originalCreditCost * factor);

			return this;
		}

		/// <summary>
		/// Resets the credit cost of the Shrine of Chance's Spawn Card to the originally perceived value.
		/// I actually have no idea if this might break things anyway or not.
		/// </summary>
		internal ShrineChanceSpawnCardManager ResetCreditCost() {
			if(!this.initialized)
				this.Initialize();

			this.spawnCard.directorCreditCost = this.originalCreditCost;

			return this;
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
	}
}
