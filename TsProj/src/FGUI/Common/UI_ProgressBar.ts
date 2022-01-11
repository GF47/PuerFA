/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

import { FairyGUI } from "csharp";

export default class UI_ProgressBar extends FairyGUI.GProgressBar {

	public m_bg: FairyGUI.GGraph;
	public static URL: string = "ui://209wp1ljncah17";

	public static createInstance(): UI_ProgressBar {
		return <UI_ProgressBar>(FairyGUI.UIPackage.CreateObject("Common", "ProgressBar"));
	}

	constructor() {
		super();
		this.__onConstruct = () => {
			this.m_bg = <FairyGUI.GGraph>(this.GetChildAt(0));
		};
	}
}