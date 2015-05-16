#region References
using Server;
#endregion

namespace VitaNex.Modules.UOFCentral
{
	public sealed class UOFCentralOptions : CoreModuleOptions
	{
		private string _PopupCommand;

		[CommandProperty(UOFCentral.Access)]
		public string PopupCommand
		{
			//
			get { return _PopupCommand; }
			set { CommandUtility.Replace(_PopupCommand, AccessLevel.Player, UOFCentral.HandlePopupCommand, (_PopupCommand = value)); }
		}

		[CommandProperty(UOFCentral.Access)]
		public bool LoginPopup { get; set; }

		[CommandProperty(UOFCentral.Access)]
		public bool VirtuePopup { get; set; }

		[CommandProperty(UOFCentral.Access)]
		public AccessLevel EditAccess { get; set; }

		public UOFCentralOptions()
			: base(typeof(UOFCentral))
		{
			PopupCommand = "UOF";
			LoginPopup = false;
			VirtuePopup = true;
			EditAccess = UOFCentral.Access;
		}

		public UOFCentralOptions(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			base.Clear();

			PopupCommand = null;
			LoginPopup = false;
			VirtuePopup = true;
			EditAccess = UOFCentral.Access;
		}

		public override void Reset()
		{
			base.Reset();

			PopupCommand = "UOF";
			LoginPopup = false;
			VirtuePopup = true;
			EditAccess = UOFCentral.Access;
		}

		public override string ToString()
		{
			return "UOFCentral Config";
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					{
						writer.Write(PopupCommand);
						writer.Write(LoginPopup);
						writer.Write(VirtuePopup);
						writer.WriteFlag(EditAccess);
					}
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
				case 0:
					{
						PopupCommand = reader.ReadString();
						LoginPopup = reader.ReadBool();
						VirtuePopup = reader.ReadBool();
						EditAccess = reader.ReadFlag<AccessLevel>();
					}
					break;
			}
		}
	}
}