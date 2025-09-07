namespace TruthAPI.Skills;

public class MurderInfo
{
    /// <summary>
    ///     目标是否可以被击杀，由于目标导致的无法击杀将该值赋值为 false
    /// </summary>
    public bool CanKill = true;

    /// <summary>
    ///     击杀者是否真的会进行击杀，由于击杀者导致的无法击杀将该值赋值为 false
    /// </summary>
    public bool DoKill = true;

    /// <summary>
    ///     是否发生从梯子上摔死等意外
    /// </summary>
    public bool IsAccident = false;

    public MurderInfo(PlayerControl attemptKiller, PlayerControl attemptTarget, PlayerControl appearanceKiller,
        PlayerControl appearancetarget)
    {
        AttemptKiller = attemptKiller;
        AttemptTarget = attemptTarget;
        AppearanceKiller = appearanceKiller;
        AppearanceTarget = appearancetarget;
    }

    /// <summary>实际击杀者，不变</summary>
    public PlayerControl AttemptKiller { get; }

    /// <summary>实际被击杀的玩家，不变</summary>
    public PlayerControl AttemptTarget { get; }

    /// <summary>视觉上的击杀者，可变</summary>
    public PlayerControl AppearanceKiller { get; set; }

    /// <summary>视觉上被击杀的玩家，可变</summary>
    public PlayerControl AppearanceTarget { get; set; }

    // 使用 (killer, target) = info.AttemptTuple; 即可获得数据
    public (PlayerControl killer, PlayerControl target) AttemptTuple => (AttemptKiller, AttemptTarget);
    public (PlayerControl killer, PlayerControl target) AppearanceTuple => (AppearanceKiller, AppearanceTarget);

    /// <summary>
    ///     真的是自杀
    /// </summary>
    public bool IsSuicide => AttemptKiller.PlayerId == AttemptTarget.PlayerId;
}