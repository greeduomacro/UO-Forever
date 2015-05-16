using System;

namespace Server.Items
{
	//[Flipable( 0x151A, 0x1512 )]
	public class SuitOfSilverArmorComponent : AddonComponent
	{
		public SuitOfSilverArmorComponent( int itemid ) : base( itemid )
		{
		    Name = "a suit of silver armor";
		}

		public SuitOfSilverArmorComponent( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( ItemID == 0x1508 || ItemID == 0x151C )
				DoSwitch();
		}

		public void DoSwitch()
		{
			switch ( ItemID )
			{
				case 0x1508: ItemID = 0x1509; Effects.PlaySound( GetWorldLocation(), Map, 0x387 ); DoTimer(); break;
				case 0x1509: ItemID = 0x1508; break;
				case 0x151C: ItemID = 0x151D; Effects.PlaySound( GetWorldLocation(), Map, 0x387 ); DoTimer(); break;
				case 0x151D: ItemID = 0x151C; break;
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

	public class SuitOfSilverArmorAddon : BaseAddon
	{
		private bool m_East;
		public override BaseAddonDeed Deed { get { return new SuitOfSilverArmorDeed( m_East ); } }

		[Constructable]
		public SuitOfSilverArmorAddon() : this( true )
		{
		}

		[Constructable]
		public SuitOfSilverArmorAddon( bool east ) : base()
		{
			m_East = east;

			AddComponent( new SuitOfSilverArmorComponent( m_East ? 0x1508 : 0x151C ), 0, 0, 0 );
		}

		public SuitOfSilverArmorAddon( Serial serial ) : base( serial )
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

	public class SuitOfSilverArmorDeed : BaseAddonDeed
	{
		private bool m_East;

		public override BaseAddon Addon { get { return new SuitOfSilverArmorAddon( m_East ); } }
		//public override int LabelNumber { get { return 1076265; } } // Suit of Silver Armor
		public override string DefaultName{ get{ return String.Format( "Deed to a Decorative Suit of Silver Armor ({0})", m_East ? "East" : "South" ); } }
		[Constructable]
		public SuitOfSilverArmorDeed() : this( true )
		{
		}
        [Constructable]
		public SuitOfSilverArmorDeed( bool east ) : base()
		{
			m_East = east;
			//LootType = LootType.Blessed;
		}

		public SuitOfSilverArmorDeed( Serial serial ) : base( serial )
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
