using BepInEx.Configuration;
using System;

namespace ConfigEntryExt.Constraints {
	public sealed record RangeEntryConstraint<TValue>(TValue Min, TValue Max): IEntryConstraint<TValue> where TValue : IComparable {
		public AcceptableValueBase AcceptableValue { get; } = new AcceptableValueRange<TValue>(Min, Max);

		public TValue Clamp(TValue value) => (TValue)this.AcceptableValue.Clamp(value);

		public bool IsValid(object value) => this.AcceptableValue.IsValid(value);
	}
}
