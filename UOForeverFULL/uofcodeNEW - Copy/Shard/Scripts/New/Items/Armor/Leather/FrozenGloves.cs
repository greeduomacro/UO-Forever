using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Items
{
	public class FrozenGloves : LeatherGloves
	{
		public override int InitMinHits{ get{ return 10; } }
		public override int InitMaxHits{ get{ return 25; } }

		public override int ArtifactRarity{ get{ return 5; } }
		public override string DefaultName{ get{ return "frozen leather gloves"; } }
		public override int LabelNumber{ get{ return 0; } }

		[Constructable]
		public FrozenGloves() : base()
		{
			Weight = 0.75;
			Hue = 0.30 > Utility.RandomDouble() ? 1261 : Utility.Random( 1361, 6 );
		}

		public FrozenGloves( Serial serial ) : base( serial )
		{
		}

		public override void OnRemoved( object parent )
		{
			Mobile from = parent as Mobile;

			if ( from != null && from.FindItemOnLayer(Layer.TwoHanded) is GlacialStaff )
			{
                from.Damage(Utility.Random(15, 5), from);
				from.PlaySound( 0x10B );
				from.SendMessage( "Your fingers freeze as they grip the staff." );
			}
			base.OnRemoved( parent );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}
}