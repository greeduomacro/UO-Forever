using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Commands;
using Server.Accounting;
using System.Collections;

namespace Server.Engines.XmlSpawner2
{
    public class XmlGroup : XmlAttachment
    {
        public enum EventScoring : byte
        {
            HighestScore = 0,
            LowestScore = 1,
            ShortestTime = 2,
            LongestTime = 3
        }

        // a serial constructor is REQUIRED
        public XmlGroup(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlGroup(string name, string eventName)
        {
            Name = name;
            m_EventName = eventName;
        }

        public XmlGroup()
        {
        }

        private int m_Score = 0; // for score events
        [CommandProperty(AccessLevel.GameMaster)]
        public int Score { get { return m_Score; } set { m_Score = value; } }

        private int m_MaxMembers = 5; // for score events
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxMembers { get { return m_MaxMembers; } set { m_MaxMembers = value; } }

        private int m_MaxParticipants = 5; // for score events
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxParticipants { get { return m_MaxParticipants; } set { m_MaxParticipants = value; } }

        private TimeSpan m_Time = TimeSpan.MinValue; // for timed events
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan Time { get { return m_Time; } set { m_Time = value; } }

        private DateTime m_StartTime = DateTime.MinValue; // for timed events
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime StartTime { get { return m_StartTime; } set { m_StartTime = value; } }

        private DateTime m_EndTime = DateTime.MaxValue; // for timed events
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime EndTime { get { return m_EndTime; } set { m_EndTime = value; } }

        private Mobile m_Captain = null;
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Captain { get { return m_Captain; } set { m_Captain = value; } }

        private ArrayList m_Members = new ArrayList();
        public ArrayList Members { get { return m_Members; } set { m_Members = value; } }

        private ArrayList m_Participants = new ArrayList();
        public ArrayList Participants { get { return m_Participants; } set { m_Participants = value; } }

        private bool m_EventInProgress = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool EventInProgress { get { return m_EventInProgress; } set { m_EventInProgress = value; } }

        private bool m_Locked = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Locked { get { return m_Locked; } set { m_Locked = value; } }

        private string m_EventName = null;
        [CommandProperty(AccessLevel.GameMaster)]
        public string EventName { get { return m_EventName; } set { m_EventName = value; } }

        private EventScoring m_EventType = EventScoring.HighestScore;
        [CommandProperty(AccessLevel.GameMaster)]
        public EventScoring EventType { get { return m_EventType; } set { m_EventType = value; } }

        private Point3D m_SpawnLocation = new Point3D(0, 0, 0);
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D SpawnLocation { get { return m_SpawnLocation; } set { m_SpawnLocation = value; } }

        private Map m_SpawnMap = Map.Felucca;
        [CommandProperty(AccessLevel.GameMaster)]
        public Map SpawnMap { get { return m_SpawnMap; } set { m_SpawnMap = value; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);
            // version 1
            writer.Write(m_SpawnLocation);
            writer.Write(m_SpawnMap);

            // version 0
            writer.Write((int)m_Score);
            writer.Write((int)m_MaxMembers);
            writer.Write((int)m_MaxParticipants);
            writer.Write((TimeSpan)m_Time);
            writer.Write((Mobile)m_Captain);
            writer.Write((int)m_Members.Count);
            foreach (Mobile mob in m_Members)
            {
                writer.Write(mob);
            }
            writer.Write((bool)m_Locked);
            writer.Write((bool)m_EventInProgress);
            writer.Write((string)m_EventName);
            writer.Write((byte)m_EventType);
            writer.Write((DateTime)m_StartTime);
            writer.Write((DateTime)m_EndTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 1:
                    m_SpawnLocation = reader.ReadPoint3D();
                    m_SpawnMap = reader.ReadMap();
                    goto case 0;
                case 0:
                    m_Score = reader.ReadInt();
                    m_MaxMembers = reader.ReadInt();
                    m_MaxParticipants = reader.ReadInt();
                    m_Time = reader.ReadTimeSpan();
                    m_Captain = reader.ReadMobile();
                    int memberCount = reader.ReadInt();
                    for (int i = 0; i < memberCount; i++)
                    {
                        Mobile mob = reader.ReadMobile();
                        if (mob != null)
                        {
                            m_Members.Add(mob);
                        }
                    }
                    m_Locked = reader.ReadBool();
                    m_EventInProgress = reader.ReadBool();
                    m_EventName = reader.ReadString();
                    m_EventType = (EventScoring)reader.ReadByte(); 
                    m_StartTime = reader.ReadDateTime();
                    m_EndTime = reader.ReadDateTime();
                    break;
            }
        }

        public static List<XmlGroup> GetGroups(IEntity m)
        {
            if (m == null) return null;
            ArrayList alist = XmlAttach.FindAttachments(m);
            if (alist == null || alist.Count == 0) { return null; }
            List<XmlGroup> output = new List<XmlGroup>();
            foreach (Object xmlattachment in alist)
            {
                if (xmlattachment is XmlGroup)
                {
                    output.Add(xmlattachment as XmlGroup);
                }
            }
            if (output.Count > 0) return output;
            return null;
        }
    }
}
