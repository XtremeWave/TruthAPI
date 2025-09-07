﻿namespace TruthAPI.Components;

/// <summary>
///     用于在 <see cref="AmongUsClient.CoStartGame" /> 的后期修饰中，用于每次游戏初始化的方法
///     在静态方法前加上 [GameModuleInitializer]，可以在游戏开始时自动调用
///     通过 [GameModuleInitializer(InitializePriority.High)] 可以指定调用顺序
/// </summary>
public sealed class GameModuleInitializerAttribute(InitializePriority priority = InitializePriority.Normal)
    : InitializerAttribute<GameModuleInitializerAttribute>(priority);