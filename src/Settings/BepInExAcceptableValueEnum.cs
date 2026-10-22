using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShrineBag.Settings {
	//why the fuck is AcceptableValueList constrained to IEquatable<T> im going to ggghghtrhrgAERHAWWWGAERATHRYSHGJN
	internal sealed class AcceptableValueEnum<TValue>: AcceptableValueBase where TValue : IComparable {
		public IReadOnlyList<TValue> AcceptableValues { get; }

		public AcceptableValueEnum(params TValue[] acceptableValues) : base(typeof(TValue)) {
			if(acceptableValues == null)
				throw new ArgumentNullException(nameof(acceptableValues));

			if(acceptableValues.Length == 0)
				throw new ArgumentException("At least one acceptable value is needed.", nameof(acceptableValues));

			this.AcceptableValues = acceptableValues;
		}

		public override object Clamp(object value)
			=> this.IsValid(value) ? value : this.AcceptableValues[0];

		public override bool IsValid(object value)
			=> value is TValue v && this.AcceptableValues.Contains(v);

		public override string ToDescriptionString()
			=> "# Acceptable values: " + string.Join(", ", [.. this.AcceptableValues.Select((TValue x) => x.ToString())]);
	}
}
