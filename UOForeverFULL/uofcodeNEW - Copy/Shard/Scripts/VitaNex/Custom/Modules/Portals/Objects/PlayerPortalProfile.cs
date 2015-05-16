#region References

using System.Collections.Generic;
using Server.Mobiles;
using VitaNex;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Engines.Portals
{
    public sealed class PlayerPortalProfile : PropertyObject
    {
        [CommandProperty(Portals.Access, true)]
        public PlayerMobile Owner { get; private set; }

        [CommandProperty(Portals.Access, true)]
        public Dictionary<PortalSerial, int> SpecificPortalScores { get; private set; }

        [CommandProperty(Portals.Access, true)]
        public int OverallScore { get; set; }

        public PlayerPortalProfile(PlayerMobile pm)
        {
            Owner = pm;
            OverallScore = 0;

            SpecificPortalScores = new Dictionary<PortalSerial, int>();
        }

        public PlayerPortalProfile(GenericReader reader)
            : base(reader)
        {}

        public override void Reset()
        {}

        public override void Clear()
        {}

        public void AddScore(int amount, Portal portal)
        {
            if (SpecificPortalScores.ContainsKey(portal.UID))
            {
                SpecificPortalScores[portal.UID] += amount;
            }
            else
            {
                SpecificPortalScores.Add(portal.UID, amount);
            }
            if (portal.UID != null && CentralGump.CentralGump.EnsureProfile(Owner).MiniGump)
            {
                var scoregump = new PortalScoreGump(Owner, SpecificPortalScores[portal.UID]).Send<PortalScoreGump>();
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

                    writer.Write(SpecificPortalScores.Count);

                    if (SpecificPortalScores.Count > 0)
                    {
                        foreach (KeyValuePair<PortalSerial, int> kvp in SpecificPortalScores)
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
            SpecificPortalScores = new Dictionary<PortalSerial, int>();

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
                            var p = new PortalSerial(reader);
                            int amt = reader.ReadInt();
                            SpecificPortalScores.Add(p, amt);
                        }
                    }
                }
                    break;
            }
        }
    }
}