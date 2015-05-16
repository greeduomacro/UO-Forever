using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Mobiles;

namespace Server.Items
{
    public enum AsylumKeyType
    {
        None,
        Lower,
        Middle,
        Upper
    }

    class AsylumKey : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public AsylumKeyType KeyType { get; set; }

        [Constructable]
        public AsylumKey()
            : base(16651)
        {
            Name = "a magical asylum key";
            Hue = 1174;
            Movable = true;
        }

        public AsylumKey(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write((int)KeyType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        KeyType = (AsylumKeyType)reader.ReadInt();
                    }
                    break;
            }
        }
    }
}
