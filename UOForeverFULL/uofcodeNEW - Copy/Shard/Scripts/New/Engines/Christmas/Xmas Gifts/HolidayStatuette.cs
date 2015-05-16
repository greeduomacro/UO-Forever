using System;
using Server;
using Server.Items;
using Server.Multis;
using Server.Mobiles;

namespace Server.Items
{
	[Flipable( 0x1224, 0x139A )]
	public class HolidayStatuette : BaseHolidayGift
	{
		public static T RandomList<T>( T[] list )
		{
     			return list[Utility.Random( list.Length )];
		}

		[Constructable]
		public HolidayStatuette(int year)
		{
            ItemID = 0x1224;

            HolidayName = "Christmas";
            HolidayYear = year;

			Name = "a statue of " + RandomList<string>( m_Names );
			Hue = Utility.RandomList( m_Hues );
			LootType = LootType.Blessed;
		}

		public override bool ForceShowProperties { get { return ObjectPropertyList.Enabled && EraAOS; } }

		private static int[] m_Hues = new int[]
		{
			0x6,
			0xB,
			0x10,
			0x15,
			0x1A,
			0x1F,
			0x24,
			0x29,
			0x2E,
			0x33,
			0x38,
			0x3D,
			0x42,
			0x47,
			0x4C,
			0x51,
			0x56,
			0x5B,
			0x60,
			0x65,
			0x47E,
			0x497,
		};

		private static string[] m_Names = new string[]
		{
			
			"Owner Shane",
		    "Admin Carl",
			"Dev Alan",
			"Dev Adam",
		};

		public HolidayStatuette( Serial serial ) : base( serial )
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
