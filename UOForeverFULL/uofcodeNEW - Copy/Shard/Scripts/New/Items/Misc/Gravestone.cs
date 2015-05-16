using System;
using Server;

namespace Server.Items
{
	public class Gravestone : Item
	{
		[CommandProperty( AccessLevel.GameMaster )]
		public int Style{ get{ return GetStyle(); } set{ ItemID = GetItemID( value ); } }

		[Constructable]
		public Gravestone() : this( Utility.Random( 43 ) )
		{
		}

		[Constructable]
		public Gravestone( int index ) : base( GetItemID( index ) )
		{
			Weight = 10;
		}

		public int GetStyle()
		{
			if ( ItemID >= 0x1165 && ItemID <= 0x1184 )
				return ItemID - 0x1165;

			if ( ItemID >= 0xED4 && ItemID <= 0xEDE )
				return ItemID - 0xED4;

			return 0;
		}

		public static int GetItemID( int index )
		{
			if ( index >= 0 && index < 32 )
				return 0x1165 + index;
			else if ( index < 43 )
				return 0xED4 + (index - 33);

			return 0xED4;
		}

		public Gravestone( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}