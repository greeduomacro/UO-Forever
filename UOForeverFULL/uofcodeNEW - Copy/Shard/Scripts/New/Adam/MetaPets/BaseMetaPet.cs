#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Items;
using Server.Mobiles.MetaPet.Skills;
using Server.Mobiles.MetaSkills;
using Server.Network;

#endregion

namespace Server.Mobiles.MetaPet
{
    [CorpseName("a pet corpse")]
    public class BaseMetaPet : BaseCreature
    {
        public Dictionary<MetaSkillType, BaseMetaSkill> Metaskills;

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

        [CommandProperty(AccessLevel.GameMaster)]
        public int NextEvolution { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxStage { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Stage
        {
            get { return _Stage; }
            set
            {
                if (value > 0 && value <= MaxStage && value != _Stage)
                {
                    _Stage = value;
                    Evolve();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxAbilities { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CurrentAbilities { get; set; }

        [Constructable]
        public BaseMetaPet() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a meta-pet";
            Body = 52;
            Hue = 0;
            BaseSoundID = 0xDB;
            _Stage = 1;

            SpecialTitle = "Meta-Pet";
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

            MaxStage = 1;

            Metaskills = new Dictionary<MetaSkillType, BaseMetaSkill>();
        }

        public virtual void DoAbility(MetaSkillType skill, BaseCreature target)
        {
            if (Metaskills != null && Metaskills.ContainsKey(skill))
            {
                BaseMetaSkill metaskill = Metaskills[skill];
                metaskill.FindAbility(target, this);
            }
        }

        public BaseMetaPet(Serial serial)
            : base(serial)
        {}

        public virtual void Evolve()
        {
            Effects.SendFlashEffect(this, (FlashType) 2);
        }


        public override void OnDeath(Container c)
        {
            if (LastKiller != null)
            {
                if (EvolutionPoints - LastKiller.Hits > 0)
                {
                    var points = LastKiller.Hits;
                    if (points > 10000)
                        points = 10000;
                    EvolutionPoints -= points;
                }
                else
                {
                    EvolutionPoints = 0;
                }
            }
            base.OnDeath(c);
        }

        public virtual void Addpoints(BaseCreature creature, int points)
        {
            if (Stage < MaxStage && MasterInRange() && !(creature is VorpalBunny))
            {
                EvolutionPoints += points;
                if (CanEvolve())
                {
                    _Stage++;
                    PublicOverheadMessage(MessageType.Label, 54, true, RawName + " begins to metamorphosise!");
                    Timer.DelayCall(TimeSpan.FromSeconds(30), Evolve);
                }
            }
        }

        public bool MasterInRange()
        {
            var list =  GetMobilesInRange(10);

            return (from object mobile in list select mobile as PlayerMobile).Any(mob => mob != null && ControlMaster == mob && ControlMaster.Alive && ControlMaster.CanSee(this));
        }

        public virtual bool CanEvolve()
        {
            return false;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant is BaseCreature && !Combatant.IsControlled())
            {
                DoAbility(MetaSkillType.Molten, Combatant as BaseCreature);
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (from is BaseCreature)
            {
                DoAbility(MetaSkillType.VenemousBlood, from as BaseCreature);
            }
            base.OnDamage(amount, from, willKill);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            if (defender is BaseCreature)
            {
                DoAbility(MetaSkillType.Quicksilver, defender as BaseCreature);
                DoAbility(MetaSkillType.Bleed, defender as BaseCreature);
            }
            base.OnGaveMeleeAttack(defender);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
            writer.Write(_Stage);
            writer.Write(_EvolutionPoints);
            writer.Write(NextEvolution);
            writer.Write(MaxStage);
            writer.Write(MaxAbilities);
            writer.Write(CurrentAbilities);

            writer.WriteDictionary(
                Metaskills,
                (t, s) =>
                {
                    writer.Write((int)t);

                    s.Serialize(writer);
                });
        }

        public override void Deserialize(GenericReader reader)
        {
            Metaskills = new Dictionary<MetaSkillType, BaseMetaSkill>();
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                {
                    _Stage = reader.ReadInt();
                    _EvolutionPoints = reader.ReadInt();
                    NextEvolution = reader.ReadInt();
                    MaxStage = reader.ReadInt();
                    MaxAbilities = reader.ReadInt();
                    CurrentAbilities = reader.ReadInt();

                    Metaskills = reader.ReadDictionary(
                        () =>
                        {
                            var c = (MetaSkillType) reader.ReadInt();

                            var s = new BaseMetaSkill(reader);

                            return new KeyValuePair<MetaSkillType, BaseMetaSkill>(c, s);
                        });
                }
                    break;
            }
        }
    }
}