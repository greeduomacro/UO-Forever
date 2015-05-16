#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Accounting;
using Server.Engines.CustomTitles;
using Server.Mobiles;
using VitaNex;

#endregion

namespace Server.Engines.EventScores
{
    public sealed class PlayerEventScoreProfile : PropertyObject
    {
        [CommandProperty(EventScores.Access, true)]
        public List<PlayerMobile> Players { get; set; }

        [CommandProperty(EventScores.Access, true)]
        public PlayerMobile DisplayCharacter { get; set; }

        [CommandProperty(EventScores.Access, true)]
        public int OverallScore { get; set; }

        [CommandProperty(EventScores.Access, true)]
        public int SpendablePoints { get; set; }

        [CommandProperty(EventScores.Access, true)]
        public Dictionary<string, int> Invasions { get; set; }

        [CommandProperty(EventScores.Access, true)]
        public Dictionary<string, int> Battles { get; set; }

        [CommandProperty(EventScores.Access, true)]
        public Dictionary<string, int> Tournaments { get; set; }

        [CommandProperty(EventScores.Access, true)]
        public List<EventObject> Events { get; set; }

        public PlayerEventScoreProfile(PlayerMobile pm)
        {
            DisplayCharacter = pm;
            OverallScore = 0;
            SpendablePoints = 0;
            Invasions = new Dictionary<string, int>();
            Battles = new Dictionary<string, int>();
            Tournaments = new Dictionary<string, int>();
            Players = new List<PlayerMobile>();
            Events = new List<EventObject>();
        }

        public PlayerEventScoreProfile(GenericReader reader)
            : base(reader)
        {}

        public override void Reset()
        {}

        public override void Clear()
        {}

        public void AddInvasion(EventInvasions.Invasion invasion, PlayerMobile player)
        {
            if (invasion.ParticipantsScores != null && invasion.ParticipantsScores.ContainsKey(player))
            {
                var score = (int) Math.Ceiling((double) invasion.ParticipantsScores[player] / 500);
                foreach (var ev in Events)
                {
                    if (ev.EventName == "Invasion of the Damned")
                    {
                        if (ev.PointsGained < 500)
                        {
                            int pointsleft = 500 - ev.PointsGained;
                            if (score > pointsleft)
                            {
                                score = pointsleft;
                            }
                            ev.PointsGained += score;
                            OverallScore += score;
                            SpendablePoints += score;
                            if (DisplayCharacter != null)
                                DisplayCharacter.SendMessage(54, "You have been awarded: " + score + " event points for " + player.RawName +"'s contribution to the invasion.");
                            return;
                        }
                        return;
                    }
                }

                if (score > 500)
                    score = 500;

                if (Events != null)
                {
                    Events.Add(new EventObject(EventType.Invasions, invasion.InvasionName, DateTime.Now, score));
                }

                player.SendMessage(54, "You have been awarded: " + score + " event points.");

                OverallScore += score;
                SpendablePoints += score;
            }
        }

        public bool AddScav(string scavname, int score, PlayerMobile player)
        {
            if (Events == null)
                return false;
            if (Events.Any(eventObject => eventObject.EventName == scavname))
            {
                return false;
            }

            Events.Add(new EventObject(EventType.Scavenger, scavname, DateTime.Now, score));

            player.SendMessage(54, "You have been awarded: " + score + " event points.");

            OverallScore += score;
            SpendablePoints += score;

            return true;
        }

        public void AddBattle(string battlename, int score, PlayerMobile player)
        {
            /*if (Battles != null && Battles.ContainsKey(battlename))
            {
                Battles[battlename] += score;
            }
            else if (Battles != null && !Battles.ContainsKey(battlename))
            {
                Battles.Add(battlename, score);
            }*/

            if (Events != null)
            {
                Events.Add(new EventObject(EventType.Battles, battlename, DateTime.Now, score));
            }

            player.SendMessage(54, "You have been awarded: " + score + " event points.");

            OverallScore += score;
            SpendablePoints += score;
        }

        public void AddTournament(string tournamentname, int score, PlayerMobile player)
        {
            /*if (Tournaments != null && Tournaments.ContainsKey(tournamentname))
            {
                Tournaments[tournamentname] += score;
            }
            else if (Tournaments != null && !Tournaments.ContainsKey(tournamentname))
            {
                Tournaments.Add(tournamentname, score);
            }*/

            if (Events != null)
            {
                Events.Add(new EventObject(EventType.Tournaments, tournamentname, DateTime.Now, score));
            }

            player.SendMessage(54, "You have been awarded: " + score + " event points");

            OverallScore += score;
            SpendablePoints += score;
        }

        public void GetScores(PlayerMobile from)
        {
            int battlescore = 0;
            battlescore += Battles.Sum(battle => battle.Value);

            int tournyscore = 0;
            tournyscore += Tournaments.Sum(tourn => tourn.Value);

            int invadescore = 0;
            invadescore += Invasions.Sum(invade => invade.Value);

            from.SendMessage(54, "Your overall score is " + OverallScore);
            from.SendMessage(54, "Your battles score is " + battlescore);
            from.SendMessage(54, "Your tournaments score is " + tournyscore);
            from.SendMessage(54, "Your invasions score is " + invadescore);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(2);

            switch (version)
            {
                case 2:
                    {
                        writer.Write(Events.Count);

                        if (Events.Count > 0)
                        {
                            foreach (EventObject eventobj in Events)
                            {
                                eventobj.Serialize(writer);
                            }
                        }
                    }
                    goto case 1;
                case 1:
                {
                    writer.Write(DisplayCharacter);
                    writer.Write(SpendablePoints);
                }
                    goto case 0;
                case 0:
                {
                    writer.Write(OverallScore);

                    writer.Write(Players.Count);

                    if (Players.Count > 0)
                    {
                        foreach (PlayerMobile player in Players)
                        {
                            writer.Write(player);
                        }
                    }

                    writer.Write(Invasions.Count);

                    if (Invasions.Count > 0)
                    {
                        foreach (KeyValuePair<string, int> score in Invasions)
                        {
                            writer.Write(score.Key);
                            writer.Write(score.Value);
                        }
                    }

                    writer.Write(Tournaments.Count);

                    if (Tournaments.Count > 0)
                    {
                        foreach (KeyValuePair<string, int> score in Tournaments)
                        {
                            writer.Write(score.Key);
                            writer.Write(score.Value);
                        }
                    }

                    writer.Write(Battles.Count);

                    if (Battles.Count > 0)
                    {
                        foreach (KeyValuePair<string, int> score in Battles)
                        {
                            writer.Write(score.Key);
                            writer.Write(score.Value);
                        }
                    }
                }
                    break;
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            Players = new List<PlayerMobile>();
            Tournaments = new Dictionary<string, int>();
            Battles = new Dictionary<string, int>();
            Invasions = new Dictionary<string, int>();
            Events = new List<EventObject>();

            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                    {
                        int count = reader.ReadInt();

                        if (count > 0)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                var level = new EventObject(reader);
                                Events.Add(level);
                            }
                        }
                    }
                    goto case 1;
                case 1:
                {
                    DisplayCharacter = reader.ReadMobile<PlayerMobile>();
                    SpendablePoints = reader.ReadInt();
                }
                    goto case 0;
                case 0:
                {
                    OverallScore = reader.ReadInt();

                    int count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var player = reader.ReadMobile<PlayerMobile>();
                            if (player != null)
                            Players.Add(player);
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            string name = reader.ReadString();
                            int score = reader.ReadInt();
                            Invasions.Add(name, score);
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            string name = reader.ReadString();
                            int score = reader.ReadInt();
                            Tournaments.Add(name, score);
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            string name = reader.ReadString();
                            int score = reader.ReadInt();
                            Battles.Add(name, score);
                        }
                    }
                }
                    break;
            }
        }
    }
}