using System;

namespace Server.Items
{
	[FlipableAttribute( 0x240D, 0x240E )]
	public class RarePainting1 : BasePainting
	{
		public override string DefaultName{ get{ return "abstract painting"; } }

		[Constructable]
		public RarePainting1() : this( PaintingQuality.Amatuer )
		{
		}

		[Constructable]
		public RarePainting1( PaintingQuality quality ) : base( 0x240D, quality )
		{
		}

		public RarePainting1( Serial serial ) : base( serial )
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