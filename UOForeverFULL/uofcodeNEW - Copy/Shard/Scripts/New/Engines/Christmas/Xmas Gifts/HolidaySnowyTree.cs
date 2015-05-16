using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	public class HolidaySnowyTree : BaseHolidayGift
	{
		[Constructable]
		public HolidaySnowyTree(int year)
		{
            ItemID = 0x2377;

            HolidayName = "Christmas";
            HolidayYear = year;

			Weight = 1.0;
			LootType = LootType.Blessed;
		}

		public HolidaySnowyTree( Serial serial ) : base( serial )
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

			int version = reader.ReadInt();
		}
	}
}