using System;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
	public class RunebookDyeTub : DyeTub, IRewardItem
	{
		public override TextDefinition TargetMessage{ get{ return new TextDefinition( 1042416 ); } } // Select the Runebook item to dye.
		public override TextDefinition FailMessage{ get{ return new TextDefinition( 1042418 ); } } // You can only dye Runebook with this tub.
		public override int LabelNumber{ get{ return 1049740; } } // Runebook Dye Tub
		public override CustomHuePicker CustomHuePicker{ get{ return CustomHuePicker.LeatherDyeTub; } }

		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get{ return m_IsRewardItem; }
			set{ m_IsRewardItem = value; InvalidateProperties(); }
		}

		[Constructable]
		public RunebookDyeTub() : this( 0 )
		{
		}

		[Constructable]
		public RunebookDyeTub( int hue ) : this( hue, true )
		{
		}

		[Constructable]
		public RunebookDyeTub( int hue, bool redyable ) : this( hue, true, -1 )
		{
		}

		[Constructable]
		public RunebookDyeTub( int hue, bool redyable, int uses ) : base( hue, redyable, uses )
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
				from.SendLocalizedMessage( 1042419 ); // You may not dye Runebook items which are locked down.
			else if ( item.Parent is Mobile )
				from.SendLocalizedMessage( 500861 ); // Can't Dye Runebook that is being worn.
			else if ( item.Dye( from, this ) )
			{
				from.PlaySound( 0x23E );
				return true;
			}

			return false;
		}

		public RunebookDyeTub( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( EraML && m_IsRewardItem )
				list.Add( 1076218 ); // 2nd Year Veteran Reward
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( (bool) m_IsRewardItem );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

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