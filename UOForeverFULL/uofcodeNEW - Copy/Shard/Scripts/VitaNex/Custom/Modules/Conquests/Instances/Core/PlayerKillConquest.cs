using System;
using Server.Items;
using Server.Mobiles;
using Server.Regions;
using VitaNex;
using VitaNex.Modules.AutoPvP;

namespace Server.Engines.Conquests
{
    public class PlayerConquestContainer
    {
        public Mobile Mobile;
        public Mobile Killer;
        public Container Corpse;

        public PlayerConquestContainer(Mobile b, Mobile k, Container c)
        {
            Mobile = b;
            Killer = k;
            Corpse = c;
        }
    }

	public class PlayerKillConquest : Conquest
	{
		public override string DefCategory { get { return "Slaying (PvP)"; } }

		public virtual NotorietyType DefNotoCompare { get { return NotorietyType.None; } }
		public virtual NotorietyType DefNotoPlayer { get { return NotorietyType.None; } }
		public virtual NotorietyType DefNotoKiller { get { return NotorietyType.None; } }

		public virtual AccessLevel DefAccessPlayer { get { return AccessLevel.Player; } }
		public virtual AccessLevel DefAccessKiller { get { return AccessLevel.Player; } }

        public virtual string DefRegionName { get { return null; } }
        public virtual Map DefMap { get { return null; } }

        public virtual Type DefBattle { get { return null; } }
        public virtual bool DefBattleChildren { get { return true; } }
        public virtual bool DefChangeBattleReset { get { return false; } }

        public virtual bool DefIsDungeon { get { return false; } }
        public virtual bool DefIsDuel { get { return false; } }

		[CommandProperty(Conquests.Access)]
		public NotorietyType NotoCompare { get; set; }

		[CommandProperty(Conquests.Access)]
		public NotorietyType NotoPlayer { get; set; }

		[CommandProperty(Conquests.Access)]
		public NotorietyType NotoKiller { get; set; }
		
		[CommandProperty(Conquests.Access)]
		public AccessLevel AccessPlayer { get; set; }

		[CommandProperty(Conquests.Access)]
		public AccessLevel AccessKiller { get; set; }

        [CommandProperty(Conquests.Access)]
        public Map Map { get; set; }

        [CommandProperty(Conquests.Access)]
        public string RegionName { get; set; }

        [CommandProperty(Conquests.Access)]
        public bool IsDungeon { get; set; }

        [CommandProperty(Conquests.Access)]
        public bool IsDuel { get; set; }


        [CommandProperty(Conquests.Access)]
        public TypeSelectProperty<PvPBattle> Battle { get; set; }

        [CommandProperty(Conquests.Access)]
        public bool BattleChildren { get; set; }

        [CommandProperty(Conquests.Access)]
        public bool ChangeBattleReset { get; set; }
		
		public PlayerKillConquest()
		{ }

		public PlayerKillConquest(GenericReader reader)
			: base(reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			NotoCompare = DefNotoCompare;
			NotoPlayer = DefNotoPlayer;
			NotoKiller = DefNotoKiller;

			AccessPlayer = DefAccessPlayer;
			AccessKiller = DefAccessKiller;

		    RegionName = DefRegionName;
		    Map = DefMap;

            Battle = DefBattle;
            BattleChildren = DefBattleChildren;
            ChangeBattleReset = DefChangeBattleReset;

		    IsDungeon = DefIsDungeon;
            IsDuel = DefIsDuel;
		}

		public override sealed int GetProgress(ConquestState state, object args)
		{
            return GetProgress(state, args as PlayerConquestContainer);
		}

        protected virtual int GetProgress(ConquestState state, PlayerConquestContainer args)
		{
            if (state.User == null)
                return 0;

            if (args == null || args.Mobile == null || args.Killer is PlayerMobile && args.Killer.Account != state.User.Account)
			{
				return 0;
			}

            if (IsDuel && args.Mobile is PlayerMobile && args.Killer is PlayerMobile && ((PlayerMobile)args.Killer).DuelContext == null)
		    {
		        return 0;
		    }

            if (IsDungeon && !args.Mobile.InRegion<DungeonRegion>())
            {
                return 0;
            }

            PvPBattle battle = AutoPvP.FindBattle(args.Killer as PlayerMobile);

            if (Battle.IsNotNull && !battle.TypeEquals(Battle, BattleChildren))
            {
                if (ChangeBattleReset)
                {
                    return -state.Progress;
                }

                return 0;
            }

			if (AccessPlayer > args.Mobile.AccessLevel)
			{
				return 0;
			}

			if (AccessKiller > args.Killer.AccessLevel)
			{
				return 0;
			}

			if (NotoCompare != NotorietyType.None && NotoCompare != args.Killer.ComputeNotoriety(args.Mobile))
			{
				return 0;
			}

			if (NotoPlayer != NotorietyType.None && NotoPlayer != args.Mobile.ComputeNotoriety())
			{
				return 0;
			}

			if (NotoKiller != NotorietyType.None && NotoKiller != args.Killer.ComputeNotoriety())
			{
				return 0;
			}

            if (Map != null && Map != Map.Internal && (args.Mobile.Map == null || args.Mobile.Map != Map))
            {
                return 0;
            }

            if (!String.IsNullOrWhiteSpace(RegionName) && (args.Mobile.Region == null || !args.Mobile.Region.IsPartOf(RegionName)))
            {
                return 0;
            }

			return 1;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(3);

			switch (version)
			{
                    case 3:
			        {
			            writer.Write(IsDuel);
                        writer.Write(IsDungeon);
                    }
                    goto case 2;
                case 2:
                    {
                        writer.WriteType(Battle);
                        writer.Write(BattleChildren);
                        writer.Write(ChangeBattleReset);

                        writer.Write(Map);
                        writer.Write(RegionName);
                    }
                    goto case 1;
				case 1:
					{
						writer.WriteFlag(AccessPlayer);
						writer.WriteFlag(AccessKiller);
					}
					goto case 0;
				case 0:
					{
						writer.WriteFlag(NotoCompare);
						writer.WriteFlag(NotoPlayer);
						writer.WriteFlag(NotoKiller);
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
                case 3:
                    {
                        IsDuel = reader.ReadBool();
                        IsDungeon = reader.ReadBool();
                    }
                    goto case 2;
                case 2:
                    {
                        Battle = reader.ReadType();
                        BattleChildren = reader.ReadBool();
                        ChangeBattleReset = reader.ReadBool();

                        RegionName = reader.ReadString();
                        Map = reader.ReadMap();
                    }
                    goto case 1;
				case 1:
					{
						AccessPlayer = reader.ReadFlag<AccessLevel>();
						AccessKiller = reader.ReadFlag<AccessLevel>();
					}
					goto case 0;
				case 0:
					{
						NotoCompare = reader.ReadFlag<NotorietyType>();
						NotoPlayer = reader.ReadFlag<NotorietyType>();
						NotoKiller = reader.ReadFlag<NotorietyType>();
					}
					break;
			}
		}
	}
}