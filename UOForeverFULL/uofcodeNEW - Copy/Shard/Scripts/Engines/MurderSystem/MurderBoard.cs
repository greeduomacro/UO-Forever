using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Menus;
using Server.Menus.Questions;
using Server.Items;

namespace Server.Engines.MurderSystem
{
    [FlipableAttribute(7774, 7775)]
    public class MurderBoard : Item
    {
        [Constructable]
        public MurderBoard()
            : base(7774)
        {
            Movable = false;
            Name = "Top Murderer Board";
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 2))
            {
                from.CloseGump(typeof(MurderBoardGump));
                from.SendGump(new MurderBoardGump(from, 0, null, null, 1));
            }
        }

        public MurderBoard(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}