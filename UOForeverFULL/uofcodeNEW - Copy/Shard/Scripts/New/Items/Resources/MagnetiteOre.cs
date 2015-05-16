using System;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.Engines.Craft;

namespace Server.Items
{
	public class MagnetiteOre : Item
	{
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

		[Constructable]
		public MagnetiteOre() : this( 1 )
		{
		}

		[Constructable]
		public MagnetiteOre( int amount ) : base( 0x19B9 )
		{
			Stackable = true;
			Weight = 10.0;
			Amount = amount;
			Hue = 2306;
		}

		public MagnetiteOre( Serial serial ) : base( serial )
		{
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			if ( Amount > 1 )
				list.Add( 1050039, "{0}\t#{1}", Amount, 1026583 ); // ~1_NUMBER~ ~2_ITEMNAME~
			else
				list.Add( 1026583 ); // ore
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( "magnetite" );
		}

		public override string DefaultName{ get{ return "magnetite ore"; } }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !Movable )
				return;

			if ( from.InRange( this.GetWorldLocation(), 2 ) )
			{
				from.SendLocalizedMessage( 501971 ); // Select the forge on which to smelt the ore, or another pile of ore with which to combine it.
				from.Target = new InternalTarget( this );
			}
			else
				from.SendLocalizedMessage( 501976 ); // The ore is too far away.
		}

		private class InternalTarget : Target
		{
			private MagnetiteOre m_Ore;

			public InternalTarget( MagnetiteOre ore ) :  base ( 2, false, TargetFlags.None )
			{
				m_Ore = ore;
			}

			private bool IsForge( object obj )
			{
				if ( /*Core.ML &&*/ obj is Mobile && ((Mobile)obj).IsDeadBondedPet )
					return false;

				if ( obj.GetType().IsDefined( typeof( ForgeAttribute ), false ) )
					return true;

				int itemID = 0;

				if ( obj is Item )
					itemID = ((Item)obj).ItemID;
				else if ( obj is StaticTarget )
					itemID = ((StaticTarget)obj).ItemID & 0x3FFF;

				return ( itemID == 4017 || (itemID >= 6522 && itemID <= 6569) || itemID == 11736 );
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Ore.Deleted )
					return;

				if ( !from.InRange( m_Ore.GetWorldLocation(), 2 ) )
					from.SendLocalizedMessage( 501976 ); // The ore is too far away.
				else if ( IsForge( targeted ) )
				{
					double difficulty = 100.0;

					double minSkill = difficulty;
					double maxSkill = difficulty + 100.0;

					if ( difficulty > 50.0 && difficulty > from.Skills[SkillName.Mining].Value )
						from.SendLocalizedMessage( 501986 ); // You have no idea how to smelt this strange ore!
					else if ( m_Ore.Amount < 2 )
						from.SendMessage( "There is not enough ore in this pile to make a stone." );
					else if ( from.CheckTargetSkill( SkillName.Mining, targeted, minSkill, maxSkill ) )
					{
						int toConsume = m_Ore.Amount;

						if ( toConsume > 60000 )
							toConsume = 60000;

						int toadd = toConsume / 2;

						m_Ore.Consume( toConsume );

						from.PlaySound( 0x2B );
						from.PlaySound( 0x240 );

						from.AddToBackpack( new MagnetiteOre( toadd ) );

						//from.PlaySound( 0x57 );

						from.SendMessage( "You smelt the ore removing the impurities and put the stone{0} in your backpack.", toadd != 1 ? "s" : String.Empty );
					}
					else if ( m_Ore.Amount < 2 )
					{
						from.SendMessage( "You burn away the impurities but are left with no useable stone." );
						m_Ore.Consume();
					}
					else
					{
						from.SendMessage( "You burn away the impurities but are left with less useable material." );
						m_Ore.Amount /= 2;
					}
				}
			}
		}
	}
}