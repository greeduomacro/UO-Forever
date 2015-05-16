using System;

namespace Server.Items
{
	[FlipableAttribute( 0x0EA6, 0x0EA5 )]
	public class LadyPortraitPainting : BasePainting
	{
		public override string DefaultName{ get{ return "portrait of a lady"; } }

		[Constructable]
		public LadyPortraitPainting() : this( PaintingQuality.Amatuer )
		{
		}

		[Constructable]
		public LadyPortraitPainting( PaintingQuality quality ) : base( 0x0EA6, quality )
		{
		}

		public LadyPortraitPainting( Serial serial ) : base( serial )
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