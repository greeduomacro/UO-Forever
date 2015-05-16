#region References
using System;
using Server.Mobiles;
using Server.Regions;
using VitaNex;
#endregion

namespace Server.Engines.Conquests
{
	public class StealingConquest : Conquest
	{
		public override string DefCategory { get { return "Stealing"; } }

		public virtual Type DefCreature { get { return null; } }
		public virtual bool DefChildren { get { return true; } }
		public virtual bool DefChangeCreatureReset { get { return false; } }

        public virtual Type DefItem { get { return null; } }
        public virtual bool DefItemChildren { get { return true; } }
        public virtual bool DefItemChangeReset { get { return false; } }

        public virtual bool DefPlayerTarget { get { return false; } }

        public virtual bool DefIsDungeon { get { return false; } }

		[CommandProperty(Conquests.Access)]
		public CreatureTypeSelectProperty Creature { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool Children { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool ChangeCreatureReset { get; set; }

        [CommandProperty(Conquests.Access)]
        public ItemTypeSelectProperty Item { get; set; }

        [CommandProperty(Conquests.Access)]
        public bool ItemChildren { get; set; }

        [CommandProperty(Conquests.Access)]
        public bool ItemChangeReset { get; set; }

        [CommandProperty(Conquests.Access)]
        public bool PlayerTarget { get; set; }

        [CommandProperty(Conquests.Access)]
        public bool IsDungeon { get; set; }

		public StealingConquest()
		{ }

        public StealingConquest(GenericReader reader)
			: base(reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

            Item = DefItem;
            ItemChildren = DefItemChildren;
            ItemChangeReset = DefItemChangeReset;

			Creature = DefCreature;
			Children = DefChildren;
			ChangeCreatureReset = DefChangeCreatureReset;

		    PlayerTarget = DefPlayerTarget;
		    IsDungeon = DefIsDungeon;
		}

		public override sealed int GetProgress(ConquestState state, object args)
		{
			return GetProgress(state, args as Item);
		}

        protected virtual int GetProgress(ConquestState state, Item stolenItem)
        {
            if (stolenItem == null)
            {
                return 0;
            }

            if (state.User == null)
                return 0;

            var creature = stolenItem.RootParent as BaseCreature;

            if (IsDungeon && !state.User.InRegion<DungeonRegion>())
            {
                return 0;
            }

            if (PlayerTarget && !(stolenItem.RootParent is PlayerMobile))
            {
                return 0;
            }

			if (Creature.IsNotNull && creature != null && !creature.TypeEquals(Creature, Children))
			{
				if (ChangeCreatureReset)
				{
					return -state.Progress;
				}

				return 0;
			}

            if (Item.IsNotNull && !stolenItem.TypeEquals(Item, ItemChildren))
            {
                if (ItemChangeReset)
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
						writer.WriteType(Creature);
						writer.Write(Children);
						writer.Write(ChangeCreatureReset);

                        writer.WriteType(Item);
                        writer.Write(ItemChildren);
                        writer.Write(ItemChangeReset);

                        writer.Write(IsDungeon);
                        writer.Write(PlayerTarget);
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
						Creature = reader.ReadType();
						Children = reader.ReadBool();
						ChangeCreatureReset = reader.ReadBool();

                        Item = reader.ReadType();
                        ItemChildren = reader.ReadBool();
                        ItemChangeReset = reader.ReadBool();

					    IsDungeon = reader.ReadBool();
					    PlayerTarget = reader.ReadBool();
					}
					break;
			}
		}
	}
}