using BepInEx.Configuration;
using System;

namespace ShrineBag.Settings {
	internal static partial class ModSettings {
		internal class Entry<TValue>(
			string section,
			string name,
			string description,
			TValue defaultValue)
			where TValue : IComparable {
			public string Section { get; } = section;

			public string Name { get; } = name;

			public string Description { get; } = description;

			public TValue Default { get; } = defaultValue;

			public virtual ConfigEntry<TValue> RegisterFor(Plugin plugin)
				=> plugin.Config.Bind(
					this.Section, this.Name, this.Default,
					new ConfigDescription(this.Description, null));
		}

		internal class ConstrainedEntry<TValue, TConstraint>(
			string section,
			string name,
			string description,
			TValue defaultValue,
			TConstraint constraint
		): Entry<TValue>(section, name, description, defaultValue)
			where TValue : IComparable
			where TConstraint : IEntryConstraint<TValue> {
			public TConstraint Constraint { get; } = constraint;

			public override ConfigEntry<TValue> RegisterFor(Plugin plugin)
				=> plugin.Config.Bind(
					this.Section, this.Name, this.Default,
					new ConfigDescription(this.Description, this.Constraint.AcceptableValue));
		}

		internal interface IEntryConstraint<TValue> where TValue : IComparable {
			public AcceptableValueBase AcceptableValue { get; }

			public TValue Clamp(TValue value);
			public bool IsValid(object value);
		}

		internal sealed record RangeEntryConstraint<TValue>(TValue Min, TValue Max): IEntryConstraint<TValue> where TValue : IComparable {
			public AcceptableValueBase AcceptableValue { get; } = new AcceptableValueRange<TValue>(Min, Max);

			public TValue Clamp(TValue value) => (TValue)this.AcceptableValue.Clamp(value);

			public bool IsValid(object value) => this.AcceptableValue.IsValid(value);
		}

		internal sealed record ListEntryConstraint<TValue>(Type ValuesEnum): IEntryConstraint<TValue> where TValue : struct, Enum {
			public AcceptableValueBase AcceptableValue { get; } = new AcceptableValueEnum<TValue>(Enum.GetValues(ValuesEnum) as TValue[]);

			public TValue Clamp(TValue value) => (TValue)this.AcceptableValue.Clamp(value);

			public bool IsValid(object value) => this.AcceptableValue.IsValid(value);
		}

		private static string Paragraph(params string[] lines)
			=> string.Join("\n", lines);
	}
}
