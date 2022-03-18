import { $generic, $typeof, $promise } from 'puerts';
import { Modules, UnityEngine } from 'csharp';
import { FairyGUI } from 'csharp';

import UI_About from './FGUI/Common/UI_About';
import CommonBinder from './FGUI/Common/CommonBinder';

require('./puerts-source-map');

console.log("hello ts");

UnityEngine.Debug.Log('hello world from u');

// GRT.GLog.Enabled=true;
// GRT.GLog.Log("hello world from g", 10);

async function new_texture(path: string) {
    let ptexture = await $promise(Modules.AssetsManager.Instance.LoadTexture(path, null));

    let image = FairyGUI.UIObjectFactory.NewObject(FairyGUI.ObjectType.Image) as FairyGUI.GImage;
    image.texture = new FairyGUI.NTexture(ptexture);

    FairyGUI.GRoot.inst.AddChild(image);
}

let path =
//*/
'Assets/AddressablesRoot/Textures/UnitySplash-cube.png'; // default
/*/
'Assets/AddressablesRoot/Textures/U_Logo_T1_MadeWith_Small_White_RGB.png'; // update
//*/

new_texture(path);

console.log('按空格打开About窗口');
FairyGUI.Stage.inst.onKeyDown.Add(context => {
    if (context.inputEvent.keyCode == UnityEngine.KeyCode.Space) {
        openAbout();
    }
});

class About {
    public ui: UI_About;
    constructor(ui: UI_About) {
        this.ui = ui;
        FairyGUI.GRoot.inst.AddChild(ui);
        ui.m_btn_close.onClick.Set(() => { ui.displayObject.gameObject.SetActive(false); });
    }
}

let about: About;
async function openAbout() {
    let pkg = await $promise(Modules.FGUI.AssetsUtil.AddAddressablePackage('Common'));
    CommonBinder.bind();
    if (about == null) {
        about = new About(UI_About.createInstance());
        about.ui.Center();
    }
    else {
        about.ui.displayObject.gameObject.SetActive(true);
        about.ui.Center();
    }
}

console.log(new Error("test for ts source map").stack);
