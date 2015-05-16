#region References

using System;
using System.Collections.Generic;
using Server.Mobiles;
using VitaNex;

#endregion

namespace Server.Engines.ZombieEvent
{
    public sealed class PlayerZombieProfile : PropertyObject
    {
        [CommandProperty(ZombieEvent.Access, true)]
        public PlayerMobile Owner { get; private set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public ZombieAvatar ZombieAvatar { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public bool Active { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public DateTime DisconnectTime { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public Point3D ZombieSavePoint { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public Timer LeaveEventTimer { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public int Deaths { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public int Kills { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public int DragonBossDamage { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public Dictionary<ZombieInstanceSerial, int> ZombieKills { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public Dictionary<ZombieInstanceSerial, int> DaemonKills { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public Dictionary<ZombieInstanceSerial, int> FeyKills { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public Dictionary<ZombieInstanceSerial, int> GoreFiendKills { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public Dictionary<ZombieInstanceSerial, int> TentacleKills { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public Dictionary<ZombieInstanceSerial, int> TreefellowKills { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public Dictionary<ZombieInstanceSerial, int> VitriolKills { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public Dictionary<ZombieInstanceSerial, int> SpiderKills { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public int LifeTimeZombieKills { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public int OverallScore { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public int SpendablePoints { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public int Tier1Cap { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public int Tier2Cap { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public int Tier3Cap { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public int Tier4Cap { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public int Tier5Cap { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public int Tier6Cap { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public int Tier7Cap { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public int Tier8Cap { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public int Tier9Cap { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public int Tier10Cap { get; set; }


        public PlayerZombieProfile(PlayerMobile pm)
        {
            Owner = pm;
            ZombieKills = new Dictionary<ZombieInstanceSerial, int>();
            DaemonKills = new Dictionary<ZombieInstanceSerial, int>();
            FeyKills = new Dictionary<ZombieInstanceSerial, int>();
            GoreFiendKills = new Dictionary<ZombieInstanceSerial, int>();
            TentacleKills = new Dictionary<ZombieInstanceSerial, int>();
            TreefellowKills = new Dictionary<ZombieInstanceSerial, int>();
            VitriolKills = new Dictionary<ZombieInstanceSerial, int>();
            SpiderKills = new Dictionary<ZombieInstanceSerial, int>();
        }

        public PlayerZombieProfile(GenericReader reader)
            : base(reader)
        {}

        public ZombieAvatar CreateAvatar()
        {
            var mob = new ZombieAvatar(Owner);

            mob.Name = Owner.RawName;
            mob.Hue = Owner.Hue;
            mob.FacialHairItemID = Owner.FacialHairItemID;
            mob.HairItemID = Owner.HairItemID;
            mob.HairHue = Owner.HairHue;
            mob.BodyValue = Owner.BodyValue;
            mob.RawStr = 45;
            mob.RawInt = 35;
            mob.RawDex = 35;
            mob.Hits = 60;
            mob.Stam = 35;
            mob.Mana = 35;
            mob.SkillsCap = 50000;
            mob.StatCap = 225;


            mob.Skills.Anatomy.Base = 35;
            mob.Skills.Macing.Base = 35;
            mob.Skills.Swords.Base = 35;
            mob.Skills.Archery.Base = 35;
            mob.Skills.MagicResist.Base = 35;
            mob.Skills.Wrestling.Base = 35;
            mob.Skills.Fencing.Base = 35;
            mob.Skills.Poisoning.Base = 40;
            mob.Skills.ItemID.Base = 25;
            mob.Skills.ArmsLore.Base = 25;
            mob.Skills.Alchemy.Base = 40;
            mob.Skills.Parry.Base = 10;
            mob.Skills.Blacksmith.Base = 40;
            mob.Skills.Fletching.Base = 25;
            mob.Skills.Peacemaking.Base = 40;
            mob.Skills.Camping.Base = 40;
            mob.Skills.Carpentry.Base = 40;
            mob.Skills.Cartography.Base = 40;
            mob.Skills.Cooking.Base = 40;
            mob.Skills.EvalInt.Base = 35;
            mob.Skills.Healing.Base = 40;
            mob.Skills.Forensics.Base = 40;
            mob.Skills.Inscribe.Base = 35;
            mob.Skills.Lockpicking.Base = 40;
            mob.Skills.Magery.Base = 35;
            mob.Skills.Tactics.Base = 35;
            mob.Skills.Tinkering.Base = 45;
            mob.Skills.Lumberjacking.Base = 40;
            mob.Skills.Mining.Base = 25;
            mob.Skills.Meditation.Base = 35;
            mob.Skills.RemoveTrap.Base = 40;
            ZombieAvatar = mob;

            return mob;
        }

        public void AddKill(BaseCreature mob)
        {
            if (mob is ZombieZEvent)
            {
                if (ZombieKills.ContainsKey(mob.ZombieSerial))
                {
                    ZombieKills[mob.ZombieSerial]++;
                }
                else
                {
                    ZombieKills.Add(mob.ZombieSerial, 1);
                }
                OverallScore++;
                SpendablePoints++;
            }
            else if (mob is DaemonZombieEvent)
            {
                if (DaemonKills.ContainsKey(mob.ZombieSerial))
                {
                    DaemonKills[mob.ZombieSerial]++;
                }
                else
                {
                    DaemonKills.Add(mob.ZombieSerial, 1);
                }
                OverallScore += 5;
                SpendablePoints += 5;
            }
            else if (mob is FeyWarrior)
            {
                if (FeyKills.ContainsKey(mob.ZombieSerial))
                {
                    FeyKills[mob.ZombieSerial]++;
                }
                else
                {
                    FeyKills.Add(mob.ZombieSerial, 1);
                }
                OverallScore += 10;
                SpendablePoints += 10;
            }
            else if (mob is GoreFiendZombieEvent)
            {
                if (GoreFiendKills.ContainsKey(mob.ZombieSerial))
                {
                    GoreFiendKills[mob.ZombieSerial]++;
                }
                else
                {
                    GoreFiendKills.Add(mob.ZombieSerial, 1);
                }
                OverallScore += 10;
                SpendablePoints += 10;
            }
            else if (mob is HorrifyingTentacle)
            {
                if (TentacleKills.ContainsKey(mob.ZombieSerial))
                {
                    TentacleKills[mob.ZombieSerial]++;
                }
                else
                {
                    TentacleKills.Add(mob.ZombieSerial, 1);
                }
                OverallScore += 1;
                SpendablePoints += 1;
            }
            else if (mob is TreeFellow)
            {
                if (TreefellowKills.ContainsKey(mob.ZombieSerial))
                {
                    TreefellowKills[mob.ZombieSerial]++;
                }
                else
                {
                    TreefellowKills.Add(mob.ZombieSerial, 1);
                }
                OverallScore += 10;
                SpendablePoints += 10;
            }
            else if (mob is Vitriol)
            {
                if (VitriolKills.ContainsKey(mob.ZombieSerial))
                {
                    VitriolKills[mob.ZombieSerial]++;
                }
                else
                {
                    VitriolKills.Add(mob.ZombieSerial, 1);
                }
                OverallScore += 20;
                SpendablePoints += 20;
            }
            else if (mob is ZombieSpider)
            {
                if (SpiderKills.ContainsKey(mob.ZombieSerial))
                {
                    SpiderKills[mob.ZombieSerial]++;
                }
                else
                {
                    SpiderKills.Add(mob.ZombieSerial, 1);
                }
                OverallScore += 2;
                SpendablePoints += 2;
            }
        }

        public void RecalculatePoints()
        {
            var instance = ZombieEvent.GetInstance();
            OverallScore = 0;
            SpendablePoints = 0;

            OverallScore += 1 * GetZombieKills(instance.Uid);
            OverallScore += 5 * GetDaemonKills(instance.Uid);
            OverallScore += 10 * GetFeyKills(instance.Uid);
            OverallScore += 1 * GetTentacleKills(instance.Uid);
            OverallScore += 10 * GetTreefellowKills(instance.Uid);
            OverallScore += 20 * GetVitriolKills(instance.Uid);
            OverallScore += 10 * GetGoreFiendKills(instance.Uid);
            OverallScore += 2 * GetSpiderKills(instance.Uid);

            SpendablePoints += 1 * GetZombieKills(instance.Uid);
            SpendablePoints += 5 * GetDaemonKills(instance.Uid);
            SpendablePoints += 10 * GetFeyKills(instance.Uid);
            SpendablePoints += 1 * GetTentacleKills(instance.Uid);
            SpendablePoints += 10 * GetTreefellowKills(instance.Uid);
            SpendablePoints += 20 * GetVitriolKills(instance.Uid);
            SpendablePoints += 10 * GetGoreFiendKills(instance.Uid);
            SpendablePoints += 2 * GetSpiderKills(instance.Uid);
        }

        public int GetZombieKills(ZombieInstanceSerial uid)
        {
            if (ZombieKills.ContainsKey(uid))
            {
                return ZombieKills[uid];
            }
            return 0;
        }

        public int GetDaemonKills(ZombieInstanceSerial uid)
        {
            if (DaemonKills.ContainsKey(uid))
            {
                return DaemonKills[uid];
            }
            return 0;
        }

        public int GetFeyKills(ZombieInstanceSerial uid)
        {
            if (FeyKills.ContainsKey(uid))
            {
                return FeyKills[uid];
            }
            return 0;
        }

        public int GetTentacleKills(ZombieInstanceSerial uid)
        {
            if (TentacleKills.ContainsKey(uid))
            {
                return TentacleKills[uid];
            }
            return 0;
        }

        public int GetTreefellowKills(ZombieInstanceSerial uid)
        {
            if (TreefellowKills.ContainsKey(uid))
            {
                return TreefellowKills[uid];
            }
            return 0;
        }

        public int GetVitriolKills(ZombieInstanceSerial uid)
        {
            if (VitriolKills.ContainsKey(uid))
            {
                return VitriolKills[uid];
            }
            return 0;
        }

        public int GetGoreFiendKills(ZombieInstanceSerial uid)
        {
            if (GoreFiendKills.ContainsKey(uid))
            {
                return GoreFiendKills[uid];
            }
            return 0;
        }

        public int GetSpiderKills(ZombieInstanceSerial uid)
        {
            if (SpiderKills.ContainsKey(uid))
            {
                return SpiderKills[uid];
            }
            return 0;
        }

        public override void Reset()
        {}

        public override void Clear()
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(2);

            switch (version)
            {
                case 2:
                    {
                        writer.Write(Tier1Cap);
                        writer.Write(Tier2Cap);
                        writer.Write(Tier3Cap);
                        writer.Write(Tier4Cap);
                        writer.Write(Tier5Cap);
                        writer.Write(Tier6Cap);
                        writer.Write(Tier7Cap);
                        writer.Write(Tier8Cap);
                        writer.Write(Tier9Cap);
                        writer.Write(Tier10Cap);
                    }
                    goto case 1;
                case 1:
                {
                    writer.Write(DragonBossDamage);
                }
                    goto case 0;
                case 0:
                {
                    writer.WriteMobile(Owner);
                    writer.Write(OverallScore);
                    writer.Write(SpendablePoints);
                    writer.Write(Kills);
                    writer.Write(Deaths);
                    writer.Write(ZombieAvatar);
                    writer.Write(Active);
                    writer.Write(ZombieSavePoint);

                    writer.Write(ZombieKills.Count);

                    if (ZombieKills.Count > 0)
                    {
                        foreach (KeyValuePair<ZombieInstanceSerial, int> kvp in ZombieKills)
                        {
                            kvp.Key.Serialize(writer);
                            writer.Write(kvp.Value);
                        }
                    }

                    writer.Write(DaemonKills.Count);

                    if (DaemonKills.Count > 0)
                    {
                        foreach (KeyValuePair<ZombieInstanceSerial, int> kvp in DaemonKills)
                        {
                            kvp.Key.Serialize(writer);
                            writer.Write(kvp.Value);
                        }
                    }

                    writer.Write(FeyKills.Count);

                    if (FeyKills.Count > 0)
                    {
                        foreach (KeyValuePair<ZombieInstanceSerial, int> kvp in FeyKills)
                        {
                            kvp.Key.Serialize(writer);
                            writer.Write(kvp.Value);
                        }
                    }

                    writer.Write(GoreFiendKills.Count);

                    if (GoreFiendKills.Count > 0)
                    {
                        foreach (KeyValuePair<ZombieInstanceSerial, int> kvp in GoreFiendKills)
                        {
                            kvp.Key.Serialize(writer);
                            writer.Write(kvp.Value);
                        }
                    }

                    writer.Write(TentacleKills.Count);

                    if (TentacleKills.Count > 0)
                    {
                        foreach (KeyValuePair<ZombieInstanceSerial, int> kvp in TentacleKills)
                        {
                            kvp.Key.Serialize(writer);
                            writer.Write(kvp.Value);
                        }
                    }

                    writer.Write(TreefellowKills.Count);

                    if (TreefellowKills.Count > 0)
                    {
                        foreach (KeyValuePair<ZombieInstanceSerial, int> kvp in TreefellowKills)
                        {
                            kvp.Key.Serialize(writer);
                            writer.Write(kvp.Value);
                        }
                    }

                    writer.Write(VitriolKills.Count);

                    if (VitriolKills.Count > 0)
                    {
                        foreach (KeyValuePair<ZombieInstanceSerial, int> kvp in VitriolKills)
                        {
                            kvp.Key.Serialize(writer);
                            writer.Write(kvp.Value);
                        }
                    }

                    writer.Write(SpiderKills.Count);

                    if (SpiderKills.Count > 0)
                    {
                        foreach (KeyValuePair<ZombieInstanceSerial, int> kvp in SpiderKills)
                        {
                            kvp.Key.Serialize(writer);
                            writer.Write(kvp.Value);
                        }
                    }
                }
                    break;
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            ZombieKills = new Dictionary<ZombieInstanceSerial, int>();
            DaemonKills = new Dictionary<ZombieInstanceSerial, int>();
            FeyKills = new Dictionary<ZombieInstanceSerial, int>();
            GoreFiendKills = new Dictionary<ZombieInstanceSerial, int>();
            TentacleKills = new Dictionary<ZombieInstanceSerial, int>();
            TreefellowKills = new Dictionary<ZombieInstanceSerial, int>();
            VitriolKills = new Dictionary<ZombieInstanceSerial, int>();
            SpiderKills = new Dictionary<ZombieInstanceSerial, int>();

            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                {
                    Tier1Cap = reader.ReadInt();
                    Tier2Cap = reader.ReadInt();
                    Tier3Cap = reader.ReadInt();
                    Tier4Cap = reader.ReadInt();
                    Tier5Cap = reader.ReadInt();
                    Tier6Cap = reader.ReadInt();
                    Tier7Cap = reader.ReadInt();
                    Tier8Cap = reader.ReadInt();
                    Tier9Cap = reader.ReadInt();
                    Tier10Cap = reader.ReadInt();
                }
                    goto case 1;
                case 1:
                {
                    DragonBossDamage = reader.ReadInt();
                }
                    goto case 0;
                case 0:
                {
                    Owner = reader.ReadMobile<PlayerMobile>();

                    OverallScore = reader.ReadInt();

                    SpendablePoints = reader.ReadInt();

                    Kills = reader.ReadInt();

                    Deaths = reader.ReadInt();

                    ZombieAvatar = reader.ReadMobile<ZombieAvatar>();

                    Active = reader.ReadBool();

                    ZombieSavePoint = reader.ReadPoint3D();

                    int count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var p = new ZombieInstanceSerial(reader);
                            int amt = reader.ReadInt();
                            ZombieKills.Add(p, amt);
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var p = new ZombieInstanceSerial(reader);
                            int amt = reader.ReadInt();
                            DaemonKills.Add(p, amt);
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var p = new ZombieInstanceSerial(reader);
                            int amt = reader.ReadInt();
                            FeyKills.Add(p, amt);
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var p = new ZombieInstanceSerial(reader);
                            int amt = reader.ReadInt();
                            GoreFiendKills.Add(p, amt);
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var p = new ZombieInstanceSerial(reader);
                            int amt = reader.ReadInt();
                            TentacleKills.Add(p, amt);
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var p = new ZombieInstanceSerial(reader);
                            int amt = reader.ReadInt();
                            TreefellowKills.Add(p, amt);
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var p = new ZombieInstanceSerial(reader);
                            int amt = reader.ReadInt();
                            VitriolKills.Add(p, amt);
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var p = new ZombieInstanceSerial(reader);
                            int amt = reader.ReadInt();
                            SpiderKills.Add(p, amt);
                        }
                    }
                }
                    break;
            }
        }
    }
}