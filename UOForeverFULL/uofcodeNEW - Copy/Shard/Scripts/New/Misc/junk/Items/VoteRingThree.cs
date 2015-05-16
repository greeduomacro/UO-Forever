using System;
using Server;

namespace Server.Items
{
	public class VoteEarringsThree : GoldEarrings
	{
		public override int ArtifactRarity{ get{ return 11; } }

		[Constructable]
		public VoteEarringsThree()
		{
			Name = "Dexxer's Delight";
            Hue = 73;
		}

		public VoteEarringsThree( Serial serial ) : base( serial )
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

			if ( Hue == 73 )
				Hue = 73;
		}
	}
}