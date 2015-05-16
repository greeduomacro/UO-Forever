using System;
using Server;

namespace Server.Items
{ 
	public class MercenaryKatana: Katana
	{
		public override int LabelNumber{ get{ return 1061099; } } // Mercenary Katana
		
        public override int InitMinHits{ get{ return 6600; } }
		public override int InitMaxHits{ get{ return 6600; } }

		public override int OldStrengthReq{ get{ return 10; } }
		public override int NewMinDamage{ get{ return 5; } }
		public override int NewMaxDamage{ get{ return 26; } }
		public override int OldSpeed{ get{ return 58; } }

		public override int DefHitSound{ get{ return 0x23B; } }
		public override int DefMissSound{ get{ return 0x23A; } }

		[Constructable]
		public MercenaryKatana()

		{
            Name = "Mercenary Katana";
			Hue = 0x497;
		}

		public MercenaryKatana( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
