using System;
using Server.Mobiles;
using VitaNex;
using VitaNex.Modules.AutoPvP;

namespace Server.Engines.Conquests
{
	public class ResConquest : Conquest
	{
		public override string DefCategory { get { return "Resurrection"; } }

        public bool DefIsPlayer { get { return false; } }
        public bool DefLastKillerCheck { get { return false; } }
        public bool DefIsCreature { get { return false; } }

        public ResConquest()
		{ }

        public ResConquest(GenericReader reader)
			: base(reader)
		{ }

        [CommandProperty(Conquests.Access)]
        public bool IsPlayer { get; set; }

        [CommandProperty(Conquests.Access)]
        public bool LastKillerCheck { get; set; }

        [CommandProperty(Conquests.Access)]
        public bool IsCreature { get; set; }


		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

		    IsPlayer = DefIsPlayer;
		    LastKillerCheck = DefLastKillerCheck;
		    IsCreature = DefIsCreature;
		}

		public override sealed int GetProgress(ConquestState state, object args)
		{
			return GetProgress(state, args as Mobile);
		}

		protected virtual int GetProgress(ConquestState state, Mobile mobile)
		{
			if (mobile == null || state.User == null)
			{
				return 0;
			}

            if (LastKillerCheck && mobile.LastKiller == null || LastKillerCheck && mobile is PlayerMobile && mobile.LastKiller is PlayerMobile && mobile.LastKiller.Account == state.User.Account || LastKillerCheck && !(mobile is PlayerMobile))
		    {
		        return 0;
		    }

            if (IsPlayer && !(mobile is PlayerMobile))
            {
                return 0;
            }

            if (IsCreature && !(mobile is BaseCreature))
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
                        writer.Write(LastKillerCheck);
                        writer.Write(IsPlayer);
                        writer.Write(IsCreature);
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
			        LastKillerCheck = reader.ReadBool();
                    IsPlayer = reader.ReadBool();
                    IsCreature = reader.ReadBool();
			    }
			        break;
			}
		}
	}
}