using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Misc;

namespace Server.Mobiles
{
	public abstract class BaseBarbarian : BaseCreature
	{
		public BaseBarbarian( bool female, AIType aiType, FightMode fightMode, int rangePerception, int rangeFight, double activeSpeed, double passiveSpeed ) : base( aiType, fightMode, rangePerception, rangeFight, activeSpeed, passiveSpeed )
		{
			if ( this.Female = female )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
				HairItemID = 0x203D;
				HairHue = Utility.RandomRedHue();
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
				HairItemID = 0x203C;
				HairHue = Utility.RandomRedHue();
			}

			//PackItem( new Gold( Utility.Random( 50, 100 ) ) );
			Hue = 0;
		}

		public override bool ClickTitle{ get{ return false; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }

		public BaseBarbarian( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}