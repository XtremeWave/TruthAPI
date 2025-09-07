using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace TruthAPI.Utilities
{
    public static class Utility
    {
        public static List<PlayerControl> GetAllPlayers()
        {
            if (PlayerControl.AllPlayerControls != null && PlayerControl.AllPlayerControls.Count > 0)
                return PlayerControl.AllPlayerControls.ToArray().ToList();
            return GameData.Instance.AllPlayers.ToArray().ToList().ConvertAll(p => p.Object);
        }

        public static void OverrideOnClickListeners(this PassiveButton passive, Action action, bool enabled = true)
        {
            passive.OnClick?.RemoveAllListeners();
            passive.OnClick = new();
            passive.OnClick.AddListener(action);
            passive.enabled = enabled;
        }

        public static void OverrideOnMouseOverListeners(this PassiveButton passive, Action action, bool enabled = true)
        {
            passive.OnMouseOver?.RemoveAllListeners();
            passive.OnMouseOver = new();
            passive.OnMouseOver.AddListener(action);
            passive.enabled = enabled;
        }

        public static void OverrideOnMouseOutListeners(this PassiveButton passive, Action action, bool enabled = true)
        {
            passive.OnMouseOut?.RemoveAllListeners();
            passive.OnMouseOut = new();
            passive.OnMouseOut.AddListener(action);
            passive.enabled = enabled;
        }
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        public static IEnumerator PerformTimedAction(float duration, Action<float> action)
        {
            for (var t = 0f; t < duration; t += Time.deltaTime)
            {
                action(t / duration);
                yield return EndFrame();
            }

            action(1f);
            yield break;
        }

        public static IEnumerator EndFrame()
        {
            yield return new WaitForEndOfFrame();
        }
    }
}