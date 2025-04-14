using System;
using System.Linq;

namespace Erenshor_Global_Chat_Mod
{
    static class SceneValidator
    {
        public static string[] InvalidScenes = new string[]
        {
            "Menu",
            "LoadScene",
        };

        public static bool IsValidScene(string sceneName)
        {
            return !InvalidScenes.Contains(sceneName, StringComparer.OrdinalIgnoreCase);
        }
    }
}
