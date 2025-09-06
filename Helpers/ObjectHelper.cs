namespace TruthAPI.Helpers
{
    public static class ObjectHelper
    {
        public static T DontDestroy<T>(this T obj) where T : UnityEngine.Object
        {
            obj.hideFlags |= HideFlags.HideAndDontSave;
            return obj.DontDestroyOnLoad();
        }
        public static T DontDestroyOnLoad<T>(this T obj) where T : UnityEngine.Object
        {
            UnityEngine.Object.DontDestroyOnLoad(obj);
            return obj;
        }
        public static T Destroy<T>(this T obj) where T : UnityEngine.Object
        {
            UnityEngine.Object.Destroy(obj);
            return obj;
        }
    }
}
