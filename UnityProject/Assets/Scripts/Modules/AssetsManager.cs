using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
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

        private SortedList<string, AsyncOperationHandle> _handles;

        public AssetsManager()
        {
            _handles = new SortedList<string, AsyncOperationHandle>();
        }

        public async Task<T> Load<T>(string path, Action<T> callback) where T : UnityEngine.Object
        {
            if (_handles.ContainsKey(path))
            {
                var result = (T)_handles[path].Result;
                callback?.Invoke(result);
                return result;
            }
            var handle = Addressables.LoadAssetAsync<T>(path);
            _handles.Add(path, handle);
            await handle.Task;
            callback?.Invoke(handle.Result);
            return handle.Result;
        }

        public async Task<Texture> LoadTexture(string path, Action<Texture> callback) => await Load<Texture>(path, callback);

        public async Task<AudioClip> LoadAudioClip(string path, Action<AudioClip> callback) => await Load<AudioClip>(path, callback);

        public async Task<TextAsset> LoadText(string path, Action<TextAsset> callback) => await Load<TextAsset>(path, callback);

        public void Release(string path)
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

        public void Release<T>(T asset)
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