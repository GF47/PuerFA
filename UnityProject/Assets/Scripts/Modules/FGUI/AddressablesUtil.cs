using FairyGUI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Modules.FGUI
{
    /// <summary>
    /// [ FairyGUI ] 工具类
    /// </summary>
    public static class AddressablesUtil
    {
        /// <summary>
        /// 默认的 [ FairyGUI ] 资源根目录
        /// </summary>
        public const string DEFAULT_PACKAGE_ROOT = "Assets/AssetBundlesRoot/FGUI"; // TODO 保持与fgui资源的根地址一致 //

        private static SortedList<string, AsyncOperationHandle> _handles;

        static AddressablesUtil()
        {
            _handles = new SortedList<string, AsyncOperationHandle>();

            NTexture.CustomDestroyMethod += texture => Addressables.Release(texture);
            NAudioClip.CustomDestroyMethod += audio => Addressables.Release(audio);
        }

        /// <summary>
        /// 加载资源包
        /// </summary>
        /// <param name="package">[ FairyGUI ] 资源包名</param>
        /// <param name="prefix">prefix of the resource file name. The file name would be in format of 'assetNamePrefix_resFileName'. It can be empty.</param>
        /// <param name="isFullPath">包名参数是否为完整的 [ Addressables ] 路径</param>
        public static async Task<UIPackage> AddUIPackage(string package, string prefix = "", bool isFullPath = false)
        {
            var pkg = UIPackage.GetByName(package);

            if (pkg == null)
            {
                var descHandle = Addressables.LoadAssetAsync<TextAsset>(isFullPath
                    ? package
                    : $"{DEFAULT_PACKAGE_ROOT}/{package}_fui.bytes");

                var desc = await descHandle.Task;

                // desc.bytes : description file data
                // prefix     : prefix of the resource file name. The file name would be in format of 'assetNamePrefix_resFileName'. It can be empty.
                // name       : asset name, without DEFAULT_PACKAGE_ROOT and extension
                // extension  : ".png" and so on
                // type       : asset type like "Texture", "AudioClip"
                // item       : FairyGUI asset object
                pkg = UIPackage.AddPackage(desc.bytes, prefix, async (name, extension, type, item) =>
                {
                    if (type == typeof(Texture))
                    {
                        var handle = Addressables.LoadAssetAsync<Texture>(isFullPath
                            ? package.Replace("fui.bytes", $"{name}{extension}")
                            : $"{DEFAULT_PACKAGE_ROOT}/{package}_{name}{extension}");
                        var texture = await handle.Task;
                        // 谷大让加上这句(
                        item.owner.SetItemAsset(item, texture, DestroyMethod.Custom);
                    }
                    else if (type == typeof(AudioClip))
                    {
                        var handle = Addressables.LoadAssetAsync<AudioClip>(isFullPath
                            ? package.Replace("fui.bytes", $"{name}{extension}")
                            : $"{DEFAULT_PACKAGE_ROOT}/{package}_{name}{extension}");
                        var clip = await handle.Task;
                        // 谷大让加上这句(
                        item.owner.SetItemAsset(item, clip, DestroyMethod.Custom);
                    }
                });

                Addressables.Release(desc);
            }

            return pkg;
        }

        /// <summary>
        /// 在FGUI和Addressable中移除指定的包
        /// </summary>
        /// <param name="packageOrID">包名或者包ID</param>
        public static void RemoveUIPackage(string packageOrID)
        {
            UIPackage.RemovePackage(packageOrID);
        }

        public static async Task<T> Load<T>(string path, Action<T> callback) where T : UnityEngine.Object
        {
            var handle = Addressables.LoadAssetAsync<T>(path);
            _handles.Add(path, handle);
            await handle.Task;
            callback?.Invoke(handle.Result);
            return handle.Result;
        }

        public static async Task<Texture> LoadTexture(string path, Action<Texture> callback) => await Load<Texture>(path, callback);

        public static async Task<AudioClip> LoadAudioClip(string path, Action<AudioClip> callback) => await Load<AudioClip>(path, callback);

        public static void Release(string path)
        {
            if (_handles.TryGetValue(path, out var handle))
            {
                Addressables.Release(handle);
                _handles.Remove(path);
            }
            else
            {
                throw new ArgumentException("asset path not found in addressables system", "path");
            }
        }

        public static void Release<T>(T asset)
        {
            int k = -1;
            for (int i = 0; i < _handles.Values.Count; i++)
            {
                var handle = _handles.Values[i];
                if (handle.Result.Equals(asset))
                {
                    Addressables.Release(handle);
                    k = i;
                    break;
                }
            }
            if (k < 0)
            {
                throw new ArgumentException("asset not found in addressables system", "asset");
            }
            _handles.RemoveAt(k);
        }
    }
}