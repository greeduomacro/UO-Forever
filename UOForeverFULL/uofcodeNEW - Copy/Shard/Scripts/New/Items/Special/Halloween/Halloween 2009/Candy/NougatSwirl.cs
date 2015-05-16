using System;
using Server;

namespace Server.Items
{
	public class NougatSwirl : BaseCandy
	{
		public override int LabelNumber { get{ return 1096936; } } // nougat swirl

		[Constructable]
		public NougatSwirl() : this( 1 )
		{
		}

		[Constructable]
		public NougatSwirl( int amount ) : base( amount, 0x4690 )
		{
		}

		public NougatSwirl( Serial serial ) : base( serial )
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