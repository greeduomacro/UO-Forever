using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x8EA, 0x8E9 )]
	public class SnowPile1 : BaseHolidayGift
	{		
		[Constructable]
		public SnowPile1(int year)
		{
            ItemID = 0x8EA;

            HolidayName = "Christmas";
            HolidayYear = year;

			Name = "Pile of Snow";
			Hue = 1153;
			Weight = 2.0;
			LootType = LootType.Blessed;
		}

		public SnowPile1( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
	
	[FlipableAttribute( 0x8E4, 0x8E5 )]
    public class SnowPile2 : BaseHolidayGift
	{		
		[Constructable]
		public SnowPile2(int year)
		{
            ItemID = 0x8E4;

            HolidayName = "Christmas";
            HolidayYear = year;

			Name = "Pile of Snow";
			Hue = 1153;
			Weight = 2.0;
			LootType = LootType.Blessed;
		}

		public SnowPile2( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
	
	[FlipableAttribute( 0x8E0, 0x8E1 )]
    public class SnowPile3 : BaseHolidayGift
	{		
		[Constructable]
		public SnowPile3(int year)
		{
            ItemID = 0x8E0;

            HolidayName = "Christmas";
            HolidayYear = year;

			Name = "Pile of Snow";
			Hue = 1153;
			Weight = 2.0;
			LootType = LootType.Blessed;
		}

		public SnowPile3( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}


    public class SnowPile4 : BaseHolidayGift
	{		
		[Constructable]
		public SnowPile4(int year)
		{
            ItemID = 0x8E7;

            HolidayName = "Christmas";
            HolidayYear = year;

			Name = "Pile of Snow";
			Hue = 1153;
			Weight = 2.0;
			LootType = LootType.Blessed;
		}

		public SnowPile4( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}


    public class SnowPile5 : BaseHolidayGift
	{		
		[Constructable]
		public SnowPile5(int year)
		{
            ItemID = 0x8E3;

            HolidayName = "Christmas";
            HolidayYear = year;

			Name = "Pile of Snow";
			Hue = 1153;
			Weight = 2.0;
			LootType = LootType.Blessed;
		}

		public SnowPile5( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}

