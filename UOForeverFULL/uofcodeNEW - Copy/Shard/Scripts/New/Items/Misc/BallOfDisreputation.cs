using System;
using Server;
using Server.Misc;

namespace Server.Items
{
	public class BallOfDisreputation : Item
	{
		public override string DefaultName{ get{ return "a crystal ball of disreputation"; } }
		public override int LabelNumber{ get{ return 0; } }

		[Constructable]
		public BallOfDisreputation() : base( 3629 )
		{
		}

		public BallOfDisreputation( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else if ( AutoRestart.ServerWars )
			{
				from.SendMessage( "The crystal ball becomes clear..." );
				from.SendMessage( "...you have lost all reputation!" );

				from.Karma = Math.Min( from.Karma, -from.Karma * 2 );
				from.Fame = Math.Min( from.Fame, -from.Fame * 2 );

				from.Kills = 5;
				from.ShortTermMurders = 5;
				Consume();
			}
			else
				from.SendMessage( "The crystal ball is clouded." );
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