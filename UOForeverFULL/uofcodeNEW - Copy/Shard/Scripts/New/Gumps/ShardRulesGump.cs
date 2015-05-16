// **********
// RunUO Shard - ShardRulesGump.cs
// **********

#region References

using Server.Accounting;
using Server.Network;

#endregion

namespace Server.Gumps
{
    public class ShardRulesGump : Gump
    {
        public ShardRulesGump() : base(0, 0)
        {
            Resizable = false;
            AddPage(0);
            AddBackground(76, 112, 600, 371, 9200);
            AddLabel(250, 127, 1193, @"UO Forever Terms of Service and Rules");
            AddImage(157, 117, 4373);
            AddImage(389, 119, 4354);
            AddImage(429, 319, 4769);
            AddImage(451, 306, 4770);
            AddImage(472, 306, 4771);
            AddHtml(88, 179, 500, 284, @" !! Any violation is subject to jailing, temporary, or permanent ban. !!


**********	
** Preface **
**********

- Shard will refer to the video game server itself including any and all 
content within.

- Company will refer to the parent organization and any subsidiaries or 
affiliates unless otherwise stated.

- Player will refer to a consumer of any content provided by the company, 
a customer, or volunteer of the company, unless otherwise stated.

- Server will refer to the physical machine(s) hosting the content 
provided by the company.

- Volunteer will refer to volunteers that are not monetarily compensated 
for their contributions to the server in a role other than a player.

- Staff will refer to any owner, employee, or volunteer.

- Administrator will refer to a volunteer, employee, or owner with the 
rank and responsibility to enforce the rules, conduct events, 
and managing volunteers.



***************
** General Legality **
***************
Illegal:

- Attempt to gain unauthorized access to other player's accounts, server 
hardware, source code, or data streams between the server and players.

- Impersonation of an owner, employee, volunteer, or another player.

- Trade, sell or buy accounts, items or other content of the shard 
including in-game gold, for real cash or other types of monetary 
compensation.

- Trade, sell or buy accounts, items or other content of the shard 
including in-game gold, for items or content on another shard or game.

- Forever will cooperate to the fullest extent of the laws in the 
United States of America and treaties with its allies throughout 
the world. The actions stated above are illegal by law throughout the 
United States of America and many countries throughout the world. 
Forever will seek legal action where obligated by law or 
by legal liability.	



***********
** Accounts **
***********

- Each player is limited to two (2) accounts. This is controlled by IP 
address. It is possible to acquire more accounts, therefore an 
administrator maintains the right to choose for the player which 
accounts that player may keep and which they may not.

- Each player is allowed to log into two (2) accounts at one time. This 
is controlled by IP address. It is possible for a player to be logged 
into multiple accounts from multiple locations, therefore an 
administrator maintains the right to disconnect excessive accounts that 
are logged in. You MAY NOT multi-box PvP!

- Players are NOT allowed to share accounts. This is tracked by IP 
address.If a player is subject to punishment, all shared accounts are 
considered as part of the violation. All members of a household or single 
connection are therefore subject to punishment even if they are not 
technically shared.

- Players may own one (1) house per account. All extra houses will be 
deleted at the discretion of an administrator.	Players given permission
to have a (3) accounts may only have a total of 2 houses!


***********************
** Player Interaction with Staff **
***********************

- Staff will not replace items. This includes scenarios of server 
malfunction, scamming, or bugs.

- Staff will not move a player. The only exception is in the case of a 
bug or problem with the shard.

- Players may not interfere with a staff member aiding another player. 
This is categorized as harassment.

- Players may not page in to a staff member for casual conversation, 
escalate a situation, or harass a staff member. All complaints should be 
forwarded to the owners of the company.



********
** Bugs **
********

- Players are obligated and required to report any and all bugs 
experienced in the game.

- Players may not abuse a bug whether it is to gain an advantage or 
otherwise. Unknowingly abusing a bug is not an excuse.



*************************************
** UO Client Version, Macroing & 3rd Party Programs **
*************************************

- No programs are allowed to interact or alter the UO client or other 
related files unless otherwise stated in this section of the rules. 
This includes 3rd party programs such that indirectly affect UO such as 
programs which speed up the operating system or general cheating/
rootkit/debugging programs.

- Players are required to use an unaltered installation of UO updated 
to at least v7.0.15.1. The company reserves the right to change this 
requirement without notice as necessary to prevent the usage of 
unauthorized 3rd party tools. Players that use an older client or 
altered installation may be subject to punishment by the administrators. 
Substandard computer hardware or an inadequate internet connection is 
not an excuse.

- Players may use an unaltered installation of the newest version of 
Razor. This is not a requirement.

- Players may NOT use EasyUO or any program like it!

- Players may create and use macros using these authorized 3rd party 
programs unless otherwise stated in this section.



****************************************
** Players may NOT use Razor or UOSteam for the following **
****************************************

- Looting corpses or containers.

- Gathering resources including but not limited to gold without being 
present at the computer.

- Buying or selling items to NPC vendors without being present at 
the computer.

- Taming animals without being present at the computer.

- PVP or PVE (Killing players or NPCs) without interaction with UO by 
the player. This includes ambushing players with multiple instances 
of the game.

- Using macros or scripts to perform an action quicker than other 
players can conceivably react. This includes stealing items from 
cancelled trades or other methods of scamming a player.

- Exploiting game mechanics or bugs which would otherwise not 
be available.



*********
** Events **
*********

- Events are conducted at the discretion of the administrators 
regardless of history, or holidays.

- Administrators attempt to hold events at the most opportune time for 
all players. This is not guaranteed.

- Events are limited to one (1) player per IP address. Players found 
attempting to abuse this will be removed from the event and any handouts 
or prizes will be removed.



************
** Harassment **
************

- Players may not degrade the quality of the experience of any player or 
staff member. This includes verbal insults, exploiting bugs, or other 
behavior which may be deemed as harassment by the administrators.

- Players may not name anything inappropriately. This includes, but is 
not limited to, in-game players' names, pets' names, guilds' names, or 
houses' names.

- Players may not excessively spam other players or staff. This is not 
monitored by staff however will be strictly enforced in areas that have 
signs indicating that there is no spamming in the area.

- Players may not advertise other shards, websites, or video games.", true, true);


            AddButton(595, 449, 247, 248, 0, GumpButtonType.Reply, 0);
        }
    }

    public class BasicShardRulesGump : Gump
    {
        public const string AccountTagName = "rules";

        public BasicShardRulesGump()
            : base(0, 0)
        {
            Resizable = false;
            Closable = false;
            Dragable = false;

            AddPage(0);
            AddBackground(76, 100, 600, 580, 9200);
            AddLabel(250, 115, 1193, @"UO Forever's Rules:");
            AddImage(157, 105, 4373);
            AddImage(389, 107, 4354);
            AddImage(429, 307, 4769);
            AddImage(451, 294, 4770);
            AddImage(472, 294, 4771);
            AddHtml(88, 133, 500, 530, @"Welcome to UOForever! Just so there can be no misunderstanding,
you must agree to UOForever's rules to continue! There are some
actions that we have ZERO tolerance for!

By playing UOForever, you agree to NEVER gather resources
AFK (except fishing), to NEVER use unapproved 3rd party programs
or anything that enables faster movement (i.e. speedhack), and
to NEVER use more than 2 accounts or share access to your accounts
with others.

You understand that:
AFK resource gathering (except fishing) will result in an
instant permanent ban WITHOUT EXCEPTION.

Using any 3rd party programs except Razor or UOSteam will 
result in an instant permanent ban WITHOUT EXCEPTION.

Speedhacking of any kind at ANY time will result in an 
instant permanent ban WITHOUT EXCEPTION.

Players found to be using more than 2 accounts will have extra
accounts removed at staff discretion and may be subject
to be banned! If you accidentally create a 3rd account,
just don't create a character and it's not a problem.

We do not allow sharing accounts!

For all rules please see http://uoforever.com/rules.php", true, false);

            AddButton(595, 590, 247, 248, 0, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
// ReSharper disable once RedundantComparisonWithNull
            if (state.Account != null && state.Account is Account)
            {
                Account acct = (Account) state.Account;
                acct.SetTag(AccountTagName, "y");
            }
            state.Mobile.CloseGump(GetType());
        }
    }

    public class YoungPlayerGump : Gump
    {
        public YoungPlayerGump()
            : base(0, 0)
        {
            Resizable = false;
            Closable = true;

            AddPage(0);
            AddBackground(76, 100, 600, 270, 9200);
            AddLabel(250, 115, 1193, @"Young Player Benefits");
            AddImage(157, 105, 4373);
            AddImage(389, 107, 4354);
            AddImage(429, 307, 4769);
            AddImage(451, 294, 4770);
            AddImage(472, 294, 4771);
            AddHtml(88, 133, 500, 230, @"Welcome to UO Forever! You have young player status... this means:
- after dying and resurrecting, you keep everything in your bag
  (normally it would drop on your body)
- you have access to the new player dungeon (just southwest of brit bank)
- special volunteers called 'Companions' may appear and assist you

Be aware that Young player status lasts 9 hours of GAMEPLAY or
until you gain enough skills. This applies to all characters 
on your account.

The new player tour is HIGHLY recommended (say 'help' to the greeter)
You may renounce these benefits by saying 'I renounce my young player status'
", true, false);

            AddButton(595, 340, 247, 248, 0, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            state.Mobile.CloseGump(GetType());
        }
    }
}