using System;
using System.Collections;
using System.Collections.Generic;
using Server.Accounting;
using Server.Commands;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Items;
using Server.Regions;

namespace Server.Games
{
    public class PseudoSeerLogTimer : Timer
    {
        public static void Initialize()
        {
            new PseudoSeerLogTimer().Start();
        }

        public PseudoSeerLogTimer()
            : base(TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(1.0))
        {
            Priority = TimerPriority.OneMinute;
        }

        protected override void OnTick()
        {
            if (PseudoSeerStone.Instance == null) return;

            List<Mobile> list = PseudoSeersControlGump.PseudoseerControlledMobiles();
            foreach (Mobile mob in list)
            {
                if (mob is BaseCreature)
                {
                    try
                    {
                        LoggingCustom.Log("LOG_PseudoseerMobs.txt", DateTime.UtcNow + "\t" + mob + "\t" + mob.Account + "\t" + mob.Region.Name + "\t" + mob.Location);
                    }
                    catch {}
                }
            }
        }



        public static void HungerDecay(Mobile m)
        {
            if (m != null && m.Hunger >= 1)
                m.Hunger--;
        }

        public static void ThirstDecay(Mobile m)
        {
            if (m != null && m.Thirst >= 1)
                m.Thirst--;
        }
    }
    
    public class CreaturePossession
    {
        public const AccessLevel FullAccessStaffLevel = AccessLevel.GameMaster;

        private static readonly List<BaseCreature> m_possessedMonsters = new List<BaseCreature>();
        internal static List<BaseCreature> PossessedMonsters { get { return m_possessedMonsters; } }

        public static void Initialize()
        {
            CommandSystem.Register("PseudoSeerList", AccessLevel.GameMaster, new CommandEventHandler(PseudoSeerList_Command));
            CommandSystem.Register("PseudoSeerRemove", AccessLevel.GameMaster, new CommandEventHandler(PseudoSeerRemove_Command));
            CommandSystem.Register("PseudoSeerAdd", AccessLevel.GameMaster, new CommandEventHandler(PseudoseerAdd_Command));
            CommandSystem.Register("unpossess", AccessLevel.Player, new CommandEventHandler(Unpossess_Command));
            CommandSystem.Register("possess", AccessLevel.Player, new CommandEventHandler(Possess_Command));
            CommandSystem.Register("direct", AccessLevel.Player, new CommandEventHandler(Direct_Command));

            CommandSystem.Register("OtherPossess", AccessLevel.GameMaster, new CommandEventHandler(OtherPossess_Command));
            //CommandSystem.Register("teamnoto", AccessLevel.GameMaster, new CommandEventHandler(TeamNoto_Command));
           // CommandSystem.Register("teamharm", AccessLevel.GameMaster, new CommandEventHandler(TeamHarm_Command));
            CommandSystem.Register("DungeonReport", AccessLevel.GameMaster, new CommandEventHandler(DungeonReport_Command));
            CommandSystem.Register("DungeonGo", AccessLevel.GameMaster, new CommandEventHandler(DungeonGo_Command));
            CommandSystem.Register("ps", AccessLevel.Player, new CommandEventHandler(PseudoseerChat_Command));
            CommandSystem.Register("say", AccessLevel.Player, new CommandEventHandler(Say_Command));
            CommandSystem.Register("LootMultiplier", AccessLevel.GameMaster, new CommandEventHandler(LootMultiplier_Command));
            CommandSystem.Register("TournamentAward", AccessLevel.GameMaster, new CommandEventHandler(TournamentAward_Command));
            CommandSystem.Register("CTFAward", AccessLevel.GameMaster, new CommandEventHandler(CTFAward_Command));
            //CommandSystem.Register("teamallowenemyhealing", AccessLevel.GameMaster, new CommandEventHandler(EnemyHealing_Command));
            
            //   Some commands that may be useful for Packet debugging... disabled for now
            //CommandSystem.Register("LogPackets", AccessLevel.GameMaster, new CommandEventHandler(LogPackets_Command));
            //CommandSystem.Register("ForceWalk", AccessLevel.GameMaster, new CommandEventHandler(ForceWalk_Command));
        }

        public static void PseudoSeerRemove_Command(CommandEventArgs e)
        {
            if (PseudoSeerStone.Instance == null)
            {
                e.Mobile.SendMessage("PseudoSeerStone not found!");
                return;
            }
            if (e.Arguments.Length == 0)
            {
                e.Mobile.SendMessage("You have to provide a username to remove. Use [PseudoSeerList to get all the possible Pseudoseer usernames.");
                return;
            }

            try
            {
                IAccount toRemove = null;
                foreach (KeyValuePair<IAccount, string> pair in PseudoSeerStone.Instance.PseudoSeers)
                {
                    if (pair.Key.Username == e.Arguments[0])
                    {
                        toRemove = pair.Key;
                        break;
                    }
                }
                if (toRemove != null)
                {
                    PseudoSeerStone.Instance.PseudoSeers.Remove(toRemove);
                    e.Mobile.SendMessage(toRemove.Username + " account has been removed.");
                }
                else
                {
                    e.Mobile.SendMessage("No account by the name of " + e.Arguments[0] + " was found to have pseudoseer permissions.");
                }
            }
            catch (Exception except)
            {
                e.Mobile.SendMessage("Exception caught: " + except.Message);
            }
        }

        public static void PseudoSeerList_Command(CommandEventArgs e)
        {
            if (PseudoSeerStone.Instance == null)
            {
                e.Mobile.SendMessage("PseudoSeerStone not found!");
                return;
            }
            try
            {
                string message = "Current pseudoseer accounts: ";
                foreach (KeyValuePair<IAccount, string> pair in PseudoSeerStone.Instance.PseudoSeers)
                {
                    message += pair.Key.Username + ": " + PseudoSeerStone.Instance.GetPermissionsFor(pair.Key) + "\n";
                }
                e.Mobile.SendMessage(message);
            }
            catch (Exception except)
            {
                e.Mobile.SendMessage("Exception caught: " + except.Message);
            }
        }

        public static void ForceWalk_Command(CommandEventArgs e)
        {
            if (e.Arguments.Length == 0)
            {
                e.Mobile.SendMessage("You must provide one of the following args: execute, target, delete, write, sendone");
                return;
            }
            try
            {
                e.Mobile.Target = new ForceWalkTarget((Byte)int.Parse(e.Arguments[0]));
            }
            catch { }
        }
        private class ForceWalkTarget : Target
        {
            public byte Direction;
            public ForceWalkTarget(byte dir)
                : base(-1, false, TargetFlags.None)
            { Direction = dir; }

            protected override void OnTarget(Mobile from, object target)
            {
                Mobile mob = target as Mobile;
                if (mob == null || mob.NetState == null)
                {
                    from.SendMessage("You must target a Mobile with a NetState!");
                    return;
                }
                //mob.NetState.Send(new PlayerMove((Direction)Direction));
                //mob.NetState.Send(new MovementAck());
                mob.Move((Direction)Direction);
            }
        }

        public static void LogPackets_Command(CommandEventArgs e)
        {
            if (e.Arguments.Length == 0) {
                e.Mobile.SendMessage("You must provide one of the following args: execute, target, delete, write, sendone"); 
                return; 
            }
            string arg = e.Arguments[0].ToLower();
            if (arg == "target")
            {
                e.Mobile.Target = new LogPacketsTarget(LogPacketCommandType.StartLogging);
            }
            else if (arg == "sendone")
            {
                e.Mobile.Target = new LogPacketsTarget(LogPacketCommandType.SendOne);
            }
            else if (arg == "execute")
            {
                e.Mobile.Target = new LogPacketsTarget(LogPacketCommandType.Execute);
            }
            else if (arg == "delete")
            {
                e.Mobile.Target = new LogPacketsTarget(LogPacketCommandType.Delete);
            }
            else if (arg == "write")
            {
                e.Mobile.SendMessage("Not yet implemented");
            }
            else
            {
                e.Mobile.SendMessage("You must provide one of the following args: execute, target, delete, write, sendone");
                return;
            }
        }

        public enum LogPacketCommandType
        {
            Execute,
            StartLogging,
            SendOne,
            Delete
        }

        private class LogPacketsTarget : Target
        {
            private LogPacketCommandType Command;


            public LogPacketsTarget(LogPacketCommandType command)
                : base(-1, false, TargetFlags.None)
            { Command = command; }

            protected override void OnTarget(Mobile from, object target)
            {
                Mobile mob = target as Mobile;
                if (mob == null || mob.NetState == null)
                {
                    from.SendMessage("You must target a Mobile with a NetState!");
                    return;
                }
                if (Command == LogPacketCommandType.StartLogging)
                {
                    mob.NetState.LogPackets = true;
                    mob.SendEverything();
                }
                else if (Command == LogPacketCommandType.Execute)
                {
                    mob.NetState.LogPackets = false;
                    new LogExecuteTimer(mob.NetState);
                }
                else if (Command == LogPacketCommandType.SendOne)
                {
                    if (mob.NetState.Logger != null && mob.NetState.Logger.Packets.Count > 0)
                    {
                        PacketSimulation sim = mob.NetState.Logger.Packets.Dequeue();
                        sim.Process(mob.NetState);
                    }
                    else
                        from.SendMessage("No logged packets to send!");
                }
                else if (Command == LogPacketCommandType.Delete)
                {
                    if (mob.NetState.Logger != null && mob.NetState.Logger.Packets.Count > 0)
                    {
                        mob.NetState.Logger.Packets.Clear();
                    }
                    mob.NetState.LogPackets = false;
                }
            }
        }

        private class LogExecuteTimer : Timer
        {
            private NetState State;
            private TimeSpan TimeDifference; // difference between the recorded time and executedtime
            //private PacketSimulation LastExecutedPacket;

            public LogExecuteTimer(NetState state)
                : base(TimeSpan.Zero, TimeSpan.Zero)
            {
                Priority = TimerPriority.EveryTick;
                State = state;
                if (State == null || State.Logger == null || State.Logger.Packets.Count == 0)
                    return;
                TimeDifference = DateTime.UtcNow - State.Logger.Packets.Peek().ProcessedTime;
                Start();
            }

            protected override void OnTick()
            {
                try
                {
                    if (State == null || State.Logger == null || State.Logger.Packets.Count == 0)
                    {
                        Stop();
                        return;
                    }
                    
                    PacketSimulation sim = State.Logger.Packets.Peek();
                    if (DateTime.UtcNow > sim.ProcessedTime + TimeDifference)
                    {
                        State.Logger.Packets.Dequeue();
                        sim.Process(State);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("PacketLogExecute error: " + e.Message);
                    Console.WriteLine(e.StackTrace);
                    Stop();
                }

            }
        }

        

        public static void Say_Command(CommandEventArgs e)
        {
            PlayerMobile player = GetAssociatedPlayerMobile(e.Mobile);

            if (e.Mobile.AccessLevel > AccessLevel.Player
                ||
                (player != null
                && player.PseudoSeerPermissions != null
                && player.PseudoSeerPermissions != ""))
            {
                if (e.Arguments == null || e.Arguments.Length == 0)
                {
                    e.Mobile.SendMessage("You must provide something for your target to say!  e.g '[say Hello!'");
                }
                else
                {
                    e.Mobile.Target = new SayTarget(e.ArgString);
                }
            }
        }

        private class SayTarget : Target
        {
            private string said = null;

            public SayTarget(string saying)
                : base(-1, false, TargetFlags.None)
            {
                said = saying;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (from == null || target == null) return;
                if (said == null) 
                {
                    from.SendMessage("You must provide something for them to say!");
                    return;
                }
                if (!(target is Mobile))
                {
                    from.SendMessage("You must target a mobile!");
                    return;
                }
                Mobile mob = target as Mobile;
                if (from.AccessLevel > AccessLevel.Player || (target is BaseCreature && HasPermissionsToPossess(from, target as BaseCreature)))
                {
                    mob.Say(said);
                }
            }
        }

        public static void LootMultiplier_Command(CommandEventArgs e)
        {
            if (PseudoSeerStone.Instance == null)
            {
                e.Mobile.SendMessage("There is no pseudoseer stone instance!  You must '[add pseudoseerstone' to use this command, since the multiplier is stored on the stone!");
                return;
            }
            if (e.Arguments == null || e.Arguments.Length == 0)
            {
                e.Mobile.SendMessage("You must give a float value!  e.g '[lootmultiplier 2' would double the loot dropped by all mobs in the world.'");
                return;
            }
            try
            {
                double value = Double.Parse(e.Arguments[0]);
                if (value >= 0)
                {
                    PseudoSeerStone.Instance.CreatureLootDropMultiplier = value;
                }
                else
                {
                    e.Mobile.SendMessage("You must provide a number greater than 0!");
                }
            }
            catch
            {
                e.Mobile.SendMessage("You must give a float value!  e.g '[lootmultiplier 2' would double the loot dropped by all mobs in the world.'");
            }
        }

        //Begin Tournament 
        public static void TournamentAward_Command(CommandEventArgs e)
        {
            if (PseudoSeerStone.Instance == null)
            {
                e.Mobile.SendMessage("There is no pseudoseer stone instance!  You must '[add pseudoseerstone' to use this command, since the multiplier is stored on the stone!");
                return;
            }
            if (e.Arguments == null || e.Arguments.Length == 0)
            {
                e.Mobile.SendMessage("You must give an integer value!  e.g '[tournamentaward 1000' would make the cash award to be 1000 * number of participants'");
                return;
            }
            try
            {
                int value = int.Parse(e.Arguments[0]);
                if (value >= 0)
                {
                    PseudoSeerStone.Instance.TournamentAward = value;
                    e.Mobile.SendMessage("Tournament award multiplier set to " + PseudoSeerStone.Instance.TournamentAward + "x number of participants.");
                }
                else
                {
                    e.Mobile.SendMessage("You must provide a number greater than 0!");
                }
            }
            catch
            {
                e.Mobile.SendMessage("You must give an integer value!  e.g '[tournamentaward 1000' would make the cash award to be 1000 * number of participants'");
            }
        }

        //Begin CTF 
        public static void CTFAward_Command(CommandEventArgs e)
        {
            if (PseudoSeerStone.Instance == null)
            {
                e.Mobile.SendMessage("There is no pseudoseer stone instance!  You must '[add pseudoseerstone' to use this command, since the multiplier is stored on the stone!");
                return;
            }
            if (e.Arguments == null || e.Arguments.Length == 0)
            {
                e.Mobile.SendMessage("You must give an integer value!  e.g '[CTFaward 1000' would make the cash award to be 1000 * number of participants'");
                return;
            }
            try
            {
                int value = int.Parse(e.Arguments[0]);
                if (value >= 0)
                {
                    PseudoSeerStone.Instance.CTFAward = value;
                    e.Mobile.SendMessage("CTF award multiplier set to " + PseudoSeerStone.Instance.CTFAward + "x number of participants.");
                }
                else
                {
                    e.Mobile.SendMessage("You must provide a number greater than 0!");
                }
            }
            catch
            {
                e.Mobile.SendMessage("You must give an integer value!  e.g '[CTFaward 1000' would make the cash award to be 1000 * number of participants'");
            }
        }

        public static void PseudoseerChat_Command(CommandEventArgs e)
        {
            if (HasAnyPossessPermissions(e.Mobile))
            {
                if (PseudoSeerStone.Instance == null) {
                    e.Mobile.SendMessage("No pseudoseerstone exists... cannot chat.");
                    return;
                }
                PlayerMobile player = GetAssociatedPlayerMobile(e.Mobile);

                string name;
                if (player == null) { name = null; }
                else { name = player.Name; }
                foreach (Mobile mob in PseudoSeersControlGump.PseudoseerControlledMobiles())
                {
                    if (mob.Deleted || mob.NetState == null)
                    {
                        continue;
                    }
                    mob.SendMessage(38, "[PS] " + name + ": " + e.ArgString);
                }
            }
            else
            {
                e.Mobile.SendMessage("You do not have access to that command.");
            }
        }

        public static PlayerMobile GetAssociatedPlayerMobile(Mobile m)
        {
            PlayerMobile player = m as PlayerMobile;
            if (player == null) 
            {
                BaseCreature bc = m as BaseCreature;
                if (bc != null
                    && !bc.Deleted
                    && bc.NetState != null
                    && bc.NetState.Account != null)
                {
                    player = bc.NetState.Account.GetPseudoSeerLastCharacter() as PlayerMobile;
                }
            }
            return player;
        }

        public static void DungeonReport_Command(CommandEventArgs e)
        {
            PlayerMobile player = GetAssociatedPlayerMobile(e.Mobile);
            
            if (e.Mobile.AccessLevel > AccessLevel.Player 
                ||
                (player != null
                && player.PseudoSeerPermissions != null
                && player.PseudoSeerPermissions != ""))
                //&& player.Pseu_DungeonWatchAllowed))
            {
                string message = "Players in each region: \n";
                foreach (Region region in Region.Regions)
                {
                    if (region is DungeonRegion)
                    {
                        message += region.Name + ": " + region.GetPlayerCount() + "\n";
                    }
                }
                e.Mobile.SendMessage(message);
            }
            else
            {
                e.Mobile.SendMessage("You do not have access to that command.");
            }
        }

        public static void DungeonGo_Command(CommandEventArgs e)
        {
            string[] args = e.Arguments;
            PlayerMobile player = GetAssociatedPlayerMobile(e.Mobile);
            if (e.Mobile.AccessLevel == AccessLevel.Player
                    &&
                        (player == null
                        || 
                        player.PseudoSeerPermissions == null
                        ||
                        player.PseudoSeerPermissions == ""))
                        //||
                        //player.Pseu_DungeonWatchAllowed == false))
            {
                e.Mobile.SendMessage("You do not have access to that command.");
                return;
            }
            if (args.Length == 0)
            {
                e.Mobile.SendMessage("You must provide a dungeon name, e.g. '[dungeongo endium'. Do '[dungeonreport' to get a list of possible names.");
                return;
            }

            foreach (Region region in Region.Regions)
            {
                if (region is DungeonRegion && region.Name.ToLower().Trim() == args[0].ToLower().Trim())
                {
                    if (region.GoLocation.Equals(new Point3D()) == false)
                    {
                        e.Mobile.Location = region.GoLocation;
                        return;
                    }
                }
            }
            e.Mobile.SendMessage(args[0] + " is not a valid location.  Use [dungeonreport to get a list of valid locations.");
        }

        public static void PseudoseerAdd_Command(CommandEventArgs e)
        {
            string[] args = e.Arguments;
            if (args != null)
            {
                if (args.Length > 0)
                { 
                    string permissions = "";
                    foreach (string arg in args)
                    {
                        if (arg == "all")
                        {
                            permissions = "all";
                            break;
                        }
                        permissions += arg + " ";
                    }
                    PseudoSeerStone.PermissionsClipboard = permissions.Trim();
                    e.Mobile.SendMessage("Permissions clipboard is now: " + permissions);
                    e.Mobile.SendMessage("Target a player to add as a pseudoseer / update their permissions.");
                    e.Mobile.Target = new PseudoseerAddTarget(permissions);
                }
                else
                {
                    e.Mobile.SendMessage("You must specify permissions!  E.g. '[pseudoseeradd orc orcishlord daemon'");
                }
            }
        }

        private class PseudoseerAddTarget : Target
        {
            public PseudoseerAddTarget(string permissions)
                : base(-1, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile gm, object target)
            {
                PlayerMobile pm = target as PlayerMobile;
                if (pm == null)
                {
                    gm.SendMessage("Not a valid player target!");
                    return;
                }
                if (PseudoSeerStone.Instance == null)
                {
                    gm.SendMessage("ERROR: No Pseudoseerstone exists!  [add pseudoseerstone somewhere in the world and try again!");
                    return;
                }
                PseudoSeerStone.Instance.PseudoSeerAdd = pm;
                gm.SendMessage("They have been successfully added with the following permissions:\n"
                              + PseudoSeerStone.Instance.CurrentPermissionsClipboard);

            }
        }

        public static void OtherPossess_Command(CommandEventArgs e)
        {
            if (PseudoSeerStone.Instance == null)
            {
                e.Mobile.SendMessage("No pseudoseerstone exists.  You must [add pseudoseerstone somewhere in the world before you can use this command");
            }
            else
            {
                e.Mobile.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(OtherPossess_PlayerTarget));
                e.Mobile.SendMessage("Target a player to add as pseudoseer with no permissions (unless they are already a pseudoseer). You will be prompted again to target a monster for them specifically to possess.");
            }
        }

        public static void OtherPossess_PlayerTarget(Mobile gm, object o)
        {
            if (o is PlayerMobile && ((PlayerMobile)o).AccessLevel == AccessLevel.Player)
            {
                gm.Target = new OtherPossessMonsterTarget(o as PlayerMobile);
                gm.SendMessage("Target a monster for that player to possess.");
            }
            else
            {
                gm.SendMessage("Not a valid PlayerMobile.");
            }
        }

        private class OtherPossessMonsterTarget : Target
        {
            private PlayerMobile m_Player;

            public OtherPossessMonsterTarget(PlayerMobile player)
                : base(-1, false, TargetFlags.None)
            {
                if (player == null) { return; }
                m_Player = player;
            }

            protected override void OnTarget(Mobile gm, object target)
			{
                if (m_Player == null) {
                    gm.SendMessage("ERROR: somehow didn't get a player target first!");
                    return;
                }
                if (PseudoSeerStone.Instance == null) 
                {
                    gm.SendMessage("ERROR: somehow didn't have a pseudoseerstone available!  [add pseudoseerstone somewhere in the world to try again!");
                    return;
                }

                if (target is BaseCreature )
                {
                    // player isn't already a pseudoseer
                    if (!PseudoSeerStone.Instance.PseudoSeers.ContainsKey(m_Player.Account)) {
                        // have to temporarily store the clipboard so we can add a pseudoseer with no permissions
                        string oldClipboard = PseudoSeerStone.PermissionsClipboard;
                        PseudoSeerStone.PermissionsClipboard = "";
                        PseudoSeerStone.Instance.PseudoSeerAdd = m_Player;
                        PseudoSeerStone.PermissionsClipboard = oldClipboard;
                    }
                    ForcePossessCreature(gm, m_Player, target as BaseCreature);
                }
                else
                {
                    gm.SendMessage("Not a valid Monster to be possessed.  Player NOT added as no-permission pseudoseer.");
                }
			}
        }

        /*
        public static void TeamNoto_Command(CommandEventArgs e)
        {
            string[] args = e.Arguments;
            if (args != null)
            {
                bool error = false;
                if (args.Length > 0)
                {
                    try 
                    {
                        int val = int.Parse(args[0]);
                        if (val < 0 || val > 3)
                        {
                            error = true;
                        }
                        else
                        {
                            AITeamList.NotoType = (AITeamList.NotoTypeEnum)Enum.ToObject(typeof(AITeamList.NotoTypeEnum), val);
                        }
                    }
                    catch (Exception) { error = true; }
                }
                else
                {
                    error = true;
                }
                if (error)
                {
                    e.Mobile.SendMessage("Current noto type = " + AITeamList.NotoType + "\n... change with [teamnoto (int)\n"
                                            + "For all options below, all enemy team members will flag orange, regardless of other noto:\n"
                                            + "   0 = all allies are green\n"
                                            + "   1 = all allies are blue/grey/red but enemy militia are not orange\n"
                                            + "   2 = all allies standard noto (enemy militia/guilds flag orange)"
                                            + "   3 = all allies standard noto, and cannot heal allies in an enemy militia");
                }
            }
        }*/

        /*
        public static void TeamHarm_Command(CommandEventArgs e)
        {
            string[] args = e.Arguments;
            if (args != null)
            {
                bool error = false;
                if (args.Length > 0)
                {
                    try
                    {
                        string val = args[0].ToLower().Trim();
                        if (val == "true")
                        {
                            AITeamList.TeamHarm = true;
                            e.Mobile.SendMessage("Successfully set to true.  Teams can now harm each other");
                        }
                        else if (val == "false")
                        {
                            AITeamList.TeamHarm = false;
                            e.Mobile.SendMessage("Successfully set to false.  Teams cannot harm each other anymore.");
                        }
                        else
                        {
                            error = true;
                        }
                    }
                    catch (Exception) { error = true; }
                }
                else
                {
                    error = true;
                }
                if (error)
                {
                    e.Mobile.SendMessage("You must provide either true or false as an argument!");
                }
            }
        }*/

        public static void Direct_Command(CommandEventArgs e)
        {
            if (CreaturePossession.HasAnyPossessPermissions(e.Mobile))
            {
                string[] args = e.Arguments;
                if (args != null)
                {
                    if (args.Length > 0)
                    {
                        if (args[0].ToLower() == "force")
                        {
                            e.Mobile.Target = new DirectTarget(true);
                            e.Mobile.SendMessage("Target location to direct nearby mobs.");
                        }
                        else
                        {
                            e.Mobile.SendMessage("Use either '[direct' or '[direct force' with this command!");
                        }
                        
                    }
                    else
                    {
                        e.Mobile.SendMessage("Target location to direct nearby mobs (use '[direct force' to move in combat).");
                        e.Mobile.Target = new DirectTarget(false);
                    }
                }
            }
        }

        private class DirectTarget : Target
        {
            public bool Force = false;
            public DirectTarget(bool force)
                : base(-1, true, TargetFlags.None)
            {
                this.Force = force;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                // fix this with a waypoint?  timeout on waypoint?  Dictionary for mobile lookup?
                if (!(target is IPoint3D))
                {
                    from.SendMessage("Not a valid target.");
                    return;
                }
                IPoint2D targetLocation = target as IPoint2D;
                foreach (Mobile m in from.GetMobilesInRange(10))
                {
                    if (m is BaseCreature && HasPermissionsToPossess(from, (BaseCreature)m) && !m.Blessed)
                    {
                        BaseCreature bc = m as BaseCreature;
                        bc.TargetLocation = targetLocation;
                        if (this.Force && bc.AIObject != null)
                        {
                            bc.ForceWaypoint = true;
                        }
                    }
                }
            }
        }       

        public static void Unpossess_Command(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile) { return; }
            if (!AttemptReturnToOriginalBody(e.Mobile.NetState))
            {
                e.Mobile.NetState.Dispose();
            }
        }

        public static bool AttemptReturnToOriginalBody(NetState monsterNetState)
        {
            if (monsterNetState == null || monsterNetState.Account == null) { return false; }

            PlayerMobile pseudoSeerLastCharacter = (PlayerMobile)monsterNetState.Account.GetPseudoSeerLastCharacter();

            Point3D newLocation = Point3D.Zero;
            if (PseudoSeerStone.Instance != null) // see if it is a pseudoseer and should be sent to where the monster was
            {
                string permissions = PseudoSeerStone.Instance.GetPermissionsFor(monsterNetState.Account);
                if (monsterNetState.Mobile != null
                    && permissions != null
                    && permissions != ""
                    && PseudoSeerStone.Instance.MovePSeerToLastPossessed)
                {
                    newLocation = monsterNetState.Mobile.Location;
                }
            }
            bool output = CreaturePossession.ConnectClientToPC(monsterNetState, pseudoSeerLastCharacter);
            // do it after they log back in
            if (newLocation != Point3D.Zero) { pseudoSeerLastCharacter.Location = newLocation; }
            return output;
        }

        public static void Possess_Command(CommandEventArgs e)
        {
            OnPossessTargetRequest(e.Mobile);
        }

        public static void OnPossessTargetRequest(Mobile from)
        {
            if (CreaturePossession.HasAnyPossessPermissions(from))
            {
                from.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(OnPossessTarget));
                from.SendMessage("Target a creature to possess.");
            }
        }
                
        public static void OnPossessTarget(Mobile from, object o)
        {
            if (o is BaseCreature)
            {
                CreaturePossession.TryPossessCreature(from, (BaseCreature)o);
            }
            else
                from.SendMessage("Not a valid Monster to be possessed.");
        }

        public static void TryPossessCreature(Mobile from, BaseCreature Subject)
        {
            if (from == null || Subject == null) return;
            if (HasPermissionsToPossess(from, Subject))
            {
                if (Subject.NetState != null)
                {
                    from.SendMessage("Target creature is already under player control!");
                    return;
                }
                if (from.AccessLevel < FullAccessStaffLevel)
                {
                    if (IsInHouseOrBoat(Subject.Location, Map.Felucca))
                    {
                        from.SendMessage("You cannot possess a creature in a house or boat.");
                        return;
                    }
                    if (Subject.Blessed == true)
                    {
                        from.SendMessage("You cannot possess invulnerable mobs.");
                        return;
                    }
                }
                if (Subject.Controlled && !IsAuthorizedStaff(from))
                {
                    from.SendMessage("You cannot possess tamed or summoned creatures!");
                    return;
                }
                //Subject.HasBeenPseudoseerControlled = true;
                if (Subject.Backpack == null)
                {
                    // create a backpack for things like animals that have no backpack
                    // ... this prevents client crash in case somebody has their pack auto-opening on login
                    Subject.PackItem(new Gold(1)); 
                }
                LoggingCustom.LogPseudoseer(DateTime.Now + "\t" + from.Account + "\t" + from.Name + "\tpossessing\t" + Subject + "\tRegion: " + Subject.Region + "\tLocation: " + Subject.Location);
                ConnectClientToNPC(from.NetState, Subject);
            }
            else
            {
                from.SendMessage("You are not permitted to possess that creature type, only staff can grant this permission.");
            }
        }

        public static bool ForcePossessCreature(Mobile gm, Mobile player, BaseCreature Subject)
        {
            if (Subject == null || player == null)
            {
                if (gm != null) gm.SendMessage("ForcePossessCreature: Player or Subject was null!  Can't continue with possession.");
                return false;
            }
            if (Subject.NetState != null)
            {
                if (gm != null) gm.SendMessage("Target creature is already under player control!");
                return false;
            }
            //Subject.HasBeenPseudoseerControlled = true;
            return ConnectClientToNPC(player.NetState, Subject);
        }

        public static bool HasPermissionsToPossess(Mobile from, BaseCreature Subject)
        {
            return from != null && (from.AccessLevel >= AccessLevel.GameMaster || HasPermissionsToPossess(from.NetState, Subject));
        }

        public static bool HasPermissionsToPossess(NetState from, BaseCreature Subject)
        {
            return from != null && IsAuthorizedAccount(from.Account, Subject);
        }

        public static bool HasAnyPossessPermissions(Mobile from)
        {
            return from != null && (from.AccessLevel >= AccessLevel.GameMaster || HasAnyPossessPermissions(from.NetState));
        }

        public static bool HasAnyPossessPermissions(NetState from)
        {
            return from != null &&
                (CreaturePossession.IsAuthorizedStaff(from) 
                || 
                (PseudoSeerStone.Instance != null 
                && PseudoSeerStone.Instance.GetPermissionsFor(from.Account) != null 
                && PseudoSeerStone.Instance.GetPermissionsFor(from.Account) != String.Empty));
        }

        public static bool HasAnyPossessPermissions(IAccount from)
        {
            return PseudoSeerStone.Instance != null 
                && PseudoSeerStone.Instance.GetPermissionsFor(from) != null 
                && PseudoSeerStone.Instance.GetPermissionsFor(from) != String.Empty;
        }

        static bool IsAuthorizedAccount(IAccount account, BaseCreature Subject)
        {
            if (account == null) return false;
            
            if (IsAuthorizedStaff(account)) return true;
            
            if (PseudoSeerStone.Instance == null) return false;
            
            Type creaturetype = Subject.GetType();
            //Console.WriteLine("Creature: " + creaturetype.ToString());
            string perms = PseudoSeerStone.Instance.GetPermissionsFor(account);
            //Console.WriteLine("perms: " + perms);
            if (perms == null)
            {
                return false;
            }
            string[] permittedTypeStrings = perms.Split();
            string[] typesegments = (creaturetype.ToString()).Split('.'); // string is something like Server.Mobiles.Orc
            if (typesegments.Length == 0)
            {
                return false;
            }
            //Console.WriteLine("permittedTypeStrings: " + permittedTypeStrings);
            foreach (string permittedTypeString in permittedTypeStrings)
            {
                //Console.WriteLine(permittedTypeString.ToLower() + "=" + typesegments[typesegments.Length - 1] + "?");
                //Console.WriteLine("" + permittedTypeString.ToLower() == typesegments[typesegments.Length - 1].ToLower());
                if (permittedTypeString.ToLower() == typesegments[typesegments.Length - 1].ToLower() || permittedTypeString == "all")
                {
                    return true;
                }

            }
            return false;
            //PossessPermissions reqPerms;
            //if (PermissionDictionary.TryGetValue(creaturetype, out reqPerms))
            //    return (perms & reqPerms) != 0;
           // return false;
        }

        internal static bool IsAuthorizedStaff(Mobile from)
        {
            return from.AccessLevel >= AccessLevel.GameMaster || (from != null && IsAuthorizedStaff(from.NetState));
        }

        internal static bool IsAuthorizedStaff(NetState from)
        {
            return from != null && (IsAuthorizedStaff(from.Account) || (from.Mobile != null && from.Mobile.AccessLevel >= AccessLevel.GameMaster));
        }

        internal static bool IsAuthorizedStaff(IAccount account)
        {
            return account!=null && account.AccessLevel >= FullAccessStaffLevel;
        }

        internal static bool ConnectClientToNPC(NetState client, BaseCreature Subject)
        {
            if (client == null) return false;

            if (Subject.NetState == null)
            {
                Mobile clientMobile = client.Mobile;
                if (client.Account != null && clientMobile is PlayerMobile) client.Account.SetPseudoSeerLastCharacter(client.Mobile);
                
                Subject.NetState = client;

                if (clientMobile != null)
                {
                    clientMobile.NetState = null;
                }

                Subject.NetState.Mobile = Subject;

                PacketHandlers.DoLogin(Subject.NetState, Subject);

                PossessedMonsters.Add(Subject);
                Subject.Account = client.Account;
                return true;
            }
            return false;
        }

        internal static bool ConnectClientToPC(NetState client, PlayerMobile Subject)
        {
            if (Subject != null && Subject.NetState == null)
            {
                Mobile clientMobile = client.Mobile;

                Subject.NetState = client;

                if (clientMobile != null)
                    clientMobile.NetState = null;

                Subject.NetState.Mobile = Subject;

                PacketHandlers.DoLogin(Subject.NetState, Subject);
                return true;
            }
            return false;
        }

        internal static void BootAllPossessions()
        {
            for (int i = 0; i < PossessedMonsters.Count; i++)
            {
                if (PossessedMonsters[i].Deleted || PossessedMonsters[i].NetState == null)
                    continue;

                //boot any player controlled monsters, attempting to return them to their original body if possible
                if (AttemptReturnToOriginalBody(PossessedMonsters[i].NetState) == false)
                {
                    PossessedMonsters[i].NetState.Dispose();
                }
            }
            PossessedMonsters.Clear();
        }

        public static bool IsInHouseOrBoat(Point3D location, Map map)
        {
            if (map == null)
                return true;
            Region region = Region.Find(location, map);
            return (region != null && region.GetRegion(typeof(Regions.HouseRegion)) != null)
                || Server.Multis.BaseBoat.FindBoatAt(location, map) != null;
        }

        static void LogoutIfPlayer(Mobile from)
        {
            if (from is PlayerMobile && from.Map != Map.Internal)
                EventSink.InvokeLogout(new LogoutEventArgs(from));
        }
    }
}
