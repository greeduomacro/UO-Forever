#region References

using System;
using System.Drawing;
using System.Linq;
using Server.Engines.ZombieEvent;
using Server.Mobiles;
using Server.Network;
using VitaNex.Notify;

#endregion

namespace Server.Items
{
    public class TheCure : Item
    {
        [Constructable]
        public TheCure()
            : base(6212)
        {
            Name = "The Cure";
            Hue = 61;
        }

        public TheCure(Serial serial)
            : base(serial)
        {}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "A potent looking liquid swirls around in the flask.", 61);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}