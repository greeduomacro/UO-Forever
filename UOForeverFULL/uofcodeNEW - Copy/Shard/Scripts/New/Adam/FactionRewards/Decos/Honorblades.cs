namespace Server.Items
{
    [Flipable(10326, 10325)]
    public class TBHonorBlade : Item
    {
        [Constructable]
        public TBHonorBlade()
            : base(10326)
        {
            Name = "faction honorblade";
            Weight = 2.0;
            LootType = LootType.Blessed;
            Hue = 2214;
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "[True Britannians]", 2214);
            base.OnSingleClick(from);
        }

        public TBHonorBlade(Serial serial)
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

    [Flipable(10326, 10325)]
    public class CoMHonorBlade : Item
    {
        [Constructable]
        public CoMHonorBlade()
            : base(10326)
        {
            Name = "faction honorblade";
            Weight = 2.0;
            LootType = LootType.Blessed;
            Hue = 1325;
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "[Council of Mages]", 1325);
            base.OnSingleClick(from);
        }

        public CoMHonorBlade(Serial serial)
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

    [Flipable(10326, 10325)]
    public class MinaxHonorBlade : Item
    {
        [Constructable]
        public MinaxHonorBlade()
            : base(10326)
        {
            Name = "faction honorblade";
            Weight = 2.0;
            LootType = LootType.Blessed;
            Hue = 1645;
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "[Minax]", 1645);
            base.OnSingleClick(from);
        }

        public MinaxHonorBlade(Serial serial)
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

    [Flipable(10326, 10325)]
    public class SLHonorBlade : Item
    {
        [Constructable]
        public SLHonorBlade()
            : base(10326)
        {
            Name = "faction honorblade";
            Weight = 2.0;
            LootType = LootType.Blessed;
            Hue = 1175;
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "[Shadowlords]", 1175);
            base.OnSingleClick(from);
        }

        public SLHonorBlade(Serial serial)
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