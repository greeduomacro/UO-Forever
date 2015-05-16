using System;

namespace Server.Items
{
	public class ElvenRobe : BaseOuterTorso
	{
		[Constructable]
		public ElvenRobe() : base( 0x2FB9 )
		{
			Hue = 0x0;
			Weight = 1;
			Name = "Elven Robe";
		}

		public ElvenRobe( Serial serial ) : base( serial )
		{
		}

		public override bool Dye( Mobile from, IDyeTub sender )
		{
			return false;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}
}