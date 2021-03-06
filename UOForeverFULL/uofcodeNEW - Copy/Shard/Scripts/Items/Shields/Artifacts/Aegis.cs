using System;
using Server;

namespace Server.Items
{
	public class Aegis : HeaterShield
	{
		public override int LabelNumber{ get{ return 1061602; } } // �gis
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public Aegis()
		{
			Hue = 0x47E;
		}

		public Aegis( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}