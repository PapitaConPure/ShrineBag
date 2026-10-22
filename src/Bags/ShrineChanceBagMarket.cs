using ShrineBag.Settings;
using System;
using UnityEngine;

namespace ShrineBag {
	/// <summary>
	/// Represents a manager of bags of outcomes for Shrines of Chance.
	/// </summary>
	internal sealed class ShrineChanceBagMarket: MonoBehaviour {
		private ShrineChanceBag currentBag;
		private int usedBags;
		private int totalBags;
		private bool infinite;
		private bool onlyTrimLastBag;
		private bool initialized;
		private System.Random rng;

		/// <summary>
		/// Whether the market ran out of usable bags (<see langword="true"/>) or not (<see langword="false"/>).
		/// </summary>
		public bool IsDepleted => this.initialized && !this.infinite && this.currentBag.IsDepleted && this.usedBags >= this.totalBags;

		/// <summary>
		/// Initializes the bag market based on the bags' configured rewards and failures and the market's configured generation settings.
		/// </summary>
		/// <param name="configuredBagRewardCount">The configured reward count for each bag.</param>
		/// <param name="configuredBagFailureCount">The configured failure count for each bag.</param>
		/// <param name="configuredBagGenMode">The configured bag generation mode.</param>
		/// <param name="configuredBagGenCount">The configured bag generation count, for when the mode is set to <see cref="ModSettings.BagGenModeKey.Exact"/> or <see cref="ModSettings.BagGenModeKey.Random"/>.</param>
		/// <remarks>Creates the first bag under <see cref="currentBag"/>.</remarks>
		private void Initialize(
			int configuredBagRewardCount,
			int configuredBagFailureCount,
			ModSettings.BagGenModeKey configuredBagGenMode,
			int configuredBagGenCount,
			bool onlyTrimLastBag) {
			ModSettings.BagGenModeKey bagGenMode = ModSettings.BagGenMode.Constraint.Clamp(configuredBagGenMode);
			int bagGenCount = ModSettings.BagGenCount.Constraint.Clamp(configuredBagGenCount);

			int sakuna = 397;
			this.rng = new(unchecked(Environment.TickCount * sakuna ^ this.GetInstanceID()));

			this.usedBags = 0;
			this.totalBags = bagGenMode switch {
				ModSettings.BagGenModeKey.Exact => bagGenCount,
				ModSettings.BagGenModeKey.Random => this.rng.Next(1, bagGenCount + 1),
				_ => -1,
			};
			this.onlyTrimLastBag = onlyTrimLastBag;
			this.infinite = bagGenMode == ModSettings.BagGenModeKey.Infinite;

			this.TryCreateBag(configuredBagRewardCount, configuredBagFailureCount);

			this.initialized = true;
		}

		/// <summary>
		/// Obtains the current bag if it's not depleted, or tries to create a new bag otherwise.
		/// </summary>
		/// <param name="configuredBagRewardCount">The configured reward count.</param>
		/// <param name="configuredBagFailureCount">The configured failure count.</param>
		/// <param name="bagGenMode">The configured bag generation mode.</param>
		/// <param name="bagGenCount">The configured bag generation count, for when the mode is set to <see cref="ModSettings.BagGenModeKey.Exact"/> or <see cref="ModSettings.BagGenModeKey.Random"/>.</param>
		/// <param name="bag">The obtained bag.</param>
		/// <returns>Whether a usable bag could be obtained (<see langword="true"/>) or not (<see langword="false"/>).</returns>
		public bool TryGetUsableBag(
			int configuredBagRewardCount,
			int configuredBagFailureCount,
			ModSettings.BagGenModeKey bagGenMode,
			int bagGenCount,
			bool onlyTrimLastBag,
			out ShrineChanceBag bag) {
			if(!this.initialized)
				this.Initialize(configuredBagRewardCount, configuredBagFailureCount, bagGenMode, bagGenCount, onlyTrimLastBag);

			if(this.currentBag.IsDepleted && !this.TryCreateBag(configuredBagRewardCount, configuredBagFailureCount)) {
				bag = null;
				return false;
			}

			bag = this.currentBag;
			return true;
		}

		/// <summary>
		/// Gets the total number of rewards available across bags owned by the market.
		/// </summary>
		public int GetTotalRewardCount(int configuredBagRewardCount)
			=> this.infinite
				? int.MaxValue
				: ModSettings.RewardCount.Constraint.Clamp(configuredBagRewardCount) * this.totalBags;

		/// <summary>
		/// Tries to create a new bag based on the configured bag generation settings.
		/// </summary>
		/// <param name="configuredBagRewardCount">The configured reward count.</param>
		/// <param name="configuredBagFailureCount">The configured failure count.</param>
		/// <param name="bag">The obtained bag.</param>
		/// <returns>Whether bag could be created (<see langword="true"/>) or not (<see langword="false"/>).</returns>
		/// <remarks>Assigns <see cref="currentBag"/>.</remarks>
		private bool TryCreateBag(int configuredBagRewardCount, int configuredBagFailureCount) {
			if(this.IsDepleted)
				return false;

			this.usedBags++;

			bool shouldTrim = !this.onlyTrimLastBag || (!this.infinite && this.usedBags >= this.totalBags);
			this.currentBag = new(
				configuredBagRewardCount,
				configuredBagFailureCount,
				new(
					RNG: this.rng,
					Trim: shouldTrim));

			return true;
		}
	}
}
