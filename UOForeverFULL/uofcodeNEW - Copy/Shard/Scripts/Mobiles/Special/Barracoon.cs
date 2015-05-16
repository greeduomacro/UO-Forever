#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.CannedEvil;
using Server.Engines.CustomTitles;
using Server.Items;
using Server.Spells.Fifth;
using Server.Spells.Seventh;

#endregion

namespace Server.Mobiles
{
    public class Barracoon : BaseChampion
    {
        public override ChampionSkullType SkullType { get { return ChampionSkullType.Greed; } }

        [Constructable]
        public Barracoon()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.05, 0.05)
        {
            Name = "Barracoon";
            SpecialTitle = "The Piper";
            TitleHue = 1174;

            //Title = "the piper";

            Body = 0x190;
            Hue = 0x83EC;

            SetStr(535, 655); //increased
            SetDex(90, 110); //increased
            SetInt(735, 890); //increased

            SetHits(11500, 13000); //increased
            SetStam(100, 110); //increased

            SetDamage(35, 45);

            SetSkill(SkillName.MagicResist, 145.0); //increased
            SetSkill(SkillName.Tactics, 97.6, 120.0);
            SetSkill(SkillName.Wrestling, 97.6, 120.0);
            SetSkill(SkillName.Magery, 97.6, 120.0);
            SetSkill(SkillName.EvalInt, 97.6, 120.0);

            Fame = 22500;
            Karma = -22500;

            VirtualArmor = 40;

            AddItem(new FancyShirt(Utility.RandomGreenHue()));
            AddItem(new LongPants(Utility.RandomYellowHue()));
            AddItem(new JesterHat(Utility.RandomPinkHue()));
            AddItem(new Cloak(Utility.RandomPinkHue()));
            AddItem(new Sandals());

            HairItemID = 0x203B; // Short Hair
            HairHue = 0x94;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 3);
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool AutoDispel { get { return true; } }
        public override double AutoDispelChance { get { return 1.0; } }
        public override bool BardImmune { get { return !EraSE; } }
        public override bool Unprovokable { get { return EraSE; } }
        public override bool Uncalmable { get { return EraSE; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }
        public override bool ShowFameTitle { get { return false; } }
        public override bool ClickTitle { get { return false; } }


        public void Polymorph(Mobile m)
        {
            if (!m.CanBeginAction(typeof(PolymorphSpell)) || !m.CanBeginAction(typeof(IncognitoSpell)) || m.IsBodyMod ||
                !m.Alive)
            {
                return;
            }

            IMount mount = m.Mount;

            if (mount != null)
            {
                mount.Rider = null;
            }

            if (m.Mounted)
            {
                return;
            }

            if (m.BeginAction(typeof(PolymorphSpell)))
            {
                Item disarm = m.FindItemOnLayer(Layer.OneHanded);

                if (disarm != null && disarm.Movable)
                {
                    m.AddToBackpack(disarm);
                }

                disarm = m.FindItemOnLayer(Layer.TwoHanded);

                if (disarm != null && disarm.Movable)
                {
                    m.AddToBackpack(disarm);
                }

                m.BodyMod = 42;
                m.HueMod = 0;
                m.Criminal = true;

                new ExpirePolymorphTimer(m).Start();
            }
        }

        public override void GiveSpecialItems(List<Mobile> eligibleMobs, List<double> eligibleMobScores,
            double totalScores)
        {
            double currentTestValue = 0.0;
            double roll = Utility.RandomDouble() * totalScores;
            if (0.2 > Utility.RandomDouble())
            {
                for (int i = 0; i < eligibleMobScores.Count; i++)
                {
                    currentTestValue += eligibleMobScores[i];

                    if (roll > currentTestValue)
                    {
                        continue;
                    }

                    var rand = new Random();
                    int value = rand.Next(0, 8);
                    Item item = null;
                    switch (value)
                    {
                        case 0:
                        {
                            item = new WaterTileArtifact();
                        }
                            break;
                        case 1:
                        {
                            item = new CreepyPortraitArtifact();
                        }
                            break;
                        case 2:
                        {
                            item = new WaterTileArtifact();
                        }
                            break;
                        case 3:
                        {
                            item = new CreepyPortraitArtifact();
                        }
                            break;
                        case 4:
                        {
                            item = new WaterTileArtifact();
                        }
                            break;
                        case 5:
                        {
                            item = new CreepyPortraitArtifact();
                        }
                            break;
                        case 6:
                        {
                            item = new PutridCapArtifact();
                        }
                            break;
                        case 7:
                        {
                            item = new KoonsKryssArtifact();
                        }
                            break;
                    }
                    if (eligibleMobs[i] is PlayerMobile && eligibleMobs[i].Backpack != null && item != null)
                    {
                        eligibleMobs[i].Backpack.DropItem(item);
                        eligibleMobs[i].SendMessage(54, "You have received " + item.Name + ".");
                        break;
                    }
                }
            }

            if (0.2 > Utility.RandomDouble())
            {
                for (int i = 0; i < eligibleMobScores.Count; i++)
                {
                    currentTestValue += eligibleMobScores[i];

                    if (roll > currentTestValue)
                    {
                        continue;
                    }

                    if (eligibleMobs[i] is PlayerMobile && eligibleMobs[i].Backpack != null)
                    {
                        eligibleMobs[i].Backpack.DropItem(new TitleScroll("The Rat King"));
                        eligibleMobs[i].SendMessage(54, "You have received a title scroll.");
                        return;
                    }
                }
            }
        }

        private class ExpirePolymorphTimer : Timer
        {
            private readonly Mobile m_Owner;

            public ExpirePolymorphTimer(Mobile owner) : base(TimeSpan.FromMinutes(1.0))
            {
                m_Owner = owner;

                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (!m_Owner.CanBeginAction(typeof(PolymorphSpell)))
                {
                    m_Owner.BodyMod = 0;
                    m_Owner.HueMod = -1;
                    m_Owner.EndAction(typeof(PolymorphSpell));
                }
            }
        }


        public void SpawnRatmen(Mobile target)
        {
            Map map = Map;

            if (map == null)
            {
                return;
            }

            int rats =
                GetMobilesInRange(10).Cast<Mobile>().Count(m => m is Ratman || m is RatmanArcher || m is RatmanMage);

            if (rats < 6)
            {
                PlaySound(0x3D);

                int newRats = Utility.RandomMinMax(3, 5);

                for (int i = 0; i < newRats; ++i)
                {
                    BaseCreature rat;

                    switch (Utility.Random(5))
                    {
                        default:
                        case 0:
                        case 1:
                            rat = new Ratman();
                            break;
                        case 2:
                        case 3:
                            rat = new RatmanArcher();
                            break;
                        case 4:
                            rat = new RatmanMage();
                            break;
                    }

                    rat.Team = Team;

                    Point3D loc = Location;

                    for (int j = 0; j < 10; ++j)
                    {
                        int x = X + Utility.Random(3) - 1;
                        int y = Y + Utility.Random(3) - 1;
                        int z = map.GetAverageZ(x, y);

                        if (false == map.CanFit(x, y, Z, 16, false, false))
                        {
                            loc = new Point3D(x, y, Z);
                        }
                        else if (false == map.CanFit(x, y, z, 16, false, false))
                        {
                            loc = new Point3D(x, y, z);
                        }
                    }

                    rat.MoveToWorld(loc, map);
                    // take away insta kill
                    //rat.Combatant = target;
                }
            }
        }

        public void DoSpecialAbility(Mobile target)
        {
            if (target == null || target.Deleted) //sanity
            {
                return;
            }

            if (0.6 >= Utility.RandomDouble()) // 60% chance to polymorph attacker into a ratman
            {
                Polymorph(target);
            }

            if (0.2 >= Utility.RandomDouble()) // 20% chance to more ratmen
            {
                SpawnRatmen(target);
            }

            if (Hits < 500 && !IsBodyMod) // Baracoon is low on life, polymorph into a ratman
            {
                Polymorph(this);
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            if (from != null)
            {
                DoSpecialAbility(from);
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            DoSpecialAbility(defender);
        }

        public Barracoon(Serial serial)
            : base(serial)
        {}

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
}