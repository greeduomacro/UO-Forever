using System;

namespace Server.Items
{
	
	public class Wall1 : BaseWall
	{
		[Constructable]
		public Wall1() : base( 0x3cf1 )
		{
			Name = "Wall1";
				
		}

		public Wall1( Serial serial ) : base( serial )
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