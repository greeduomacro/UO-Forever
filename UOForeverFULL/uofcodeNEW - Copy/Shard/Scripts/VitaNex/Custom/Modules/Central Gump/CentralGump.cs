#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Server.HuePickers;
using Server.Mobiles;
using VitaNex.IO;
using VitaNex.SuperGumps;

#endregion

namespace Server.Engines.CentralGump
{
    public static partial class CentralGump
    {
        public const AccessLevel Access = AccessLevel.EventMaster;

        public static CentralGumpOptions CSOptions { get; private set; }

        public static BinaryDataStore<PlayerMobile, CentralGumpProfile> PlayerProfiles { get; private set; }

        public static XmlDataStore<String, Message> Messages { get; private set; }

        public static List<PlayerMobile> Adventurers { get; private set; }

        public static CentralGumpProfile EnsureProfile(PlayerMobile pm)
        {
            if (pm == null)
                return null;

            CentralGumpProfile p;

            if (!PlayerProfiles.TryGetValue(pm, out p))
            {
                PlayerProfiles.Add(pm, p = new CentralGumpProfile(pm));
            }
            else if (p == null)
            {
                PlayerProfiles[pm] = p = new CentralGumpProfile(pm);
            }

            return p;
        }

        public static Message GetMostRecentMessage()
        {
            return GetSortedMessages().FirstOrDefault(m => m.Published);
        }

        public static List<Message> GetSortedMessages()
        {
            return Messages.Values.OrderByDescending(m => m.Date.Stamp).ToList();
        }

        public static void SendCentralGump(this PlayerMobile user)
        {
            if (user != null && !user.Deleted && user.NetState != null)
            {
                SuperGump.Send(new CentralGumpUI(user, EnsureProfile(user), CentralGumpType.News));
            }
        }

        public static void CloseCentralGump(this PlayerMobile user)
        {
            foreach (CentralGumpUI g in SuperGump.GetInstances<CentralGumpUI>(user))
            {
                g.Close(true);
            }
        }

        private static void OnVirtueGumpRequest(VirtueGumpRequestEventArgs e)
        {
            if (e.Beheld != null && e.Beheld == e.Beholder)
            {
                SendCentralGump(e.Beheld as PlayerMobile);
            }
        }

        #region Gump info strings

        public static string ConstructYoungBenefitsString()
        {
            var str = new StringBuilder();

            str.Append("<CENTER><BIG>Welcome to the UO Forever Young Player Program</BIG></CENTER>\n");

            str.Append(
                "   You have begun your journey in Britain, the central hub of Britannia and the seat of Lord British.  Lord British has offered you his protection as a new player to his realm.  This protection period will offer you extra safety while learning the basics of Ultima Online Forever.\n\n");

            str.Append("Ultima Online Forever Young Player Program Details\n");

            str.Append("* You are protected from aggressive actions and cannot\n  steal from other players.\n");

            str.Append(
                "* Your possessions will not fall to your corpse upon death as\n  long as you have young status.\n");

            str.Append("* Monsters will not take aggressive action against you while you\n  have young player status.\n");

            str.Append("* You are able to teleport to both the New Player Dungeon and\n  Britain.\n");

            str.Append("* Young players are immune to poison.\n");

            str.Append(
                "* Young players are able to page companions in-game to assist\n  them with general questions or to go adventuring together.\n");

            str.Append(
                "* Young players are able to take part in the young player quest\n  located in the New Player Dungeon.\n* This quest will allow them to obtain the Candle of \n  Forgiveness and the Ring of Forgiveness.\n");

            str.Append(
                "* Young players will be offered to join the New Player Guild\n  [NEW] upon their first login.  [NEW] is a place for both \n  veteran and new players alike to adventure together.\n");

            str.Append("* You will be unable to loot kills you did not participate in as a   young player.\n");

            str.Append(
                "* Taking part in IDOCs while a young player is illegal on UO \n  Forever.  If you are found to be doing this, your young status   will be taken away.\n\n\n");

            str.Append(
                "The young player program is constantly evolving.  If you have any suggestions or comments, please feel free to contact a staff member!\n");
            return str.ToString();
        }

        public static string ConstructYoungLeaveString()
        {
            var str = new StringBuilder();

            str.Append("<CENTER><BIG>Leaving the Young Program</BIG></CENTER>\n");

            str.Append(
                "   There are several conditions that, when met, will remove you from the Young Player Program.\n\n");

            str.Append("* Obtaining a total skill point level of 600.\n");

            str.Append("* Reaching 60 hours total play time.\n");

            str.Append("* Killing another player.\n");

            str.Append(
                "* Taking part in an IDOC.  You will be warned before your \n  young status is officially removed.\n");

            str.Append("* Saying, \"I renounce my young player status\"\n");

            str.Append("* Leaving voluntarily via the Young Player portion of the central   gump.\n\n");

            str.Append(
                "Once you have left the Young Player Program, the only way to rejoin is by contacting a staff member.\n\n");

            str.Append(
                "The young player program is constantly evolving.  If you have any suggestions or comments, please feel free to contact a staff member about them!\n");
            return str.ToString();
        }

        public static string ConstructYoungDungeonString()
        {
            var str = new StringBuilder();

            str.Append("<CENTER><BIG>Young Player Dungeon - The Defiled Crypt</BIG></CENTER>\n");

            str.Append(
                "   The Defiled Crypt is Ultima Online Forever's Young Player dungeon.  Within the walls of this crypt, you will find various monsters of low level quality.  These monsters include: graver robbers, zombies, liches, wraths and other various low level undead creatures.\n\n");

            str.Append(
                "   Young players will be able to explore this dungeon in relative safety as the monsters here will not engage in aggressive actions.  If you perish within this dungeon, fear not, for a healer is located at the entrance of the crypt for your convenience.  Furthermore, as a young player, you will retain all of your items upon death.\n\n");

            str.Append(
                "- To begin combat, hit ALT+C to toggle into war mode.  You \n  can also press tab to toggle war mode.\n");

            str.Append("- War mode is indicated by a red cursor.\n");

            str.Append(
                "- To configure your tab key and other key binds, press ALT+O \n  to bring up your options menu and then press the tab\n  indicated with a mouse icon.\n");

            str.Append("- Whilst in war mode, double click on a monster to engage in \n  melee combat with it.\n");

            str.Append("- Don't forget, there are other forms of combat such as \n  magery or archery!\n");

            str.Append(
                "- A quest can be initiated via a lost spirit NPC in The Defiled    Crypt.  To initiate the quest, double click the NPC.\n");

            str.Append(
                "- The NPC requires you to kill 10 grave robbers and then to \n  lead her back to her family's crypt located in the east\n  portion of The Defiled Crypt.\n");

            str.Append(
                "- Upon completing the quest, you will be rewarded with a Candle \n  of Forgiveness and a Ring of Forgiveness.\n");

            str.Append(
                "- The Ring of Forgiveness will allow you to teleport to a healer \n  once per day as a ghost.  To activate the ring, speak \"I seek   forgiveness\" as a ghost.\n");
            return str.ToString();
        }

        public static string ConstructBankString()
        {
            var str = new StringBuilder();

            str.Append("<CENTER><BIG>The Banking System</BIG></CENTER>\n");

            str.Append(
                "   Banks are found in each town of Britannia and provide players secure storage for their goods.  Each player can store a total of 125 items in their bank.  Banks are global and each bank will allow you access to the same bankbox.  Players commonly store runebooks, valuables and other heavy goods in their bank.\n\n");

            str.Append("- To access your bank, say \"Bank\" near a banker NPC.\n");

            str.Append("- To see your gold balance, say \"Balance\" near a banker NPC.\n");

            str.Append(
                "- To create a bank check, say \"Check ###\" near a banker npc   where ### is the amount you wish to create a check for.\n");

            str.Append("- To redeem a bank check, double click it while it is in your\n  bank box.\n");

            str.Append("- To buy items from a banker, say vendor buy while near the \n  NPC.\n");

            str.Append(
                "- UO Forever also has a credit system.  To deposit all current\n  checks and gold in to credit form, say credit or deposit while\n  near a banker.  This will cause all gold and checks to be\n  removed from the bank and stored in credit.\n");

            str.Append(
                "- To withdraw from your credit balance, say \"Withdraw ###\" \n  near a banker npc where ### is the amount you wish to\n  withdraw.\n\n");

            str.Append("   Bankers also sell other useful goods such as vendor contracts and commodity deeds.\n");

            return str.ToString();
        }

        public static string ConstructBardicGuildString()
        {
            var str = new StringBuilder();

            str.Append("<CENTER><BIG>The Bardic System</BIG></CENTER>\n");

            str.Append(
                "   The Bardic guild will allow a player to train in the basics of being a bard along with selling basic instruments . The Bard can train you in peacemaking, discordance, provoking, musicianship and archery. The guildmaster can also train you in swordsmanship.\n\n");

            str.Append("- To buy from the vendors say \"vendor buy\" \n");

            str.Append("- To sell items to the vendors say \"vendor sell\" \n");

            str.Append("- To See which skills a vendor can train say \"name train\" \n");

            str.Append("- To ask the vendor to train a skill say \"name train skill\" \n");

            str.Append("  Where name is the vendors name and skill is the skill to \n  train. \n");

            str.Append("- To learn more about Archery, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            str.Append("- To learn more about Discordance, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            str.Append("- To learn more about Musicianship, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            str.Append("- To see a list of monsters barding difficulties, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            str.Append("- To learn more about Peacemaking, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            str.Append("- To learn more about Provoking, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            str.Append("- To learn more about Swordsmanship, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            return str.ToString();
        }

        public static string ConstructBlacksmithGuildString()
        {
            var str = new StringBuilder();

            str.Append("<CENTER><BIG>The Blacksmith System</BIG></CENTER>\n");

            str.Append(
                "   Blacksmith shops allow players to purchase weapons and armor. The Armorer and Weaponsmith can also train you in the Arms Lore and Blacksmithy skills.\n\n");

            str.Append("- To buy from the vendors say \"vendor buy\" \n");

            str.Append("- To sell items to the vendors say \"vendor sell\" \n");

            str.Append("- To See which skills a vendor can train say \"name train\" \n");

            str.Append("- To ask the vendor to train a skill say \"name train skill\" \n");

            str.Append("  Where name is the vendors name and skill is the skill to \n  train. \n");

            str.Append("- To learn more about Blacksmithing, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            str.Append("- To learn more about Arms Lore, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            str.Append(
                "  \nOnce you have reached 70 Blacksmithing you can start to receive bulk order by selling items or saying \"order status\" \n\n");

            str.Append("- For information about bulk order deeds, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a> \n");

            str.Append("- To see the rewards available for smith bulk orders, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a> \n");

            return str.ToString();
        }

        public static string ConstructLumberjackGuildString()
        {
            var str = new StringBuilder();

            str.Append("<CENTER><BIG>The Lumberjack Guild</BIG></CENTER>\n");

            str.Append(
                "   In the woodworkers shop you will find the architect, carpenter and the real estate broker. The carpenter sells various carpentry supplies. He can also teach you about lumberjacking and carpentry.\n\n");

            str.Append("- To buy from the vendors say \"vendor buy\" \n");

            str.Append("- To sell items to the vendors say \"vendor sell\" \n");

            str.Append("- To See which skills a vendor can train say \"name train\" \n");

            str.Append("- To ask the vendor to train a skill say \"name train skill\" \n");

            str.Append("  Where name is the vendors name and skill is the skill to\n  train. \n\n");

            str.Append(
                "   To harvest logs for carpentry you will need an axe and a tree. Double click the axe and target the nearest tree. Once you have harvested some logs, use the axe on them to turn the logs into boards. \n\n");

            str.Append("- To learn more about Carpentry, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a> \n");

            str.Append("- To learn more about Lumberjacking, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a> \n");

            str.Append("- To learn more about Carpenter trade, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a> \n");

            str.Append(
                "   \nThe architect will sell players house deeds that allow you to place your own private house. \n\n");

            str.Append("- House deeds are NOT newbied or blessed items!! \n");

            str.Append("- You can buy a house placement tool but which is blessed but     will cost more \n");

            str.Append(
                "   \nThe real estate agent will buy back any house deeds but you will not get full value back. \n\n");

            str.Append("- To appraise a house deed say \"appraise\" and target the deed. \n");

            str.Append("- To redeem a house deed for it's appraised value drag it onto     the real estate agent. \n");

            return str.ToString();
        }

        public static string ConstructDocksString()
        {
            var str = new StringBuilder();

            str.Append("<CENTER><BIG>The Docks</BIG></CENTER>\n");

            str.Append(
                "   The docks is located in West Britain south of the West Britain Bank. Here is where tournaments are held as well as duels. This is also a safe haven and a great spot to trade your items with other players.\n\n");

            return str.ToString();
        }

        public static string ConstructHealersGuildString()
        {
            var str = new StringBuilder();

            str.Append("<CENTER><BIG>Healers</BIG></CENTER>\n");

            str.Append(
                "   Healers located throughout the land will provide you with resurrection service and while you are young will offer to heal you as well. The healer and healer guildmaster can train you in swordsmanship, healing, anatomy, spirit speak, resisting spells, and forensic evaluation.\n\n");

            str.Append("- To be Resurrected simply walk close to a healer. \n");

            str.Append("- If you are young the healers will also heal you. \n");

            str.Append("- To See which skills a vendor can train say \"name train\" \n");

            str.Append("- To ask the vendor to train a skill say \"name train skill\" \n");

            str.Append("  Where name is the vendors name and skill is the skill to\n  train. \n\n");

            str.Append("  The healers also sell bandages, potions, and reagents. \n");

            str.Append("- To buy from the vendors say \"vendor buy\" \n");

            str.Append("- To learn more about Swordsmanship, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a> \n");

            str.Append("- To learn more about Healing, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            str.Append("- To learn more about Anatomy, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            str.Append("- To learn more about Spirit Speak, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            str.Append("- To learn more about Resisting Spells, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            str.Append("- To learn more about Forensic Evaluation, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            return str.ToString();
        }

        public static string ConstructInnGuildString()
        {
            var str = new StringBuilder();

            str.Append("<CENTER><BIG>The Inn</BIG></CENTER>\n");

            str.Append(
                "   Players starting their adventure in Ultima Online Forever will begin at West Britain Inn.\n\n");

            str.Append(
                "   While inside an Inn you may log off instantly allowing you to switch between your characters without waiting for them to time out. Normally you would have to wait 5 minutes to log in another character.\n");

            str.Append("\n  The innkeeper sells a variety of food and misc items including backpacks. \n\n");

            str.Append("- To buy from the vendors say \"vendor buy\" \n");

            str.Append("- To log out open your paperdoll with Alt+P and choose logout. \n");

            str.Append("- To see the new player guide, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            str.Append("- To see a detailed guide on getting started, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            return str.ToString();
        }

        public static string ConstructMageGuildString()
        {
            var str = new StringBuilder();

            str.Append("<CENTER><BIG>Mage Guild</BIG></CENTER>\n");

            str.Append(
                "   Mage shops allow the players to purchase reagents for spells, inscription supplies, spell books, and low level scrolls. The mages can train you in evaluate intelligence, inscription, magery, resist spells, wrestling, meditation. The alchemist can teach you about alchemy and taste identification.\n\n");

            str.Append("- To buy from the vendors say \"vendor buy\" \n");

            str.Append("- To sell items to the vendors say \"vendor sell\" \n");

            str.Append("- To See which skills a vendor can train say \"name train\" \n");

            str.Append("- To ask the vendor to train a skill say \"name train skill\" \n");

            str.Append("  Where name is the vendors name and skill is the skill to\n  train. \n");

            str.Append("- To learn more about Alchemy, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            str.Append("- To learn more about Evaluating Intelligence, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            str.Append("- To learn more about Inscription, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            str.Append("- To learn more about Magery, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            str.Append("- To see a list of spells, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            str.Append("- To learn more about Meditation, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a> \n");

            str.Append("- To learn more about Resisting Spells, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            str.Append("- To learn more about Taste Identification, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            str.Append("- To learn more about Wrestling, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            return str.ToString();
        }

        public static string ConstructMinersGuildString()
        {
            var str = new StringBuilder();

            str.Append("<CENTER><BIG>Miners Guild</BIG></CENTER>\n");

            str.Append(
                "   Players wanting to pursue a career in mining or stone crafting can learn the trade here. The Miner Guildmaster can teach you about mining and item identification. The stone crafter can teach you the basics of carpentry.\n\n");

            str.Append("- To See which skills a vendor can train say \"name train\" \n");

            str.Append("- To ask the vendor to train a skill say \"name train skill\" \n");

            str.Append("  Where name is the vendors name and skill is the skill to\n  train. \n");

            str.Append(
                "\n   To begin your career as a miner you will need either a shovel or pickaxe. The mining camp connects to a cave in Cove which you can practice mining. Double click the mining tool and target the ground while you in the cave to begin mining. \n\n");

            str.Append(
                "   Once you have some ore you want to process into ingots, double click the ore and target a forge found at the entrance of the cave. \n\n");

            str.Append("- To learn more about being a miner, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a> \n");

            str.Append("- To learn more about stonecrafting, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            return str.ToString();
        }

        public static string ConstructStablesGuildString()
        {
            var str = new StringBuilder();

            str.Append("<CENTER><BIG>The Stables</BIG></CENTER>\n");

            str.Append(
                "   Stables are located throughout the towns of Britannia and allow you to purchase pets such as horses and pack horses. You can also stable animals and claim them from any stable in the land.\n\n");

            str.Append(
                "   The stable master can also provide the player training in the animal lore, taming and veterinary skills.\n\n");

            str.Append("- To See which skills a vendor can train say \"name train\" \n");

            str.Append("- To ask the vendor to train a skill say \"name train skill\" \n");

            str.Append("  Where name is the vendors name and skill is the skill to\n  train. \n");

            str.Append("- To buy from the stable master say \"vendor buy\" \n");

            str.Append("- To stable a pet say \"Stable\" \n");

            str.Append("- To claim all of your pets from the stable say \"claim\" \n");

            str.Append("- Stabled pets cost 30-90 gold a week but do not need to be\n  refreshed. \n\n");

            str.Append("- To learn more about being an animal tamer, <a href=\"http://uofwiki.com/index.php?title=AnimalTaming\">click here</a> \n");

            str.Append("- To learn more about Animal Lore, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a> \n");

            str.Append("- To learn more about Animal Taming, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a> \n");

            str.Append("- To learn more about Veterinary, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a> \n");

            return str.ToString();
        }

        public static string ConstructTailorGuildString()
        {
            var str = new StringBuilder();

            str.Append("<CENTER><BIG>Tailor Shop</BIG></CENTER>\n");

            str.Append(
                "   Tailor shops allow players to purchase clothing, dye tubs, cloth, and tailoring supplies. Tailors can also train you in the tailoring skill.\n\n");

            str.Append("- To buy from the vendors say \"vendor buy\" \n");

            str.Append("- To sell items to the vendors say \"vendor sell\" \n");

            str.Append("- To See which skills a vendor can train say \"name train\" \n");

            str.Append("- To ask the vendor to train a skill say \"name train skill\" \n");

            str.Append("  Where name is the vendors name and skill is the skill to\n  train. \n");

            str.Append("- To learn more about Tailoring, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            str.Append(
                "\n   Once you have reached 70 tailoring you can start to receive bulk orders by selling items or saying \"order status\" \n\n");

            str.Append("- For more information about bulk order deeds, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            str.Append("- To see the rewards available for tailor bulk orders, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a>\n");

            return str.ToString();
        }

        public static string ConstructPlayerVendorsString()
        {
            var str = new StringBuilder();

            str.Append("<CENTER><BIG>Public Houses & Player Vendors</BIG></CENTER>\n");

            str.Append(
                "   One unique feature of Ultima Online is the ability to own and operation your own shop. You can either run your own vendor house or place a vendor in another players shop.\n\n");

            str.Append(
                "Players can mark their house as public allowing them to place vendors and provide services to other players such as rune libraries or guild houses. \n");

            str.Append("- To learn more about the different houses available, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a> \n");

            str.Append("- To learn about running your own player vendor, <a href=\"http://uofwiki.com/index.php?title=Archery\">click here</a> \n");

            str.Append("A variety of other guides on house ownership are also available \n");

            str.Append("- <a href=\"http://uofwiki.com/index.php?title=Archery\">Entrance and Security</a> \n");

            str.Append("- <a href=\"http://uofwiki.com/index.php?title=Archery\">Protecting your valuables</a> \n");

            str.Append("- <a href=\"http://uofwiki.com/index.php?title=Housing\">House ownership and commands</a> \n");

            return str.ToString();
        }

        public static string ConstructBattlesString()
        {
            var str = new StringBuilder();

            str.Append("<CENTER><BIG>The Battles System</BIG></CENTER>\n");

            str.Append(
                "   The Battles System is Ultima Online Forever's PvP event system.  It allows us to easily start and stop all types of PvP events.  When an event is started, a notification will appear on your screen asking if you would like to join the event.");

            str.Append(
                "  If you select yes, you will be teleported to the event arena immediately.  \nThere are 2 distinct phases to each event:\n   1) The Preperation Phase\n   2) The Running Phase\n\n");

            str.Append(
                "   During the preperation phase, everyone is invulnerable to eachother.  This phase allows people to queue up and join the event prior to it starting.  The running phase is the actual game.  After the event ends, all players will be sent to the Britain Docks and a scoreboard will appear on your screen.\n");

            str.Append("<BIG><U><CENTER>Joining</CENTER></U></BIG>\n");

            str.Append(
                "   There are 2 ways in which you can join a battle once the queue has started: \n\n   1) Click to join the queue when the global notification is sent\n   2) Type [battles then click on the relevant event and click      to join the queue\n\n");

            str.Append(
                "   It should be noted that you are not required to join the queue as soon as it pops.  You are able to join every battle just prior to it switching to the running phase.  This means that you can join the queue when there are a few seconds left on the queuing phase.");

            str.Append(
                "\n\n   Furthermore, in BoW (Battle of Wind) and CTF (Capture the Flag), you are able to join the game even while it is running.  All you have to do is type [battles and then click on the arrow beside the relevant battle then join the queue.  It should pop for you immediately and allow you to join the battle.");

            str.Append(
                "\n\n   This does not work in games where you have one life, such as FFA (Free for all) and TvT (Team vs Team).  If you have missed the queue and it has already entered the running stage, you will be uanble to join the battle.\n");

            str.Append("<BIG><U><CENTER>Leaving</CENTER></U></BIG>\n");

            str.Append(
                "   The simplest way to leave a battle is by typing @leave while in the battle.  This will instantly force you out of the battle and teleport you to the Britain Docks.  The second method is to type [battles, click on the relevant battle and then click Leave Battle.\n");

            str.Append("<BIG><U><CENTER>Scoring</CENTER></U></BIG>\n");

            str.Append(
                "   A variety of statistics are tracked via the battles system.  These statistics are also gathered and stored to your battles profile at the end of the battle.  Your battle profile can be accessed by typing [battles and clicking on the WAR button located at the top of the gump.");

            str.Append(
                "\n\n  Tracked statistics are also displayed to you at the end of the battle in scoreboard form.  It can be sorted by the column headers above each column to display statistics in ascending and descending order.\n\n");

            str.Append("Some of the tracked statistics are: \n");

            str.Append("- Damage Done\n");

            str.Append("- Healing Done\n");

            str.Append("- Total Kills\n");

            str.Append("- Total Deaths\n");

            str.Append("- Total Walls Cast\n");

            str.Append("- Total Flag Assaults\n");

            str.Append("- Total Flag Defends\n");

            str.Append("- Flag Captures\n");

            str.Append("And many more..\n\n");

            str.Append(
                "  Trophies are also given out for many of these statistics at the end of a battle.  Trophies are also awarded in FFA for 1st, 2nd and 3rd place.\n");

            str.Append("<BIG><U><CENTER>Commands</CENTER></U></BIG>\n");

            str.Append(
                "   Once you enter into a battle, a few special commands will be accessable.  All commands are prefaced with a @ symbol.  Here is the list of battle commands:\n");
            

            str.Append("\n<u>Commands\n</u>");

            str.Append("@help - Displays a list of available commands for this battle.\n");

            str.Append("@b message - Broadcasts a message to all battle participants\n  and spectators.\n");

            str.Append("@team message - Broadcasts a message to all team members in\n  the battle.\n");

            str.Append("@leave - Removes you from the battle.\n");

            str.Append("@time - Displays the current stages time.\n");

            str.Append("@scoreboard - Displays battle statistics at any point in the\n  game.\n\n");

            str.Append("   To learn more about Battles and the Battle System, <a href=\"http://uofwiki.com/index.php?title=Battles\">click here</a> \n");
            return str.ToString();
        }

        #endregion

        public class GuildChatColorPicker : HuePicker
        {
            private readonly PlayerMobile User;

            private readonly CentralGumpProfile Profile;

            public GuildChatColorPicker(PlayerMobile user, CentralGumpProfile profile)
                : base(0xFAB)
            {
                User = user;
                Profile = profile;
            }

            public override void OnResponse(int hue)
            {
                if (User != null)
                {
                    User.SendMessage(hue, "Your guild messages will now appear in this hue.");
                    User.GuildMessageHue = hue;
                    SuperGump.Send(new CentralGumpUI(User, Profile, CentralGumpType.Information));
                }
            }
        }

        public class AllianceChatColorPicker : HuePicker
        {
            private readonly PlayerMobile User;

            private readonly CentralGumpProfile Profile;

            public AllianceChatColorPicker(PlayerMobile user, CentralGumpProfile profile)
                : base(0xFAB)
            {
                User = user;
                Profile = profile;
            }

            public override void OnResponse(int hue)
            {
                if (User != null)
                {
                    User.SendMessage(hue, "Your alliance messages will now appear in this hue.");
                    User.AllianceMessageHue = hue;
                    SuperGump.Send(new CentralGumpUI(User, Profile, CentralGumpType.Information));
                }
            }
        }
    }
}