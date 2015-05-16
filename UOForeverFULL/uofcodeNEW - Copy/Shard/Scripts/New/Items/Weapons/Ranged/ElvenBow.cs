using System;
using Server;

namespace Server.Items
{
	public class ElvenBow : Bow
	{
		public override int OldStrengthReq{ get{ return 50; } }
		public override int NewMinDamage{ get{ return 19; } }
		public override int NewMaxDamage{ get{ return 33; } }
		//public override int DiceDamage { get { return Utility.Dice( 7, 3, 12 ); } } // 7d3+12 = 19-33
		public override int OldSpeed{ get{ return 35; } }

		public override int DefMaxRange{ get{ return 9; } }

		public override int InitMinHits{ get{ return 45; } }
		public override int InitMaxHits{ get{ return 75; } }

		public override string DefaultName{ get{ return "an elven bow"; } }

		[Constructable]
		public ElvenBow() : base()
		{
			Hue = 2212;
			AccuracyLevel = WeaponAccuracyLevel.Exceedingly;
		}

		public ElvenBow( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}