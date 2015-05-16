using System;
using System.Collections.Generic;
using Server;
using Server.Misc;

namespace Server.Items
{
	public abstract class BaseCandy : Food
	{
		public BaseCandy( int itemID ) : this( 1, itemID )
		{
		}

		public BaseCandy( int amount, int itemID ) : base( itemID )
		{
			FillFactor = 0; //Candy is bad for you!
		}

		public override bool Eat( Mobile from )
		{
			if ( base.Eat( from ) )
			{
				TrickorTreatPersistence.AteCandy( from );
				return true;
			}

			return false;
		}

		public BaseCandy( Serial serial ) : base( serial )
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