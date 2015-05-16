using System;
using Server.Items;
using Server.Mobiles;
using Server.Spells;

namespace Server.Mobiles
{
    public class FamousPiratesMace : FamousPirates
    {
        [Constructable]
        public FamousPiratesMace()
            : base(AIType.AI_Melee, FightMode.Closest, 15, 1, 0.9, 0.9)
        {

            int Hue = 2075;

            // Skills and Stats
            this.InitStats(350, 350, 310);
            this.Skills[SkillName.Macing].Base = 150;
            this.Skills[SkillName.Anatomy].Base = 150;
            this.Skills[SkillName.Healing].Base = 150;
            this.Skills[SkillName.Tactics].Base = 150;

            // Name
            this.Name = "Captain Hector Barbossa";

            // Equip
            WarHammer war = new WarHammer();
            war.Movable = true;
            war.Crafter = this;
            war.Quality = WeaponQuality.Exceptional;
            AddItem(war);

            Boots bts = new Boots();
			bts.Movable = false;
            bts.Hue = Hue;
            AddItem(bts);

            ChainChest cht = new ChainChest();
            cht.Movable = false;
            cht.LootType = LootType.Regular;
            cht.Crafter = this;
            cht.Quality = ArmorQuality.Exceptional;
            AddItem(cht);

            ChainLegs chl = new ChainLegs();
            chl.Movable = false;
            chl.LootType = LootType.Regular;
            chl.Crafter = this;
            chl.Quality = ArmorQuality.Exceptional;
            AddItem(chl);

            PlateArms pla = new PlateArms();
            pla.Movable = false;
            pla.LootType = LootType.Regular;
            pla.Crafter = this;
            pla.Quality = ArmorQuality.Exceptional;
            AddItem(pla);

            TricorneHat tch = new TricorneHat();
            tch.Movable = false;
            tch.Hue = Hue;
            AddItem(tch);

            Bandage band = new Bandage(20);
            AddToBackpack(band);
        }
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 3);
            AddLoot(LootPack.Gems, 5);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            double rand = Utility.RandomDouble();

            if (0.0020 > rand)
                c.AddItem(new KrakenStatue());
            else if (0.0030 > rand)
                c.AddItem(new Fountainofyouth());
            else if (0.0040 > rand)
                c.AddItem(new BlackPearlShip());
            else if (0.0055 > rand)
                c.AddItem(new Netofthedead());
            else if (0.0060 > rand)
                c.AddItem(new PirateCap());
            else if (0.0065 > rand)
                c.AddItem(new BlackPearlOars());
            else if (0.0070 > rand)
                c.AddItem(new GhostShipAnchor());
            else if (0.0080 > rand)
                c.AddItem(new DavyJonesOar());
            else if (0.0085 > rand)
                c.AddItem(new PirateCoffin());
            else if (0.0090 > rand)
                c.AddItem(new Pulley());
            else if (0.0095 > rand)
                c.AddItem(new Hook());
            else if (0.01 > rand)
                c.AddItem(new SlipKnot());
            else if (0.02 > rand)
                c.AddItem(new MessageInABottle());
        }

        public FamousPiratesMace(Serial serial)
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

    public class FamousPiratesFence : FamousPirates
    {
        [Constructable]
        public FamousPiratesFence()
            : base(AIType.AI_Melee, FightMode.Closest, 15, 1, 0.2, 0.6)
        {
            // A FamousPirates Fencer
            int Hue = 2075;

            // Skills and Stats
            this.InitStats(450, 250, 410);
            this.Skills[SkillName.Fencing].Base = 150;
            this.Skills[SkillName.Anatomy].Base = 150;
            this.Skills[SkillName.Stealing].Base = 150;
            this.Skills[SkillName.Tactics].Base = 150;

            // Name
            this.Name = "Joshamee Gibbs";

            // Equip
            Spear ssp = new Spear();
            ssp.Movable = true;
            ssp.Crafter = this;
            ssp.Quality = WeaponQuality.Exceptional;
            AddItem(ssp);

            Sandals snd = new Sandals();
			snd.Movable = false;
            snd.Hue = Hue;
            snd.LootType = LootType.Regular;
            AddItem(snd);

            ChainChest cht = new ChainChest();
            cht.Movable = false;
            cht.LootType = LootType.Regular;
            cht.Crafter = this;
            cht.Quality = ArmorQuality.Exceptional;
            AddItem(cht);

            ChainLegs chl = new ChainLegs();
            chl.Movable = false;
            chl.LootType = LootType.Regular;
            chl.Crafter = this;
            chl.Quality = ArmorQuality.Exceptional;
            AddItem(chl);

            PlateArms pla = new PlateArms();
            pla.Movable = false;
            pla.LootType = LootType.Regular;
            pla.Crafter = this;
            pla.Quality = ArmorQuality.Exceptional;
            AddItem(pla);


            TricorneHat tch = new TricorneHat();
            tch.Movable = false;
            tch.Hue = Hue;
            AddItem(tch);

            Bandage band = new Bandage(20);
            AddToBackpack(band);
        }
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 3);
            AddLoot(LootPack.Gems, 5);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            double rand = Utility.RandomDouble();

            if (0.0020 > rand)
                c.AddItem(new KrakenStatue());
            else if (0.0030 > rand)
                c.AddItem(new Fountainofyouth());
            else if (0.0040 > rand)
                c.AddItem(new BlackPearlShip());
            else if (0.0055 > rand)
                c.AddItem(new Netofthedead());
            else if (0.0060 > rand)
                c.AddItem(new PirateCap());
            else if (0.0065 > rand)
                c.AddItem(new BlackPearlOars());
            else if (0.0070 > rand)
                c.AddItem(new GhostShipAnchor());
            else if (0.0080 > rand)
                c.AddItem(new DavyJonesOar());
            else if (0.0085 > rand)
                c.AddItem(new PirateCoffin());
            else if (0.0090 > rand)
                c.AddItem(new Pulley());
            else if (0.0095 > rand)
                c.AddItem(new Hook());
            else if (0.01 > rand)
                c.AddItem(new SlipKnot());
            else if (0.02 > rand)
                c.AddItem(new MessageInABottle());
        }
        public FamousPiratesFence(Serial serial)
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

    public class FamousPiratesSword : FamousPirates
    {
        [Constructable]
        public FamousPiratesSword()
            : base(AIType.AI_Melee, FightMode.Closest, 15, 1, 0.2, 0.6)
        {
            // A FamousPirates Swordsman
            int Hue = 1107;

            // Skills and Stats
            this.InitStats(350, 350, 310);
            this.Skills[SkillName.Swords].Base = 150;
            this.Skills[SkillName.Anatomy].Base = 150;
            this.Skills[SkillName.Healing].Base = 150;
            this.Skills[SkillName.Tactics].Base = 150;
            this.Skills[SkillName.Parry].Base = 150;

            // Name
            this.Name = "Elizabeth Swann";

            // Equip
            Katana kat = new Katana();
            kat.Crafter = this;
            kat.Movable = true;
            kat.Quality = WeaponQuality.Exceptional;
            AddItem(kat);

            Boots bts = new Boots();
			bts.Movable = false;
            bts.Hue = Hue;
            AddItem(bts);

            ChainChest cht = new ChainChest();
            cht.Movable = false;
            cht.LootType = LootType.Regular;
            cht.Crafter = this;
            cht.Quality = ArmorQuality.Exceptional;
            AddItem(cht);

            ChainLegs chl = new ChainLegs();
            chl.Movable = false;
            chl.LootType = LootType.Regular;
            chl.Crafter = this;
            chl.Quality = ArmorQuality.Exceptional;
            AddItem(chl);

            PlateArms pla = new PlateArms();
            pla.Movable = false;
            pla.LootType = LootType.Regular;
            pla.Crafter = this;
            pla.Quality = ArmorQuality.Exceptional;
            AddItem(pla);


            TricorneHat tch = new TricorneHat();
            tch.Movable = false;
            tch.Hue = Hue;
            AddItem(tch);

            FancyShirt fst = new FancyShirt();
            fst.Movable = false;
            fst.Hue = Hue;
            AddItem(fst);

            Skirt srt = new Skirt();
            srt.Movable = false;
            srt.Hue = Hue;
            AddItem(srt);

            Bandage band = new Bandage(50);
            AddToBackpack(band);
        }
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 3);
            AddLoot(LootPack.Gems, 5);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            double rand = Utility.RandomDouble();

            if (0.0020 > rand)
                c.AddItem(new KrakenStatue());
            else if (0.0030 > rand)
                c.AddItem(new Fountainofyouth());
            else if (0.0040 > rand)
                c.AddItem(new BlackPearlShip());
            else if (0.0055 > rand)
                c.AddItem(new Netofthedead());
            else if (0.0060 > rand)
                c.AddItem(new PirateCap());
            else if (0.0065 > rand)
                c.AddItem(new BlackPearlOars());
            else if (0.0070 > rand)
                c.AddItem(new GhostShipAnchor());
            else if (0.0080 > rand)
                c.AddItem(new DavyJonesOar());
            else if (0.0085 > rand)
                c.AddItem(new PirateCoffin());
            else if (0.0090 > rand)
                c.AddItem(new Pulley());
            else if (0.0095 > rand)
                c.AddItem(new Hook());
            else if (0.01 > rand)
                c.AddItem(new SlipKnot());
            else if (0.02 > rand)
                c.AddItem(new MessageInABottle());
        }
        public FamousPiratesSword(Serial serial)
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

    public class FamousPiratesNox : FamousPirates
    {
        [Constructable]
        public FamousPiratesNox()
            : base(AIType.AI_Mage, FightMode.Closest, 15, 1, 0.2, 0.6)
        {
            // A FamousPirates Nox or Pure Mage
            int Hue = 2075;

            // Skills and Stats
            this.InitStats(210, 210, 250);
            this.Skills[SkillName.Magery].Base = 150;
            this.Skills[SkillName.EvalInt].Base = 150;
            this.Skills[SkillName.Inscribe].Base = 100;
            this.Skills[SkillName.Wrestling].Base = 150;
            this.Skills[SkillName.Meditation].Base = 150;
            this.Skills[SkillName.Poisoning].Base = 100;


            // Name
            this.Name = "Davy Jones";

            // Equip
            Spellbook book = FullSpellbook();
            AddItem(book);

            Kilt kilt = new Kilt();
			kilt.Movable = false;
            kilt.Hue = Hue;
            AddItem(kilt);

            Boots snd = new Boots();
			snd.Movable = false;
            snd.Hue = Hue;
            snd.LootType = LootType.Regular;
            AddItem(snd);

            SkullCap skc = new SkullCap();
			skc.Movable = false;
            skc.Hue = Hue;
            AddItem(skc);

            // Spells
            AddSpellAttack(typeof(Spells.First.MagicArrowSpell));
            AddSpellAttack(typeof(Spells.First.WeakenSpell));
            AddSpellAttack(typeof(Spells.Sixth.ExplosionSpell));
            AddSpellDefense(typeof(Spells.Third.WallOfStoneSpell));
            AddSpellDefense(typeof(Spells.Fourth.GreaterHealSpell));
        }
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 3);
            AddLoot(LootPack.Gems, 5);
            AddItem(new Gunpowder());
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            double rand = Utility.RandomDouble();

            if (0.0020 > rand)
                c.AddItem(new KrakenStatue());
            else if (0.0030 > rand)
                c.AddItem(new Fountainofyouth());
            else if (0.0040 > rand)
                c.AddItem(new BlackPearlShip());
            else if (0.0055 > rand)
                c.AddItem(new Netofthedead());
            else if (0.0060 > rand)
                c.AddItem(new PirateCap());
            else if (0.0065 > rand)
                c.AddItem(new BlackPearlOars());
            else if (0.0070 > rand)
                c.AddItem(new GhostShipAnchor());
            else if (0.0080 > rand)
                c.AddItem(new DavyJonesOar());
            else if (0.0085 > rand)
                c.AddItem(new PirateCoffin());
            else if (0.0090 > rand)
                c.AddItem(new Pulley());
            else if (0.0095 > rand)
                c.AddItem(new Hook());
            else if (0.01 > rand)
                c.AddItem(new SlipKnot());
            else if (0.02 > rand)
                c.AddItem(new MessageInABottle());
        }
        public FamousPiratesNox(Serial serial)
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

    public class FamousPiratesStun : FamousPirates
    {

        [Constructable]
        public FamousPiratesStun()
            : base(AIType.AI_Mage, FightMode.Closest, 15, 1, 0.2, 0.6)
        {

            // A FamousPirates Stun Mage
            int Hue = 2075;

            // Skills and Stats
            this.InitStats(210, 210, 250);
            this.Skills[SkillName.Magery].Base = 100;
            this.Skills[SkillName.EvalInt].Base = 150;
            this.Skills[SkillName.Anatomy].Base = 80;
            this.Skills[SkillName.Wrestling].Base = 80;
            this.Skills[SkillName.Meditation].Base = 100;
            this.Skills[SkillName.Poisoning].Base = 100;


            // Name
            this.Name = "William Turner";

            // Equip
            Spellbook book = FullSpellbook();
            AddItem(book);

            LeatherArms lea = new LeatherArms();
            lea.Movable = false;
            lea.LootType = LootType.Regular;
            lea.Crafter = this;
            lea.Quality = ArmorQuality.Exceptional;
            AddItem(lea);

            LeatherChest lec = new LeatherChest();
            lec.Movable = false;
            lec.LootType = LootType.Regular;
            lec.Crafter = this;
            lec.Quality = ArmorQuality.Exceptional;
            AddItem(lec);

            LeatherGorget leg = new LeatherGorget();
            leg.Movable = false;
            leg.LootType = LootType.Regular;
            leg.Crafter = this;
            leg.Quality = ArmorQuality.Exceptional;
            AddItem(leg);

            LeatherLegs lel = new LeatherLegs();
            lel.Movable = false;
            lel.LootType = LootType.Regular;
            lel.Crafter = this;
            lel.Quality = ArmorQuality.Exceptional;
            AddItem(lel);

            Boots bts = new Boots();
			bts.Movable = false;
            bts.Hue = Hue;
            AddItem(bts);

            TricorneHat tch = new TricorneHat();
            tch.Movable = false;
            tch.Hue = Hue;
            AddItem(tch);

            // Spells
            AddSpellAttack(typeof(Spells.First.MagicArrowSpell));
            AddSpellAttack(typeof(Spells.First.WeakenSpell));
            AddSpellAttack(typeof(Spells.Sixth.ExplosionSpell));
            AddSpellDefense(typeof(Spells.Third.WallOfStoneSpell));
            AddSpellDefense(typeof(Spells.Fourth.GreaterHealSpell));
        }
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 3);
            AddLoot(LootPack.Gems, 5);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            double rand = Utility.RandomDouble();

            if (0.0020 > rand)
                c.AddItem(new KrakenStatue());
            else if (0.0030 > rand)
                c.AddItem(new Fountainofyouth());
            else if (0.0040 > rand)
                c.AddItem(new BlackPearlShip());
            else if (0.0055 > rand)
                c.AddItem(new Netofthedead());
            else if (0.0060 > rand)
                c.AddItem(new PirateCap());
            else if (0.0065 > rand)
                c.AddItem(new BlackPearlOars());
            else if (0.0070 > rand)
                c.AddItem(new GhostShipAnchor());
            else if (0.0080 > rand)
                c.AddItem(new DavyJonesOar());
            else if (0.0085 > rand)
                c.AddItem(new PirateCoffin());
            else if (0.0090 > rand)
                c.AddItem(new Pulley());
            else if (0.0095 > rand)
                c.AddItem(new Hook());
            else if (0.01 > rand)
                c.AddItem(new SlipKnot());
            else if (0.02 > rand)
                c.AddItem(new MessageInABottle());
        }
        public FamousPiratesStun(Serial serial)
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

    public class FamousPiratesSuper : FamousPirates
    {
        [Constructable]
        public FamousPiratesSuper()
            : base(AIType.AI_Mage, FightMode.Closest, 15, 1, 0.2, 0.6)
        {
            // A FamousPirates Super Mage
            int Hue = 2075;

            // Skills and Stats
            this.InitStats(250, 250, 250);
            this.Skills[SkillName.Magery].Base = 150;
            this.Skills[SkillName.EvalInt].Base = 150;
            this.Skills[SkillName.Anatomy].Base = 150;
            this.Skills[SkillName.Wrestling].Base = 150;
            this.Skills[SkillName.Meditation].Base = 150;
            this.Skills[SkillName.Poisoning].Base = 100;
            this.Skills[SkillName.Inscribe].Base = 100;

            // Name
            this.Name = "Captain Jack Sparrow";

            // Equip
            Spellbook book = FullSpellbook();
            AddItem(book);

            LeatherArms lea = new LeatherArms();
            lea.Movable = false;
            lea.LootType = LootType.Regular;
            lea.Crafter = this;
            lea.Quality = ArmorQuality.Exceptional;
            AddItem(lea);

            LeatherChest lec = new LeatherChest();
            lec.Movable = false;
            lec.LootType = LootType.Regular;
            lec.Crafter = this;
            lec.Quality = ArmorQuality.Exceptional;
            AddItem(lec);

            LeatherGorget leg = new LeatherGorget();
            leg.Movable = false;
            leg.LootType = LootType.Regular;
            leg.Crafter = this;
            leg.Quality = ArmorQuality.Exceptional;
            AddItem(leg);

            LeatherLegs lel = new LeatherLegs();
            lel.Movable = false;
            lel.LootType = LootType.Regular;
            lel.Crafter = this;
            lel.Quality = ArmorQuality.Exceptional;
            AddItem(lel);

            Boots snd = new Boots();
			snd.Movable = false;
            snd.Hue = Hue;
            snd.LootType = LootType.Regular;
            AddItem(snd);

            JesterHat jhat = new JesterHat();
			jhat.Movable = false;
            jhat.Hue = Hue;
            AddItem(jhat);

            Doublet dblt = new Doublet();
			dblt.Movable = false;
            dblt.Hue = Hue;
            AddItem(dblt);

            // Spells
            AddSpellAttack(typeof(Spells.First.MagicArrowSpell));
            AddSpellAttack(typeof(Spells.First.WeakenSpell));
            AddSpellAttack(typeof(Spells.Sixth.ExplosionSpell));
            AddSpellDefense(typeof(Spells.Third.WallOfStoneSpell));
            AddSpellDefense(typeof(Spells.Fourth.GreaterHealSpell));
        }
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 3);
            AddLoot(LootPack.Gems, 5);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            double rand = Utility.RandomDouble();

            if (0.0020 > rand)
                c.AddItem(new KrakenStatue());
            else if (0.0030 > rand)
                c.AddItem(new Fountainofyouth());
            else if (0.0040 > rand)
                c.AddItem(new BlackPearlShip());
            else if (0.0055 > rand)
                c.AddItem(new Netofthedead());
            else if (0.0060 > rand)
                c.AddItem(new PirateCap());
            else if (0.0065 > rand)
                c.AddItem(new BlackPearlOars());
            else if (0.0070 > rand)
                c.AddItem(new GhostShipAnchor());
            else if (0.0080 > rand)
                c.AddItem(new DavyJonesOar());
            else if (0.0085 > rand)
                c.AddItem(new PirateCoffin());
            else if (0.0090 > rand)
                c.AddItem(new Pulley());
            else if (0.0095 > rand)
                c.AddItem(new Hook());
            else if (0.01 > rand)
                c.AddItem(new SlipKnot());
            else if (0.02 > rand)
                c.AddItem(new MessageInABottle());
        }
       
        public FamousPiratesSuper(Serial serial)
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

    public class FamousPiratesHealer : FamousPirates
    {
        [Constructable]
        public FamousPiratesHealer()
            : base(AIType.AI_Healer, FightMode.Closest, 15, 1, 0.2, 0.6)
        {
            // A FamousPirates Healer Mage
            int Hue = 2075;

            // Skills and Stats
            this.InitStats(250, 250, 250);
            this.Skills[SkillName.Magery].Base = 150;
            this.Skills[SkillName.EvalInt].Base = 150;
            this.Skills[SkillName.Anatomy].Base = 150;
            this.Skills[SkillName.Wrestling].Base = 150;
            this.Skills[SkillName.Meditation].Base = 150;
            this.Skills[SkillName.Healing].Base = 100;

            // Name
            this.Name = "Governor Swann";

            // Equip
            Spellbook book = FullSpellbook();
            AddItem(book);

            LeatherArms lea = new LeatherArms();
            lea.Movable = false;
            lea.LootType = LootType.Regular;
            lea.Crafter = this;
            lea.Quality = ArmorQuality.Exceptional;
            AddItem(lea);

            LeatherChest lec = new LeatherChest();
            lec.Movable = false;
            lec.LootType = LootType.Regular;
            lec.Crafter = this;
            lec.Quality = ArmorQuality.Exceptional;
            AddItem(lec);

            LeatherGorget leg = new LeatherGorget();
            leg.Movable = false;
            leg.LootType = LootType.Regular;
            leg.Crafter = this;
            leg.Quality = ArmorQuality.Exceptional;
            AddItem(leg);

            LeatherLegs lel = new LeatherLegs();
            lel.Movable = false;
            lel.LootType = LootType.Regular;
            lel.Crafter = this;
            lel.Quality = ArmorQuality.Exceptional;
            AddItem(lel);

            Boots snd = new Boots();
			snd.Movable = false;
            snd.Hue = Hue;
            snd.LootType = LootType.Regular;
            AddItem(snd);

            TricorneHat tch = new TricorneHat();
            tch.Movable = false;
            tch.Hue = Hue;
            AddItem(tch);

            Robe robe = new Robe();
			robe.Movable = false;
            robe.Hue = Hue;
            AddItem(robe);

        }
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 3);
            AddLoot(LootPack.Gems, 5);
            AddItem(new Gunpowder());
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            double rand = Utility.RandomDouble();

            if (0.0020 > rand)
                c.AddItem(new KrakenStatue());
            else if (0.0030 > rand)
                c.AddItem(new Fountainofyouth());
            else if (0.0040 > rand)
                c.AddItem(new BlackPearlShip());
            else if (0.0055 > rand)
                c.AddItem(new Netofthedead());
            else if (0.0060 > rand)
                c.AddItem(new PirateCap());
            else if (0.0065 > rand)
                c.AddItem(new BlackPearlOars());
            else if (0.0070 > rand)
                c.AddItem(new GhostShipAnchor());
            else if (0.0080 > rand)
                c.AddItem(new DavyJonesOar());
            else if (0.0085 > rand)
                c.AddItem(new PirateCoffin());
            else if (0.0090 > rand)
                c.AddItem(new Pulley());
            else if (0.0095 > rand)
                c.AddItem(new Hook());
            else if (0.01 > rand)
                c.AddItem(new SlipKnot());
            else if (0.02 > rand)
                c.AddItem(new MessageInABottle());
        }
       

        public FamousPiratesHealer(Serial serial)
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

    public class FamousPiratesAssassin : FamousPirates
    {
        [Constructable]
        public FamousPiratesAssassin()
            : base(AIType.AI_Melee, FightMode.Closest, 15, 1, 0.2, 0.6)
        {
            // A FamousPirates Hybrid Assassin
            int Hue = 2075;

            // Skills and Stats
            this.InitStats(305, 305, 305);
            this.Skills[SkillName.Magery].Base = 150;
            this.Skills[SkillName.EvalInt].Base = 150;
            this.Skills[SkillName.Swords].Base = 150;
            this.Skills[SkillName.Tactics].Base = 150;
            this.Skills[SkillName.Meditation].Base = 150;
            this.Skills[SkillName.Poisoning].Base = 100;

            // Name
            this.Name = "BootStrap Bill Turner";

            // Equip
            Spellbook book = FullSpellbook();
            AddToBackpack(book);

            Katana kat = new Katana();
            kat.Movable = false;
            kat.LootType = LootType.Regular;
            kat.Crafter = this;
            kat.Poison = Poison.Deadly;
            kat.PoisonCharges = 30;
            kat.Quality = WeaponQuality.Exceptional;
            AddToBackpack(kat);

            LeatherArms lea = new LeatherArms();
            lea.Movable = false;
            lea.LootType = LootType.Regular;
            lea.Crafter = this;
            lea.Quality = ArmorQuality.Exceptional;
            AddItem(lea);

            LeatherChest lec = new LeatherChest();
            lec.Movable = false;
            lec.LootType = LootType.Regular;
            lec.Crafter = this;
            lec.Quality = ArmorQuality.Exceptional;
            AddItem(lec);

            LeatherGorget leg = new LeatherGorget();
            leg.Movable = false;
            leg.LootType = LootType.Regular;
            leg.Crafter = this;
            leg.Quality = ArmorQuality.Exceptional;
            AddItem(leg);

            LeatherLegs lel = new LeatherLegs();
            lel.Movable = false;
            lel.LootType = LootType.Regular;
            lel.Crafter = this;
            lel.Quality = ArmorQuality.Exceptional;
            AddItem(lel);

            Boots snd = new Boots();
			snd.Movable = false;
            snd.Hue = Hue;
            snd.LootType = LootType.Regular;
            AddItem(snd);

            TricorneHat tch = new TricorneHat();
            tch.Movable = false;
            tch.Hue = Hue;
            AddItem(tch);

            DeadlyPoisonPotion pota = new DeadlyPoisonPotion();
            pota.LootType = LootType.Regular;
            AddToBackpack(pota);

            DeadlyPoisonPotion potb = new DeadlyPoisonPotion();
            potb.LootType = LootType.Regular;
            AddToBackpack(potb);

            DeadlyPoisonPotion potc = new DeadlyPoisonPotion();
            potc.LootType = LootType.Regular;
            AddToBackpack(potc);

            DeadlyPoisonPotion potd = new DeadlyPoisonPotion();
            potd.LootType = LootType.Regular;
            AddToBackpack(potd);

            Bandage band = new Bandage(50);
            AddToBackpack(band);

        }
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 3);
            AddLoot(LootPack.Gems, 5);
            AddItem(new Gunpowder());
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            double rand = Utility.RandomDouble();

            if (0.0020 > rand)
                c.AddItem(new KrakenStatue());
            else if (0.0030 > rand)
                c.AddItem(new Fountainofyouth());
            else if (0.0040 > rand)
                c.AddItem(new BlackPearlShip());
            else if (0.0055 > rand)
                c.AddItem(new Netofthedead());
            else if (0.0060 > rand)
                c.AddItem(new PirateCap());
            else if (0.0065 > rand)
                c.AddItem(new BlackPearlOars());
            else if (0.0070 > rand)
                c.AddItem(new GhostShipAnchor());
            else if (0.0080 > rand)
                c.AddItem(new DavyJonesOar());
            else if (0.0085 > rand)
                c.AddItem(new PirateCoffin());
            else if (0.0090 > rand)
                c.AddItem(new Pulley());
            else if (0.0095 > rand)
                c.AddItem(new Hook());
            else if (0.01 > rand)
                c.AddItem(new SlipKnot());
            else if (0.02 > rand)
                c.AddItem(new MessageInABottle());
        }
        public FamousPiratesAssassin(Serial serial)
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

    [TypeAlias("Server.Mobiles.FamousPiratesTheif")]
    public class FamousPiratesThief : FamousPirates
    {
        [Constructable]
        public FamousPiratesThief()
            : base(AIType.AI_Thief, FightMode.Closest, 15, 1, 0.2, 0.6)
        {
            // A FamousPirates Hybrid Thief
            int Hue = 2075;

            // Skills and Stats
            this.InitStats(205, 205, 205);
            this.Skills[SkillName.Healing].Base = 150;
            this.Skills[SkillName.Anatomy].Base = 150;
            this.Skills[SkillName.Stealing].Base = 150;
            this.Skills[SkillName.ArmsLore].Base = 100;
            this.Skills[SkillName.Meditation].Base = 150;
            this.Skills[SkillName.Wrestling].Base = 150;

            // Name
            this.Name = "Cutler Beckettt";

            // Equip
            Spellbook book = FullSpellbook();
            AddItem(book);

            LeatherArms lea = new LeatherArms();
            lea.Movable = false;
            lea.LootType = LootType.Regular;
            lea.Crafter = this;
            lea.Quality = ArmorQuality.Exceptional;
            AddItem(lea);

            LeatherChest lec = new LeatherChest();
            lec.Movable = false;
            lec.LootType = LootType.Regular;
            lec.Crafter = this;
            lec.Quality = ArmorQuality.Exceptional;
            AddItem(lec);

            LeatherGorget leg = new LeatherGorget();
            leg.Movable = false;
            leg.LootType = LootType.Regular;
            leg.Crafter = this;
            leg.Quality = ArmorQuality.Exceptional;
            AddItem(leg);

            LeatherLegs lel = new LeatherLegs();
            lel.Movable = false;
            lel.LootType = LootType.Regular;
            lel.Crafter = this;
            lel.Quality = ArmorQuality.Exceptional;
            AddItem(lel);

            Boots snd = new Boots();
			snd.Movable = false;
            snd.Hue = Hue;
            snd.LootType = LootType.Regular;
            AddItem(snd);

            TricorneHat tch = new TricorneHat();
            tch.Movable = false;
            tch.Hue = Hue;
            AddItem(tch);

             Kilt Klt = new Kilt();
			 Klt.Movable = false;
             Klt.Hue = Hue;
            AddItem(Klt);

            Bandage band = new Bandage(50);
            AddToBackpack(band);

        }
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 3);
            AddLoot(LootPack.Gems, 5);
            AddItem(new Gunpowder());
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            double rand = Utility.RandomDouble();

            if (0.0020 > rand)
                c.AddItem(new KrakenStatue());
            else if (0.0030 > rand)
                c.AddItem(new Fountainofyouth());
            else if (0.0040 > rand)
                c.AddItem(new BlackPearlShip());
            else if (0.0055 > rand)
                c.AddItem(new Netofthedead());
            else if (0.0060 > rand)
                c.AddItem(new PirateCap());
            else if (0.0065 > rand)
                c.AddItem(new BlackPearlOars());
            else if (0.0070 > rand)
                c.AddItem(new GhostShipAnchor());
            else if (0.0080 > rand)
                c.AddItem(new DavyJonesOar());
            else if (0.0085 > rand)
                c.AddItem(new PirateCoffin());
            else if (0.0090 > rand)
                c.AddItem(new Pulley());
            else if (0.0095 > rand)
                c.AddItem(new Hook());
            else if (0.01 > rand)
                c.AddItem(new SlipKnot());
            else if (0.02 > rand)
                c.AddItem(new MessageInABottle());
        }
        public FamousPiratesThief(Serial serial)
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
