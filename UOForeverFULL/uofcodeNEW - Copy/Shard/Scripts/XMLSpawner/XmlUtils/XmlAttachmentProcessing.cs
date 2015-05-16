#region References
using System;
using System.Collections;
using System.Collections.Generic;

using Server.Commands;
using Server.Engines.XmlSpawner2;

using CPA = Server.CommandPropertyAttribute;
#endregion

/*
** 
 *  based on XmlFind
** utility for locating objects in the world.
** ArteGordon
** original version 1.0
** 4/13/04
**
*/

namespace Server.Mobiles
{
	public class XmlAttachmentProcessing
	{
		/*
	    public class XmlAttachmentProcessingThread
        {
            string[] m_names;
            Mobile m_From;

            public XmlAttachmentProcessingThread(Mobile from, string input_filename, string output_filename)
            {
                try
                {
                    System.IO.StreamReader file =
                       new System.IO.StreamReader(input_filename);
                    // file format is:
                    // formula
                    string firstline = file.ReadLine();
                    string[] args = firstline.Split();
                    
                    file.Close();
                    if (from != null) { from.SendMessage("Successfully processed"); }
                }
                catch (Exception e)
                {
                    using (StreamWriter writer = new StreamWriter("ERROR-XmlAttachmentProcessing.txt", true))
                    {
                        writer.WriteLine("ERROR: " + e.Message);
                    }
                    if (from != null) { from.SendMessage("Error: " + e.Message); }
                }
                m_From = from;
            }


            public void XmlAttachmentProcessingThreadMain()
            {
                //if (m_From == null || m_type == null || m_name == null || m_props == null) return;

                //string status_str;

                //ArrayList results = XmlAttachmentProcessing.Search(, out status_str);
                
                // display the status_str to the one who called it                
            }
        }*/

		public static void Initialize()
		{
			CommandSystem.Register("XmlFindMaxValue", AccessLevel.GameMaster, XmlFindMaxValue_OnCommand);
			CommandSystem.Register("XmlTestReward", AccessLevel.GameMaster, XmlTestReward_OnCommand);
		}

		public static void XmlFindMaxValue_OnCommand(CommandEventArgs e)
		{
			var args = e.Arguments;
			if (args.Length == 0)
			{
				e.Mobile.SendMessage("You must provide an XmlValue name as an argument!");
				return;
			}

			var list = Search(args[0]);
			if (list.Count > 0)
			{
				e.Mobile.SendMessage(args[0] + " highest score:  " + list[0].Value + " by " + list[0].Owner);
			}
			else
			{
				e.Mobile.SendMessage("No playermobiles found with that XmlValue!");
			}
		}

		public static List<XmlValue> Search(string name)
		{
			var sortedXmlValues = new List<XmlValue>();
			foreach (Mobile mob in World.Mobiles.Values)
			{
				if (!(mob is PlayerMobile))
				{
					continue;
				}

				ArrayList alist = XmlAttach.FindAttachments(mob);
				if (alist != null)
				{
					foreach (XmlAttachment a in alist)
					{
						if (a is XmlValue && !a.Deleted && a.Name == name)
						{
							XmlValue xmlValue = a as XmlValue;
							int i = 0;
							bool inserted = false;
							// sort from highest to lowest value
							while (i < sortedXmlValues.Count)
							{
								if (sortedXmlValues[i].Value < xmlValue.Value)
								{
									sortedXmlValues.Insert(i, xmlValue);
									inserted = true;
									break;
								}
								i++;
							}
							if (!inserted) // append to the end
							{
								sortedXmlValues.Add(xmlValue);
							}
						}
						//if (a != null && !a.Deleted &&  )
						//{
						//a.OnBeforeKill(m_killed, m_killer);
						//}
					}
				}
			}
			return sortedXmlValues;
		}

		public static void XmlTestReward_OnCommand(CommandEventArgs e)
		{
			string status_str = null;
			// the syntax is [xmltestreward xmlvalueName highest|linear|exponential platinumAmount
			// 

			string xmlvalueName = null;
			string scaletype = null;

			int platinumAmount = 0;
			try
			{
				xmlvalueName = e.Arguments[0];
				scaletype = e.Arguments[1].ToLower();
				if (scaletype != "highest" && scaletype != "linear" && scaletype != "exponential" && scaletype != "exponential2" &&
					scaletype != "uniform")
				{
					throw new Exception();
				}
				platinumAmount = int.Parse(e.Arguments[2]);
			}
			catch
			{
				e.Mobile.SendMessage(
					"Usage: [xmltestreward xmlvalueName highest|linear|exponential1|exponential2|uniform PlatinumAmount");
				return;
			}

			// disburse the platinum reward and/or rare to the one with the highest score
			var sortedValues = Search(xmlvalueName);
			try
			{
				if (sortedValues.Count > 0 && sortedValues[0].Value != 0)
				{
					int total = 0;
					int top20Processed = 0;
					if (scaletype == "highest")
					{
						Mobile owner = sortedValues[0].Owner as Mobile;
						if (owner != null && !owner.Deleted)
						{
							e.Mobile.SendMessage(owner.Name + ": " + platinumAmount);
							total = platinumAmount;
						}
					}
					else if (scaletype == "linear")
					{
						Mobile owner = sortedValues[0].Owner as Mobile;
						double maxValue = sortedValues[0].Value;
						if (maxValue > 0)
						{
							double slope = platinumAmount / maxValue;

							foreach (XmlValue xmlValue in sortedValues)
							{
								owner = xmlValue.Owner as Mobile;
								int awardedAmount = (int)Math.Round(xmlValue.Value * slope);

								if (owner != null && !owner.Deleted)
								{
									total += awardedAmount;
									top20Processed++;
									if (top20Processed < 20)
									{
										e.Mobile.SendMessage(owner.Name + ": " + awardedAmount);
									}
								}
							}
						}
					} //ASSOCIATION, Teams, Gumps, PARTY ACCESS, 
					else if (scaletype == "exponential")
					{
						double maxValue = sortedValues[0].Value;
						if (maxValue > 0)
						{
							double slope = 1.0 / maxValue;
							foreach (XmlValue xmlValue in sortedValues)
							{
								Mobile owner = xmlValue.Owner as Mobile;
								double linearScaledValue = xmlValue.Value * slope; // 0-1
								double exponentialParameter = 2.0; // arbitrarily set to increase the slope at lower levels
								int awardedAmount =
									(int)Math.Round((1.0 - Math.Exp(-linearScaledValue * exponentialParameter)) * platinumAmount);
								if (owner != null && !owner.Deleted)
								{
									total += awardedAmount;
									top20Processed++;
									if (top20Processed < 20)
									{
										e.Mobile.SendMessage(owner.Name + ": " + awardedAmount);
									}
								}
							}
						}
					}
					else if (scaletype == "exponential2")
					{
						double maxValue = sortedValues[0].Value;
						if (maxValue > 0)
						{
							double slope = 1.0 / maxValue;
							foreach (XmlValue xmlValue in sortedValues)
							{
								Mobile owner = xmlValue.Owner as Mobile;
								double linearScaledValue = xmlValue.Value * slope; // 0-1
								double exponentialParameter = 6.0; // arbitrarily set to increase the slope at lower levels
								int awardedAmount =
									(int)Math.Round((1.0 - Math.Exp(-linearScaledValue * exponentialParameter)) * platinumAmount);
								if (owner != null && !owner.Deleted)
								{
									total += awardedAmount;
									top20Processed++;
									if (top20Processed < 20)
									{
										e.Mobile.SendMessage(owner.Name + ": " + awardedAmount);
									}
								}
							}
						}
					}
					else if (scaletype == "uniform")
					{
						// give everybody that amount of platinum if they have at least 1 point
						foreach (XmlValue xmlValue in sortedValues)
						{
							Mobile owner = xmlValue.Owner as Mobile;
							if (xmlValue.Value > 0 && owner != null && !owner.Deleted)
							{
								total += platinumAmount;
								top20Processed++;
								if (top20Processed < 20)
								{
									e.Mobile.SendMessage(owner.Name + ": " + platinumAmount);
								}
							}
						}
					}
					else
					{
						status_str = "bad scaletype arg in REWARD";
					}
					e.Mobile.SendMessage(38, "Total platinum that would be awarded = " + total);
				}
				else
				{
					e.Mobile.SendMessage("There were no XmlValues with the name " + xmlvalueName);
				}
			}
			catch
			{ }
			if (status_str != null)
			{
				e.Mobile.SendMessage(status_str);
			}
		}

		/*
        [Usage("XmlFind [objecttype] [range]")]
        [Description("Finds objects in the world")]
        public static void XmlFind_OnCommand(CommandEventArgs e)
        {
            if (e == null || e.Mobile == null) return;

            Account acct = e.Mobile.Account as Account;
            int x = 0;
            int y = 0;
            XmlSpawnerDefaults.DefaultEntry defs = null;
            if (acct != null)
                defs = XmlSpawnerDefaults.GetDefaults(acct.ToString(), e.Mobile.Name);

            if (defs != null)
            {
                x = defs.FindGumpX;
                y = defs.FindGumpY;
            }

            string typename = "Xmlspawner";
            int range = -1;
            bool dorange = false;

            if (e.Arguments.Length > 0)
            {
                typename = e.Arguments[0];
            }

            if (e.Arguments.Length > 1)
            {
                dorange = true;
                try
                {
                    range = int.Parse(e.Arguments[1]);
                }
                catch
                {
                    dorange = false;
                    e.Mobile.SendMessage("Invalid range argument {0}", e.Arguments[1]);
                }
            }

            e.Mobile.SendGump(new XmlFindGump(e.Mobile, e.Mobile.Location, e.Mobile.Map, typename, range, dorange, x, y));
        }

        private void SortFindList()
        {
            if (m_SearchList != null && m_SearchList.Count > 0)
            {
                if (Sorttype)
                {
                    this.m_SearchList.Sort(new ListTypeSorter(Descendingsort));
                }
                else
                    if (Sortname)
                    {
                        this.m_SearchList.Sort(new ListNameSorter(Descendingsort));
                    }
                    else
                        if (Sortmap)
                        {
                            this.m_SearchList.Sort(new ListMapSorter(Descendingsort));
                        }
                        else
                            if (Sortrange)
                            {
                                this.m_SearchList.Sort(new ListRangeSorter(m_From, Descendingsort));
                            }
                            else
                                if (Sortselect)
                                {
                                    this.m_SearchList.Sort(new ListSelectSorter(m_From, Descendingsort));
                                }
            }
        }

        private class ListTypeSorter : IComparer
        {
            private bool Dsort;

            public ListTypeSorter(bool descend)
                : base()
            {
                Dsort = descend;
            }

            public int Compare(object e1, object e2)
            {
                object x = null;
                object y = null;
                if (e1 is SearchEntry)
                    x = ((SearchEntry)e1).Object;
                if (e2 is SearchEntry)
                    y = ((SearchEntry)e2).Object;

                string xstr = null;
                string ystr = null;
                string str = null;
                if (x is Item)
                {
                    str = ((Item)x).GetType().ToString();
                }
                else
                    if (x is Mobile)
                    {
                        str = ((Mobile)x).GetType().ToString();
                    }
                if (str != null)
                {
                    string[] arglist = str.Split('.');
                    xstr = arglist[arglist.Length - 1];
                }

                str = null;
                if (y is Item)
                {
                    str = ((Item)y).GetType().ToString();
                }
                else
                    if (y is Mobile)
                    {
                        str = ((Mobile)y).GetType().ToString();
                    }
                if (str != null)
                {
                    string[] arglist = str.Split('.');
                    ystr = arglist[arglist.Length - 1];
                }
                if (Dsort)
                    return String.Compare(ystr, xstr, true);
                else
                    return String.Compare(xstr, ystr, true);
            }
        }

        private class ListNameSorter : IComparer
        {
            private bool Dsort;

            public ListNameSorter(bool descend)
                : base()
            {
                Dsort = descend;
            }

            public int Compare(object e1, object e2)
            {
                object x = null;
                object y = null;
                if (e1 is SearchEntry)
                    x = ((SearchEntry)e1).Object;
                if (e2 is SearchEntry)
                    y = ((SearchEntry)e2).Object;

                string xstr = null;
                string ystr = null;

                if (x is Item)
                {
                    xstr = ((Item)x).Name;
                }
                else
                    if (x is Mobile)
                    {
                        xstr = ((Mobile)x).Name;
                    }

                if (y is Item)
                {
                    ystr = ((Item)y).Name;
                }
                else
                    if (y is Mobile)
                    {
                        ystr = ((Mobile)y).Name;
                    }
                if (Dsort)
                    return String.Compare(ystr, xstr, true);
                else
                    return String.Compare(xstr, ystr, true);
            }
        }
        */
	}
}