using VitaNex.Modules.AutoPvP.Battles;

namespace Server.Items
{
    public class TemplateGreaterHealPotion : GreaterHealPotion
    {
        [Constructable]
        public TemplateGreaterHealPotion()
        {
            Weight = 0;
            Stackable = true;
            Amount = 20;
            Association = 51;
        }

        public TemplateGreaterHealPotion(Serial serial)
            : base(serial)
        { }

        public override void OnSingleClick(Mobile m)
        {
            LabelTo(m, "[Battles]", 54);

            base.OnSingleClick(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }
    public class TemplateGreaterExplosionPotion : GreaterExplosionPotion
    {
        [Constructable]
        public TemplateGreaterExplosionPotion()
        {
            Weight = 0;
            Stackable = true;
            //Amount = 20;
            Association = 51;
        }

        public TemplateGreaterExplosionPotion(Serial serial)
            : base(serial)
        { }

        public override void OnSingleClick(Mobile m)
        {
            LabelTo(m, "[Battles]", 54);

            base.OnSingleClick(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }
    public class TemplateGreaterCurePotion : GreaterCurePotion
    {
        [Constructable]
        public TemplateGreaterCurePotion()
        {
            Weight = 0;
            Stackable = true;
            Amount = 20;
            Association = 51;
        }

        public TemplateGreaterCurePotion(Serial serial)
            : base(serial)
        { }

        public override void OnSingleClick(Mobile m)
        {
            LabelTo(m, "[Battles]", 54);

            base.OnSingleClick(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }
    public class TemplateDeadlyPoisonPotion : DeadlyPoisonPotion
    {
        [Constructable]
        public TemplateDeadlyPoisonPotion()
        {
            Weight = 0;
            Stackable = true;
            Amount = 20;
            Association = 51;
        }

        public TemplateDeadlyPoisonPotion(Serial serial)
            : base(serial)
        { }

        public override void OnSingleClick(Mobile m)
        {
            LabelTo(m, "[Battles]", 54);

            base.OnSingleClick(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class TemplateTotalRefreshPotion : TotalRefreshPotion
    {
        [Constructable]
        public TemplateTotalRefreshPotion()
        {
            Weight = 0;
            Stackable = true;
            Amount = 20;
            Association = 51;
        }

        public TemplateTotalRefreshPotion(Serial serial)
            : base(serial)
        { }

        public override void OnSingleClick(Mobile m)
        {
            LabelTo(m, "[Battles]", 54);

            base.OnSingleClick(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class TemplateArrows : Arrow
    {
        [Constructable]
        public TemplateArrows()
        {
            Weight = 0;
            Stackable = true;
            Amount = 50;
            Association = 51;
        }

        public TemplateArrows(Serial serial)
            : base(serial)
        { }

        public override void OnSingleClick(Mobile m)
        {
            LabelTo(m, "[Battles]", 54);

            base.OnSingleClick(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class TemplateBolts : Bolt
    {
        [Constructable]
        public TemplateBolts()
        {
            Weight = 0;
            Stackable = true;
            Amount = 50;
            Association = 51;
        }

        public TemplateBolts(Serial serial)
            : base(serial)
        { }

        public override void OnSingleClick(Mobile m)
        {
            LabelTo(m, "[Battles]", 54);

            base.OnSingleClick(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class TemplateBandages : Bandage
    {
        [Constructable]
        public TemplateBandages()
        {
            Weight = 0;
            Stackable = true;
            Amount = 50;
            Association = 51;
        }

        public TemplateBandages(Serial serial)
            : base(serial)
        { }

        public override void OnSingleClick(Mobile m)
        {
            LabelTo(m, "[Battles]", 54);

            base.OnSingleClick(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class TemplateSpellBook : FullSpellbook
    {
        [Constructable]
        public TemplateSpellBook()
        {
            Weight = 0;
            Association = 51;
        }

        public TemplateSpellBook(Serial serial)
            : base(serial)
        { }

        public override void OnSingleClick(Mobile m)
        {
            LabelTo(m, "[Battles]", 54);

            base.OnSingleClick(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class TemplateBlackPearl : BlackPearl
    {
        [Constructable]
        public TemplateBlackPearl()
        {
            Weight = 0;
            Association = 51;
        }

        public TemplateBlackPearl(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile m)
        {
            LabelTo(m, "[Battles]", 54);

            base.OnSingleClick(m);
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

    public class TemplateBloodmoss : Bloodmoss
    {
        [Constructable]
        public TemplateBloodmoss()
        {
            Weight = 0;
            Association = 51;
        }

        public TemplateBloodmoss(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile m)
        {
            LabelTo(m, "[Battles]", 54);

            base.OnSingleClick(m);
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

    public class TemplateGarlic : Garlic
    {
        [Constructable]
        public TemplateGarlic()
        {
            Weight = 0;
            Association = 51;
        }

        public TemplateGarlic(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile m)
        {
            LabelTo(m, "[Battles]", 54);

            base.OnSingleClick(m);
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

    public class TemplateGinseng : Ginseng
    {
        [Constructable]
        public TemplateGinseng()
        {
            Weight = 0;
            Association = 51;
        }

        public TemplateGinseng(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile m)
        {
            LabelTo(m, "[Battles]", 54);

            base.OnSingleClick(m);
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

    public class TemplateMandrakeRoot : MandrakeRoot
    {
        [Constructable]
        public TemplateMandrakeRoot()
        {
            Weight = 0;
            Association = 51;
        }

        public TemplateMandrakeRoot(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile m)
        {
            LabelTo(m, "[Battles]", 54);

            base.OnSingleClick(m);
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

    public class TemplateNightshade : Nightshade
    {
        [Constructable]
        public TemplateNightshade()
        {
            Weight = 0;
            Association = 51;
        }

        public TemplateNightshade(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile m)
        {
            LabelTo(m, "[Battles]", 54);

            base.OnSingleClick(m);
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

    public class TemplateSulfurousAsh : SulfurousAsh
    {
        [Constructable]
        public TemplateSulfurousAsh()
        {
            Weight = 0;
            Association = 51;
        }

        public TemplateSulfurousAsh(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile m)
        {
            LabelTo(m, "[Battles]", 54);

            base.OnSingleClick(m);
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

    public class TemplateSpidersSilk : SpidersSilk
    {
        [Constructable]
        public TemplateSpidersSilk()
        {
            Weight = 0;
            Association = 51;
        }

        public TemplateSpidersSilk(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile m)
        {
            LabelTo(m, "[Battles]", 54);

            base.OnSingleClick(m);
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

    public class TemplateBagOfReagents : Bag
    {
        [Constructable]
        public TemplateBagOfReagents()
            : this(50)
        { }

        [Constructable]
        public TemplateBagOfReagents(int amount)
        {
            Association = 51;
            DropItem(new TemplateBlackPearl() { Amount = amount });
            DropItem(new TemplateBloodmoss() { Amount = amount });
            DropItem(new TemplateGarlic() { Amount = amount });
            DropItem(new TemplateGinseng() { Amount = amount });
            DropItem(new TemplateMandrakeRoot() { Amount = amount });
            DropItem(new TemplateNightshade() { Amount = amount });
            DropItem(new TemplateSulfurousAsh() { Amount = amount });
            DropItem(new TemplateSpidersSilk() { Amount = amount });
        }

        [Constructable]
        public TemplateBagOfReagents(int amount, int weight)
        {
            DropItem(new BlackPearl(amount) { Amount = amount });
        }

        public TemplateBagOfReagents(Serial serial)
            : base(serial)
        { }

        public override void OnSingleClick(Mobile m)
        {
            LabelTo(m, "[Battles]", 54);

            base.OnSingleClick(m);
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