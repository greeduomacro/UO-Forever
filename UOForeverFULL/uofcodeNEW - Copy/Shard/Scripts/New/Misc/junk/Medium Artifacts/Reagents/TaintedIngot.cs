using System;
using Server;

namespace Server.Items
{
    public class TaintedIngot : Item
    {
        [Constructable]
		public TaintedIngot() : this( 1 )
		{
		}

		[Constructable]
		public TaintedIngot( int amount ) : base( 0x1BF5 )
		{
           		 Name = "Tainted Ingot";
			 Stackable = true;
           	  	 Hue = 1367;
			 Amount = amount;
		}

        public TaintedIngot(Serial serial)
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