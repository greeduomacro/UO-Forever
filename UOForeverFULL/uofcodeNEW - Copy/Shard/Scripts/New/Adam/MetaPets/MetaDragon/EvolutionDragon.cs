#region References

using System;
using Server.Items;
using Server.Network;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a dragon corpse")]
    public class EvolutionDragon : BaseCreature
    {
        private int _EvolutionPoints;

        private int _Stage;

        [CommandProperty(AccessLevel.GameMaster)]
        public int EvolutionPoints
        {
            get { return _EvolutionPoints; }
            set
            {
                _EvolutionPoints = value;
                if (CanEvolve())
                {
                    _Stage++;
                    Evolve();
                }
            }
        }

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
                    return Poison.Greater;
                }
                if (Stage == 3)
                {
                    return Poison.Deadly;
                }
                return null;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int NextEvolution { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Stage
        {
            get { return _Stage; }
            set
            {
                if (value > 0 && value <= 7 && value != _Stage)
                {
                    _Stage = value;
                    Evolve();
                }
            }
        }


        public override bool HasBreath { get { return Stage > 3; } } // fire breath enabled

        public override bool AutoDispel { get { return Stage == 7; } }

        public override FoodType FavoriteFood { get { return FoodType.Meat; } }

        public override int BreathEffectHue { get { return Body != 104 ? base.BreathEffectHue : 0x480; } }

        public override int DefaultBloodHue { get { return Hue; } } //Template
        public override int BloodHueTemplate { get { return Hue; } }


        [Constructable]
        public EvolutionDragon() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a dragon hatchling";
            Body = 52;
            Hue = 0;
            BaseSoundID = 0xDB;
            _Stage = 1;

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
        }

        public EvolutionDragon(Serial serial) : base(serial)
        {}

        public void Evolve()
        {
            Effects.SendFlashEffect(this, (FlashType) 2);
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
                    _EvolutionPoints = 0;

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
                    _EvolutionPoints = 0;

                    MinTameSkill = 60.0;

                    SetSkill(SkillName.Poisoning, 90.1, 100.0);
                    SetSkill(SkillName.MagicResist, 95.1, 100.0);
                    SetSkill(SkillName.Tactics, 80.1, 95.0);
                    SetSkill(SkillName.Wrestling, 85.1, 100.0);

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
                    _EvolutionPoints = 0;

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
                    _EvolutionPoints = 0;

                    MinTameSkill = 90.0;

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
                    _EvolutionPoints = 0;

                    MinTameSkill = 100.0;

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

                    NextEvolution = 3000000;
                    _EvolutionPoints = 0;

                    MinTameSkill = 110.0;

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

                    SpecialTitle = "Elder Dragon";

                    NextEvolution = 0;
                    _EvolutionPoints = 0;

                    MinTameSkill = 120.0;

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

        public void Addpoints(int points)
        {
            if (Stage < 7)
            {
                EvolutionPoints += points;
                if (CanEvolve())
                {
                    _Stage++;
                    PublicOverheadMessage(MessageType.Label, 54, true, RawName + " begins to evolve!");
                    Timer.DelayCall(TimeSpan.FromSeconds(30), Evolve);
                }
            }
        }

        public bool CanEvolve()
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
            writer.Write(_Stage);
            writer.Write(_EvolutionPoints);
            writer.Write(NextEvolution);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                {
                    _Stage = reader.ReadInt();
                    _EvolutionPoints = reader.ReadInt();
                    NextEvolution = reader.ReadInt();
                }
                    break;
            }
        }
    }
}