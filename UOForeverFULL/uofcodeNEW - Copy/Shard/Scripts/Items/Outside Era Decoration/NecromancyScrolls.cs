using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    public class BloodOathScroll : SpellScroll
    {
        [Constructable]
        public BloodOathScroll()
            : this(1)
        {
        }

        [Constructable]
        public BloodOathScroll(int amount)
            : base(101, 0x2261, amount)
        {
        }

        public BloodOathScroll(Serial serial)
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

    public class CorpseSkinScroll : SpellScroll
    {
        [Constructable]
        public CorpseSkinScroll()
            : this(1)
        {
        }

        [Constructable]
        public CorpseSkinScroll(int amount)
            : base(102, 0x2262, amount)
        {
        }

        public CorpseSkinScroll(Serial serial)
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

    public class CurseWeaponScroll : SpellScroll
    {
        [Constructable]
        public CurseWeaponScroll()
            : this(1)
        {
        }

        [Constructable]
        public CurseWeaponScroll(int amount)
            : base(103, 0x2263, amount)
        {
        }

        public CurseWeaponScroll(Serial serial)
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

    public class EvilOmenScroll : SpellScroll
    {
        [Constructable]
        public EvilOmenScroll()
            : this(1)
        {
        }

        [Constructable]
        public EvilOmenScroll(int amount)
            : base(104, 0x2264, amount)
        {
        }

        public EvilOmenScroll(Serial serial)
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

    public class ExorcismScroll : SpellScroll
    {
        [Constructable]
        public ExorcismScroll()
            : this(1)
        {
        }

        [Constructable]
        public ExorcismScroll(int amount)
            : base(116, 0x2270, amount)
        {
        }

        public ExorcismScroll(Serial serial)
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

    public class HorrificBeastScroll : SpellScroll
    {
        [Constructable]
        public HorrificBeastScroll()
            : this(1)
        {
        }

        [Constructable]
        public HorrificBeastScroll(int amount)
            : base(105, 0x2265, amount)
        {
        }

        public HorrificBeastScroll(Serial serial)
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

    public class LichFormScroll : SpellScroll
    {
        [Constructable]
        public LichFormScroll()
            : this(1)
        {
        }

        [Constructable]
        public LichFormScroll(int amount)
            : base(106, 0x2266, amount)
        {
        }

        public LichFormScroll(Serial serial)
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

    public class MindRotScroll : SpellScroll
    {
        [Constructable]
        public MindRotScroll()
            : this(1)
        {
        }

        [Constructable]
        public MindRotScroll(int amount)
            : base(107, 0x2267, amount)
        {
        }

        public MindRotScroll(Serial serial)
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

    public class PainSpikeScroll : SpellScroll
    {
        [Constructable]
        public PainSpikeScroll()
            : this(1)
        {
        }

        [Constructable]
        public PainSpikeScroll(int amount)
            : base(108, 0x2268, amount)
        {
        }

        public PainSpikeScroll(Serial serial)
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

    public class PoisonStrikeScroll : SpellScroll
    {
        [Constructable]
        public PoisonStrikeScroll()
            : this(1)
        {
        }

        [Constructable]
        public PoisonStrikeScroll(int amount)
            : base(109, 0x2269, amount)
        {
        }

        public PoisonStrikeScroll(Serial serial)
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

    public class StrangleScroll : SpellScroll
    {
        [Constructable]
        public StrangleScroll()
            : this(1)
        {
        }

        [Constructable]
        public StrangleScroll(int amount)
            : base(110, 0x226A, amount)
        {
        }

        public StrangleScroll(Serial serial)
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

    public class SummonFamiliarScroll : SpellScroll
    {
        [Constructable]
        public SummonFamiliarScroll()
            : this(1)
        {
        }

        [Constructable]
        public SummonFamiliarScroll(int amount)
            : base(111, 0x226B, amount)
        {
        }

        public SummonFamiliarScroll(Serial serial)
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

    public class VampiricEmbraceScroll : SpellScroll
    {
        [Constructable]
        public VampiricEmbraceScroll()
            : this(1)
        {
        }

        [Constructable]
        public VampiricEmbraceScroll(int amount)
            : base(112, 0x226C, amount)
        {
        }

        public VampiricEmbraceScroll(Serial serial)
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

    public class VengefulSpiritScroll : SpellScroll
    {
        [Constructable]
        public VengefulSpiritScroll()
            : this(1)
        {
        }

        [Constructable]
        public VengefulSpiritScroll(int amount)
            : base(113, 0x226D, amount)
        {
        }

        public VengefulSpiritScroll(Serial serial)
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

    public class WitherScroll : SpellScroll
    {
        [Constructable]
        public WitherScroll()
            : this(1)
        {
        }

        [Constructable]
        public WitherScroll(int amount)
            : base(114, 0x226E, amount)
        {
        }

        public WitherScroll(Serial serial)
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

    public class WraithFormScroll : SpellScroll
    {
        [Constructable]
        public WraithFormScroll()
            : this(1)
        {
        }

        [Constructable]
        public WraithFormScroll(int amount)
            : base(115, 0x226F, amount)
        {
        }

        public WraithFormScroll(Serial serial)
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
