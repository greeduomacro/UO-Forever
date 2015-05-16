#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using VitaNex;
using VitaNex.Notify;

#endregion

namespace Server.Engines.EventScores
{
    public sealed class EventObject : PropertyObject
    {
        [CommandProperty(EventScores.Access, true)]
        public EventSerial UID { get; private set; }

        [CommandProperty(EventScores.Access, true)]
        public EventType EventType{ get; set; }

        [CommandProperty(EventScores.Access, true)]
        public string EventName { get; set; }

        [CommandProperty(EventScores.Access, true)]
        public DateTime TimeAwarded { get; set; }

        [CommandProperty(EventScores.Access, true)]
        public int PointsGained { get; set; }

        public EventObject(EventType type, string name, DateTime time, int points)
        {
            UID = new EventSerial();

            EventType = type;
            EventName = name;
            TimeAwarded = time;
            PointsGained = points;
        }

        public EventObject(GenericReader reader)
            : base(reader)
        { }

        public override void Reset()
        { }

        public override void Clear()
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(0);

            UID.Serialize(writer);

            switch (version)
            {
                case 0:
                {
                    writer.Write((int)EventType);
                    writer.Write(EventName);
                    writer.Write(TimeAwarded);
                    writer.Write(PointsGained);
                }
                    break;
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            UID = new EventSerial(reader);

            switch (version)
            {
                case 0:
                {
                    EventType = (EventType)reader.ReadInt();
                    EventName = reader.ReadString();
                    TimeAwarded = reader.ReadDateTime();
                    PointsGained = reader.ReadInt();
                }
                    break;
            }
        }
    }
}