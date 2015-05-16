using System;

namespace Server.Items
{
	[FlipableAttribute( 0x0EA7, 0x0EA8 )]
	public class Lady2PortraitPainting : BasePainting
	{
		public override string DefaultName{ get{ return "portrait of a lady"; } }

		[Constructable]
		public Lady2PortraitPainting() : this( PaintingQuality.Amatuer )
		{
		}

		[Constructable]
		public Lady2PortraitPainting( PaintingQuality quality ) : base( 0x0EA7, quality )
		{
		}

		public Lady2PortraitPainting( Serial serial ) : base( serial )
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