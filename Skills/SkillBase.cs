using System;
using TruthAPI.XtremeGameData;
using UnityEngine;

namespace TruthAPI.Skills;

public abstract class SkillBase : IDisposable
{
    public virtual void Dispose()
    {
    }

    #region 保护相关处理

    /// <summary>
    ///     保护别人时调用的函数
    ///     不需要验证您的身份，因为调用前已经验证
    ///     请注意：全部模组端都会调用
    /// </summary>
    /// <param name="target">守护目标</param>
    public virtual bool OnProtectPlayer(PlayerControl target)
    {
        return true;
    }

    #endregion

    #region 任务相关处理

    /// <summary>
    ///     每次任务完成时调用的函数
    ///     设置 cancel=true 后返回 true 来取消原版的任务完成处理
    /// </summary>
    /// <returns>true：确认覆盖</returns>
    public virtual bool OnCompleteTask(out bool cancel)
    {
        cancel = false;
        return false;
    }

    #endregion

    protected enum GeneralOption
    {
        Cooldown,
        KillCooldown,
        CanVent,
        CanKill,
        VentCooldown,
        ImpostorVision,
        CanUseSabotage,
        CanCreateMadmate,
        CanKillAllies,
        CanKillSelf,
        ShapeshiftDuration,
        ShapeshiftCooldown,
        SkillDuration,
        SkillCooldown,
        SkillLimit
    }

    #region 倒计时相关处理

    /// <summary>
    ///     总计时列表
    /// </summary>
    public List<long> CooldownList { get; } = [];

    /// <summary>
    ///     倒计时列表
    /// </summary>
    public List<long> CountdownList { get; } = [];

    public void CreateCountdown(float cd, long now = -1)
    {
        CooldownList.Add((long)cd);
        CountdownList.Add(now);
    }

    /// <summary>
    ///     使用宠物总冷却时间
    /// </summary>
    public virtual long UsePetCooldown { get; set; } = -1;

    /// <summary>
    ///     使用宠物冷却时间倒计时
    /// </summary>
    public virtual long UsePetCooldown_Timer { get; set; } = GetTimeStamp();

    /// <summary>
    ///     摸宠物倒计时未设置
    /// </summary>
    public virtual bool PetUnSet()
    {
        return UsePetCooldown_Timer == -1;
    }

    /// <summary>
    ///     每帧倒计时更新
    /// </summary>
    public virtual void CD_Update()
    {
    }

    /// <summary>
    ///     倒计时未设置
    /// </summary>
    public bool CheckForUnSet(int Item)
    {
        return CountdownList[Item] == -1;
    }

    /// <summary>
    ///     检查倒计时是否生效
    /// </summary>
    public bool CheckForOnGuard(int Item)
    {
        return CountdownList[Item] != -1 && CountdownList[Item] + CooldownList[Item] >= GetTimeStamp();
    }

    /// <summary>
    ///     检查倒计时是否失效
    /// </summary>
    public bool CheckForOffGuard(int Item)
    {
        return CountdownList[Item] != -1 && CountdownList[Item] + CooldownList[Item] < GetTimeStamp();
    }

    /// <summary>
    ///     倒计时结束时行为
    /// </summary>
    public virtual void AfterOffGuard()
    {
    }

    /// <summary>
    ///     倒计时结束时击破护盾特效
    /// </summary>
    public virtual bool SetOffGuardProtect(out string notify, out int format_int, out float format_float)
    {
        notify = "";
        format_int = -255;
        format_float = -255;
        return true;
    }

    /// <summary>
    ///     重置倒计时
    /// </summary>
    public void ResetCountdown(int Item = 0)
    {
        CountdownList[Item] = GetTimeStamp();
    }

    /// <summary>
    ///     归零倒计时
    /// </summary>
    public void ZeroingCountdown(int Item = 0)
    {
        CountdownList[Item] = -1;
    }

    public long RemainTime(int Item = 0)
    {
        return CountdownList[Item] + CooldownList[Item] - GetTimeStamp();
    }

    #endregion

    #region 击杀与玩家死亡相关处理

    /// <summary>
    ///     CheckMurder 作为目标处理函数<br />
    ///     该函数用于您被击杀前的检查，调用该函数前已经调用过 OnCheckMurderAsKiller 函数<br />
    ///     因此您不需要判断击杀者是否真的尝试击杀你，击杀者对您尝试的击杀是确定的<br />
    ///     对于无法被击杀的状态（无敌、被保护）等，设置 info.CanKill = false<br />
    ///     若本次击杀本身就不合法，您可以返回 false 以完全终止本次击杀事件<br />
    /// </summary>
    /// <param name="info">击杀事件的信息</param>
    /// <returns>false：不再向下执行击杀事件</returns>
    public virtual bool OnCheckMurderAsTargetAfter(MurderInfo info)
    {
        return true;
    }

    /// <summary>
    ///     CheckMurder 作为目标处理函数<br />
    ///     该函数用于您被击杀前的检查，调用该函数前已经调用过 OnCheckMurderAsKiller 函数<br />
    ///     因此您不需要判断击杀者是否真的尝试击杀你，击杀者对您尝试的击杀是确定的<br />
    ///     对于无法被击杀的状态（无敌、被保护）等，设置 info.CanKill = false<br />
    ///     若本次击杀本身就不合法，您可以返回 false 以完全终止本次击杀事件<br />
    /// </summary>
    /// <param name="info">击杀事件的信息</param>
    /// <returns>false：不再向下执行击杀事件</returns>
    public virtual bool OnCheckMurderAsTargetBefore(MurderInfo info)
    {
        return true;
    }

    /// <summary>
    ///     已确定本次击杀会发生，但在真正发生前，您想要做点什么吗？
    /// </summary>
    /// <param name="info">击杀事件的信息</param>
    public virtual void BeforeMurderPlayerAsTarget(MurderInfo info)
    {
    }

    /// <summary>
    ///     MurderPlayer 作为目标处理函数
    /// </summary>
    /// <param name="info">击杀事件的信息</param>
    public virtual void OnMurderPlayerAsTarget(MurderInfo info)
    {
    }

    /// <summary>
    ///     玩家死亡时调用的函数
    ///     无论死亡玩家是谁，所有玩家都会调用，所以您需要判断死亡玩家的身份
    /// </summary>
    /// <param name="player">死亡玩家</param>
    /// <param name="deathReason">死亡原因</param>
    public virtual void OnPlayerDeath(PlayerControl player, AllDeathReason deathReason, bool isOnMeeting = false)
    {
    }

    #endregion

    #region 变形相关处理

    /// <summary>
    ///     仅自己视角变身
    ///     可以将外壳留在自己视角处
    /// </summary>
    public virtual bool CanDesyncShapeshift => false;

    /// <summary>
    ///     在变身检查时被调用
    ///     仅在自己变身时被调用
    ///     可以通过操作animate来切断变身动画
    /// </summary>
    /// <param name="target">变身目标</param>
    /// <param name="animate">是否播放动画</param>
    /// <returns>返回false可取消变身</returns>
    public virtual bool OnCheckShapeshift(PlayerControl target, ref bool animate)
    {
        return true;
    }

    public virtual bool OnCheckShapeshiftWithUsePet(ref bool animate, PlayerControl target = null)
    {
        return true;
    }

    /// <summary>
    ///     在变身检查时被调用
    ///     仅在自己变身时被调用
    ///     不会触发面板，立即执行技术操作（我吃柠檬这个玩意完全就是和隐身技术持平的东西）
    /// </summary>
    public virtual void UnShapeShiftButton(PlayerControl shapeshifter)
    {
    }

    /// <summary>
    ///     变形时调用的函数
    ///     不需要验证您的身份，因为调用前已经验证
    ///     请注意：全部模组端都会调用
    /// </summary>
    /// <param name="target">变形目标</param>
    public virtual void OnShapeshift(PlayerControl target)
    {
    }

    public virtual void OnShapeshiftWithUsePet(PlayerControl target = null)
    {
    }

    #endregion

    #region 隐身相关处理

    /// <summary>
    ///     隐身时调用的函数
    ///     不需要验证您的身份，因为调用前已经验证
    ///     请注意：全部模组端都会调用
    /// </summary>
    /// <returns>true：继续往下执行隐身操作</returns>
    public virtual bool OnCheckVanish()
    {
        return true;
    }

    /// <summary>
    ///     解除隐身时调用的函数
    ///     不需要验证您的身份，因为调用前已经验证
    ///     请注意：全部模组端都会调用
    /// </summary>
    /// <param name="animate">是否播放动画</param>
    public virtual bool OnAppear(bool animate)
    {
        return true;
    }

    #endregion

    #region 帧、秒task相关处理

    /// <summary>
    ///     帧 Task 处理函数<br />
    ///     不需要验证您的身份，因为调用前已经验证<br />
    ///     请注意：全部模组端都会调用<br />
    ///     如果您想在帧 Task 处理不是您自己时进行处理，请使用相同的参数将其实现为静态
    ///     并注册为 CustomRoleManager.OnFixedUpdateOthers
    /// </summary>
    /// <param name="player">目标玩家</param>
    public virtual void OnFixedUpdate(PlayerControl player)
    {
    }

    /// <summary>
    ///     秒 Task 处理函数<br />
    ///     不需要验证您的身份，因为调用前已经验证<br />
    ///     请注意：全部模组端都会调用<br />
    /// </summary>
    /// <param name="player">目标玩家</param>
    /// <param name="now">当前10位时间戳</param>
    public virtual void OnSecondsUpdate(PlayerControl player, long now)
    {
    }

    #endregion

    #region 报告相关处理

    /// <summary>
    ///     报告前检查调用的函数
    ///     与报告事件无关的玩家也会调用该函数
    /// </summary>
    /// <param name="reporter">报告者</param>
    /// <param name="target">被报告的玩家</param>
    /// <returns>false：取消报告</returns>
    public virtual bool OnCheckReportDeadBody(PlayerControl reporter, NetworkedPlayerInfo target)
    {
        return true;
    }

    /// <summary>
    ///     报告时调用的函数
    ///     与报告事件无关的玩家也会调用该函数
    /// </summary>
    /// <param name="reporter">报告者</param>
    /// <param name="target">被报告的玩家</param>
    public virtual void OnReportDeadBody(PlayerControl reporter, NetworkedPlayerInfo target)
    {
    }

    #endregion

    #region 宠物管道相关处理

    /// <summary>
    ///     <para>进入通风管时调用的函数</para>
    ///     <para>可以取消</para>
    /// </summary>
    /// <param name="physics"></param>
    /// <param name="ventId">通风管 ID</param>
    /// <returns>false：将玩家被踢出通风管，其他人将看不到动画。</returns>
    public virtual bool OnEnterVent(PlayerPhysics physics, int ventId)
    {
        return true;
    }

    /// <summary>
    ///     <para>离开通风管时调用的函数</para>
    ///     <para>可以取消</para>
    /// </summary>
    /// <param name="physics"></param>
    /// <param name="ventId">通风管 ID</param>
    /// <returns>false：将玩家被无法退出通风管，其他人将看不到动画。</returns>
    public virtual bool OnExitVent(PlayerPhysics physics, int ventId)
    {
        return true;
    }

    /// <summary>
    ///     摸宠物时调用的函数
    ///     不需要验证您的身份，因为调用前已经验证
    ///     请注意：全部模组端都会调用
    /// </summary>
    public virtual void OnUsePet()
    {
    }

    public virtual bool OnEnterVentWithUsePet(PlayerPhysics physics = null, int ventId = 173)
    {
        return true;
    }

    public virtual bool EnablePetSkill()
    {
        return false;
    }

    #endregion

    #region 会议相关处理

    /// <summary>
    ///     会议开始时调用的函数
    /// </summary>
    public virtual void OnStartMeeting()
    {
    }

    /// <summary>
    ///     会议开始时的全部提示信息
    /// </summary>
    /// <param name="msgToSend">等待发送的信息列表</param>
    public virtual void NotifyOnMeetingStart(ref List<(string, byte, string)> msgToSend)
    {
    }

    /// <summary>
    ///     在玩家投票时触发，此时还未计票<br />
    ///     如果返回 false，本次投票将被忽略，玩家可以再次投票<br />
    ///     如果不想忽略投票操作本身，也不希望计票，请使用 <see cref="ModifyVote" /> 并将 doVote 设置为 false
    /// </summary>
    /// <param name="votedFor">投票给</param>
    /// <returns>如果返回 false，则假装什么都没发生，除了投票者本身谁也不知道本次投票，并且投票者可以重新投票</returns>
    public virtual bool CheckVoteAsVoter(PlayerControl votedFor)
    {
        return true;
    }

    /// <summary>
    ///     在玩家投票时之后触发，此时还未计票<br />
    ///     通常用作在玩家投票后替换投票目标等用途（例如换票师）<br />
    ///     如果不想忽略投票操作本身，也不希望计票，请使用 <see cref="ModifyVote" /> 并将 doVote 设置为 false
    /// </summary>
    /// <param name="votedFor">被投票的玩家</param>
    /// <param name="voter">投票玩家</param>
    public virtual void AfterVoter(PlayerControl votedFor, PlayerControl voter)
    {
    }

    /// <summary>
    ///     玩家投票并确定计票时调用，并可以在此处修改投票<br />
    ///     如果希望忽略投票操作本身，请使用 <see cref="CheckVoteAsVoter" />
    /// </summary>
    /// <param name="voterId">投票人的ID</param>
    /// <param name="sourceVotedForId">被投票人的ID</param>
    /// <returns>(修改后的被票者的ID(不修改则为 null), 修改后的票数(不修改则为 null), 是否玩家自行操作)</returns>
    public virtual (byte? votedForId, int? numVotes, bool doVote) ModifyVote(byte voterId, byte sourceVotedForId,
        bool isIntentional)
    {
        return (null, null, true);
    }

    /// <summary>
    /// 当有人投票时触发
    /// </summary>
    /// <param name="voterId">投票者的ID</param>
    /// <param name="sourceVotedForId">被投票者的ID</param>
    /// <param name="roleVoteFor">修改此值以更改投票目标</param>
    /// <param name="roleNumVotes">修改此值以更改票数</param>
    /// <param name="clearVote">改为 true 则将投票者视为未投票状态，允许其再次进行投票。但投票数据还是会计入，若多次投票将以最后一次投票的数据为准</param>
    /// <returns>false: 忽略本次投票，不计入数据</returns>
    //public virtual bool OnVote(byte voterId, byte sourceVotedForId, ref byte roleVoteFor, ref int roleNumVotes, ref bool clearVote) => true;

    /// <summary>
    ///     驱逐玩家后调用的函数
    /// </summary>
    /// <param name="exiled">被驱逐的玩家</param>
    /// <param name="DecidedWinner">是否决定了胜利玩家</param>
    public virtual void OnExileWrapUp(NetworkedPlayerInfo exiled, ref bool DecidedWinner)
    {
    }

    /// <summary>
    ///     将要驱逐玩家调用的函数
    /// </summary>
    /// <param name="exiled">被驱逐的玩家</param>
    /// <param name="DecidedWinner">是否决定了胜利玩家</param>
    /// <param name="winDescriptionText">胜利描述文本</param>
    /// <returns>OnExileWrapUp 将要执行的函数</returns>
    public virtual Action CheckExile(NetworkedPlayerInfo exiled, ref bool DecidedWinner,
        ref List<string> WinDescriptionText)
    {
        return null;
    }

    /// <summary>
    ///     每次会议结束后调用的函数
    /// </summary>
    public virtual void AfterMeetingTasks()
    {
    }

    #endregion

    #region 破坏相关处理

    /// <summary>
    ///     当玩家造成破坏时调用
    ///     若禁止将无法关门
    /// </summary>
    /// <param name="systemType">破坏的设施类型</param>
    /// <returns>false：取消破坏</returns>
    public virtual bool OnInvokeSabotage(SystemTypes systemType)
    {
        return true;
    }

    /// <summary>
    ///     当有人破坏时调用
    /// </summary>
    /// <param name="player">造成破坏的玩家</param>
    /// <param name="systemType">造成破坏的设施</param>
    /// <returns>返回false取消破坏</returns>
    public virtual bool OnSabotage(PlayerControl player, SystemTypes systemType)
    {
        return true;
    }

    #endregion

    #region 名称相关处理

    // NameSystem
    // 显示的名字结构如下
    // [Role][Progress]
    // [Name][Mark]
    // [Lower][suffix]
    // Progress：任务进度、剩余子弹等信息
    // Mark：通过位置能力等进行目标标记
    // Lower：附加文本信息，模组端则会显示在屏幕下方
    // Suffix：其他信息，例如箭头

    /// <summary>
    ///     作为 seen 重写显示上的 RoleName
    /// </summary>
    /// <param name="seer">将要看到您的 RoleName 的玩家</param>
    /// <param name="enabled">是否显示 RoleName</param>
    /// <param name="roleColor">RoleName 的颜色</param>
    /// <param name="roleText">RoleName 的文本</param>
    public virtual void OverrideDisplayRoleNameAsSeen(PlayerControl seer, ref bool enabled, ref Color roleColor,
        ref string roleText)
    {
    }

    /// <summary>
    ///     作为 seer 重写显示上的 RoleName
    /// </summary>
    /// <param name="seen">您将要看到其 RoleName 的玩家</param>
    /// <param name="enabled">是否显示 RoleName</param>
    /// <param name="roleColor">RoleName 的颜色</param>
    /// <param name="roleText">RoleName 的文本</param>
    public virtual void OverrideDisplayRoleNameAsSeer(PlayerControl seen, ref bool enabled, ref Color roleColor,
        ref string roleText)
    {
    }

    /// <summary>
    ///     重写原来的职业名
    /// </summary>
    /// <param name="roleColor">RoleName 的颜色</param>
    /// <param name="roleText">RoleName 的文本</param>
    public virtual void OverrideTrueRoleName(ref Color roleColor, ref string roleText)
    {
    }

    /// <summary>
    ///     作为 seer 重写 ProgressText
    /// </summary>
    /// <param name="seen">您将要看到其 ProgressText 的玩家</param>
    /// <param name="enabled">是否显示 ProgressText</param>
    /// <param name="text">ProgressText 的文本</param>
    public virtual void OverrideProgressTextAsSeer(PlayerControl seen, ref bool enabled, ref string text)
    {
    }

    /// <summary>
    ///     显示在职业旁边的文本
    /// </summary>
    /// <param name="comms">目前是否为通讯破坏状态</param>
    public virtual string GetProgressText(bool comms = false)
    {
        return "";
    }

    /// <summary>
    ///     作为 seen 重写 Name
    /// </summary>
    /// <param name="seer">将要看到您的 Name 的玩家</param>
    /// <param name="nameText">Name 的文本</param>
    /// <param name="isForMeeting">是否用于显示在会议上</param>
    public virtual void OverrideNameAsSeen(PlayerControl seer, ref string nameText, bool isForMeeting = false)
    {
    }

    /// <summary>
    ///     作为 seer 重写 Name
    /// </summary>
    /// <param name="seen">您将要看到其 Name 的玩家</param>
    /// <param name="roleColor">Name 的颜色</param>
    /// <param name="nameText">Name 的文本</param>
    /// <param name="isForMeeting">是否用于显示在会议上</param>
    public virtual void OverrideNameAsSeer(PlayerControl seen, ref string nameText, bool isForMeeting = false)
    {
    }

    /// <summary>
    ///     作为 seer 时获取 Mark 的函数
    ///     如果您想在 seer,seen 都不是您时进行处理，请使用相同的参数将其实现为静态
    ///     并注册为 CustomRoleManager.MarkOthers
    /// </summary>
    /// <param name="seer">看到的人</param>
    /// <param name="seen">被看到的人</param>
    /// <param name="isForMeeting">是否正在会议中</param>
    /// <returns>構築したMark</returns>
    public virtual string GetMark(PlayerControl seer, PlayerControl seen = null, bool isForMeeting = false)
    {
        return "";
    }

    /// <summary>
    ///     作为 seer 时获取 LowerTex 的函数
    ///     如果您想在 seer,seen 都不是您时进行处理，请使用相同的参数将其实现为静态
    ///     并注册为 CustomRoleManager.LowerOthers
    /// </summary>
    /// <param name="seer">看到的人</param>
    /// <param name="seen">被看到的人</param>
    /// <param name="isForMeeting">是否正在会议中</param>
    /// <param name="isForHud">是否显示在模组端的HUD</param>
    /// <returns>组合后的全部 LowerText</returns>
    public virtual string GetLowerText(PlayerControl seer, PlayerControl seen = null, bool isForMeeting = false,
        bool isForHud = false)
    {
        return "";
    }

    /// <summary>
    ///     作为 seer 时获取 Suffix 的函数
    ///     如果您想在 seer,seen 都不是您时进行处理，请使用相同的参数将其实现为静态
    ///     并注册为 CustomRoleManager.SuffixOthers
    /// </summary>
    /// <param name="seer">看到的人</param>
    /// <param name="seen">被看到的人</param>
    /// <param name="isForMeeting">是否正在会议中</param>
    /// <returns>组合后的全部 Suffix</returns>
    public virtual string GetSuffix(PlayerControl seer, PlayerControl seen = null, bool isForMeeting = false)
    {
        return "";
    }

    #endregion

    #region 技能按钮相关处理

    /// <summary>
    ///     修改技能按钮的剩余次数
    /// </summary>
    public virtual int OverrideAbilityButtonUsesRemaining()
    {
        return -1;
    }

    /// <summary>
    ///     可以使用技能按钮
    /// </summary>
    /// <returns>true：可以使用能力按钮</returns>
    public virtual bool CanUseAbilityButton()
    {
        return true;
    }

    #endregion

    #region 游戏开始相关处理

    /// <summary>
    ///     更改游戏开始时的音效
    /// </summary>
    public virtual bool GetGameStartSound(out string sound)
    {
        sound = null;
        return false;
    }

    /// <summary>
    ///     游戏开始后会立刻调用该函数
    ///     默认为全体玩家调用
    /// </summary>
    public virtual void OnGameStart()
    {
    }

    #endregion
}