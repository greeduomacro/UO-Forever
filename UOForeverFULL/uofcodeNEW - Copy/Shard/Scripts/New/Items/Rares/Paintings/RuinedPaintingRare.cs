using System;
using Server;

namespace Server.Items
{
	public class RuinedPaintingRare : BasePainting
	{
		public override string DefaultName{ get{ return "painting"; } }

		public override string GetQualityName()
		{
			switch ( Quality )
			{
				default:
				case PaintingQuality.Amatuer: return "a ruined amatuer";
				case PaintingQuality.Neophyte: return "a ruined neophyte";
				case PaintingQuality.Novice: return "a ruined novice";
				case PaintingQuality.Apprentice: return "a ruined apprentice";
				case PaintingQuality.Expert: return "a ruined expert";
				case PaintingQuality.Artisan: return "a ruined artisan";
				case PaintingQuality.Master: return "a ruined master";
				case PaintingQuality.Grandmaster: return "a ruined grandmaster";
				case PaintingQuality.Legendary: return "a ruined legendary";
			}
		}

		[Constructable]
		public RuinedPaintingRare() : this( PaintingQuality.Amatuer )
		{
		}

		[Constructable]
		public RuinedPaintingRare( PaintingQuality quality ) : base( 0xC2C, quality )
		{
		}

		public RuinedPaintingRare( Serial serial ) : base( serial )
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