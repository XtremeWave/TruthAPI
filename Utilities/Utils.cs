using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using AmongUs.GameOptions;
using TruthAPI.Helpers;
using TruthAPI.Extension.PlayerControlExtension;
using static TruthAPI.XtremeGameData.XtremeGameData.GameStates;
using Il2CppInterop.Runtime.InteropTypes;
using InnerNet;
using UnityEngine;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace TruthAPI.Utilities;

public static class Utils
{
    private static readonly DateTime timeStampStartTime = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


    private static readonly Dictionary<byte, PlayerControl> cachedPlayers = new();
    public static long TimeStamp => (long)(DateTime.Now.ToUniversalTime() - timeStampStartTime).TotalSeconds;

    public static long GetTimeStamp(DateTime? dateTime = null)
    {
        return (long)((dateTime ?? DateTime.Now).ToUniversalTime() - timeStampStartTime).TotalSeconds;
    }

    public static float GetResolutionOffset()
    {
        return (float)Screen.width / Screen.height / (16f / 9f);
    }

    private static ClientData GetClientById(int id)
    {
        try
        {
            var client = AmongUsClient.Instance.allClients.ToArray().FirstOrDefault(cd => cd.Id == id);
            return client;
        }
        catch
        {
            return null;
        }
    }

    public static string PadRightV2(this object text, int num)
    {
        var t = text.ToString();
        var bc = t!.Sum(c => Encoding.GetEncoding("UTF-8").GetByteCount(c.ToString()) == 1 ? 1 : 2);
        return t.PadRight(Mathf.Max(num - (bc - t.Length), 0));
    }

    /// <summary>
    ///     乱数の簡易的なヒストグラムを取得する関数
    ///     <params name="nums">生成した乱数を格納したint配列</params>
    ///     <params name="scale">ヒストグラムの倍率 大量の乱数を扱う場合、この値を下げることをお勧めします。</params>
    /// </summary>
    public static bool AmDev()
    {
        return IsDev(EOSManager.Instance.FriendCode);
    }

    public static bool IsDev(string friendCode)
    {
        return friendCode
            is "teamelder#5856" //Slok
            or "cloakhazy#9133" //LezaiYa
            or "aideproof#8388"; //ELinmei
    }

    public static PlayerControl GetPlayerById(int playerId)
    {
        return GetPlayerById((byte)playerId);
    }

    public static PlayerControl GetPlayerById(byte playerId)
    {
        if (cachedPlayers.TryGetValue(playerId, out var cachedPlayer) && cachedPlayer) return cachedPlayer;

        var player = TruthAPI.AllPlayerControls.FirstOrDefault(pc => pc.PlayerId == playerId);
        cachedPlayers[playerId] = player;
        return player;
    }

    public static void ExecuteWithTryCatch(this Action action, bool Log = false)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            if (Log) Error(ex.ToString(), "Execute With Try Catch");
        }
    }

    public static void FormatButtonColor(MainMenuManager __instance, PassiveButton button, Color inActiveColor,
        Color activeColor, Color inActiveTextColor, Color activeTextColor)
    {
        button.activeSprites.transform.FindChild("Shine")?.gameObject.SetActive(false);
        button.inactiveSprites.transform.FindChild("Shine")?.gameObject.SetActive(false);
        var activeRenderer = button.activeSprites.GetComponent<SpriteRenderer>();
        var inActiveRenderer = button.inactiveSprites.GetComponent<SpriteRenderer>();
        activeRenderer.sprite = __instance.quitButton.activeSprites.GetComponent<SpriteRenderer>().sprite;
        inActiveRenderer.sprite = __instance.quitButton.activeSprites.GetComponent<SpriteRenderer>().sprite;
        activeRenderer.color = activeColor.a == 0f
            ? new Color(inActiveColor.r, inActiveColor.g, inActiveColor.b, 1f)
            : activeColor;
        inActiveRenderer.color = inActiveColor;
        button.activeTextColor = activeTextColor;
        button.inactiveTextColor = inActiveTextColor;
    }

    public static long GetCurrentTimestamp()
    {
        return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }

    #region Sprite

    public static Dictionary<string, Sprite> CachedSprites = new();
    public static Sprite loadSpriteFromResources(string path, float pixelsPerUnit, bool cache = true)
    {
        try
        {
            if (cache && CachedSprites.TryGetValue(path + pixelsPerUnit, out var sprite)) return sprite;
            Texture2D texture = loadTextureFromResources(path);
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            if (cache) sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
            if (!cache) return sprite;
            return CachedSprites[path + pixelsPerUnit] = sprite;
        }
        catch
        {
            System.Console.WriteLine("Error loading sprite from path: " + path);
        }
        return null;
    }
    public static unsafe Texture2D loadTextureFromResources(string path)
    {
        try
        {
            Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(path);
            var length = stream.Length;
            var byteTexture = new Il2CppStructArray<byte>(length);
            stream.Read(new Span<byte>(IntPtr.Add(byteTexture.Pointer, IntPtr.Size * 4).ToPointer(), (int)length));
            if (path.Contains("HorseHats"))
            {
                byteTexture = new Il2CppStructArray<byte>(byteTexture.Reverse().ToArray());
            }
            ImageConversion.LoadImage(texture, byteTexture, false);
            return texture;
        }
        catch
        {
            System.Console.WriteLine("Error loading texture from resources: " + path);
        }
        return null;
    }

    #endregion

}