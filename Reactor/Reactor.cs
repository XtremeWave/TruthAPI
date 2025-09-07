using BepInEx.Core.Logging.Interpolation;
using BepInEx.Unity.IL2CPP.Utils;
using GameCore;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TruthAPI.Reactor;

public static class Reactor
{
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> collection)
    {
        return collection.SelectMany(x => x);
    }

    /// <summary>
    ///     销毁对象的<see cref="TextTranslatorTMP" />组件
    /// </summary>
    public static void DestroyTranslator(this GameObject obj)
    {
        if (!obj) return;
        obj.ForEachChild((Action<GameObject>)(x => DestroyTranslator(x)));
        TextTranslatorTMP[] translator = obj.GetComponentsInChildren<TextTranslatorTMP>(true);
        translator?.Do(UnityEngine.Object.Destroy);
    }

    /// <summary>
    ///     销毁对象的 <see cref="TextTranslatorTMP" /> 组件
    /// </summary>
    public static void DestroyTranslator(this MonoBehaviour obj)
    {
        obj?.gameObject.DestroyTranslator();
    }

    public static void Destroy(this UnityEngine.Object obj)
	{
		UnityEngine.Object.Destroy(obj);
	}

	public static void DestroyImmediate(this UnityEngine.Object obj)
	{
		UnityEngine.Object.DestroyImmediate(obj);
	}

	public static void SetSize(this RectTransform rectTransform, float width, float height)
	{
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
	}
}
public static class RandomExtensions
{
    public static double NextDouble(this System.Random random, double minValue, double maxValue)
    {
        return random.NextDouble() * (maxValue - minValue) + minValue;
    }
    public static T? Random<T>(this IEnumerable<T> input)
    {
        IList<T> list = (input as IList<T>) ?? input.ToList();
        if (list.Count != 0)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        return default(T);
    }
    public static T? Random<T>(this IEnumerable<T> input, System.Random random)
    {
        IList<T> list = (input as IList<T>) ?? input.ToList();
        if (list.Count != 0)
        {
            return list[random.Next(0, list.Count)];
        }

        return default(T);
    }
}
public static class Coroutines
{
	[RegisterInIl2Cpp]
	public class Component : MonoBehaviour
	{
        public static Component Instance { get; set; }

		public Component(IntPtr ptr) : base(ptr)
		{
		}

		private void Awake()
		{
			Instance = this;
		}

		private void OnDestroy()
		{
			Instance = null;
		}
	}

	private static readonly ConditionalWeakTable<IEnumerator, Coroutine> _ourCoroutineStore = new();

	[return: NotNullIfNotNull("routine")]
	public static IEnumerator Start(IEnumerator routine)
	{
		if (routine != null)
		{
			_ourCoroutineStore.AddOrUpdate(routine, Component.Instance!.StartCoroutine(routine));
		}

		return routine;
	}

	public static void Stop(IEnumerator enumerator)
	{
		if (enumerator != null && _ourCoroutineStore.TryGetValue(enumerator, out var routine))
		{
			Component.Instance!.StopCoroutine(routine);
		}
	}
}
[AttributeUsage(AttributeTargets.Class)]
public class RegisterInIl2CppAttribute : Attribute
{
	public Type[] Interfaces { get; }

	public RegisterInIl2CppAttribute()
	{
		Interfaces = Type.EmptyTypes;
	}

	public RegisterInIl2CppAttribute(params Type[] interfaces)
	{
		Interfaces = interfaces;
	}

	[Obsolete("You don't need to call this anymore", true)]
	public static void Register()
	{
	}

	private static void Register(Type type, Type[] interfaces)
	{
		var baseTypeAttribute = type.BaseType?.GetCustomAttribute<RegisterInIl2CppAttribute>();
		if (baseTypeAttribute != null)
		{
			Register(type.BaseType!, baseTypeAttribute.Interfaces);
		}

		if (ClassInjector.IsTypeRegisteredInIl2Cpp(type))
		{
			return;
		}

		try
		{
			ClassInjector.RegisterTypeInIl2Cpp(type, new RegisterTypeOptions { Interfaces = interfaces });
		}
		catch { }
	}

	public static void Register(Assembly assembly)
	{
		foreach (var type in assembly.GetTypes())
		{
			var attribute = type.GetCustomAttribute<RegisterInIl2CppAttribute>();
			if (attribute != null)
			{
				Register(type, attribute.Interfaces);
			}
		}
	}
}
public static class CustomStringName
{
    private static int _lastId = -2147483647;
    public static StringNames Create()
    {
        return (StringNames)(_lastId++);
    }
    public static StringNames CreateAndRegister(string text)
    {
        StringNames num = Create();
        HardCodedLocalizationProvider.Register(num, text);
        return num;
    }
}
public sealed class HardCodedLocalizationProvider : LocalizationProvider
{
    private static readonly Dictionary<StringNames, string> _strings = new Dictionary<StringNames, string>();

    public override int Priority => -400;
    public static void Register(StringNames stringName, string value)
    {
        if (_strings.ContainsKey(stringName))
        {
            bool isEnabled;
            BepInExWarningLogInterpolatedStringHandler bepInExWarningLogInterpolatedStringHandler = new BepInExWarningLogInterpolatedStringHandler(43, 1, out isEnabled);
            if (isEnabled)
            {
                bepInExWarningLogInterpolatedStringHandler.AppendLiteral("Registering StringName ");
                bepInExWarningLogInterpolatedStringHandler.AppendFormatted(stringName);
                bepInExWarningLogInterpolatedStringHandler.AppendLiteral(" that already exists");
            }
        }

        _strings[stringName] = value;
    }

    public override bool TryGetText(StringNames stringName, out string? result)
    {
        return _strings.TryGetValue(stringName, out result);
    }
}
public abstract class LocalizationProvider
{
    public SupportedLangs? CurrentLanguage { get; private set; }
    public virtual int Priority => 0;
    public virtual bool TryGetText(StringNames stringName, out string? result)
    {
        result = null;
        return false;
    }
    public virtual bool TryGetTextFormatted(StringNames stringName, Il2CppReferenceArray<Il2CppSystem.Object> parts, out string? result)
    {
        if (!TryGetText(stringName, out result))
        {
            return false;
        }

        result = String.Format(result, parts);
        return true;
    }
    public virtual bool TryGetStringName(SystemTypes systemType, out StringNames? result)
    {
        result = null;
        return false;
    }
    public virtual bool TryGetStringName(TaskTypes taskType, out StringNames? result)
    {
        result = null;
        return false;
    }
    public virtual void OnLanguageChanged(SupportedLangs newLanguage)
    {
    }

    internal void SetLanguage(SupportedLangs newLanguage)
    {
        OnLanguageChanged(newLanguage);
        CurrentLanguage = newLanguage;
    }
}
