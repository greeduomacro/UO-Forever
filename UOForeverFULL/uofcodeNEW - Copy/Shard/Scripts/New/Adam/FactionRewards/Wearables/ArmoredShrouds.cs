using System;
using Server;
using Server.Engines.VeteranRewards;
using Server.Factions;

namespace Server.Items
{
    public class MinaxArmoredFactionRobe : BaseOuterTorso
    {
        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (parent is Mobile)
            {
                ((Mobile)parent).VirtualArmorMod += 4;
            }
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is Mobile)
            {
                ((Mobile)parent).VirtualArmorMod -= 4;
            }
        }

        public override bool Dye(Mobile from, IDyeTub sender)
        {
            return false;
        }

        [Constructable]
        public MinaxArmoredFactionRobe()
            : base(0x2684)
        {
            Name = "armored shroud";
            Weight = 3.0;
            LootType = LootType.Blessed;
            FactionItem.Imbue(this, Minax.Instance, false, Minax.Instance.Definition.HuePrimary);
        }

        public MinaxArmoredFactionRobe(Serial serial)
            : base(serial)
        { }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "[Minax]", Minax.Instance.Definition.HuePrimary);
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

            switch (version)
            {
                case 0:
                    {
                        break;
                    }
            }

            if (Parent is Mobile)
            {
                ((Mobile)Parent).VirtualArmorMod += 4;
            }
        }
    }

    public class TBArmoredFactionRobe : BaseOuterTorso
    {
        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (parent is Mobile)
            {
                ((Mobile)parent).VirtualArmorMod += 4;
            }
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is Mobile)
            {
                ((Mobile)parent).VirtualArmorMod -= 4;
            }
        }

        public override bool Dye(Mobile from, IDyeTub sender)
        {
            return false;
        }

        [Constructable]
        public TBArmoredFactionRobe()
            : base(0x2684)
        {
            Name = "armored shroud";
            Weight = 3.0;
            LootType = LootType.Blessed;
            FactionItem.Imbue(this, TrueBritannians.Instance, false, TrueBritannians.Instance.Definition.HuePrimary);
        }

        public TBArmoredFactionRobe(Serial serial)
            : base(serial)
        { }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "[True Britannians]", TrueBritannians.Instance.Definition.HuePrimary);
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

            switch (version)
            {
                case 0:
                    {
                        break;
                    }
            }

            if (Parent is Mobile)
            {
                ((Mobile)Parent).VirtualArmorMod += 4;
            }
        }
    }

    public class CoMArmoredFactionRobe : BaseOuterTorso
    {
        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (parent is Mobile)
            {
                ((Mobile)parent).VirtualArmorMod += 4;
            }
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is Mobile)
            {
                ((Mobile)parent).VirtualArmorMod -= 4;
            }
        }

        public override bool Dye(Mobile from, IDyeTub sender)
        {
            return false;
        }

        [Constructable]
        public CoMArmoredFactionRobe()
            : base(0x2684)
        {
            Name = "armored shroud";
            Weight = 3.0;
            LootType = LootType.Blessed;
            FactionItem.Imbue(this, CouncilOfMages.Instance, false, CouncilOfMages.Instance.Definition.HuePrimary);
        }

        public CoMArmoredFactionRobe(Serial serial)
            : base(serial)
        { }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "[Council of Mages]", CouncilOfMages.Instance.Definition.HuePrimary);
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

            switch (version)
            {
                case 0:
                    {
                        break;
                    }
            }

            if (Parent is Mobile)
            {
                ((Mobile)Parent).VirtualArmorMod += 4;
            }
        }
    }
    public class SLArmoredFactionRobe : BaseOuterTorso
    {
        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (parent is Mobile)
            {
                ((Mobile)parent).VirtualArmorMod += 4;
            }
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is Mobile)
            {
                ((Mobile)parent).VirtualArmorMod -= 4;
            }
        }

        public override bool Dye(Mobile from, IDyeTub sender)
        {
            return false;
        }

        [Constructable]
        public SLArmoredFactionRobe()
            : base(0x2684)
        {
            Name = "armored shroud";
            Weight = 3.0;
            LootType = LootType.Blessed;
            FactionItem.Imbue(this, Shadowlords.Instance, false, Shadowlords.Instance.Definition.HuePrimary);
        }

        public SLArmoredFactionRobe(Serial serial)
            : base(serial)
        { }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "[Shadowlords]", Shadowlords.Instance.Definition.HuePrimary);
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

            switch (version)
            {
                case 0:
                    {
                        break;
                    }
            }

            if (Parent is Mobile)
            {
                ((Mobile)Parent).VirtualArmorMod += 4;
            }
        }
    }
}