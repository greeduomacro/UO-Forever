using System;
using System.Collections.Generic;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.Engines.Craft;
using Server.Mobiles;

namespace Server.Items
{
	public abstract class BaseOre : Item, ICommodity
	{
		private CraftResource m_Resource;

		[CommandProperty( AccessLevel.GameMaster )]
		public CraftResource Resource
		{
			get{ return m_Resource; }
			set{ m_Resource = value; InvalidateProperties(); }
		}

		int ICommodity.DescriptionNumber { get { return LabelNumber; } }
		bool ICommodity.IsDeedable { get { return true; } }

		public abstract BaseIngot GetIngot();

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( (int) m_Resource );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_Resource = (CraftResource)reader.ReadInt();
					break;
				}
				case 0:
				{
					OreInfo info;

					switch ( reader.ReadInt() )
					{
						case 0: info = OreInfo.Iron; break;
						case 1: info = OreInfo.DullCopper; break;
						case 2: info = OreInfo.ShadowIron; break;
						case 3: info = OreInfo.Copper; break;
						case 4: info = OreInfo.Bronze; break;
						case 5: info = OreInfo.Gold; break;
						case 6: info = OreInfo.Agapite; break;
						case 7: info = OreInfo.Verite; break;
						case 8: info = OreInfo.Valorite; break;
						default: info = null; break;
					}

					m_Resource = CraftResources.GetFromOreInfo( info );
					break;
				}
			}
		}

		public BaseOre( CraftResource resource ) : this( resource, 1 )
		{
		}

		public BaseOre( CraftResource resource, int amount ) : base( 6585 )
		{
			Stackable = true;
			Amount = amount;
			Hue = CraftResources.GetHue( resource );

			m_Resource = resource;
		}

		public override bool CanStackWith( Item dropped )
		{
			return dropped.Stackable && Stackable && dropped.GetType() == GetType() && dropped.Hue == Hue && dropped.Name == Name && (dropped.Amount + Amount) <= 60000 && dropped != this;
		}
/*
		protected override void OnAmountChange( int oldValue )
		{
			base.OnAmountChange( oldValue );
			
			if ( Amount >= 4 )
				ItemID = 6585;
			else if ( Amount == 3 )
				ItemID = 6584;
			else if ( Amount == 2 )
				ItemID = 6586;
			else if ( Amount == 1 )
				ItemID = 6583;
			else
				ItemID = 6585;
		}
*/
		public BaseOre( Serial serial ) : base( serial )
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

			if ( !CraftResources.IsStandard( m_Resource ) )
			{
				int num = CraftResources.GetLocalizationNumber( m_Resource );

				if ( num > 0 )
					list.Add( num );
				else
					list.Add( CraftResources.GetName( m_Resource ) );
			}
		}

		public override int LabelNumber
		{
			get
			{
				if ( m_Resource >= CraftResource.DullCopper && m_Resource <= CraftResource.Valorite )
					return 1042845 + (int)(m_Resource - CraftResource.DullCopper);

				return 1042853; // iron ore;
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( Movable )
			{
				BaseCreature bc = RootParent as BaseCreature;

                if (bc != null && bc.NetState == null && !(bc.Controlled && bc.GetMaster() == from))
                {
					from.SendLocalizedMessage( 500447 ); // That is not accessible
                }
				else if ( IsChildOf( from ) || from.InRange( this.GetWorldLocation(), 2 ) )
				{
					//from.SendLocalizedMessage( 501971 ); // Select the forge on which to smelt the ore, or another pile of ore with which to combine it.
					SmeltOre( from );
					//from.Target = new InternalTarget( this );
				}
				else
					from.SendLocalizedMessage( 501976 ); // The ore is too far away.
			}
		}

		public void SmeltOre( Mobile from )
		{
			object forge = null;

			IPooledEnumerable eable = from.GetItemsInRange( 2 );

			foreach ( Item item in eable )
			{
				if ( IsForge( item ) )
				{
					forge = item;
					break;
				}
			}

			eable.Free();

			eable = from.GetMobilesInRange( 2 );

			foreach ( Mobile mob in eable )
			{
				if ( IsForge( mob ) )
				{
					forge = mob;
					break;
				}
			}

			eable.Free();

			if ( forge == null )
			{
				for ( int x = from.X-2;forge == null && x < from.X+2; x++ )
				{
					for ( int y = from.Y-2;forge == null &&  y < from.Y+2; y++ )
					{
						StaticTile[] tiles = from.Map.Tiles.GetStaticTiles( x, y, true );
						for ( int j = 0;forge == null && j < tiles.Length; j++ )
						{
							StaticTarget st = new StaticTarget( tiles[j], tiles[j].ID );
							if ( IsForge( st ) )
								forge = st;
						}
					}
				}
			}

			if ( forge != null )
			{
				double difficulty;

				switch ( Resource )
				{
					default: difficulty = 50.0; break;
					case CraftResource.DullCopper: difficulty = 65.0; break;
					case CraftResource.ShadowIron: difficulty = 70.0; break;
					case CraftResource.Copper: difficulty = 75.0; break;
					case CraftResource.Bronze: difficulty = 80.0; break;
					case CraftResource.Gold: difficulty = 85.0; break;
					case CraftResource.Agapite: difficulty = 90.0; break;
					case CraftResource.Verite: difficulty = 95.0; break;
					case CraftResource.Valorite: difficulty = 99.0; break;
				}

				double minSkill = difficulty - 25.0;
				double maxSkill = difficulty + 25.0;

				if ( difficulty > 50.0 && difficulty > from.Skills[SkillName.Mining].Value )
					from.SendLocalizedMessage( 501986 ); // You have no idea how to smelt this strange ore!
				//else if ( Amount <= 1 )
				//	from.SendLocalizedMessage( 501987 ); // There is not enough metal-bearing ore in this pile to make an ingot.
				else
				{
					/*
					int successes = 0;
					int count = Amount / 2;

					for ( int i = 0;i < count; i++ )
						if ( from.CheckTargetSkill( SkillName.Mining, forge, minSkill, maxSkill ) )
							successes++;

					Consume( count * 2 );

					if ( successes > 0 )
					{
						string[] locals = new string[3];

						BaseIngot ingot = GetIngot();
						ingot.Amount = successes;

						if ( Parent != from.Backpack && Parent is Container && RootParent == from && ((Container)Parent).TryDropItem( from, ingot, false ) ) //Its in a container, on the player
						{
							if ( locals[0] == null )
								locals[0] = "back in the same container";
						}
						else if ( from.AddToBackpack( ingot ) )
						{
							if ( locals[1] == null )
								locals[1] = "in your backpack";
						}
						else	if ( locals[2] == null )
								locals[2] = "on the floor";

						string localText = String.Empty;
						//Trimmed list
						List<string> localslist = new List<string>();
						for ( int i = 0;i < locals.Length; i++ )
							if ( locals[i] != null )
								localslist.Add( locals[i] );

						//We know there is at least ONE location
						localText = localslist[0];

						for ( int i = 1;i < localslist.Count; i++ )
							localText = String.Format( "{0},{1}{2}", localText, (i+1 < localslist.Count) ? " and" : " ", localslist[i] );

						from.SendMessage( "You smelt the ore removing the impurities and put the metal {0}.", localText );
					}
					else
						from.SendLocalizedMessage( 501990 ); // You burn away the impurities but are left with less useable metal.

					from.PlaySound( 0x2B ); // Smelting/Bellow noise
*/
					bool atcap = from.Skills[SkillName.Mining].Value > from.Skills[SkillName.Mining].Cap || from.Skills[SkillName.Mining].Value >= maxSkill;
					//int amountIngot = atcap ? (Amount / 2) : 1;
					//int amountOre = amountIngot * 2;
					int amountIngot = atcap ? Amount : 1;
					from.PlaySound( 0x2B ); // Smelting/Bellow noise

					if ( from.CheckTargetSkill( SkillName.Mining, forge, minSkill, maxSkill ) )
					{
						string[] locals = new string[3];

						BaseIngot ingot = GetIngot();
                        double chanceOfDoublingIronSmelt = from.Skills[SkillName.Mining].Value / 200.0; // 50% chance at 100
                        if (ingot.Resource == CraftResource.Iron && chanceOfDoublingIronSmelt > Utility.RandomDouble()) // per 10 skill, 5% chance of 2 ingots
                        {
                            ingot.Amount = amountIngot * 2;
                            from.SendMessage("You skillfully extract extra metal out of the ore pile.");
                        }
                        else
                        {
                            ingot.Amount = amountIngot;
                        }

						if ( Parent != from.Backpack && Parent is Container && RootParent == from && ((Container)Parent).TryDropItem( from, ingot, false ) ) //Its in a container, on the player
						{
							if ( locals[0] == null )
								locals[0] = "back in the same container";
						}
						else if ( from.AddToBackpack( ingot ) )
						{
							if ( locals[1] == null )
								locals[1] = "in your backpack";
						}
						else	if ( locals[2] == null )
								locals[2] = "on the floor";

						string localText = String.Empty;
						//Trimmed list
						List<string> localslist = new List<string>();
						for ( int i = 0;i < locals.Length; i++ )
							if ( locals[i] != null )
								localslist.Add( locals[i] );

						//We know there is at least ONE location
						localText = localslist[0];

						for ( int i = 1;i < localslist.Count; i++ )
							localText = String.Format( "{0},{1}{2}", localText, (i+1 < localslist.Count) ? " and" : " ", localslist[i] );

						from.SendMessage( "You smelt the ore removing the impurities and put the metal {0}.", localText );
						Consume( amountIngot );
					}
					else
					{
						Consume( Math.Max(amountIngot / 2, 1) );
						from.SendLocalizedMessage( 501990 ); // You burn away the impurities but are left with less useable metal.
					}
				}
			}
			else
				from.SendLocalizedMessage( 500420 ); // You are not near a forge.
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
				itemID = ((StaticTarget)obj).ItemID;

			return ( itemID == 4017 || (itemID >= 6522 && itemID <= 6569) || itemID == 11736 );
		}
	}

	public class IronOre : BaseOre
	{
		[Constructable]
		public IronOre() : this( 1 )
		{
		}

		[Constructable]
		public IronOre( int amount ) : base( CraftResource.Iron, amount )
		{
		}

		public IronOre( Serial serial ) : base( serial )
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

		public override BaseIngot GetIngot()
		{
			return new IronIngot();
		}
	}

	public class DullCopperOre : BaseOre
	{
		[Constructable]
		public DullCopperOre() : this( 1 )
		{
		}

		[Constructable]
		public DullCopperOre( int amount ) : base( CraftResource.DullCopper, amount )
		{
		}

		public DullCopperOre( Serial serial ) : base( serial )
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

		public override BaseIngot GetIngot()
		{
			return new DullCopperIngot();
		}
	}

	public class ShadowIronOre : BaseOre
	{
		[Constructable]
		public ShadowIronOre() : this( 1 )
		{
		}

		[Constructable]
		public ShadowIronOre( int amount ) : base( CraftResource.ShadowIron, amount )
		{
		}

		public ShadowIronOre( Serial serial ) : base( serial )
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

		public override BaseIngot GetIngot()
		{
			return new ShadowIronIngot();
		}
	}

	public class CopperOre : BaseOre
	{
		[Constructable]
		public CopperOre() : this( 1 )
		{
		}

		[Constructable]
		public CopperOre( int amount ) : base( CraftResource.Copper, amount )
		{
		}

		public CopperOre( Serial serial ) : base( serial )
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

		public override BaseIngot GetIngot()
		{
			return new CopperIngot();
		}
	}

	public class BronzeOre : BaseOre
	{
		[Constructable]
		public BronzeOre() : this( 1 )
		{
		}

		[Constructable]
		public BronzeOre( int amount ) : base( CraftResource.Bronze, amount )
		{
		}

		public BronzeOre( Serial serial ) : base( serial )
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

		public override BaseIngot GetIngot()
		{
			return new BronzeIngot();
		}
	}

	public class GoldOre : BaseOre
	{
		[Constructable]
		public GoldOre() : this( 1 )
		{
		}

		[Constructable]
		public GoldOre( int amount ) : base( CraftResource.Gold, amount )
		{
		}

		public GoldOre( Serial serial ) : base( serial )
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

		public override BaseIngot GetIngot()
		{
			return new GoldIngot();
		}
	}

	public class AgapiteOre : BaseOre
	{
		[Constructable]
		public AgapiteOre() : this( 1 )
		{
		}

		[Constructable]
		public AgapiteOre( int amount ) : base( CraftResource.Agapite, amount )
		{
		}

		public AgapiteOre( Serial serial ) : base( serial )
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

		public override BaseIngot GetIngot()
		{
			return new AgapiteIngot();
		}
	}

	public class VeriteOre : BaseOre
	{
		[Constructable]
		public VeriteOre() : this( 1 )
		{
		}

		[Constructable]
		public VeriteOre( int amount ) : base( CraftResource.Verite, amount )
		{
		}

		public VeriteOre( Serial serial ) : base( serial )
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

		public override BaseIngot GetIngot()
		{
			return new VeriteIngot();
		}
	}

	public class ValoriteOre : BaseOre
	{
		[Constructable]
		public ValoriteOre() : this( 1 )
		{
		}

		[Constructable]
		public ValoriteOre( int amount ) : base( CraftResource.Valorite, amount )
		{
		}

		public ValoriteOre( Serial serial ) : base( serial )
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

		public override BaseIngot GetIngot()
		{
			return new ValoriteIngot();
		}
	}
}