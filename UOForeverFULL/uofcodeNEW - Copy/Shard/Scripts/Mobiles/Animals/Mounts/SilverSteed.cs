using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a silver steed corpse" )]
	public class SilverSteed : BaseMount
	{
		public override string DefaultName{ get{ return "a silver steed"; } }

        public override int InternalItemItemID { get { return 0x3EA8; } }

		[Constructable]
		public SilverSteed() : base( 0x75, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			InitStats( Utility.Random( 50, 30 ), Utility.Random( 50, 30 ), 10 );
			Skills[SkillName.MagicResist].Base = 25.0 + (Utility.RandomDouble() * 5.0);
			Skills[SkillName.Wrestling].Base = 35.0 + (Utility.RandomDouble() * 10.0);
			Skills[SkillName.Tactics].Base = 30.0 + (Utility.RandomDouble() * 15.0);

			ControlSlots = 1;
			Tamable = true;
			MinTameSkill = 100.0;
		}

		public SilverSteed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}