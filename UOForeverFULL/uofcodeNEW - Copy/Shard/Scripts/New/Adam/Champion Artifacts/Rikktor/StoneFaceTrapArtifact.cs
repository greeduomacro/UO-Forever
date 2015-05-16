using System;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{

	public class StoneFaceTrapNoDamageArtifact : StoneFaceTrap
	{
		[Constructable]
		public StoneFaceTrapNoDamageArtifact()
		{
		    Name = "stone effigy";
            Type = StoneFaceTrapType.NorthWestWall;
		    Movable = true;
		}

		public StoneFaceTrapNoDamageArtifact( Serial serial ) : base( serial )
		{
		}

		public override void TriggerDamage()
		{
			// nothing..
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