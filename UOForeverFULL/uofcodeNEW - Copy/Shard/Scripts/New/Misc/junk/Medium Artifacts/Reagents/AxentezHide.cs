using System;
using Server;

namespace Server.Items
{
    public class AxentezHide : Item
    {
        [Constructable]
		public AxentezHide() : this( 1 )
		{
		}

		[Constructable]
		public AxentezHide( int amount ) : base( 0x1078 )
		{
           		 Name = "Axentez Hide";
				 Stackable = true;
           	  	 Hue = 1367;
				 Amount = amount;
		}

        public AxentezHide(Serial serial)
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