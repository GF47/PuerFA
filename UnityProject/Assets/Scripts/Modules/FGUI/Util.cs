using FairyGUI;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
        public static string DEFAULT_PACKAGE_ROOT = "Assets/AssetBundlesRoot/FGUI"; // TODO 保持与fgui资源的根地址一致 //

        static Util()
        {
            NTexture.CustomDestroyMethod += texture => Addressables.Release(texture);
            NAudioClip.CustomDestroyMethod += audio => Addressables.Release(audio);
        }

        /// <summary>
        /// 加载资源包
        /// </summary>
        /// <param name="package">[ FairyGUI ] 资源包名</param>
        /// <param name="prefix">prefix of the resource file name. The file name would be in format of 'assetNamePrefix_resFileName'. It can be empty.</param>
        /// <param name="isFullPath">包名参数是否为完整的 [ Addressables ] 路径</param>
        public static async Task<UIPackage> AddAddressablePackage(string package, string prefix = "", bool isFullPath = false)
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
        public static void RemoveAddressablePackage(string packageOrID)
        {
            UIPackage.RemovePackage(packageOrID);
        }
    }
}