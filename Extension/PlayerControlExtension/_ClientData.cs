using InnerNet;

namespace TruthAPI.Extension.PlayerControlExtension;

public static class _ClientData
{
    public static ClientData GetClient(this PlayerControl player)
    {
        try
        {
            var client = AmongUsClient.Instance.allClients
                .ToArray().FirstOrDefault(cd => cd.Character.PlayerId == player.PlayerId);
            return client;
        }
        catch
        {
            return null;
        }
    }

    public static int GetClientId(this PlayerControl player)
    {
        if (!player) return -1;
        var client = player.GetClient();
        return client?.Id ?? -1;
    }

    public static bool IsHost(this PlayerControl player)
    {
        try
        {
            return AmongUsClient.Instance.GetHost().Id == player.GetClient().Id;
        }
        catch
        {
            return false;
        }
    }
}