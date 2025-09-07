using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruthAPI.Roles;

namespace TruthAPI.Patches.Role
{
    public static class Kill
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        public static class PlayerControlMurderPlayerPatch
        {
            public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
            {
                ModRoleManager.Roles.Do(r => r.OnKill(__instance, target));
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetKillTimer))]
        public static class PlayerControlSetKillTimerPatch
        {
            public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] float time)
            {
                if (__instance.GetCustomRole() != null && __instance.GetCustomRole().CanKill() || __instance.Data.Role.CanUseKillButton)
                {
                    if (GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown <= 0f)
                        return false;
                    __instance.killTimer = Mathf.Clamp(time, 0f, GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
                    DestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(__instance.killTimer, GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
        public static class KillButtonManagerSetTargetPatch
        {
            public static bool Prefix(KillButton __instance, [HarmonyArgument(0)] PlayerControl target)
            {
                if (!PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.Data.Role)
                    return false;
                RoleTeamTypes teamType = PlayerControl.LocalPlayer.GetCustomRole() == null ? PlayerControl.LocalPlayer.Data.Role.TeamType : PlayerControl.LocalPlayer.GetCustomRole().CanKill() ? RoleTeamTypes.Impostor : RoleTeamTypes.Crewmate;
                if (__instance.currentTarget && __instance.currentTarget != target)
                {
                    __instance.currentTarget.ToggleHighlight(false, teamType);
                }
                __instance.currentTarget = target;
                if (__instance.currentTarget)
                {
                    __instance.currentTarget.ToggleHighlight(true, teamType);
                    __instance.SetEnabled();
                    return false;
                }
                __instance.SetDisabled();
                return false;
            }
        }

        [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
        [HarmonyPrefix]
        public static bool RemoveCheckMurder(KillButton __instance)
        {
            var target = __instance.currentTarget;
            var killer = PlayerControl.LocalPlayer;
            if (__instance.isActiveAndEnabled && target && !__instance.isCoolingDown && !killer.Data.IsDead && killer.CanMove)
            {
                if (AmongUsClient.Instance.IsGameOver)
                {
                    return false;
                }
                if (!target || killer.Data.IsDead || killer.Data.Disconnected)
                {
                    int num = target ? target.PlayerId : -1;
                    Debug.LogWarning(string.Format("Bad kill from {0} to {1}", killer.PlayerId, num));
                    return false;
                }
                NetworkedPlayerInfo data = target.Data;
                if (data == null || data.IsDead || target.inVent)
                {
                    Debug.LogWarning("Invalid target data for kill");
                    return false;
                }
                PlayerControl.LocalPlayer.RpcMurderPlayer(__instance.currentTarget, true);
                __instance.SetTarget(null);
            }
            return false;
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        [HarmonyPrefix]
        private static bool PreKillPatch(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            return ModRoleManager.Roles.Count(r => r.Members.Count != 0 && !r.PreKill(__instance, target)) == 0;
        }
    }
}
