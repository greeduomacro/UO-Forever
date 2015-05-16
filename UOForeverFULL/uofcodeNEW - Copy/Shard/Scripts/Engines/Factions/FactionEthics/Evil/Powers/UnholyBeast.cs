#region References

using System;
using Server.Mobiles;
using Server.Network;
using Server.Spells;

#endregion

namespace Server.Ethics.Evil
{
    public sealed class SummonFamiliar : Power
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Unholy Beast", "", 230);

        public SummonFamiliar()
            : base(null, m_Info)
        {
            m_Definition = new PowerDefinition(
                25,
                "Unholy Beast",
                "Xen Vingir",
                "Summon an unholy beast to aid you in combat.",
                20491
                );
        }

        public SummonFamiliar(Player Caster)
            : base(Caster.Mobile, m_Info)
        {
            m_Definition = new PowerDefinition(
                25,
                "Unholy Beast",
                "Xen Vingir",
                "Summon an unholy beast to aid you in combat.",
                20491
                );
            EthicCaster = Caster;
        }

        public override void OnCast()
        {
            if (EthicCaster.Familiar != null && EthicCaster.Familiar.Deleted)
            {
                EthicCaster.Familiar = null;
            }

            if (EthicCaster.Familiar != null)
            {
                EthicCaster.Mobile.LocalOverheadMessage(MessageType.Regular, 0x3B2, false,
                    "You already have an unholy familiar.");
                return;
            }

            if ((EthicCaster.Mobile.Followers + 1) > EthicCaster.Mobile.FollowersMax)
            {
                EthicCaster.Mobile.SendLocalizedMessage(1049645);
                // You have too many followers to summon that creature.
                return;
            }

            var familiar = new UnholyFamiliar();

            if (BaseCreature.Summon(familiar, EthicCaster.Mobile, EthicCaster.Mobile.Location, 0x217,
                TimeSpan.FromHours(1.0)))
            {
                familiar.Hue = 1175;
                EthicCaster.Familiar = familiar;

                FinishInvoke(EthicCaster);
            }

            FinishSequence();
        }

        public override void BeginInvoke(Player from)
        {
            new SummonFamiliar(from).Cast();
        }
    }
}