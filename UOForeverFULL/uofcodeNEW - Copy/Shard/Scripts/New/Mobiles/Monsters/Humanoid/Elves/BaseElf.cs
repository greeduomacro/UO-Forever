using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class BaseElf : BaseCreature
	{
		public override string DefaultName{ get{ return "an elf"; } }

		public BaseElf( AIType ai, FightMode mode, int iRangePerception, int iRangeFight, double dActiveSpeed, double dPassiveSpeed )
		: base( ai, mode, iRangePerception, iRangeFight, dActiveSpeed, dPassiveSpeed )
		{
			Hue = 1801;
			Body = 0x605;
			Race = Race.Elf;

			Utility.AssignRandomHair( this, false );
			if ( Utility.Random( 10 ) <= 4 )
				HairHue = 0;
			else
				HairHue = Utility.RandomBool() ? Utility.RandomMinMax( 1102, 1149 ) : Utility.RandomMinMax( 1801, 1908 );
		}

		public override bool ClickTitle{ get{ return false; } }
		public override bool AlwaysMurderer{ get{ return true; } }

		public override int GetAttackSound()
		{
			return Utility.Random( 0x434, 9 );
		}

		public override int GetAngerSound()
		{
			return 0x44A;
		}

		public override int GetHurtSound()
		{
			return Utility.Random( 0x434, 3 );
		}

		public BaseElf( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}