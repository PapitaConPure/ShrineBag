using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ShrineBag {
	internal static class ModIcon {
		public static Sprite Load() {
			using Stream stream = Assembly.GetExecutingAssembly()
				.GetManifestResourceStream("ShrineBag.icon.png")
				?? throw new InvalidOperationException("Embedded mod icon resource was not found.");

			using MemoryStream memoryStream = new();
			stream.CopyTo(memoryStream);

			Texture2D texture = new(2, 2, TextureFormat.RGBA32, false);

			if(!texture.LoadImage(memoryStream.ToArray(), markNonReadable: true)) {
				UnityEngine.Object.Destroy(texture);
				throw new InvalidOperationException("Failed to load mod icon.");
			}

			texture.name = "PapitaPure_ShrineBagIcon";

			return Sprite.Create(
				texture,
				new Rect(0, 0, texture.width, texture.height),
				new Vector2(0.5f, 0.5f));
		}
	}
}
