using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruthAPI.Patches.Role
{
    public static class Sabotage
    {
        [HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.DoClick))]
        public static class UseButtonManagerDoClickPatch
        {
            public static bool Prefix(SabotageButton __instance)
            {
                if (__instance.isActiveAndEnabled && XtremeGameData.XtremeGameData.GameStates.GameStarted)
                {
                    var role = PlayerControl.LocalPlayer.GetCustomRole();
                    if (role == null)
                        return true;

                    var mapOptions = new MapOptions
                    {
                        Mode = MapOptions.Modes.Sabotage
                    };

                    MapBehaviour.Instance.Show(mapOptions);

                    foreach (MapRoom mapRoom in MapBehaviour.Instance.infectedOverlay.rooms.ToArray())
                    {
                        mapRoom.gameObject.SetActive(role.CanSabotage(mapRoom.room));
                    }

                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowSabotageMap))]
        public static class MapBehaviourShowSabotageMapPatch
        {
            public static bool Prefix(MapBehaviour __instance)
            {
                if (XtremeGameData.XtremeGameData.GameStates.GameStarted)
                {
                    var role = PlayerControl.LocalPlayer.GetCustomRole();

                    if (role == null)
                        return true;

                    foreach (MapRoom mapRoom in __instance.infectedOverlay.rooms.ToArray())
                    {
                        mapRoom.gameObject.SetActive(role.CanSabotage(mapRoom.room));
                    }
                }

                return true;
            }
        }
    }
}
