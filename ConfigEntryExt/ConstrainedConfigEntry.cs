using BepInEx;
using BepInEx.Configuration;
using System;

namespace ConfigEntryExt {
	public sealed class ConstrainedConfigEntry<TValue, TConstraint>(
		string section,
		string name,
		string description,
		TValue defaultValue,
		TConstraint constraint
	): ConfigEntryExt<TValue>(section, name, description, defaultValue)
		where TValue : IComparable
		where TConstraint : IEntryConstraint<TValue> {
		public TConstraint Constraint { get; } = constraint;

		public override ConfigEntry<TValue> RegisterFor(BaseUnityPlugin plugin)
			=> plugin.Config.Bind(
				this.Section, this.Name, this.Default,
				new ConfigDescription(this.Description, this.Constraint.AcceptableValue));
	}
}
