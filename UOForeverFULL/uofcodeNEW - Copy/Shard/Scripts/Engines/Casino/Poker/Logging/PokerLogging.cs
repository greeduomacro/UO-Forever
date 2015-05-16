#region References
using System;
using System.IO;
using System.Text;

using Server.Accounting;
using Server.Multis;
using Server.Network;
using Server.Poker;
using VitaNex;
using VitaNex.IO;
#endregion

namespace Server.Commands
{
	public class PokerLogging
	{
		private static StreamWriter m_Output;
		private static bool m_Enabled = true;

		public static bool Enabled { get { return m_Enabled; } set { m_Enabled = value; } }

		public static StreamWriter Output { get { return m_Output; } }

		public static void createPokerLog(string tablename)
		{
			if (!Directory.Exists("Logs"))
			{
				Directory.CreateDirectory("Logs");
			}

			const string directory = "Logs/Poker";

			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

            if (!File.Exists("Logs/Poker/" + tablename + ".log"))
		    {
		        try
		        {
		            m_Output = new StreamWriter(
		                Path.Combine(directory, String.Format("{0}.log", tablename)), true)
		            {
		                AutoFlush = true
		            };

		            m_Output.WriteLine("##############################");
		            m_Output.WriteLine("Log started on {0}", DateTime.UtcNow);
		            m_Output.WriteLine();
                    m_Output.Close();
		        }
		        catch
		        {}
		    }
		}

	    public static void StartNewHand(string tablename)
	    {
	        try
	        {
                using (var op = new StreamWriter("Logs/Poker/" + tablename + ".log", true))
                {
                    op.WriteLine(
                        "----------------------------------------------------");
                    op.WriteLine("New hand started on {0}", DateTime.UtcNow);
                    op.Close();
                }
	        }
	        catch
	        {
	        }
	    }

        public static void EndHand(string tablename)
        {
            try
            {
                using (var op = new StreamWriter("Logs/Poker/" + tablename + ".log", true))
                {
                    op.WriteLine("Hand ended at {0}", DateTime.UtcNow);
                    op.WriteLine(
                        "----------------------------------------------------");
                    op.Close();
                }
            }
            catch
            {
            }
        }

        public static void TotalRaked(string tablename, int raked, bool isDonation)
        {
            try
            {
                using (var op = new StreamWriter("Logs/Poker/" + tablename + ".log", true))
                {
                    op.WriteLine(
    "****************************************************");
                    op.WriteLine(
                        "Total raked this game: {0:#,0} {1}.",
                        raked,
                        (isDonation ? "donation coins" : "gold"));
                    op.WriteLine(
    "****************************************************");
                    op.Close();
                }
            }
            catch
            {
            }
        }

        public static void EndHandCurrency(string tablename, PokerPlayer player, bool isDonation)
        {
            try
            {
                using (var op = new StreamWriter("Logs/Poker/" + tablename + ".log", true))
                {
                    op.WriteLine("{0} ended the hand with {1:#,0} {2}.", player.Mobile.Name,
                        player.Currency,
                        (isDonation ? "donation coins" : "gold"));
                    op.Close();
                }
            }
            catch
            { }
        }

        public static void HandWinnings(string tablename, PokerPlayer player, int amount, bool isDonation)
        {
            try
            {
                using (var op = new StreamWriter("Logs/Poker/" + tablename + ".log", true))
                {
                    op.WriteLine("{0} has won {1:#,0} {2}.", player.Mobile.RawName,
                        amount,
                        (isDonation ? "donation coins" : "gold"));
                    op.Close();
                }
            }
            catch
            { }
        }


        public static void PokerPlayerAction(PokerPlayer player, PlayerAction action, string tablename, bool isDonation)
        {
            try
            {
                using (var op = new StreamWriter("Logs/Poker/" + tablename + ".log", true))
                {

                        op.WriteLine(
                            "Player {0} with account {1} {4} for {2:#,0} {3}.",
                            player.Mobile.Name,
                            player.Mobile.Account == null ? "(-null-)" : player.Mobile.Account.Username,
                            player.Bet,
                            (isDonation ? "donation coins" : "gold"),
                            player.Action);
                }
            }
            catch
            { }
        }
	}
}