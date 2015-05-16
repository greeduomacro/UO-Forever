using System;
using Server;

namespace Server.Items
{
	public class TwilightLantern : Lantern
	{
		public override string DefaultName{ get{ return "a twilight lantern"; } }

        public override bool AllowEquippedCast(Mobile from)
        {
            return true;
        }

		[Constructable]
		public TwilightLantern()
		{
			Hue = Utility.RandomList( 47, 997 );
		}

		public TwilightLantern( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt((int) 0); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}