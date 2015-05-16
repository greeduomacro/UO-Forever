#region References

using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles.MetaPet;
using Server.Mobiles.MetaPet.Skills;
using Server.Mobiles.MetaSkills;
using Server.Network;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a dragon corpse")]
    public class MetaDragon : BaseMetaPet
    {
        public override Poison HitPoison
        {
            get
            {
                if (Stage == 1)
                {
                    return Poison.Lesser;
                }
                if (Stage == 2)
                {
                    return Poison.Regular;
                }
                if (Stage == 3)
                {
                    return Poison.Greater;
                }
                return null;
            }
        }

        public override bool HasBreath { get { return Stage > 3; } } // fire breath enabled

        public override bool AutoDispel { get { return Stage == 7; } }

        public override FoodType FavoriteFood { get { return FoodType.Meat; } }

        public override int BreathEffectHue { get { return Body != 104 ? base.BreathEffectHue : 0x480; } }

        public override int DefaultBloodHue { get { return Hue; } } //Template
        public override int BloodHueTemplate { get { return Hue; } }


        [Constructable]
        public MetaDragon()
        {
            Name = "a dragon hatchling";
            Body = 52;
            Hue = 0;
            BaseSoundID = 0xDB;
            Stage = 1;

            SpecialTitle = "Dragon Hatchling";
            TitleHue = 1174;

            SetStr(166, 195);
            SetDex(46, 65);
            SetInt(46, 70);

            SetHits(100, 117);

            SetSkill(SkillName.Poisoning, 90.1, 100.0);
            SetSkill(SkillName.MagicResist, 55.1, 70.0);
            SetSkill(SkillName.Tactics, 60.1, 70.0);
            SetSkill(SkillName.Wrestling, 70.1, 80.0);

            SetDamage(11, 17);

            Fame = 300;
            Karma = -300;

            VirtualArmor = 30;

            ControlSlots = 3;

            NextEvolution = 25000;

            MinTameSkill = 40.0;

            MaxStage = 7;

            Metaskills = new Dictionary<MetaSkillType, BaseMetaSkill>();

            MaxAbilities = 0;
            CurrentAbilities = 0;
        }

        public MetaDragon(Serial serial)
            : base(serial)
        {}

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is EvolutionEgg && Controlled && from == ControlMaster)
            {
                Say("*Eats the dragon egg*");
                Effects.SendFlashEffect(this, (FlashType)2);
                Hue = dropped.Hue;
                dropped.Consume();
            }

            return base.OnDragDrop(from, dropped);
        }

        public override void Addpoints(BaseCreature creature, int points)
        {
            if (creature is VorpalBunny)
                return;
            if (Stage < MaxStage && MasterInRange())
            {
                switch (Stage)
                {
                    case 2:
                    {
                        if (creature.HitsMax <= 75)
                        {
                            points = (int) Math.Ceiling((double) points / 4);
                        }
                    }
                        break;
                    case 3:
                    {
                        if (creature.HitsMax <= 150)
                        {
                            points = (int) Math.Ceiling((double) points / 4);
                        }
                    }
                        break;
                    case 4:
                    {
                        if (creature.HitsMax <= 250)
                        {
                            points = (int) Math.Ceiling((double) points / 4);
                        }
                    }
                        break;
                    case 5:
                    {
                        if (creature.HitsMax <= 350)
                        {
                            points = (int) Math.Ceiling((double) points / 4);
                        }
                    }
                        break;
                    case 6:
                    {
                        if (creature.HitsMax <= 450)
                        {
                            points = (int) Math.Ceiling((double) points / 4);
                        }
                    }
                        break;
                }
                if (creature.HitsMax + 150 > HitsMax)
                {
                    points += (int)Math.Ceiling(points * 0.1);
                }
                EvolutionPoints += points;
                if (CanEvolve())
                {
                    Stage++;
                    PublicOverheadMessage(MessageType.Label, 54, true, RawName + " begins to metamorphosise!");
                    Timer.DelayCall(TimeSpan.FromSeconds(30), Evolve);
                }
            }
        }

        public override void Evolve()
        {
            base.Evolve();
            switch (Stage)
            {
                case 1:
                {
                    VirtualArmor = 30;

                    Warmode = false;
                    SetDamage(11, 17);
                    ;
                    Body = 52;
                    BaseSoundID = 219;

                    SpecialTitle = "Dragon Hatchling";

                    AI = AIType.AI_Melee;

                    SetStr(166, 195);
                    SetDex(46, 65);
                    SetInt(46, 70);

                    SetHits(100, 117);

                    NextEvolution = 25000;
                    EvolutionPoints = 0;

                    SetSkill(SkillName.Poisoning, 90.1, 100.0);
                    SetSkill(SkillName.MagicResist, 95.1, 100.0);
                    SetSkill(SkillName.Tactics, 80.1, 95.0);
                    SetSkill(SkillName.Wrestling, 85.1, 100.0);

                    break;
                }
                case 2:
                {
                    VirtualArmor = 40;

                    Warmode = false;
                    SetDamage(5, 21);
                    BodyValue = 92;
                    BaseSoundID = 219;

                    AI = AIType.AI_Melee;

                    SpecialTitle = "Dragon Hatchling";

                    SetStr(161, 360);
                    SetDex(151, 300);
                    SetInt(21, 40);

                    SetHits(200);

                    NextEvolution = 75000;
                    EvolutionPoints = 0;

                    MinTameSkill = 60.0;

                    SetSkill(SkillName.Poisoning, 90.1, 100.0);
                    SetSkill(SkillName.MagicResist, 95.1, 100.0);
                    SetSkill(SkillName.Tactics, 80.1, 95.0);
                    SetSkill(SkillName.Wrestling, 85.1, 100.0);

                    MaxAbilities = 1;

                    break;
                }
                case 3:
                {
                    VirtualArmor = 45;

                    Warmode = false;
                    SetDamage(5, 21);
                    BodyValue = 206;
                    BaseSoundID = 219;

                    AI = AIType.AI_Melee;

                    SetHits(350);

                    SetStr(400);
                    SetDex(250);
                    SetInt(21, 40);

                    SpecialTitle = "Dragon Youngling";

                    SetSkill(SkillName.Poisoning, 90.1, 100.0);
                    SetSkill(SkillName.MagicResist, 110.0, 110.0);
                    SetSkill(SkillName.Tactics, 100.0, 110.0);
                    SetSkill(SkillName.Wrestling, 100.0, 110.0);

                    NextEvolution = 250000;
                    EvolutionPoints = 0;

                    MaxAbilities = 1;

                    MinTameSkill = 85.0;

                    break;
                }
                case 4:
                {
                    VirtualArmor = 55;

                    Warmode = false;
                    SetDamage(10, 22);
                    BodyValue = 794;
                    BaseSoundID = 219;

                    AI = AIType.AI_Mage;

                    SetHits(420);

                    SetStr(500);
                    SetDex(110);
                    SetInt(400);

                    AI = AIType.AI_Mage;
                    SpecialTitle = "Dragon Whelpling";

                    SetSkill(SkillName.Poisoning, 90.1, 100.0);
                    SetSkill(SkillName.EvalInt, 50.1, 60.0);
                    SetSkill(SkillName.Magery, 60.1, 70.0);
                    SetSkill(SkillName.MagicResist, 110.0, 110.0);
                    SetSkill(SkillName.Tactics, 100.0, 110.0);
                    SetSkill(SkillName.Wrestling, 100.0, 110.0);

                    NextEvolution = 750000;
                    EvolutionPoints = 0;

                    MinTameSkill = 90.0;

                    MaxAbilities = 2;

                    break;
                }
                case 5:
                {
                    SetHits(500);

                    SetStr(600);
                    SetDex(86);
                    SetInt(400);

                    VirtualArmor = 65;

                    SetDamage(17, 22);

                    AI = AIType.AI_Mage;

                    Warmode = false;
                    BodyValue = 61;
                    BaseSoundID = 219;

                    SpecialTitle = "Young Dragon";

                    SetSkill(SkillName.Poisoning, 110.0, 115.0);
                    SetSkill(SkillName.EvalInt, 100.0, 105.0);
                    SetSkill(SkillName.Magery, 100.0, 105.0);
                    SetSkill(SkillName.MagicResist, 110.0, 110.0);
                    SetSkill(SkillName.Tactics, 100.0, 110.0);
                    SetSkill(SkillName.Wrestling, 100.0, 110.0);

                    NextEvolution = 1500000;
                    EvolutionPoints = 0;

                    MinTameSkill = 100.0;

                    MaxAbilities = 2;

                    break;
                }
                case 6:
                {
                    SetHits(525);

                    SetStr(600, 650);
                    SetDex(86, 105);
                    SetInt(436, 475);

                    SetDamage(17, 22);

                    VirtualArmor = 75;

                    Warmode = false;
                    BodyValue = 59;
                    BaseSoundID = 219;

                    SpecialTitle = "Mature Dragon";

                    AI = AIType.AI_Mage;

                    SetSkill(SkillName.EvalInt, 115.0);
                    SetSkill(SkillName.Meditation, 115.0);
                    SetSkill(SkillName.Magery, 120.0);
                    SetSkill(SkillName.Poisoning, 120.0);
                    SetSkill(SkillName.Tactics, 115.0);
                    SetSkill(SkillName.Wrestling, 115.0);
                    SetSkill(SkillName.Anatomy, 115.0);
                    SetSkill(SkillName.MagicResist, 110.0, 110.0);

                    NextEvolution = 3000000;
                    EvolutionPoints = 0;

                    MinTameSkill = 110.0;

                    MaxAbilities = 3;

                    break;
                }
                case 7:
                {
                    SetHits(550);

                    SetStr(796, 825);
                    SetDex(86, 105);
                    SetInt(550);

                    SetDamage(22, 30);

                    VirtualArmor = 80;

                    AI = AIType.AI_Mage;

                    Warmode = false;
                    BodyValue = Utility.RandomDouble() <= 0.1 ? 39 : 46;
                    BaseSoundID = 219;

                    SetSkill(SkillName.EvalInt, 125.0);
                    SetSkill(SkillName.Meditation, 125.0);
                    SetSkill(SkillName.Magery, 140.0);
                    SetSkill(SkillName.Poisoning, 140.0);
                    SetSkill(SkillName.Tactics, 125.0);
                    SetSkill(SkillName.Wrestling, 125.0);
                    SetSkill(SkillName.Anatomy, 125.0);
                    SetSkill(SkillName.MagicResist, 110.0, 110.0);

                    SpecialTitle = "Elder Dragon";

                    NextEvolution = 0;
                    EvolutionPoints = 0;

                    MinTameSkill = 120.0;

                    MaxAbilities = 4;

                    break;
                }
            }
        }

        public override void OnDeath(Container c)
        {
            if (LastKiller != null)
            {
                if (EvolutionPoints - LastKiller.Hits > 0)
                {
                    EvolutionPoints -= LastKiller.Hits;
                }
                else
                {
                    EvolutionPoints = 0;
                }
            }
            base.OnDeath(c);
        }

        public override bool CanEvolve()
        {
            if (Stage < 7 && EvolutionPoints >= NextEvolution && Controlled)
            {
                switch (Stage)
                {
                    case 1:
                    {
                        return true;
                    }
                    case 2:
                    {
                        if (ControlMaster.Skills.AnimalTaming.Base >= 60)
                        {
                            return true;
                        }
                        break;
                    }
                    case 3:
                    {
                        if (ControlMaster.Skills.AnimalTaming.Base >= 85)
                        {
                            return true;
                        }
                        break;
                    }
                    case 4:
                    {
                        if (ControlMaster.Skills.AnimalTaming.Base >= 90)
                        {
                            return true;
                        }
                        break;
                    }
                    case 5:
                    {
                        if (ControlMaster.Skills.AnimalTaming.Base >= 100)
                        {
                            return true;
                        }
                        break;
                    }
                    case 6:
                    {
                        if (ControlMaster.Skills.AnimalTaming.Base >= 110)
                        {
                            return true;
                        }
                        break;
                    }
                    case 7:
                    {
                        if (ControlMaster.Skills.AnimalTaming.Base >= 120)
                        {
                            return true;
                        }
                        break;
                    }
                }
            }
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                {}
                    break;
            }
        }
    }
}