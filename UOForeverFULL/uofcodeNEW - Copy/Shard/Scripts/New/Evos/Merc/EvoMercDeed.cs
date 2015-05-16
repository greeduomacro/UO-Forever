//////////////////////////////////////////////////$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
////==========================================////$                                 $
////        Upgraded By: Triple               ////$   Will like to thank for all    $
////==========================================////$   who help me with this upgrade!$
//////////////////////////////////////////////////$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$

using System; 
using System.Collections;
using Server.Items; 
using Server.Mobiles; 
using Server.Misc;
using Server.Network;

namespace Server.Items
{

	public class EvoMercDeed : Item // Create the item class which is derived from the base item class
	{
		[Constructable]
		public EvoMercDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
			Name = "A Mercenary Contract";
			LootType = LootType.Blessed;
		}

		public EvoMercDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			LootType = LootType.Blessed;

			int version = reader.ReadInt();
		}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from ) // Override double click of the deed to call our target
		{
			if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack
			{
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else if ( (from.Followers + 3) < from.FollowersMax + 1 )
			{
				this.Delete();

				from.SendMessage( "You use the contract." );

				EvoMerc merc = new EvoMerc();

         			merc.Map = from.Map; 
         			merc.Location = from.Location; 

				merc.Controlled = true;

				merc.ControlMaster = from;

				merc.IsBonded = true;

			}
			else
			{
				from.SendMessage( "You have too many followers to hire your Mercenary." );			
			}
		}	
	}
}


