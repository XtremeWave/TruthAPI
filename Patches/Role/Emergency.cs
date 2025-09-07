using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Rewired.Utils.Classes.Utility.ObjectInstanceTracker;
using TruthAPI.Modules;
using TruthAPI.Roles;

namespace TruthAPI.Patches.Role
{
    [HarmonyPatch(typeof(EmergencyMinigame), nameof(EmergencyMinigame.Update))]
    public class EmergencyMinigameUpdatePatch
    {
        public static void Postfix(EmergencyMinigame __instance)
        {
            BaseRole role = PlayerControl.LocalPlayer.GetCustomRole();
            var roleCanCallEmergency = true;

            if (role != null && !role.CanCallEmergency)
            {
                roleCanCallEmergency = false;
            }

            if (!roleCanCallEmergency && role != null)
            {
                __instance.StatusText.text = string.Format(GetString("CallEmergency.Text"), role.Name);
                __instance.NumberText.text = string.Empty;
                __instance.ClosedLid.gameObject.SetActive(true);
                __instance.OpenLid.gameObject.SetActive(false);
                __instance.ButtonActive = false;
                return;
            }
        }
    }
}
