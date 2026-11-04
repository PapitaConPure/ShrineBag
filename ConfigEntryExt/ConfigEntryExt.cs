using BepInEx;
using BepInEx.Configuration;
using System;

namespace ConfigEntryExt {
	public class ConfigEntryExt<TValue>(
		string section,
		string name,
		string description,
		TValue defaultValue)
		where TValue : IComparable {
		public string Section { get; } = section;

		public string Name { get; } = name;

		public string Description { get; } = description;

		public TValue Default { get; } = defaultValue;

		public virtual ConfigEntry<TValue> RegisterFor(BaseUnityPlugin plugin)
			=> plugin.Config.Bind(
				this.Section, this.Name, this.Default,
				new ConfigDescription(this.Description, null));
	}
}
