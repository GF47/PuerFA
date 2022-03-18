using Puerts;
using UnityEngine;
using System.IO;

namespace Modules.JS
{
    public class AddressableLoader : ILoader
    {
        public bool FileExists(string filepath)
        {
            var path = FormatFilePath(filepath);

            if (Resources.Load(path) != null) { return true; }

            return JSModule.Instance.JSFiles.ContainsKey(path);
        }

        public string ReadFile(string filepath, out string debugpath)
        {
            var path = FormatFilePath(filepath);

            if (InResources(path, out var content))
            {
                debugpath = Path.Combine(Application.dataPath, "3rd/Puerts/Src/Resources", filepath).Replace('\\', '/');
                return content;
            }

            debugpath = Path.Combine(Application.dataPath, "AddressablesRoot/Scripts", filepath + ".txt").Replace('\\', '/');
            return JSModule.Instance.JSFiles.TryGetValue(path, out content) ? content : string.Empty;
        }

        private bool InResources(string path, out string text)
        {
            var res = Resources.Load<TextAsset>(path);
            if (res != null)
            {
                text = res.text;
                return true;
            }
            else
            {
                text = string.Empty;
                return false;
            }
        }

        private string FormatFilePath(string filepath)
        {
            return filepath.EndsWith(".cjs") || filepath.EndsWith(".mjs") ? filepath.Substring(0, filepath.Length - 4) : filepath;
        }
    }
}
