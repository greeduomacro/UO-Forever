#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Games;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using VitaNex;
using VitaNex.Notify;

#endregion

namespace Server.Engines.ZombieEvent
{
    public sealed class ZombieInstance : PropertyObject
    {
        [CommandProperty(ZombieEvent.Access, true)]
        public ZombieInstanceSerial Uid { get; private set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public string ZombieEventName { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public ZombieEventStatus Status { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public List<Mobile> Daemons { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public List<Mobile> FeyWarriors { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public List<Mobile> TreeFellows { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public List<Mobile> Birds { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public List<Mobile> ZombieSpiders { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public List<Mobile> HorrifyingTentacles { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public List<Mobile> Vitriols { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public List<Mobile> GoreFiends { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public List<PlayerMobile> CureCompleters { get; set; }

        [CommandProperty(ZombieEvent.Access, true)]
        public PlayerMobile CureWinner { get; set; }

        public ZombieInstance()
        {
            Uid = new ZombieInstanceSerial();
            Daemons = new List<Mobile>();
            FeyWarriors = new List<Mobile>();
            TreeFellows = new List<Mobile>();
            Birds = new List<Mobile>();
            ZombieSpiders = new List<Mobile>();
            HorrifyingTentacles = new List<Mobile>();
            Vitriols = new List<Mobile>();
            GoreFiends = new List<Mobile>();
            Status = ZombieEventStatus.Running;

            CureCompleters = new List<PlayerMobile>();

            _ZombiesLoc1 = new List<Mobile>();
            _ZombiesLoc2 = new List<Mobile>();
            _ZombiesLoc3 = new List<Mobile>();
            _ZombiesLoc4 = new List<Mobile>();
        }

        public ZombieInstance(ZombieInstanceSerial serial)
        {
            Uid = serial;
            Daemons = new List<Mobile>();
            FeyWarriors = new List<Mobile>();
            TreeFellows = new List<Mobile>();
            Birds = new List<Mobile>();
            ZombieSpiders = new List<Mobile>();
            HorrifyingTentacles = new List<Mobile>();
            Vitriols = new List<Mobile>();
            GoreFiends = new List<Mobile>();
            Status = ZombieEventStatus.Running;

            CureCompleters = new List<PlayerMobile>();

            _ZombiesLoc1 = new List<Mobile>();
            _ZombiesLoc2 = new List<Mobile>();
            _ZombiesLoc3 = new List<Mobile>();
            _ZombiesLoc4 = new List<Mobile>();
        }

        public ZombieInstance(GenericReader reader)
            : base(reader)
        {}

        private readonly Rectangle2D[] _MainSpawnRectangle =
        {
            new Rectangle2D(new Point2D(5121, 3083), new Point2D(5828, 4094)),
            new Rectangle2D(new Point2D(5809, 3204), new Point2D(6141, 4095)),
            new Rectangle2D(new Point2D(5400, 2617), new Point2D(5802, 3130)),
            new Rectangle2D(new Point2D(5286, 2480), new Point2D(5824, 2616))
        };

        private readonly Rectangle2D _SpiderSpawnRectangle = new Rectangle2D(new Point2D(5424, 3113),
            new Point2D(5478, 3141));

        private readonly Rectangle2D _TentacleSpawnRectangle = new Rectangle2D(new Point2D(5912, 3271),
            new Point2D(6078, 3546));

        private readonly Point3D[] _TentaclePoints =
        {
            new Point3D(5780, 2865, 0), new Point3D(5689, 2934, 0), new Point3D(5723, 3014, 0),
            new Point3D(5797, 2971, 0), new Point3D(5704, 3124, -15), new Point3D(5645, 3157, -15),
            new Point3D(5667, 3266, -15), new Point3D(5657, 3312, -15), new Point3D(5771, 3303, -15),
            new Point3D(5930, 3367, 0)
        };

        private List<Mobile> _ZombiesLoc1;
        private List<Mobile> _ZombiesLoc2;
        private List<Mobile> _ZombiesLoc3;
        private List<Mobile> _ZombiesLoc4;

        private int _CoreTicks;
        private PollTimer _CoreTimer;

        public override void Reset()
        {}

        public override void Clear()
        {}

        public void init()
        {
            _CoreTimer = PollTimer.FromSeconds(1.0, OnTick, () => Status == ZombieEventStatus.Running);
        }

        public void Stop()
        {
            Status = ZombieEventStatus.Finished;
            CleanUp();
            ZombieEvent.CleanUpAvatars();
        }

        public void Pause()
        {
            Status = ZombieEventStatus.Paused;
            CleanUp();
        }

        public void Unpause()
        {
            Status = ZombieEventStatus.Running;
            init();
        }

        public void JoinZombieInstance(PlayerMobile player)
        {
            PlayerZombieProfile profile = ZombieEvent.EnsureProfile(player);
            ZombieAvatar avatar = profile.ZombieAvatar;
            profile.Active = true;
            if (avatar == null)
            {
                avatar = profile.CreateAvatar();
            }

            if (profile.ZombieSavePoint == Point3D.Zero && avatar != null &&
                DateTime.UtcNow > (profile.DisconnectTime + TimeSpan.FromMinutes(2)))
            {
                if (avatar.Backpack != null && avatar.Items.Count <= 1)
                {
                    avatar.EquipItem(new Dagger {Speed = 1});
                    avatar.Backpack.DropItem(new Bandage(7));
                    ZombieEvent.RandomClothing(avatar);
                }
                avatar.MoveToWorld(GetRandomLocation(), Map.ZombieLand);
            }

            CreaturePossession.ForcePossessCreature(null, player, avatar);

            if (avatar != null)
            {
                avatar.Blessed = false;
                avatar.Hidden = false;
                avatar.IgnoreMobiles = false;
                avatar.CantWalk = false;
            }

            player.LogoutLocation = player.Location;
            player.Map = Map.Internal;
            player.LogoutMap = Map.Felucca;
        }

        public void LeaveZombieInstance(PlayerZombieProfile profile)
        {
            if (profile.LeaveEventTimer == null || !profile.LeaveEventTimer.Running)
            {
                profile.ZombieAvatar.SendMessage(54,
                    "You will log out in one minute.  Your location and items wil be saved upon a successful logout.");
                profile.LeaveEventTimer = Timer.DelayCall(TimeSpan.FromMinutes(1), () =>
                {
                    profile.Active = false;
                    profile.ZombieSavePoint = profile.ZombieAvatar.Location;
                    profile.ZombieAvatar.SendMessage(54,
                        "You have successfully logged out of the Zombie Event.  Your location and items have been saved.  You will now be disconnected.");
                    profile.ZombieAvatar.Blessed = true;
                    profile.ZombieAvatar.Hidden = true;
                    profile.ZombieAvatar.IgnoreMobiles = true;
                    profile.ZombieAvatar.CantWalk = true;
                    if (profile.ZombieAvatar.NetState != null)
                        profile.ZombieAvatar.NetState.Dispose();
                });
            }
        }

        public bool HandleAvatarDeath(PlayerZombieProfile profile, Mobile lastkiller)
        {
            ZombieAvatar avatar = profile.ZombieAvatar;
            profile.Deaths++;

            if (lastkiller is ZombieAvatar)
            {
                var enemyavatar = lastkiller as ZombieAvatar;
                if (enemyavatar.Owner != null)
                {
                    PlayerZombieProfile enemyprofile = ZombieEvent.EnsureProfile(enemyavatar.Owner);
                    enemyprofile.Kills++;
                }
            }

            Effects.SendIndividualFlashEffect(avatar, FlashType.LightFlash);
            avatar.DropHolding();

            var corpse = CreatePlayerCorpse(avatar) as Corpse;
            if (corpse != null)
            {
                Effects.PlaySound(avatar, avatar.Map, avatar.GetDeathSound());
                ZombieEvent.ZombieDeathAnim(avatar, corpse);
                corpse.MoveToWorld(avatar.Location, Map.ZombieLand);
            }

            avatar.MoveToWorld(GetRandomLocation(), Map.ZombieLand);

            if (avatar.Backpack != null)
            {
                avatar.EquipItem(new Dagger {Speed = 1});
                avatar.Backpack.DropItem(new Bandage(7));
                ZombieEvent.RandomClothing(avatar);
            }
            avatar.Hits = avatar.HitsMax;
            avatar.Stam = avatar.StamMax;
            avatar.Poison = null;

            return false;
        }

        #region Cleanup Event

        public void CleanUp()
        {
            foreach (Mobile zombie in _ZombiesLoc1.ToList())
            {
                if (zombie != null)
                {
                    zombie.Delete();
                }
                _ZombiesLoc1.Remove(zombie);
            }
            _ZombiesLoc1.Clear();
            _ZombiesLoc1.TrimExcess();

            foreach (Mobile zombie in _ZombiesLoc2.ToList())
            {
                if (zombie != null)
                {
                    zombie.Delete();
                }
                _ZombiesLoc2.Remove(zombie);
            }
            _ZombiesLoc2.Clear();
            _ZombiesLoc2.TrimExcess();

            foreach (Mobile zombie in _ZombiesLoc3.ToList())
            {
                if (zombie != null)
                {
                    zombie.Delete();
                }
                _ZombiesLoc3.Remove(zombie);
            }
            _ZombiesLoc3.Clear();
            _ZombiesLoc3.TrimExcess();

            foreach (Mobile zombie in _ZombiesLoc4.ToList())
            {
                if (zombie != null)
                {
                    zombie.Delete();
                }
                _ZombiesLoc4.Remove(zombie);
            }
            _ZombiesLoc4.Clear();
            _ZombiesLoc4.TrimExcess();

            foreach (Mobile daemon in Daemons.ToList())
            {
                if (daemon != null)
                {
                    daemon.Delete();
                }
                Daemons.Remove(daemon);
            }
            Daemons.Clear();
            Daemons.TrimExcess();

            foreach (Mobile bird in Birds.ToList())
            {
                if (bird != null)
                {
                    bird.Delete();
                }
                Birds.Remove(bird);
            }
            Birds.Clear();
            Birds.TrimExcess();

            foreach (Mobile dk in GoreFiends.ToList())
            {
                if (dk != null)
                {
                    dk.Delete();
                }
                GoreFiends.Remove(dk);
            }
            GoreFiends.Clear();
            GoreFiends.TrimExcess();

            foreach (Mobile fey in FeyWarriors.ToList())
            {
                if (fey != null)
                {
                    fey.Delete();
                }
                FeyWarriors.Remove(fey);
            }
            FeyWarriors.Clear();
            FeyWarriors.TrimExcess();

            foreach (Mobile tentacle in HorrifyingTentacles.ToList())
            {
                if (tentacle != null)
                {
                    tentacle.Delete();
                }
                HorrifyingTentacles.Remove(tentacle);
            }
            HorrifyingTentacles.Clear();
            HorrifyingTentacles.TrimExcess();

            foreach (Mobile tf in TreeFellows.ToList())
            {
                if (tf != null)
                {
                    tf.Delete();
                }
                TreeFellows.Remove(tf);
            }
            TreeFellows.Clear();
            TreeFellows.TrimExcess();

            foreach (Mobile vitriol in Vitriols.ToList())
            {
                if (vitriol != null)
                {
                    vitriol.Delete();
                }
                Vitriols.Remove(vitriol);
            }
            Vitriols.Clear();
            Vitriols.TrimExcess();

            foreach (Mobile spider in ZombieSpiders.ToList())
            {
                if (spider != null)
                {
                    spider.Delete();
                }
                ZombieSpiders.Remove(spider);
            }

            ZombieSpiders.Clear();
            ZombieSpiders.TrimExcess();
        }



        #endregion

        public void OnTick()
        {
            if (_CoreTicks == 0)
            {
                SpawnInitialZombies();
                SpawnDelayedCorpses();
                SpawnDaemons();
                SpawnBirds();
                SpawnFey();
                SpawnSpider();
                SpawnTentacles();
                SpawnTreeFellow();
                SpawnVitriol();
                SpawnGoreFiends();
            }

            if (_CoreTicks > 30 && _CoreTicks % 30 == 0)
            {
                SpawnZombies();
                SpawnDaemons();
                SpawnBirds();
                SpawnFey();
                SpawnSpider();
                SpawnTentacles();
                SpawnTreeFellow();
                SpawnVitriol();
                SpawnGoreFiends();
            }

            if (_CoreTicks >= 600 && _CoreTicks % 600 == 0)
            {
                SpawnDelayedCorpses();
            }

            ++_CoreTicks;
        }

        public void SpawnDelayedCorpses()
        {
            for (int i = 0; i < 17; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i * 2), SpawnCorpses);
            }
        }

        public List<Point3D> GenerateSpawnLocations(Rectangle2D area)
        {
            var locations = new List<Point3D>();

            int x = area.Start.X;
            int y = area.Start.Y;

            while (y != area.End.Y)
            {
                while (x != area.End.X)
                {
                    int z = Map.ZombieLand.GetAverageZ(x, y);
                    if (Map.ZombieLand.CanSpawnMobile(new Point3D(x, y, z)))
                    {
                        locations.Add(new Point3D(x, y, z));
                    }
                    x++;
                }
                x = area.Start.X;
                y++;
            }

            return locations;
        }

        public Point3D GetValidLocation(Rectangle2D area)
        {
            int xbegin = area.Start.X;
            int xend = area.End.X;
            int ybegin = area.Start.Y;
            int yend = area.End.Y;

            int randx = Utility.RandomMinMax(xbegin, xend);
            int randy = Utility.RandomMinMax(ybegin, yend);

            int z = Map.ZombieLand.GetAverageZ(randx, randy);
            while (!Map.ZombieLand.CanSpawnMobile(new Point3D(randx, randy, z)))
            {
                randx = Utility.RandomMinMax(xbegin, xend);
                randy = Utility.RandomMinMax(ybegin, yend);

                z = Map.ZombieLand.GetAverageZ(randx, randy);
            }

            return new Point3D(randx, randy, z);
        }

        public Point3D GetRandomLocation()
        {
            int select = Utility.Random(0, 4);

            Point3D point = Point3D.Zero;
            switch (select)
            {
                case 0:
                {
                    point = GetValidLocation(_MainSpawnRectangle[0]);
                }
                    break;
                case 1:
                {
                    point = GetValidLocation(_MainSpawnRectangle[1]);
                }
                    break;
                case 2:
                {
                    point = GetValidLocation(_MainSpawnRectangle[2]);
                }
                    break;
                case 3:
                {
                    point = GetValidLocation(_MainSpawnRectangle[3]);
                }
                    break;
            }
            return point;
        }

        public void SpawnZombies()
        {
            //We want 2400 zombies in area 1
            //int count = MainSpawnRectangle[0].GetMobiles(Map.ZombieLand).Count(x => x is ZombieZEvent);
            for (int i = _ZombiesLoc1.Count; i <= ZombieEvent.CSOptions.MaxZombiesQuadrant1; i++)
            {
                var zombie = new ZombieZEvent();
                zombie.MoveToWorld(GetValidLocation(_MainSpawnRectangle[0]), Map.ZombieLand);
                zombie.Home = zombie.Location;
                _ZombiesLoc1.Add(zombie);
                zombie.ZombieSerial = Uid;
            }

            //We want 800 zombies in area 2
            //count = MainSpawnRectangle[1].GetMobiles(Map.ZombieLand).Count(x => x is ZombieZEvent);
            for (int i = _ZombiesLoc2.Count; i <= ZombieEvent.CSOptions.MaxZombiesQuadrant2; i++)
            {
                var zombie = new ZombieZEvent();
                zombie.MoveToWorld(GetValidLocation(_MainSpawnRectangle[1]), Map.ZombieLand);
                zombie.Home = zombie.Location;
                _ZombiesLoc2.Add(zombie);
                zombie.ZombieSerial = Uid;
            }

            //We want 800 zombies in area 3
            //count = MainSpawnRectangle[2].GetMobiles(Map.ZombieLand).Count(x => x is ZombieZEvent);
            for (int i = _ZombiesLoc3.Count; i <= ZombieEvent.CSOptions.MaxZombiesQuadrant3; i++)
            {
                var zombie = new ZombieZEvent();
                zombie.MoveToWorld(GetValidLocation(_MainSpawnRectangle[2]), Map.ZombieLand);
                zombie.Home = zombie.Location;
                _ZombiesLoc3.Add(zombie);
                zombie.ZombieSerial = Uid;
            }

            //We want 400 zombies in area 4
            //count = MainSpawnRectangle[3].GetMobiles(Map.ZombieLand).Count(x => x is ZombieZEvent);
            for (int i = _ZombiesLoc4.Count; i <= ZombieEvent.CSOptions.MaxZombiesQuadrant4; i++)
            {
                var zombie = new ZombieZEvent();
                zombie.MoveToWorld(GetValidLocation(_MainSpawnRectangle[3]), Map.ZombieLand);
                zombie.Home = zombie.Location;
                _ZombiesLoc4.Add(zombie);
                zombie.ZombieSerial = Uid;
            }
        }

        public void HandleMobDeath(BaseCreature mob, ZombieAvatar avatar)
        {
            if (mob is ZombieZEvent)
            {
                _ZombiesLoc1.Remove(mob);
                _ZombiesLoc2.Remove(mob);
                _ZombiesLoc3.Remove(mob);
                _ZombiesLoc4.Remove(mob);
            }
            else if (mob is DaemonZombieEvent)
            {
                Daemons.Remove(mob);
            }
            else if (mob is TreeFellow)
            {
                TreeFellows.Remove(mob);
            }
            else if (mob is ZombieSpider)
            {
                ZombieSpiders.Remove(mob);
            }
            else if (mob is FeyWarrior)
            {
                FeyWarriors.Remove(mob);
            }
            else if (mob is Vitriol)
            {
                FeyWarriors.Remove(mob);
            }
            else if (mob is Bird)
            {
                FeyWarriors.Remove(mob);
            }
            else if (mob is HorrifyingTentacle)
            {
                HorrifyingTentacles.Remove(mob);
            }
            else if (mob is GoreFiendZombieEvent)
            {
                GoreFiends.Remove(mob);
            }

            if (avatar.Owner != null)
            {
                PlayerZombieProfile profile = ZombieEvent.EnsureProfile(avatar.Owner);
                profile.AddKill(mob);
            }
        }

        public void RespawnEvent()
        {
            foreach (Mobile zombie in _ZombiesLoc1.ToList())
            {
                if (zombie != null)
                {
                    zombie.Delete();
                }
                _ZombiesLoc1.Remove(zombie);
            }
            _ZombiesLoc1.Clear();
            _ZombiesLoc1.TrimExcess();

            foreach (Mobile zombie in _ZombiesLoc2.ToList())
            {
                if (zombie != null)
                {
                    zombie.Delete();
                }
                _ZombiesLoc2.Remove(zombie);
            }
            _ZombiesLoc2.Clear();
            _ZombiesLoc2.TrimExcess();

            foreach (Mobile zombie in _ZombiesLoc3.ToList())
            {
                if (zombie != null)
                {
                    zombie.Delete();
                }
                _ZombiesLoc3.Remove(zombie);
            }
            _ZombiesLoc3.Clear();
            _ZombiesLoc3.TrimExcess();

            foreach (Mobile zombie in _ZombiesLoc4.ToList())
            {
                if (zombie != null)
                {
                    zombie.Delete();
                }
                _ZombiesLoc4.Remove(zombie);
            }
            _ZombiesLoc4.Clear();
            _ZombiesLoc4.TrimExcess();

            foreach (Mobile daemon in Daemons.ToList())
            {
                if (daemon != null)
                {
                    daemon.Delete();
                }
                Daemons.Remove(daemon);
            }
            Daemons.Clear();
            Daemons.TrimExcess();

            foreach (Mobile bird in Birds.ToList())
            {
                if (bird != null)
                {
                    bird.Delete();
                }
                Birds.Remove(bird);
            }
            Birds.Clear();
            Birds.TrimExcess();

            foreach (Mobile dk in GoreFiends.ToList())
            {
                if (dk != null)
                {
                    dk.Delete();
                }
                GoreFiends.Remove(dk);
            }
            GoreFiends.Clear();
            GoreFiends.TrimExcess();

            foreach (Mobile fey in FeyWarriors.ToList())
            {
                if (fey != null)
                {
                    fey.Delete();
                }
                FeyWarriors.Remove(fey);
            }
            FeyWarriors.Clear();
            FeyWarriors.TrimExcess();

            foreach (Mobile tentacle in HorrifyingTentacles.ToList())
            {
                if (tentacle != null)
                {
                    tentacle.Delete();
                }
                HorrifyingTentacles.Remove(tentacle);
            }
            HorrifyingTentacles.Clear();
            HorrifyingTentacles.TrimExcess();

            foreach (Mobile tf in TreeFellows.ToList())
            {
                if (tf != null)
                {
                    tf.Delete();
                }
                TreeFellows.Remove(tf);
            }
            TreeFellows.Clear();
            TreeFellows.TrimExcess();

            foreach (Mobile vitriol in Vitriols.ToList())
            {
                if (vitriol != null)
                {
                    vitriol.Delete();
                }
                Vitriols.Remove(vitriol);
            }
            Vitriols.Clear();
            Vitriols.TrimExcess();

            foreach (Mobile spider in ZombieSpiders.ToList())
            {
                if (spider != null)
                {
                    spider.Delete();
                }
                ZombieSpiders.Remove(spider);
            }

            ZombieSpiders.Clear();
            ZombieSpiders.TrimExcess();

            SpawnZombies();
            SpawnDaemons();
            SpawnBirds();
            SpawnFey();
            SpawnSpider();
            SpawnTentacles();
            SpawnTreeFellow();
            SpawnVitriol();
            SpawnGoreFiends();
        }

        public void SpawnInitialZombies()
        {
            for (int i = 0; i <= ZombieEvent.CSOptions.MaxZombiesQuadrant1; i++)
            {
                var zombie = new ZombieZEvent();
                zombie.MoveToWorld(GetValidLocation(_MainSpawnRectangle[0]), Map.ZombieLand);
                zombie.Home = zombie.Location;
                _ZombiesLoc1.Add(zombie);
                zombie.ZombieSerial = Uid;
            }

            for (int i = 0; i <= ZombieEvent.CSOptions.MaxZombiesQuadrant2; i++)
            {
                var zombie = new ZombieZEvent();
                zombie.MoveToWorld(GetValidLocation(_MainSpawnRectangle[1]), Map.ZombieLand);
                zombie.Home = zombie.Location;
                _ZombiesLoc2.Add(zombie);
                zombie.ZombieSerial = Uid;
            }

            for (int i = 0; i <= ZombieEvent.CSOptions.MaxZombiesQuadrant3; i++)
            {
                var zombie = new ZombieZEvent();
                zombie.MoveToWorld(GetValidLocation(_MainSpawnRectangle[2]), Map.ZombieLand);
                zombie.Home = zombie.Location;
                _ZombiesLoc3.Add(zombie);
                zombie.ZombieSerial = Uid;
            }

            for (int i = 0; i <= ZombieEvent.CSOptions.MaxZombiesQuadrant4; i++)
            {
                var zombie = new ZombieZEvent();
                zombie.MoveToWorld(GetValidLocation(_MainSpawnRectangle[3]), Map.ZombieLand);
                zombie.Home = zombie.Location;
                _ZombiesLoc4.Add(zombie);
                zombie.ZombieSerial = Uid;
            }
        }

        public void SpawnBirds()
        {
            for (int i = Birds.Count; i <= 1000; i++)
            {
                var bird = new Bird();

                bird.MoveToWorld(GetRandomLocation(), Map.ZombieLand);
                Birds.Add(bird);
                bird.ZombieSerial = Uid;
            }
        }

        public void SpawnDaemons()
        {
            for (int i = Daemons.Count; i <= 250; i++)
            {
                var daemon = new DaemonZombieEvent();

                daemon.MoveToWorld(GetRandomLocation(), Map.ZombieLand);
                daemon.Home = daemon.Location;
                Daemons.Add(daemon);
                daemon.ZombieSerial = Uid;
            }
        }

        public void SpawnFey()
        {
            for (int i = FeyWarriors.Count; i <= 100; i++)
            {
                var fey = new FeyWarrior();

                fey.MoveToWorld(GetRandomLocation(), Map.ZombieLand);
                FeyWarriors.Add(fey);
                fey.ZombieSerial = Uid;
            }
        }


        public void SpawnTreeFellow()
        {
            for (int i = TreeFellows.Count; i <= 100; i++)
            {
                var treefellow = new TreeFellow();

                treefellow.MoveToWorld(GetRandomLocation(), Map.ZombieLand);
                TreeFellows.Add(treefellow);
                treefellow.ZombieSerial = Uid;
            }
        }


        public void SpawnSpider()
        {
            for (int i = ZombieSpiders.Count; i <= 50; i++)
            {
                var spider = new ZombieSpider();

                spider.MoveToWorld(GetValidLocation(_SpiderSpawnRectangle), Map.ZombieLand);

                ZombieSpiders.Add(spider);
                spider.ZombieSerial = Uid;
            }
        }


        public void SpawnTentacles()
        {
            for (int i = HorrifyingTentacles.Count; i < 100; i++)
            {
                var tentacle = new HorrifyingTentacle();

                double select = Utility.RandomDouble();

                tentacle.MoveToWorld(
                    @select <= 0.75 ? GetValidLocation(_TentacleSpawnRectangle) : _TentaclePoints.GetRandom(),
                    Map.ZombieLand);

                tentacle.Home = tentacle.Location;
                HorrifyingTentacles.Add(tentacle);
                tentacle.ZombieSerial = Uid;
            }
        }

        public void SpawnVitriol()
        {
            for (int i = Vitriols.Count; i <= 150; i++)
            {
                var vitriol = new Vitriol();

                vitriol.MoveToWorld(GetRandomLocation(), Map.ZombieLand);
                Vitriols.Add(vitriol);
                vitriol.ZombieSerial = Uid;
            }
        }

        public void SpawnGoreFiends()
        {
            for (int i = GoreFiends.Count; i <= 150; i++)
            {
                var goref = new GoreFiendZombieEvent();

                goref.MoveToWorld(GetRandomLocation(), Map.ZombieLand);
                GoreFiends.Add(goref);
                goref.ZombieSerial = Uid;
            }
        }


        public void SpawnCorpses()
        {
            for (int i = 0; i <= 25; i++)
            {
                var c = new HumanMob {Direction = (Direction) Utility.Random(8)};

                var corpse = CreateDummyCorpse(c) as Corpse;

                if (corpse != null && corpse.ProxyCorpse != null)
                {
                    corpse = corpse.ProxyCorpse;
                }
                if (corpse != null)
                {
                    corpse.MoveToWorld(GetRandomLocation(), Map.ZombieLand);
                    ZombieEvent.AddItem(corpse);
                }
                c.Delete();
            }
        }

        public static Container CreateDummyCorpse(Mobile mob)
        {
            HairInfo hair = null;

            if (mob.HairItemID != 0)
            {
                hair = new HairInfo(mob.HairItemID, mob.HairHue);
            }

            FacialHairInfo facialhair = null;

            if (mob.FacialHairItemID != 0)
            {
                facialhair = new FacialHairInfo(mob.FacialHairItemID, mob.FacialHairHue);
            }

            return Mobile.CreateCorpseHandler != null
                ? Mobile.CreateCorpseHandler(mob, hair, facialhair, new List<Item>(), new List<Item>())
                : null;
        }

        public static Container CreatePlayerCorpse(Mobile mob)
        {
            if (mob == null)
            {
                return null;
            }

            var content = new List<Item>();
            var equip = new List<Item>();
            var moveToPack = new List<Item>();
            var itemsCopy = new List<Item>(mob.Items);

            Container pack = mob.Backpack;

            foreach (Item item in itemsCopy)
            {
                if (item == pack)
                {
                    continue;
                }

                DeathMoveResult res = mob.GetParentMoveResultFor(item);

                switch (res)
                {
                    case DeathMoveResult.MoveToCorpse:
                    {
                        content.Add(item);
                        equip.Add(item);
                        break;
                    }
                    case DeathMoveResult.MoveToBackpack:
                    {
                        moveToPack.Add(item);
                        break;
                    }
                }
            }

            if (pack != null)
            {
                var packCopy = new List<Item>();

                if (pack.Items != null && pack.Items.Count > 0)
                {
                    packCopy.AddRange(pack.Items);

                    foreach (Item item in packCopy)
                    {
                        DeathMoveResult res = mob.GetInventoryMoveResultFor(item);

                        if (res == DeathMoveResult.MoveToCorpse)
                        {
                            content.Add(item);

                            //RunUO SVN 610 - Core change instead.
                            var subItems = new List<Item>();
                            List<Item> lookup = item.LookupItems();

                            if (lookup != null && lookup.Count > 0)
                            {
                                subItems.AddRange(lookup);
                            }

                            moveToPack.AddRange(
                                subItems.Where(
                                    subItem =>
                                        !subItem.Deleted && (subItem.LootType == LootType.Blessed || subItem.Insured)));
                        }
                        else
                        {
                            moveToPack.Add(item);
                        }
                    }
                }

                foreach (Item item in moveToPack.Where(item => !mob.RetainPackLocsOnDeath || item.Parent != pack))
                {
                    pack.DropItem(item);
                }
            }

            HairInfo hair = null;

            if (mob.HairItemID != 0)
            {
                hair = new HairInfo(mob.HairItemID, mob.HairHue);
            }

            FacialHairInfo facialhair = null;

            if (mob.FacialHairItemID != 0)
            {
                facialhair = new FacialHairInfo(mob.FacialHairItemID, mob.FacialHairHue);
            }

            return Mobile.CreateCorpseHandler != null
                ? Mobile.CreateCorpseHandler(mob, hair, facialhair, content, equip)
                : null;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(0);

            Uid.Serialize(writer);

            switch (version)
            {
                case 0:
                {
                    writer.Write(ZombieEventName);
                    writer.Write((int) Status);
                    writer.Write(_CoreTicks);
                    writer.Write(CureWinner);

                    if (CureCompleters == null)
                        CureCompleters = new List<PlayerMobile>();
                    writer.Write(CureCompleters.Count);

                    if (CureCompleters.Count > 0)
                    {
                        foreach (PlayerMobile mob in CureCompleters)
                        {
                            writer.Write(mob);
                        }
                    }

                    writer.Write(_ZombiesLoc1.Count);

                    if (_ZombiesLoc1.Count > 0)
                    {
                        foreach (Mobile mob in _ZombiesLoc1)
                        {
                            writer.Write(mob);
                        }
                    }
                    writer.Write(_ZombiesLoc2.Count);

                    if (_ZombiesLoc2.Count > 0)
                    {
                        foreach (Mobile mob in _ZombiesLoc2)
                        {
                            writer.Write(mob);
                        }
                    }
                    writer.Write(_ZombiesLoc3.Count);

                    if (_ZombiesLoc3.Count > 0)
                    {
                        foreach (Mobile mob in _ZombiesLoc3)
                        {
                            writer.Write(mob);
                        }
                    }
                    writer.Write(_ZombiesLoc4.Count);

                    if (_ZombiesLoc4.Count > 0)
                    {
                        foreach (Mobile mob in _ZombiesLoc4)
                        {
                            writer.Write(mob);
                        }
                    }

                    writer.Write(Daemons.Count);

                    if (Daemons.Count > 0)
                    {
                        foreach (Mobile mob in Daemons)
                        {
                            writer.Write(mob);
                        }
                    }

                    writer.Write(FeyWarriors.Count);

                    if (FeyWarriors.Count > 0)
                    {
                        foreach (Mobile mob in FeyWarriors)
                        {
                            writer.Write(mob);
                        }
                    }

                    writer.Write(TreeFellows.Count);

                    if (TreeFellows.Count > 0)
                    {
                        foreach (Mobile mob in TreeFellows)
                        {
                            writer.Write(mob);
                        }
                    }

                    writer.Write(Birds.Count);

                    if (Birds.Count > 0)
                    {
                        foreach (Mobile mob in Birds)
                        {
                            writer.Write(mob);
                        }
                    }

                    writer.Write(ZombieSpiders.Count);

                    if (ZombieSpiders.Count > 0)
                    {
                        foreach (Mobile mob in ZombieSpiders)
                        {
                            writer.Write(mob);
                        }
                    }

                    writer.Write(HorrifyingTentacles.Count);

                    if (HorrifyingTentacles.Count > 0)
                    {
                        foreach (Mobile mob in HorrifyingTentacles)
                        {
                            writer.Write(mob);
                        }
                    }

                    writer.Write(Vitriols.Count);

                    if (Vitriols.Count > 0)
                    {
                        foreach (Mobile mob in Vitriols)
                        {
                            writer.Write(mob);
                        }
                    }

                    writer.Write(GoreFiends.Count);

                    if (GoreFiends.Count > 0)
                    {
                        foreach (Mobile mob in GoreFiends)
                        {
                            writer.Write(mob);
                        }
                    }
                }
                    break;
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            Daemons = new List<Mobile>();
            FeyWarriors = new List<Mobile>();
            TreeFellows = new List<Mobile>();
            Birds = new List<Mobile>();
            ZombieSpiders = new List<Mobile>();
            HorrifyingTentacles = new List<Mobile>();
            Vitriols = new List<Mobile>();
            GoreFiends = new List<Mobile>();

            _ZombiesLoc1 = new List<Mobile>();
            _ZombiesLoc2 = new List<Mobile>();
            _ZombiesLoc3 = new List<Mobile>();
            _ZombiesLoc4 = new List<Mobile>();

            CureCompleters = new List<PlayerMobile>();

            base.Deserialize(reader);

            int version = reader.ReadInt();

            Uid = new ZombieInstanceSerial(reader);

            switch (version)
            {
                case 0:
                {
                    ZombieEventName = reader.ReadString();
                    Status = (ZombieEventStatus) reader.ReadInt();
                    _CoreTicks = reader.ReadInt();
                    CureWinner = reader.ReadMobile<PlayerMobile>();

                    int count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var player = reader.ReadMobile<PlayerMobile>();
                            if (player != null)
                            {
                                CureCompleters.Add(player);
                            }
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var zombie = reader.ReadMobile<ZombieZEvent>();
                            if (zombie != null)
                            {
                                zombie.ZombieSerial = Uid;
                                _ZombiesLoc1.Add(zombie);
                            }
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var zombie = reader.ReadMobile<ZombieZEvent>();
                            if (zombie != null)
                            {
                                zombie.ZombieSerial = Uid;
                                _ZombiesLoc2.Add(zombie);
                            }
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var zombie = reader.ReadMobile<ZombieZEvent>();
                            if (zombie != null)
                            {
                                zombie.ZombieSerial = Uid;
                                _ZombiesLoc3.Add(zombie);
                            }
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var zombie = reader.ReadMobile<ZombieZEvent>();
                            if (zombie != null)
                            {
                                zombie.ZombieSerial = Uid;
                                _ZombiesLoc4.Add(zombie);
                            }
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var daemon = reader.ReadMobile<DaemonZombieEvent>();
                            if (daemon != null)
                            {
                                daemon.ZombieSerial = Uid;
                                Daemons.Add(daemon);
                            }
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var fey = reader.ReadMobile<FeyWarrior>();
                            if (fey != null)
                            {
                                fey.ZombieSerial = Uid;
                                FeyWarriors.Add(fey);
                            }
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var tf = reader.ReadMobile<TreeFellow>();
                            if (tf != null)
                            {
                                tf.ZombieSerial = Uid;
                                TreeFellows.Add(tf);
                            }
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var bird = reader.ReadMobile<Bird>();
                            if (bird != null)
                            {
                                bird.ZombieSerial = Uid;
                                Birds.Add(bird);
                            }
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var spider = reader.ReadMobile<ZombieSpider>();
                            if (spider != null)
                            {
                                spider.ZombieSerial = Uid;
                                ZombieSpiders.Add(spider);
                            }
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var tent = reader.ReadMobile<HorrifyingTentacle>();
                            if (tent != null)
                            {
                                tent.ZombieSerial = Uid;
                                HorrifyingTentacles.Add(tent);
                            }
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var vitriol = reader.ReadMobile<Vitriol>();
                            if (vitriol != null)
                            {
                                vitriol.ZombieSerial = Uid;
                                Vitriols.Add(vitriol);
                            }
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var gf = reader.ReadMobile<GoreFiendZombieEvent>();
                            if (gf != null)
                            {
                                gf.ZombieSerial = Uid;
                                GoreFiends.Add(gf);
                            }
                        }
                    }
                }
                    break;
            }
        }
    }
}