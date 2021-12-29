using FairyGUI;
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
    public static class Util
    {
        /// <summary>
        /// 默认的 [ FairyGUI ] 资源根目录
        /// </summary>
        public static string DEFAULT_PACKAGE_ROOT = "Assets/AssetBundlesRoot/FGUI";

        private static Dictionary<string, AsyncOperationHandle<TextAsset>> _pkghandles = new Dictionary<string, AsyncOperationHandle<TextAsset>>();
        private static Dictionary<string, List<AsyncOperationHandle<Texture>>> _texHandles = new Dictionary<string, List<AsyncOperationHandle<Texture>>>();
        private static Dictionary<string, List<AsyncOperationHandle<AudioClip>>> _audioHandles = new Dictionary<string, List<AsyncOperationHandle<AudioClip>>>();

        /// <summary>
        /// 加载资源包
        /// </summary>
        /// <param name="package">[ FairyGUI ] 资源包名</param>
        /// <param name="prefix">prefix of the resource file name. The file name would be in format of 'assetNamePrefix_resFileName'. It can be empty.</param>
        /// <param name="isFullPath">包名参数是否为完整的 [ Addressables ] 路径</param>
        public static async Task<UIPackage> AddAddressablePackage(string package, string prefix = "", bool isFullPath = false)
        {
            if (_pkghandles.ContainsKey(package)) { return UIPackage.GetByName(package); }

            var descHandle = Addressables.LoadAssetAsync<TextAsset>(isFullPath
                ? package
                : $"{DEFAULT_PACKAGE_ROOT}/{package}_fui.bytes");
            _pkghandles.Add(package, descHandle);

            var desc = await descHandle.Task;

            // desc.bytes : description file data
            // prefix     : prefix of the resource file name. The file name would be in format of 'assetNamePrefix_resFileName'. It can be empty.
            // name       : asset name, without DEFAULT_PACKAGE_ROOT and extension
            // extension  : ".png" and so on
            // type       : asset type like "Texture", "AudioClip"
            // item       : FairyGUI asset object
            return UIPackage.AddPackage(desc.bytes, prefix, async (name, extension, type, item) =>
            {
                if (type == typeof(Texture))
                {
                    var handle = Addressables.LoadAssetAsync<Texture>(isFullPath
                        ? package.Replace("fui.bytes", $"{name}{extension}")
                        : $"{DEFAULT_PACKAGE_ROOT}/{package}_{name}{extension}");
                    GetTextureHandles(package).Add(handle);
                    var texture = await handle.Task;
                    // 谷大让加上这句(
                    item.owner.SetItemAsset(item, texture, DestroyMethod.None);
                }
                else if (type == typeof(AudioClip))
                {
                    var handle = Addressables.LoadAssetAsync<AudioClip>(isFullPath
                        ? package.Replace("fui.bytes", $"{name}{extension}")
                        : $"{DEFAULT_PACKAGE_ROOT}/{package}_{name}{extension}");
                    GetAudioClipHandles(package).Add(handle);
                    var clip = await handle.Task;
                    // 谷大让加上这句(
                    item.owner.SetItemAsset(item, clip, DestroyMethod.None);
                }
            });
        }

        /// <summary>
        /// 在FGUI和Addressable中移除指定的包
        /// </summary>
        /// <param name="packageOrID">包名或者包ID</param>
        public static void RemoveAddressablePackage(string packageOrID)
        {
            var package = UIPackage.GetById(packageOrID);
            var packageName = package == null ? packageOrID : package.name;

            UIPackage.RemovePackage(packageOrID);

            if (!string.IsNullOrEmpty(packageName))
            {
                if (_pkghandles.TryGetValue(packageName, out var pkgHandle))
                {
                    Addressables.Release(pkgHandle);
                    _pkghandles.Remove(packageName);
                }

                if (_texHandles.TryGetValue(packageName, out var texHandles))
                {
                    foreach (var handle in texHandles)
                    {
                        Addressables.Release(handle);
                    }

                    _texHandles.Remove(packageName);
                }

                if (_audioHandles.TryGetValue(packageName, out var audioHandles))
                {
                    foreach (var handle in audioHandles)
                    {
                        Addressables.Release(handle);
                    }

                    _audioHandles.Remove(packageName);
                }
            }
        }

        /// <summary>
        /// 获取加载记录中的句柄列表
        /// </summary>
        /// <param name="pkgName">包名</param>
        private static List<AsyncOperationHandle<Texture>> GetTextureHandles(string pkgName)
        {
            List<AsyncOperationHandle<Texture>> handles;
            if (!_texHandles.TryGetValue(pkgName, out handles))
            {
                handles = new List<AsyncOperationHandle<Texture>>();
                _texHandles.Add(pkgName, handles);
            }
            return handles;
        }

        /// <summary>
        /// 获取加载记录中的句柄列表
        /// </summary>
        /// <param name="pkgName">包名</param>
        private static List<AsyncOperationHandle<AudioClip>> GetAudioClipHandles(string pkgName)
        {
            List<AsyncOperationHandle<AudioClip>> handles;
            if (!_audioHandles.TryGetValue(pkgName, out handles))
            {
                handles = new List<AsyncOperationHandle<AudioClip>>();
                _audioHandles.Add(pkgName, handles);
            }
            return handles;
        }
    }
}