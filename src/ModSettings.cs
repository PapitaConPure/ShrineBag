using BepInEx.Configuration;
using System;

namespace ShrineBag {
	internal static class ModSettings {
		internal interface IEntryConstraint<TValue> where TValue : IComparable {
			public AcceptableValueBase AcceptableValue { get; }

			public TValue Clamp(TValue value);

			public bool IsValid(object value);
		}

		internal sealed class RangeEntryConstraint<TValue>(TValue min, TValue max): IEntryConstraint<TValue> where TValue : IComparable {
			public AcceptableValueBase AcceptableValue { get; } = new AcceptableValueRange<TValue>(min, max);

			public TValue Min { get; } = min;

			public TValue Max { get; } = max;

			public TValue Clamp(TValue value) => (TValue)this.AcceptableValue.Clamp(value);

			public bool IsValid(object value) => this.AcceptableValue.IsValid(value);
		}

		internal class Entry<TValue>(
			string name,
			string description,
			TValue defaultValue
		) where TValue : IComparable {
			public string Name { get; } = name;

			public string Description { get; } = description;

			public TValue Default { get; } = defaultValue;
		}

		internal class ConstrainedEntry<TValue, TConstraint>(
			string name,
			string description,
			TValue defaultValue,
			TConstraint constraint
		): Entry<TValue>(name, description, defaultValue) where TValue : IComparable where TConstraint : IEntryConstraint<TValue> {
			public IEntryConstraint<TValue> Constraint { get; } = constraint;
		}

		public static readonly Entry<bool> Enabled = new(
			name: "Enabled",
			description: "Enables the mod.",
			defaultValue: true);

		public static readonly ConstrainedEntry<int, RangeEntryConstraint<int>> Rewards = new(
			name: "Total Rewards",
			description: "Number of prayers that constitute a reward.\n=== You offer to the shrine and are rewarded! ===",
			defaultValue: 2,
			constraint: new(min: 1, max: 20));

		public static readonly ConstrainedEntry<int, RangeEntryConstraint<int>> Failures = new(
			name: "Max. Failures",
			description: "Number of prayers that will yield nothing.\n=== You offer to the shrine, but gain nothing. ===",
			defaultValue: 6,
			constraint: new(min: 0, max: 20));
	}
}
