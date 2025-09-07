using System.Linq;
using AmongUs.GameOptions;
using HarmonyLib;
using Rewired.UI.ControlMapper;
using TruthAPI.CustomButtons;
using TruthAPI.CustomRpc;
using UnityEngine;
using TruthAPI.Roles;

namespace TruthAPI.Patches.Role
{
    [HarmonyPatch]
    public static class Patches
    {
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ExitGame))]
        [HarmonyPrefix]
        public static void ResetRolePatch(AmongUsClient __instance)
        {
            PlayerControl.AllPlayerControls.ToArray().Where(player => player != null).Do(player => player.SetCustomRole(null));
        }

        [HarmonyPatch(typeof(global::RoleManager), nameof(global::RoleManager.SelectRoles))]
        [HarmonyPostfix]
        public static void InitializeRolesPatch()
        {
            Rpc<RpcInitializeRoles>.Instance.Send();
        }

        [HarmonyPatch(typeof(global::LogicRoleSelectionNormal), nameof(global::LogicRoleSelectionNormal.AssignRolesFromList))]
        [HarmonyPrefix]
        public static bool ChangeImpostors(global::LogicRoleSelectionNormal __instance,
            [HarmonyArgument(0)] List<NetworkedPlayerInfo> players, [HarmonyArgument(1)] int teamMax,
            [HarmonyArgument(2)] List<RoleTypes> roleList, [HarmonyArgument(3)] ref int rolesAssigned)
        {
            while (roleList.Count > 0 && players.Count > 0 && rolesAssigned < teamMax)
            {
                int index = HashRandom.FastNext(roleList.Count);
                RoleTypes roleType = roleList.ToArray()[index];
                roleList.RemoveAt(index);
                int index2 = global::RoleManager.IsImpostorRole(roleType) && ModRoleManager.HostMod.IsImpostor
                    ? 0
                    : HashRandom.FastNext(players.Count);
                players.ToArray()[index2].Object.RpcSetRole(roleType);
                players.RemoveAt(index2);
                rolesAssigned++;
            }

            return false;
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public static class PlayerControlFixedUpdatePatch
        {
            public static void Postfix(PlayerControl __instance)
            {
                if (XtremeGameData.XtremeGameData.GameStates.GameStarted)
                {
                    var localRole = PlayerControl.LocalPlayer.GetCustomRole();

                    if (localRole != null && __instance.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    {
                        HudManager.Instance.KillButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead &&
                                                                            localRole.CanKill(null) && CustomButton.HudActive);

                        if (localRole.CanKill(null) && __instance.CanMove && !__instance.Data.IsDead)
                        {
                            if (!__instance.Data.Role.IsImpostor)
                                __instance.SetKillTimer(__instance.killTimer - Time.fixedDeltaTime);
                            PlayerControl target = __instance.Data.Role.FindClosestTarget();
                            HudManager.Instance.KillButton.SetTarget(target);
                        }
                        else
                        {
                            HudManager.Instance.KillButton.SetTarget(null);
                            HudManager.Instance.KillButton.SetDisabled();
                        }

                        HudManager.Instance.SabotageButton.gameObject.SetActive(
                            !PlayerControl.LocalPlayer.Data.IsDead && localRole.CanSabotage(null) && CustomButton.HudActive);

                        if (localRole.CanSabotage(null) && __instance.CanMove && !__instance.Data.IsDead)
                        {
                            HudManager.Instance.SabotageButton.SetEnabled();
                        }
                        else
                        {
                            HudManager.Instance.SabotageButton.SetDisabled();
                        }

                        HudManager.Instance.ImpostorVentButton.gameObject.SetActive(
                            !PlayerControl.LocalPlayer.Data.IsDead && localRole.CanVent && CustomButton.HudActive);

                        if (localRole.CanVent && __instance.CanMove && !__instance.Data.IsDead)
                        {
                            HudManager.Instance.ImpostorVentButton.SetEnabled();
                        }
                        else
                        {
                            HudManager.Instance.ImpostorVentButton.SetDisabled();
                        }
                    }
                }
            }
        }










    }
}