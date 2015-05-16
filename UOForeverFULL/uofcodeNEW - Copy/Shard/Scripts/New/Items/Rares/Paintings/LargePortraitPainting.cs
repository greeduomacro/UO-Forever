using System;

namespace Server.Items
{
	public class LargePortraitPainting : BasePainting
	{
		public override string DefaultName{ get{ return "large portrait"; } }

		[Constructable]
		public LargePortraitPainting() : this( PaintingQuality.Amatuer )
		{
		}

		[Constructable]
		public LargePortraitPainting( PaintingQuality quality ) : base( 0x0EA0, quality )
		{
		}

		public LargePortraitPainting( Serial serial ) : base( serial )
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