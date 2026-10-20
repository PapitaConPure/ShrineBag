using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using System;


#if RISK_OF_OPTIONS
using RiskOfOptions;
using RiskOfOptions.Options;
using RiskOfOptions.OptionConfigs;
#endif

namespace ShrineBag {
	internal static class ModSettingsManagerCompat {
		private static bool? enabled;

		public static bool Enabled {
			get {
				enabled ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
				return (bool)enabled;
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void SetModIcon(UnityEngine.Sprite sprite) {
#if RISK_OF_OPTIONS
			ModSettingsManager.SetModIcon(sprite);
#endif
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void SetModDescription(string desc) {
#if RISK_OF_OPTIONS
			ModSettingsManager.SetModDescription(desc);
#endif
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void AddCheckboxOption(ConfigEntry<bool> entry) {
#if RISK_OF_OPTIONS
			ModSettingsManager.AddOption(new CheckBoxOption(entry));
#endif
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void AddSliderOption<TValue>(
			ConfigEntry<TValue> entry,
			ModSettings.RangeEntryConstraint<TValue> range,
			string formatString = "{0}")
			where TValue: IComparable {
#if RISK_OF_OPTIONS
			if(entry is ConfigEntry<int> intEntry && range is ModSettings.RangeEntryConstraint<int> intRange) {
				ModSettingsManager.AddOption(new IntSliderOption(
					intEntry,
					new IntSliderConfig {
						min = intRange.Min,
						max = intRange.Max,
						formatString = formatString
					}));
				return;
			}

			if(entry is not ConfigEntry<float> floatEntry || range is not ModSettings.RangeEntryConstraint<float> floatRange)
				throw new ArgumentException("Expected an int or float entry/constraint for slider option.");

			ModSettingsManager.AddOption(new SliderOption(
				floatEntry,
				new SliderConfig {
					min = floatRange.Min,
					max = floatRange.Max,
					FormatString = formatString
				}));
#endif
		}
	}
}
