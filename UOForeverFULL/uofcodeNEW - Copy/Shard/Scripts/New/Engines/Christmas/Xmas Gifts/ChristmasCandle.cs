using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
    [Flipable(0x2373, 0x236E)]
    public class ChristmasCandle : BaseHolidayGift
    {
        public static string GetRandomTitle()
        {
            string[] titles = new string[]
				{
			        "Rudolph",
			        "Santa Claus",
			        "Prancer",
                    "Vixen",
                    "Dancer",
                    "Comet",
                    "Cupid",
                    "Donner",
                    "Blitzen",
				};

            if (titles.Length > 0)
                return titles[Utility.Random(titles.Length)];

            return null;
        }

        [Constructable]
        public ChristmasCandle(int hue, string title)
            : base(0x236E)
        {

            Weight = 10.0;
            Hue = 33;
            LootType = LootType.Blessed;
            Light = LightType.Circle300;
    //        m_Title = title;
      //  }

        //public override int LabelNumber { get { return 1070875; } }

       // public override void GetProperties(ObjectPropertyList list)
        //{
          //  base.GetProperties(list);

            //if (m_Title != null)
              //  list.Add(m_Title);
        
        }

        public ChristmasCandle(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

          //  writer.Write((int)1); // version

        //    writer.Write((string)m_Title);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            //int version = reader.ReadInt();

           // switch (version)
           // {
                //case 1:
             //       {
            //            m_Title = reader.ReadString();
              //          break;
             //       }
            //}
        }
    }
}
