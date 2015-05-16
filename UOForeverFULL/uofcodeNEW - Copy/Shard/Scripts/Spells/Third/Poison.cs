#region References

using System.Linq;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

#endregion

namespace Server.Spells.Third
{
    public class PoisonSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Poison", "In Nox", 203, 9051, Reagent.Nightshade);

        public override SpellCircle Circle { get { return SpellCircle.Third; } }

        public override double GetResistPercentForCircle(Mobile target, SpellCircle circle)
        {
            if (Caster.EraUOR)
            {
                var p = Caster as PlayerMobile;
                if (p != null && p.DuelContext != null && !p.DuelContext.Ruleset.GetOption("Skills", "Poisoning"))
                {
                    return (0.5 +
                            ((target.Skills[SkillName.MagicResist].Value - (Caster.Skills[SkillName.Magery].Value + 10)) /
                             40));
                }
                var poison = (0.5 +
                        ((target.Skills[SkillName.MagicResist].Value - (Caster.Skills[SkillName.Magery].Value + 10)) /
                         40))
                       * (1.0 - (Caster.Skills[SkillName.Poisoning].Value / 100));

                if (poison <= 0)
                    poison = 0.05;
                return poison;
            }

            if (Caster.EraT2A)
            {
                var p = Caster as PlayerMobile;
                if (p != null && p.DuelContext != null && p.DuelContext.Ruleset.GetOption("Skills", "Poisoning"))
                {
                    return (0.5 +
                            ((target.Skills[SkillName.MagicResist].Value - (Caster.Skills[SkillName.Magery].Value + 10)) /
                             40));
                }
                return (0.5 +
                        ((target.Skills[SkillName.MagicResist].Value - (Caster.Skills[SkillName.Magery].Value + 10)) /
                         40))
                       * (1.0 - (Caster.Skills[SkillName.Poisoning].Value / 100));
            }

            return base.GetResistPercentForCircle(target, circle) -
                   (0.01 * Caster.Skills[SkillName.Poisoning].Value * SpellDamageController._PoisonSkillResistModifier);
        }

        public PoisonSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {}

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public bool CheckSlayerResist(Mobile target)
        {
            if (Caster == null || !(Caster is BaseCreature) || !target.Player)
            {
                return false;
            }

            var bcCaster = (BaseCreature) Caster;
            int slayerPieces =
                target.Items.OfType<BaseArmor>()
                    .Count(armor => armor.CheckSlayers(bcCaster, target) == CheckSlayerResult.Slayer);

            if (slayerPieces > 0)
            {
                int hitSlayerPieceChance = Utility.Random(6);
                double roll = Utility.RandomDouble();

                // have a 75% chance to resist the poison if it "lands" on a slayer armor piece (full suit is 100% landing rate)
                if (hitSlayerPieceChance < slayerPieces && roll < 0.75)
                {
                    return true;
                }
            }

            return false;
        }

        public void Target(Mobile m)
        {
            if (!Caster.CanSee(m))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (CheckHSequence(m))
            {
                SpellHelper.Turn(Caster, m);
                Caster.DoHarmful(m);

                SpellHelper.CheckReflect((int) Circle, Caster, ref m);

                if (m.Spell != null)
                {
                    m.Spell.OnCasterHurt();
                }

                m.Paralyzed = false;

                if (CheckResisted(m) || CheckSlayerResist(m))
                {
                    m.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                }
                else
                {
                    int level = 0;

                    if (Caster.EraAOS)
                    {
                        //double total = Caster.Skills[SkillName.Magery].Value + Caster.Skills[SkillName.Poisoning].Value;

                        #region Dueling

                        double total = Caster.Skills[SkillName.Magery].Value;

                        if (Caster is PlayerMobile)
                        {
                            var pm = (PlayerMobile) Caster;

                            if (pm.DuelContext == null || !pm.DuelContext.Started || pm.DuelContext.Finished ||
                                pm.DuelContext.Ruleset.GetOption("Skills", "Poisoning"))
                            {
                                total += Caster.Skills[SkillName.Poisoning].Value;
                            }
                        }
                        else
                        {
                            total += Caster.Skills[SkillName.Poisoning].Value;
                        }

                        #endregion

                        double dist = Caster.GetDistanceToSqrt(m);

                        if (dist >= 3.0)
                        {
                            total -= (dist - 3.0) * 10.0;
                        }

                        if (total >= 200.0 && 1 > Utility.Random(15))
                        {
                            level = 3;
                        }
                        else if (total > (Caster.EraAOS ? 170.1 : 170.0))
                        {
                            level = 2;
                        }
                        else if (total > (Caster.EraAOS ? 130.1 : 130.0))
                        {
                            level = 1;
                        }
                    }
                    else if (!Caster.IsT2A)
                    {
                        #region Dueling
                        double total = Caster.Skills[SkillName.Magery].Value;
                        if (Caster is PlayerMobile)
                        {
                            var pm = (PlayerMobile) Caster;
                            if (pm.DuelContext != null && pm.DuelContext.Started && !pm.DuelContext.Finished &&
                                !pm.DuelContext.Ruleset.GetOption("Skills", "Poisoning"))
                            {}
                            else
                            {
                                total += Caster.Skills[SkillName.Poisoning].Value;
                            }
                        }
                        else
                        {
                            total += Caster.Skills[SkillName.Poisoning].Value;
                        }

                        #endregion

                        double dist = Caster.GetDistanceToSqrt(m);
                        if (dist >= 3.0)
                        {
                            total -= (dist - 3.0) * 10.0;
                        }
                        if (total >= 200.0 && 1 > Utility.Random(10))
                        {
                            level = 3;
                        }
                        else if (total > 170.0)
                        {
                            level = 2;
                        }
                        else if (total > 130.0)
                        {
                            level = 1;
                        }
                        else
                        {
                            level = 0;
                        }
                    }

                    m.ApplyPoison(Caster, Poison.GetPoison(level));
                }

                m.FixedParticles(0x374A, 10, 15, 5021, EffectLayer.Waist);
                m.PlaySound(0x205);

                HarmfulSpell(m);
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly PoisonSpell m_Owner;

            public InternalTarget(PoisonSpell owner)
                : base(owner.Caster.EraML ? 10 : 12, false, TargetFlags.Harmful)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                var entity = o as IEntity;
                if (XmlScript.HasTrigger(entity, TriggerName.onTargeted) &&
                    UberScriptTriggers.Trigger(entity, from, TriggerName.onTargeted, null, null, m_Owner))
                {
                    return;
                }
                if (o is Mobile)
                {
                    m_Owner.Target((Mobile) o);
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}