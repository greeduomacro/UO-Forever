using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Commands;

namespace Server.Engines.XmlSpawner2
{
    public class XmlTeam : XmlAttachment
    {
        
        
        // a serial constructor is REQUIRED
        public XmlTeam(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlTeam(string teamName)
        {
            Team parsed = Team.EnemyToAll;
            try
            {
                parsed = (Team)Enum.Parse(typeof(Team), teamName, true);
            }
            catch { }
            TeamVal = parsed;
        }

        [Attachable]
        public XmlTeam()
        {
        }

        /*
        public enum Priority : int
        {
            None = 0, // normal noto except against enemy team members (orange). This is what Order and Chaos / factions would be implemented under.
            Guild = 1, // overrides order / chaos / factions & could be used to prevent stat loss (with uberscript)... does not override murder, criminal, or aggressor.
            SpecialEvent = 2, // overrides all other notoriety (including murder, criminal, etc.)
            SpecialEventOverride = 3, // ^ +1
            CompleteOverride = 4 // ^ +1
        }*/

        public enum Team : int // lots of options in case we want to do tons of different teams
        {
            EnemyToAll = 0x0,
            Team1 = 0x1,
            Team2 = 0x2,
            Team3 = 0x4,
            Team4 = 0x8,
            Team5 = 0x10,
            Team6 = 0x20,
            Team7 = 0x40,
            Team8 = 0x80,
            Team9 = 0x100,
            Order = 0x200,
            Chaos = 0x400,
            ShadowLords = 0x800,
            TrueBritannians = 0x1000,
            CouncilOfMages = 0x2000,
            Minax = 0x4000
        }

        private int m_Score = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Score { get { return m_Score; } set { m_Score = value; } }

        // perhaps will use priority some day in some way, but not now
        //private Priority m_Priority = 0;
        //[CommandProperty(AccessLevel.GameMaster)]
        //public Priority PriorityVal { get { return m_Priority; } set { m_Priority = value; } }

        private Team m_Team = Team.EnemyToAll;
        [CommandProperty(AccessLevel.GameMaster)]
        public Team TeamVal { get { return m_Team; } 
            set { 
                m_Team = value;
                if (this.AttachedTo is Mobile)
                {
                    Mobile m = (Mobile)this.AttachedTo;
                    m.Delta(MobileDelta.Noto);
                    m.InvalidateProperties();
                }
            } 
        
        }

        private bool m_TeamHarmAllowed = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool TeamHarmAllowed { get { return m_TeamHarmAllowed; } set { m_TeamHarmAllowed = value; } }
        
        private bool m_TeamGreen = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool TeamGreen { get { return m_TeamGreen; } 
            set { 
                m_TeamGreen = value;
                if (this.AttachedTo is Mobile)
                {
                    Mobile m = (Mobile)this.AttachedTo;
                    m.Delta(MobileDelta.Noto);
                    m.InvalidateProperties();
                }
            } }

        private bool m_HealEnemyAllowed = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool HealEnemyAllowed { get { return m_HealEnemyAllowed; } set { m_HealEnemyAllowed = value; } }

        public static bool SameTeam(List<XmlTeam> teams1, List<XmlTeam> teams2)
        {
            if (teams1 == null || teams2 == null) return false;
            int team1Final = 0;
            int team2Final = 0;
            foreach (XmlTeam team in teams1)
            {
                team1Final |= (int)team.TeamVal;
            }
            foreach (XmlTeam team in teams2)
            {
                team2Final |= (int)team.TeamVal;
            }
            return (team1Final & team2Final) > 0;
        }

        public static bool ShowGreen(List<XmlTeam> teams1, List<XmlTeam> teams2)
        {
            foreach (XmlTeam team1 in teams1)
            {
                foreach (XmlTeam team2 in teams2)
                {
                    if ((team1.TeamVal & team2.TeamVal) > 0) // same team
                    {
                        if (team1.TeamGreen && team2.TeamGreen) // only green if both have TeamGreen on!
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool AllowTeamHarmful(List<XmlTeam> teams1, List<XmlTeam> teams2)
        {
            foreach (XmlTeam team1 in teams1)
            {
                foreach (XmlTeam team2 in teams2)
                {
                    if (team1.TeamVal == 0 && team2.TeamVal == 0) { return true; } // enemy to all
                    if ((team1.TeamVal & team2.TeamVal) > 0) // same team
                    {
                        if (team1.TeamHarmAllowed && team2.TeamHarmAllowed) // only allow harmful if both have allowharmful
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool AllowHealEnemy(List<XmlTeam> teams1, List<XmlTeam> teams2)
        {
            foreach (XmlTeam team1 in teams1)
            {
                foreach (XmlTeam team2 in teams2)
                {
                    if ((team1.TeamVal & team2.TeamVal) == 0) // enemy team
                    {
                        if (team1.HealEnemyAllowed && team2.HealEnemyAllowed) // only allow harmful if both have allowhealenemy
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public override void OnDelete()
        {
            if (this.AttachedTo is Mobile)
            {
                // shouldn't have to worry about teams including "this" team, as it's already Deleted == true by this point
                List<XmlTeam> teams = XmlAttach.GetTeams(this.AttachedTo);
                Mobile m = (Mobile)this.AttachedTo;
                if (teams == null)
                {
                    m.CustomTeam = false;
                }
                else
                {
                    m.CustomTeam = true;
                }
            }
            base.OnDelete();
        }

        public override void OnReattach()
        {
            Mobile mob = AttachedTo as Mobile;
            if (mob != null)
            {
                mob.CustomTeam = true;
            }
            base.OnReattach();
        }

        public override void OnAttach()
        {
            if (!(AttachedTo is Mobile || AttachedTo is Corpse))
            {
                this.Delete();
                return;
            }
            
            List<XmlTeam> current = XmlAttach.GetTeams(AttachedTo);
            if (current != null)
            {
                foreach (XmlTeam team in current)
                {
                    if (team != this) team.Delete();
                }
            }

            Mobile mob = AttachedTo as Mobile;
            if (mob != null)
            {
                mob.Delta(MobileDelta.Noto);
                mob.InvalidateProperties();
                mob.CustomTeam = true;
            }
            base.OnAttach();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write((int)m_Score);
            //writer.Write((int)m_Priority);
            writer.Write((int)m_Team);
            writer.Write((bool)m_TeamHarmAllowed);
            writer.Write((bool)m_TeamGreen);
            writer.Write((bool)m_HealEnemyAllowed);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_Score = reader.ReadInt();
            //m_Priority = (Priority)reader.ReadInt();
            m_Team = (Team)reader.ReadInt();
            m_TeamHarmAllowed = reader.ReadBool();
            m_TeamGreen = reader.ReadBool();
            m_HealEnemyAllowed = reader.ReadBool();
        }
    }
}
