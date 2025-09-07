using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using InnerNet;
using TruthAPI.Components;
using TruthAPI.GameModes;
using TruthAPI.Managers;
using TruthAPI.Options;
using UnityEngine;
using Random = System.Random;

namespace TruthAPI
{
    [HarmonyPatch]
    [BepInPlugin(Id, "TruthAPI", VersionString)]
    [BepInProcess("Among Us.exe")]
    public class TruthAPI : BasePlugin
    {
        public const string ModName_EN = "TruthAPI";
        public const string Id = "xtreme.wave.truthapi";
        public const string VersionString = "1.0.0";

        public Harmony Harmony { get; } = new Harmony(Id);

        public static Version Version = Version.Parse(VersionString);

        public static readonly Random Random = new Random();
        public static IEnumerable<PlayerControl> AllPlayerControls =>
    PlayerControl.AllPlayerControls.ToArray().Where(p => p);
        public static ConfigFile ConfigFile { get; private set; }
        public static ManualLogSource Logger;

        public override void Load()
        {
            ConfigFile = Config;

            RegisterCustomRoleAttribute.Load();
            RegisterCustomGameModeAttribute.Load();

            new CustomHeaderOption(MultiMenu.Main, "General Settings");

            Harmony.PatchAll();
        }
    }
}