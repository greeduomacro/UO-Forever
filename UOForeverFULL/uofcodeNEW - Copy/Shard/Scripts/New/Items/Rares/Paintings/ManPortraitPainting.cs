using System;

namespace Server.Items
{
	[FlipableAttribute( 0x0EA2, 0x0EA1 )]
	public class ManPortraitPainting : BasePainting
	{
		public override string DefaultName{ get{ return "portrait of a man"; } }

		[Constructable]
		public ManPortraitPainting() : this( PaintingQuality.Amatuer )
		{
		}

		[Constructable]
		public ManPortraitPainting( PaintingQuality quality ) : base( 0x0EA2, quality )
		{
		}

		public ManPortraitPainting( Serial serial ) : base( serial )
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