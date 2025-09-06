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
        public const string Id = "xtreme.wave.truthapi";
        public const string VersionString = "1.0.0";

        public Harmony Harmony { get; } = new Harmony(Id);

        public static Version Version = Version.Parse(VersionString);

        public static readonly Random Random = new Random();

        public static ConfigFile ConfigFile { get; private set; }

        public static ManualLogSource Logger { get; private set; }

        public static bool Logging
        {
            get
            {
                if (ConfigFile == null)
                    return true;
                return ConfigFile.Bind("Settings", "Logging", true).Value;
            }
        }

        public static bool GameStarted
        {
            get
            {
                return GameData.Instance && ShipStatus.Instance && AmongUsClient.Instance &&
                       (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started ||
                        AmongUsClient.Instance.NetworkMode == global::NetworkModes.FreePlay);
            }
        }

        /// <summary>
        /// If you set this to false please provide credit! I mean this stuff is free and open-source so a little credit would be nice :)
        /// </summary>
        public static bool ShamelessPlug = true;

        public static CustomToggleOption ShowRolesOfDead;

        public override void Load()
        {
            Logger = this.Log;
            ConfigFile = Config;

            var useCustomServer = ConfigFile.Bind("CustomServer", "UseCustomServer", false);
            if (useCustomServer.Value)
            {
                CustomServerManager.RegisterServer(ConfigFile.Bind("CustomServer", "Name", "CustomServer").Value,
                    ConfigFile.Bind("CustomServer", "Ipv4 or Hostname", "au.peasplayer.tk").Value,
                    ConfigFile.Bind("CustomServer", "Port", (ushort)22023).Value);
            }

            UpdateManager.RegisterGitHubUpdateListener("fangkuaiclub", "TruthAPI-R");

            RegisterCustomRoleAttribute.Load();
            RegisterCustomGameModeAttribute.Load();

            new CustomHeaderOption(MultiMenu.Main, "General Settings");
            ShowRolesOfDead =
                new CustomToggleOption(MultiMenu.Main, "Show the roles of dead player", false);
            GameModeManager.GameModeOption = new CustomStringOption(MultiMenu.Main, "GameMode", new string[] { "None" });

            Harmony.PatchAll();
        }

        [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
        [HarmonyPrefix]
        public static void PatchToTestSomeStuff(KeyboardJoystick __instance)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
            }
        }
    }
}