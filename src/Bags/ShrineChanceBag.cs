using ShrineBag.Settings;
using System;

namespace ShrineBag {
	/// <summary>
	/// Represents a server-side finite shuffled bag for a Shrine of Chance.
	/// </summary>
	internal sealed class ShrineChanceBag {
		private readonly bool[] outcomes;
		private int nextIndex;

		/// <summary>
		/// Whether the bag is depleted (<see langword="true"/>) or not (<see langword="false"/>).
		/// </summary>
		public bool IsDepleted => this.nextIndex >= this.outcomes.Length;

		/// <summary>
		/// The amount of non-consumed outcomes stored in the bag.
		/// </summary>
		public int Size => this.outcomes.Length - nextIndex;

		/// <summary>
		/// The number of outcomes in the bag, including the ones that have been "consumed".
		/// </summary>
		public int Capacity => this.outcomes.Length;

		public record ShrineChanceBagConfig(Random RNG = null, bool Trim = false);
		public ShrineChanceBag(int configuredRewardCount, int configuredFailureCount, ShrineChanceBagConfig config = null) {
			int rewardCount = ModSettings.RewardCount.Constraint.Clamp(configuredRewardCount);
			int failureCount = ModSettings.FailureCount.Constraint.Clamp(configuredFailureCount);

			bool[] outcomes = GenerateOutcomes(rewardCount, failureCount);
			ShuffleOutcomes(outcomes, config?.RNG ?? new());
			if(config != null && config.Trim)
				TrimOutcomesEnd(ref outcomes);

			this.outcomes = outcomes;
		}

		/// <summary>
		/// Tries to get the next entry in the bag.
		/// </summary>
		/// <param name="rewarded">
		/// <see langword="true"/> means reward, <see langword="false"/> means failure.
		/// </param>
		/// <returns>Whether the draw was successful (<see langword="true"/>) or not (<see langword="false"/>).</returns>
		public bool TryDraw(out bool rewarded) {
			if(this.IsDepleted) {
				rewarded = false;
				return false;
			}

			rewarded = this.outcomes[this.nextIndex++];
			return true;
		}

		/// <summary>
		/// Fills the bag from zero, with '<paramref name="rewards"/>' number of reward entries first and '<paramref name="failures"/>' number of failure entries next.
		/// </summary>
		/// <param name="rewards">The number of rewards.</param>
		/// <param name="failures">The number of failures.</param>
		/// <returns>The total number of entries in the bag.</returns>
		private static bool[] GenerateOutcomes(int rewards, int failures) {
			int size = rewards + failures;

			if(size == 0)
				return [true];

			bool[] outcomes = new bool[size];

			for(int i = 0; i < rewards; i++)
				outcomes[i] = true;

			return outcomes;
		}

		/// <summary>
		/// Shuffles the bag.
		/// </summary>
		/// <remarks>Uses a private PRNG so bag shuffling doesn't consume Unity's shared gameplay random stream.</remarks>
		private static void ShuffleOutcomes(bool[] outcomes, Random rng) {
			for(int currentIndex = outcomes.Length - 1; currentIndex > 0; currentIndex--) {
				int nextIndex = rng.Next(currentIndex + 1);
				(outcomes[currentIndex], outcomes[nextIndex]) = (outcomes[nextIndex], outcomes[currentIndex]);
			}
		}

		/// <summary>
		/// Trims the end of the bag so that the last entry is a reward.
		/// </summary>
		private static void TrimOutcomesEnd(ref bool[] outcomes) {
			int lastRewardIndex = outcomes.Length - 1;

			while(lastRewardIndex > 0 && !outcomes[lastRewardIndex])
				lastRewardIndex--;

			bool[] trimmed = new bool[lastRewardIndex + 1];
			for(int i = 0; i <= lastRewardIndex; i++)
				trimmed[i] = outcomes[i];

			outcomes = trimmed;
		}
	}
}
