using System;
using AmongUs.GameOptions;
using InnerNet;

namespace TruthAPI.XtremeGameData;

public partial class APIXtremeGameData
{
    public static class GameStates
    {
        private static bool InGame { get; set; }
        private static bool InMeeting { get; set; }
        public static bool GameStarted
        {
            get
            {
                return GameData.Instance && ShipStatus.Instance && AmongUsClient.Instance &&
                       (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started ||
                        AmongUsClient.Instance.NetworkMode == global::NetworkModes.FreePlay);
            }
        }

        public static bool IsLobby =>
            AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Joined && !IsFreePlay;

        public static bool IsInGame => InGame || IsFreePlay;
        public static bool IsNotJoined => AmongUsClient.Instance.GameState == InnerNetClient.GameStates.NotJoined;
        public static bool IsOnlineGame => AmongUsClient.Instance.NetworkMode == NetworkModes.OnlineGame;
        public static bool IsLocalGame => AmongUsClient.Instance.NetworkMode == NetworkModes.LocalGame;
        public static bool IsFreePlay => AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay;
        public static bool IsInTask => IsInGame && !MeetingHud.Instance;
        public static bool IsInMeeting => IsInGame && MeetingHud.Instance && InMeeting;

        public static bool IsCountDown => GameStartManager.InstanceExists &&
                                          GameStartManager.Instance.startState ==
                                          GameStartManager.StartingStates.Countdown;

        public static bool IsShip => ShipStatus.Instance;
        public static bool IsShhh => HudManager.Instance.shhhEmblem;
        public static bool IsCanMove => PlayerControl.LocalPlayer?.CanMove is true;
        public static bool IsDead => PlayerControl.LocalPlayer?.Data?.IsDead is true && !IsLobby;

        public static bool IsNormalGame =>
            GameOptionsManager.Instance.CurrentGameOptions.GameMode is AmongUs.GameOptions.GameModes.Normal or AmongUs.GameOptions.GameModes.NormalFools;

        public static bool IsHideNSeek =>
            GameOptionsManager.Instance.CurrentGameOptions.GameMode is AmongUs.GameOptions.GameModes.HideNSeek or AmongUs.GameOptions.GameModes.SeekFools;

        public static bool IsVanillaServer
        {
            get
            {
                if (!IsOnlineGame) return false;
                const string Domain = "among.us";
                //Reactor.gg
                return ServerManager.Instance.CurrentRegion?.TryCast<StaticHttpRegionInfo>() is { } regionInfo &&
                       regionInfo.PingServer.EndsWith(Domain, StringComparison.Ordinal) &&
                       regionInfo.Servers.All(serverInfo => serverInfo.Ip.EndsWith(Domain, StringComparison.Ordinal));
            }
        }

        public static void UpdateGameState_IsInGame(bool inGame)
        {
            InGame = inGame;
        }

        public static void UpdateGameState_IsInMeeting(bool inMeeting)
        {
            InMeeting = inMeeting;
        }

        public static bool MapIsActive(MapNames name)
        {
            return (MapNames)GameOptionsManager.Instance.CurrentGameOptions.MapId == name;
        }
    }
}