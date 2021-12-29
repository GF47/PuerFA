using FairyGUI;
using System;
using UnityEngine;

namespace Modules
{
    public class Launcher
    {
        public async void Init(Action completed)
        {
            var commonPkg = await FGUI.Util.AddAddressablePackage("Common");
            var remoteUpdatePkg = await FGUI.Util.AddAddressablePackage("RemoteUpdate");

            var obj = remoteUpdatePkg.CreateObject("View");
            var view = GRoot.inst.AddChild(obj).asCom;

            var title = view.GetChild("title").asTextField;
            title.text = "远程更新";

            var message = view.GetChild("message").asTextField;
            var progress = view.GetChild("progress").asProgress;
            var btnUpdate = view.GetChild("btnUpdate").asButton;
            var btnCancel = view.GetChild("btnCancel").asButton;

            var updater = new RemoteAssetsUpdater();
            updater.InfoChanging += msg => message.text = msg;
            updater.PercentChanging += p => progress.value = p;
            btnUpdate.onClick.Add(() => updater.StartUpdate());
            btnCancel.onClick.Add(() => Application.Quit());

            await updater.Init();

            message.text = "更新完毕";

            btnUpdate.onClick.Clear();

            btnUpdate.GetChild("title").asTextField.text = "进入";
            btnUpdate.onClick.Add(() =>
            {
                view.TweenFade(0f, 1f).OnComplete(() =>
                {
                    view.Dispose();
                    FGUI.Util.RemoveAddressablePackage(remoteUpdatePkg.id);

                    completed?.Invoke();
                });
            });
        }
    }
}