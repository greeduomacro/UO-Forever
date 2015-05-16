namespace Server.Items
{
    [Flipable(7923, 7905)]
    public class TBEffigy : Item
    {
        [Constructable]
        public TBEffigy()
            : base(7923)
        {
            Name = "sacred effigy";
            Weight = 2.0;
            LootType = LootType.Blessed;
            Hue = 2214;
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "[True Britannians]", 2214);
            base.OnSingleClick(from);
        }

        public TBEffigy(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [Flipable(7923, 7905)]
    public class CoMEffigy : Item
    {
        [Constructable]
        public CoMEffigy()
            : base(7923)
        {
            Name = "sacred effigy";
            Weight = 2.0;
            LootType = LootType.Blessed;
            Hue = 1325;
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "[Council of Mages]", 1325);
            base.OnSingleClick(from);
        }

        public CoMEffigy(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [Flipable(7923, 7905)]
    public class MinaxEffigy : Item
    {
        [Constructable]
        public MinaxEffigy()
            : base(7923)
        {
            Name = "sacred effigy";
            Weight = 2.0;
            LootType = LootType.Blessed;
            Hue = 1645;
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "[Minax]", 1645);
            base.OnSingleClick(from);
        }

        public MinaxEffigy(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [Flipable(7923, 7905)]
    public class SLEffigy : Item
    {
        [Constructable]
        public SLEffigy()
            : base(7923)
        {
            Name = "sacred effigy";
            Weight = 2.0;
            LootType = LootType.Blessed;
            Hue = 1175;
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "[Shadowlords]", 1175);
            base.OnSingleClick(from);
        }

        public SLEffigy(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}