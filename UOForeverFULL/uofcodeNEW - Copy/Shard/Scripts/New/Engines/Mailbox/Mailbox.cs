using System;
using Server;
using Server.Engines.MailboxSystem;

namespace Server.Items
{
	public class BaseMailbox : Item
	{
		public BaseMailbox( int itemid ) : base( itemid )
		{
			Movable = false;
		}

		public override void OnDoubleClick( Mobile from )
		{
			//from.CloseGump( typeof( MailGump ) );
			//from.CloseGump( typeof( WriteMailGump ) );
			//from.CloseGump( typeof( ReadMailGump ) );
		}

		public BaseMailbox( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int)0 ); //version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}