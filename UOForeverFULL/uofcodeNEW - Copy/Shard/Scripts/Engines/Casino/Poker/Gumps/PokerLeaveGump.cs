#region References

using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
#endregion

namespace Server.Poker
{
	public class PokerLeaveGump : Gump
	{
		private readonly PokerGame _MGame;

		public PokerLeaveGump(PokerGame game)
			: base(50, 50)
		{
			_MGame = game;

            AddBackground(0, 0, 400, 165, 9270);
            AddAlphaRegion(115, 10, 165, 24);
            AddHtml(14, 14, 371, 24, String.Format("<CENTER><BIG><BASEFONT COLOR=#FFFFFF>{0}</BASEFONT></BIG></CENTER>", "Stand Up and Leave?"), false, false);
            AddImageTiled(20, 34, 347, 2, 9277);
            AddImageTiled(34, 36, 347, 2, 9277);

            AddHtml(20, 41, 365, 130, String.Format("<LEFT><BASEFONT COLOR=#F7D358>{0}</BASEFONT><</LEFT>", "Are you sure you want to cash-in and leave the poker table? You will play the current hand to completion and then any winnings will be deposited in your bank box. You will be unable to join another poker table for 60 seconds."), false, false);
            AddImageTiled(20, 115, 347, 2, 9277);
            AddImageTiled(34, 117, 347, 2, 9277);

            AddButton(250, 125, 247, 248, (int)Handlers.BtnOkay, GumpButtonType.Reply, 0);
            AddButton(320, 125, 242, 241, (int)Handlers.None, GumpButtonType.Reply, 0);
		}

		public enum Handlers
		{
			None,
			BtnOkay
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;

			if (from == null)
			{
				return;
			}

			PokerPlayer player = _MGame.GetPlayer(from);

			if (player != null)
			{
				if (info.ButtonID == 1)
				{
					if (_MGame.State == PokerGameState.Inactive)
					{
						if (_MGame.Players.Contains(player))
						{
							_MGame.RemovePlayer(player);
						}
						return;
					}

					if (player.RequestLeave)
					{
						from.SendMessage(0x22, "You have already submitted a request to leave.");
					}
					else
					{
						from.SendMessage(0x22, "You have submitted a request to leave the table.");
						player.RequestLeave = true;
					}
				}
			}
		}
	}
}