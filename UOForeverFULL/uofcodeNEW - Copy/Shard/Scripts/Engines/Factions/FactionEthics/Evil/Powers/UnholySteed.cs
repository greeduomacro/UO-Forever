#region References

using System;
using Server.Mobiles;
using Server.Network;
using Server.Spells;

#endregion

namespace Server.Ethics.Evil
{
    public sealed class UnholySteedPower : Power
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Unholy Steed", "", 230);

        public UnholySteedPower()
            : base(null, m_Info)
        {
            m_Definition = new PowerDefinition(
                50,
                "Unholy Steed",
                "Xen Yeliab",
                "Summon an unholy steed.",
                20997
                );
        }

        public UnholySteedPower(Player Caster)
            : base(Caster.Mobile, m_Info)
        {
            m_Definition = new PowerDefinition(
                50,
                "Unholy Steed",
                "Xen Yeliab",
                "Summon an unholy steed.",
                20997
                );
            EthicCaster = Caster;
        }

        public override void BeginInvoke(Player from)
        {
            new UnholySteedPower(from).Cast();
        }

        public override void OnCast()
        {
            if (EthicCaster.Steed != null && EthicCaster.Steed.Deleted)
            {
                EthicCaster.Steed = null;
            }

            if (EthicCaster.Steed != null)
            {
                EthicCaster.Mobile.LocalOverheadMessage(MessageType.Regular, 0x3B2, false,
                    "You already have an unholy steed.");
            }
            else if ((EthicCaster.Mobile.Followers + 1) > EthicCaster.Mobile.FollowersMax)
            {
                EthicCaster.Mobile.SendLocalizedMessage(1049645);
                // You have too many followers to summon that creature.
            }
            else
            {
                var steed = new UnholySteed();

                if (BaseCreature.Summon(steed, EthicCaster.Mobile, EthicCaster.Mobile.Location, 0x217,
                    TimeSpan.FromDays(1.0)))
                {
                    EthicCaster.Steed = steed;

                    FinishInvoke(EthicCaster);
                }
            }
            FinishSequence();
        }
    }
}