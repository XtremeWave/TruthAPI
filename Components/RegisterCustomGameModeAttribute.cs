using System;
using System.Reflection;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using TruthAPI.GameModes;

namespace TruthAPI.Components
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterCustomGameModeAttribute : Attribute
    {
        public static void Register(BasePlugin plugin)
        {
            Register(Assembly.GetCallingAssembly(), plugin);
        }

        public static void Register(Assembly assembly, BasePlugin plugin)
        {
            foreach (var type in assembly.GetTypes())
            {
                var attribute = type.GetCustomAttribute<RegisterCustomGameModeAttribute>(); 

                if (attribute != null)
                {
                    if (!type.IsSubclassOf(typeof(GameMode)))
                    {
                        throw new InvalidOperationException($"Type {type.FullDescription()} must extend {nameof(GameMode)}.");
                    }

                    Info($"Registered mode {type.Name} from {type.Assembly.GetName().Name}", "Registered GameMode");

                    Activator.CreateInstance(type, plugin);
                }
            }
        }

        public static void Load()
        {
            IL2CPPChainloader.Instance.PluginLoad += (pluginInfo, assembly, plugin) => Register(assembly, plugin);
        }
    }
}