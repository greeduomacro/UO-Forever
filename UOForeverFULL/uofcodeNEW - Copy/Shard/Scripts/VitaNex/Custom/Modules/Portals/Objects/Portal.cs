#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Items;
using Server.Mobiles;
using Server.Regions;
using VitaNex;
using VitaNex.FX;
using VitaNex.Network;
using VitaNex.Notify;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Engines.Portals
{
    public sealed class Portal : PropertyObject
    {
        private int _CoreTicks;
        private PollTimer _CoreTimer;
        private int count1 = 1;
        private int count2 = 1;
        private int count3;

        public Portal(PortalType ptype, DateTime date)
        {
            UID = new PortalSerial();

            PortalType = ptype;
            DateStarted = date;
            Mobs = new List<Mobile>();
            PortalCrystals = new List<PortalCrystal>();
            ParticipantsScores = new Dictionary<PlayerMobile, int>();
            BossSpawned = false;
            CurrentParticipants = new List<Mobile>();
            GetPortalLocation();
            StartPortal();
        }

        public Portal(GenericReader reader)
            : base(reader)
        {}

        [CommandProperty(Portals.Access, true)]
        public PortalSerial UID { get; private set; }

        [CommandProperty(Portals.Access, true)]
        public PortalItem PortalItem { get; set; }

        [CommandProperty(Portals.Access, true)]
        public PortalItem PortalItem2 { get; set; }

        [CommandProperty(Portals.Access, true)]
        public FireElemental Anim1 { get; set; }

        [CommandProperty(Portals.Access, true)]
        public FireElemental Anim2 { get; set; }

        [CommandProperty(Portals.Access, true)]
        public PortalType PortalType { get; set; }

        [CommandProperty(Portals.Access, true)]
        public Point3D PortalEntrance { get; set; }

        [CommandProperty(Portals.Access, true)]
        public string RegionName { get; set; }

        [CommandProperty(Portals.Access, true)]
        public Region Region { get; set; }

        [CommandProperty(Portals.Access, true)]
        public Point3D DungeonEntrance { get; set; }

        [CommandProperty(Portals.Access, true)]
        public Map DungeonMap { get; set; }

        [CommandProperty(Portals.Access, true)]
        public List<Mobile> Mobs { get; set; }

        [CommandProperty(Portals.Access, true)]
        public List<Mobile> CurrentParticipants { get; set; }

        [CommandProperty(Portals.Access, true)]
        public BaseCreature Boss { get; set; }

        [CommandProperty(Portals.Access, true)]
        public List<PortalCrystal> PortalCrystals { get; set; }

        [CommandProperty(Portals.Access, true)]
        public DateTime DateStarted { get; set; }

        [CommandProperty(Portals.Access, true)]
        public PortalStatus Status { get; set; }

        [CommandProperty(Portals.Access, true)]
        public bool BossSpawned { get; set; }

        [CommandProperty(Portals.Access, true)]
        public bool PortalCompleted { get; set; }

        [CommandProperty(Portals.Access, true)]
        public Dictionary<PlayerMobile, int> ParticipantsScores { get; set; }

        [CommandProperty(Portals.Access, true)]
        public List<Point3D> ValidSpawnPoints { get; set; }

        public override void Reset()
        {}

        public override void Clear()
        {}

        public void init()
        {
            bool success = SpawnPortal();
            if (!success)
            {
                Console.WriteLine("Could not find a valid portal location, aborting.");
                return;
            }
            GenerateAnims();
            _CoreTimer = PollTimer.FromSeconds(1.0, OnTick, () => Status == PortalStatus.Running);
        }

        public void OnTick()
        {
            if (_CoreTicks == 0)
            {
                SpawnMobs();
                SpawnCrystals();
            }

            if (Mobs.Count == 0 && !BossSpawned)
            {
                BossSpawned = true;
                string msg = "The master of the portal has begun to awaken!";
                Message(msg);
                if (ValidSpawnPoints.Count == 0)
                {
                    Region r = Region.Regions.Find(x => x.Name == RegionName);
                    ValidSpawnPoints = GenerateSpawnLocations(r);
                }
                Point3D loc = ValidSpawnPoints.GetRandom();
                FlameSpirtal(loc);
                Timer.DelayCall(TimeSpan.FromMinutes(0.1), SpawnBoss, loc);
            }


            if (_CoreTicks == Portals.CSOptions.PortalCloseTime * 3600)
            {
                string msg = "The attempt to seal the portal has failed.  The portal will close in 2 minutes.";
                Message(msg);
                GenerateScoreboards();

                _CoreTimer.Stop();

                Status = PortalStatus.Failed;

                Timer.DelayCall(TimeSpan.FromMinutes(2), FinishPortal);
            }

            ++_CoreTicks;
        }

        public void StartPortal()
        {
            Status = PortalStatus.Running;
            init();
        }

        public void StopPortal()
        {
            Status = PortalCompleted ? PortalStatus.Finished : PortalStatus.Failed;
            Eject();
            PortalItem.Delete();
            PortalItem2.Delete();
            RemoveMobs();
            RemoveCrystals();
            _CoreTimer.Stop();
            ParticipantsScores.Clear();
            if (ValidSpawnPoints != null)
            {
                ValidSpawnPoints.Clear();
                ValidSpawnPoints.TrimExcess();
            }
        }

        public void FinishPortal()
        {
            Status = PortalCompleted ? PortalStatus.Finished : PortalStatus.Failed;
            Eject();
            PortalItem.Delete();
            PortalItem2.Delete();
            RemoveMobs();
            RemoveCrystals();
            _CoreTimer.Stop();
            ParticipantsScores.Clear();
            if (ValidSpawnPoints != null)
            {
                ValidSpawnPoints.Clear();
                ValidSpawnPoints.TrimExcess();
            }
            Portals.GeneratePortal();
        }

        public void BossDeathCleanup()
        {
            PortalCompleted = true;
            Eject();
            PortalItem.Delete();
            PortalItem2.Delete();
            RemoveMobs();
            RemoveCrystals();
            ParticipantsScores.Clear();
            if (ValidSpawnPoints != null)
            {
                ValidSpawnPoints.Clear();
                ValidSpawnPoints.TrimExcess();
            }
        }

        public void GenerateScoreboards()
        {
            foreach (PlayerMobile mobile in ParticipantsScores.Keys)
            {
                PlayerMobile mobile1 = mobile;
                Timer.DelayCall(
                    TimeSpan.FromMinutes(1),
                    () =>
                    {
                        var scrollgump = new PortalScoresScrollGump(mobile1, onAccept: x =>
                        {
                            var scoreboard = new PortalScoresOverviewGump(mobile1, this).
                                Send<InvasionScoresOverviewGump>();
                        }).Send<PortalScoresScrollGump>();
                    });
            }
        }

        public void SpawnMobs()
        {
            switch (PortalType)
            {
                case PortalType.Undead:
                {
                    UndeadPortal();
                    break;
                }
                case PortalType.Beetle:
                {
                    BeetlePortal();
                    break;
                }
                case PortalType.Demon:
                {
                    DemonPortal();
                    break;
                }
                case PortalType.Wyrm:
                {
                    WyrmPortal();
                    break;
                }
                case PortalType.Lummox:
                {
                    LummoxPortal();
                    break;
                }
                case PortalType.Minotaur:
                {
                    MinotaurPortal();
                    break;
                }
            }

            Region r = Region.Regions.Find(x => x.Name == RegionName);

            if (r == null)
            {
                Console.WriteLine("NO REGION FOUND");
                return;
            }

            if (ValidSpawnPoints == null || ValidSpawnPoints.Count == 0)
            {
                ValidSpawnPoints = GenerateSpawnLocations(r);
            }
            foreach (Mobile mob in Mobs.Where(mob => mob.Map == Map.Internal))
            {
                mob.MoveToWorld(ValidSpawnPoints.GetRandom(), DungeonMap);
            }
            if (PortalType == PortalType.Wyrm && Utility.RandomDouble() <= 0.05)
            {
                var mob = new ChromaticDragonPortal {Portal = this};
                mob.MoveToWorld(ValidSpawnPoints.GetRandom(), DungeonMap);
            }
        }

        public void SpawnCrystals()
        {
            for (int i = 0; i < 4; i++)
            {
                var portalcrystal = new PortalCrystal();
                PortalCrystals.Add(portalcrystal);
                portalcrystal.MoveToWorld(ValidSpawnPoints.GetRandom(), DungeonMap);
            }
        }

        public void SpawnBoss(Point3D spawnlocation)
        {
            switch (PortalType)
            {
                case PortalType.Undead:
                {
                    Boss = new LockeColePortal {Portal = this, PortalBoss = true};
                    break;
                }
                case PortalType.Beetle:
                {
                    Boss = new IrradiatedBeetlePortal {Portal = this, PortalBoss = true};
                    break;
                }
                case PortalType.Demon:
                {
                    Boss = new DarkFatherPortal {Portal = this, PortalBoss = true};
                    break;
                }
                case PortalType.Wyrm:
                {
                    Boss = new Bahamut {Portal = this, PortalBoss = true};
                    break;
                }
                case PortalType.Lummox:
                {
                    Boss = new LummoxWarHeroPortal {Portal = this, PortalBoss = true};
                    break;
                }
                case PortalType.Minotaur:
                {
                    Boss = new MinotaurWarHeroPortal {Portal = this, PortalBoss = true};
                    break;
                }
            }

            foreach (var kvp in ParticipantsScores)
            {
                double score = kvp.Value;
                Boss.AwardScorePoints(kvp.Key, ref score);
            }

            BaseSpecialEffect e = SpecialFX.FirePentagram.CreateInstance(
                spawnlocation, DungeonMap, 10, 0, TimeSpan.FromMilliseconds(1000 - ((10 - 1) * 100)));
            e.Send();
            Boss.MoveToWorld(spawnlocation, DungeonMap);
        }

        public void RemoveMobs()
        {
            foreach (Mobile target in Mobs.ToArray())
            {
                target.Delete();
            }
            if (Boss != null)
            {
                Boss.Delete();
            }
            if (Anim1 != null)
            {
                Anim1.Delete();
            }
            if (Anim2 != null)
            {
                Anim2.Delete();
            }
            Mobs.Clear();
            Mobs.TrimExcess();
        }

        public void RemoveCrystals()
        {
            foreach (PortalCrystal target in PortalCrystals.ToArray())
            {
                target.Delete();
            }
            PortalCrystals.Clear();
            PortalCrystals.TrimExcess();
        }

        public void AddScore(Mobile damager, int amount)
        {
            Mobile creditMob = null;
            if (damager is BaseCreature)
            {
                var bc = (BaseCreature) damager;

                if (bc.ControlMaster is PlayerMobile)
                {
                    creditMob = bc.ControlMaster;
                }
                else if (bc.SummonMaster is PlayerMobile)
                {
                    creditMob = bc.SummonMaster;
                }
                else if (bc.BardMaster is PlayerMobile)
                {
                    creditMob = bc.BardMaster;
                }
            }
            else if (damager is PlayerMobile)
            {
                creditMob = damager;
            }

            if (creditMob != null)
            {
                PlayerPortalProfile profile = Portals.EnsureProfile(creditMob as PlayerMobile);
                profile.AddScore(amount, this);
                if (ParticipantsScores.ContainsKey(creditMob as PlayerMobile))
                {
                    ParticipantsScores[creditMob as PlayerMobile] += amount;
                }
                else
                {
                    ParticipantsScores.Add(creditMob as PlayerMobile, amount);
                }
            }
        }

        public List<Point3D> GenerateSpawnLocations(Region r)
        {
            var locations = new List<Point3D>();

            foreach (Rectangle3D area in r.Area)
            {
                int x = area.Start.X;
                int y = area.Start.Y;
                int z;

                while (y != area.End.Y)
                {
                    while (x != area.End.X)
                    {
                        z = DungeonMap.GetAverageZ(x, y);
                        if (DungeonMap.CanSpawnMobile(new Point3D(x, y, z)))
                        {
                            locations.Add(new Point3D(x, y, z));
                        }
                        x++;
                    }
                    x = area.Start.X;
                    y++;
                }
            }
            return locations;
        }

        public void HandleMobDeath(BaseCreature mob)
        {
            if (mob.PortalBoss)
            {
                PortalCompleted = true;
                string msg =
                    ("The master of the portal has successfully been slain!  The portal will collapse in 2 minutes.");
                Message(msg);
                GenerateScoreboards();
                Timer.DelayCall(TimeSpan.FromMinutes(2), BossDeathCleanup);
            }
            else
            {
                RemoveMob(mob);
            }
        }

        public void RemoveMob(Mobile mob)
        {
            Mobs.Remove(mob);
        }

        public void UndeadPortal()
        {
            for (int i = 0; i < 10; i++)
            {
                Mobs.Add(new DreamWraithPortal {Portal = this});
            }
            for (int i = 0; i < 10; i++)
            {
                Mobs.Add(new UndeadWarDogPortal {Portal = this});
            }
            for (int i = 0; i < 2; i++)
            {
                Mobs.Add(new HarrowerTentaclesPortal {Portal = this});
            }
        }

        public void LummoxPortal()
        {
            for (int i = 0; i < 10; i++)
            {
                Mobs.Add(new LummoxWarriorPortal {Portal = this});
            }
            for (int i = 0; i < 10; i++)
            {
                Mobs.Add(new LummoxMagePortal {Portal = this});
            }
            for (int i = 0; i < 2; i++)
            {
                Mobs.Add(new HarrowerTentaclesPortal {Portal = this});
            }
        }

        public void DemonPortal()
        {
            for (int i = 0; i < 3; i++)
            {
                Mobs.Add(new AbysmalHorrorPortal {Portal = this});
            }
            for (int i = 0; i < 6; i++)
            {
                Mobs.Add(new DevourerPortal {Portal = this});
            }
            for (int i = 0; i < 2; i++)
            {
                Mobs.Add(new HarrowerTentaclesPortal {Portal = this});
            }
        }

        public void MinotaurPortal()
        {
            for (int i = 0; i < 12; i++)
            {
                Mobs.Add(new MinotaurWarriorPortal {Portal = this});
            }
            for (int i = 0; i < 2; i++)
            {
                Mobs.Add(new HarrowerTentaclesPortal {Portal = this});
            }
        }

        public void BeetlePortal()
        {
            for (int i = 0; i < 6; i++)
            {
                Mobs.Add(new RockBeetlePortal {Portal = this});
            }
            for (int i = 0; i < 11; i++)
            {
                Mobs.Add(new DoomBeetlePortal {Portal = this});
            }
            for (int i = 0; i < 2; i++)
            {
                Mobs.Add(new HarrowerTentaclesPortal {Portal = this});
            }
        }

        public void WyrmPortal()
        {
            for (int i = 0; i < 7; i++)
            {
                Mobs.Add(new DragonPortal {Portal = this});
            }
            for (int i = 0; i < 3; i++)
            {
                Mobs.Add(new ShadowWyrmPortal {Portal = this});
            }
            for (int i = 0; i < 3; i++)
            {
                Mobs.Add(new AncientWyrm {Portal = this});
            }
            for (int i = 0; i < 2; i++)
            {
                Mobs.Add(new HarrowerTentaclesPortal {Portal = this});
            }
        }

        public void Eject()
        {
            Region r = Region.Regions.Find(x => x.Name == RegionName);

            if (r != null)
            {
                List<Mobile> toEject = r.GetMobiles();

                if (toEject != null)
                {
                    Mobile master;
                    foreach (
                        Mobile mobile in
                            toEject.Where(
                                mobile =>
                                    mobile is PlayerMobile || mobile.IsControlled(out master) && master is PlayerMobile)
                        )
                    {
                        mobile.MoveToWorld(PortalEntrance, Map.Felucca);
                        mobile.SendMessage(54, "A powerful force ejects you from the portal's plane of existence.");
                    }
                }
            }
        }

        public void GetPortalLocation()
        {
            if (PortalType == PortalType.Undead)
            {
                RegionName = "Undead Portal";

                DungeonEntrance = new Point3D(1788, 68, -26);

                if (Region.Regions.Find(x => x.Name == RegionName) == null)
                {
                    return;
                }
                DungeonMap = Region.Regions.Find(x => x.Name == RegionName).Map;
            }
            else if (PortalType == PortalType.Demon)
            {
                RegionName = "Demon Portal";

                DungeonEntrance = new Point3D(6539, 138, -15);

                if (Region.Regions.Find(x => x.Name == RegionName) == null)
                {
                    return;
                }
                DungeonMap = Region.Regions.Find(x => x.Name == RegionName).Map;
            }
            else if (PortalType == PortalType.Lummox)
            {
                RegionName = "Lummox Portal";

                DungeonEntrance = new Point3D(87, 1955, 0);

                if (Region.Regions.Find(x => x.Name == RegionName) == null)
                {
                    return;
                }
                DungeonMap = Region.Regions.Find(x => x.Name == RegionName).Map;
            }
            else if (PortalType == PortalType.Minotaur)
            {
                RegionName = "Minotaur Portal";

                DungeonEntrance = new Point3D(342, 613, 27);

                if (Region.Regions.Find(x => x.Name == RegionName) == null)
                {
                    return;
                }
                DungeonMap = Region.Regions.Find(x => x.Name == RegionName).Map;
            }
            else if (PortalType == PortalType.Beetle)
            {
                RegionName = "Beetle Portal";

                DungeonEntrance = new Point3D(6489, 849, 40);

                if (Region.Regions.Find(x => x.Name == RegionName) == null)
                {
                    return;
                }
                DungeonMap = Region.Regions.Find(x => x.Name == RegionName).Map;
            }
            else if (PortalType == PortalType.Wyrm)
            {
                RegionName = "Wyrm Portal";

                DungeonEntrance = new Point3D(85, 744, -28);

                if (Region.Regions.Find(x => x.Name == RegionName) == null)
                {
                    return;
                }
                DungeonMap = Region.Regions.Find(x => x.Name == RegionName).Map;
            }
        }

        public bool SpawnPortal()
        {
            bool success = false;
            bool CanFit = false;
            Map map = Map.Felucca;
            int count = 0;

            if (PortalItem != null)
            {
                return true;
            }
            while (!CanFit)
            {
                int x = Utility.Random(0, 5072);
                int y = Utility.Random(0, 4072);
                int z = map.GetAverageZ(x, y);
                CanFit = map.CanSpawnMobile(new Point3D(x, y, z));
                Region region = Region.Find(new Point3D(x, y, z), map);

                if (CanFit && !(region is HouseRegion || region is GuardedRegion))
                {
                    PortalEntrance = new Point3D(x, y, z);
                    PortalItem = new PortalItem(DungeonEntrance, DungeonMap) {_PortalSerial = UID};
                    PortalItem.MoveToWorld(PortalEntrance, map);

                    PortalItem2 = new PortalItem(PortalEntrance, map);
                    PortalItem2.MoveToWorld(DungeonEntrance, DungeonMap);

                    Anim1 = new FireElemental
                    {
                        CantWalk = true,
                        Blessed = true,
                        Direction = Direction.Mask,
                        IgnoreMobiles = true,
                        Name = ""
                    };
                    Anim2 = new FireElemental
                    {
                        CantWalk = true,
                        Blessed = true,
                        Direction = Direction.Mask,
                        IgnoreMobiles = true,
                        Name = "a mysterious gate"
                    };

                    Anim1.MoveToWorld(PortalEntrance, map);
                    Anim2.MoveToWorld(PortalEntrance, map);
                    success = true;
                }
                else
                {
                    CanFit = false;
                }
                count++;
                if (count >= 10000)
                {
                    break;
                }
            }

            return success;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(0);

            UID.Serialize(writer);

            switch (version)
            {
                case 0:
                {
                    writer.Write(PortalItem);
                    writer.Write(PortalItem2);
                    writer.Write(Boss);
                    writer.Write((int) PortalType);
                    writer.Write(PortalEntrance);
                    writer.Write(RegionName);
                    writer.Write(DungeonEntrance);
                    writer.Write(DungeonMap);
                    writer.Write(DateStarted);
                    writer.Write((int) Status);
                    writer.Write(PortalCompleted);
                    writer.Write(BossSpawned);
                    writer.Write(_CoreTicks);
                    writer.Write(Anim1);
                    writer.Write(Anim2);

                    writer.Write(Mobs.Count);

                    if (Mobs.Count > 0)
                    {
                        foreach (Mobile mob in Mobs)
                        {
                            writer.Write(mob);
                        }
                    }

                    writer.Write(PortalCrystals.Count);

                    if (PortalCrystals.Count > 0)
                    {
                        foreach (PortalCrystal crystal in PortalCrystals)
                        {
                            writer.Write(crystal);
                        }
                    }

                    writer.Write(ParticipantsScores.Count);

                    if (ParticipantsScores.Count > 0)
                    {
                        foreach (var score in ParticipantsScores)
                        {
                            writer.Write(score.Key);
                            writer.Write(score.Value);
                        }
                    }
                }
                    break;
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            Mobs = new List<Mobile>();
            PortalCrystals = new List<PortalCrystal>();
            ParticipantsScores = new Dictionary<PlayerMobile, int>();
            CurrentParticipants = new List<Mobile>();
            ValidSpawnPoints = new List<Point3D>();

            base.Deserialize(reader);

            int version = reader.ReadInt();

            UID = new PortalSerial(reader);

            switch (version)
            {
                case 0:
                {
                    PortalItem = reader.ReadItem<PortalItem>();
                    PortalItem2 = reader.ReadItem<PortalItem>();
                    Boss = reader.ReadMobile<BaseCreature>();
                    PortalType = (PortalType) reader.ReadInt();
                    PortalEntrance = reader.ReadPoint3D();
                    RegionName = reader.ReadString();
                    DungeonEntrance = reader.ReadPoint3D();
                    DungeonMap = reader.ReadMap();
                    DateStarted = reader.ReadDateTime();
                    Status = (PortalStatus) reader.ReadInt();
                    PortalCompleted = reader.ReadBool();
                    BossSpawned = reader.ReadBool();
                    _CoreTicks = reader.ReadInt();
                    Anim1 = reader.ReadMobile<FireElemental>();
                    Anim2 = reader.ReadMobile<FireElemental>();

                    int count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            Mobile mob = reader.ReadMobile();
                            if (!mob.Deleted)
                            {
                                ((BaseCreature) mob).Portal = this;
                                Mobs.Add(mob);
                            }
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var crystal = reader.ReadItem<PortalCrystal>();
                            if (!crystal.Deleted)
                            {
                                PortalCrystals.Add(crystal);
                            }
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var player = reader.ReadMobile<PlayerMobile>();
                            int score = reader.ReadInt();
                            ParticipantsScores.Add(player, score);
                        }
                    }
                }
                    break;
            }
        }

        public void FlameSpirtal(Point3D location)
        {
            var queue = new EffectQueue
            {
                Deferred = false
            };

            var points = new List<Point3D>();
            double d;
            double r = 1;
            int newx;
            int newy;
            points.Add(location);
            //calculate spiral vector
            for (d = 0; d < 4 * Math.PI; d += 0.01)
            {
                newx = (int) Math.Floor(location.X + (Math.Sin(d) * d) * r);
                newy = (int) Math.Floor(location.Y + (Math.Sin(d + (Math.PI / 2)) * (d + (Math.PI / 2))) * r);
                var to = new Point3D(newx, newy, location.Z);
                if (!points.Contains(to))
                {
                    points.Add(to);
                }
            }
            int n = 0;
            //Build the queue based on the points in the line.
            foreach (Point3D p in points)
            {
                n += 20;
                queue.Add(
                    new EffectInfo(p, DungeonMap, 14089, 0, 10, 30, EffectRender.Normal, TimeSpan.FromMilliseconds(n),
                        () => { }));
            }
            n += 400; //used to offset when the spiral reverses so it doesn't overlap
            foreach (Point3D p in points.AsEnumerable().Reverse())
            {
                n += 20;
                queue.Add(
                    new EffectInfo(p, DungeonMap, 14089, 0, 10, 30, EffectRender.Normal, TimeSpan.FromMilliseconds(n),
                        () => { }));
            }
            queue.Process();
        }

        public void Message(string msg)
        {
            Region r = Region.Regions.Find(x => x.Name == RegionName);

            if (r != null)
            {
                List<Mobile> toBroadcast = r.GetMobiles();
                foreach (Mobile mobile in toBroadcast)
                {
                    var player = mobile as PlayerMobile;
                    if (player != null)
                    {
                        Notify.Send(player, msg, true, 0, 10);
                    }
                }
            }
        }

        public void GenerateAnims()
        {
            if (Anim1 != null && Anim2 != null)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(1), OneSecondDelay);
                Timer.DelayCall(TimeSpan.FromSeconds(0.25), MSDelay);
            }
        }

        public void OneSecondDelay()
        {
            if (count1 == 1)
            {
                count1 = 0;
                Anim1.Animate(12, 8, 1, false, false, 0);
            }
            else
            {
                count1 = 1;
                Anim1.Animate(12, 10, 0, true, false, 0);
            }
            Timer.DelayCall(TimeSpan.FromSeconds(1), OneSecondDelay);
        }

        public void MSDelay()
        {
            count3 += 1;
            if (count3 > 1)
            {
                if (count2 == 1)
                {
                    count2 = 0;
                    Anim2.Animate(3, 0, 1, true, false, 1);
                }
                else
                {
                    count2 = 1;
                    Anim2.Animate(3, 4, 1, false, false, 1);
                }
                count3 = 0;
            }
            Timer.DelayCall(TimeSpan.FromSeconds(0.25), MSDelay);
        }

        #region Rewards

        public void GiveRewards()
        {
            var scores = new Dictionary<PlayerMobile, int>(ParticipantsScores);

            // first find all eligible receivers
            var eligibleMobs = new List<Mobile>();
            var eligibleMobScores = new List<double>();

            double totalScores = 0.0;

            foreach (var pair in scores)
            {
                Mobile mob = pair.Key;

                if (mob == null)
                {
                    continue;
                }

                eligibleMobs.Add(mob);
                eligibleMobScores.Add(pair.Value);

                totalScores += pair.Value;
            }

            if (0.1 > Utility.RandomDouble())
            {
                GiveRelic(eligibleMobs, eligibleMobScores, totalScores);
            }
        }

        public void GiveRelic(List<Mobile> eligibleMobs, List<double> eligibleMobScores, double totalScores)
        {
            double currentTestValue = 0.0;
            double roll = Utility.RandomDouble() * totalScores;

            for (int i = 0; i < eligibleMobScores.Count; i++)
            {
                currentTestValue += eligibleMobScores[i];

                if (roll > currentTestValue)
                {
                    continue;
                }

                BaseMetaRelic relic = BaseMetaRelic.GetRandomRelic();
                if (eligibleMobs[i] is PlayerMobile && eligibleMobs[i].Backpack != null)
                {
                    eligibleMobs[i].Backpack.DropItem(relic);
                    eligibleMobs[i].SendMessage(54, "You have received a relic!");
                    return;
                }
            }
        }

        #endregion
    }
}