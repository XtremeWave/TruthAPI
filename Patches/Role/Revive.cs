using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruthAPI.Roles;

namespace TruthAPI.Patches.Role
{
    public static class Revive
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Revive))]
        [HarmonyPrefix]
        private static void OnRevivePatch(PlayerControl __instance)
        {
            player.SetKillCooldown();
            ModRoleManager.Roles.Do(r => r.OnRevive(__instance));
        }
    }
}
