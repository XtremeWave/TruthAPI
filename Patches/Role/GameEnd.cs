using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruthAPI.Roles;

namespace TruthAPI.Patches.Role
{
    public static class GameEnd
    {
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
        [HarmonyPrefix]
        public static void OnGameEndPatch(AmongUsClient __instance)
        {
            ModRoleManager.Roles.Do(r => r.OnGameStop());
        }
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.RpcEndGame))]
        [HarmonyPrefix]
        private static bool ShouldGameEndPatch(GameManager __instance, [HarmonyArgument(0)] GameOverReason endReason)
        {
            return ModRoleManager.Roles.Count(r => r.Members.Count != 0 && !r.ShouldGameEnd(endReason)) == 0;
        }
    }
}
