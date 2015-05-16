using System;
using Server;
using Server.Multis.Deeds;

namespace Server.Items
{
	public class KeepToken : PromotionalToken
	{
		[Constructable]
		public KeepToken() : base()
		{
		}

		public KeepToken( Serial serial ) : base( serial )
		{
		}

		public override Item CreateItemFor( Mobile from )
		{
			if ( from != null && from.Account != null )
				return new KeepDeed();
			else
				return null;
		}

		public override string DefaultName{ get{ return "a promotional token for a keep"; } }

		public override TextDefinition ItemName{ get{ return new TextDefinition( "Keep Deed" ); } }
		public override TextDefinition ItemReceiveMessage { get{ return new TextDefinition( "A Keep deed has been created in your bank box." ); } }
		public override TextDefinition ItemGumpName { get{ return new TextDefinition( @"<center>Keep Deed</center>" ); } }

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