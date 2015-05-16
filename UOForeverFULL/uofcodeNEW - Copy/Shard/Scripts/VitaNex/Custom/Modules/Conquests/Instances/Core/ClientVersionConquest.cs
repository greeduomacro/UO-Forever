#region References

using Server.Mobiles;
using VitaNex;
#endregion

namespace Server.Engines.Conquests
{
	public class ClientVersionConquest : Conquest
	{
		public override string DefCategory { get { return "Misc"; } }

		public virtual string DefVersion { get { return "7.0.15.0"; } }

		[CommandProperty(Conquests.Access)]
		public VersionInfo Version { get; set; }

		public ClientVersionConquest()
		{ }

		public ClientVersionConquest(GenericReader reader)
			: base(reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			Version = DefVersion;
		}

		public override sealed int GetProgress(ConquestState state, object args)
		{
			return GetProgress(state, args as ClientVersionReceivedArgs);
		}

		protected virtual int GetProgress(ConquestState state, ClientVersionReceivedArgs args)
		{
            if (state.User == null)
                return 0;

            if (args == null || args.State == null || args.State.Mobile is PlayerMobile && args.State.Mobile.Account != state.User.Account || args.Version == null)
			{
				return 0;
			}

			if (Version != null)
			{
				VersionInfo v = args.Version.ToString();

				if (v < Version)
				{
					return 0;
				}
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
					Version.Serialize(writer);
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
					Version = new VersionInfo(reader);
					break;
			}
		}
	}
}