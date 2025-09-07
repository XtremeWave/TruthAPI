using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruthAPI.Roles;

namespace TruthAPI.Patches.Role
{
    public static class Hud
    {
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class HudManagerUpdatePatch
        {
            public static void Prefix(HudManager __instance)
            {
                if (XtremeGameData.XtremeGameData.GameStates.GameStarted)
                {
                    ModRoleManager.Roles.Do(r => r._OnUpdate());
                }
            }
        }
    }
}
