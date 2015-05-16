using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	[Flipable( 0x2328, 0x2329 )]
	public class HolidaySnowman : BaseHolidayGift
	{
		public static string GetRandomTitle()
		{
			// All hail OSI staff
			string[] titles = new string[]
				{
			        "Lord Hog Fred",
			        "God",
			        "Shakuhachi",
			        "Mallard",
                    "Unmentionable",
			        "Rudyom",
			        "The Avatar",
			        "Ian Ore",
			        "Karaktikus",
                    "Kul 'Unaro",
			        "Santa Claus",
			        "The Mongbat King",
			        "Kul 'Unaro",
			        "Skeldon",
			        "Dave the Chimp",
                    "Levante",
                    "El Hoggo",
                    "Ergon",
                    "Relgaro",
                    "Oscaro",
                    "Yarrick",
                    "Halfhitch"
				};

			if ( titles.Length > 0 )
				return titles[Utility.Random( titles.Length )];

			return null;
		}

		private string m_Title;

		[CommandProperty( AccessLevel.GameMaster )]
		public string Title
		{
			get{ return m_Title; }
			set{ m_Title = value; InvalidateProperties(); }
		}

        [Constructable]
        public HolidaySnowman(int year)
        {
            ItemID = 0x2328;

            HolidayName = "Christmas";
            HolidayYear = year;

            Weight = 10.0;
            Hue = Utility.RandomDyedHue();
            LootType = LootType.Blessed;

            m_Title = GetRandomTitle();
        }

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_Title != null )
				list.Add( 1062841, m_Title ); // ~1_NAME~ the HolidaySnowman
		}

		public bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted )
				return false;

			Hue = sender.DyedHue;

			return true;
		}

		public HolidaySnowman( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( (string) m_Title );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_Title = reader.ReadString();
					break;
				}
			}

			Utility.Intern( ref m_Title );
		}
	}
}