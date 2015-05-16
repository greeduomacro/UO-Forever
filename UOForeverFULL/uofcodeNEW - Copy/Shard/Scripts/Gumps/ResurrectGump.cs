using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server.Gumps
{
	public enum ResurrectMessage
	{
		ChaosShrine = 0,
		VirtueShrine = 1,
		Healer = 2,
		Generic = 3,
	}

	public class ResurrectGump : Gump
	{
		private Mobile m_Healer;
		private int m_Price;
		private bool m_FromSacrifice;
		private double m_HitsScalar;

		public ResurrectGump( Mobile owner )
			: this( owner, owner, ResurrectMessage.Generic, false )
		{
		}

		public ResurrectGump( Mobile owner, double hitsScalar )
			: this( owner, owner, ResurrectMessage.Generic, false, hitsScalar )
		{
		}

		public ResurrectGump( Mobile owner, bool fromSacrifice )
			: this( owner, owner, ResurrectMessage.Generic, fromSacrifice )
		{
		}

		public ResurrectGump( Mobile owner, Mobile healer )
			: this( owner, healer, ResurrectMessage.Generic, false )
		{
		}

		public ResurrectGump( Mobile owner, ResurrectMessage msg )
			: this( owner, owner, msg, false )
		{
		}

		public ResurrectGump( Mobile owner, Mobile healer, ResurrectMessage msg )
			: this( owner, healer, msg, false )
		{
		}

		public ResurrectGump( Mobile owner, Mobile healer, ResurrectMessage msg, bool fromSacrifice )
			: this( owner, healer, msg, fromSacrifice, 0.0 )
		{
		}

		public ResurrectGump( Mobile owner, Mobile healer, ResurrectMessage msg, bool fromSacrifice, double hitsScalar )
			: base( 100, 0 )
		{
			m_Healer = healer;
			m_FromSacrifice = fromSacrifice;
			m_HitsScalar = hitsScalar;

			owner.CloseGump( typeof( ResurrectGump ) );

			AddPage( 0 );

			AddBackground( 0, 0, 400, 350, 2600 );

			AddHtmlLocalized( 0, 20, 400, 35, 1011022, false, false ); // <center>Resurrection</center>

			AddHtmlLocalized( 50, 55, 300, 140, 1011023 + (int)msg, true, true ); /* It is possible for you to be resurrected here by this healer. Do you wish to try?<br>
																				   * CONTINUE - You chose to try to come back to life now.<br>
																				   * CANCEL - You prefer to remain a ghost for now.
																				   */

			AddButton( 200, 227, 4005, 4007, 0, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 235, 230, 110, 35, 1011012, false, false ); // CANCEL

			AddButton( 65, 227, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 100, 230, 110, 35, 1011011, false, false ); // CONTINUE
		}

		public ResurrectGump( Mobile owner, Mobile healer, int price )
			: base( 150, 50 )
		{
			owner.CloseGump( typeof( ResurrectGump ) );

			m_Healer = healer;
			m_Price = price;
			int newprice = (int)Math.Round( m_Price * 1.5 );

			Closable = false;

			AddPage( 0 );

			AddImage( 0, 0, 3600 );

			AddImageTiled( 0, 14, 15, 260, 3603 );
			AddImageTiled( 380, 14, 14, 260, 3605 );

			AddImage( 0, 261, 3606 );

			AddImageTiled( 15, 261, 370, 16, 3607 );
			AddImageTiled( 15, 0, 370, 16, 3601 );

			AddImage( 380, 0, 3602 );

			AddImage( 380, 261, 3608 );

			AddImageTiled( 15, 15, 365, 250, 2624 );

			AddRadio( 30, 150, 9727, 9730, true, 2 );
			AddLabel( 65, 150, 1500, "Grudgingly pay the money." ); // Grudgingly pay the money

			AddRadio( 30, 190, 9727, 9730, false, 1 );
			AddLabel( 65, 192, 1500, "I'll resurrect without paying and take my chances." ); // I'd rather stay dead, you scoundrel!!!

			AddRadio( 30, 230, 9727, 9730, false, 0 );
			AddLabel( 65, 232, 1500, "I'd rather stay dead, you scoundrel!!!"); // I'd rather stay dead, you scoundrel!!!
			
			AddLabel( 25, 20, 1645, "Wishing to rejoin the living are you, criminal?  I can" ); // Wishing to rejoin the living, are you?  I can restore your body... for a price of course...
			AddLabel( 25, 35, 1645, "restore your body and skills intact... for the price of");
			AddLabel( 25, 50, 1645, "your bounty ...");			
			AddHtmlLocalized( 30, 105, 345, 40, 1060018, 0x5B2D, false, false ); // Do you accept the fee, which will be withdrawn from your bank?

			AddImage( 65, 72, 5605 );

			AddImageTiled( 80, 90, 200, 1, 9107 );
			AddImageTiled( 95, 92, 200, 1, 9157 );

			AddLabel( 90, 70, 1645, newprice.ToString() );
			AddHtmlLocalized( 140, 70, 100, 25, 1023823, 0x7FFF, false, false ); // gold coins

			AddButton( 290, 230, 247, 248, 2, GumpButtonType.Reply, 0 );


		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
			PlayerMobile spm = from as PlayerMobile;

			from.CloseGump( typeof( ResurrectGump ) );

			if( info.ButtonID == 1 || info.ButtonID == 2 )
			{
				if( from.Map == null || !from.Map.CanFit( from.Location, 16, false, false ) )
				{
					from.SendLocalizedMessage( 502391 ); // Thou can not be resurrected there!
					return;
				}

				/*
				if( m_Price > 0 )
				{
				
					m_Price = (int)Math.Round( m_Price * 1.5 );
					if( info.IsSwitched( 2 ) )
					{
						if( Banker.Withdraw( from, m_Price ) )
						{
							from.SendLocalizedMessage( 1060398, m_Price.ToString() ); // ~1_AMOUNT~ gold has been withdrawn from your bank box.
							from.SendLocalizedMessage( 1060022, Banker.GetBalance( from ).ToString() ); // You have ~1_AMOUNT~ gold in cash remaining in your bank box.
							spm.MurderBounty = 0;
						}
						else
						{
							from.SendLocalizedMessage( 1060020 ); // Unfortunately, you do not have enough cash in your bank to cover the cost of the healing.
							return;
						}
					}
					if( info.IsSwitched( 1 ) )
					{
						if ( spm != null )
						{
							ApplySkillLoss( from );
							spm.MurderBounty = 0;
						}
					}
					if( info.IsSwitched( 0 ) )
					{
						from.SendLocalizedMessage( 1060019 ); // You decide against paying the healer, and thus remain dead.
						return;
					}
				}
				*/

				from.PlaySound( 0x214 );
				from.FixedEffect( 0x376A, 10, 16 );

				from.Resurrect();

				if( from.Fame > 0 )
				{
					int amount = from.Fame / 10;

					Misc.Titles.AwardFame( from, -amount, true );
				}

				/*if ( spm != null && !spm.FactionDeath )
					ApplySkillLoss( from );*/

				if ( from.Alive && m_HitsScalar > 0 )
					from.Hits = (int)(from.HitsMax * m_HitsScalar);
			}
		}

		#region Skill/Stat Loss
		private static Dictionary<Mobile, SkillLossContext> m_SkillLoss = new Dictionary<Mobile, SkillLossContext>();

		private class SkillLossContext
		{
			public Timer m_Timer;
			public List<SkillMod> m_SkillMods;
		}

		public static void ApplySkillLoss( Mobile mob )
		{
			PlayerMobile Killer = mob as PlayerMobile;
			if ( mob.ShortTermMurders >= 5 && Killer.MurderBounty <= 0)
			{
				double loss = (100.0 - (19.0 + (mob.ShortTermMurders / 5.0))) / 100.0; // 20 to 30% loss

				if( loss < 0.75 )
					loss = 0.60;// changed to .80 from .95
				else if( loss > 0.95 )
					loss = 0.60; // changed to .80 from /95

				TimeSpan lossperiod = TimeSpan.FromMinutes( 240.0 );

				loss = 1.0 - loss;

				ClearSkillLoss( mob );

				SkillLossContext context = new SkillLossContext();
				m_SkillLoss[mob] = context;

				List<SkillMod> mods = context.m_SkillMods = new List<SkillMod>();

				for ( int i = 0; i < mob.Skills.Length; ++i )
				{
					Skill sk = mob.Skills[i];
					double baseValue = sk.Base;

					if ( baseValue > 0 )
					{
						SkillMod mod = new DefaultSkillMod( sk.SkillName, true, -(baseValue * loss) );

						mods.Add( mod );
						mob.AddSkillMod( mod );
					}
				}

				mob.AddStatMod( new StatMod( StatType.Str, "Murder Penalty Str", -(int)(mob.RawStr * loss), lossperiod ) );
				mob.AddStatMod( new StatMod( StatType.Dex, "Murder Penalty Dex", -(int)(mob.RawDex * loss), lossperiod ) );
				mob.AddStatMod( new StatMod( StatType.Int, "Murder Penalty Int", -(int)(mob.RawInt * loss), lossperiod ) );

				context.m_Timer = Timer.DelayCall<Mobile>( lossperiod, new TimerStateCallback<Mobile>( ClearSkillLoss_Callback ), mob );
			}
			else  ///Removed short term statloss. 
			if ( mob.ShortTermMurders < 5 && Killer.MurderBounty > 0 )
			{
				double loss = (100.0 - (19.0 + (Killer.MurderBounty / 10000.0))) / 100.0; // 20 to 30% loss
				if( loss < 0.75 )
					loss = 0.60;// changed to .80 from .95
				else if( loss > 0.95 )
					loss = 0.60; // changed to .80 from /95
				mob.SendMessage( "Current MurderBounty: " + Killer.MurderBounty);
				double timeloss = (double)Killer.MurderBounty/33.3;
				TimeSpan lossperiod = TimeSpan.FromMinutes( timeloss );
				
				int m = (int)Math.Round( timeloss );
				mob.SendMessage( "You have entered a bounty statloss for: " + m + " minutes.");

				loss = 1.0 - loss;

				ClearSkillLoss( mob );

				SkillLossContext context = new SkillLossContext();
				m_SkillLoss[mob] = context;

				List<SkillMod> mods = context.m_SkillMods = new List<SkillMod>();

				for ( int i = 0; i < mob.Skills.Length; ++i )
				{
					Skill sk = mob.Skills[i];
					double baseValue = sk.Base;

					if ( baseValue > 0 )
					{
						SkillMod mod = new DefaultSkillMod( sk.SkillName, true, -(baseValue * loss) );

						mods.Add( mod );
						mob.AddSkillMod( mod );
					}
				}

				mob.AddStatMod( new StatMod( StatType.Str, "Murder Penalty Str", -(int)(mob.RawStr * loss), lossperiod ) );
				mob.AddStatMod( new StatMod( StatType.Dex, "Murder Penalty Dex", -(int)(mob.RawDex * loss), lossperiod ) );
				mob.AddStatMod( new StatMod( StatType.Int, "Murder Penalty Int", -(int)(mob.RawInt * loss), lossperiod ) );

				context.m_Timer = Timer.DelayCall<Mobile>( lossperiod, new TimerStateCallback<Mobile>( ClearSkillLoss_Callback ), mob );
			}
			else if ( mob.ShortTermMurders >= 5 && Killer.MurderBounty > 0 )
			{
				double loss = (100.0 - (19.0 + (mob.ShortTermMurders / 5.0))) / 100.0; // 20 to 30% loss
				if( loss < 0.75 )
					loss = 0.60;// changed to .80 from .95
				else if( loss > 0.95 )
					loss = 0.60; // changed to .80 from /95
				mob.SendMessage( "Current MurderBounty: " + Killer.MurderBounty);
				double timeloss = (double)Killer.MurderBounty/33.3;
				TimeSpan lossperiod = TimeSpan.FromMinutes( timeloss + 240.0);
				
				Killer.MurderBounty = 0;
				
				int m = (int)Math.Round( timeloss + 240.0);
				mob.SendMessage( "You have entered statloss for: " + m + " minutes.");

				loss = 1.0 - loss;

				ClearSkillLoss( mob );

				SkillLossContext context = new SkillLossContext();
				m_SkillLoss[mob] = context;

				List<SkillMod> mods = context.m_SkillMods = new List<SkillMod>();

				for ( int i = 0; i < mob.Skills.Length; ++i )
				{
					Skill sk = mob.Skills[i];
					double baseValue = sk.Base;

					if ( baseValue > 0 )
					{
						SkillMod mod = new DefaultSkillMod( sk.SkillName, true, -(baseValue * loss) );

						mods.Add( mod );
						mob.AddSkillMod( mod );
					}
				}

				mob.AddStatMod( new StatMod( StatType.Str, "Murder Penalty Str", -(int)(mob.RawStr * loss), lossperiod ) );
				mob.AddStatMod( new StatMod( StatType.Dex, "Murder Penalty Dex", -(int)(mob.RawDex * loss), lossperiod ) );
				mob.AddStatMod( new StatMod( StatType.Int, "Murder Penalty Int", -(int)(mob.RawInt * loss), lossperiod ) );

				context.m_Timer = Timer.DelayCall<Mobile>( lossperiod, new TimerStateCallback<Mobile>( ClearSkillLoss_Callback ), mob );
			}
		}

		private static void ClearSkillLoss_Callback( Mobile mob )
		{
			ClearSkillLoss( mob );
		}

		public static bool ClearSkillLoss( Mobile mob )
		{
			SkillLossContext context;
			m_SkillLoss.TryGetValue( mob, out context );

			if ( context != null )
			{
				m_SkillLoss.Remove( mob );

				List<SkillMod> mods = context.m_SkillMods;

				for ( int i = 0; i < mods.Count; ++i )
					mob.RemoveSkillMod( mods[i] );

				context.m_Timer.Stop();

				return true;
			}

			mob.RemoveStatMod( "Murder Penalty Str" );
			mob.RemoveStatMod( "Murder Penalty Dex" );
			mob.RemoveStatMod( "Murder Penalty Int" );

			return false;
		}

		public static bool InSkillLoss( Mobile mob )
		{
			SkillLossContext context;
			m_SkillLoss.TryGetValue( mob, out context );

			return context != null;
		}
		#endregion
	}
}