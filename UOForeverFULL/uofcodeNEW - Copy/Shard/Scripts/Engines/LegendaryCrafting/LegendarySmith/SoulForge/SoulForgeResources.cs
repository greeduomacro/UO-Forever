using System;
using System.Collections.Generic;
using System.Text;
using Server.Items;

namespace Server.Engines.Craft
{
    public class HarrowerSoul : Item
    {
        [Constructable]
        public HarrowerSoul()
            : base(3982)
        {
            Name = "Soul of The Harrower";
            Stackable = false;
            Weight = 16.0;
            Hue = 38;
        }

        public HarrowerSoul(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "The demonic soul of The Harrower swirls within the dark depths of the crystal.", 1100);
            base.OnSingleClick(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DevourerSoul : Item
    {
        [Constructable]
        public DevourerSoul()
            : base(3982)
        {
            Name = "Soul of The Devourer";
            Stackable = false;
            Weight = 16.0;
            Hue = 1150;
        }

        public DevourerSoul(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "The malevolent soul of the devourer is housed within this crystal.", 1100);
            base.OnSingleClick(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class HeartofRikktor : Item
    {
        [Constructable]
        public HeartofRikktor()
            : base(10248)
        {
            Name = "Rikktor's Smouldering Heart";
            Stackable = false;
            Weight = 16.0;
        }

        public HeartofRikktor(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "The smouldering heart of Rikktor, carved from the Dragon-King's chest while he still lived.", 2049);
            base.OnSingleClick(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DragonBoneShards : Item
    {
        [Constructable]
        public DragonBoneShards()
            : base(0x1B1A)
        {
            Name = "dragon bone shards";
            Stackable = false;
            Weight = 1.0;
            Hue = 2498;
        }

        public DragonBoneShards(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DragonHeart : Item
    {
        [Constructable]
        public DragonHeart()
            : base(10248)
        {
            Name = "an intact dragon's heart";
            Stackable = false;
            Weight = 1.0;
            Hue = 2718;
        }

        public DragonHeart(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
