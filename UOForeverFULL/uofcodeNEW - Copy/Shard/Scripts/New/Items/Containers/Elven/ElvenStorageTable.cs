using System;
using System.Collections;
using Server;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
	[Furniture]
	[Flipable( 0x2DE9, 0x2DEA )]
	public class ElvenStorageTable : BaseContainer
	{
		public override int DefaultGumpID{ get{ return 0x10C; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 80, 5, 140, 70 ); }
		}

		[Constructable]
		public ElvenStorageTable() : base( 0x2DE9 )
		{
			Weight = 13.0;
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public ElvenStorageTable( Serial serial ) : base( serial )
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