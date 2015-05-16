// Generate a list of accounts inactive more than 90 days.  Write list to text file in server root.
// List to include account name, player name, creation date, last login date, house check and house location

using System;
using System.Collections;
using System.IO;
using Server.Accounting;
using Server.Multis;

namespace Server.Commands
{
    public class GenInactives
    {

        public static void Initialize()
        {
            CommandSystem.Register("GenInactives", AccessLevel.Administrator, GenInactives_OnCommand);
        }

        [Usage("GenInactives")]
        [Description("Generate a list of accounts inactive more than 90 days.")]
        public static void GenInactives_OnCommand(CommandEventArgs e)
        {
            DateTime minTime = DateTime.Now - TimeSpan.FromDays(90.0); //Change the "90" to adjust inactive time.

            foreach (var account in Accounts.GetAccounts())           //Start the loop through the accounts
            {
                var acct = (Account) account;
                if (acct != null && acct.Username != null)             //null check for accounts
                {
                    if (acct.LastLogin <= minTime)                     //if the account meets the inactive time requirement, do the following
                    {
                        // Begin check to see if account is on the Hold List
                        string tag = acct.GetTag("holdacct");
                        string hashold = tag != "true" ? "- - -" : "HOLD";
                        // End check 
                        // Begin check to see if Player has a house	
                        Mobile m = acct[0];                            //Get the first player in the account and assign to variable m

                        String hashouse;
                        if (m != null)
                        {
                            hashouse = BaseHouse.HasAccountHouse(m) ? "Yes" : "No";
                        }
                        else hashouse = "No";
                        // End check
                        ArrayList list = new ArrayList();           //<---- Start new arraylist called "list"

                        list.AddRange(BaseHouse.GetHouses(m));      //<---- add to the "list" all houses of mobile m

                        if (list.Count != 0)                        //<---- make sure the list is not null
                        {
                            BaseHouse sel = list[0] as BaseHouse;   //<---- here I want to get to start to get information on the first house in the list
                            //<---- and want to then pull the map and coordinates for that house (like 2554 3401 20 )   
                            Map map = sel.Map;                      //<---- set variable map to the map that is sel.Map

                            const string FileName = "Inactives.txt";

                            if (File.Exists(FileName))
                            {
                                StreamWriter sr = File.AppendText(FileName);
                                sr.WriteLine("Acct: {0} Player: {1} Created: {2} Inactive since: {3} House: {4} Location: {5} Map: {6} Hold: {7}", acct.Username, ((m != null) ? m.Name : "No Player"), acct.Created.ToString("d"), acct.LastLogin.ToString("d"), hashouse, sel.Location.ToString(), map.Name, hashold);
                                sr.Flush();
                                sr.Close();
                            }
                            else
                            {
                                StreamWriter tw = new StreamWriter(FileName);
                                tw.WriteLine("Acct: {0} Player: {1} Created: {2} Inactive since: {3} House: {4} Location: {5} Map: {6} Hold: {7}", acct.Username, ((m != null) ? m.Name : "No Player"), acct.Created.ToString("d"), acct.LastLogin.ToString("d"), hashouse, sel.Location.ToString(), map.Name, hashold);
                                tw.Flush();
                                tw.Close();
                            }
                        }
                    }
                }
            }
            e.Mobile.SendMessage("Inactive accounts have been written to UOForever Root");
            Console.WriteLine("Inactive accounts have been written to UOForever Root");
        }
    }
}