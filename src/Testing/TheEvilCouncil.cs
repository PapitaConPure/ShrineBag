using RoR2;
using ShrineBag.Settings;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace ShrineBag.Testing {
	//We don't use the Director for tihs
	internal static class TheEvilCouncil {
		private const float COUNCIL_MIN_DISTANCE = 5f;
		private const float COUNCIL_MAX_DISTANCE = 15f;

		private static bool initialized;
		private static GameObject shrinePrefab;

		public static bool AlreadySpawned { get; private set; }

		public static void Prepare() {
			AlreadySpawned = false;
		}

		public static void BeEvil(int configuredCouncilSize) {
			int councilSize = ModSettings.CouncilSize.Constraint.Clamp(configuredCouncilSize);
			float councilRadius = COUNCIL_MIN_DISTANCE + councilSize * (COUNCIL_MAX_DISTANCE - COUNCIL_MIN_DISTANCE) / ModSettings.CouncilSize.Constraint.Max;

			if(!initialized)
				Initialize();

			CharacterBody playerBody = PlayerCharacterMasterController.instances.FirstOrDefault()?.body
				?? throw new NullReferenceException("Failed to find a PlayerBody during 'The Council' spawn procedure.");

			for(int i = 0; i < councilSize; i++) {
				Vector3 pos = GetCouncilMemberPosition(playerBody.corePosition, i, councilSize, councilRadius);
				Quaternion rot = GetCouncilMemberRotation(playerBody.corePosition, pos);
				GameObject shrineInst = UnityEngine.Object.Instantiate(shrinePrefab, pos, rot);
				NetworkServer.Spawn(shrineInst);
			}

			AlreadySpawned = true;
		}

		private static Vector3 GetCouncilMemberPosition(Vector3 center, int index, int total, float radius) {
			float angle = index * (360f / total) * Mathf.Deg2Rad;
			float x = Mathf.Cos(angle) * radius;
			float z = Mathf.Sin(angle) * radius; //why the fuck is it Z

			return center + new Vector3(x, 0f, z);
		}

		private static Quaternion GetCouncilMemberRotation(Vector3 center, Vector3 memberPos) {
			Vector3 direction = center - memberPos;
			return Quaternion.LookRotation(direction);
		}

		private static void Initialize() {
			shrinePrefab = Addressables
				.LoadAssetAsync<GameObject>("RoR2/Base/ShrineChance/ShrineChance.prefab")
				.WaitForCompletion() ?? throw new InvalidOperationException(
					"Failed to load Shrine of Chance prefab."
				);

			initialized = true;
		}
	}
}
