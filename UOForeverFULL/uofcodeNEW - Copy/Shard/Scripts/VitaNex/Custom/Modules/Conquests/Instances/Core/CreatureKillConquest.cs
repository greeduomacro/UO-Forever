#region References
using System;
using Server.Items;
using Server.Mobiles;
using VitaNex;
#endregion

namespace Server.Engines.Conquests
{
    public class CreatureConquestContainer
    {
        public Mobile Creature;
        public Mobile Killer;
        public Container Corpse;

        public CreatureConquestContainer(Mobile b, Mobile k, Container c)
        {
            Creature = b;
            Killer = k;
            Corpse = c;
        }
    }

	public class CreatureKillConquest : Conquest
	{
		public override string DefCategory { get { return "Slaying (PvM)"; } }

		public virtual Type DefCreature { get { return null; } }
		public virtual bool DefChildren { get { return true; } }
		public virtual bool DefChangeCreatureReset { get { return false; } }

		public virtual NotorietyType DefNotoCompare { get { return NotorietyType.None; } }
		public virtual NotorietyType DefNotoCreature { get { return NotorietyType.None; } }
		public virtual NotorietyType DefNotoKiller { get { return NotorietyType.None; } }

		public virtual AccessLevel DefAccessCreature { get { return AccessLevel.Player; } }
		public virtual AccessLevel DefAccessKiller { get { return AccessLevel.Player; } }

        public virtual bool DefIsParagon { get { return false; } }

		[CommandProperty(Conquests.Access)]
		public NotorietyType NotoCompare { get; set; }

		[CommandProperty(Conquests.Access)]
		public NotorietyType NotoCreature { get; set; }

		[CommandProperty(Conquests.Access)]
		public NotorietyType NotoKiller { get; set; }

		[CommandProperty(Conquests.Access)]
		public AccessLevel AccessCreature { get; set; }

		[CommandProperty(Conquests.Access)]
		public AccessLevel AccessKiller { get; set; }

		[CommandProperty(Conquests.Access)]
		public CreatureTypeSelectProperty Creature { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool Children { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool ChangeCreatureReset { get; set; }

        [CommandProperty(Conquests.Access)]
        public bool IsParagon { get; set; }

		public CreatureKillConquest()
		{ }

		public CreatureKillConquest(GenericReader reader)
			: base(reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			Creature = DefCreature;
			Children = DefChildren;
			ChangeCreatureReset = DefChangeCreatureReset;

			NotoCompare = DefNotoCompare;
			NotoCreature = DefNotoCreature;
			NotoKiller = DefNotoKiller;

			AccessCreature = DefAccessCreature;
			AccessKiller = DefAccessKiller;

		    IsParagon = DefIsParagon;
		}

		public override sealed int GetProgress(ConquestState state, object args)
		{
            return GetProgress(state, args as CreatureConquestContainer);
		}

        protected virtual int GetProgress(ConquestState state, CreatureConquestContainer args)
		{
            if (state.User == null)
                return 0;

            if (args == null || args.Creature == null || args.Killer is PlayerMobile && args.Killer.Account != state.User.Account)
			{
				return 0;
			}

			if (Creature.IsNotNull && !args.Creature.TypeEquals(Creature, Children))
			{
				if (ChangeCreatureReset)
				{
					return -state.Progress;
				}

				return 0;
			}

			if (AccessCreature > args.Creature.AccessLevel)
			{
				return 0;
			}

			if (AccessKiller > args.Killer.AccessLevel)
			{
				return 0;
			}

			if (NotoCompare != NotorietyType.None && NotoCompare != args.Killer.ComputeNotoriety(args.Creature))
			{
				return 0;
			}

			if (NotoCreature != NotorietyType.None && NotoCreature != args.Creature.ComputeNotoriety())
			{
				return 0;
			}

			if (NotoKiller != NotorietyType.None && NotoKiller != args.Killer.ComputeNotoriety())
			{
				return 0;
			}

		    if (((BaseCreature) args.Creature).IsBonded)
		    {
		        return 0;
		    }

		    if (IsParagon && !((BaseCreature) args.Creature).IsParagon)
		    {
		        return 0;
		    }

			return 1;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(2);

			switch (version)
			{
                case 2:
                    {
                        writer.Write(IsParagon);
                    }
                    goto case 1;
				case 1:
					{
						writer.WriteFlag(AccessCreature);
						writer.WriteFlag(AccessKiller);
					}
					goto case 0;
				case 0:
					{
						writer.WriteType(Creature);
						writer.Write(Children);
						writer.Write(ChangeCreatureReset);

						writer.WriteFlag(NotoCompare);
						writer.WriteFlag(NotoCreature);
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
                case 2:
			        {
			            IsParagon = reader.ReadBool();
			        }
                    goto case 1;
				case 1:
					{
						AccessCreature = reader.ReadFlag<AccessLevel>();
						AccessKiller = reader.ReadFlag<AccessLevel>();
					}
					goto case 0;
				case 0:
					{
						Creature = reader.ReadType();
						Children = reader.ReadBool();
						ChangeCreatureReset = reader.ReadBool();

						NotoCompare = reader.ReadFlag<NotorietyType>();
						NotoCreature = reader.ReadFlag<NotorietyType>();
						NotoKiller = reader.ReadFlag<NotorietyType>();
					}
					break;
			}
		}
	}
}