using System;
using System.Collections.Generic;

using Server.Engines.Conquests;
using Server.Multis;
using Server.Mobiles;
using Server.Network;
using Server.Targets;
using Server.Prompts;
using Server.ContextMenus;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{
	public class ReagentMoveEntry : ContextMenuEntry
	{
		private Mobile m_From;
		private BaseContainer m_Container;

		public ReagentMoveEntry( Mobile from, BaseContainer cont ) : base( 10066 )
		{
			m_From = from;
			m_Container = cont;
		}

		public override void OnClick()
		{
			if ( m_From.CheckAlive() && m_Container.IsChildOf( m_From ) )
			{
				m_From.Prompt = new ReagentStackPrompt( m_Container );
				m_From.SendMessage( "How many reagents from each stack would you like to move?" );

				//m_From.Target = new MoveReagentsTarget( m_Container );
				//m_From.SendMessage( "What reagents or container of reagents would you like to move?" );
			}
		}
	}

	public class ReagentStackPrompt : Prompt
	{
		private BaseContainer m_Container;

		public ReagentStackPrompt( BaseContainer cont )
		{
			m_Container = cont;
		}

		public override void OnCancel( Mobile from )
		{
			from.SendMessage( "What reagents or container of reagents would you like to move?" );
			from.Target = new MoveReagentsTarget( m_Container, -1 );
		}

		public override void OnResponse( Mobile from, string text )
		{
			int amount = Utility.ToInt32( text.Trim() );

			if ( amount <= 0 || amount > 1000 )
				from.SendMessage( "Please choose an amount between 1 and 1000." );
			else
			{
				from.SendMessage( "What reagents or container of reagents would you like to move?" );
				from.Target = new MoveReagentsTarget( m_Container, amount );
			}
		}
	}

	public abstract class BaseContainer : Container
	{
        // Alan Mod ===================================
        // this is when THIS container is added or removed from something
        public override void OnAdded(object parent)
        {
            if (parent is Item)
            {
                Item parentItem = (Item)parent;

                if (XmlScript.HasTrigger(this, TriggerName.onAdded))
                    UberScriptTriggers.Trigger(this, parentItem.RootParentEntity as Mobile, TriggerName.onAdded, parentItem);
            }
            base.OnAdded(parent);
        }
        
        public override void OnRemoved(object parent)
        {
            if (parent is Item)
            {
                Item parentItem = (Item)parent;
                if (XmlScript.HasTrigger(this, TriggerName.onRemove))
                    UberScriptTriggers.Trigger(this, parentItem.RootParentEntity as Mobile, TriggerName.onRemove, parentItem);
            }

            //Server.Engines.XmlSpawner2.XmlAttach.CheckOnRemoved(this, parent);
            base.OnRemoved(parent);
        }
        public override void OnDelete()
        {
            if (XmlScript.HasTrigger(this, TriggerName.onDelete))
                UberScriptTriggers.Trigger(this, this.RootParentEntity as Mobile, TriggerName.onDelete);
            base.OnDelete();
        }
        // end Alan Mod ==============================
        
        public override int DefaultMaxWeight
		{
			get
			{
				if ( IsSecure )
					return 0;

				return base.DefaultMaxWeight;
			}
		}

		public BaseContainer( int itemID ) : base( itemID )
		{
		}

		public override void OnItemAdded(Item item)
		{
			base.OnItemAdded(item);

			if (item.RootParent is PlayerMobile)
			{
				Conquests.CheckProgress<ObtainConquest>((PlayerMobile)item.RootParent, item);
			}
		}

		public override bool IsAccessibleTo( Mobile m )
		{
			if ( !BaseHouse.CheckAccessible( m, this ) )
				return false;

			return base.IsAccessibleTo( m );
		}

		public override bool CheckHold( Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight )
		{
			if ( this.IsSecure && !BaseHouse.CheckHold( m, this, item, message, checkItems, plusItems, plusWeight ) )
				return false;

			return base.CheckHold( m, item, message, checkItems, plusItems, plusWeight );
		}

		public override bool CheckItemUse( Mobile from, Item item )
		{
			if ( IsDecoContainer && item is BaseBook )
				return true;

			return base.CheckItemUse( from, item );
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			if ( from.AccessLevel >= AccessLevel.GameMaster || (from.CheckAlive() && IsChildOf( from ) && !(this is ILockable && ((ILockable)this).Locked)) ) //Backpack or Bankbox or a container within
				list.Add( new ReagentMoveEntry( from, this ) );

			SetSecureLevelEntry.AddTo( from, this, list );
		}

		public override bool TryDropItem( Mobile from, Item dropped, bool sendFullMessage )
		{
			if ( !CheckHold( from, dropped, sendFullMessage, true ) )
				return false;

            if (XmlScript.HasTrigger(this, TriggerName.onDropIntoContainer) && UberScriptTriggers.Trigger(this, from, TriggerName.onDropIntoContainer, dropped)) // true if return override
            {
                return false;
            }

			BaseHouse house = BaseHouse.FindHouseAt( this );

			if ( house != null && house.IsLockedDown( this ) )
			{
				if ( dropped is VendorRentalContract || ( dropped is Container && ((Container)dropped).FindItemByType( typeof( VendorRentalContract ) ) != null ) )
				{
					from.SendLocalizedMessage( 1062492 ); // You cannot place a rental contract in a locked down container.
					return false;
				}

				if ( !house.LockDown( from, dropped, false ) )
					return false;
			}

			List<Item> list = this.Items;

			for ( int i = 0; i < list.Count; ++i )
			{
				Item item = list[i];

				if ( !(item is Container) && item.StackWith( from, dropped, false ) )
					return true;
			}

			DropItem( dropped );

            // ARTEGORDONMOD
            // Begin mod for spawner release of items
            // set flag to have item taken off spawner list at next defrag
            ItemFlags.SetTaken(dropped, true);
            // End mod for spawner release of items

			return true;
		}

		public override bool OnDragDropInto( Mobile from, Item item, Point3D p )
		{
			if ( !CheckHold( from, item, true, true ) )
				return false;

            if (XmlScript.HasTrigger(this, TriggerName.onDropIntoContainer) && UberScriptTriggers.Trigger(this, from, TriggerName.onDropIntoContainer, item)) // true if return override
            {
                return false;
            }

			BaseHouse house = BaseHouse.FindHouseAt( this );

			if ( house != null && house.IsLockedDown( this ) )
			{
				if ( item is VendorRentalContract || ( item is Container && ((Container)item).FindItemByType( typeof( VendorRentalContract ) ) != null ) )
				{
					from.SendLocalizedMessage( 1062492 ); // You cannot place a rental contract in a locked down container.
					return false;
				}

				if ( !house.LockDown( from, item, false ) )
					return false;
			}

			item.Location = new Point3D( p.X, p.Y, 0 );
			AddItem( item );

			from.SendSound( GetDroppedSound( item ), GetWorldLocation() );

            // ARTEGORDONMOD
            // Begin mod for spawner release of items
            // set flag to have item taken off spawner list at next defrag
            ItemFlags.SetTaken(item, true);
            // End mod for spawner release of items

			return true;
		}

		public override void UpdateTotal( Item sender, TotalType type, int delta )
		{
			base.UpdateTotal( sender, type, delta );

			if ( type == TotalType.Weight && RootParent is Mobile )
				((Mobile) RootParent).InvalidateProperties();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.AccessLevel > AccessLevel.Player || from.InRange( this.GetWorldLocation(), 2 ) || this.RootParent is PlayerVendor )
				Open( from );
			else
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
		}

		public virtual void Open( Mobile from )
		{
			DisplayTo( from );
		}

		public BaseContainer( Serial serial ) : base( serial )
		{
		}

		/* Note: base class insertion; we cannot serialize anything here */
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
		}
	}

	public class CreatureBackpack : Backpack	//Used on BaseCreature
	{
		[Constructable]
		public CreatureBackpack( string name ) : base()
		{
			//Name = name;
			//Layer = Layer.Backpack;
			//Hue = 5;
			//Weight = 3.0;
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			if ( Name != null )
				list.Add( 1075257, Name ); // Contents of ~1_PETNAME~'s pack.
			else
				base.AddNameProperty( list );
		}

		public override void OnItemRemoved( Item item )
		{
			if ( Items.Count == 0 )
				this.Delete();

			base.OnItemRemoved( item );
		}

		public override bool OnDragLift( Mobile from )
		{
			if ( from.AccessLevel > AccessLevel.Player )
				return true;

			from.SendLocalizedMessage( 500169 ); // You cannot pick that up.
			return false;
		}

		public override bool OnDragDropInto( Mobile from, Item item, Point3D p )
		{
			return false;
		}

		public override bool TryDropItem( Mobile from, Item dropped, bool sendFullMessage )
		{
			return false;
		}

		public CreatureBackpack( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			//if ( version == 0 )
			//	Weight = 13.0;
		}
	}

	public class StrongBackpack : Backpack	//Used on Pack animals
	{
		[Constructable]
		public StrongBackpack() : base()
		{
			//Layer = Layer.Backpack;
			Weight = 13.0;
		}

		public override bool CheckHold( Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight )
		{
			return base.CheckHold( m, item, false, checkItems, plusItems, plusWeight );
		}

		public override int DefaultMaxWeight{ get{ return 1600; } }

		public override bool CheckContentDisplay( Mobile from )
		{
			BaseCreature bc = this.RootParent as BaseCreature;

			return (bc != null && bc.Controlled && bc.ControlMaster == from) || base.CheckContentDisplay( from );
		}

		public StrongBackpack( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			//if ( version == 0 )
			//	Weight = 13.0;
		}
	}

	public class Backpack : BaseContainer
	{
		[CommandProperty(AccessLevel.Counselor)]
		public override bool ExpansionChangeAllowed { get { return false; } }

		public override bool DisplayDyable{ get{ return false; } }
		public override bool DisplayLootType{ get{ return !(Parent is Mobile); } }

		public override int DefaultMaxWeight {
			get {
				if (EraML)
				{
					Mobile m = ParentEntity as Mobile;
					if ( m != null && m.Player && m.Backpack == this ) {
						return 550;
					} else {
						return base.DefaultMaxWeight;
					}
				} else {
					return base.DefaultMaxWeight;
				}
			}
		}

		[Constructable]
		public Backpack()
			: this(Expansion.None)
		{ }

		public Backpack(Expansion e) : base( 0xE75 )
		{
			Expansion = e;
			Layer = Layer.Backpack;
			Weight = 3.0;
			Dyable = true;
		}

		public Backpack( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			//if ( version == 0 && ItemID == 0x9B2 )
			//	ItemID = 0xE75;
		}
	}

	public class Pouch : TrapableContainer
	{
		public override bool HueOnMagicTrap{ get{ return true; } }

		[Constructable]
		public Pouch() : base( 0xE79 )
		{
			Weight = 1.0;
		}

		public Pouch( Serial serial ) : base( serial )
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
	public class TPouch : TrapableContainer
	{
		public override bool HueOnMagicTrap{ get{ return true; } }

		[Constructable]
		public TPouch() : base( 0xE79 )
		{
			Weight = 1.0;
			TrapType = TrapType.MagicTrap;
			TrapPower = 1;
		}

		public TPouch( Serial serial ) : base( serial )
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
	public class CTFPouch : TrapableContainer
	{
		public override bool HueOnMagicTrap{ get{ return true; } }

		[Constructable]
		public CTFPouch() : base( 0xE79 )
		{
			Weight = 1.0;
			TrapType = TrapType.MagicTrap;
			TrapPower = 1;
		}

		public CTFPouch( Serial serial ) : base( serial )
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
	public abstract class BaseBagBall : BaseContainer
	{
		public BaseBagBall( int itemID ) : base( itemID )
		{
			Weight = 1.0;
			Dyable = true;
		}

		public override bool DisplayDyable{ get{ return false; } }

		public BaseBagBall( Serial serial ) : base( serial )
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

	public class SmallBagBall : BaseBagBall
	{
		[Constructable]
		public SmallBagBall() : base( 0x2256 )
		{
		}

		public SmallBagBall( Serial serial ) : base( serial )
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

	public class LargeBagBall : BaseBagBall
	{
		[Constructable]
		public LargeBagBall() : base( 0x2257 )
		{
		}

		public LargeBagBall( Serial serial ) : base( serial )
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

	public class Bag : BaseContainer
	{
		[Constructable]
		public Bag() : base( 0xE76 )
		{
			Weight = 2.0;
			Dyable = true;
		}

		public override bool DisplayDyable{ get{ return false; } }

		public Bag( Serial serial ) : base( serial )
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

	public class Barrel : BaseContainer
	{
		[Constructable]
		public Barrel() : base( 0xE77 )
		{
			Weight = 25.0;
		}

		public Barrel( Serial serial ) : base( serial )
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

			if ( Weight == 0.0 )
				Weight = 25.0;
		}
	}

	public class Keg : BaseContainer
	{
		[Constructable]
		public Keg() : base( 0xE7F )
		{
			Weight = 15.0;
		}

		public Keg( Serial serial ) : base( serial )
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

	public class PicnicBasket : BaseContainer
	{
		[Constructable]
		public PicnicBasket() : base( 0xE7A )
		{
			Weight = 2.0; // Stratics doesn't know weight
		}

		public PicnicBasket( Serial serial ) : base( serial )
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

	public class Basket : BaseContainer
	{
		[Constructable]
		public Basket() : base( 0x990 )
		{
			Weight = 1.0; // Stratics doesn't know weight
		}

		public Basket( Serial serial ) : base( serial )
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

	[Furniture]
	[Flipable( 0x9AA, 0xE7D )]
	public class WoodenBox : LockableContainer
	{
		[Constructable]
		public WoodenBox() : base( 0xE7D )
		{
			Weight = 4.0;
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public WoodenBox( Serial serial ) : base( serial )
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

	[Furniture]
	[Flipable( 0x2DF1, 0x2DF1 )]
	public class ElevenBox : LockableContainer
	{
		[Constructable]
		public ElevenBox() : base( 0x2DF1 )
		{
			Weight = 4.0;
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public ElevenBox( Serial serial ) : base( serial )
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

	[Furniture]
	[Flipable( 0x9A9, 0xE7E )]
	public class SmallCrate : LockableContainer
	{
		[Constructable]
		public SmallCrate() : base( 0xE7E )
		{
			Weight = 2.0;
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public SmallCrate( Serial serial ) : base( serial )
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

			if ( Weight == 4.0 )
				Weight = 2.0;
		}
	}

	[Furniture]
	[Flipable( 0xE3F, 0xE3E )]
	public class MediumCrate : LockableContainer
	{
		[Constructable]
		public MediumCrate() : base( 0xE3E )
		{
			Weight = 3.0;
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public MediumCrate( Serial serial ) : base( serial )
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

			if ( Weight == 6.0 || Weight == 2.0 )
				Weight = 3.0;
		}
	}

	[Furniture]
	[Flipable( 0xE3D, 0xE3C )]
	public class LargeCrate : LockableContainer
	{
		[Constructable]
		public LargeCrate() : base( 0xE3C )
		{
			Weight = 4.0;
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public LargeCrate( Serial serial ) : base( serial )
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

			if ( Weight == 8.0 )
				Weight = 1.0;
		}
	}

	[DynamicFliping]
	[Flipable( 0x9A8, 0xE80 )]
	public class MetalBox : LockableContainer
	{
		[Constructable]
		public MetalBox() : base( 0x9A8 )
		{
		}

		public MetalBox( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && Weight == 3 )
				Weight = -1;
		}
	}

	[DynamicFliping]
	[Flipable( 0x9AB, 0xE7C )]
	public class MetalChest : LockableContainer
	{
		[Constructable]
		public MetalChest() : base( 0x9AB )
		{
		}

		public MetalChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && Weight == 25 )
				Weight = -1;
		}
	}

	[DynamicFliping]
	[Flipable( 0xE41, 0xE40 )]
	public class MetalGoldenChest : LockableContainer
	{
		[Constructable]
		public MetalGoldenChest() : base( 0xE41 )
		{
		}

		public MetalGoldenChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && Weight == 25 )
				Weight = -1;
		}
	}

	[Furniture]
	[Flipable( 0xE43, 0xE42 )]
	public class WoodenChest : LockableContainer
	{
		[Constructable]
		public WoodenChest() : base( 0xE42 )
		{
			Weight = 2.0;
		}

		public WoodenChest( Serial serial ) : base( serial )
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

			if ( Weight == 15.0 )
				Weight = 2.0;
		}
	}

	[Furniture]
	[Flipable( 0x280B, 0x280C )]
	public class PlainWoodenChest : LockableContainer
	{
		[Constructable]
		public PlainWoodenChest() : base( 0x280C )
		{
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public PlainWoodenChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && Weight == 15 )
				Weight = -1;
		}
	}

	[Furniture]
	[Flipable( 0x280D, 0x280E )]
	public class OrnateWoodenChest : LockableContainer
	{
		[Constructable]
		public OrnateWoodenChest() : base( 0x280E )
		{
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public OrnateWoodenChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && Weight == 15 )
				Weight = -1;
		}
	}

	[Furniture]
	[Flipable( 0x280F, 0x2810 )]
	public class GildedWoodenChest : LockableContainer
	{
		[Constructable]
		public GildedWoodenChest() : base( 0x2810 )
		{
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public GildedWoodenChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && Weight == 15 )
				Weight = -1;
		}
	}

	[Furniture]
	[Flipable( 0x2811, 0x2812 )]
	public class WoodenFootLocker : LockableContainer
	{
		[Constructable]
		public WoodenFootLocker() : base( 0x2812 )
		{
			Dyable = true;
			GumpID = 0x10B;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public WoodenFootLocker( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && Weight == 15 )
				Weight = -1;

			if ( version < 2 )
				GumpID = 0x10B;
		}
	}

	[Furniture]
	[Flipable( 0x2813, 0x2814 )]
	public class FinishedWoodenChest : LockableContainer
	{
		[Constructable]
		public FinishedWoodenChest() : base( 0x2814 )
		{
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public FinishedWoodenChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && Weight == 15 )
				Weight = -1;
		}
	}
}