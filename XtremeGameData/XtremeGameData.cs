using TruthAPI.Components;
using UnityEngine;

namespace TruthAPI.XtremeGameData;

public static partial class XtremeGameData
{
    public static string LastResultText;
    public static string LastRoomCode;
    public static string LastServer;
    public static string LastGameData;
    public static string LastGameResult;
    public static Color LastLocalPlayerRoleColor;
    public static bool JoinedCompleted;

    [GameModuleInitializer]
    public static void Init()
    {
        LastResultText = LastGameData = LastGameResult = LastRoomCode = LastServer = "";
    }
}

public enum AllDeathReason
{
    None,
    Exile,
    Kill,
    Disconnect
}