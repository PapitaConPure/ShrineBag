using BepInEx.Configuration;
using System;

namespace ConfigEntryExt {
	public interface IEntryConstraint<TValue> where TValue : IComparable {
		public AcceptableValueBase AcceptableValue { get; }

		public TValue Clamp(TValue value);
		public bool IsValid(object value);
	}
}
