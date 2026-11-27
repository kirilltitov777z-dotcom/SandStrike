using UnityEngine;

namespace SandStrike
{
    /** Автостарт без правки сцен: игра подхватывает управление после загрузки любой сцены. */
    public static class SandBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Boot()
        {
            if (Object.FindObjectOfType<SandGame>() != null) return;
            GameObject root = new GameObject("SandStrike");
            Object.DontDestroyOnLoad(root);
            root.AddComponent<SandGame>();
        }
    }
}
