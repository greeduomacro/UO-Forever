using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Commands
{
	public class StaffDress
	{
		public static void Initialize()
		{
			CommandSystem.Register( "StaffDress", AccessLevel.Counselor, new CommandEventHandler( StaffDress_OnCommand ) );
		}

		private static Type[] m_StaffRobeTypes = new Type[]
			{
				null,
				typeof( TrialCounselorRobe ),
				typeof( CounselorRobe ),
				typeof( LeadCounselorRobe ),
				typeof( TrialSeerRobe ),
				typeof( SeerRobe ),
				typeof( LeadSeerRobe ),
				typeof( TrialGMRobe ),
				typeof( GMRobe ),
				typeof( LeadGMRobe ),
				typeof( TrialAdminRobe ),
				typeof( AdminRobe ),
				typeof( LeadAdminRobe ),
				typeof( TrialDevRobe ),
				typeof( DevRobe ),
				typeof( LeadDevRobe ),
				typeof( OwnerRobe ),
				typeof( QMRobe ),
				typeof( EmissaryRobe )
			};

		private static int[] m_StaffRobeHues = new int[]
			{
				0,
				3,
				1156,
				1365,
				467,
				1267,
				2003,
				37,
				38,
				1157,
				1072,
				1150,
				1153,
				2046,
				1175,
				1281,
				1154,
				1151,
				2575
			};

		[Usage( "StaffDress" )]
		[Description( "Dresses the staff member appropriately.  Should always be used in conjunction with going visible." )]
		public static void StaffDress_OnCommand( CommandEventArgs e )
		{
			try
			{
				PlayerMobile pm = e.Mobile as PlayerMobile;
				if ( pm != null )
				{
					bool foundsuit = false;
					int rank = (int)pm.StaffRank;

					if ( pm.AccessLevel < AccessLevel.Administrator )
					{
						int hue = pm.Hue % 16384;

						if ( hue < 1002 || hue > 1058 )
							pm.Hue = Utility.RandomMinMax( 1002, 1058 ) + 32767;
					}

					pm.Karma = pm.Fame = pm.Kills = pm.ShortTermMurders = pm.BodyMod = 0;
					pm.BodyValue = 987;
					pm.SolidHueOverride = pm.HueMod = -1;
					pm.Blessed = true;
					pm.DisplayChampionTitle = false;

					if ( pm.Mount != null )
						pm.Mount.Rider = null;

					pm.Send( SpeedControl.MountSpeed );

					for ( int i = pm.Items.Count - 1; i >= 0; i-- )
					{
						Item item = pm.Items[i];
						if ( item.GetType() == m_StaffRobeTypes[rank] )
						{
							if ( pm.AccessLevel >= AccessLevel.Administrator || ( item.Hue == m_StaffRobeHues[rank] && item.Name == null ) )
								foundsuit = true;
							else
								item.Delete();
						}
						else
						{
							Layer layer = item.Layer;
							if ( layer != Layer.Backpack && layer != Layer.Bank && layer != Layer.FacialHair
								&& layer != Layer.Hair && layer != Layer.Mount && layer != Layer.ShopBuy && layer != Layer.ShopResale
								&& layer != Layer.ShopSell )
								pm.AddToBackpack( item );
						}
					}

					if ( !foundsuit )
					{
							Item item = Activator.CreateInstance( m_StaffRobeTypes[rank] ) as Item;
							if ( item != null )
								pm.AddItem( item );
					}
				}	
			}
			catch ( Exception exp )
			{
				Console.WriteLine( exp.ToString() );
			}
		}
	}
}