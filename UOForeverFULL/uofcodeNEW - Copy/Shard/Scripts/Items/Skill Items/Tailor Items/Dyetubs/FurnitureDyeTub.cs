using System;
using Server.Multis;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
	public class FurnitureDyeTub : DyeTub, IRewardItem //Needs to be BaseDyeTub without wiping on restart
	{
		public override TextDefinition TargetMessage{ get{ return new TextDefinition( 501019 ); } } // Select the furniture to dye.
		public override TextDefinition FailMessage{ get{ return new TextDefinition( 501021 ); } } // That is not a piece of furniture.
		public override int LabelNumber{ get{ return 1041246; } } // Furniture Dye Tub

		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get{ return m_IsRewardItem; }
			set{ m_IsRewardItem = value; InvalidateProperties(); }
		}

		[Constructable]
		public FurnitureDyeTub() : this( 0 )
		{
		}

		[Constructable]
		public FurnitureDyeTub( int hue ) : this( hue, true )
		{
		}

		[Constructable]
		public FurnitureDyeTub( int hue, bool redyable ) : this( hue, true, -1 )
		{
		}

		[Constructable]
		public FurnitureDyeTub( int hue, bool redyable, int uses ) : base( hue, redyable, uses )
		{
			LootType = LootType.Blessed;
		}

        [Constructable]
        public FurnitureDyeTub(int hue, int uses)
            : base(hue, false, uses)
        {
            LootType = LootType.Blessed;
        }

		public override void OnDoubleClick( Mobile from )
		{
			if ( m_IsRewardItem && !RewardSystem.CheckIsUsableBy( from, this, null ) )
				return;

			base.OnDoubleClick( from );
		}

		public override bool IsDyable( Item item )
		{
			return base.IsDyable( item ) || FurnitureAttribute.Check( item );
		}

		public override bool Dye( Mobile from, Item item )
		{
			bool okay = ( item.IsChildOf( from.Backpack ) );
		    bool IsFurniture = FurnitureAttribute.Check(item);

			if ( !okay )
			{
				if ( item.RootParent == null )
				{
					BaseHouse house = BaseHouse.FindHouseAt( item );

					if ( house == null || ( !house.IsLockedDown( item ) && !house.IsSecure( item ) ) )
						from.SendLocalizedMessage( 501022 ); // Furniture must be locked down to paint it.
					else if ( !house.IsCoOwner( from ) )
						from.SendLocalizedMessage( 501023 ); // You must be the owner to use this item.
					else
						okay = true;
				}
				else
					from.SendLocalizedMessage( 1048135 ); // The furniture must be in your backpack to be painted.
			}

		    if (!IsFurniture && !(item is PotionKeg))
		    {
		        from.SendMessage("This is not a piece of furniture!");
		        return false;
		    }

			if ( okay && item.Dye( from, this ) )
			{
				from.PlaySound( 0x23E );
				return true;
			}
			return false;
		}

		public FurnitureDyeTub( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( EraML && m_IsRewardItem )
				list.Add( 1076217 ); // 1st Year Veteran Reward
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

			if ( LootType == LootType.Regular )
				LootType = LootType.Blessed;
		}
	}
}