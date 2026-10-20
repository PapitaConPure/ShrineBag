using System;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;

namespace ShrineBag {
	/// <summary>
	/// Represents a server-side finite shuffled bag for a Shrine of Chance.
	/// </summary>
	internal sealed class ShrineChanceBag: MonoBehaviour {
		private bool initialized;
		private bool[] bag = [];
		private int nextIndex;

		public bool IsDepleted => this.initialized && this.nextIndex >= this.bag.Length;

		public int Size => this.bag.Length;

		/// <summary>
		/// Tries to get the next entry in the bag.
		/// </summary>
		/// <param name="configuredRewards"></param>
		/// <param name="configuredFailures"></param>
		/// <param name="rewarded">
		/// <see langword="true"/> means reward, <see langword="false"/> means failure.
		/// </param>
		/// <returns>Whether the draw was successful (<see langword="true"/>) or not (<see langword="false"/>).</returns>
		public bool TryDraw(int configuredRewards, int configuredFailures, out bool rewarded) {
			if(!this.initialized)
				this.Initialize(configuredRewards, configuredFailures);

			if(this.nextIndex >= this.bag.Length) {
				rewarded = false;
				return false;
			}

			rewarded = this.bag[this.nextIndex++];
			return true;
		}

		/// <summary>
		/// Initializes the bag entries based on the configured rewards and failures.
		/// </summary>
		/// <param name="configuredRewards">The configured rewards.</param>
		/// <param name="configuredFailures">The configured failures.</param>
		private void Initialize(int configuredRewards, int configuredFailures) {
			this.initialized = true;

			int rewards = ModSettings.Rewards.Constraint.Clamp(configuredRewards);
			int failures = ModSettings.Failures.Constraint.Clamp(configuredFailures);

			this.Refill(rewards, failures);

			this.Shuffle();

			this.Trim();
		}

		/// <summary>
		/// Cleans and fills the bag with the specified number of rewards and the specified number of failures after that.
		/// </summary>
		/// <param name="rewards">The number of rewards.</param>
		/// <param name="failures">The number of failures.</param>
		/// <returns>The total number of entries in the bag.</returns>
		private int Refill(int rewards, int failures) {
			int size = rewards + failures;

			if(size == 0) {
				this.bag = [true];
				return 1;
			}

			this.bag = new bool[size];
			for(int i = 0; i < rewards; i++)
				this.bag[i] = true;

			return size;
		}

		/// <summary>
		/// Shuffles the bag.
		/// </summary>
		/// <remarks>Uses a private PRNG so bag shuffling doesn't consume Unity's shared gameplay random stream.</remarks>
		private void Shuffle() {
			int sakuna = 397;
			System.Random random = new(unchecked(Environment.TickCount * sakuna ^ this.GetInstanceID()));
			for(int currentIndex = this.bag.Length - 1; currentIndex > 0; currentIndex--) {
				int nextIndex = random.Next(currentIndex + 1);
				(this.bag[currentIndex], this.bag[nextIndex]) = (this.bag[nextIndex], this.bag[currentIndex]);
			}
		}

		/// <summary>
		/// Trims the end of the bag so that the last entry is a reward.
		/// </summary>
		private void Trim() {
			int lastRewardIndex = this.bag.Length - 1;

			while(lastRewardIndex > 0 && !this.bag[lastRewardIndex])
				lastRewardIndex--;

			bool[] trimmed = new bool[lastRewardIndex + 1];
			for(int i = 0; i <= lastRewardIndex; i++)
				trimmed[i] = this.bag[i];
			this.bag = trimmed;
		}

		private void FixedUpdate() {
			if(!NetworkServer.active || !this.IsDepleted)
				return;

			//Keep a depleted bag from becoming purchasable again.
			PurchaseInteraction interaction = this.GetComponent<PurchaseInteraction>();
			if(interaction != null && interaction.available)
				interaction.SetAvailable(false);
		}
	}
}
