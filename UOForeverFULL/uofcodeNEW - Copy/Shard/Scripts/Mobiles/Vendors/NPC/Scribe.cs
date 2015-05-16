using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Network;
using Server.Factions;

namespace Server.Mobiles
{
	public class Scribe : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
	 protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

	public override NpcGuild NpcGuild{ get{ return NpcGuild.MagesGuild; } }

		[Constructable]
		public Scribe() : base( "the scribe" )
		{
			SetSkill( SkillName.EvalInt, 60.0, 83.0 );
			SetSkill( SkillName.Inscribe, 60.0, 100.0 );
		}

		//public override bool HandlesOnSpeech( Mobile from )
		//{
		//	if ( from.Alive && from.InRange( this, 3 ) )
		//		return true;

		//	return base.HandlesOnSpeech( from );
		//}

		//private DateTime m_NextCheckPack;

		//public override void OnMovement( Mobile m, Point3D oldLocation )
		//{
		//	if ( DateTime.UtcNow > m_NextCheckPack && InRange( m, 4 ) && !InRange( oldLocation, 4 ) && m.Player )
		//	{
		//		Container pack = m.Backpack;

		//		if ( pack != null )
		//		{
		//			m_NextCheckPack = DateTime.UtcNow + TimeSpan.FromSeconds( 2.0 );

//					Item spellbook = m.FindItemOnLayer( Layer.FirstValid ) as Spellbook;
//					if ( spellbook == null )
//						spellbook = pack.FindItemByType( typeof( Spellbook ), false );

//					if ( spellbook != null )
//					{
//						PrivateOverheadMessage( MessageType.Regular, 0x3B2, false, "If thou hath a spellbook, I can fill it for you.", m.NetState );
//						PrivateOverheadMessage( MessageType.Regular, 0x3B2, false, "Simply hand the spellbook to me to fill it.", m.NetState );
//					}
//				}
//			}

//			base.OnMovement( m, oldLocation );
//		}

//		public override void OnSpeech( SpeechEventArgs e )
//		{
//			Mobile m = e.Mobile;
//			Faction fact = Faction.Find( m );

//			if ( !e.Handled && m.Alive && e.Speech.ToLower().IndexOf( "fill" ) > -1 ) //*fill*
//			{
//				if ( FactionAllegiance != null && fact != null && FactionAllegiance != fact )
//					Say( true, "I will not do business with the enemy!" );
//				else
//				{
//					PublicOverheadMessage( MessageType.Regular, 0x3B2, false, "Which spellbook would you like to fill?" );
//					m.BeginTarget( 12, false, TargetFlags.None, new TargetCallback( Fill_OnTarget ) );
//					e.Handled = true;
//				}
//			}

//			base.OnSpeech( e );
//		}

//		public void Fill_OnTarget( Mobile from, object obj )
//		{
//			if ( obj is Spellbook )
//			{
//				Spellbook book = obj as Spellbook;
//				if ( book.Content == ulong.MaxValue ) //Its already filled
//					Say( true, "Thy book requires no spells." );
//				else
//				{
//					int totalprice = 0;
//
//					Type[] types = Loot.RegularScrollTypes;
//
//					for ( int i = 0;i < types.Length; ++i )
//					{
//						if ( !book.HasSpell( i ) )
//						{
//							int price = 0;
//							if ( (i/8) > 4 )
//								price = 42 + ((i/8)*8);
//							else
//								price = 10 + ((i/8)*4);

//							totalprice += price;
//						}
//					}

//					if ( from.Backpack.ConsumeTotal( typeof( Gold ), totalprice ) )
//					{
//						book.Content = ulong.MaxValue;
//						Say( true, String.Format( "I hath filled thy spell book for {0} gold pieces.", totalprice ) );
//					}
//					else
//						Say( true, String.Format( "Thou needs {0} gold pieces to fill a spell book.", totalprice ) );
//				}
//			}
//			else
//				Say( true, "A joke from thee? Doth thou even have a spellbook?" );
//		}

//		public override bool OnDragDrop( Mobile from, Item dropped )
//		{
//			Faction fact = Faction.Find( from );
//			if ( FactionAllegiance != null && fact != null && FactionAllegiance != fact )
//				Say( true, "I will not do business with the enemy!" );
//			else if ( dropped is Spellbook )
//			{
//				Spellbook book = dropped as Spellbook;
//				if ( book.Content == ulong.MaxValue ) //Its already filled
//					Say( true, "Thy book requires no spells." );
//				else
//				{
//					int totalprice = 0;

//					Type[] types = Loot.RegularScrollTypes;

//					for ( int i = 0;i < types.Length; ++i )
//					{
//						if ( !book.HasSpell( i ) )
//						{
//							int price = 0;
//							if ( (i/8) > 4 )
//								price = 42 + ((i/8)*8);
//							else
//								price = 10 + ((i/8)*4);
//
//							totalprice += price;
//						}
//					}
//
//					if ( Banker.WithdrawPackAndBank( from, totalprice ) )
//					{
//						book.Content = ulong.MaxValue;
//						Say( true, String.Format( "I hath filled thy spell book for {0} gold pieces.", totalprice ) );
						//return true;
//					}
//					else
//						Say( true, String.Format( "Thou needs {0} gold pieces to fill a spell book.", totalprice ) );
//				}
//
//				return false;
//			}
			//else
			//	Say( true, "A joke from thee? Doth thou even have a spellbook?" );

//			return base.OnDragDrop( from, dropped );
//		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBScribe() );
		}

		public override VendorShoeType ShoeType
		{
			get{ return Utility.RandomBool() ? VendorShoeType.Shoes : VendorShoeType.Sandals; }
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( new Server.Items.Robe( Utility.RandomNeutralHue() ) );
		}

		public Scribe( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}