using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public class CitrinePower : Item
	{
    
		public override string DefaultName{ get{ return "Citrine Power"; } }
		public override double DefaultWeight
		{
			get { return 0.1; }
		}

		[Constructable]
		public CitrinePower() : this( 1 )
		{
		}

		[Constructable]
		public CitrinePower( int amount ) : base( 0x3198 )
		{
			Stackable = true;
			Hue = 496;
            Amount = amount;
}

		public CitrinePower( Serial serial ) : base( serial )
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