/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

import { FairyGUI } from "csharp";

import UI_About from "./UI_About";
import UI_ProgressBar from "./UI_ProgressBar";

export default class CommonBinder {
	public static bind(): void {
		FairyGUI.UIObjectFactory.SetPackageItemExtension(UI_About.URL, () => new UI_About());
		FairyGUI.UIObjectFactory.SetPackageItemExtension(UI_ProgressBar.URL, () => new UI_ProgressBar());
	}
}
