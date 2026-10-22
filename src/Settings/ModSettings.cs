namespace ShrineBag.Settings {
	internal static partial class ModSettings {
		public static readonly Entry<bool> Enabled = new(
			section: "General",
			name: "Enabled",
			description: "Enables the mod.",
			@default: true);

		public static readonly ConstrainedEntry<int, RangeEntryConstraint<int>> Rewards = new(
			section: "General",
			name: "Total Rewards",
			description: "Number of prayers that constitute a reward." +
				"\n=== You offer to the shrine and are rewarded! ===",
			defaultValue: 2,
			constraint: new(Min: 1, Max: 20));

		public static readonly ConstrainedEntry<int, RangeEntryConstraint<int>> Failures = new(
			section: "General",
			name: "Max. Failures",
			description: "Number of prayers that will yield nothing." +
				"\n=== You offer to the shrine, but gain nothing. ===",
			defaultValue: 4,
			constraint: new(Min: 0, Max: 20));

		public enum BagsPerShrineOption {
			One,
			Random,
			Infinite,
		}

		public static readonly ConstrainedEntry<BagsPerShrineOption, ListEntryConstraint<BagsPerShrineOption>> BagsPerShrine = new(
			section: "General",
			name: "Bags per Shrine",
			description: "If you want to, each shrine can hold more than one bag of outcomes." +
				"\nWhen one bag is depleted, the shrine discards it and starts using the next one available until no bags are left.",
			defaultValue: BagsPerShrineOption.One,
			constraint: new(typeof(BagsPerShrineOption)));

		public static readonly ConstrainedEntry<float, RangeEntryConstraint<float>> CreditsFactor = new(
			section: "General",
			name: "Stage Credits Multiplier",
			description: "You can scale the spawn cost of each shrine by a factor in relation to vanilla.\nThis may help with balance but it's usually better left untouched.",
			defaultValue: 1f,
			constraint: new(Min: 0.2f, Max: 2f));

		public static readonly Entry<bool> TheCouncil = new(
			section: "Testing",
			name: "Spawn The Council",
			description: "Spawns 'The Council' on stage 1 for testing purposes.",
			@default: false);

		public static readonly Entry<bool> Megumin = new(
			section: "Megumin",
			name: "Megumin",
			description: "Megumin",
			@default: true);
	}
}
