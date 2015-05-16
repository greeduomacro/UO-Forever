#region References

using System;
using Server.Gumps;
using Server.Regions;
using Server.Spells;

#endregion

namespace Server.Ethics.Hero
{
    public sealed class HolyWord : Power
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Holy Word", "", 230);

        private DateTime m_rescheck;

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Lead)]
        public DateTime ResCheck { get { return m_rescheck; } set { m_rescheck = value; } }

        public HolyWord()
            : base(null, m_Info)
        {
            m_Definition = new PowerDefinition(
                100,
                "Holy Word",
                "Erst Oostrac",
                "Expend all of your life force to resurrect yourself.  Works once per day.",
                20742
                );
        }

        public HolyWord(Player Caster)
            : base(Caster.Mobile, m_Info)
        {
            m_Definition = new PowerDefinition(
                100,
                "Holy Word",
                "Erst Oostrac",
                "Expend all of your life force to resurrect yourself.  Works once per day.",
                20742
                );
            EthicCaster = Caster;
        }

        public override void OnCast()
        {
            if (!EthicCaster.Mobile.CheckAlive(false) && !(EthicCaster.Mobile.Region.IsPartOf(typeof(HouseRegion))) &&
                DateTime.UtcNow >= (m_rescheck + TimeSpan.FromHours(24)))
            {
                EthicCaster.Mobile.CloseGump(typeof(ResurrectGump));
                EthicCaster.Mobile.SendGump(new ResurrectGump(EthicCaster.Mobile, true));
                FinishInvoke(EthicCaster);
                m_rescheck = DateTime.UtcNow;
            }
            else if (EthicCaster.Mobile.CheckAlive(false))
            {
                EthicCaster.Mobile.SendMessage("You must be dead to use this spell.", 32);
            }
            else
            {
                EthicCaster.Mobile.SendMessage("You cannot use this spell inside a house.", 32);
            }

            FinishSequence();
        }

        public override void BeginInvoke(Player from)
        {
            new HolyWord(from).Cast();
        }
    }
}