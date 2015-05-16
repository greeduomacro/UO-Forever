using System;
using Server;
using Server.Multis.Deeds;

namespace Server.Items
{
	public class CastleToken : PromotionalToken
	{
		[Constructable]
		public CastleToken() : base()
		{
		}

		public CastleToken( Serial serial ) : base( serial )
		{
		}

		public override Item CreateItemFor( Mobile from )
		{
			if ( from != null && from.Account != null )
				return new CastleDeed();
			else
				return null;
		}

		public override string DefaultName{ get{ return "a promotional token for a castle"; } }

		public override TextDefinition ItemName{ get{ return new TextDefinition( "Castle Deed" ); } }
		public override TextDefinition ItemReceiveMessage { get{ return new TextDefinition( "A castle deed has been created in your bank box." ); } }
		public override TextDefinition ItemGumpName { get{ return new TextDefinition( @"<center>Castle Deed</center>" ); } }

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