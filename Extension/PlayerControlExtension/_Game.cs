using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using Hazel;
using Il2CppSystem.Data;
using InnerNet;
using Rewired.Utils.Platforms.Windows;

namespace TruthAPI.Extension.PlayerControlExtension
{
    public static class _Game
    {
        public static void SetKillCooldown(this PlayerControl player, float time = -1f)
        {
            if (player == null) return;
            if (!player.CanUseKillButton()) return;
            if (time >= 0f) TruthAPI.AllPlayerKillCooldown[player.PlayerId] = time * 2;
            else TruthAPI.AllPlayerKillCooldown[player.PlayerId] *= 2;
            player.RpcProtectedMurderPlayer();
            player.ResetKillCooldown();
        }
        public static bool CanUseKillButton(this PlayerControl pc)
        {
            if (pc.Data.IsDead) return false;
            return pc.GetCustomRole().CanKill();
        }
        public static void RpcProtectedMurderPlayer(this PlayerControl killer, PlayerControl target = null)
        {
            //killerが死んでいる場合は実行しない
            if (killer.Data.IsDead) return;

            if (target == null) target = killer;
            // Host
            if (killer.AmOwner)
            {
                killer.MurderPlayer(target, MurderResultFlags.FailedProtected);
            }
            // Other Clients
            if (killer.PlayerId != 0)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId, (byte)RpcCalls.MurderPlayer, SendOption.Reliable);
                writer.WriteNetObject(target);
                writer.Write((int)MurderResultFlags.FailedProtected);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
        public static void ResetKillCooldown(this PlayerControl player)
        {
            if (player.GetCustomRole().CanKill()) TruthAPI.AllPlayerKillCooldown[player.PlayerId] = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
        }
    }
}
