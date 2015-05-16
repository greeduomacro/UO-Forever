using System;
using Server;
using Server.Network;
using Server.Engines.Craft;
using Server.Gumps;
using Server.Engines.Apiculture;
using Server.Targeting;

namespace Server.Items
{
	public class SmallWaxPot : Item
	{
		public static readonly int MaxWax = 255; //the maximuum amount the pot can hold

		private int m_UsesRemaining;
		private int m_RawBeeswax;
		private int m_PureBeeswax;

		[CommandProperty( AccessLevel.GameMaster )]
		public int UsesRemaining
		{
			get { return m_UsesRemaining; }
			set { m_UsesRemaining = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int RawBeeswax
		{
			get { return m_RawBeeswax; }
			set { if(value<0)value=0;if(value>MaxWax)value=MaxWax;m_RawBeeswax = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int PureBeeswax
		{
			get { return m_PureBeeswax; }
			set { if(value<0)value=0;if(value>MaxWax)value=MaxWax;m_PureBeeswax = value; InvalidateProperties(); }
		}

		public override string DefaultName{ get{ return "a small wax pot"; } }

		[Constructable]
		public SmallWaxPot() : this( 50 )
		{
		}

		[Constructable]
		public SmallWaxPot( int uses ) : base( 2532 )
		{
			m_UsesRemaining = uses;
			Weight = 3.0;
			m_RawBeeswax = 0;
			m_PureBeeswax = 0;
		}

		public SmallWaxPot( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~

			if( PureBeeswax < 1 && RawBeeswax < 1 )
				list.Add( 1049644 , "Empty" );
			else if( PureBeeswax > 0 )
			{
				list.Add( 1060663,"{0}\t{1}" ,"Wax", PureBeeswax.ToString() );
				list.Add( 1049644 , "Rendered" );
			}
			else
			{
				list.Add( 1060663,"{0}\t{1}" ,"Wax", RawBeeswax.ToString() );
				list.Add( 1049644 , "Raw" );
			}
		}

		public virtual void DisplayDurabilityTo( Mobile m )
		{
			LabelToAffix( m, 1017323, AffixType.Append, ": " + m_UsesRemaining.ToString() ); // Durability
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			DisplayDurabilityTo( from );
		}

		public override void OnDoubleClick(Mobile from)
		{
			from.SendGump( new BeehiveSmallPotGump( from, this ) );
		}

		public void BeginAdd( Mobile from )
		{
			if ( m_RawBeeswax < MaxWax )
				from.Target = new AddWaxTarget( this );
			else
				from.PrivateOverheadMessage( 0, 1154, false,  "The pot cannot hold any more raw beeswax.", from.NetState );
		}

		public void EndAdd( Mobile from, object o )
		{
			if ( o is Item && ((Item)o).IsChildOf( from.Backpack ) )
			{
				if( o is RawBeeswax )
				{
					RawBeeswax wax = (RawBeeswax)o;

					if( (wax.Amount + RawBeeswax) > MaxWax )
					{
						wax.Amount -= (MaxWax - RawBeeswax);
						RawBeeswax = MaxWax;
					}
					else
					{
						RawBeeswax += wax.Amount;
						wax.Delete();
					}

					from.PrivateOverheadMessage( 0, 1154, false,  "You put raw beeswax in the pot.", from.NetState );

					if( from.HasGump( typeof(BeehiveSmallPotGump)) )
						from.CloseGump( typeof(BeehiveSmallPotGump) );

					from.SendGump( new BeehiveSmallPotGump( from, this ) ); //resend the gump

					if ( m_RawBeeswax < MaxWax )
						BeginAdd( from );
				}
				else
					from.PrivateOverheadMessage( 0, 1154, false,  "You can only put raw beeswax in the pot.", from.NetState );
			}
			else
			{
				from.PrivateOverheadMessage( 0, 1154, false,  "The wax must be in your pack to target it.", from.NetState );
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
			writer.Write( (int) m_UsesRemaining );
			writer.Write( (int) m_RawBeeswax );
			writer.Write( (int) m_PureBeeswax );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			switch ( version )
			{
				case 0:
				{
					m_UsesRemaining = reader.ReadInt();
					m_RawBeeswax = reader.ReadInt();
					m_PureBeeswax = reader.ReadInt();
					break;
				}
			}
		}
	}

	public class AddWaxTarget : Target
	{
		private SmallWaxPot m_Pot;

		public AddWaxTarget( SmallWaxPot pot ) : base( 18, false, TargetFlags.None )
		{
			m_Pot = pot;
		}

		protected override void OnTarget( Mobile from, object targeted )
		{
			if ( m_Pot.Deleted || !m_Pot.IsChildOf( from.Backpack ) )
				return;

			m_Pot.EndAdd( from, targeted );
		}
	}

	public class BeehiveSmallPotGump : Gump
	{
		SmallWaxPot m_Pot = null;

		public static bool GiveSlumgum = true;  //does rendering produce slumgum? (impurities in wax)

		public BeehiveSmallPotGump( Mobile from, SmallWaxPot pot ): base( 20, 20 )
		{
			m_Pot = pot;

			Closable=true;
			Disposable=true;
			Dragable=true;
			Resizable=false;

			AddPage(0);

			AddBackground(15, 12, 352, 140, 9270);
			AddAlphaRegion(30, 27, 321, 109);
			AddImage(326, 110, 210);

			//vines
			AddItem(10, 5, 3311);
			AddItem(11, 49, 3311);
			AddItem(328, 50, 3307);
			AddItem(327, 3, 3307);

			//pot image
			if( m_Pot.PureBeeswax > 0 )
				AddItem(231, 105, 0x142B);
			else
				AddItem(231, 105, 2532);

			//labels
			AddLabel(76 , 71 , 1153, "Render Beeswax");
			AddLabel(76 , 40 , 1153, "Add Raw Beeswax");
			AddLabel(76 , 101, 1153, "Empty Pot");
			AddLabel(331, 110, 1153, "?");

			//buttons
			AddButton(42, 39, 4005, 4006, (int)Buttons.cmdAddRaw, GumpButtonType.Reply, 0);
			AddButton(42, 70, 4005, 4006, (int)Buttons.cmdRenderWax, GumpButtonType.Reply, 0);
			AddButton(42, 102, 4005, 4006, (int)Buttons.cmdEmptyPot, GumpButtonType.Reply, 0);
			AddButton(326, 110, 212, 212, (int)Buttons.cmdHelp, GumpButtonType.Reply, 0);

			//wax amounts
			AddLabel(207, 40, 1153, "Raw Beeswax: " + m_Pot.RawBeeswax );
			AddLabel(207, 71, 1153, "Pure Beeswax: " + m_Pot.PureBeeswax );
		}

		public enum Buttons
		{
			cmdAddRaw = 1,
			cmdRenderWax,
			cmdEmptyPot,
			cmdHelp
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			if ( info.ButtonID == 0 || m_Pot.Deleted || !from.InRange( m_Pot.GetWorldLocation(), 3 ) )
				return;

			if( !m_Pot.IsAccessibleTo( from ) )
			{
				from.PrivateOverheadMessage( 0, 1154, false, "I cannot use that.", from.NetState );
				return;
			}

			switch ( info.ButtonID )
			{
				case (int)Buttons.cmdHelp:
				{
					from.SendGump( new BeehiveSmallPotGump(from,m_Pot) );
					from.SendGump( new BeehiveHelpGump(from, 1) );
					break;
				}
				case (int)Buttons.cmdAddRaw: //Add Raw Honey
				{
					from.SendGump( new BeehiveSmallPotGump(from, m_Pot) );

					if ( m_Pot.PureBeeswax > 0 )
					{
						from.PrivateOverheadMessage( 0, 1154, false,  "You cannot mix raw beeswax with rendered wax.  Please empty the pot first.", from.NetState );
						return;
					}

					from.PrivateOverheadMessage( 0, 1154, false,  "Choose the raw beeswax you wish to add to the pot.", from.NetState );
					m_Pot.BeginAdd( from );

					break;
				}
				case (int)Buttons.cmdEmptyPot: //Empty the pot
				{
					if( m_Pot.PureBeeswax < 1 && m_Pot.RawBeeswax < 1 )
					{
						from.PrivateOverheadMessage( 0, 1154, false, "There is no wax in the pot.", from.NetState );
						from.SendGump( new BeehiveSmallPotGump(from,m_Pot) );
						return;
					}

					Item wax;

					if( m_Pot.PureBeeswax > 0 )
					{
						wax = new Beeswax(m_Pot.PureBeeswax);
					}
					else
					{
						wax = new RawBeeswax(m_Pot.RawBeeswax);
					}

					if ( !from.PlaceInBackpack( wax ) )
					{
						wax.Delete();
						from.PrivateOverheadMessage( 0, 1154, false,  "There is not enough room in your backpack for the wax!", from.NetState );
						from.SendGump( new BeehiveSmallPotGump( from, m_Pot ) );
						break;
					}

					m_Pot.RawBeeswax = 0;
					m_Pot.PureBeeswax = 0;

					m_Pot.ItemID = 2532; //empty pot

					from.SendGump( new BeehiveSmallPotGump(from, m_Pot) );
					from.PrivateOverheadMessage( 0, 1154, false,  "You place the beeswax in your pack.", from.NetState );

					break;
				}
				case (int)Buttons.cmdRenderWax: //render the wax
				{
					if( m_Pot.UsesRemaining < 1 )
					{//no uses remaining
						from.PrivateOverheadMessage( 0, 1154, false,  "The pot is too damamged to render beeswax.", from.NetState );
						from.SendGump( new BeehiveSmallPotGump(from, m_Pot) );
						return;
					}
					else if( m_Pot.PureBeeswax > 1 )
					{//already rendered
						from.PrivateOverheadMessage( 0, 1154, false,  "The pot is already full of rendered beeswax.", from.NetState );
						from.SendGump( new BeehiveSmallPotGump(from, m_Pot) );
						return;
					}
					else if( m_Pot.RawBeeswax < 10 )
					{//not enough raw beeswax
						from.PrivateOverheadMessage( 0, 1154, false,  "There is not enough raw beeswax in the pot.", from.NetState );
						from.SendGump( new BeehiveSmallPotGump(from, m_Pot) );
						return;
					}
					else if( !BeehiveHelper.HasHeatSource( from ) )
					{//need a heat source to melt the wax
						from.PrivateOverheadMessage( 0, 1154, false,  "You must be near a heat source to render beeswax.", from.NetState );
						from.SendGump( new BeehiveSmallPotGump(from, m_Pot) );
						return;
					}

					m_Pot.ItemID = 0x142b; //pot overflowing with wax

					m_Pot.UsesRemaining--;
					if( m_Pot.UsesRemaining < 0 )
						m_Pot.UsesRemaining = 0;

					int waste = Utility.RandomMinMax( 1, m_Pot.RawBeeswax / 5 );

					if( GiveSlumgum )
					{//give slumgum
						Item gum = new Slumgum( Math.Max( 1, waste ) );

						if ( !from.PlaceInBackpack( gum ) )
							gum.Delete();
					}

					from.PlaySound( 0x21 );
					from.PrivateOverheadMessage( 0, 1154, false,  "You slowly melt the raw beeswax and remove the impurities.", from.NetState );

					m_Pot.PureBeeswax = m_Pot.RawBeeswax - waste;
					m_Pot.RawBeeswax = 0;

					break;
				}
			}
		}
	}
}