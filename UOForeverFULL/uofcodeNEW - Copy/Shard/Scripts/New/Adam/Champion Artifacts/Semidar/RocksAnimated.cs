using System;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{

	public class RockAnimatedArtifact : Item
	{
		[Constructable]
		public RockAnimatedArtifact()
            :base(13446)
		{
		    Name = "rock";
		}

        public RockAnimatedArtifact(Serial serial)
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