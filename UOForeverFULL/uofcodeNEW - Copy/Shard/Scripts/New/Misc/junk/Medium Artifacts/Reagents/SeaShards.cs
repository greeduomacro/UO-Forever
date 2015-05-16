using System;
using Server;

namespace Server.Items
{
    public class SeaShards : Item
    {
        [Constructable]
		public SeaShards() : this( 1 )
		{
		}

		[Constructable]
		public SeaShards( int amount ) : base( 0x2248 )
		{
           		 Name = "Sea Shard";
			 Stackable = true;
           	  	 Hue = 1367;
			 Amount = amount;
		}

        public SeaShards(Serial serial)
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