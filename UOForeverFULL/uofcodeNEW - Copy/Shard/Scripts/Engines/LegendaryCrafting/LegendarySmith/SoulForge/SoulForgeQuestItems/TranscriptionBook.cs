#region References

using System.Collections.Generic;
using Server.Mobiles;

#endregion

namespace Server.Engines.Craft
{
    public class TranscriptionBook : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Completed { get; set; }

        public List<Item> ScrollsFound { get; set; }

        [Constructable]
        public TranscriptionBook()
            : base(0xFF2)
        {
            Name = "a notepad";
            Weight = 1.0;
            Hue = 1160;
            LootType = LootType.Blessed;
            ScrollsFound = new List<Item>();
        }

        public TranscriptionBook(Serial serial)
            : base(serial)
        {}

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage(54,
                "You recall that Llewellyn wanted you to transcribe 5 different scrolls located in various dungeons.");
            from.SendMessage(54, "You currently have {0} scrolls transcribed on your notepad.", ScrollsFound.Count);
        }

        public bool AddScroll(Item scroll)
        {
            if (ScrollsFound != null && !ScrollsFound.Contains(scroll))
            {
                ScrollsFound.Add(scroll);
                if (ScrollsFound.Count == 5)
                {
                    var mobile = RootParentEntity as Mobile;
                    if (mobile != null)
                    {
                        mobile.SendMessage(54,
                            "You have transcribed the 5 scrolls Llewellyn asked you to.  You decide you should return to the library and upate him on your progress.");
                    }
                }
                return true;
            }
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int) 0);

            writer.Write(Completed);

            if (ScrollsFound == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(ScrollsFound.Count);

                foreach (Item item in ScrollsFound)
                {
                    writer.Write(item);
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            ScrollsFound = new List<Item>();

            Completed = reader.ReadBool();

            int scrollscount = reader.ReadInt();

            if (scrollscount > 0)
            {
                for (int i = 0; i < scrollscount; i++)
                {
                    Item p = reader.ReadItem();
                    ScrollsFound.Add(p);
                }
            }
        }
    }
}