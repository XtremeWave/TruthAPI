using TruthAPI.Options;
using Hazel;

namespace TruthAPI.CustomRpc;
//¶Ô¸¶ÓÃµÄRPC
public static class RpcUpdate
    {
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    public static class HandleRpc
    {
        private static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
        {
            switch (callId)
            {
                case (byte)CustomRpcCalls.UpdateSetting:
                    //OptionRpcs.ReceiveOptionRpc(reader, reader.BytesRemaining > 8);
                    break;
            }
        }
    }
}
