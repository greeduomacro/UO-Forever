using System;
using Server;

namespace Server.Items
{
    public class BlueSoulIngot : Item
    {
        [Constructable]
		public BlueSoulIngot() : this( 1 )
		{
		}

		[Constructable]
		public BlueSoulIngot( int amount ) : base( 0x1BF5 )
		{
           		 Name = "Blue Soul Ingot";
			 Stackable = true;
           	  	 Hue = 1367;
			 Amount = amount;
		}

        public BlueSoulIngot(Serial serial)
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