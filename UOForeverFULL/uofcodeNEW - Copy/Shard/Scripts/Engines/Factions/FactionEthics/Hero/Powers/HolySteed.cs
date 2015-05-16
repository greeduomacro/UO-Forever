#region References

using System;
using Server.Mobiles;
using Server.Network;
using Server.Spells;

#endregion

namespace Server.Ethics.Hero
{
    public sealed class HolySteedPower : Power
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Holy Steed", "", 230);

        public HolySteedPower()
            : base(null, m_Info)
        {
            m_Definition = new PowerDefinition(
                50,
                "Holy Steed",
                "Xen Yeliab",
                "Summon a holy steed.",
                20997
                );
        }

        public HolySteedPower(Player Caster)
            : base(Caster.Mobile, m_Info)
        {
            m_Definition = new PowerDefinition(
                50,
                "Holy Steed",
                "Xen Yeliab",
                "Summon a holy steed.",
                20997
                );
            EthicCaster = Caster;
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
                    "You already have a holy steed.");
            }
            else if ((EthicCaster.Mobile.Followers + 1) > EthicCaster.Mobile.FollowersMax)
            {
                EthicCaster.Mobile.SendLocalizedMessage(1049645);
                    // You have too many followers to summon that creature.
            }
            else
            {
                var steed = new HolySteed();

                if (BaseCreature.Summon(steed, EthicCaster.Mobile, EthicCaster.Mobile.Location, 0x217,
                    TimeSpan.FromDays(1.0)))
                {
                    EthicCaster.Steed = steed;

                    FinishInvoke(EthicCaster);
                }
            }

            FinishSequence();
        }

        public override void BeginInvoke(Player from)
        {
            new HolySteedPower(from).Cast();
        }
    }
}