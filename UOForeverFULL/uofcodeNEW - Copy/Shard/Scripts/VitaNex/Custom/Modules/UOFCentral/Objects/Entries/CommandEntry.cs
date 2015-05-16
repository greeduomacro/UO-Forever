#region References
using System;
using System.Drawing;

using Server;
using Server.Commands;
using Server.Network;
#endregion

namespace VitaNex.Modules.UOFCentral
{
	public class CommandEntry : Entry
	{
		private Server.Commands.CommandEntry _Entry;

		private string _Command;

		[CommandProperty(UOFCentral.Access)]
		public string Command
		{
			get
			{
				if (!CommandSystem.Entries.TryGetValue(_Command, out _Entry))
				{
					_Command = String.Empty;
				}

				return _Command;
			}
			set
			{
				_Command = value;

				if (!CommandSystem.Entries.TryGetValue(_Command, out _Entry))
				{
					_Command = String.Empty;
				}
			}
		}

		public CommandEntry(Page parent)
			: base(parent)
		{ }

		public CommandEntry(Page parent, GenericReader reader)
			: base(parent, reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			Label = "New Command";
			LabelColor = KnownColor.SkyBlue;

			_Command = String.Empty;
		}

		public override bool Valid(UOFCentralGump g)
		{
			return base.Valid(g) && !String.IsNullOrWhiteSpace(Command) && (g.Edit || g.User.AccessLevel >= _Entry.AccessLevel);
		}

		public override void Select(UOFCentralGump g)
		{
			base.Select(g);

			if (!String.IsNullOrWhiteSpace(Command))
			{
				CommandSystem.Handle(g.User, Command, MessageType.Command);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.Write(_Command);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			_Command = reader.ReadString();
		}
	}
}