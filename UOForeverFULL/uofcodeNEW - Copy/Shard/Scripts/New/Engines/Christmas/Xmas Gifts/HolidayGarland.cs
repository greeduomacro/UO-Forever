using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x3BBB, 0x3BBC )]
	public class HolidayGarland : BaseHolidayGift
	{		
		[Constructable]
		public HolidayGarland(int year)
		{
            ItemID = 0x3BBB;

            HolidayName = "Christmas";
            HolidayYear = year;

			Name = "Holiday Garland";
			Weight = 2.0;
			LootType = LootType.Blessed;
		}

		public HolidayGarland( Serial serial ) : base( serial )
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

