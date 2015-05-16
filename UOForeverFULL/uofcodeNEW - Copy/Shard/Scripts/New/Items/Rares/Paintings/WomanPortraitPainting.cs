using System;

namespace Server.Items
{
	[FlipableAttribute( 0x0E9F, 0x0EC8 )]
	public class WomanPortraitPainting : BasePainting
	{
		public override string DefaultName{ get{ return "portrait of a woman"; } }

		[Constructable]
		public WomanPortraitPainting() : this( PaintingQuality.Amatuer )
		{
		}

		[Constructable]
		public WomanPortraitPainting( PaintingQuality quality ) : base( 0x0E9F, quality )
		{
		}

		public WomanPortraitPainting( Serial serial ) : base( serial )
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