using System;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
	public class RewardMetalDyeTub : BaseDyeTub, IRewardItem
	{
		public override TextDefinition TargetMessage{ get{ return new TextDefinition( 1080393 ); } } // Select the metal item to dye.
		public override TextDefinition FailMessage{ get{ return new TextDefinition( 1080394 ); } } // You can only dye metal with this tub.
		public override int LabelNumber{ get{ return 1080396; } } // Reward Metal Dye Tub

		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get{ return m_IsRewardItem; }
			set{ m_IsRewardItem = value; InvalidateProperties(); }
		}

		[Constructable]
		public RewardMetalDyeTub() : this( 0 )
		{
		}

		[Constructable]
		public RewardMetalDyeTub( int hue ) : this( hue, true )
		{
		}

		[Constructable]
		public RewardMetalDyeTub( int hue, bool redyable ) : this( hue, true, -1 )
		{
		}

		[Constructable]
		public RewardMetalDyeTub( int hue, bool redyable, int uses ) : base( hue, redyable, uses )
		{
			LootType = LootType.Blessed;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( m_IsRewardItem && !RewardSystem.CheckIsUsableBy( from, this, null ) )
				return;

			base.OnDoubleClick( from );
		}

		public override bool Dye( Mobile from, Item item )
		{
			if ( !item.Movable )
				from.SendLocalizedMessage( 1080395 ); // You may not dye metal items which are locked down.
			else if ( item.Parent is Mobile )
				from.SendLocalizedMessage( 500861 ); // Can't Dye clothing that is being worn.
			else if ( item.Dye( from, this ) )
			{
				from.PlaySound( 0x23E );
				return true;
			}

			return false;
		}

		public RewardMetalDyeTub( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( EraML && m_IsRewardItem )
				list.Add( 1113803 ); // 13th Year Veteran Reward
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 1 ); // version

			writer.Write( (bool) m_IsRewardItem );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			switch ( version )
			{
				case 1:
				{
					m_IsRewardItem = reader.ReadBool();
					break;
				}
			}
		}
	}
}