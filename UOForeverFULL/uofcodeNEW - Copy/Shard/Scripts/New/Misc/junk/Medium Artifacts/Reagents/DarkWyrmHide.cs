using System;
using Server;

namespace Server.Items
{
    public class DarkWyrmHide : Item
    {
        [Constructable]
		public DarkWyrmHide() : this( 1 )
		{
		}

		[Constructable]
		public DarkWyrmHide( int amount ) : base( 0x1078 )
		{
           		 Name = "Dark Wyrm Hide";
			 Stackable = true;
           	  	 Hue = 1367;
			 Amount = amount;
		}

        public DarkWyrmHide(Serial serial)
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