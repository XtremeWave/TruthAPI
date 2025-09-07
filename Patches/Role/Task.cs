using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruthAPI.Roles;

namespace TruthAPI.Patches.Role
{
    public static class Task
    {
        [HarmonyPatch(typeof(PlayerControl._CoSetTasks_d__103), nameof(PlayerControl._CoSetTasks_d__103.MoveNext))]
        public static class PlayerControlSetTasks
        {
            public static void Postfix(PlayerControl._CoSetTasks_d__103 __instance)
            {
                if (__instance == null)
                    return;

                var player = __instance.__4__this;
                var role = player.GetCustomRole();

                if (role == null)
                    return;

                if (player.PlayerId != PlayerControl.LocalPlayer.PlayerId)
                    return;

                if (!role.AssignTasks)
                    player.ClearTasks();

                if (role.TaskText == null)
                    return;

                if (!player.Data.Role.IsImpostor && !role.HasToDoTasks && role.AssignTasks)
                {
                    var fakeTasks = new GameObject("FakeTasks").AddComponent<ImportantTextTask>();
                    fakeTasks.transform.SetParent(player.transform, false);
                    fakeTasks.Text = $"</color>{role.Color.GetTextColor()}Fake Tasks:</color>";
                    player.myTasks.Insert(0, fakeTasks);
                }

                var roleTask = new GameObject(role.Name + "Task").AddComponent<ImportantTextTask>();
                roleTask.transform.SetParent(player.transform, false);
                roleTask.Text = $"</color>Role: {role.Color.GetTextColor()}{role.Name}\n{role.TaskText}</color>";
                player.myTasks.Insert(0, roleTask);
            }
        }
        [HarmonyPatch(typeof(GameData), nameof(GameData.RecomputeTaskCounts))]
        [HarmonyPrefix]
        private static bool DoTasksCountPatch(GameData __instance)
        {

            __instance.TotalTasks = 0;
            __instance.CompletedTasks = 0;
            foreach (var playerInfo in __instance.AllPlayers)
            {
                if (!playerInfo.Disconnected && playerInfo.Tasks != null && playerInfo.Object && (GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks || !playerInfo.IsDead) && playerInfo.Role && playerInfo.Role.TasksCountTowardProgress && (playerInfo.GetCustomRole() == null || playerInfo.GetCustomRole().HasToDoTasks))
                {
                    foreach (var task in playerInfo.Tasks)
                    {
                        __instance.TotalTasks++;
                        if (task.Complete)
                        {
                            __instance.CompletedTasks++;
                        }
                    }
                }
            }
            return false;
        }
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
        [HarmonyPrefix]
        private static void OnTaskCompletePatch(PlayerControl __instance, [HarmonyArgument(0)] uint idx)
        {
            PlayerTask playerTask = __instance.myTasks.ToArray().ToList().Find(p => p.Id == idx);
            ModRoleManager.Roles.Do(r => r.OnTaskComplete(__instance, playerTask));
        }
    }
}
