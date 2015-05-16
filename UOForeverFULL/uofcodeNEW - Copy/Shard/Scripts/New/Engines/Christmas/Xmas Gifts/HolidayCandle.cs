using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
    [Flipable(0x2373, 0x236E)]
    public class HolidayCandle : BaseHolidayGift
    {
        public static string GetRandomTitle()
        {
            string[] titles = new string[]
				{
			        "Shane",
			        "Carl",
			        "Alan",
			        "Jake",
                    "Nate",
			        "Adam"
                };

            if (titles.Length > 0)
                return titles[Utility.Random(titles.Length)];

            return null;
        }

        private string m_Title;

        [CommandProperty(AccessLevel.GameMaster)]
        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; InvalidateProperties(); }
        }

        [Constructable]
        public HolidayCandle(int year)
        {
            ItemID = 0x236E;

            HolidayName = "Christmas";
            HolidayYear = year;

            Weight = 10.0;
            Hue = Utility.RandomDyedHue();
            LootType = LootType.Blessed;

            Light = LightType.Circle300;
            m_Title = GetRandomTitle();
        }

        public override int LabelNumber { get { return 1070875; } }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Title != null)
                list.Add(1070881, m_Title);
        }

        public bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;

            return true;
        }

        public HolidayCandle(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((string)m_Title);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_Title = reader.ReadString();
                        break;
                    }
            }
        }
    }
}
