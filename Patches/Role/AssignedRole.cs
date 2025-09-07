using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruthAPI.Patches.Role
{
    public static class AssignedRole
    {
        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
        [HarmonyPrefix]
        public static void RoleTeamPatch(IntroCutscene __instance,
            [HarmonyArgument(0)] ref List<PlayerControl> yourTeam)
        {
            if (PlayerControl.LocalPlayer.GetCustomRole() != null)
            {
                var role = PlayerControl.LocalPlayer.GetCustomRole();
                if (role.Team == TeamTypes.Neutral)
                {
                    yourTeam = new List<PlayerControl>();
                    yourTeam.Add(PlayerControl.LocalPlayer);
                }
                //else if (role.Team == TeamTypes.Role)
                //{
                //    yourTeam = new List<PlayerControl>();
                //    yourTeam.Add(PlayerControl.LocalPlayer);
                //    foreach (var player in role.Members)
                //    {
                //        if (player != PlayerControl.LocalPlayer.PlayerId)
                //            yourTeam.Add(player.GetPlayer());
                //    }
                //}
                else if (role.Team == TeamTypes.Impostor)
                {
                    yourTeam = new List<PlayerControl>();
                    yourTeam.Add(PlayerControl.LocalPlayer);
                    foreach (var player in role.Members)
                    {
                        if (player != PlayerControl.LocalPlayer.PlayerId &&
                            player.GetPlayer().Data.Role.IsImpostor)
                            yourTeam.Add(player.GetPlayer());
                    }
                }
            }
        }
        [HarmonyPatch(typeof(IntroCutscene._ShowRole_d__41), nameof(IntroCutscene._ShowRole_d__41.MoveNext))]
        [HarmonyPostfix]
        public static void RoleTextPatch(IntroCutscene._ShowRole_d__41 __instance)
        {
            if (PlayerControl.LocalPlayer.GetCustomRole() != null)
            {
                var role = PlayerControl.LocalPlayer.GetCustomRole();
                var scene = __instance.__4__this;

                scene.RoleText.text = role.Name;
                scene.RoleBlurbText.text = role.Description;
                scene.RoleText.color = role.Color;
                scene.RoleBlurbText.color = role.Color;
            }
        }
        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
        [HarmonyPostfix]
        public static void TeamTextPatch(IntroCutscene __instance)
        {
            if (PlayerControl.LocalPlayer.GetCustomRole() != null)
            {
                var role = PlayerControl.LocalPlayer.GetCustomRole();
                var scene = __instance;

                scene.TeamTitle.text = role.Name;
                scene.ImpostorText.gameObject.SetActive(true);
                scene.ImpostorText.text = role.Description;
                scene.BackgroundBar.material.color = role.Color;
                scene.TeamTitle.color = role.Color;
            }
        }
    }
}
