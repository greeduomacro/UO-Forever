using System;
using Server;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{

	public class StaffTravelingBook : Item
	{

		[Constructable]
		public StaffTravelingBook () : this( null )
		{
		}

		[Constructable]
		public StaffTravelingBook ( string name ) : base ( 0x0FBD )
		{
			Name = "Staff Traveling Book";
			LootType = LootType.Blessed;
			Hue = 1173;
		}

		public StaffTravelingBook ( Serial serial ) : base ( serial )
		{
		}

      		public override void OnDoubleClick( Mobile from ) 
      		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 );
			}
			else
			{ 
				GoGump.DisplayTo( from );
 
			}
		}

		public override void Serialize ( GenericWriter writer)
		{
			base.Serialize ( writer );

			writer.Write ( (int) 0);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize ( reader );

			int version = reader.ReadInt();
		}
	}
}
