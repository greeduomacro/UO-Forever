// **********
// RunUO Shard - CapturePoint.cs
// **********

#region References

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Regions;

#endregion

namespace Server.Scripts.New.Adam
{
    public class CaptureZone : Item
    {
        public static void Initialize()
        {
            CommandSystem.Register("EventMsg", AccessLevel.GameMaster, EventMsg_OnCommand);
        }

        [Usage("EventMsg")]
        [Description("Turn on and off event messages")]
        private static void EventMsg_OnCommand(CommandEventArgs e)
        {
            var m = e.Mobile as PlayerMobile;
            if (m == null) return;
            if (m.EventMsgFlag)
            {
                m.SendMessage("You have opted out of receiving event messages, to opt back in, type [Eventmsg");
                m.EventMsgFlag = false;
            }
            else
            {
                m.SendMessage("You have opted into receiving event messages, to opt out, type [Eventmsg");
                m.EventMsgFlag = true;
            }
        }

        private bool _mActive;

        //private int m_SpawnRange;
        private Rectangle2D _mArea;
        private CaptureZoneRegion _mRegion;

        //Goes back each level, below level 0 and it goes off!
        private TimeSpan _mExpireDelay;
        private DateTime _mExpireTime;

        private DateTime _mNextActiveTime;

        private TimeSpan _mRestartDelay;
        private DateTime _mRestartTime;

        private Timer _mTimer, _mRestartTimer;

        private bool _mPreActive;

        private bool _mAlwaysGrey, _mNoCorpseCarving;

        public Timer RestartTimer
        {
            get { return _mRestartTimer; }
            set { _mRestartTimer = value; }
        }

        public override bool HandlesOnMovement
        {
            get { return true; }
        } // Tell the core that we implement OnMovement

        public override bool HandlesOnSpeech
        {
            get { return true; }
        } // Tell the core that we implement OnMovement

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;

            if (!e.Handled && from.InRange(this, 10) && e.Speech.ToLower() == "active" && !Active)
            {
                PublicOverheadMessage(MessageType.Emote, 2049, false, "The Battle for Bucaneer's Den will begin in " + NextProximityTime.ToString(CultureInfo.InvariantCulture) + "minutes.");
            }
        }

        public static double MaxKillPoints = 1000000; // max number of points allowed for an individual

        [CommandProperty(AccessLevel.GameMaster)]
        public bool AlwaysGrey
        {
            get { return _mAlwaysGrey; }
            set { _mAlwaysGrey = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool NoCorpseCarving
        {
            get { return _mNoCorpseCarving; }
            set { _mNoCorpseCarving = value; }
        }


        [CommandProperty(AccessLevel.GameMaster)]
        public double CurrentTopScore
        {
            get
            {
                if (_mPlayerScores == null || _mPlayerScores.Count == 0) return 0.0;

                return _mPlayerScores.Select(pair => pair.Value).Concat(new[] {0.0}).Max();
            }
        }


        [Constructable]
        public CaptureZone()
            : base(0xBD2)
        {
            Movable = false;
            Visible = false;

            Name = "Buc's Den Capture Point";
            _mExpireDelay = TimeSpan.FromMinutes(30.0);
            _mRestartDelay = TimeSpan.FromMinutes(30.0);
            _mNoCorpseCarving = true;
            _mAlwaysGrey = true;

            Timer.DelayCall(TimeSpan.Zero, SetArea);
        }

        public void SetArea()
        {
            Area = new Rectangle2D(new Point2D(X - 100, Y - 100), new Point2D(X + 100, Y + 100));
        }

        public virtual CaptureZoneRegion GetRegion()
        {
            return new CaptureZoneRegion(this);
        }

        public void UpdateRegion()
        {
            if (_mRegion != null)
                _mRegion.Unregister();

            if (Deleted || Map == Map.Internal) return;
            _mRegion = GetRegion();
            _mRegion.Register();
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (!m.Player || GetRegion() == null || Active || _mPreActive) return;
            if (DateTime.UtcNow < NextProximityTime) return;
            var mobs = GetMobilesInRange(10);
            var players =
                (from Mobile mob in mobs
                    where mob is PlayerMobile && mob.AccessLevel >= AccessLevel.Player
                    select mob as PlayerMobile).ToList();

            //if ( addresses.Count >= 3 )
            if (players.Count < 2) return;
            foreach (
                var pm in
                    NetState.Instances.Where(ns => ns != null && ns.Socket != null && ns.Mobile is PlayerMobile)
                        .Select(ns => (PlayerMobile) ns.Mobile)
                        .Where(p => p.EventMsgFlag))
            {
                pm.SendMessage(44,
                    "The Battle for Bucaneer's Den has been activated!  The event will begin in 2 minutes.");
                if (pm.Region is CaptureZoneRegion)
                    pm.CaptureZone = this;
            }
            BeginRestart(TimeSpan.FromMinutes(2.0));
            _mPreActive = true;
            if (m.Player && ((PlayerMobile) m).CaptureZone == null)
            {
                ((PlayerMobile) m).CaptureZone = this;
            }
            base.OnMovement(m, oldLocation);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Rectangle2D Area
        {
            get { return _mArea; }
            set
            {
                _mArea = value;
                InvalidateProperties();
                UpdateRegion();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan RestartDelay
        {
            get { return _mRestartDelay; }
            set { _mRestartDelay = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime RestartTime
        {
            get { return _mRestartTime; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan ExpireDelay
        {
            get { return _mExpireDelay; }
            set { _mExpireDelay = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime ExpireTime
        {
            get { return _mExpireTime; }
            set { _mExpireTime = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get { return _mActive; }
            set
            {
                if (value)
                    Start();
                else
                {
                    Stop();
                }

                InvalidateProperties();
            }
        }

        public override void OnDelete()
        {
            foreach (
                var pm in
                    NetState.Instances.Where(ns => ns != null && ns.Socket != null && ns.Mobile is PlayerMobile)
                        .Select(ns => (PlayerMobile) ns.Mobile)
                        .Where(p => p.CaptureZone != null))
            {
                pm.CaptureZone = null;
            }
            base.OnDelete();
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextProximityTime
        {
            get { return _mNextActiveTime; }
            set { _mNextActiveTime = value; }
        }

        public void Start()
        {
            if (_mActive || Deleted)
                return;

            foreach (
                var pm in
                    NetState.Instances.Where(ns => ns != null && ns.Socket != null && ns.Mobile is PlayerMobile)
                        .Select(ns => (PlayerMobile) ns.Mobile)
                        .Where(p => p.EventMsgFlag))
            {
                pm.SendMessage(44, "The Battle for Bucaneer's Den has begun!");
            }
            PublicOverheadMessage(MessageType.Emote, 2049, false, "The Battle for Bucaneer's Den has begun!");
            _mPreActive = false;
            _mActive = true;

            if (_mTimer != null)
                _mTimer.Stop();

            _mTimer = new RegionTimer(this);
            _mTimer.Start();

            if (_mRestartTimer != null)
                _mRestartTimer.Stop();

            _mRestartTimer = null;

            _mExpireTime = DateTime.UtcNow + TimeSpan.FromMinutes(30);
        }

        public void Stop()
        {
            if (!_mActive || Deleted)
                return;

            PublicOverheadMessage(MessageType.Emote, 2049, false, "The Battle for Bucaneer's Den has ended.");
            _mActive = false;
            var myList = _mPlayerScores.OrderByDescending(x => x.Value).ToList();

            foreach (
                var pm in
                    NetState.Instances.Where(ns => ns != null && ns.Socket != null && ns.Mobile is PlayerMobile)
                        .Select(ns => (PlayerMobile) ns.Mobile)
                        .Where(p => p.EventMsgFlag || PlayerScores.ContainsKey(p)))
            {
                if (PlayerScores.ContainsKey(pm))
                {
                    pm.SendMessage(44, "Your score was " + PlayerScores[pm]);
                    pm.SendGump(new ScoreGumpBbd(myList));
                }
                pm.SendMessage(44, "The Battle for Bucaneer's Den has ended.  It will be available again in 30 minutes.");
                pm.CaptureZone = null;
            }
            if (_mTimer != null)
                _mTimer.Stop();
            _mPlayerScores.Clear();

            _mTimer = null;

            if (_mRestartTimer != null)
                _mRestartTimer.Stop();

            _mRestartTimer = null;


            NextProximityTime = DateTime.UtcNow + TimeSpan.FromMinutes(30);
        }

        public void BeginRestart(TimeSpan ts)
        {
            if (_mRestartTimer != null)
                _mRestartTimer.Stop();

            _mRestartTime = DateTime.UtcNow + ts;

            _mRestartTimer = new RestartTimer(this, ts);
            _mRestartTimer.Start();
        }

        public void EndRestart()
        {
            Start();
            //Start();
        }

        public void OnSlice()
        {
            if (!_mActive || Deleted)
                return;

            if (DateTime.UtcNow >= _mExpireTime)
                Stop();
        }


        private Dictionary<Mobile, double> _mPlayerScores = new Dictionary<Mobile, double>();

        public Dictionary<Mobile, double> PlayerScores
        {
            get { return _mPlayerScores; }
        }


        public override void OnDoubleClick(Mobile from)
        {
            from.SendGump(new PropertiesGump(from, this));
        }

        public override void OnLocationChange(Point3D oldLoc)
        {
            if (Deleted)
                return;

            _mArea.X += Location.X - oldLoc.X;
            _mArea.Y += Location.Y - oldLoc.Y;

            UpdateRegion();
        }

        public override void OnMapChange()
        {
            if (Deleted)
                return;

            UpdateRegion();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            Stop();

            UpdateRegion();
        }

        public CaptureZone(Serial serial)
            : base(serial)
        {
        }


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(_mPlayerScores.Count);
            foreach (var kvp in _mPlayerScores)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }

            writer.WriteDeltaTime(_mNextActiveTime);
            writer.Write(_mArea);

            writer.Write(_mAlwaysGrey);
            writer.Write(_mNoCorpseCarving);

            writer.Write(_mActive);
            writer.Write(_mExpireDelay);
            writer.WriteDeltaTime(_mExpireTime);
            writer.Write(_mRestartDelay);

            writer.Write(_mRestartTimer != null);

            if (_mRestartTimer != null)
                writer.WriteDeltaTime(_mRestartTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            _mPlayerScores = new Dictionary<Mobile, double>();

            var version = reader.ReadInt();

            switch (version)
            {
                case 0:
                {
                    var entries = reader.ReadInt();
                    for (var i = 0; i < entries; ++i)
                    {
                        var m = reader.ReadMobile();
                        var damage = reader.ReadDouble();
                        if (m != null)
                            _mPlayerScores.Add(m, damage);
                    }

                    _mNextActiveTime = reader.ReadDeltaTime();

                    _mArea = reader.ReadRect2D();

                    _mAlwaysGrey = reader.ReadBool();
                    _mNoCorpseCarving = reader.ReadBool();

                    var active = reader.ReadBool();
                    _mExpireDelay = reader.ReadTimeSpan();
                    _mExpireTime = reader.ReadDeltaTime();
                    _mRestartDelay = reader.ReadTimeSpan();

                    if (reader.ReadBool())
                    {
                        _mRestartTime = reader.ReadDeltaTime();
                        BeginRestart(_mRestartTime - DateTime.UtcNow);
                    }


                    if (active)
                        Start();

                    break;
                }
            }

            Timer.DelayCall(TimeSpan.Zero, UpdateRegion);
        }
    }

    public class CaptureZoneRegion : BaseRegion
    {
        private readonly CaptureZone _mCzone;

        public CaptureZone Czone
        {
            get { return _mCzone; }
        }

        public CaptureZoneRegion(CaptureZone spawn)
            : base(null, spawn.Map, Find(spawn.Location, spawn.Map), spawn.Area)
        {
            _mCzone = spawn;
        }

        public override void OnEnter(Mobile m)
        {
            if (!m.Player) return;
            m.SendMessage(44, "You have joined the Battle for Bucaneer's Den.");
            if (!Czone.Active) return;
            if (m.Player && ((PlayerMobile) m).CaptureZone == null)
            {
                ((PlayerMobile) m).CaptureZone = Czone;
            }
        }

        public override void OnExit(Mobile m)
        {
            if (!m.Player) return;

            m.SendMessage(44, "You have left the Battle for Bucaneer's Den.");
            ((PlayerMobile) m).CaptureZone = null;
        }
    }

    public class RegionTimer : Timer
    {
        private readonly CaptureZone _mZone;

        public RegionTimer(CaptureZone spawn)
            : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
        {
            _mZone = spawn;
            Priority = TimerPriority.OneSecond;
        }

        protected override void OnTick()
        {
            _mZone.OnSlice();
        }
    }

    public class RestartTimer : Timer
    {
        private readonly CaptureZone _mZone;

        public RestartTimer(CaptureZone zone, TimeSpan delay)
            : base(delay)
        {
            _mZone = zone;
            Priority = TimerPriority.FiveSeconds;
        }

        protected override void OnTick()
        {
            _mZone.EndRestart();
        }
    }
}