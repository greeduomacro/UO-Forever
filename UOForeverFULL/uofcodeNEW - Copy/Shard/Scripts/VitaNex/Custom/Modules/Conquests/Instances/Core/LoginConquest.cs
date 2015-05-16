using System;
using Server.Mobiles;

namespace Server.Engines.Conquests
{
	public class LoginConquest : Conquest
	{
		public override string DefCategory { get { return "Activity"; } }

		public virtual string DefRegion { get { return null; } }
		public virtual Map DefMap { get { return null; } }

		public virtual bool DefRegionChangeReset { get { return false; } }
		public virtual bool DefMapChangeReset { get { return false; } }

		[CommandProperty(Conquests.Access)]
		public string Region { get; set; }

		[CommandProperty(Conquests.Access)]
		public Map Map { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool RegionChangeReset { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool MapChangeReset { get; set; }

		public LoginConquest()
		{ }

		public LoginConquest(GenericReader reader)
			: base(reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			Region = DefRegion;
			Map = DefMap;

			RegionChangeReset = DefRegionChangeReset;
			MapChangeReset = DefMapChangeReset;
		}

		public override sealed int GetProgress(ConquestState state, object args)
		{
			return GetProgress(state, args as LoginEventArgs);
		}

		protected virtual int GetProgress(ConquestState state, LoginEventArgs args)
		{
            if (state.User == null)
                return 0;

            if (args == null || args.Mobile is PlayerMobile && args.Mobile.Account != state.User.Account)
			{
				return 0;
			}

			if (Map != null && Map != Map.Internal && (args.Mobile.Map == null || args.Mobile.Map != Map))
			{
				if (MapChangeReset)
				{
					return -state.Progress;
				}

				return 0;
			}

			if (!String.IsNullOrWhiteSpace(Region) && (args.Mobile.Region == null || !args.Mobile.Region.IsPartOf(Region)))
			{
				if (RegionChangeReset)
				{
					return -state.Progress;
				}

				return 0;
			}

			return 1;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
					{
						writer.Write(Region);
						writer.Write(Map);

						writer.Write(RegionChangeReset);
						writer.Write(MapChangeReset);
					}
					goto case 0;
				case 0:
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
				case 1:
					{
						Region = reader.ReadString();
						Map = reader.ReadMap();

						RegionChangeReset = reader.ReadBool();
						MapChangeReset = reader.ReadBool();
					}
					goto case 0;
				case 0:
					break;
			}
		}
	}
}