using Puerts;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Modules.JS
{
    public partial class JSModule : MonoBehaviour
    {
        /// <summary>
        /// Puerts 引擎
        /// </summary>
        public static JSModule Instance { get; private set; }

        /// <summary>
        /// js 脚本字典
        /// </summary>
        public Dictionary<string, string> JSFiles { get; private set; }

        /// <summary>
        /// js 运行环境
        /// </summary>
        public JsEnv Env { get; private set; }

        /// <summary>
        /// 是否已经加载完毕
        /// </summary>
        public bool Ready { get; private set; }

        private void Awake()
        {
            Instance = this;

            JSFiles = new Dictionary<string, string>();

            // TODO 调试
            // Env = new JsEnv(new DefaultLoader($"tsproj output"), 8080);
            // Env.WaitDebugger();

            Env = new JsEnv(new AddressableLoader());

            Using(Env); // 先于js环境

            LoadScripts();
        }

        private void OnDestroy() => Env.Dispose();

        private void Update() => Env.Tick();

        private async void LoadScripts()
        {
            var root = "Assets/AssetBundlesRoot/Scripts/";
            var rootLength = root.Length;
            var ext = ".txt";
            var extLength = ext.Length;

            var tasks = new List<Task>();

#if UNITY_EDITOR
            await Addressables.InitializeAsync().Task;
#endif

            foreach (var locator in Addressables.ResourceLocators)
            {
                if (locator.Locate("Scripts", typeof(TextAsset), out var locations)) // 脚本需要被标记为 Scripts
                {
                    if (locations != null)
                    {
                        foreach (var location in locations)
                        {
                            // Debug.Log($"{location.ProviderId}\n{location.PrimaryKey}\n{location.InternalId}\n{location.Data}");

                            var path = location.InternalId;
                            if (path.EndsWith("js.map.txt")) { continue; } // 忽略map文件

                            var operation = Addressables.LoadAssetAsync<TextAsset>(location);
                            operation.Completed += op =>
                            {
                                JSFiles.Add(path.Substring(rootLength, path.Length - rootLength - extLength), op.Result.text);
                            };

                            tasks.Add(operation.Task);
                        }
                    }
                }
            }

            await Task.WhenAll(tasks);

            Ready = true;
        }
    }
}