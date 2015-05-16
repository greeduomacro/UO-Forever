using System;
using Server;
using Server.Gumps;
using Server.Network;
/*
** IOU
** updated 1/3/04
** ArteGordon
** adds a simple item that displays text messages in a scroll gump.  The size can be varied and the note text and text-color can be specified.
** The title of the note and its color can also be set.
*/
namespace Server.Items
{
    public class IOU : Item
    {
        private string m_NoteString;

        [Constructable]
        public IOU()
            : base(0x14EE)
        {
            Name = "Congratulations! This note is good for one special item from the virtually unique rare item list. That list will be ready very soon, so hold on to this!";
        }

        public IOU(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string NoteString
        {
            get { return m_NoteString; }
            set { m_NoteString = value; InvalidateProperties(); }
        }


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version 

            writer.Write(this.m_NoteString);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 0:
                    {
                        this.m_NoteString = reader.ReadString();
                    }
                    break;
            }
        }

    }
}
