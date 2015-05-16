using System;
using Server;

namespace Server.Items
{
	public class MagnetiteStone : Item
	{
		public override string DefaultName{ get{ return "magnetite stone"; } }

		[Constructable]
		public MagnetiteStone() : this( 1 )
		{
		}

		[Constructable]
		public MagnetiteStone( int amount ) : base( 0xF89 )
		{
			Hue = 2306;
			Weight = 5.0;
			Stackable = true;
		}

		public MagnetiteStone( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}