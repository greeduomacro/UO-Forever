using System;
using Server;
using Server.Multis;
using Server.Gumps;

namespace Server.Items
{
	public class HolidaySnowglobe : BaseHolidayGift
	{
		public override bool IsAccessibleTo( Mobile m )
		{
			if ( !BaseHouse.CheckAccessible( m, this ) )
				return true;

			return base.IsAccessibleTo( m );
		}

		public override bool ForceShowProperties { get { return ObjectPropertyList.Enabled && EraAOS; } }

		public static T RandomList<T>( T[] list )
		{
     			return list[Utility.Random( list.Length )];
		}

		[Constructable]
		public HolidaySnowglobe(int year)
		{
            ItemID = 0xE2E;

            HolidayName = "Christmas";
            HolidayYear = year;

			Name = "a snowy scene of " + RandomList<string>( m_Names );
			LootType = LootType.Blessed;
		}

		private static string[] m_Names = new string[]
		{
			"Britain",
			"Buccaneers Den",
			"Cove",
			"Delucia",
			"Empath Abbey",
			"Jhelom",
			"Magincia",
			"Minoc",
			"Moonglow",
			"Nujel'm",
			"Occlo",
			"Papua",
			"Serpent's Hold",
			"Skara Brae",
			"The Lycaeum",
			"Trinsic",
			"Vesper",
			"Wind",
			"Yew",
            "Luna",
            "Umbra",
            "Zento",
			"Shrine of Humility",
			"Shrine of Sacrifice",
			"Shrine of Compassion",
			"Shrine of Honor",
			"Shrine of Honesty",
			"Shrine of Spirituality",
			"Shrine of Justice",
			"Shrine of Valor",
			"Lord Hog Fred's Castle"
		};
		
		public HolidaySnowglobe( Serial serial ) : base( serial )
		{
		}

      	public override void OnDoubleClick( Mobile from )
      	{
      		from.SendGump( new SnowglobeGump(from) );
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
