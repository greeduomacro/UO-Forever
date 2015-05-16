#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Commands;
using Server.Engines.EventScores;
using Server.Items;
using Server.Mobiles;
using VitaNex;
using VitaNex.Notify;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Engines.EventInvasions
{
    public sealed class Invasion : PropertyObject
    {
        [CommandProperty(EventInvasions.Access, true)]
        public InvasionSerial UID { get; private set; }

        [CommandProperty(EventInvasions.Access, true)]
        public string InvasionName { get; set; }

        [CommandProperty(EventInvasions.Access, true)]
        public string RegionName { get; set; }

        [CommandProperty(EventInvasions.Access, true)]
        public List<Mobile> Invaders { get; set; }

        [CommandProperty(EventInvasions.Access, true)]
        public DateTime DateStarted { get; set; }

        [CommandProperty(EventInvasions.Access, true)]
        public InvasionStatus Status { get; set; }

        [CommandProperty(EventInvasions.Access, true)]
        public Dictionary<PlayerMobile, int> ParticipantsScores { get; set; }

        [CommandProperty(EventInvasions.Access, true)]
        public List<Level> Levels { get; set; }

        [CommandProperty(EventInvasions.Access, true)]
        public Level CurrentLevel { get; set; }

        [CommandProperty(EventInvasions.Access, true)]
        public List<Point3D> ValidSpawnPoints { get; set; }

        [CommandProperty(EventInvasions.Access, true)]
        public int CurrentLevelKills { get; set; }

        [CommandProperty(EventInvasions.Access, true)]
        public bool SpawnTownGates { get; set; }

        public List<Moongate> TownGates { get; set; }

        private int _CoreTicks;
        private PollTimer _CoreTimer;

        public Invasion(string invasionname, string regionname, DateTime date, List<Level> levels, bool gates)
        {
            UID = new InvasionSerial();

            InvasionName = invasionname;
            RegionName = regionname;
            DateStarted = date;
            Invaders = new List<Mobile>();
            Status = InvasionStatus.Waiting;
            Levels = levels;
            CurrentLevel = Levels.First();
            ParticipantsScores = new Dictionary<PlayerMobile, int>();
            SpawnTownGates = gates;
            TownGates = new List<Moongate>();
        }

        public Invasion(Invasion invasion)
        {
            UID = new InvasionSerial();

            InvasionName = invasion.InvasionName;
            RegionName = invasion.RegionName;
            DateStarted = DateTime.UtcNow;
            Invaders = new List<Mobile>();
            Status = InvasionStatus.Waiting;
            Levels = invasion.Levels;
            CurrentLevel = invasion.Levels.First();
            ParticipantsScores = new Dictionary<PlayerMobile, int>();
        }

        public void init()
        {
            if (TownGates != null)
            {
                DeleteGates();
            }

            if (SpawnTownGates)
            {
                TownGates = SpawnGates();
            }

            _CoreTimer = PollTimer.FromSeconds(1.0, OnTick, () => Status == InvasionStatus.Running);
        }

        public void OnTick()
        {
            if (_CoreTicks == 0)
            {
                SpawnInvaders(CurrentLevel.SpawnAmount);
            }

            if (_CoreTicks > 60 && _CoreTicks % 30 == 0)
            {
                if (Invaders.Count != CurrentLevel.SpawnAmount)
                {
                    SpawnInvaders(CurrentLevel.SpawnAmount - Invaders.Count);
                }
            }

            if (_CoreTicks % 600 == 0)
            {
                CommandHandlers.BroadcastMessage(AccessLevel.Player, 33,
                    "Defenders are needed to halt the " + InvasionName + ".");
            }

            if (_CoreTicks >= (int) CurrentLevel.TimeLimit.TotalSeconds && CurrentLevel.KillAmount == 0 ||
                (int) CurrentLevel.TimeLimit.TotalSeconds == 0 && CurrentLevel.KillAmount > 0 &&
                CurrentLevelKills >= CurrentLevel.KillAmount
                ||
                _CoreTicks < (int) CurrentLevel.TimeLimit.TotalSeconds && CurrentLevel.KillAmount > 0 &&
                CurrentLevelKills >= CurrentLevel.KillAmount)
            {
                if (Levels.Last() == CurrentLevel || Levels.Count == 1)
                {
                    FinishInvasion();
                    _CoreTimer.Stop();
                    return;
                }
                else
                {
                    IncrementLevel();
                    return;
                }
            }

            if (CurrentLevel != null && (int) CurrentLevel.TimeLimit.TotalSeconds > 0 &&
                _CoreTicks >= (int) CurrentLevel.TimeLimit.TotalSeconds && CurrentLevel.KillAmount > 0 &&
                CurrentLevelKills < CurrentLevel.KillAmount)
            {
                FinishInvasion();
                _CoreTimer.Stop();
                return;
            }

            ++_CoreTicks;
        }

        public Invasion(GenericReader reader)
            : base(reader)
        {}

        public override void Reset()
        {}

        public override void Clear()
        {}

        public string GetTimeLeft()
        {
            double timeleft = CurrentLevel.TimeLimit.TotalSeconds - _CoreTicks;
            TimeSpan t = TimeSpan.FromSeconds(timeleft);
            return string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                t.Hours,
                t.Minutes,
                t.Seconds);
        }

        public void StartInvasion()
        {
            Status = InvasionStatus.Running;
            CurrentLevel = Levels.First();
            init();
        }

        public void RestartInvasion()
        {
            CurrentLevel = Levels.First();
            Invaders.Clear();
            Invaders.TrimExcess();
            CurrentLevelKills = 0;
            _CoreTicks = 0;
            DateStarted = DateTime.UtcNow;
            ParticipantsScores.Clear();
            SpawnInvaders(CurrentLevel.SpawnAmount);
            Status = InvasionStatus.Running;
            Notify.Broadcast<HydraMotMNotifyGump>(
                "The " + InvasionName + " has begun!",
                true,
                1.0,
                10.0);

            init();
        }

        public void FinishInvasion()
        {
            Status = InvasionStatus.Finished;
            _CoreTimer.Stop();
            RemoveInvaders();
            if (ValidSpawnPoints != null)
            {
                ValidSpawnPoints.Clear();
                ValidSpawnPoints.TrimExcess();
            }
            if (TownGates != null)
            {
                DeleteGates();
            }
            Notify.Broadcast<HydraMotMNotifyGump>(
                "The " + InvasionName + " has ended!",
                true,
                1.0,
                10.0);
            GenerateScoreboards();
            CurrentLevel = Levels.First();
        }

        public List<Moongate> SpawnGates()
        {
            var gates = new List<Moongate>();

            if (ValidSpawnPoints == null || ValidSpawnPoints.Count == 0)
            {
                ValidSpawnPoints = GenerateSpawnLocations();
            }
            gates.Add(new Moongate(ValidSpawnPoints.GetRandom(), Map.Felucca)
            {
                Dispellable = false,
                DoesNotDecay = true,
                Name = "Portal to the " + InvasionName,
                Hue = 1172,
                Location = new Point3D(1427, 1701, 0),
                Map = Map.Felucca
            });
            gates.Add(new Moongate(ValidSpawnPoints.GetRandom(), Map.Felucca)
            {
                Dispellable = false,
                DoesNotDecay = true,
                Name = "Portal to the " + InvasionName,
                Hue = 1172,
                Location = new Point3D(2512, 564, 0),
                Map = Map.Felucca
            });
            gates.Add(new Moongate(ValidSpawnPoints.GetRandom(), Map.Felucca)
            {
                Dispellable = false,
                DoesNotDecay = true,
                Name = "Portal to the " + InvasionName,
                Hue = 1172,
                Location = new Point3D(2717, 2173, 0),
                Map = Map.Felucca
            });
            gates.Add(new Moongate(ValidSpawnPoints.GetRandom(), Map.Felucca)
            {
                Dispellable = false,
                DoesNotDecay = true,
                Name = "Portal to the " + InvasionName,
                Hue = 1172,
                Location = new Point3D(2237, 1195, 0),
                Map = Map.Felucca
            });
            gates.Add(new Moongate(ValidSpawnPoints.GetRandom(), Map.Felucca)
            {
                Dispellable = false,
                DoesNotDecay = true,
                Name = "Portal to the " + InvasionName,
                Hue = 1172,
                Location = new Point3D(4453, 1166, 0),
                Map = Map.Felucca
            });
            gates.Add(new Moongate(ValidSpawnPoints.GetRandom(), Map.Felucca)
            {
                Dispellable = false,
                DoesNotDecay = true,
                Name = "Portal to the " + InvasionName,
                Hue = 1172,
                Location = new Point3D(1821, 2819, 0),
                Map = Map.Felucca
            });
            gates.Add(new Moongate(ValidSpawnPoints.GetRandom(), Map.Felucca)
            {
                Dispellable = false,
                DoesNotDecay = true,
                Name = "Portal to the " + InvasionName,
                Hue = 1172,
                Location = new Point3D(2895, 689, 0),
                Map = Map.Felucca
            });
            return gates;
        }

        public void DeleteGates()
        {
            if (TownGates != null)
            {
                Moongate[] list = TownGates.ToArray();
                foreach (Moongate moongate in list)
                {
                    moongate.Delete();
                }
                TownGates.Clear();
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
                        var scrollgump = new InvasionScoresScrollGump(mobile1, onAccept: x =>
                        {
                            var scoreboard = new InvasionScoresOverviewGump(mobile1, this).
                                Send<InvasionScoresOverviewGump>();
                        }).Send<InvasionScoresScrollGump>();
                    });

               // PlayerEventScoreProfile eventprofile = EventScores.EventScores.EnsureProfile(mobile);
                //eventprofile.AddInvasion(this, mobile);
            }
        }

        public void IncrementLevel()
        {
            int index = Levels.FindIndex(l => l.UID == CurrentLevel.UID);
            index++;
            if (Levels.Count - 1 >= index)
            {
                RemoveInvaders();
                CurrentLevelKills = 0;
                CurrentLevel = Levels[index];
                _CoreTicks = 0;
            }
            else
            {
                FinishInvasion();
                _CoreTimer.Stop();
            }
        }

        public void ForceIncrementLevel()
        {
            int index = Levels.FindIndex(l => l.UID == CurrentLevel.UID);
            index++;
            if (Levels.Count - 1 >= index)
            {
                CurrentLevel = Levels[index];
                RemoveInvaders();
                CurrentLevelKills = 0;
                _CoreTicks = 0;
            }
            else
            {
                FinishInvasion();
                _CoreTimer.Stop();
            }
        }

        /*public void DeleteInvasion()
        {
            if (Status == InvasionStatus.Running)
            {
                Status = InvasionStatus.Finished;
                _CoreTimer.Stop();
            }

            Levels.Clear();
            RemoveInvaders();
            if (ValidSpawnPoints != null)
            {
                ValidSpawnPoints.Clear();
            }

            foreach (PlayerInvasionProfile profile in EventInvasions.PlayerProfiles.Values)
            {
                if (profile.SpecificInvasionScores.ContainsKey(UID))
                {
                    profile.OverallScore -= profile.SpecificInvasionScores[UID];
                    profile.SpecificInvasionScores.Remove(UID);
                }
            }

            EventInvasions.Invasions.Remove(UID);

        }*/

        public void SpawnInvaders(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                if (Invaders.Count >= CurrentLevel.SpawnAmount)
                {
                    break;
                }
                Type selected = CurrentLevel.Creatures.GetRandom();
                if (selected.IsEqualOrChildOf<Mobile>())
                {
                    var mob = selected.CreateInstanceSafe<BaseCreature>();
                    if (mob != null)
                    {
                        if (!String.IsNullOrWhiteSpace(CurrentLevel.InvaderTitles))
                        {
                            mob.SpecialTitle = CurrentLevel.InvaderTitles;
                        }
                        else
                        {
                            mob.SpecialTitle = "Invader";
                        }
                        mob.TitleHue = 1174;
                        mob.GuardImmune = true;
                        mob.Invasion = this;

                        if (Utility.RandomDouble() <= 0.01)
                        {
                            mob.IsParagon = true;
                        }
                        if (CurrentLevel.Plat > 0 && mob.Backpack != null)
                        {
                            mob.AddToBackpack(new Platinum(CurrentLevel.Plat));
                        }
                        Invaders.Add(mob);
                    }
                }
            }
            if (ValidSpawnPoints == null || ValidSpawnPoints.Count == 0)
            {
                ValidSpawnPoints = GenerateSpawnLocations();
            }
            foreach (Mobile invader in Invaders.Where(invader => invader.Map == Map.Internal))
            {
                invader.MoveToWorld(ValidSpawnPoints.GetRandom(), Map.Felucca);
            }
        }

        public void RestartLevel()
        {
            RemoveInvaders();
            Invaders.Clear();
            Invaders.TrimExcess();
            CurrentLevelKills = 0;
            _CoreTicks = 0;
        }

        public void ForceSpawnInvaders()
        {
            if (Invaders.Count != CurrentLevel.SpawnAmount)
            {
                SpawnInvaders(CurrentLevel.SpawnAmount - Invaders.Count);
            }
        }

        public void RemoveInvaders()
        {
            foreach (Mobile target in Invaders.ToArray())
            {
                target.Delete();
            }
            Invaders.Clear();
            Invaders.TrimExcess();
        }

        public void AddScore(Mobile damager, int amount)
        {
            Mobile creditMob = null;

            var oneHandedWeapon = damager.FindItemOnLayer(Layer.OneHanded) as BaseWeapon;
            var twoHandedWeapon = damager.FindItemOnLayer(Layer.TwoHanded) as BaseWeapon;
            var equipRanged = twoHandedWeapon as BaseRanged;

            if (damager is BaseCreature)
            {
                var bc = (BaseCreature) damager;

                if (bc.ControlMaster is PlayerMobile)
                {
                    creditMob = bc.ControlMaster;
                    amount = (int) (Math.Ceiling(amount * EventInvasions.CSOptions.TamerMod));
                }
                else if (bc.SummonMaster is PlayerMobile)
                {
                    creditMob = bc.SummonMaster;
                    amount = (int) (Math.Ceiling(amount * EventInvasions.CSOptions.SummonMod));
                }
                else if (bc.BardMaster is PlayerMobile)
                {
                    creditMob = bc.BardMaster;
                    amount = (int) (Math.Ceiling(amount * EventInvasions.CSOptions.BardMod));
                }
            }
            else if (damager is PlayerMobile)
            {
                creditMob = damager;
                if (equipRanged != null)
                {
                    if (twoHandedWeapon.Slayer != SlayerName.None)
                    {
                        amount = (int) (Math.Ceiling(amount * 0.5));
                    }

                    amount = (int) (Math.Ceiling(amount * EventInvasions.CSOptions.ArcherMod));
                }
                else if (oneHandedWeapon != null || twoHandedWeapon != null)
                {
                    if (oneHandedWeapon != null && oneHandedWeapon.Slayer != SlayerName.None ||
                        twoHandedWeapon != null && twoHandedWeapon.Slayer != SlayerName.None)
                    {
                        amount = (int) (Math.Ceiling(amount * 0.5));
                    }

                    amount = (int) (Math.Ceiling(amount * EventInvasions.CSOptions.MeleeMod));
                }
            }

            if (creditMob != null)
            {
                PlayerInvasionProfile profile = EventInvasions.EnsureProfile(creditMob as PlayerMobile);
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

        public void HandleInvaderDeath(BaseCreature invader)
        {
            if (CurrentLevel.RewardItems != null && CurrentLevel.RewardItems.Count > 0)
            {
                Type selected = CurrentLevel.RewardItems.GetRandom();
                if (selected.IsEqualOrChildOf<Item>())
                {
                    var item = selected.CreateInstanceSafe<Item>();

                    if (item != null && Utility.RandomDouble() <= (CurrentLevel.DropChance/100))
                    {
                        invader.PackItem(item);
                    }
                }
            }
            CurrentLevelKills++;
            RemoveInvader(invader);
        }

        public List<Point3D> GenerateSpawnLocations()
        {
            Map map = Map.Felucca;
            Region r = Region.Regions.Find(x => x.Name == RegionName);

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
                        z = map.GetAverageZ(x, y);
                        if (map.CanSpawnMobile(new Point3D(x, y, z)))
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

        public void RemoveInvader(Mobile invader)
        {
            Invaders.Remove(invader);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(1);

            UID.Serialize(writer);

            switch (version)
            {
                case 1:
                {
                    writer.Write(SpawnTownGates);
                    writer.Write(TownGates.Count);
                    if (TownGates.Count > 0)
                    {
                        foreach (Moongate gate in TownGates)
                        {
                            writer.Write(gate);
                        }
                    }

                    goto case 0;
                }
                case 0:
                {
                    writer.Write(InvasionName);
                    writer.Write(RegionName);
                    writer.Write(DateStarted);
                    writer.Write((int) Status);
                    writer.Write(_CoreTicks);
                    writer.Write(CurrentLevelKills);

                    writer.Write(Invaders.Count);

                    if (Invaders.Count > 0)
                    {
                        foreach (Mobile invader in Invaders)
                        {
                            writer.Write(invader);
                        }
                    }

                    writer.Write(Levels.Count);

                    if (Levels.Count > 0)
                    {
                        foreach (Level level in Levels)
                        {
                            level.Serialize(writer);
                        }
                    }

                    writer.Write(ParticipantsScores.Count);

                    if (ParticipantsScores.Count > 0)
                    {
                        foreach (KeyValuePair<PlayerMobile, int> score in ParticipantsScores)
                        {
                            writer.Write(score.Key);
                            writer.Write(score.Value);
                        }
                    }

                    CurrentLevel.Serialize(writer);
                }
                    break;
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            Invaders = new List<Mobile>();
            Levels = new List<Level>();
            ParticipantsScores = new Dictionary<PlayerMobile, int>();
            TownGates = new List<Moongate>();

            base.Deserialize(reader);

            int version = reader.ReadInt();

            UID = new InvasionSerial(reader);

            switch (version)
            {
                case 1:
                {
                    SpawnTownGates = reader.ReadBool();
                    int countgates = reader.ReadInt();

                    if (countgates > 0)
                    {
                        for (int i = 0; i < countgates; i++)
                        {
                            var gate = reader.ReadItem<Moongate>();
                            if (gate != null)
                            {
                                TownGates.Add(gate);
                            }
                        }
                    }


                    goto case 0;
                }
                case 0:
                {
                    InvasionName = reader.ReadString();
                    RegionName = reader.ReadString();
                    DateStarted = reader.ReadDateTime();
                    Status = (InvasionStatus) reader.ReadInt();
                    _CoreTicks = reader.ReadInt();
                    CurrentLevelKills = reader.ReadInt();

                    int count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            Mobile invader = reader.ReadMobile();
                            if (invader != null)
                            {
                                ((BaseCreature) invader).Invasion = this;
                                Invaders.Add(invader);
                            }
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var level = new Level(reader);
                            Levels.Add(level);
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var player = reader.ReadMobile<PlayerMobile>();
                            int score = reader.ReadInt();
                            if (player != null)
                            {
                                ParticipantsScores.Add(player, score);
                            }
                        }
                    }

                    CurrentLevel = new Level(reader);
                }
                    break;
            }
        }
    }
}