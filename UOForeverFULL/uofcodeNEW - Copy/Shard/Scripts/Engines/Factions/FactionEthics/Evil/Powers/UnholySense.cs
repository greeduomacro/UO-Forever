#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Factions;
using Server.Network;
using Server.Spells;

#endregion

namespace Server.Ethics.Evil
{
    public sealed class UnholySense : Power
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Unholy Sense", "", 230);

        public UnholySense()
            : base(null, m_Info)
        {
            m_Definition = new PowerDefinition(
                0,
                "Unholy Sense",
                "Drewrok Velgo",
                "Sense the presence of good near you.  Strength is based off your current life force.",
                20486
                );
        }

        public UnholySense(Player Caster)
            : base(Caster.Mobile, m_Info)
        {
            m_Definition = new PowerDefinition(
                0,
                "Unholy Sense",
                "Drewrok Velgo",
                "Sense the presence of good near you.  Strength is based off your current life force.",
                20486
                );
            EthicCaster = Caster;
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                //return TimeSpan.FromSeconds( ((m_Mount.IsDonationItem && RewardSystem.GetRewardLevel( m_Rider ) < 3)? 10.5 : 3.0) );
                return TimeSpan.FromSeconds(0.5);
            }
        }

        public override void BeginInvoke(Player from)
        {
            new UnholySense(from).Cast();
        }

        public override void OnCast()
        {
            var sensed = new List<Player>();

            var casterfaction = Faction.Find(EthicCaster.Mobile);

            int enemyCount = 0;

            int maxRange = 18 + EthicCaster.Power;

            Player primary = null;

            foreach (var faction in Faction.Factions.Where(x => x != casterfaction))
            {
                sensed.AddRange(faction.Members.Select(player => Player.Find(player.Mobile)));
            }

            foreach (var pl in sensed)
            {
                if (pl != null)
                {

                    Mobile mob = pl.Mobile;

                    if (mob == null || mob.Map != EthicCaster.Mobile.Map || !mob.Alive)
                    {
                        continue;
                    }

                    if (!mob.InRange(EthicCaster.Mobile, Math.Max(18, maxRange)))
                    {
                        continue;
                    }

                    if (primary == null || pl.Power > primary.Power)
                    {
                        primary = pl;
                    }

                    ++enemyCount;
                }
            }

            var sb = new StringBuilder();

            sb.Append("You sense ");
            sb.Append(enemyCount == 0 ? "no" : enemyCount.ToString());
            sb.Append(enemyCount == 1 ? " enemy" : " enemies");

            if (primary != null)
            {
                sb.Append(", and a strong presense");

                switch (EthicCaster.Mobile.GetDirectionTo(primary.Mobile))
                {
                    case Direction.West:
                        sb.Append(" to the west.");
                        break;
                    case Direction.East:
                        sb.Append(" to the east.");
                        break;
                    case Direction.North:
                        sb.Append(" to the north.");
                        break;
                    case Direction.South:
                        sb.Append(" to the south.");
                        break;

                    case Direction.Up:
                        sb.Append(" to the north-west.");
                        break;
                    case Direction.Down:
                        sb.Append(" to the south-east.");
                        break;
                    case Direction.Left:
                        sb.Append(" to the south-west.");
                        break;
                    case Direction.Right:
                        sb.Append(" to the north-east.");
                        break;
                }
            }
            else
            {
                sb.Append('.');
            }

            EthicCaster.Mobile.LocalOverheadMessage(MessageType.Regular, 0x59, false, sb.ToString());

            FinishInvoke(EthicCaster);

            FinishSequence();
        }
    }
}