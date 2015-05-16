using System;
using Server;
using Server.Items;
using Server.Engines.Craft;

namespace Server.Items
{
    public sealed class StoneScale : Item
    {
        [Constructable]
        public StoneScale()
            : base(9908)
        {
            Name = "a stone dragon scale";
            Weight = 2;
            Hue = 2407;
        }

        public StoneScale(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(0);

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.GetVersion();

        }
    }
    public sealed class StoneFeather : Item
    {
        [Constructable]
        public StoneFeather()
            : base(7121)
        {
            Name = "a stone harpy feather";
            Weight = 2;
            Hue = 2407;
        }

        public StoneFeather(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(0);

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.GetVersion();

        }
    }

    public sealed class StoneEye : Item
    {
        [Constructable]
        public StoneEye()
            : base(3859)
        {
            Name = "a stone gargoyle eye";
            Weight = 2;
            Hue = 2407;
        }

        public StoneEye(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(0);

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.GetVersion();

        }
    }

    public sealed class StoneClaw : Item
    {
        [Constructable]
        public StoneClaw()
            : base(11704)
        {
            Name = "a basalisk claw";
            Weight = 2;
            Hue = 2407;
        }

        public StoneClaw(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(0);

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.GetVersion();

        }
    }

    public sealed class GoldenNeedle : Item
    {
        [Constructable]
        public GoldenNeedle()
            : base(2548)
        {
            Name = "a golden needle";
            Weight = 2;
            Hue = 1177;
        }

        public GoldenNeedle(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(0);

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.GetVersion();

        }
    }
}