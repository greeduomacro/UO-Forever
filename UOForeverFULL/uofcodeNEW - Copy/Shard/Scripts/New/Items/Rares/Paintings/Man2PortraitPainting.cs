using System;

namespace Server.Items
{
	[FlipableAttribute( 0x0EA3, 0x0EA4 )]
	public class Man2PortraitPainting : BasePainting
	{
		public override string DefaultName{ get{ return "portrait of a man"; } }

		[Constructable]
		public Man2PortraitPainting() : this( PaintingQuality.Amatuer )
		{
		}

		[Constructable]
		public Man2PortraitPainting( PaintingQuality quality ) : base( 0x0EA3, quality )
		{
		}

		public Man2PortraitPainting( Serial serial ) : base( serial )
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