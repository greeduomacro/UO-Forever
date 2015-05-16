using System;

namespace Server.Items
{
	public class Bushel : BaseContainer
	{
		[Constructable]
		public Bushel() : base( 0x9AC )
		{
		}

		public Bushel( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			//Container
			//writer.Write( 2 );
			//writer.Write( (byte)0 );
			//End container

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}