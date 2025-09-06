using System;
using AmongUs.GameOptions;
using FracturedTruth.DataHandling.Options;
using FracturedTruth.Helpers;
using FracturedTruth.Roles.Core;
using static FracturedTruth.Helpers.ColorHelper;
using static Rewired.Utils.Classes.Utility.ObjectInstanceTracker;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace FracturedTruth.Helpers;

public static class RoleHelper
{
    #region 主职业

    public static bool IsMainImpostor(this FracturedRoles role)
    {
        var roleId = (int)role;
        return roleId is >= 0 and < 500;
    }

    public static bool IsMainCrewmate(this FracturedRoles role)
    {
        var roleId = (int)role;
        return roleId is >= 500 and < 1000;
    }

    // 分不分邪恶中立和友善中立你们决定即可
    public static bool IsMainNeutral(this FracturedRoles role)
    {
        var roleId = (int)role;
        return roleId is >= 1000 and < 1500;
    }

    public static FracturedRoleTeamTypes GetMainRoleType(this FracturedRoles role)
    {
        if (role.IsMainImpostor()) return FracturedRoleTeamTypes.Impostor;
        if (role.IsMainCrewmate()) return FracturedRoleTeamTypes.Crewmate;
        if (role.IsMainNeutral()) return FracturedRoleTeamTypes.Neutral;
        return FracturedRoleTeamTypes.Invalid;
    }

    public static bool IsMainRole(this FracturedRoles role)
    {
        var roleId = (int)role;
        return roleId is >= 0 and < 1500;
    }

    #endregion

    #region 副职业

    public static bool IsSubCommon(this FracturedRoles role)
    {
        var roleId = (int)role;
        return roleId is >= 1500 and < 2000;
    }

    public static bool IsSubImpostor(this FracturedRoles role)
    {
        var roleId = (int)role;
        return roleId is >= 2000 and < 2500;
    }

    public static bool IsSubCrewmate(this FracturedRoles role)
    {
        var roleId = (int)role;
        return roleId is >= 2500 and < 3000;
    }

    public static bool IsSubNeutral(this FracturedRoles role)
    {
        var roleId = (int)role;
        return roleId is >= 3000 and < 3500;
    }

    public static FracturedRoleTeamTypes GetSubRoleType(this FracturedRoles role)
    {
        if (role.IsSubCommon()) return FracturedRoleTeamTypes.Common;
        if (role.IsSubImpostor()) return FracturedRoleTeamTypes.Impostor;
        if (role.IsSubCrewmate()) return FracturedRoleTeamTypes.Crewmate;
        if (role.IsSubNeutral()) return FracturedRoleTeamTypes.Neutral;
        return FracturedRoleTeamTypes.Invalid;
    }
    public static bool IsSubRole(this FracturedRoles role)
    {
        var roleId = (int)role;
        return roleId is >= 1500 and < 3500;
    }
    #endregion

    #region 附加效果

    public static bool IsModiCommon(this FracturedRoles role)
    {
        var roleId = (int)role;
        return roleId is >= 3500 and < 4000;
    }

    public static bool IsModiImpostor(this FracturedRoles role)
    {
        var roleId = (int)role;
        return roleId is >= 4000 and < 4500;
    }

    public static bool IsModiCrewmate(this FracturedRoles role)
    {
        var roleId = (int)role;
        return roleId is >= 4500 and < 5000;
    }

    public static bool IsModiNeutral(this FracturedRoles role)
    {
        var roleId = (int)role;
        return roleId is >= 5000 and < 5500;
    }

    public static FracturedRoleTeamTypes GetModiRoleType(this FracturedRoles role)
    {
        if (role.IsModiCommon()) return FracturedRoleTeamTypes.Common;
        if (role.IsModiImpostor()) return FracturedRoleTeamTypes.Impostor;
        if (role.IsModiCrewmate()) return FracturedRoleTeamTypes.Crewmate;
        if (role.IsModiNeutral()) return FracturedRoleTeamTypes.Neutral;
        return FracturedRoleTeamTypes.Invalid;
    }
    public static bool IsModiRole(this FracturedRoles role)
    {
        var roleId = (int)role;
        return roleId is >= 3500 and < 5500;
    }

    #endregion

    #region 对枚举本身的简单操作
    public static Dictionary<RoleTypes, string> VRoleColors;
    public static Dictionary<FracturedRoles, Color> FTRoleColors;
    public static void MemonyStorage()
    {
        FTRoleColors = new Dictionary<FracturedRoles, Color>
        {
            { FracturedRoles.Crewmate, Palette.CrewmateBlue},
            { FracturedRoles.Impostor, Palette.ImpostorRed},
            { FracturedRoles.Sheriff, new Color32(248, 205, 70, byte.MaxValue)},

        };
        VRoleColors = new Dictionary<RoleTypes, string>
        {
             { RoleTypes.CrewmateGhost,"#8CFFFF"},
             { RoleTypes.GuardianAngel, "#8CFFDB" },
             { RoleTypes.Crewmate, "#8CFFFF" },
             { RoleTypes.Scientist, "#8FF8C" },
             { RoleTypes.Engineer, "#A5A8FF" },
             { RoleTypes.Noisemaker, "#FFC08C" },
             { RoleTypes.Tracker, "#93FF8C" },
             { RoleTypes.ImpostorGhost, "#FF1919" },
             { RoleTypes.Impostor, "#FF1919" },
             { RoleTypes.Shapeshifter, "#FF819E" },
             { RoleTypes.Phantom, "#CA8AFF" }
        };
    }
    public static string GetName(FracturedRoles fracturedRoles)
    {
        return fracturedRoles.ToString();
    }
    public static string GetOptionName(FracturedRoles fracturedRoles)
    {
        return $"Role.{fracturedRoles.ToString()}";
    }
    public static Color GetColor(FracturedRoles fracturedRoles)
    {
        MemonyStorage();
        FTRoleColors.TryGetValue(fracturedRoles, out var color);
        return color;
    }
    #endregion

    #region 职业管理
    /// <summary>
    /// Gets a <see cref="PlayerControl"/> from it's id
    /// </summary>
    public static PlayerControl GetPlayer(this byte id)
    {
        return GameData.Instance.GetPlayerById(id).Object;
    }

    /// <summary>
    /// Gets a <see cref="NetworkedPlayerInfo"/> from it's id
    /// </summary>
    public static NetworkedPlayerInfo GetPlayerInfo(this byte id)
    {
        return GameData.Instance.GetPlayerById(id);
    }

    public static bool IsLocal(this PlayerControl player)
    {
        return player.PlayerId == PlayerControl.LocalPlayer.PlayerId;
    }

    public static void ToggleOutline(this PlayerControl player, bool active, Color color = default)
    {
        if (active)
        {
            player.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 1f);
            player.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", color);
            return;
        }

        player.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 0f);
    }

    public static Color SetAlpha(this Color original, float alpha)
    {
        return new Color(original.r, original.g, original.b, alpha);
    }

    public static Vector3 SetX(this Vector3 original, float x)
    {
        return new Vector3(x, original.y, original.z);
    }

    public static Vector3 SetY(this Vector3 original, float y)
    {
        return new Vector3(original.x, y, original.z);
    }

    public static Vector3 SetZ(this Vector3 original, float z)
    {
        return new Vector3(original.x, original.y, z);
    }

    /// <summary>
    /// Gets the text color from a <see cref="Color"/>
    /// </summary>
    public static string GetTextColor(this Color color)
    {
        var r = Mathf.RoundToInt(color.r * 255f).ToString("X2");

        var g = Mathf.RoundToInt(color.g * 255f).ToString("X2");

        var b = Mathf.RoundToInt(color.b * 255f).ToString("X2");

        var a = Mathf.RoundToInt(color.a * 255f).ToString("X2");

        return $"<color=#{r}{g}{b}{a}>";
    }

    public static string GetTranslation(this StringNames stringName)
    {
        return TranslationController.Instance.GetString(stringName);
    }

    #region Roles

    /// <summary>
    /// Gets the role of a <see cref="PlayerControl"/>
    /// </summary>
    public static BaseRole GetCustomRole(this PlayerControl player)
    {
        if (ModRoleManager.Roles == null) return null;

        foreach (var _role in ModRoleManager.Roles)
        {
            if (_role.Members.Contains(player.PlayerId))
                return _role;
        }

        return null;
    }

    public static bool IsCustomRole(this PlayerControl player)
    {
        if (player == null)
            return false;

        return GetCustomRole(player) != null;
    }

    /// <summary>
    /// Gets the role of a <see cref="NetworkedPlayerInfo"/>
    /// </summary>
    public static BaseRole GetCustomRole(this NetworkedPlayerInfo player)
    {
        foreach (var _role in ModRoleManager.Roles)
        {
            if (_role.Members.Contains(player.PlayerId))
                return _role;
        }

        return null;
    }

    /// <summary>
    /// Checks if a <see cref="PlayerControl"/> is a certain role
    /// </summary>
    public static bool IsCustomRole(this PlayerControl player, BaseRole role) => player.GetCustomRole() == role;

#nullable enable
    /// <summary>
    /// Gets the role of a <see cref="PlayerControl"/>
    /// </summary>
    public static T? GetCustomRole<T>(this PlayerControl player) where T : BaseRole
        => player.GetCustomRole() as T;

    /// <summary>
    /// Checks if a <see cref="PlayerControl"/> is a certain role
    /// </summary>
    public static bool IsCustomRole<T>(this PlayerControl player) where T : BaseRole
        => player.GetCustomRole<T>() != null;

    /// <summary>
    /// Sets the role of a <see cref="PlayerControl"/>
    /// </summary>
    public static void SetCustomRole(this PlayerControl player, BaseRole? role)
    {
        var oldRole = ModRoleManager.Roles.Where(r => r.Members.Contains(player.PlayerId)).ToList();
        if (oldRole.Count != 0)
            oldRole[0].Members.Remove(player.PlayerId);

        if (role != null)
        {
            role.Members.Add(player.PlayerId);
        }
        else if (player.IsLocal())
        {
            var isImpostor = player.Data.Role.IsImpostor;
            var isDead = player.Data.IsDead;

            if (oldRole.Count != 0)
                GameObject.Find(oldRole[0].Name + "Task").Destroy();
            HudManager.Instance.SabotageButton.gameObject.SetActive(isImpostor);
            HudManager.Instance.KillButton.gameObject.SetActive(isImpostor && !isDead);
            HudManager.Instance.ImpostorVentButton.gameObject.SetActive(isImpostor && !isDead);

            player.cosmetics.nameText.color = isImpostor ? Palette.ImpostorRed : Color.white;
            player.cosmetics.nameText.text = player.name;
        }
    }

    public static void SetVanillaRole(this PlayerControl player, RoleTypes role)
    {
        player.roleAssigned = true;
        if (RoleManager.IsGhostRole(role))
        {
            RoleManager.Instance.SetRole(player, role);
            player.Data.Role.SpawnTaskHeader(player);
            return;
        }
        HudManager.Instance.MapButton.gameObject.SetActive(true);
        HudManager.Instance.ReportButton.gameObject.SetActive(true);
        HudManager.Instance.UseButton.gameObject.SetActive(true);
        PlayerControl.LocalPlayer.RemainingEmergencies = GameOptionsManager.Instance.currentNormalGameOptions.NumEmergencyMeetings;
        RoleManager.Instance.SetRole(player, role);
        player.Data.Role.SpawnTaskHeader(player);
        if (!DestroyableSingleton<TutorialManager>.InstanceExists)
        {
            if (GetAllPlayers().All(pc => pc.roleAssigned))
            {
                GetAllPlayers().ForEach(pc =>
                {
                    if (pc.Data.Role.TeamType == PlayerControl.LocalPlayer.Data.Role.TeamType)
                    {
                        pc.cosmetics.nameText.color = pc.Data.Role.NameColor;
                    }
                    else
                    {
                        pc.cosmetics.nameText.color = Palette.White;
                    }
                });
            }
        }
    }

    /// <summary>
    /// Sets the role of a <see cref="PlayerControl"/>
    /// </summary>
    public static void RpcSetRole(this PlayerControl player, BaseRole? role)
    {
        //Rpc<RpcSetRole>.Instance.Send(new RpcSetRole.Data(player, role));

        player.SetCustomRole(role);
    }

    public static bool IsOnSameTeam(this PlayerControl player, PlayerControl otherPlayer)
    {
        var role = player.GetCustomRole();
        var otherRole = otherPlayer.GetCustomRole();

        if (role != null)
        {
            switch (role.Team)
            {
                case FracturedRoleTeamTypes.Invalid:
                    return false;
                case FracturedRoleTeamTypes.Role:
                    return role.Id == otherRole.Id;
                case FracturedRoleTeamTypes.Crewmate:
                    if (otherRole != null)
                        return otherRole.Team == FracturedRoleTeamTypes.Crewmate;
                    else
                        return !otherPlayer.Data.Role.IsImpostor;
                case FracturedRoleTeamTypes.Impostor:
                    if (otherRole != null)
                        return otherRole.Team == FracturedRoleTeamTypes.Impostor;
                    else
                        return otherPlayer.Data.Role.IsImpostor;
            }
        }
        else if (otherRole == null)
        {
            return player.Data.Role.IsImpostor == otherPlayer.Data.Role.IsImpostor;
        }
        else
        {
            switch (otherRole.Team)
            {
                case FracturedRoleTeamTypes.Invalid:
                    return false;
                case FracturedRoleTeamTypes.Role:
                    return false;
                case FracturedRoleTeamTypes.Crewmate:
                    return !otherPlayer.Data.Role.IsImpostor;
                case FracturedRoleTeamTypes.Impostor:
                    return otherPlayer.Data.Role.IsImpostor;
            }
        }

        return false;
    }

    public static RoleTypes GetSimpleRoleType(this RoleTypes role)
    {
        switch (role)
        {
            case RoleTypes.Engineer:
                return RoleTypes.Crewmate;
            case RoleTypes.Scientist:
                return RoleTypes.Crewmate;
            case RoleTypes.GuardianAngel:
                return RoleTypes.Crewmate;
            case RoleTypes.Shapeshifter:
                return RoleTypes.Impostor;
            case RoleTypes.Crewmate:
                return RoleTypes.Crewmate;
            case RoleTypes.Impostor:
                return RoleTypes.Impostor;
        }

        return RoleTypes.Crewmate;
    }
    public static List<PlayerControl> GetAllPlayers()
    {
        if (PlayerControl.AllPlayerControls != null && PlayerControl.AllPlayerControls.Count > 0)
            return PlayerControl.AllPlayerControls.ToArray().ToList();
        return GameData.Instance.AllPlayers.ToArray().ToList().ConvertAll(p => p.Object);
    }
    #endregion Roles

    #region Position
    public static Vector3 SetX(this Transform transform, float x)
    {
        var currentPosition = transform.position;
        var vector = new Vector3(x, currentPosition.y, currentPosition.z);
        transform.position = vector;
        return vector;
    }

    public static Vector3 SetY(this Transform transform, float y)
    {
        var currentPosition = transform.position;
        var vector = new Vector3(currentPosition.x, y, currentPosition.z);
        transform.position = vector;
        return vector;
    }

    public static Vector3 SetZ(this Transform transform, float z)
    {
        var currentPosition = transform.position;
        var vector = new Vector3(currentPosition.x, currentPosition.y, z);
        transform.position = vector;
        return vector;
    }
    #endregion
#endregion 
}
# region 这里面是一堆枚举
/// <summary>
/// 主职业
/// </summary>

public enum FracturedRoleTeamTypes
{
    Invalid = -1,
    Common,
    Crewmate,
    Impostor,
    Neutral,
    Role,
    MainRole,
    SubRole,
    Addon
}
public enum FracturedRoleMoreTeamTypes
{
    Invalid = -1,
    Common,
    Crewmate,
    Impostor,
    Neutral,
    MainRole,
    SubRole,
    Addon,
    Jester,
}
public enum FracturedRoles
{
    NotAssigned = -1,

    // 伪装者 - 主职业
    Impostor = 0,
    ImpostorGhost,
    Shapeshifter,
    Phantom,
    //模组职业

    // 船员 - 主职业
    Crewmate = 500,
    CrewmateGhost,
    GuardianAngel,
    Engineer,
    Scientist,
    Tracker,
    Noisemaker,
    //模组职业
    Sheriff,


    // 中立 - 主职业
    Jackal = 1000,
    Jester,

    // 共生 - 副职业
    Guesser = 1500,

    // 伪装者 - 副职业
    Mastermind = 2000, //幕后主使

    // 船员 - 副职业
    // 暂时没有 = 2500,

    // 中立 - 副职业
    Drillmaster = 3000, // 演习者

    // 共生 - 附加职业
    NeverExisted = 3500, // 从未存在

    // 伪装者 - 附加职业
    DeathArtist = 4000, // 死亡艺术家

    // 船员 - 附加职业
    // 暂时没有 = 4500,

    // 中立 - 附加职业
    Unifier = 5000, // 联合者
}
#endregion 