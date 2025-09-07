using System.Collections.Generic;
using HarmonyLib;
using InnerNet;
using TMPro;
using UnityEngine;

namespace TruthAPI.Managers
{
    public class WatermarkManager
    {
        public class Watermark
        {
            /// <summary>
            /// VersionShower文本
            /// </summary>
            public string VersionText { get; set; }

            /// <summary>
            /// PingTrancker文本
            /// </summary>
            public string PingText { get; set; }

            public Watermark(string versionText, string pingText)
            {
                VersionText = versionText;
                PingText = pingText;
            }
        }

        private static List<Watermark> Watermarks = new List<Watermark>();

        public static Watermark ApiWatermark = new Watermark($"<color=#00168A>TruthAPI <color=#A20004> {TruthAPI.Version} <color=#ffffffff> by <color=#cdfffd>XtremeWave",
            $"\n{ColorHelper.GradientColorText("00168A", "A20004","TruthAPI")}");

        public static void AddWatermark(string versionText, string pingText)
        {
            var watermark = new Watermark(versionText, pingText);
            Watermarks.Add(watermark);
        }
        [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
        public static class PingTrackerUpdatePatch
        {
            public static void Postfix(PingTracker __instance)
            {
                var position = __instance.GetComponent<AspectPosition>();
                if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started)
                {
                    __instance.text.alignment = TextAlignmentOptions.Top;
                    position.Alignment = AspectPosition.EdgeAlignments.Top;
                    position.DistanceFromEdge = new Vector3(1.5f, 0.11f, 0);
                }
                else
                {
                    position.Alignment = AspectPosition.EdgeAlignments.LeftTop;
                    __instance.text.alignment = TextAlignmentOptions.TopLeft;
                    position.DistanceFromEdge = new Vector3(0.5f, 0.11f);
                }
                foreach (var watermark in Watermarks)
                {                
                    if (watermark.PingText != null)
                        __instance.text.text += watermark.PingText;
                }

                if (ApiWatermark.PingText != null)
                    __instance.text.text += ApiWatermark.PingText;
            }
        }
    }
}
