using Server.Engines.Conquests;
using Server.Mobiles;

namespace Server.ContextMenus
{
    public class ConquestContextEntry : ContextMenuEntry
    {
        private Mobile m_From;
        private Mobile m_Target;

        public ConquestContextEntry(Mobile from, Mobile target)
            : base(6121)
        {
            m_From = from;
            m_Target = target;
        }

        public override void OnClick()
        {
            if (m_From is PlayerMobile && m_Target is PlayerMobile)
            {
                Conquests.SendConquestsGump(m_From as PlayerMobile, m_Target as PlayerMobile);
            }
        }
    }
}