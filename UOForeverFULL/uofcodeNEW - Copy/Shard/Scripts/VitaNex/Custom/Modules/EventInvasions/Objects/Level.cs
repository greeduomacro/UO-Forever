#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using VitaNex;
using VitaNex.Notify;

#endregion

namespace Server.Engines.EventInvasions
{
    public sealed class Level : PropertyObject
    {
        [CommandProperty(EventInvasions.Access, true)]
        public LevelSerial UID { get; private set; }

        [CommandProperty(EventInvasions.Access, true)]
        public List<Type> Creatures{ get; set; }

        [CommandProperty(EventInvasions.Access, true)]
        public List<Type> RewardItems { get; set; }

        [CommandProperty(EventInvasions.Access, true)]
        public double DropChance { get; set; }

        [CommandProperty(EventInvasions.Access, true)]
        public string InvaderTitles { get; set; }

        [CommandProperty(EventInvasions.Access, true)]
        public TimeSpan TimeLimit { get; set; }

        [CommandProperty(EventInvasions.Access, true)]
        public int KillAmount { get; set; }

        [CommandProperty(EventInvasions.Access, true)]
        public int Plat { get; set; }

        [CommandProperty(EventInvasions.Access, true)]
        public int SpawnAmount { get; set; }

        public Level(TimeSpan timelimit, int spawnamount, int killamount, string invadertitles, int plat)
        {
            UID = new LevelSerial();

            TimeLimit = timelimit;
            SpawnAmount = spawnamount;
            KillAmount = killamount;
            InvaderTitles = invadertitles;
            Creatures = new List<Type>();
            Plat = plat;
        }

        public Level(GenericReader reader)
            : base(reader)
        { }

        public override void Reset()
        { }

        public override void Clear()
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(1);

            UID.Serialize(writer);

            switch (version)
            {
                case 1:
                {
                    writer.Write(Plat);
                    goto case 0;
                }
                case 0:
                {
                    writer.Write(InvaderTitles);
                    writer.Write(TimeLimit);
                    writer.Write(SpawnAmount);
                    writer.Write(KillAmount);

                    writer.Write(Creatures.Count);

                    if (Creatures.Count > 0)
                    {
                        foreach (Type creature in Creatures)
                        {
                            writer.WriteType(creature);
                        }
                    }
                }
                    break;
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            Creatures = new List<Type>();
            RewardItems = new List<Type>();

            int version = reader.ReadInt();

            UID = new LevelSerial(reader);

            switch (version)
            {
                case 1:
                {
                    Plat = reader.ReadInt();
                }
                goto case 0;
                case 0:
                {
                    InvaderTitles = reader.ReadString();
                    TimeLimit = reader.ReadTimeSpan();
                    SpawnAmount = reader.ReadInt();
                    KillAmount = reader.ReadInt();

                    int count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            Type creature = reader.ReadType();
                            Creatures.Add(creature);
                        }
                    }
                }
                    break;
            }
        }
    }
}