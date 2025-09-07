using UnityEngine;

namespace TruthAPI.Helpers;

public static class APIColorHelper
{
    private const float MarkerSat = 1f;
    private const float MarkerVal = 1f;
    private const float MarkerAlpha = 0.2f;

    /// <summary>将颜色转换为荧光笔颜色</summary>
    /// <param name="color">颜色</param>
    /// <param name="bright">是否将颜色调整为最大亮度。如果希望较暗的颜色保持不变，请传入 false</param>
    public static Color ToMarkingColor(this Color color, bool bright = true)
    {
        Color.RGBToHSV(color, out var h, out _, out var v);
        var markingColor = Color.HSVToRGB(h, MarkerSat, bright ? MarkerVal : v).SetAlpha(MarkerAlpha);
        return markingColor;
    }

    public static string SetStringColor(Color c, string s)
    {
        return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", (byte)(Mathf.Clamp01(c.r) * 255), (byte)(Mathf.Clamp01(c.g) * 255), (byte)(Mathf.Clamp01(c.b) * 255),
            (byte)(Mathf.Clamp01(c.a) * 255), s);
    }

    public static Color HexToColor(string hex)
    {
        Color color = new();
        _ = ColorUtility.TryParseHtmlString("#" + hex, out color);
        return color;
    }

    public static string ColorToHex(Color color)
    {
        Color32 color32 = color;
        return $"{color32.r:X2}{color32.g:X2}{color32.b:X2}{color32.a:X2}";
    }
    /// <summary>
    ///     Darkness: 按1的比例混合黑色与原色。负值则与白色混合。
    /// </summary>
    public static Color ShadeColor(this Color color, float Darkness = 0)
    {
        var IsDarker = Darkness >= 0; //与黑色混合
        if (!IsDarker) Darkness = -Darkness;
        var Weight = IsDarker ? 0 : Darkness; //黑/白的混合比例
        var R = (color.r + Weight) / (Darkness + 1);
        var G = (color.g + Weight) / (Darkness + 1);
        var B = (color.b + Weight) / (Darkness + 1);
        return new Color(R, G, B, color.a);
    }

    private static void ColorToHSV(Color color, out float hue /*, out float saturation, out float value*/)
    {
        var max = Mathf.Max(color.r, Mathf.Max(color.g, color.b));
        var min = Mathf.Min(color.r, Mathf.Min(color.g, color.b));
        var delta = max - min;

        hue = 0f;
        //saturation = 0f;
        //value = max;

        if (delta != 0)
        {
            if (Mathf.Approximately(max, color.r))
            {
                hue = (color.g - color.b) / delta;
            }
            else if (Mathf.Approximately(max, color.g))
            {
                hue = 2 + (color.b - color.r) / delta;
            }
            else
            {
                hue = 4 + (color.r - color.g) / delta;
            }

            hue *= 60;
            if (hue < 0) hue += 360;
        }

        //if (max != 0)
        //{
        //saturation = delta / max;
        //}
    }

    private static Color HSVToColor(float hue, float saturation, float value)
    {
        var i = Mathf.FloorToInt(hue / 60) % 6;
        var f = hue / 60 - Mathf.Floor(hue / 60);
        var p = value * (1 - saturation);
        var q = value * (1 - f * saturation);
        var t = value * (1 - (1 - f) * saturation);

        return i switch
        {
            0 => new Color(value, t, p),
            1 => new Color(q, value, p),
            2 => new Color(p, value, t),
            3 => new Color(p, q, value),
            4 => new Color(t, p, value),
            _ => new Color(value, p, q)
        };
    }

    public static Color ConvertToLightGray(Color color)
    {
        ColorToHSV(color, out var hue /*, out _, out _*/);
        return HSVToColor(hue, 0f, 0.9f);
    }

    public static Color GetColorByPercentage(float percentage)
    {
        return new Color(
            r: Mathf.Clamp01(0.6f + percentage * 0.008f), // 0.6->1.0
            g: Mathf.Clamp01(1.0f - percentage * 0.01f), // 1.0->0.0
            b: Mathf.Clamp01(0.6f - percentage * 0.006f) // 0.6->0.0
        );
    }
     public static string GradientColorText(string startColorHex, string endColorHex, string text)
    {
        if (startColorHex.Length != 6 || endColorHex.Length != 6)
        {
            Error("GradientColorText : Invalid Color Hex Code, Hex code should be 6 characters long (without #) (e.g., FFFFFF).","GradientColorText");
            return text;
        }

        Color startColor = HexToColor(startColorHex);
        Color endColor = HexToColor(endColorHex);

        int textLength = text.Length;
        float stepR = (endColor.r - startColor.r) / (float)textLength;
        float stepG = (endColor.g - startColor.g) / (float)textLength;
        float stepB = (endColor.b - startColor.b) / (float)textLength;
        float stepA = (endColor.a - startColor.a) / (float)textLength;

        string gradientText = "";

        for (int i = 0; i < textLength; i++)
        {
            float r = startColor.r + (stepR * i);
            float g = startColor.g + (stepG * i);
            float b = startColor.b + (stepB * i);
            float a = startColor.a + (stepA * i);


            string colorhex = ColorToHex(new Color(r, g, b, a));
            gradientText += $"<color=#{colorhex}>{text[i]}</color>";
        }

        return gradientText;
    }
    public class StringColor
    {
        public const string Reset = "<color=#ffffffff>";
        public const string White = "<color=#ffffffff>";
        public const string Black = "<color=#000000ff>";
        public const string Red = "<color=#ff0000ff>";
        public const string Green = "<color=#169116ff>";
        public const string Blue = "<color=#0400ffff>";
        public const string Yellow = "<color=#f5e90cff>";
        public const string Purple = "<color=#a600ffff>";
        public const string Cyan = "<color=#00fff2ff>";
        public const string Pink = "<color=#e34dd4ff>";
        public const string Orange = "<color=#ff8c00ff>";
        public const string Brown = "<color=#8c5108ff>";
        public const string Lime = "<color=#1eff00ff>";
    }
}