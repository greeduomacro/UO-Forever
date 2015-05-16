using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class AltRealmFragment : Item
	{
		public override string DefaultName{ get{ return "fragments from an alternate realm"; } }

		[Constructable]
		public AltRealmFragment() : base( 0x400A )
		{
			Hue = 2403;
			LootType = LootType.Blessed;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.BankBox ) )
			{
				double rnd = Utility.RandomDouble();

				if ( rnd > 0.65 )
					from.BankBox.DropItem( new UnholySkullDeed() );
				else if ( rnd > 0.30 )
					from.BankBox.DropItem( new HolyBlessStatue() );
				else if ( rnd > 0.15 )
					from.BankBox.DropItem( BaseCreature.Renamed( BaseCreature.ChangeLootType( new Sandals( 1 ), LootType.Blessed ), "sandals of an unknown origin" ) );
				else
					from.BankBox.DropItem( BaseCreature.Renamed( BaseCreature.ChangeLootType( BaseCreature.Rehued( new Spellbook( ulong.MaxValue ), 2205 ), LootType.Blessed ), "book of the forgotten" ) );

				from.SendLocalizedMessage( 1072224 ); // An item has been placed in your bank box.
				Delete();
			}
			else
				from.SendMessage( "You must have this in your bank box to use it." );
		}

		public AltRealmFragment( Serial serial ) : base( serial )
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