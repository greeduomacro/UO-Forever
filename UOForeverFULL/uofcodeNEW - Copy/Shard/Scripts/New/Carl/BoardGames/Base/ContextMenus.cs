#region References

using Server;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;

#endregion

namespace Solaris.BoardGames
{
    public class ViewBoardGameScoresEntry : ContextMenuEntry
    {
        private Mobile _From;
        private BoardGameControlItem _ControlItem;

        //3006239 = "View events"
        public ViewBoardGameScoresEntry(Mobile from, BoardGameControlItem controlitem, int index) : base(6239, index)
        {
            _From = from;
            _ControlItem = controlitem;
        }

        public override void OnClick()
        {
            if (_ControlItem == null || _ControlItem.Deleted)
            {
                return;
            }

            _From.SendGump(new BoardGameScoresGump(_From, _ControlItem));
        }
    }

    public class ResetBoardGameScoresEntry : ContextMenuEntry
    {
        private Mobile _From;
        private BoardGameControlItem _ControlItem;

        //3006162 = "Reset Game"
        public ResetBoardGameScoresEntry(Mobile from, BoardGameControlItem controlitem, int index) : base(6162, index)
        {
            _From = from;
            _ControlItem = controlitem;
        }

        public override void OnClick()
        {
            if (_ControlItem == null || _ControlItem.Deleted)
            {
                return;
            }

            _From.SendGump(new ConfirmResetGameScoreGump(_From, _ControlItem));
        }
    }
}