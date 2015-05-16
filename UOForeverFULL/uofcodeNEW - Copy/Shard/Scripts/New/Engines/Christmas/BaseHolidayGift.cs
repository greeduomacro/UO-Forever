using System;
using Server;
using Server.Engines.Craft;
using Server.Items;

namespace Server.Items
{
	public class BaseHolidayGift : Item
	{
        private string m_HolidayName;
        private int m_HolidayYear;

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual string HolidayName
        {
            get { return m_HolidayName; }
            set { m_HolidayName = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int HolidayYear
        {
            get { return m_HolidayYear; }
            set { m_HolidayYear = value; }
        }

        [Constructable]
        public BaseHolidayGift(int itemID, string name, string holName, int holYear)
		{
            ItemID = itemID;
            Name = name;

            m_HolidayName = holName;
            m_HolidayYear = holYear;

            Weight = 1.0;
			LootType = LootType.Blessed;
		}

        public BaseHolidayGift()
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

		public BaseHolidayGift( Serial serial ) : base( serial )
		{
		}

        public override bool ForceShowProperties { get { return true; } }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060662, "{0}\t{1}", m_HolidayName, m_HolidayYear);
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

            writer.Write((int)m_HolidayYear);
            writer.Write((string)m_HolidayName);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_HolidayYear = reader.ReadInt();
                        m_HolidayName = reader.ReadString();
                        break;
                    }
            }
		}
	}
}