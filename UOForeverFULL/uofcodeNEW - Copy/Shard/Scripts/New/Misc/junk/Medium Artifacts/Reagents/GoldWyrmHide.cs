using System;
using Server;

namespace Server.Items
{
    public class GoldenWyrmHide : Item
    {
        [Constructable]
		public GoldenWyrmHide() : this( 1 )
		{
		}

		[Constructable]
		public GoldenWyrmHide( int amount ) : base( 0x1078 )
		{
           		 Name = "Golden Wyrm Hide";
			 Stackable = true;
           	  	 Hue = 1367;
			 Amount = amount;
		}

        public GoldenWyrmHide(Serial serial)
            : base(serial)
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