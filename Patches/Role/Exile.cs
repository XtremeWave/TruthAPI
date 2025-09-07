using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruthAPI.Roles;

namespace TruthAPI.Patches.Role
{
    public static class Exile
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Exiled))]
        [HarmonyPostfix]
        public static void OnPlayerExiledPatch(PlayerControl __instance)
        {
            ModRoleManager.Roles.Do(r => r.OnExiled(__instance));
        }

        [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
        [HarmonyPostfix]
        public static void ChangeExileTextPatch(ExileController __instance, [HarmonyArgument(0)] NetworkedPlayerInfo player, [HarmonyArgument(1)] bool voteTie)
        {
            if (voteTie || player == null)
                return;

            var role = player.Object.GetCustomRole();
            if (role != null)
            {
                var article = role.Members.Count > 1 ? "其中之一的" : "";
                __instance.completeString = $"{ExileController.Instance.initData.networkedPlayer.PlayerName} was {article} {role.Name}.";
            }
        }
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Exiled))]
        [HarmonyPrefix]
        private static bool PreExiledPatch(PlayerControl __instance)
        {
            return ModRoleManager.Roles.Count(r => r.Members.Count != 0 && !r.PreExile(__instance)) == 0;
        }
    }
}
