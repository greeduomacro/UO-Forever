using System;
using Server;
using Server.Mobiles;
using Server.Commands;

namespace Server.Items
{
	public abstract class BaseSuit : Item
	{
		private AccessLevel m_AccessLevel;

		[CommandProperty( AccessLevel.Administrator )]
		public AccessLevel AccessLevel{ get{ return m_AccessLevel; } set{ m_AccessLevel = value; } }

		public override bool DisplayLootType{ get{ return false; } }

		public BaseSuit( AccessLevel level, int hue, int itemID ) : base( itemID )
		{
			Hue = hue;
			Weight = 1.0;
			Movable = false;
			LootType = LootType.Blessed;
			Layer = Layer.OuterTorso;

			m_AccessLevel = level;
		}

		public BaseSuit( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (int) m_AccessLevel );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_AccessLevel = (AccessLevel)reader.ReadInt();
					break;
				}
			}
		}

		public bool Validate()
		{
			object root = RootParent;

			Mobile from = root as Mobile;

			if ( from != null )
			{
				PlayerMobile pm = from as PlayerMobile;
				AccessLevel level = from.AccessLevel;

				if ( pm != null )
				{
					AccessLevelMod mod = AccessLevelToggler.GetMod( pm );
					if ( mod != null )
						level = mod.Level;
				}

				if ( level < m_AccessLevel )
				{
					Delete();
					return false;
				}
			}

			return true;
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( Validate() )
				base.OnSingleClick( from );
			else
				LabelToExpansion(from);
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( Validate() )
				base.OnDoubleClick( from );
		}

		public override bool VerifyMove( Mobile from )
		{
			PlayerMobile pm = from as PlayerMobile;
			AccessLevel level = from.AccessLevel;

			if ( pm != null )
			{
				AccessLevelMod mod = AccessLevelToggler.GetMod( pm );
				if ( mod != null )
					level = mod.Level;
			}

			return ( level >= m_AccessLevel );
		}

		public override bool OnEquip( Mobile from )
		{

			PlayerMobile pm = from as PlayerMobile;
			AccessLevel level = from.AccessLevel;

			if ( pm != null )
			{
				AccessLevelMod mod = AccessLevelToggler.GetMod( pm );
				if ( mod != null )
					level = mod.Level;
			}

			if ( level < m_AccessLevel )
			{
				from.SendMessage( "You may not wear this." );
				return false;
			}

			return true;
		}
	}
}