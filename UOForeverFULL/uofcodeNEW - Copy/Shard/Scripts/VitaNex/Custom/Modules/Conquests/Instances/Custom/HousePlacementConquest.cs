#region References
using System;
using Server.Multis;
using VitaNex;
#endregion

namespace Server.Engines.Conquests
{
	public class HousePlacementConquest : Conquest
	{
		public override string DefCategory { get { return "Houses"; } }
        public virtual Type DefHouse { get { return null; } }

		[CommandProperty(Conquests.Access)]
		public TypeSelectProperty<BaseHouse> House { get; set; }


		public HousePlacementConquest()
		{ }

        public HousePlacementConquest(GenericReader reader)
			: base(reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

            House = DefHouse;
		}

		public override sealed int GetProgress(ConquestState state, object args)
		{
			return GetProgress(state, args as BaseHouse);
		}

		protected virtual int GetProgress(ConquestState state, BaseHouse house)
		{
			if (house == null)
			{
				return 0;
			}

            if (state.User == null)
                return 0;

			if (House.IsNotNull && !house.TypeEquals(House, false))
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
						writer.WriteType(House);
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
			        House = reader.ReadType();
			    }
					break;
			}
		}
	}
}