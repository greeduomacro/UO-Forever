#region References

using System;
using Server.Mobiles;
using Server.Network;
using Server.Spells;

#endregion

namespace Server.Ethics.Hero
{
    public sealed class SummonFamiliar : Power
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Holy Beast", "", 230);

        public SummonFamiliar()
            : base(null, m_Info)
        {
            m_Definition = new PowerDefinition(
                25,
                "Holy Beast",
                "Xen Vingir",
                "Summon a holy beast to aid you in combat.",
                20491
                );
        }

        public SummonFamiliar(Player Caster)
            : base(Caster.Mobile, m_Info)
        {
            m_Definition = new PowerDefinition(
                25,
                "Holy Beast",
                "Xen Vingir",
                "Summon a holy beast to aid you in combat.",
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
                    "You already have a holy familiar.");
                FinishSequence();
                return;
            }

            if ((EthicCaster.Mobile.Followers + 1) > EthicCaster.Mobile.FollowersMax)
            {
                EthicCaster.Mobile.SendLocalizedMessage(1049645);
                    // You have too many followers to summon that creature.
                FinishSequence();
                return;
            }

            var familiar = new HolyFamiliar();

            if (BaseCreature.Summon(familiar, EthicCaster.Mobile, EthicCaster.Mobile.Location, 0x217,
                TimeSpan.FromHours(1.0)))
            {
                familiar.Hue = 2955;
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