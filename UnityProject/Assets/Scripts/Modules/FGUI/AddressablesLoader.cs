using FairyGUI;

namespace Modules.FGUI
{
    public class AddressablesLoader : GLoader
    {
        protected override void LoadExternal()
        {
            /*
            开始外部载入，地址在url属性
            载入完成后调用OnExternalLoadSuccess
            载入失败调用OnExternalLoadFailed

            注意：如果是外部载入，在载入结束后，调用OnExternalLoadSuccess或OnExternalLoadFailed前，
            比较严谨的做法是先检查url属性是否已经和这个载入的内容不相符。
            如果不相符，表示loader已经被修改了。
            这种情况下应该放弃调用OnExternalLoadSuccess或OnExternalLoadFailed。
            */

#if FAIRYGUI_PUERTS
            if (__loadExternal != null)
            {
                __loadExternal();
                return;
            }
#endif

            LoadTexture(url);
        }

        private async void LoadTexture(string url)
        {
            var tex = await AddressablesUtil.LoadTexture(url, null);
            if (tex != null)
            {
                onExternalLoadSuccess(new NTexture(tex));
            }
            else
            {
                onExternalLoadFailed();
            }
        }

        protected override void FreeExternal(NTexture texture)
        {
#if FAIRYGUI_PUERTS
            if (__freeExternal != null)
            {
                __freeExternal(texture);
                return;
            }
#endif

            AddressablesUtil.Release(texture.nativeTexture);
            if (texture.alphaTexture != null)
            {
                AddressablesUtil.Release(texture.alphaTexture);
            }
        }
    }
}