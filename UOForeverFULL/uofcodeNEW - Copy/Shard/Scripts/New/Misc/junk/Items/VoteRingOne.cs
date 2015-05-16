using System;
using Server;

namespace Server.Items
{
	public class VoteEarringsOne : GoldEarrings
	{
		public override int ArtifactRarity{ get{ return 11; } }

		[Constructable]
		public VoteEarringsOne()
		{
			Name = "Hybrid's Pathogen";
			Hue = 1266;
		}

		public VoteEarringsOne( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Hue == 1266 )
				Hue = 1266;
		}
	}
}