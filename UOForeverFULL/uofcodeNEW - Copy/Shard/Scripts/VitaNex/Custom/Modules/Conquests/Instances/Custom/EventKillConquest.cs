using System;
using Server.Mobiles;
using VitaNex;
using VitaNex.Modules.AutoPvP;

namespace Server.Engines.Conquests
{
	public class BattleKillConquest : Conquest
	{
		public override string DefCategory { get { return "PvP"; } }

        public virtual Type DefBattle { get { return null; } }
        public virtual bool DefBattleChildren { get { return true; } }
        public virtual bool DefChangeBattleReset { get { return false; } }

        [CommandProperty(Conquests.Access)]
        public TypeSelectProperty<PvPBattle> Battle { get; set; }

        [CommandProperty(Conquests.Access)]
        public bool BattleChildren { get; set; }

        [CommandProperty(Conquests.Access)]
        public bool ChangeBattleReset { get; set; }

        public BattleKillConquest()
		{ }

        public BattleKillConquest(GenericReader reader)
			: base(reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

            Battle = DefBattle;
            BattleChildren = DefBattleChildren;
            ChangeBattleReset = DefChangeBattleReset;
		}

		public override sealed int GetProgress(ConquestState state, object args)
		{
			return GetProgress(state, args as PlayerMobile);
		}

		protected virtual int GetProgress(ConquestState state, PlayerMobile victim)
		{
			if (victim == null)
			{
				return 0;
			}

            if (state.User == null)
                return 0;

            PvPBattle battle = AutoPvP.FindBattle(state.User);

            if (Battle.IsNotNull && !battle.TypeEquals(Battle, BattleChildren))
            {
                if (ChangeBattleReset)
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

			int version = writer.SetVersion(0);

			switch (version)
			{
                case 0:
                    {
                        writer.WriteType(Battle);
                        writer.Write(BattleChildren);
                        writer.Write(ChangeBattleReset);
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
			        Battle = reader.ReadType();
			        BattleChildren = reader.ReadBool();
			        ChangeBattleReset = reader.ReadBool();
			    }
			        break;
			}
		}
	}
}