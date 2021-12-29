using Puerts;
using System.Collections.Generic;
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
        public bool State { get; private set; }

        private void Awake()
        {
            Instance = this;

            JSFiles = new Dictionary<string, string>();

            // TODO 调试
            // Env = new JsEnv(new DefaultLoader($"tsproj output"), 8080);
            // Env.WaitDebugger();

            Env = new JsEnv(new AddressableLoader());

            Using(Env); // 先于js环境

            var operation = Addressables.LoadAssetsAsync<TextAsset>("Scripts", null); // 脚本需要被标记为 Scripts
            operation.Completed += list =>
            {
                foreach (var item in list.Result)
                {
                    JSFiles.Add(item.name, item.text);
                }

                State = true;
            };
        }

        private void OnDestroy()
        {
            Env.Dispose();
        }

        private void Update()
        {
            if (State)
            {
                Env.Tick();
            }
        }
    }
}