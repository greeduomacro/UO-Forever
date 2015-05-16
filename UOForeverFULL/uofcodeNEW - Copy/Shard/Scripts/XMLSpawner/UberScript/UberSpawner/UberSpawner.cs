using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Engines.XmlSpawner2
{
    public class UberSpawner : Item, ISpawner
    {
        List<UberSpawnerEntry> m_Entries = new List<UberSpawnerEntry>();
        public List<UberSpawnerEntry> Entries { get { return m_Entries; } }
        Dictionary<ISpawnable, UberSpawnerEntry> m_Spawned = new Dictionary<ISpawnable, UberSpawnerEntry>();
        public Dictionary<ISpawnable, UberSpawnerEntry> Spawned { get { return m_Spawned; } }
        
        private bool m_Running;
        public void Start()
        {
            if (!m_Running)
            {
                if (m_Entries.Count > 0)
                {
                    m_Running = true;
                   // DoTimer();
                }
            }
        }

        public void Stop()
        {
            if (m_Running)
            {
                //if (m_Timer != null)
                 //   m_Timer.Stop();
                m_Running = false;
            }
        }
        
        public bool UnlinkOnTaming { get { return true; } }

        public Point3D HomeLocation { get { return Location; } }

        private int m_HomeRange;
        [CommandProperty( AccessLevel.GameMaster )]
		public int HomeRange{ get { return m_HomeRange; } set { m_HomeRange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Running
        {
            get { return m_Running; }
            set
            {
                if (value)
                    Start();
                else
                    Stop();

                InvalidateProperties();
            }
        }

        public Region Region { get { return Region.Find(Location, Map); } }

        public void Remove(ISpawnable spawn)
        {

        }

        [Constructable]
        public UberSpawner()
            : base(4846)
        {
            Visible = false;
            Movable = false;
            //m_Running = true;
        }

        public UberSpawner(Serial serial)
            : base(serial)
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class UberSpawnerEntry
    {
        private string m_SpawnedName;
        private int m_SpawnedProbability;
        private List<ISpawnable> m_Spawned;
        private int m_SpawnedMaxCount;
        private string m_Properties;
        private string m_Parameters;
        //private EntryFlags m_Valid;

        public int SpawnedProbability
        {
            get { return m_SpawnedProbability; }
            set { m_SpawnedProbability = value; }
        }

        public int SpawnedMaxCount
        {
            get { return m_SpawnedMaxCount; }
            set { m_SpawnedMaxCount = value; }
        }

        public string SpawnedName
        {
            get { return m_SpawnedName; }
            set { m_SpawnedName = value; }
        }

        public string Properties
        {
            get { return m_Properties; }
            set { m_Properties = value; }
        }

        public string Parameters
        {
            get { return m_Parameters; }
            set { m_Parameters = value; }
        }

        public List<ISpawnable> Spawned { get { return m_Spawned; } }
        public bool IsFull { get { return m_Spawned.Count >= m_SpawnedMaxCount; } }

        public UberSpawnerEntry(string name, int probability, int maxcount)
        {
            m_SpawnedName = name;
            m_SpawnedProbability = probability;
            m_SpawnedMaxCount = maxcount;
            m_Spawned = new List<ISpawnable>();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)0); // version

            writer.Write(m_SpawnedName);
            writer.Write(m_SpawnedProbability);
            writer.Write(m_SpawnedMaxCount);

            writer.Write(m_Properties);
            writer.Write(m_Parameters);

            writer.Write(m_Spawned.Count);

            for (int i = 0; i < m_Spawned.Count; ++i)
            {
                object o = m_Spawned[i];

                if (o is Item)
                    writer.Write((Item)o);
                else if (o is Mobile)
                    writer.Write((Mobile)o);
                else
                    writer.Write(Serial.MinusOne);
            }
        }

        public UberSpawnerEntry(UberSpawner parent, GenericReader reader)
        {
            int version = reader.ReadInt();

            m_SpawnedName = reader.ReadString();
            m_SpawnedProbability = reader.ReadInt();
            m_SpawnedMaxCount = reader.ReadInt();

            m_Properties = reader.ReadString();
            m_Parameters = reader.ReadString();

            int count = reader.ReadInt();

            m_Spawned = new List<ISpawnable>(count);

            for (int i = 0; i < count; ++i)
            {
                //IEntity e = World.FindEntity( reader.ReadInt() );
                ISpawnable e = reader.ReadEntity() as ISpawnable;

                if (e != null)
                {
                    e.Spawner = parent;

                    if (e is BaseCreature)
                        ((BaseCreature)e).RemoveIfUntamed = true;

                    m_Spawned.Add(e);

                    if (!parent.Spawned.ContainsKey(e))
                        parent.Spawned.Add(e, this);
                }
            }
        }

        public void Defrag(UberSpawner parent)
        {
            for (int i = 0; i < m_Spawned.Count; ++i)
            {
                ISpawnable e = m_Spawned[i];
                bool remove = false;

                if (e is Item)
                {
                    Item item = (Item)e;

                    if (item.Deleted || item.RootParent is Mobile || item.IsLockedDown || item.IsSecure || item.Spawner == null)
                        remove = true;
                }
                else if (e is Mobile)
                {
                    Mobile m = (Mobile)e;

                    if (m.Deleted)
                        remove = true;
                    else if (m is BaseCreature)
                    {
                        BaseCreature c = (BaseCreature)m;

                        if (c.Controlled || c.IsStabled)
                            remove = true;
                        /*
                                                else if ( c.Combatant == null && ( c.GetDistanceToSqrt( Location ) > (c.RangeHome * 4) ) )
                                                {
                                                    //m_Spawned[i].Delete();
                                                    m_Spawned.RemoveAt( i );
                                                    --i;
                                                    c.Delete();
                                                    remove = true;
                                                }
                        */
                    }
                    else if (m.Spawner == null)
                        remove = true;
                }
                else
                    remove = true;

                if (remove)
                {
                    m_Spawned.RemoveAt(i--);
                    if (parent.Spawned.ContainsKey(e))
                        parent.Spawned.Remove(e);
                }
            }
        }
    }
}
