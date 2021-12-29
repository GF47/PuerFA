import { $generic, $typeof, $promise } from 'puerts';
import { Modules, UnityEngine } from 'csharp';
import { FairyGUI } from 'csharp';

console.log("hello ts");

UnityEngine.Debug.Log('hello world from u');

// GRT.GLog.Enabled=true;
// GRT.GLog.Log("hello world from g", 10);

async function new_texture(path: string) {
    let ptexture = await $promise(Modules.AssetsManager.Instance.LoadTexture(path));

    let image = FairyGUI.UIObjectFactory.NewObject(FairyGUI.ObjectType.Image) as FairyGUI.GImage;
    image.texture = new FairyGUI.NTexture(ptexture);

    FairyGUI.GRoot.inst.AddChild(image);
}

let path = 
//*/
'Assets/AssetBundlesRoot/Textures/UnitySplash-cube.png'; // default
/*/
'Assets/AssetBundlesRoot/Textures/U_Logo_T1_MadeWith_Small_White_RGB.png'; // update
//*/

new_texture(path);

FairyGUI.Stage.inst.onKeyDown.Add(context => {
    if (context.inputEvent.keyCode == UnityEngine.KeyCode.Space) {
        console.log(Modules.AssetsManager.Instance.Release(path));
    }
});