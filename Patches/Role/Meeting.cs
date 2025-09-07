using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruthAPI.Roles;

namespace TruthAPI.Patches.Role
{
    public static class MeetingStart
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
        [HarmonyPrefix]
        public static void OnMeetingStart(MeetingHud __instance)
        {
            ModRoleManager.Roles.Do(r => r.OnMeetingStart(__instance));
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        public static class MeetingUpdatePatch
        {
            public static void Postfix(MeetingHud __instance)
            {
                ModRoleManager.Roles.Do(r => r._OnMeetingUpdate(__instance));
            }
        }
    }
}
