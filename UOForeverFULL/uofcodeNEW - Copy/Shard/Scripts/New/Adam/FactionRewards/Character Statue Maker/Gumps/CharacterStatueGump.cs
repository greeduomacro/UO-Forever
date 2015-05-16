#region References

using Server.Mobiles;
using Server.Network;

#endregion

namespace Server.Gumps
{
    public class CharacterStatueGump : Gump
    {
        private Item m_Maker;
        private CharacterStatue m_Statue;
        private Mobile m_Owner;

        private enum Buttons
        {
            Close,
            Sculpt,
            PosePrev,
            PoseNext,
            DirPrev,
            DirNext,
            Restore
        }

        public CharacterStatueGump(Item maker, CharacterStatue statue, Mobile owner)
            : base(60, 36)
        {
            m_Maker = maker;
            m_Statue = statue;
            m_Owner = owner;

            if (m_Statue == null)
            {
                return;
            }

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            AddBackground(0, 0, 327, 324, 0x13BE);
            AddImageTiled(10, 10, 307, 20, 0xA40);
            AddImageTiled(10, 40, 307, 244, 0xA40);
            AddImageTiled(10, 294, 307, 20, 0xA40);
            AddAlphaRegion(10, 10, 307, 304);
            AddHtmlLocalized(14, 12, 327, 20, 1076156, 0x7FFF, false, false); // Character Statue Maker

            // pose
            AddHtmlLocalized(133, 41, 120, 20, 1076168, 0x7FFF, false, false); // Choose Pose
            AddHtmlLocalized(133, 61, 120, 20, 1076208 + (int) m_Statue.Pose, 0x77E, false, false);
            AddButton(163, 81, 0xFA5, 0xFA7, (int) Buttons.PoseNext, GumpButtonType.Reply, 0);
            AddButton(133, 81, 0xFAE, 0xFB0, (int) Buttons.PosePrev, GumpButtonType.Reply, 0);

            // direction
            AddHtmlLocalized(133, 126, 120, 20, 1076170, 0x7FFF, false, false); // Choose Direction
            AddHtmlLocalized(133, 146, 120, 20, GetDirectionNumber(m_Statue.Direction), 0x77E, false, false);
            AddButton(163, 167, 0xFA5, 0xFA7, (int) Buttons.DirNext, GumpButtonType.Reply, 0);
            AddButton(133, 167, 0xFAE, 0xFB0, (int) Buttons.DirPrev, GumpButtonType.Reply, 0);

            // cancel
            AddButton(10, 294, 0xFB1, 0xFB2, (int) Buttons.Close, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 294, 80, 20, 1006045, 0x7FFF, false, false); // Cancel

            // sculpt
            AddButton(234, 294, 0xFB7, 0xFB9, (int) Buttons.Sculpt, GumpButtonType.Reply, 0);
            AddHtmlLocalized(269, 294, 80, 20, 1076174, 0x7FFF, false, false); // Sculpt

            // restore
            if (m_Maker is CharacterStatueDeed)
            {
                AddButton(107, 294, 0xFAB, 0xFAD, (int) Buttons.Restore, GumpButtonType.Reply, 0);
                AddHtmlLocalized(142, 294, 80, 20, 1076193, 0x7FFF, false, false); // Restore
            }
        }

        private int GetDirectionNumber(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return 1075389;
                case Direction.Right:
                    return 1075388;
                case Direction.East:
                    return 1075387;
                case Direction.Down:
                    return 1076204;
                case Direction.South:
                    return 1075386;
                case Direction.Left:
                    return 1075391;
                case Direction.West:
                    return 1075390;
                case Direction.Up:
                    return 1076205;
                default:
                    return 1075386;
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_Statue == null || m_Statue.Deleted)
            {
                return;
            }

            bool sendGump = false;

            if (info.ButtonID == (int) Buttons.Sculpt)
            {
                if (m_Maker is CharacterStatueDeed)
                {
                    CharacterStatue backup = ((CharacterStatueDeed) m_Maker).Statue;

                    if (backup != null)
                    {
                        backup.Delete();
                    }
                }

                if (m_Maker != null)
                {
                    m_Maker.Delete();
                }

                m_Statue.Sculpt(state.Mobile);
            }
            else if (info.ButtonID == (int) Buttons.PosePrev)
            {
                m_Statue.Pose = (StatuePose) (((int) m_Statue.Pose + 5) % 6);
                m_Statue.InvalidatePose();
                sendGump = true;
            }
            else if (info.ButtonID == (int) Buttons.PoseNext)
            {
                m_Statue.Pose = (StatuePose) (((int) m_Statue.Pose + 1) % 6);
                m_Statue.InvalidatePose();
                sendGump = true;
            }
            else if (info.ButtonID == (int) Buttons.DirPrev)
            {
                m_Statue.Direction = (Direction) (((int) m_Statue.Direction + 7) % 8);
                m_Statue.InvalidatePose();
                sendGump = true;
            }
            else if (info.ButtonID == (int) Buttons.DirNext)
            {
                m_Statue.Direction = (Direction) (((int) m_Statue.Direction + 1) % 8);
                m_Statue.InvalidatePose();
                sendGump = true;
            }
            else if (info.ButtonID == (int) Buttons.Restore)
            {
                if (m_Maker is CharacterStatueDeed)
                {
                    CharacterStatue backup = ((CharacterStatueDeed) m_Maker).Statue;

                    if (backup != null)
                    {
                        m_Statue.Restore(backup);
                    }
                }

                sendGump = true;
            }
            else // Close
            {
                sendGump = !m_Statue.Demolish(state.Mobile);
            }

            if (sendGump)
            {
                state.Mobile.SendGump(new CharacterStatueGump(m_Maker, m_Statue, m_Owner));
            }
        }
    }
}