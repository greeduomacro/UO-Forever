using System;
using Server;

namespace Server.Items
{
    public class GreenGlowingGem : Item
    {
        [Constructable]
		public GreenGlowingGem() : this( 1 )
		{
		}

		[Constructable]
		public GreenGlowingGem( int amount ) : base( 0x2808 )
		{
           		 Name = "Green Glowing Gem";
			 Stackable = true;
           	  	 Hue = 1367;
			 Amount = amount;
		}

        public GreenGlowingGem(Serial serial)
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