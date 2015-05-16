using System;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0x1441, 0x1440 )]
	public class KoonsKryssArtifact : Kryss
	{
		[Constructable]
		public KoonsKryssArtifact()
		{
		    Name = "barracoon's putrid kryss";
			Weight = 8.0;
		    Hue = 2966;
		    Identified = true;
		}

        public KoonsKryssArtifact(Serial serial)
            : base(serial)
		{
		}

        public override void OnSingleClick(Mobile m)
        {
            base.OnSingleClick(m);
            LabelTo(m,
                "[Champion Artifact]",
                134);
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}