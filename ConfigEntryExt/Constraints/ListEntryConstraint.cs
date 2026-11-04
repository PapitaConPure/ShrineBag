using BepInEx.Configuration;
using System;
using ConfigEntryExt.Patches;

namespace ConfigEntryExt.Constraints {
	public sealed record ListEntryConstraint<TValue>(Type ValuesEnum): IEntryConstraint<TValue> where TValue : struct, Enum {
		public AcceptableValueBase AcceptableValue { get; } = new AcceptableValueEnum<TValue>(Enum.GetValues(ValuesEnum) as TValue[]);

		public TValue Clamp(TValue value) => (TValue)this.AcceptableValue.Clamp(value);

		public bool IsValid(object value) => this.AcceptableValue.IsValid(value);
	}
}
