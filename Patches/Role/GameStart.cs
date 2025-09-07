using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rewired.Utils.Platforms.Windows;
using TruthAPI.Roles;

namespace TruthAPI.Patches.Role
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CoStartGame))]
    internal class GameStart
    {
        public static void Postfix(AmongUsClient __instance)
        {
            TruthAPI.AllPlayerKillCooldown = new();

            ModRoleManager.Roles.Do(r => r.OnGameStart());
        }
    }
}
