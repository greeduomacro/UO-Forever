using System;
using System.IO;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;

namespace Server.Items
{
    public class MoveSpawner : Item
    {
        #region Variables/Properties
        private bool m_Active;
        private bool m_ForgetOnAcquire = true;
        private string m_Creature;
        private ArrayList m_Spawned;
        private SpawnedObject m_Object;
        private string m_Message;
        private Point3D m_Location;
        private int m_HomeRange;
        private int m_Team;
        private int m_Limit;
        private DateTime m_NextSpawn;
        private TimeSpan m_Delay;
        private double m_Chance;
        private int m_Sound = 0;
        private TimeSpan m_Decay = TimeSpan.Zero;
        private AccessLevel m_Level = AccessLevel.Owner; // Highest AccessLevel that will trigger a Spawn
        private bool m_DoingDefragNow = false;
        private static TimeSpan TIMER_RES = TimeSpan.FromSeconds(60); // How often all MoveSpawners will be checked for Decay time

        [CommandProperty(AccessLevel.GameMaster)]
        public int SpawnChance
        {
            get { return (int)(m_Chance * 100); }
            set
            {
                int num = value;

                if (num > 100)
                    num = 100;
                else if (num < 1)
                {
                    m_Chance = 0;
                    m_Active = false;
                    return;
                }

                m_Chance = (double)num / 100;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get { return m_Active; }
            set { m_Active = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public String Message
        {
            get { return m_Message; }
            set { m_Message = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D SpawnPoint
        {
            get { return m_Location; }
            set { m_Location = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HomeRange
        {
            get { return m_HomeRange; }
            set { m_HomeRange = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Limit
        {
            get { return m_Limit; }
            set { m_Limit = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Team
        {
            get { return m_Team; }
            set { m_Team = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public String Creature
        {
            get { return m_Creature; }
            set
            {
                string str = value;

                if (str != null)
                {
                    str = str.ToLower();
                    str = str.Trim();

                    Type type = SpawnerType.GetType(str);

                    if (type != null)
                        m_Creature = str;
                    else
                        m_Creature = "-invalid-";
                }
                else
                    m_Creature = null;

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextSpawn
        {
            get { return m_NextSpawn; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan Delay
        {
            get { return m_Delay; }
            set
            {
                m_NextSpawn = m_NextSpawn - m_Delay;
                m_Delay = value;
                m_NextSpawn = m_NextSpawn + m_Delay;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Sound
        {
            get { return m_Sound; }
            set { m_Sound = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Reset
        {
            get { return false; }
            set
            {
                if (value)
                    DeSpawn();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan SpawnDecay
        {
            get { return m_Decay; }
            set { m_Decay = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public AccessLevel MaxAccessLevel
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ForgetOnAcquire
        {
            get { return m_ForgetOnAcquire; }
            set { m_ForgetOnAcquire = value; }
        }

        #endregion

        #region SpawnedObject struct
        public struct SpawnedObject
        {
            public SpawnedObject(Object o)
                : this(o, DateTime.UtcNow)
            {
            }

            public SpawnedObject(Object o, DateTime time)
            {
                Spawned = o;
                Time = time;
            }

            public Object Spawned;
            public DateTime Time;
        }
        #endregion

        #region Constructors
        [Constructable]
        public MoveSpawner()
            : this(null, null, 1, 1, 4, 0, TimeSpan.FromMinutes(5), false, 0)
        {
        }

        [Constructable]
        public MoveSpawner(string creature)
            : this(creature, null, 1, 1, 4, 0, TimeSpan.FromMinutes(5), true, 0)
        {
        }

        [Constructable]
        public MoveSpawner(string creature, string msg)
            : this(creature, msg, 1, 1, 4, 0, TimeSpan.FromMinutes(5), true, 0)
        {
        }

        [Constructable]
        public MoveSpawner(string creature, string msg, int limit)
            : this(creature, msg, limit, 1, 4, 0, TimeSpan.FromMinutes(5), true, 0)
        {
        }

        [Constructable]
        public MoveSpawner(string creature, string msg, int limit, double chance)
            : this(creature, msg, limit, chance, 4, 0, TimeSpan.FromMinutes(5), true, 0)
        {
        }

        [Constructable]
        public MoveSpawner(string creature, string msg, int limit, int range, int team)
            : this(creature, msg, limit, 1, range, team, TimeSpan.FromMinutes(5), true, 0)
        {
        }

        [Constructable]
        public MoveSpawner(string creature, string msg, int limit, double chance, int range, int team)
            : this(creature, msg, limit, chance, range, team, TimeSpan.FromMinutes(5), true, 0)
        {
        }

        [Constructable]
        public MoveSpawner(string creature, string msg, int limit, double chance, int range, int team, int sound)
            : this(creature, msg, limit, chance, range, team, TimeSpan.FromMinutes(5), true, sound)
        {
        }

        public MoveSpawner(string creature, string msg, int limit, double chance, int range, int team, TimeSpan delay, bool active, int sound)
            : base(0x1B72)
        {
            Movable = false;
            Visible = false;
            Name = "Movement Spawner";

            if (creature != null)
                m_Creature = creature.ToLower();
            else
                m_Creature = null;

            m_Spawned = new ArrayList();
            m_Message = msg;
            m_Limit = limit;
            m_HomeRange = range;
            m_Team = team;
            m_Delay = delay;
            m_Active = active;
            m_NextSpawn = DateTime.UtcNow;
            m_Chance = chance;
            m_Sound = sound;

            if (m_Chance > 1)
                m_Chance = 1;
            else if (m_Chance <= 0)
            {
                m_Chance = 0;
                m_Active = false;
            }

        }

        public MoveSpawner(Serial serial)
            : base(serial)
        {
        }
        #endregion

        #region Methods
        public override bool OnMoveOver(Mobile m)
        {
            if (m_Active && (m.AccessLevel <= m_Level))
            {
                if (m.Player)
                {
                    if (Utility.RandomDouble() > m_Chance)
                    {
                        //PublicOverheadMessage( MessageType.Regular, 0x3BD, false, string.Format( "Failed on chance to spawn" )); // debugging
                        return true;
                    }

                    if (m_NextSpawn > DateTime.UtcNow)
                    {
                        //PublicOverheadMessage( MessageType.Regular, 0x3BD, false, string.Format( "Not time to spawn" )); // debugging
                        return true;
                    }

                    Defrag();

                    if (m_Spawned.Count >= m_Limit)
                    {
                        //PublicOverheadMessage( MessageType.Regular, 0x3BD, false, string.Format( "Spawn Limit exceeded" )); // debugging
                        return true;
                    }

                    Spawn();
                }
            }
            return true;
        }

        public void Spawn()
        {
            Map map = Map;

            if (map == null || map == Map.Internal || m_Location == Point3D.Zero || m_Creature == null || m_Creature == "-invalid-")
                return;

            Type type = SpawnerType.GetType((string)m_Creature);

            if (type != null)
            {
                m_NextSpawn = DateTime.UtcNow + m_Delay;

                try
                {
                    object o = Activator.CreateInstance(type);

                    if (o is Mobile)
                    {
                        Mobile m = (Mobile)o;

                        m_Object = new SpawnedObject(m);
                        m_Spawned.Add(m_Object);
                        InvalidateProperties();

                        m.Map = map;
                        m.Location = m_Location;

                        if (m_Message != null)
                            m.PublicOverheadMessage(MessageType.Regular, 0x76C, false, m_Message);

                        if (m_Sound > 0)
                            Effects.PlaySound(m.Location, m.Map, m_Sound);

                        if (m is BaseCreature)
                        {
                            BaseCreature c = (BaseCreature)m;

                            c.RangeHome = m_HomeRange;

                            if (m_Team > 0)
                                c.Team = m_Team;

                            c.Home = m_Location;
                        }
                    }
                    else if (o is Item)
                    {
                        Item item = (Item)o;

                        m_Object = new SpawnedObject(item);
                        m_Spawned.Add(m_Object);
                        InvalidateProperties();

                        item.MoveToWorld(m_Location, map);

                        if (m_Message != null)
                            item.PublicOverheadMessage(MessageType.Regular, 0x76C, false, m_Message);

                        if (m_Sound > 0)
                            Effects.PlaySound(item.Location, item.Map, m_Sound);
                    }
                }
                catch
                {
                    PublicOverheadMessage(MessageType.Regular, 0x3BD, false, string.Format("Exception Caught!")); // debugging
                }
            }
        }

        public void Defrag()
        {
            if (m_DoingDefragNow)
                return;

            m_DoingDefragNow = true;
            bool removed = false;

            for (int i = 0; i < m_Spawned.Count; ++i)
            {
                if (m_Spawned[i] is SpawnedObject)
                {
                    m_Object = (SpawnedObject)m_Spawned[i];
                    object o = m_Object.Spawned;

                    if (o != null && o is Item)
                    {
                        Item item = (Item)o;

                        if (item.Deleted || (m_ForgetOnAcquire && item.Parent != null))
                        {
                            m_Spawned.RemoveAt(i);
                            --i;
                            removed = true;
                        }
                    }

                    else if (o != null && o is Mobile)
                    {
                        if (((Mobile)o).Deleted)
                        {
                            m_Spawned.RemoveAt(i);
                            --i;
                            removed = true;
                        }
                        else if (o is BaseCreature)
                        {
                            if (m_ForgetOnAcquire && (((BaseCreature)o).Controlled || ((BaseCreature)o).IsStabled))
                            {
                                m_Spawned.RemoveAt(i);
                                --i;
                                removed = true;
                            }
                        }
                    }
                    else
                    {
                        m_Spawned.RemoveAt(i);
                        --i;
                        removed = true;
                    }
                }
                else
                {
                    m_Spawned.RemoveAt(i);
                    --i;
                    removed = true;
                }
            }
            if (removed)
                InvalidateProperties();

            m_DoingDefragNow = false;
        }

        public void DeSpawn()
        {
            for (int i = 0; i < m_Spawned.Count; ++i)
            {
                if (m_Spawned[i] is SpawnedObject)
                {
                    m_Object = (SpawnedObject)m_Spawned[i];
                    object o = m_Object.Spawned;

                    if (o != null && o is Item && !((Item)o).Deleted)
                        ((Item)o).Delete();
                    else if (o != null && o is Mobile && !((Mobile)o).Deleted)
                        ((Mobile)o).Delete();
                }
            }

            Defrag();
        }

        public override void OnDelete()
        {
            base.OnDelete();
            DeSpawn();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(this.Name);

            if (m_Location == Point3D.Zero)
                list.Add("Spawn Point is not set!");
            else if (m_Creature != null)
                list.Add(string.Format("{0}, {1} of {2}", m_Creature, m_Spawned.Count, m_Limit));

            if (m_Active)
                list.Add(1060742); // active
            else
                list.Add(1060743); // inactive
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (m_Location == Point3D.Zero)
            {
                LabelTo(from, "Spawn Point is not set!");
            }
            else if (m_Active)
            {
                LabelTo(from, m_Creature);
                LabelTo(from, string.Format("{0} of {1}", m_Spawned.Count, m_Limit));
            }
            else
            {
                LabelTo(from, "(inactive)");
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel < AccessLevel.GameMaster)
                return;

            from.SendGump(new PropertiesGump(from, this));
        }

        public void DecaySpawns()
        {
            if (m_Decay == TimeSpan.Zero)
                return;

            for (int i = 0; i < m_Spawned.Count; ++i)
            {
                if (m_Spawned[i] is SpawnedObject)
                {
                    m_Object = (SpawnedObject)m_Spawned[i];

                    if (DateTime.UtcNow <= m_Object.Time + m_Decay)
                        return;

                    object o = m_Object.Spawned;

                    if (o != null && o is Item && !((Item)o).Deleted)
                        ((Item)o).Delete();
                    else if (o != null && o is Mobile && !((Mobile)o).Deleted)
                        ((Mobile)o).Delete();
                }
            }

            Defrag();
        }
        #endregion

        #region Serialize
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3); // version

            writer.Write((string)m_Decay.ToString());
            writer.Write((int)m_Level);

            writer.Write(m_Sound);
            writer.Write(m_Chance);
            writer.Write(m_Active);
            writer.Write(m_Creature);
            writer.Write(m_Message);
            writer.Write(m_Location);
            writer.Write(m_HomeRange);
            writer.Write(m_Team);
            writer.Write(m_Limit);
            writer.Write((string)m_NextSpawn.ToString());
            writer.Write(m_Delay);

            Defrag();
            writer.Write(m_Spawned.Count);

            for (int i = 0; i < m_Spawned.Count; ++i)
            {
                if (m_Spawned[i] is SpawnedObject)
                {
                    m_Object = (SpawnedObject)m_Spawned[i];
                    object o = m_Object.Spawned;

                    if (o is Item)
                        writer.Write((Item)o);
                    else if (o is Mobile)
                        writer.Write((Mobile)o);
                    else
                        writer.Write(Serial.MinusOne);

                    writer.Write((m_Object.Time).ToString());
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            DateTime time;
            base.Deserialize(reader);

            int version = reader.ReadInt();
            if (version < 3)
            {
                Old_Deserialize(reader, version);
                return;
            }

            switch (version)
            {
                case 3:
                    {
                        TimeSpan.TryParse(reader.ReadString(), out m_Decay);
                        m_Level = (AccessLevel)reader.ReadInt();
                        goto case 2;
                    }
                case 2:
                    {
                        //case 2
                        m_Sound = reader.ReadInt();
                        //case 1
                        m_Chance = reader.ReadDouble();
                        //case 0
                        m_Active = reader.ReadBool();
                        m_Creature = reader.ReadString();
                        m_Message = reader.ReadString();
                        m_Location = reader.ReadPoint3D();
                        m_HomeRange = reader.ReadInt();
                        m_Team = reader.ReadInt();
                        m_Limit = reader.ReadInt();
                        DateTime.TryParse(reader.ReadString(), out m_NextSpawn);
                        m_Delay = reader.ReadTimeSpan();

                        int size = reader.ReadInt();
                        m_Spawned = new ArrayList(size);

                        for (int i = 0; i < size; ++i)
                        {
                            IEntity e = World.FindEntity(reader.ReadInt());
                            DateTime.TryParse(reader.ReadString(), out time);

                            if (e != null)
                            {
                                m_Object = new SpawnedObject(e, time);
                                m_Spawned.Add(m_Object);
                            }
                        }

                        break;
                    }
            }
        }

        public void Old_Deserialize(GenericReader reader, int version)
        {
            switch (version)
            {
                case 2:
                    {
                        m_Sound = reader.ReadInt();
                        goto case 1;
                    }
                case 1:
                    {
                        m_Chance = reader.ReadDouble();
                        goto case 0;
                    }
                case 0:
                    {
                        m_Active = reader.ReadBool();
                        m_Creature = reader.ReadString();
                        m_Message = reader.ReadString();
                        m_Location = reader.ReadPoint3D();
                        m_HomeRange = reader.ReadInt();
                        m_Team = reader.ReadInt();
                        m_Limit = reader.ReadInt();
                        DateTime.TryParse(reader.ReadString(), out m_NextSpawn);
                        m_Delay = reader.ReadTimeSpan();

                        int size = reader.ReadInt();
                        m_Spawned = new ArrayList(size);

                        for (int i = 0; i < size; ++i)
                        {
                            IEntity e = World.FindEntity(reader.ReadInt());

                            if (e != null)
                            {
                                m_Object = new SpawnedObject(e);
                                m_Spawned.Add(m_Object);
                            }
                        }

                        break;
                    }
            }
            if (version == 0)
                m_Chance = 1;
        }
        #endregion

        #region Decay Timer
        public static void Initialize()
        {
            DecayTimer MoveSpawnerDecay_timer = new DecayTimer();
            MoveSpawnerDecay_timer.Start();
        }

        // Note: TIMER_RES is set at the top of the script about line 31.
        private class DecayTimer : Timer
        {
            public DecayTimer()
                : base(TIMER_RES, TIMER_RES)
            {
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                try
                {
                    ArrayList to_check = new ArrayList();

                    foreach (Item i in World.Items.Values)
                        if (i != null && i is MoveSpawner && !i.Deleted && ((MoveSpawner)i).Active)
                            to_check.Add(i);

                    for (int i = 0; i < to_check.Count; ++i)
                        ((MoveSpawner)to_check[i]).DecaySpawns();

                }
                catch (Exception e)
                {
                    Console.WriteLine("/nException caught in MoveSpawner Decay Timer:/n{0}/n", e);
                }
            }
        }
        #endregion
    }
}