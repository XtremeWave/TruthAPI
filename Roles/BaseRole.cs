using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Unity.IL2CPP;
using TruthAPI.Enum;
using TruthAPI.Managers;
using TruthAPI.Options;
using TruthAPI.Utilities;
using UnityEngine;
using static TruthAPI.Enum.Visibility;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

namespace TruthAPI.Roles
{
    public abstract class BaseRole
    {
        public int Id { get; }

        public List<byte> Members = new List<byte>();

        public RoleBehaviour RoleBehaviour;

        /// <summary>
        /// 职业名称
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 职业介绍
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// 职业详细介绍
        /// </summary>
        public abstract string LongDescription { get; }

        /// <summary>
        /// 职业图标
        /// </summary>
        public virtual Sprite Icon { get; } = loadSpriteFromResources("PeasAPI.Resources.Placeholder.png",115f);
        
        /// <summary>
        /// 任务栏介绍
        /// </summary>
        public abstract string TaskText { get; }

        /// <summary>
        /// 职业名称，Outro的颜色
        /// </summary>
        public abstract Color Color { get; }

        /// <summary>
        /// 职业的技能，详情在另一个Base
        /// </summary>
        //public abstract  Skill { get; }
        /// <summary>
        /// 字面意思
        /// </summary>
        public abstract Visibility Visibility { get; }

        /// <summary>
        /// 职业阵营
        /// </summary>
        public abstract TeamTypes Team { get; }

        /// <summary>
        /// 注册的任务
        /// </summary>
        public virtual bool AssignTasks { get; set; } = true;
        /// <summary>
        /// 决定你是否有活可以干的喵
        /// </summary>
        public abstract bool HasToDoTasks { get; }

        /// <summary>
        /// 应该出现此职业的概率
        /// </summary>
        public virtual int Chance { get; set; } = 0;
        /// <summary>
        /// 出现多少个此职业
        /// </summary>
        public virtual int Count { get; set; } = 0;
        /// <summary>
        /// 最大出现多少个此职业
        /// </summary>
        public virtual int MaxCount { get; set; } = 15;
        
        public virtual bool CreateRoleOption { get; set; } = true;
        /// <summary>
        /// 该角色的固定生成设置
        /// </summary>
        public CustomRoleOption Option;
        
        public virtual Dictionary<string, CustomOption> AdvancedOptions { get; set; } = new Dictionary<string, CustomOption>();

        public virtual string AdvancedOptionsPrefix { get; set; } = "└ ";
        /// <summary>
        /// 游戏模式白名单（雾
        /// </summary>
        public virtual Type[] GameModeWhitelist { get; } = Array.Empty<Type>();
        /// <summary>
        /// 击杀距离
        /// </summary>
        public virtual float KillDistance { get; set; } = Mathf.Clamp(GameManager.Instance?.LogicOptions?.GetKillDistance() ?? 1.8f, 0, 2);

        /// <summary>
        /// If a member of the role should be able to kill that player / in general
        /// </summary>
        public virtual bool CanKill(PlayerControl victim = null)
        {
            return false;
        }

        /// <summary>
        /// If a member of the role should be able to use vents
        /// </summary>
        public virtual bool CanVent { get; } = false;
        
        /// <summary>
        /// If a member of the role should be able to sabotage that sabotage type / in general
        /// </summary>
        public virtual bool CanSabotage(SystemTypes? sabotage)
        {
            return false;
        }
        
        /// <summary>
        /// If a player should be able to see a person with this role.
        /// Only works with Visibility.Custom
        /// </summary>
        public virtual bool IsRoleVisible(PlayerControl playerWithRole, PlayerControl perspective)
        {
            return false;
        }
        
        public virtual bool _IsRoleVisible(PlayerControl playerWithRole, PlayerControl perspective)
        {
            if (playerWithRole.PlayerId == perspective.PlayerId)
                return true;

            if (playerWithRole.Data.IsDead)
                return true;
            
            switch (this.Visibility)
            {
                case Visibility.Role: return perspective.IsCustomRole(this);
                case Visibility.Impostor: return perspective.Data.Role.IsImpostor;
                case Visibility.Crewmate: return true;
                case Visibility.NoOne: return false;
                case Visibility.Custom: return this.IsRoleVisible(playerWithRole, perspective);
                default: throw new NotImplementedException("Unknown Visibility");
            }
        }
        
        public virtual bool ShouldGameEnd(GameOverReason reason) => true;
        
        /// <summary>
        /// Gets called when the game starts
        /// </summary>
        public virtual void OnGameStart()
        {
        }
        
        /// <summary>
        /// Gets called when the game stops
        /// </summary>
        public virtual void OnGameStop()
        {
        }

        internal void _OnUpdate()
        {
            foreach (var player in Members)
            {
                var playerControl = player.GetPlayer();
                if (playerControl == null)
                    continue;
                if (PlayerControl.LocalPlayer == null)
                    continue;
                if (playerControl.IsCustomRole(this) && _IsRoleVisible(playerControl, PlayerControl.LocalPlayer))
                {
                    playerControl.cosmetics.nameText.color = this.Color;
                    playerControl.cosmetics.nameText.text = $"{player.GetPlayer().name}\n{Name}";
                }
            }

            OnUpdate();
        }

        /// <summary>
        /// Gets called every frame
        /// </summary>
        public virtual void OnUpdate()
        {
        }

        internal void _OnMeetingUpdate(MeetingHud __instance)
        {
            if (PlayerMenuManager.IsMenuOpen)
                return;
            
            foreach (var player in Members)
            {
                var playerControl = player.GetPlayer();
                if (playerControl == null)
                    continue;
                if (PlayerControl.LocalPlayer == null)
                    continue;
                if (playerControl.IsCustomRole(this) && _IsRoleVisible(playerControl, PlayerControl.LocalPlayer))
                {
                    playerControl.cosmetics.nameText.color = this.Color;
                    playerControl.cosmetics.nameText.text = $"{player.GetPlayer().name}\n{Name}";
                }
            }

            foreach (var pstate in __instance.playerStates)
            {
                var player = pstate.TargetPlayerId.GetPlayer();
                if (player == null)
                    continue;
                if (PlayerControl.LocalPlayer == null)
                    continue;
                if (player.IsCustomRole(this) && _IsRoleVisible(player, PlayerControl.LocalPlayer))
                {
                    pstate.NameText.color = Color;
                    pstate.NameText.text = $"{player.name}\n{Name}";
                }
            }
            
            OnMeetingUpdate(__instance);
        }

        /// <summary>
        /// Gets called every frame when a meeting is active. The meeting gets passed on
        /// </summary>
        public virtual void OnMeetingUpdate(MeetingHud meeting)
        {
        }

        public virtual void OnMeetingStart(MeetingHud meeting)
        {
        }

        public virtual bool PreKill(PlayerControl killer, PlayerControl victim)
        {
            return true;
        }
        
        public virtual void OnKill(PlayerControl killer, PlayerControl victim)
        {
        }
        
        public virtual bool PreExile(PlayerControl victim)
        {
            return true;
        }
        
        public virtual void OnExiled(PlayerControl victim)
        {
        }

        public virtual void OnRevive(PlayerControl player)
        {
        }
        
        public virtual void OnTaskComplete(PlayerControl player, PlayerTask task)
        {
        }

        public int GetCount()
        {
            return Option?.GetCount() ?? Count;
        }

        public int GetChance()
        {
            return Option?.GetChance() ?? Chance;
        }

        public BaseRole(BasePlugin plugin)
        {
            Id = ModRoleManager.GetRoleId();
            RoleBehaviour = ModRoleManager.ToRoleBehaviour(this);
            if (CreateRoleOption)
                Option = new CustomRoleOption(this, AdvancedOptionsPrefix, AdvancedOptions.Values.ToArray());
            ModRoleManager.RegisterRole(this);
        }
    }
}
