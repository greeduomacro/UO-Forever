using System;
using Server.Engines.Craft;

namespace Server.Items
{
	//[Flipable( 0x151A, 0x1512 )]
    public class SuitOfGoldArmorComponent : AddonComponent
	{
		public override int LabelNumber { get { return 1076265; } } // Suit of Gold Armor
                [Constructable]
		public SuitOfGoldArmorComponent( int itemid ) : base( itemid )
		{
		}
                [Constructable]
		public SuitOfGoldArmorComponent( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( ItemID == 0x15A1A || ItemID == 0x1512 )
				DoSwitch();
		}

		public void DoSwitch()
		{
			switch ( ItemID )
			{
				case 0x151A: ItemID = 0x151B; Effects.PlaySound( GetWorldLocation(), Map, 0x387 ); DoTimer(); break;
				case 0x151B: ItemID = 0x151A; break;
				case 0x1512: ItemID = 0x1513; Effects.PlaySound( GetWorldLocation(), Map, 0x387 ); DoTimer(); break;
				case 0x1513: ItemID = 0x1512; break;
			}
		}

		public void DoTimer()
		{
			Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerCallback( DoSwitch ) );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class SuitOfGoldArmorAddon : BaseAddon
	{
		private bool m_East;
		public override BaseAddonDeed Deed { get { return new SuitOfGoldArmorDeed( m_East ); } }

		[Constructable]
		public SuitOfGoldArmorAddon() : this( true )
		{
		}

		[Constructable]
		public SuitOfGoldArmorAddon( bool east ) : base()
		{
			m_East = east;

			AddComponent( new AddonComponent( m_East ? 0x151A : 0x1512 ), 0, 0, 0 );
		}

		public SuitOfGoldArmorAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 1 ); // version
			writer.Write( m_East );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			if ( version >= 1 )
				m_East = reader.ReadBool();
			else
				m_East = true;
		}
	}

	public class SuitOfGoldArmorDeed : BaseAddonDeed
	{
		private bool m_East;

		public override BaseAddon Addon { get { return new SuitOfGoldArmorAddon( m_East ); } }
		//public override int LabelNumber { get { return 1076265; } } // Suit of Gold Armor
		public override string DefaultName{ get{ return String.Format( "Deed to a Decorative Suit of Gold Armor ({0})", m_East ? "East" : "South" ); } }

		[Constructable]
		public SuitOfGoldArmorDeed() : this( true )
		{
		}

		[Constructable]
		public SuitOfGoldArmorDeed( bool east ) : base()
		{
			m_East = east;
			//LootType = LootType.Blessed;
		}

		public SuitOfGoldArmorDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 1 ); // version
			writer.Write( m_East );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			if ( version >= 1 )
				m_East = reader.ReadBool();
			else
				m_East = true;
		}
	}
}