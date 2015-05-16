#region References
using System;

using Server.Commands;
using Server.Mobiles;

#endregion

namespace Server.Engines.Conquests
{
	public class CommandConquest : Conquest
	{
		public override string DefCategory { get { return "Misc"; } }

		private string _Command;

		[CommandProperty(Conquests.Access)]
		public string Command
		{
			get { return _Command; }
			set
			{
				if (!CommandSystem.Entries.ContainsKey(value))
				{
					value = null;
				}

				_Command = value;
			}
		}

		public CommandConquest()
		{ }

		public CommandConquest(GenericReader reader)
			: base(reader)
		{ }

		public override sealed int GetProgress(ConquestState state, object args)
		{
			return GetProgress(state, args as CommandEventArgs);
		}

		protected virtual int GetProgress(ConquestState state, CommandEventArgs args)
		{
            if (state.User == null)
                return 0;

            if (args == null || args.Mobile is PlayerMobile && args.Mobile.Account != state.User.Account)
			{
				return 0;
			}

			if (!String.IsNullOrWhiteSpace(Command) && !Insensitive.Equals(args.Command, Command))
			{
				return 0;
			}

			return 1;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.Write(_Command);
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
					_Command = reader.ReadString();
					break;
			}
		}
	}
}