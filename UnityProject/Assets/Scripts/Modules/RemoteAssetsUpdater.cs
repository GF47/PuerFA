using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Modules
{
    /// <summary>
    /// 远程资源更新
    /// </summary>
    public class RemoteAssetsUpdater
    {
        private string info;
        private int percent;
        private TaskCompletionSource<bool> updating;
        public Action Completed;
        public Action<string> InfoChanging;
        public Action<int> PercentChanging;

        /// <summary>
        /// 更新信息
        /// </summary>
        public string Info
        {
            get => info; private set
            {
                info = value;
                InfoChanging?.Invoke(info);
            }
        }

        /// <summary>
        /// 更新进度
        /// </summary>
        public int Percent
        {
            get => percent; private set
            {
                percent = value;
                PercentChanging?.Invoke(percent);
            }
        }

        /// <summary>
        /// 准备更新
        /// </summary>
        public async Task Init()
        {
            var stopwatch = Stopwatch.StartNew();

            await Addressables.InitializeAsync().Task;

            Info = "updating catalogs";
            Percent = 0;

            var catalogsOperation = Addressables.CheckForCatalogUpdates(false);
            var catalogs = await catalogsOperation.Task;

            Info = $"catalogs got in {stopwatch.ElapsedMilliseconds}ms";
            Percent = 100;

            if (catalogs != null && catalogs.Count > 0)
            {
                Info = $"assets need to be updated";
                Percent = 0;

                updating = new TaskCompletionSource<bool>();
                await updating.Task;

                stopwatch.Restart();

                var locatorsOperation = Addressables.UpdateCatalogs(catalogs, false);
                var locators = await locatorsOperation.Task;

                Info = $"updating assets...\ncount: {locators.Count}";

                for (int i = 0; i < locators.Count; i++)
                {
                    var downloaderOperation = Addressables.DownloadDependenciesAsync(locators[i].Keys, Addressables.MergeMode.Union);
                    var downloader = await downloaderOperation.Task;

                    Addressables.Release(downloaderOperation);

                    Percent = i * 100 / locators.Count;
                }

                Percent = 100;

                Addressables.Release(locatorsOperation);
            }

            Addressables.Release(catalogsOperation);

            Completed?.Invoke();
        }

        public void StartUpdate()
        {
            updating?.SetResult(true);
        }
    }
}