using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Modules
{
    public class AssetsManager
    {
        #region Singleton
        public static AssetsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AssetsManager();
                }
                return _instance;
            }
        }
        private static AssetsManager _instance;
        #endregion Singleton

        Dictionary<string, AsyncOperationHandle> _dict;

        public AssetsManager()
        {
            _dict = new Dictionary<string, AsyncOperationHandle>();
        }

        public async Task<T> Load<T>(string path) where T : UnityEngine.Object
        {
            var handle = Addressables.LoadAssetAsync<T>(path);
            _dict.Add(path, handle);
            await handle.Task;
            return handle.Result;
        }

        public async Task<Texture> LoadTexture(string path) => await Load<Texture>(path);
        public async Task<AudioClip> LoadAudioClip(string path) => await Load<AudioClip>(path);

        public bool Release(string path)
        {
            if (_dict.TryGetValue(path, out var handle))
            {
                Addressables.Release(handle);
                _dict.Remove(path);
                return true;
            }
            return false;
        }
    }
}
