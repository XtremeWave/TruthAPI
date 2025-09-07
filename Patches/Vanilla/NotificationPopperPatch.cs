using TruthAPI.CustomRpc;

namespace TruthAPI.Patches.Vanilla;

[HarmonyPatch(typeof(NotificationPopper), nameof(NotificationPopper.AddDisconnectMessage))]
public class NotificationPopperPatch
{
    private static readonly List<string> WaitToSend = [];

    private static void AddItem(string text)
    {
        WaitToSend.Add(text);
        if (DestroyableSingleton<HudManager>._instance)
            DestroyableSingleton<HudManager>.Instance.Notifier.AddDisconnectMessage(text);
        else WaitToSend.Remove(text);
    }

    public static void NotificationPop(string text)
    {
        AddItem(text);
    }
}