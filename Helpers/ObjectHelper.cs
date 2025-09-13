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
        public static T DontUnload<T>(this T obj) where T : UnityEngine.Object
        {
            obj.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return obj;
        }
        private static GameObject CreateObject(string objName, Transform parent, Vector3 localPosition, int? layer = null)
        {
            var obj = new GameObject(objName);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = localPosition;
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            if (layer.HasValue) obj.layer = layer.Value;
            else if (parent) obj.layer = parent.gameObject.layer;
            return obj;
        }

        private static T CreateObject<T>(string objName, Transform parent, Vector3 localPosition, int? layer = null)
            where T : Component
        {
            return CreateObject(objName, parent, localPosition, layer).AddComponent<T>();
        }

        public static SpriteRenderer CreateSpriteRenderer(string name, string spriteName, float pixelsPerUnit,
            Vector3 position, Transform parent = null)
        {
            var renderer = CreateObject<SpriteRenderer>(name, null, position);
            if (parent != null)
                renderer.gameObject.transform.SetParent(parent);
            renderer.gameObject.transform.localPosition = position;
            renderer.sprite = LoadSprite(spriteName, pixelsPerUnit);
            renderer.color = Color.clear;

            return renderer;
        }
    }
}
