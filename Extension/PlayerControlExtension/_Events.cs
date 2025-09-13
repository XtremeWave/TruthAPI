namespace TruthAPI.Extension.PlayerControlExtension;

public static class _Events
{
    public static void OnCompleteTask(this PlayerControl pc)
    {
        pc.GetXtremeData().UpdateProcess();
    }
}