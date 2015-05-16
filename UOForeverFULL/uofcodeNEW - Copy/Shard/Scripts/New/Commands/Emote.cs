using System;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Items;
using Server.Gumps;

namespace Server.Commands
{
	public class Emote
	{
		public static void Initialize()
		{
			CommandSystem.Register( "emote", AccessLevel.Player, new CommandEventHandler( Emote_OnCommand ) );
			CommandSystem.Register( "e", AccessLevel.Player, new CommandEventHandler( Emote_OnCommand ) );
		}

		public static void DoEmote( Mobile from, int sound )
		{
			if ( from.Squelched || !from.Alive )
				from.SendMessage( "You cannot do that in your current state." );
			else if ( from.BeginAction( typeof( Emote ) ) )
			{
				switch( sound )
				{
					case 1:
						from.PlaySound( from.Female ? 778 : 1049 );
						from.Emote( "*ah!*" );
						break;
					case 2:
						from.PlaySound( from.Female ? 779 : 1050 );
						from.Emote( "*ah ha!*" );
						break;
					case 3:
						from.PlaySound( from.Female ? 780 : 1051 );
						from.Emote( "*applauds*" );
						break;
					case 4:
						from.PlaySound( from.Female ? 781 : 1052 );
						from.Emote( "*blows nose*" );
						if ( !from.Mounted )
							from.Animate( 34, 5, 1, true, false, 0 );
						break;
					case 5:
						from.Emote( "*bows*" );
						if ( !from.Mounted )
							from.Animate( 32, 5, 1, true, false, 0 );
						break;
					case 6:
						from.PlaySound( from.Female ? 786 : 1057 );
						from.Emote( "*bs cough*" );
						break;
					case 7:
						from.PlaySound( from.Female ? 782 : 1053 );
						from.Emote( "*burp!*" );
						if ( !from.Mounted )
							from.Animate( 33, 5, 1, true, false, 0 );
						break;
					case 8:
						from.PlaySound( from.Female ? 0x312 : 0x420 );
						from.Emote( "*clears throat*" );
						if ( !from.Mounted )
							from.Animate( 33, 5, 1, true, false, 0 );
						break;
					case 9:
						from.PlaySound( from.Female ? 785 : 1056 );
						from.Emote( "*cough!*" );
						if ( !from.Mounted )
							from.Animate( 33, 5, 1, true, false, 0 );
						break;
					case 10:
						from.PlaySound( from.Female ? 787 : 1058 );
						from.Emote( "*cries*" );
						break;
					case 11:
						from.PlaySound( from.Female ? 791 : 1063 );
						from.Emote( "*faints*" );
						if ( !from.Mounted )
							from.Animate( 22, 5, 1, true, false, 0 );
						break;
					case 12:
						from.PlaySound( from.Female ? 792 : 1064 );
						from.Emote( "*farts*" );
						break;
					case 13:
						from.PlaySound( from.Female ? 793 : 1065 );
						from.Emote( "*gasp!*" );
						break;
					case 14:
						from.PlaySound( from.Female ? 794 : 1066 );
						from.Emote( "*giggles*" );
						break;
					case 15:
						from.PlaySound( from.Female ? 795 : 1067 );
						from.Emote( "*groans*" );
						break;
					case 16:
						from.PlaySound( from.Female ? 796 : 1068 );
						from.Emote( "*growls*" );
						break;
					case 17:
						from.PlaySound( from.Female ? 797 : 1069 );
						from.Emote( "*hey!*" );
						break;
					case 18:
						from.PlaySound( from.Female ? 798 : 1070 );
						from.Emote( "*hiccup!*" );
						break;
					case 19:
						from.PlaySound( from.Female ? 799 : 1071 );
						from.Emote( "*huh?*" );
						break;
					case 20:
						from.PlaySound( from.Female ? 800 : 1072 );
						from.Emote( "*kisses*" );
						break;
					case 21:
						from.PlaySound( from.Female ? 801 : 1073 );
						from.Emote( "*laughs*" );
						break;
					case 22:
						from.PlaySound( from.Female ? 802 : 1074 );
						from.Emote( "*no!*" );
						break;
					case 23:
						from.PlaySound( from.Female ? 803 : 1075 );
						from.Emote( "*oh!*" );
						break;
					case 24:
						from.PlaySound( from.Female ? 811 : 1085 );
						from.Emote( "*oooh*" );
						break;
					case 25:
						from.PlaySound( from.Female ? 812 : 1086 );
						from.Emote( "*oops*" );
						break;
					case 26:
					{
						from.PlaySound( from.Female ? 813 : 1087 );
						from.Emote( "*pukes*" );
							if ( !from.Mounted )
								from.Animate( 32, 5, 1, true, false, 0 );

							Point3D p = new Point3D( from.Location );
							switch ( from.Direction )
							{
								case Direction.North:
									p.Y--; break;
								case Direction.South:
									p.Y++; break;
								case Direction.East:
									p.X++; break;
								case Direction.West:
									p.X--; break;
								case Direction.Right:
									p.X++; p.Y--; break;
								case Direction.Down:
									p.X++; p.Y++; break;
								case Direction.Left:
									p.X--; p.Y++; break;
								case Direction.Up:
									p.X--; p.Y--; break;
							}

							p.Z = from.Z;

							//p.Z = from.Map.GetAverageZ( p.X, p.Y );

							new Puke().MoveToWorld( p, from.Map );
						break;
					}
					case 27:
						from.PlaySound( 315 );
						from.Emote( "*punches*" );
						if ( !from.Mounted )
							from.Animate( 31, 5, 1, true, false, 0 );
						break;
					case 28:
						from.PlaySound( from.Female ? 814 : 1088 );
						from.Emote( "*ahhhh!*" );
						break;
					case 29:
						from.PlaySound( from.Female ? 815 : 1089 );
						from.Emote( "*shhh!*" );
						break;
					case 30:
						from.PlaySound( from.Female ? 816 : 1090 );
						from.Emote( "*sigh*" );
						break;
					case 31:
						from.PlaySound( 948 );
						from.Emote( "*slaps*" );
						if ( !from.Mounted )
							from.Animate( 11, 5, 1, true, false, 0 );
						break;
					case 32:
						from.PlaySound( from.Female ? 817 : 1091 );
						from.Emote( "*ahh-choo!*" );
						if ( !from.Mounted )
							from.Animate( 32, 5, 1, true, false, 0 );
						break;
					case 33:
						from.PlaySound( from.Female ? 818 : 1092 );
						from.Emote( "*sniff*" );
						if( !from.Mounted )
							from.Animate( 34, 5, 1, true, false, 0 );
						break;
					case 34:
						from.PlaySound( from.Female ? 819 : 1093 );
						from.Emote( "*snore*" );
						break;
					case 35:
						from.PlaySound( from.Female ? 820 : 1094 );
						from.Emote( "*spits*" );
						if ( !from.Mounted )
							from.Animate( 6, 5, 1, true, false, 0 );
						break;
					case 36:
						from.PlaySound( 792 );
						from.Emote( "*sticks out tongue*" );
						break;
					case 37:
						from.PlaySound( 874 );
						from.Emote( "*taps foot*" );
						if ( !from.Mounted )
							from.Animate( 38, 5, 1, true, false, 0 );
						break;
					case 38:
						from.PlaySound( from.Female ? 821 : 1095 );
						from.Emote( "*whistles*" );
						if ( !from.Mounted )
							from.Animate( 5, 5, 1, true, false, 0 );
						break;
					case 39:
						from.PlaySound( from.Female ? 783 : 1054 );
						from.Emote( "*woohoo!*" );
						break;
					case 40:
						from.PlaySound( from.Female ? 822 : 1096 );
						from.Emote( "*yawns*" );
						if ( !from.Mounted )
							from.Animate( 17, 5, 1, true, false, 0 );
						break;
					case 41:
						from.PlaySound( from.Female ? 823 : 1097 );
						from.Emote( "*yea!*" );
						break;
					case 42:
						from.PlaySound( from.Female ? 823 : 1098 );
						from.Emote( "*yells*" );
						break;
				}

				Timer.DelayCall<Mobile>( TimeSpan.FromSeconds( 5.0 ), new TimerStateCallback<Mobile>( ReleaseEmote_Callback ), from );
			}
		}

		public static void ReleaseEmote_Callback( Mobile from )
		{
			from.EndAction( typeof( Emote ) );
		}

		[Usage( "<sound>" )]
		[Description( "Emote with sounds, words, and possibly an animation with one command!")]
		public static void Emote_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			string param = e.ArgString.Trim();

			int sound = 0;

			switch( param )
			{
				case "ah":
					sound = 1;
					break;
				case "ahha":
					sound = 2;
					break;
				case "applaud":
					sound = 3;
					break;
				case "blownose":
					sound = 4;
					break;
				case "bow":
					sound = 5;
					break;
				case "bscough":
					sound = 6;
					break;
				case "burp":
					sound = 7;
					break;
				case "clearthroat":
					sound = 8;
					break;
				case "cough":
					sound = 9;
					break;
				case "cry":
					sound = 10;
					break;
				case "faint":
					sound = 11;
					break;
				case "fart":
					sound = 12;
					break;
				case "gasp":
					sound = 13;
					break;
				case "giggle":
					sound = 14;
					break;
				case "groan":
					sound = 15;
					break;
				case "growl":
					sound = 16;
					break;
				case "hey":
					sound = 17;
					break;
				case "hiccup":
					sound = 18;
					break;
				case "huh":
					sound = 19;
					break;
				case "kiss":
					sound = 20;
					break;
				case "laugh":
					sound = 21;
					break;
				case "no":
					sound = 22;
					break;
				case "oh":
					sound = 23;
					break;
				case "oooh":
					sound = 24;
					break;
				case "oops":
					sound = 25;
					break;
				case "puke":
					sound = 26;
					break;
				case "punch":
					sound = 27;
					break;
				case "scream":
					sound = 28;
					break;
				case "shush":
					sound = 29;
					break;
				case "sigh":
					sound = 30;
					break;
				case "slap":
					sound = 31;
					break;
				case "sneeze":
					sound = 32;
					break;
				case "sniff":
					sound = 33;
					break;
				case "snore":
					sound = 34;
					break;
				case "spit":
					sound = 35;
					break;
				case "tongue":
					sound = 36;
					break;
				case "tapfoot":
					sound = 37;
					break;
				case "whistle":
					sound = 38;
					break;
				case "woohoo":
					sound = 39;
					break;
				case "yawn":
					sound = 40;
					break;
				case "yea":
					sound = 41;
					break;
				case "yell":
					sound = 42;
					break;
			}

			if ( sound > 0 )
				DoEmote( from, sound );
			else
				from.SendGump( new EmoteGump( from ) );
		}
	}

	public class EmoteGump : Gump
	{
		private const int Label32 = 0xFFFFFF;
		private const int Selected32 = 0x8080FF;

		private Mobile m_From;
		private int m_Page;

		public void AddPageButton( int x, int y, int buttonID, string text, int page )
		{
			bool selection = m_Page == page;

			AddButton( x, y - 1, selection ? 4006 : 4005, 4007, buttonID, GumpButtonType.Reply, 0 );
			AddHtml( x + 35, y, 200, 20, Color( text, selection ? Selected32 : Label32 ), false, false );
		}

		public void AddButtonLabeled( int x, int y, int buttonID, string text )
		{
			AddButton( x, y - 1, 4005, 4007, buttonID, GumpButtonType.Reply, 0 );
			AddHtml( x + 35, y, 240, 20, Color( text, Label32 ), false, false );
		}

		public int GetButtonID( int type, int index )
		{
			return 1 + (index * 15) + type;
		}

		public string Color( string text, int color )
		{
			return String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text );
		}

		public EmoteGump( Mobile from ) : this( from, 1 )
		{
		}

		public EmoteGump( Mobile from, int page ) : base ( 600, 50 )
		{
			from.CloseGump( typeof( EmoteGump ) );

			m_From = from;
			m_Page = page;

			Closable = true;
			Dragable = true;

			AddPage( 0 );
			AddBackground( 0, 65, 130, 360, 5054 );
			AddAlphaRegion( 10, 70, 110, 350 );
			AddImageTiled( 10, 70, 110, 20, 9354 );
			AddLabel( 13, 70, 200, "Emote List" );
			AddImage( 100, 0, 10410 );
			AddImage( 100, 305, 10412 );
			AddImage( 100, 150, 10411 );

			switch ( page )
			{
				default:
				case 1:
				{
					AddButtonLabeled( 10, 90, GetButtonID( 1, 1 ), "Ah" );
					AddButtonLabeled( 10, 115, GetButtonID( 1, 2 ), "Ah-ha" );
					AddButtonLabeled( 10, 140, GetButtonID( 1, 3 ), "Applaud" );
					AddButtonLabeled( 10, 165, GetButtonID( 1, 4 ), "Blow Nose" );
					AddButtonLabeled( 10, 190, GetButtonID( 1, 5 ), "Bows" );
					AddButtonLabeled( 10, 215, GetButtonID( 1, 6 ), "BS Cough" );
					AddButtonLabeled( 10, 240, GetButtonID( 1, 7 ), "Burp" );
					AddButtonLabeled( 10, 265, GetButtonID( 1, 8 ), "Clear Throat" );
					AddButtonLabeled( 10, 290, GetButtonID( 1, 9 ), "Cough" );
					AddButtonLabeled( 10, 315, GetButtonID( 1, 10 ), "Cry" );
					AddButtonLabeled( 10, 340, GetButtonID( 1, 11 ), "Faint" );
					AddButtonLabeled( 10, 365, GetButtonID( 1, 12 ), "Fart" );
					AddButton( 70, 380, 4502, 0504, GetButtonID( 0,2 ), GumpButtonType.Reply, 0 );
					break;
				}
				case 2:
				{
					AddButtonLabeled( 10, 90, GetButtonID( 1, 13 ), "Gasp" );
					AddButtonLabeled( 10, 115, GetButtonID( 1, 14 ), "Giggle" );
					AddButtonLabeled( 10, 140, GetButtonID( 1, 15 ), "Groan" );
					AddButtonLabeled( 10, 165, GetButtonID( 1, 16 ), "Growl" );
					AddButtonLabeled( 10, 190, GetButtonID( 1, 17 ), "Hey" );
					AddButtonLabeled( 10, 215, GetButtonID( 1, 18 ), "Hiccup" );
					AddButtonLabeled( 10, 240, GetButtonID( 1, 19 ), "Huh" );
					AddButtonLabeled( 10, 265, GetButtonID( 1, 20 ), "Kiss" );
					AddButtonLabeled( 10, 290, GetButtonID( 1, 21 ), "Laugh" );
					AddButtonLabeled( 10, 315, GetButtonID( 1, 22 ), "No" );
					AddButtonLabeled( 10, 340, GetButtonID( 1, 23 ), "Oh" );
					AddButtonLabeled( 10, 365, GetButtonID( 1, 24 ), "Oooh" );
					AddButton( 10, 380, 4506, 4508, GetButtonID( 0,1 ), GumpButtonType.Reply, 0 );
					AddButton( 70, 380, 4502, 0504, GetButtonID( 0,3 ), GumpButtonType.Reply, 0 );
					break;
				}
				case 3:
				{
					AddButtonLabeled( 10, 90, GetButtonID( 1, 25 ), "Oops" );
					AddButtonLabeled( 10, 115, GetButtonID( 1, 26 ), "Puke" );
					AddButtonLabeled( 10, 140, GetButtonID( 1, 27 ), "Punch" );
					AddButtonLabeled( 10, 165, GetButtonID( 1, 28 ), "Scream" );
					AddButtonLabeled( 10, 190, GetButtonID( 1, 29 ), "Shush" );
					AddButtonLabeled( 10, 215, GetButtonID( 1, 30 ), "Sigh" );
					AddButtonLabeled( 10, 240, GetButtonID( 1, 31 ), "Slap" );
					AddButtonLabeled( 10, 265, GetButtonID( 1, 32 ), "Sneeze" );
					AddButtonLabeled( 10, 290, GetButtonID( 1, 33 ), "Sniff" );
					AddButtonLabeled( 10, 315, GetButtonID( 1, 34 ), "Snore" );
					AddButtonLabeled( 10, 340, GetButtonID( 1, 35 ), "Spit" );
					AddButtonLabeled( 10, 365, GetButtonID( 1, 36 ), "Stick Out Tongue" );
					AddButton( 10, 380, 4506, 4508, GetButtonID( 0,2 ), GumpButtonType.Reply, 0 );
					AddButton( 70, 380, 4502, 0504, GetButtonID( 0,4 ), GumpButtonType.Reply, 0 );
					break;
				}
				case 4:
				{
					AddButtonLabeled( 10, 90, GetButtonID( 1, 37 ), "Tap Foot" );
					AddButtonLabeled( 10, 115, GetButtonID( 1, 38 ), "Whistle" );
					AddButtonLabeled( 10, 140, GetButtonID( 1, 39 ), "Woohoo" );
					AddButtonLabeled( 10, 165, GetButtonID( 1, 40 ), "Yawn" );
					AddButtonLabeled( 10, 190, GetButtonID( 1, 41 ), "Yea" );
					AddButtonLabeled( 10, 215, GetButtonID( 1, 42 ), "Yell" );
					AddButton( 10, 380, 4506, 4508, GetButtonID( 0,3 ), GumpButtonType.Reply, 0 );
					break;
				}
			}
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			int val = info.ButtonID - 1;

			if ( val < 0 )
				return;

			Mobile from = m_From;

			int type = val % 15;
			int index = val / 15;

			switch ( type )
			{
				case 0:
				{
					if ( index < 5 && index > 0 )
						from.SendGump( new EmoteGump( from, index ) );
					break;
				}
				case 1:
				{
					if ( index > 0 && index < 43 )
					{
						Emote.DoEmote( from, index );
						from.SendGump( new EmoteGump( from, ((index-1) / 12) + 1 ) );
					}
					break;
				}
			}
		}
	}

	public class Puke : Item
	{
		[Constructable]
		public Puke() : base( Utility.Random( 0x122B,4))//0xF3B, 2 ) )
		{
			Name = "puke";
			Hue = 0x557;
			Movable = false;

			new InternalTimer( this, TimeSpan.FromSeconds( 4.0 ) ).Start();
		}

		public Puke( Serial serial ) : base( serial )
		{
			new InternalTimer( this, TimeSpan.FromSeconds( 4.0 ) ).Start();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}

		private class InternalTimer : Timer
		{
			private Item m_Puke;

			public InternalTimer( Item puke, TimeSpan duration ) : base( duration )
			{
				Priority = TimerPriority.OneSecond;

				m_Puke = puke;
			}

			protected override void OnTick()
			{
				m_Puke.Delete();
			}
		}
	}
}