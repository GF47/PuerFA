/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

import { FairyGUI } from "csharp";

export default class UI_View extends FairyGUI.GComponent {

	public m_bg: FairyGUI.GGraph;
	public m_title: FairyGUI.GTextField;
	public m_message: FairyGUI.GTextField;
	public m_progress: FairyGUI.GProgressBar;
	public m_btnUpdate: FairyGUI.GButton;
	public m_btnCancel: FairyGUI.GButton;
	public m_btn: FairyGUI.GGroup;
	public static URL: string = "ui://2ddcaxpyncah1";

	public static createInstance(): UI_View {
		return <UI_View>(FairyGUI.UIPackage.CreateObject("RemoteUpdate", "View"));
	}

	constructor() {
		super();
		this.__onConstruct = () => {
			this.m_bg = <FairyGUI.GGraph>(this.GetChildAt(0));
			this.m_title = <FairyGUI.GTextField>(this.GetChildAt(1));
			this.m_message = <FairyGUI.GTextField>(this.GetChildAt(2));
			this.m_progress = <FairyGUI.GProgressBar>(this.GetChildAt(3));
			this.m_btnUpdate = <FairyGUI.GButton>(this.GetChildAt(4));
			this.m_btnCancel = <FairyGUI.GButton>(this.GetChildAt(5));
			this.m_btn = <FairyGUI.GGroup>(this.GetChildAt(6));
		};
	}
}