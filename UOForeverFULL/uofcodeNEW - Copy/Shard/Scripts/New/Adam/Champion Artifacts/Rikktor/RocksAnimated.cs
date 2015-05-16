using System;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{

	public class RocksAnimatedArtifact : Item
	{
		[Constructable]
		public RocksAnimatedArtifact()
            :base(13451)
		{
		    Name = "rocks";
		}

        public RocksAnimatedArtifact(Serial serial)
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