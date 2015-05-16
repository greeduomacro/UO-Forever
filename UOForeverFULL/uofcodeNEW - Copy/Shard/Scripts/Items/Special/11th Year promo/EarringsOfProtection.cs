using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Items
{
	public class EarringBoxSet : RedVelvetGiftBox
	{
		[Constructable]
		public EarringBoxSet()
			: base()
		{
			DropItem( new SilverEarrings() );
			DropItem( new SilverEarrings() );
			DropItem( new SilverEarrings() );
			DropItem( new SilverEarrings() );
			DropItem( new SilverEarrings() );
		}

		public EarringBoxSet( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}