using System;
using Server;

namespace Server.Gumps
{
	public class ShardChatGump : Gump
	{
		public ShardChatGump() : base( 0, 0 )
		{
			Resizable = false;
			AddPage(0);
			AddBackground(76, 112, 600, 371, 9200);
			AddLabel(250, 127, 1193, @"UOF Terms of Chat Rules");
			AddImage(157, 117, 4373);
			AddImage(389, 119, 4354);
			AddImage(429, 319, 4769);
			AddImage(451, 306, 4770);
			AddImage(472, 306, 4771);
			AddHtml( 88, 179, 500, 284, @" !! Any violation is subject to jailing, temporary, or permanent ban. !!

The following will be considered a breach of the rules.

*Persistant insulting of other players*
*Racism*
*Spam, including spaming other players*
*Harrasment of players and staff*

We understand that the use of foul language is widley used on freeshards but please try to keep it to a minimum.
.", true, true);

			this.AddButton(595, 449, 247, 248, 0, GumpButtonType.Reply, 0);
		}
	}
}