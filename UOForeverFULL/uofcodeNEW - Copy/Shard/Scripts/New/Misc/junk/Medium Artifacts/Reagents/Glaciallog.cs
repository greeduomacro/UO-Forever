using System;
using Server;

namespace Server.Items
{
    public class GlacialLog : Item
    {
        [Constructable]
		public GlacialLog() : this( 1 )
		{
		}

		[Constructable]
		public GlacialLog( int amount ) : base( 0x1BDD )
		{
           		 Name = "Glacial Log";
			 Stackable = true;
           	  	 Hue = 1367;
			 Amount = amount;
		}

        public GlacialLog(Serial serial)
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