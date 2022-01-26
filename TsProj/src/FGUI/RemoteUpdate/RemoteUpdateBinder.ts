/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

import { FairyGUI } from "csharp";

import UI_View from "./UI_View";

export default class RemoteUpdateBinder {
	public static bind(): void {
		FairyGUI.UIObjectFactory.SetPackageItemExtension(UI_View.URL, () => new UI_View());
	}
}
