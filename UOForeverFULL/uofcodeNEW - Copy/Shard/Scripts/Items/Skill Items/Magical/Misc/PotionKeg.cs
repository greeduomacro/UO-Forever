using System;
using Server;
using Server.Targeting;

namespace Server.Items
{
	public class PotionKeg : Item
	{
		private PotionEffect m_Type;
		private int m_Held;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Held
		{
			get
			{
				return m_Held;
			}
			set
			{
				if ( m_Held != value )
				{
					m_Held = value;
					UpdateWeight();
					InvalidateProperties();
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public PotionEffect Type
		{
			get
			{
				return m_Type;
			}
			set
			{
				m_Type = value;
				InvalidateProperties();
			}
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		[Constructable]
		public PotionKeg() : base( 0x1940 )
		{
			UpdateWeight();
			Dyable = true;
		}

		public virtual void UpdateWeight()
		{
			int held = Math.Max( 0, Math.Min( m_Held, 100 ) );

			this.Weight = 20 + ((held * 80) / 100);
		}

		public PotionKeg( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( (int) m_Type );
			writer.Write( (int) m_Held );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				case 0:
				{
					m_Type = (PotionEffect)reader.ReadInt();
					m_Held = reader.ReadInt();

					break;
				}
			}

			if ( version < 1 )
				Timer.DelayCall( TimeSpan.Zero, new TimerCallback( UpdateWeight ) );
		}

		public override int LabelNumber
		{
			get
			{
				if ( m_Held > 0 )
				{
					if ( m_Type < PotionEffect.Conflagration )
						return 1041620 + (int)m_Type;
					else if ( m_Type < PotionEffect.ManaRefresh )
						return 1072658 + ((int)(m_Type-PotionEffect.Conflagration));
					else
						return 0;
				}
				else
					return 1041084; // A specially lined keg for potions.
			}
		}

		public override string DefaultName{ get{ return ( ( m_Held <= 0 || m_Type < PotionEffect.ManaRefresh ) ? null : GetNameType() ); } }

		private string GetNameType()
		{
			switch ( m_Type )
			{
				default:
				case PotionEffect.Nightsight:					return "a keg of Nightsight potions";

				case PotionEffect.CureLesser:					return "a keg of Lesser Cure potions";
				case PotionEffect.Cure:							return "a keg of Cure potions";
				case PotionEffect.CureGreater:					return "a keg of Greater Cure potions";

				case PotionEffect.Agility:						return "a keg of Agility potions";
				case PotionEffect.AgilityGreater:				return "a keg of Greater Agility potions";

				case PotionEffect.Strength:						return "a keg of Strength potions";
				case PotionEffect.StrengthGreater:				return "a keg of Greater Strength potions";

				case PotionEffect.PoisonLesser:					return "a keg of Lesser Poison potions";
				case PotionEffect.Poison:						return "a keg of Poison potions";
				case PotionEffect.PoisonGreater:				return "a keg of Greater Poison potions";
				case PotionEffect.PoisonDeadly:					return "a keg of Deadly Poison potions";

				case PotionEffect.Refresh:						return "a keg of Refresh potions";
				case PotionEffect.RefreshTotal:					return "a keg of Total Refresh potions";

				case PotionEffect.HealLesser:					return "a keg of Lesser Heal potions";
				case PotionEffect.Heal:							return "a keg of Heal potions";
				case PotionEffect.HealGreater:					return "a keg of Greater Heal potions";

				case PotionEffect.ExplosionLesser:				return "a keg of Lesser Explosion potions";
				case PotionEffect.Explosion:					return "a keg of Explosion potions";
				case PotionEffect.ExplosionGreater:				return "a keg of Greater Explosion potions";

				case PotionEffect.Conflagration: 				return "a keg of Conflagration potions";
				case PotionEffect.ConflagrationGreater:			return "a keg of Greater Conflagration potions";

				case PotionEffect.MaskOfDeath: 					return "a keg of Mask of Death potions";
				case PotionEffect.MaskOfDeathGreater: 			return "a keg of Greater Mask of Death potions";

				case PotionEffect.ConfusionBlast: 				return "a keg of Confuse Blast potions";
				case PotionEffect.ConfusionBlastGreater: 		return "a keg of Greater Confuse Blast potions";

				case PotionEffect.ManaRefresh:					return "a keg of Mana Refresh potions";
				case PotionEffect.TotalManaRefresh:				return "a keg of Total Mana Refresh potions";
			}
		}

		public int GetFillLevel()
		{
			if ( m_Held <= 0 )
				return 502246; // The keg is empty.
			else if ( m_Held < 5 )
				return 502248; // The keg is nearly empty.
			else if ( m_Held < 20 )
				return 502249; // The keg is not very full.
			else if ( m_Held < 30 )
				return 502250; // The keg is about one quarter full.
			else if ( m_Held < 40 )
				return 502251; // The keg is about one third full.
			else if ( m_Held < 47 )
				return 502252; // The keg is almost half full.
			else if ( m_Held < 54 )
				return 502254; // The keg is approximately half full.
			else if ( m_Held < 70 )
				return 502253; // The keg is more than half full.
			else if ( m_Held < 80 )
				return 502255; // The keg is about three quarters full.
			else if ( m_Held < 96 )
				return 502256; // The keg is very full.
			else if ( m_Held < 100 )
				return 502257; // The liquid is almost to the top of the keg.
			else
				return 502258; // The keg is completely full.
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( GetFillLevel() );
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			this.LabelTo( from, GetFillLevel() );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( GetWorldLocation(), 2 ) )
			{
				if ( m_Held > 0 )
				{
					Container pack = from.Backpack;

					if ( pack == null || !PourBottle( from, pack.FindItemByType( typeof( Bottle ) ) ) )
					{
						from.SendLocalizedMessage( 502241 ); // Where is a container for your potion?
						from.Target = new InternalTarget( this );
					}
				}
				else
				{
					from.SendLocalizedMessage( 502246 ); // The keg is empty.
				}
			}
			else
			{
				from.LocalOverheadMessage( Network.MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
			}
		}

		public bool PourBottle( Mobile from, Item item )
		{
			if ( item is Bottle )
			{
				Container pack = from.Backpack;

				if ( pack != null )
				{
				    Container parentpack = item.Parent as Container;

					item.Consume(); //Consume a bottle

					from.SendLocalizedMessage( 502242 ); // You pour some of the keg's contents into an empty bottle...

					BasePotion pot = FillBottle();

					if ( parentpack != null && parentpack.TryDropItem( from, pot, false ) )
					{
						from.SendLocalizedMessage( 502243 ); // ...and place it into your backpack.
						from.PlaySound( 0x240 );

						if ( --Held == 0 )
							from.SendLocalizedMessage( 502245 ); // The keg is now empty.
					}
					else
					{
						from.SendLocalizedMessage( 502244 ); // ...but there is no room for the bottle in your backpack.
						//pot.Delete();
						pot.MoveToWorld( pack.GetWorldLocation(), pack.Map );
					}
					return true;
				}
			}
			else if ( item is PotionKeg )
			{
				PotionKeg keg = item as PotionKeg;

				if ( keg.Held >= 100 )
					from.SendLocalizedMessage( 502233 ); // The keg will not hold any more!
				else if ( m_Type != keg.Type )
					from.SendLocalizedMessage( 502236 ); // You decide that it would be a bad idea to mix different types of potions.
				else
				{
					int toHold = Math.Min( 100 - keg.Held, m_Held );

					keg.Held += toHold;

					if ( ( Held -= toHold ) == 0 )
						from.SendLocalizedMessage( 502245 ); // The keg is now empty.

					from.PlaySound( 0x240 );

					return true;
				}
			}

			return false;
		}

		public class InternalTarget : Target
		{
			private PotionKeg m_Keg;

			public InternalTarget( PotionKeg keg ) : base( 2, false, TargetFlags.None )
			{
				m_Keg = keg;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( m_Keg == null || m_Keg.Deleted || !m_Keg.IsAccessibleTo( from ) ) //Make sure the keg exists, and is on the source
					return;

				Item item = o as Item;

				if ( item == m_Keg )
					from.SendLocalizedMessage( 502225 ); // The potion is already in that keg!
				else if ( item is Bottle || item is PotionKeg )
					m_Keg.PourBottle( from, item );
				else
					from.SendLocalizedMessage( 502227 ); // That cannot be used to hold a potion.
			}
		}

		public override bool OnDragDrop( Mobile from, Item item )
		{
			if ( item is BasePotion )
			{
				BasePotion pot = (BasePotion)item;
				int toHold = Math.Min( 100 - m_Held, pot.Amount );

				if ( toHold <= 0 )
					from.SendLocalizedMessage( 502233 ); // The keg will not hold any more!
				else if ( m_Held > 0 && pot.PotionEffect != m_Type )
					from.SendLocalizedMessage( 502236 ); // You decide that it would be a bad idea to mix different types of potions.
				else if ( pot.PotionEffect > PotionEffect.TotalManaRefresh )
						from.SendLocalizedMessage( 502232 ); // The keg is not designed to hold that type of object.
/*
					#region Mondain's Legacy
					if ( (int) pot.PotionEffect >= (int) PotionEffect.Invisibility )
					{
						from.SendLocalizedMessage( 502232 ); // The keg is not designed to hold that type of object.
						return false;
					}
					#endregion
*/

				else
				{
					if ( GiveBottle( from, toHold ) )
					{
						if ( m_Held == 0 )
							m_Type = pot.PotionEffect;

						Held += toHold;

						from.PlaySound( 0x240 );

						from.SendLocalizedMessage( 502237 ); // You place the empty bottle in your backpack.

						item.Consume( toHold );

						if( !item.Deleted )
							item.Bounce( from );

						return true;
					}
					else
						from.SendLocalizedMessage( 502238 ); // You don't have room for the empty bottle in your backpack.
				}
			}
			else
				from.SendLocalizedMessage( 502232 ); // The keg is not designed to hold that type of object.

			return false;
		}

		public bool GiveBottle( Mobile m, int amount )
		{
			Container pack = m.Backpack;

			Bottle bottle = new Bottle( amount );

			if ( pack == null || !pack.TryDropItem( m, bottle, false ) )
			{
				bottle.Delete();
				return false;
			}

			return true;
		}

		public BasePotion FillBottle()
		{
			switch ( m_Type )
			{
				default:
				case PotionEffect.Nightsight:					return new NightSightPotion();

				case PotionEffect.CureLesser:					return new LesserCurePotion();
				case PotionEffect.Cure:							return new CurePotion();
				case PotionEffect.CureGreater:					return new GreaterCurePotion();

				case PotionEffect.Agility:						return new AgilityPotion();
				case PotionEffect.AgilityGreater:				return new GreaterAgilityPotion();

				case PotionEffect.Strength:						return new StrengthPotion();
				case PotionEffect.StrengthGreater:				return new GreaterStrengthPotion();

				case PotionEffect.PoisonLesser:					return new LesserPoisonPotion();
				case PotionEffect.Poison:						return new PoisonPotion();
				case PotionEffect.PoisonGreater:				return new GreaterPoisonPotion();
				case PotionEffect.PoisonDeadly:					return new DeadlyPoisonPotion();

				case PotionEffect.Refresh:						return new RefreshPotion();
				case PotionEffect.RefreshTotal:					return new TotalRefreshPotion();

				case PotionEffect.HealLesser:					return new LesserHealPotion();
				case PotionEffect.Heal:							return new HealPotion();
				case PotionEffect.HealGreater:					return new GreaterHealPotion();

				case PotionEffect.ExplosionLesser:				return new LesserExplosionPotion();
				case PotionEffect.Explosion:					return new ExplosionPotion();
				case PotionEffect.ExplosionGreater:				return new GreaterExplosionPotion();

				case PotionEffect.Conflagration:				return new ConflagrationPotion();
				case PotionEffect.ConflagrationGreater:			return new GreaterConflagrationPotion();

				case PotionEffect.ConfusionBlast:				return new ConfusionBlastPotion();
				case PotionEffect.ConfusionBlastGreater:		return new GreaterConfusionBlastPotion();

				case PotionEffect.ManaRefresh:					return new ManaRefreshPotion();
				case PotionEffect.TotalManaRefresh:				return new TotalManaRefreshPotion();
			}
		}

		public static void Initialize()
		{
			TileData.ItemTable[0x1940].Height = 4;
		}
	}
}