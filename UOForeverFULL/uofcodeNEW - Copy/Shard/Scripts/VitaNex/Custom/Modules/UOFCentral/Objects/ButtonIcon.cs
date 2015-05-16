#region References
#endregion

namespace VitaNex.Modules.UOFCentral
{
	public enum ButtonIcon
	{
		Okay = 0,
		Cancel = 1,
		Previous = 2,
		Next = 3,
		Add = 4,
		Remove = 5,
		Clear = 6,
		Edit = 7,
		Apply = 8,
		Properties = 9
	}

	public class ButtonIconInfo
	{
		public static ButtonIconInfo[] Table { get; private set; }

		static ButtonIconInfo()
		{
			Table = new[]
			{
				new ButtonIconInfo(ButtonIcon.Okay, "Okay", 3000093, 4024, 4025),
				new ButtonIconInfo(ButtonIcon.Cancel, "Cancel", 1006045, 4018, 4019),
				new ButtonIconInfo(ButtonIcon.Previous, "Previous", 1043354, 4015, 4016),
				new ButtonIconInfo(ButtonIcon.Next, "Next", 1043353, 4006, 4007),
				new ButtonIconInfo(ButtonIcon.Add, "Add", 1079279, 4009, 4010),
				new ButtonIconInfo(ButtonIcon.Remove, "Remove", 1011403, 4003, 4004),
				new ButtonIconInfo(ButtonIcon.Clear, "Clear", 3000154, 4021, 4022),
				new ButtonIconInfo(ButtonIcon.Edit, "Edit", 3005101, 4027, 4028),
				new ButtonIconInfo(ButtonIcon.Apply, "Apply", 3000090, 4030, 4031),
				new ButtonIconInfo(ButtonIcon.Remove, "Properties", 1062761, 4012, 4013)
			};
		}

		public static ButtonIconInfo GetInfo(ButtonIcon icon)
		{
			return Table[(int)icon];
		}

		public static string GetLabel(ButtonIcon icon)
		{
			return GetInfo(icon).Label;
		}

		public static int GetTooltip(ButtonIcon icon)
		{
			return GetInfo(icon).Tooltip;
		}

		public ButtonIcon Icon { get; private set; }
		public string Label { get; private set; }
		public int Tooltip { get; private set; }
		public int Normal { get; private set; }
		public int Pressed { get; private set; }

		private ButtonIconInfo(ButtonIcon icon, string label, int tooltip, int nID, int pID)
		{
			Icon = icon;
			Label = label;
			Tooltip = tooltip;
			Normal = nID;
			Pressed = pID;
		}
	}
}