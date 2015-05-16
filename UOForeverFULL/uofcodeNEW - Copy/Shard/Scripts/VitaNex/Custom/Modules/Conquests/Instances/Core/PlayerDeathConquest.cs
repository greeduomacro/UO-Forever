using Server.Mobiles;
using Server.Regions;

namespace Server.Engines.Conquests
{
	public class PlayerDeathConquest : Conquest
	{
		public override string DefCategory { get { return "Dirt Naps"; } }

        public virtual bool DefIsDungeon { get { return false; } }
        public virtual bool DefIsDuel { get { return false; } }


        [CommandProperty(Conquests.Access)]
        public bool IsDungeon { get; set; }

        [CommandProperty(Conquests.Access)]
        public bool IsDuel { get; set; }

		public PlayerDeathConquest()
		{ }

		public PlayerDeathConquest(GenericReader reader)
			: base(reader)
		{ }

        public override void EnsureDefaults()
        {
            base.EnsureDefaults();

            IsDungeon = DefIsDungeon;
            IsDuel = DefIsDuel;
        }

		public override sealed int GetProgress(ConquestState state, object args)
		{
			return GetProgress(state, args as PlayerDeathEventArgs);
		}

		protected virtual int GetProgress(ConquestState state, PlayerDeathEventArgs args)
		{
            if (state.User == null)
                return 0;

            if (args == null || args.Mobile is PlayerMobile && args.Mobile.Account != state.User.Account)
			{
				return 0;
			}

            if (IsDuel && args.Killer is PlayerMobile && ((PlayerMobile)args.Killer).DuelContext == null || IsDuel && !(args.Killer is PlayerMobile))
            {
                return 0;
            }

            if (IsDungeon && !args.Killer.InRegion<DungeonRegion>())
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
			    {
                    writer.Write(IsDuel);
                    writer.Write(IsDungeon);
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
                    IsDuel = reader.ReadBool();
                    IsDungeon = reader.ReadBool();
			    }
					break;
			}
		}
	}
}