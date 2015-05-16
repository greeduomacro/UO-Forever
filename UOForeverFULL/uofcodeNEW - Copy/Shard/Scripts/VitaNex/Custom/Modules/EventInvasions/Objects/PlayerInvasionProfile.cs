#region References

using System.Collections.Generic;
using Server.Mobiles;
using VitaNex;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Engines.EventInvasions
{
    public sealed class PlayerInvasionProfile : PropertyObject
    {
        [CommandProperty(EventInvasions.Access, true)]
        public PlayerMobile Owner { get; private set; }

        [CommandProperty(EventInvasions.Access, true)]
        public Dictionary<InvasionSerial, int> SpecificInvasionScores { get; private set; }

        [CommandProperty(EventInvasions.Access, true)]
        public int OverallScore { get; set; }

        public PlayerInvasionProfile(PlayerMobile pm)
        {
            Owner = pm;
            OverallScore = 0;

            SpecificInvasionScores = new Dictionary<InvasionSerial, int>();
        }

        public PlayerInvasionProfile(GenericReader reader)
            : base(reader)
        {}

        public override void Reset()
        {}

        public override void Clear()
        {}

        public void AddScore(int amount, Invasion invasion)
        {
            if (SpecificInvasionScores.ContainsKey(invasion.UID))
            {
                SpecificInvasionScores[invasion.UID] += amount;
            }
            else
            {
                SpecificInvasionScores.Add(invasion.UID, amount);
            }
            if (invasion.UID != null)
            {
                var scoregump = new InvasionScoreGump(Owner, SpecificInvasionScores[invasion.UID]).Send<InvasionScoreGump>();
            }
            OverallScore += amount;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(0);

            switch (version)
            {
                case 0:
                {
                    writer.WriteMobile(Owner);
                    writer.Write(OverallScore);

                    writer.Write(SpecificInvasionScores.Count);

                    if (SpecificInvasionScores.Count > 0)
                    {
                        foreach (KeyValuePair<InvasionSerial, int> kvp in SpecificInvasionScores)
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
            SpecificInvasionScores = new Dictionary<InvasionSerial, int>();

            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                {
                    Owner = reader.ReadMobile<PlayerMobile>();

                    OverallScore = reader.ReadInt();

                    int count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var p = new InvasionSerial(reader);
                            int amt = reader.ReadInt();
                            SpecificInvasionScores.Add(p, amt);
                        }
                    }
                }
                    break;
            }
        }
    }
}