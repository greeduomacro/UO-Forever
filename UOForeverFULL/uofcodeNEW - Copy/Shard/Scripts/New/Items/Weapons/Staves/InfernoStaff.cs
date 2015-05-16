using System;
using Server;

namespace Server.Items
{
	public class InfernoStaff : BlackStaff
	{
		public override int ArtifactRarity { get{ return 7; } }
		public override string DefaultName{ get{ return "an infernal staff"; } }
		public override int LabelNumber{ get{ return 0; } }

		[Constructable]
		public InfernoStaff() : base()
		{
			Hue = 1360;
			//Attributes.SpellChanneling = 1;
			StrRequirement = 45;
			Identified = true;
		}

		public override int InitMinHits{ get{ return 35; } }
		public override int InitMaxHits{ get{ return 40; } }

		public InfernoStaff( Serial serial ) : base( serial )
		{
		}

		public override void OnAdded( object parent )
		{
			Mobile from = parent as Mobile;
			if ( from != null && !( from.FindItemOnLayer( Layer.Gloves ) is BurntGloves ) && Utility.RandomBool() )
			{
				from.Damage( Utility.Random( 5, 11 ), from );
				from.PlaySound( 84 );
				from.SendMessage( "Your fingers burn as they grip the staff." );
			}

			if ( from != null )
				base.OnAdded( parent );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}