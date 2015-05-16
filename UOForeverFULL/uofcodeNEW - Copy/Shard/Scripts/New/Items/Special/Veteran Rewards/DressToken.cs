using System;
using Server;

namespace Server.Items
{
	public class DressToken : PromotionalToken
	{
		[Constructable]
		public DressToken() : base()
		{
		}

		public DressToken( Serial serial ) : base( serial )
		{
		}

		public override Item CreateItemFor( Mobile from )
		{
			return new VeteranDress();
		}

		public override string DefaultName{ get{ return "a promotional token for a veteran dress"; } }

		public override TextDefinition ItemName{ get{ return new TextDefinition( "Veteran Dress" ); } }
		public override TextDefinition ItemReceiveMessage { get{ return new TextDefinition( "A veteran dress has been created in your bank box." ); } }
		public override TextDefinition ItemGumpName { get{ return new TextDefinition( @"<center>Veteran Dress</center>" ); } }

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