using System;

namespace Server.Items
{
	[FlipableAttribute( 0x0EE7, 0x0EC9 )]
	public class Woman2PortraitPainting : BasePainting
	{
		public override string DefaultName{ get{ return "portrait of a woman"; } }

		[Constructable]
		public Woman2PortraitPainting() : this( PaintingQuality.Amatuer )
		{
		}

		[Constructable]
		public Woman2PortraitPainting( PaintingQuality quality ) : base( 0xEE7, quality )
		{
		}

		public Woman2PortraitPainting( Serial serial ) : base( serial )
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