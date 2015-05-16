#region References
using System;
#endregion

namespace VitaNex.Modules.UOFCentral
{
	public struct ToolbarButton
	{
		public ButtonIconInfo Icon { get; private set; }
		public Action Callback { get; private set; }

		public ToolbarButton(ButtonIcon icon, Action cb)
			: this()
		{
			Icon = ButtonIconInfo.GetInfo(icon);
			Callback = cb;
		}
	}
}