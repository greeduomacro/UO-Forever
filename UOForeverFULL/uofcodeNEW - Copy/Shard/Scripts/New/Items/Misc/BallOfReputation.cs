using System;
using Server;

namespace Server.Items
{
	public class BallOfReputation : Item
	{
		public override string DefaultName{ get{ return "a crystal ball of reputation"; } }
		public override int LabelNumber{ get{ return 0; } }

		[Constructable]
		public BallOfReputation() : base( 3629 )
		{
		}

		public BallOfReputation( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				from.SendMessage( "The crystal ball becomes clear..." );
				from.SendMessage( "...your karma is {0}, fame is {1}...", from.Karma, from.Fame );
				from.SendMessage( "...your short term murders {0}, and long term murders {1}.", from.ShortTermMurders, from.Kills );
				Consume();
			}
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}