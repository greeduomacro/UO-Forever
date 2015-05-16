using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	public class HolidayPoinsettia : BaseHolidayGift
	{
		[Constructable]
		public HolidayPoinsettia(int year)
		{
            switch (Utility.Random(2))
            {
                case 0: ItemID = 0x2330; break;
                case 1: ItemID = 0x2331; break;
            }

            HolidayName = "Christmas";
            HolidayYear = year;

			Weight = 1.0;
			LootType = LootType.Blessed;
		}

		public HolidayPoinsettia( Serial serial ) : base( serial )
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