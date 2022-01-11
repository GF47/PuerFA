/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

import { FairyGUI } from "csharp";

export default class UI_About extends FairyGUI.GComponent {

	public m_bg: FairyGUI.GGraph;
	public m_title: FairyGUI.GTextField;
	public m_btn_close: FairyGUI.GButton;
	public m_content: FairyGUI.GRichTextField;
	public static URL: string = "ui://209wp1ljmun019";

	public static createInstance(): UI_About {
		return <UI_About>(FairyGUI.UIPackage.CreateObject("Common", "About"));
	}

	constructor() {
		super();
		this.__onConstruct = () => {
			this.m_bg = <FairyGUI.GGraph>(this.GetChildAt(0));
			this.m_title = <FairyGUI.GTextField>(this.GetChildAt(1));
			this.m_btn_close = <FairyGUI.GButton>(this.GetChildAt(2));
			this.m_content = <FairyGUI.GRichTextField>(this.GetChildAt(3));
		};
	}
}