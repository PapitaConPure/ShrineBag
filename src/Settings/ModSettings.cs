namespace ShrineBag.Settings {
	internal static partial class ModSettings {
		public static readonly Entry<bool> Enabled = new(
			section: "General",
			name: "Enabled",
			description: "Enables the mod.",
			defaultValue: true);

		public static readonly ConstrainedEntry<int, RangeEntryConstraint<int>> RewardCount = new(
			section: "General",
			name: "Total Rewards per Bag",
			description: Paragraph(
				"Number of prayers that give a reward, per bag.",
				"=== You offer to the shrine and are rewarded! ==="),
			defaultValue: 2,
			constraint: new(Min: 1, Max: 20));

		public static readonly ConstrainedEntry<int, RangeEntryConstraint<int>> FailureCount = new(
			section: "General",
			name: "Max. Failures per Bag",
			description: Paragraph(
				"Number of prayers that give nothing, per bag.",
				"=== You offer to the shrine, but gain nothing. ==="),
			defaultValue: 4,
			constraint: new(Min: 0, Max: 20));

		public enum BagGenModeKey {
			Exact,
			Random,
			Infinite,
		}

		public static readonly ConstrainedEntry<BagGenModeKey, ListEntryConstraint<BagGenModeKey>> BagGenMode = new(
			section: "General",
			name: "Bag Gen. Mode",
			description: Paragraph(
				"Determines how many bags of outcomes each shrine holds. Works together with 'Bag Gen. Count'.",
				"When a bag is depleted, the shrine uses the next bag if one is available.",
				"",
				"Let N be the configured 'Bag Count':",
				"* Exact: each shrine holds exactly N bags.",
				"* Random: each shrine randomly holds 1 to N bags.",
				"* Infinite: every shrine holds infinite bags!"),
			defaultValue: BagGenModeKey.Exact,
			constraint: new(typeof(BagGenModeKey)));

		public static readonly ConstrainedEntry<int, RangeEntryConstraint<int>> BagGenCount = new(
			section: "General",
			name: "Bag Gen. Count",
			description: Paragraph(
				"Sets the number of bags used by 'Bag Gen. Mode'.",
				"When a bag is depleted, the shrine uses the next bag if one is available.",
				"",
				"* Exact: each shrine holds exactly this many bags.",
				"* Random: each shrine randomly holds up to this many bags.",
				"* Infinite: this value has no effect."),
			defaultValue: 1,
			constraint: new(Min: 1, Max: 5));

		public static readonly Entry<bool> OnlyTrimLastBag = new(
			section: "General",
			name: "Only Trim Last Bag",
			description: Paragraph(
				"Bags can be 'trimmed' so that their last outcome is always a reward.",
				"",
				"* When enabled, only the final bag is trimmed. This can result in wider gaps between rewards.",
				"* When disabled, every bag is trimmed. This makes in-between bags that would otherwise end with one or more failures smaller."),
			defaultValue: true);

		public static readonly ConstrainedEntry<float, RangeEntryConstraint<float>> CreditsFactor = new(
			section: "General",
			name: "Credit Cost Multiplier",
			description: Paragraph(
				"You can scale the spawn cost of each shrine by a factor in relation to vanilla." +
				"This may help with balance but it's usually better left untouched.",
				"",
				"By the way, I didn't actually test this works. Whoops! Default should tho."),
			defaultValue: 1f,
			constraint: new(Min: 0.1f, Max: 8f));

		public static readonly Entry<bool> CouncilEnabled = new(
			section: "Testing",
			name: "Spawn The Council",
			description: "Spawns 'The Council' on stage 1 for testing purposes.",
			defaultValue: false);

		public static readonly ConstrainedEntry<int, RangeEntryConstraint<int>> CouncilSize = new(
			section: "Testing",
			name: "Council Size",
			description: "Number of members that constitute 'The Council'.",
			defaultValue: 5,
			constraint: new(Min: 3, Max: 12));

		public static readonly Entry<bool> Megumin = new(
			section: "Megumin",
			name: "Megumin",
			description: "Megumin",
			defaultValue: true);
	}
}
