using System;
using System.Linq;
using Server;
using Server.Items;
using Server.Guilds;
using Server.Mobiles;
using Server.Gumps;
using Server.Spells;
using Server.Engines.MurderSystem;

namespace Server.Misc
{
	public class Keywords
	{
		public static void Initialize()
		{
			// Register our speech handler
			EventSink.Speech += new SpeechEventHandler( EventSink_Speech );
		}

        public static string FormatTimeSpan(TimeSpan ts)
        {
            return String.Format("{0:D2} Days, {1:D2} Hours, {2:D2} Minutes and {3:D2} Seconds.", ts.Days, ts.Hours % 24, ts.Minutes % 60, ts.Seconds % 60);
        }

		public static void EventSink_Speech( SpeechEventArgs args )
		{
			Mobile from = args.Mobile;
			PlayerMobile speaker = from as PlayerMobile;
            if (speaker == null) return; // in case they are pseudoseering

			int[] keywords = args.Keywords;

            if (args.Speech.ToLower().IndexOf("i wish to buy my head") > -1)
            {
                var turnedinhead = BaseShieldGuard.PendingStatloss.FirstOrDefault(x => x.HeadOwner == speaker);
                if (turnedinhead != null)
                {
                    if (Banker.Withdraw(speaker, typeof(Gold), 30000))
                    {
                        BaseShieldGuard.PendingStatloss.Remove(turnedinhead);
                        from.SendMessage(61, "You have successfully purchased your head back from the guards.");
                        turnedinhead.TurnedIn.BankBox.DropItem(new Gold(25000));
                        turnedinhead.TurnedIn.SendMessage(61, "You have received 25,000gp for the head of " + from.RawName + ".");
                    }
                    else
                    {
                        from.SendMessage(61, "You do not have enough gold to buy back your head from the guards!");
                    }
                }
                else
                {
                    foreach (Head2 head in Head2.AllHeads)
                    {
                        if (head == null)
                            continue;
                        if (head.Player == from)
                        {
                            from.SendMessage("Select a player with whom you wish to negotiate for your head!");
                            from.CloseGump(typeof(HeadOwnerListGump));
                            from.SendGump(new HeadOwnerListGump(from));
                            return;
                        }
                    }
                    from.SendMessage("Nobody in possession of your head was found.");
                }
            }

			for ( int i = 0; i < keywords.Length; ++i )
			{
				switch ( keywords[i] )
				{
					case 0x002A: // *i resign from my guild*
					{
						if ( from.Guild != null )
						{
							if ( SpellHelper.CheckCombat( from ) || PublicMoongate.CheckCombat( from ) )
								from.SendMessage( "You cannot leave your guild while in combat." );
							else
								((Guild)from.Guild).RemoveMember( from );
						}

						break;
					}
					case 0x0032: // *i must consider my sins*
					{
                        speaker.CheckKillDecay();
					    from.SendMessage( 32,"Murder Counts : {0}",  from.Kills );
                        if (speaker.InStat)  //check to see if statend time is still greater than current time
                        {
                            TimeSpan remaining = (speaker.StatEnd - DateTime.UtcNow);
                            from.SendMessage(32, "You are in statloss for: {0}", FormatTimeSpan(remaining));
                        }
                        else
                        {  //always try to clear the stat loss at this time
                            // note that it's checked on a 30 minute timer (in FoodDecay.cs)
                            // so this could save them a little time
                            BaseShieldGuard.StatLossDecay(speaker);
                        }
						/*if ( from.ShortTermMurders < Mobile.MurderCount )
						{
							if ( from.Kills < Mobile.MurderCount ) // not red, no stat loss
							{
								if ( from.ShortTermMurders <= 0 )
									from.SendLocalizedMessage( 502122, Notoriety.Hues[Notoriety.Innocent] ); // Fear not, thou hast not slain the innocent.
								else
									from.SendLocalizedMessage( 502124, Notoriety.Hues[Notoriety.Innocent] ); // Fear not, thou hast not slain the innocent in some time...
							}
							else
								from.SendLocalizedMessage( 502123, Notoriety.Hues[Notoriety.Innocent] ); // You are known throughout the land as a murderous brigand.
						}
						else // stat loss
						{
							if ( from.Kills < Mobile.MurderCount ) //not red
								from.SendLocalizedMessage( 502126, Notoriety.Hues[Notoriety.Murderer] ); // If thou should return to the land of the living, the innocent shall wreak havoc upon thy soul.
							else
								from.SendLocalizedMessage( 502123, Notoriety.Hues[Notoriety.Murderer] ); // You are known throughout the land as a murderous brigand.
						}
/*
						if( !Core.SE )
						{
							from.SendMessage( "Short Term Murders : {0}", from.ShortTermMurders );
							from.SendMessage( "Long Term Murders : {0}",  from.Kills );
						}
						else
						{
							from.SendMessage( 0x3B2, "Short Term Murders: {0} Long Term Murders: {1}", from.ShortTermMurders, from.Kills );
						}
*/
						break;
					}
					case 0x0035: // i renounce my young player status*
					{
						if ( from is PlayerMobile && ((PlayerMobile)from).Young && !from.HasGump( typeof( RenounceYoungGump ) ) )
						{
							from.SendGump( new RenounceYoungGump() );
						}

						break;
					}
				}
			}
		}
	}

}