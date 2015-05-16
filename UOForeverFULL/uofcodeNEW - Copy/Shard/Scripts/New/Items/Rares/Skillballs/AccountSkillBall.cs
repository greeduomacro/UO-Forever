using System;
using System.Collections;
using Server.Network;
using Server.Mobiles;
using Server.Accounting;

namespace Server.Items
{
	public class AccountSkillBall : SkillBall
	{
		private IAccount m_Account;
		
		//[CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
		public IAccount Account{ get{ return m_Account; } set{ m_Account = value; } }

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
		public string AccountName{ get{ return m_Account.Username; } set{ m_Account = Accounts.GetAccount( value ); } } 

		[Constructable]
		public AccountSkillBall( string account ) : this( 25, account )
		{
		}

		[Constructable]
		public AccountSkillBall( int bonus, string account ) : this( bonus, account, -1 )
		{
		}

		[Constructable]
		public AccountSkillBall( int bonus, string account, int days ) : base( bonus, false, days )
		{
			m_Account = Accounts.GetAccount( account );
		}

		public AccountSkillBall( Serial serial ) : base( serial )
		{
		}

		public override void UpdateHue()
		{
			Hue = 215;
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			if ( Deleted || !from.CanSee( this ) )
				return;

			//if ( m_Account == null || from.Account != m_Account )
			LabelTo( from, 38, 1149839 ); // * Non-Transferable Account Bound Item *
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1149839 );
		}

		public override bool Nontransferable{ get { return false; } }

		public override bool CheckItemUse( Mobile from, Item item )
		{
			if ( base.CheckItemUse( from, item ) && from.Account == m_Account )
				return true;

			from.SendLocalizedMessage( 1149841 ); // This is an account-bound item only usable by a character on the account of the original purchaser. This item is not bound to your account.

			//from.SendMessage( "A magical force prevents you from using this skill ball..." );
			//from.SendMessage( "You decide that this item is intended for someone else." );

			return false;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version

			string un = String.Empty;
			if ( m_Account != null )
				un = m_Account.Username;

			writer.Write( un );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_Account = Accounts.GetAccount( reader.ReadString() );
		}
	}
}