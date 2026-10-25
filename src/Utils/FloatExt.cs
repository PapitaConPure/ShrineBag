using System;

namespace ShrineBag.Utils {
	internal static class FloatExt {
		public static bool AboutEquals(this float self, float other, float tolerance = 0.000001f)
			=> self == other
			|| Math.Abs(self - other) <= tolerance;
	}
}
