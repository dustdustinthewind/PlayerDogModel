﻿using BepInEx;
using HarmonyLib;
using System.Reflection;
using LC_API;
using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System;
using System.Runtime.CompilerServices;

namespace PlayerDogModel
{
	[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
	[BepInDependency("LC_API")]
	[BepInProcess("Lethal Company.exe")]
	public class Plugin : BaseUnityPlugin
	{
		public static Harmony _harmony;

		private void Awake()
		{
			_harmony = new Harmony(PluginInfo.PLUGIN_GUID);
			_harmony.PatchAll();
			Logger.LogInfo($"{PluginInfo.PLUGIN_GUID} loaded");
		}

		[HarmonyPatch(typeof(PlayerControllerB))]
		internal class PlayerControllerBPatch
		{
			// SpawnPlayerAnimation is called locally when spawning.
			[HarmonyPatch("SpawnPlayerAnimation")]
			[HarmonyPostfix]
			public static void SpawnPlayerAnimationPatch(ref PlayerControllerB __instance)
			{
				// Find all the players and add the script to them if they don't have it yet.
				// This is done for every player every time a player spawns just to be sure.
				foreach (GameObject player in StartOfRound.Instance.allPlayerObjects)
				{
					if (!player.GetComponent<PlayerModelReplacer>())
					{
						player.gameObject.AddComponent<PlayerModelReplacer>();
					}
				}
			}
		}

		[HarmonyPatch(typeof(StartOfRound))]
		internal class StartOfRoundPatch
		{
			// SpawnPlayerAnimation is called locally when spawning.
			[HarmonyPatch("PositionSuitsOnRack")]
			[HarmonyPostfix]
			public static void PositionSuitsOnRackPatch(ref StartOfRound __instance)
			{
				PlayerModelSwitcher switcher = GameObject.FindObjectOfType<PlayerModelSwitcher>();
				if (!switcher)
				{
					GameObject suitHanger = GameObject.Find("NurbsPath.002");
					suitHanger.AddComponent<PlayerModelSwitcher>();
				}
			}
		}
	}
}