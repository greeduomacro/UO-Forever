using System;
using Server;

namespace Server.Items
{
	[Flipable(5363, 5364)]
	public class PirateShipModel : Item
    {
		public override string DefaultName{ get{ return "a model of a pirate ship"; } }

        [Constructable]
		public PirateShipModel() : this(5363)
        {
        }

        [Constructable]
        public PirateShipModel( int itemID ) : base( itemID )
        {
            Weight = 6;
		    Hue = 1175;
        }

        public PirateShipModel( Serial serial ) : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();
        }
    }
}