using System;
using System.Text;
using System.Collections.Generic;
using Server.Factions;
using Server.Network;
using Server.Spells;
using Server.Spells.First;
using Server.Spells.Second;
using Server.Spells.Third;
using Server.Spells.Fourth;
using Server.Targeting;

namespace Server.Items
{
	public enum StaffEffect
	{
		None	= 0,
		Healing = 3,
		Harm = 11,
		Fireball = 17,
		GreaterHealing = 28,
		Lightning = 29
	}

	[FlipableAttribute( 0x13F8, 0x13F9 )]
	public class GnarledStaff : BaseStaff
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }
        
		public override int OldStrengthReq{ get{ return 20; } }
        public override int NewMinDamage { get { return WeaponDamageController._GnarledStaffDamageMin; } }
        public override int NewMaxDamage { get { return WeaponDamageController._GnarledStaffDamageMax; } }
		public override int DiceDamage { get { return Utility.Dice( 5, 5, 5 ); } } // 5d5+5 (10-30)
		public override int OldSpeed{ get{ return 33; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 50; } }

		private StaffEffect m_Effect;
		private int m_Charges;

		public static TimeSpan GetUseDelay { get { return TimeSpan.FromSeconds( 4.0 ); } }
		public int MaxCharges{ get{ return 40 / ( ( (int)m_Effect / 8 ) + 1 ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public StaffEffect Effect
		{
			get { return m_Effect; }
			set	{ m_Effect = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges
		{
			get { return m_Charges; }
			set	{ m_Charges = value; }
		}

		[Constructable]
		public GnarledStaff() : base( 0x13F8 )
		{
			Weight = 3.0;
			Resource = CraftResource.RegularWood;
		}

		public GnarledStaff( Serial serial ) : base( serial )
		{
		}

		public void Charge( Mobile from, SpellScroll scroll )
		{
			if ( m_Effect != StaffEffect.None )
				from.SendLocalizedMessage( 1010379 ); // This staff has already been charged - you may not recharge it!
			else
			{
				m_Effect = (StaffEffect)scroll.SpellID;
				Charges = MaxCharges;
				scroll.Consume();
				from.SendLocalizedMessage( 1010380 ); // The staff is now charged
			}
		}

		public void ConsumeCharge( Mobile from )
		{
			if ( --Charges == 0 )
				from.SendLocalizedMessage( 1019073 ); // This item is out of charges.

			ApplyDelayTo( from );
		}

		public void ApplyDelayTo( Mobile from )
		{
			from.BeginAction( typeof( GnarledStaff ) );
			Timer.DelayCall<Mobile>( GetUseDelay, new TimerStateCallback<Mobile>( ReleaseStaffLock_Callback ), from );
		}

		public void ReleaseStaffLock_Callback( Mobile from )
		{
			from.EndAction( typeof( GnarledStaff ) );
		}

		public override bool AddEquipInfoAttributes( Mobile from, List<EquipInfoAttribute> attrs )
		{
			if ( FactionItemState != null && m_Effect != StaffEffect.None )
				attrs.Add( new EquipInfoAttribute( 3002011 + (int)m_Effect, m_Charges ) );

			return base.AddEquipInfoAttributes( from, attrs );
		}

		public override int LabelNumber{ get{ return ( FactionItemState == null ) ? base.LabelNumber : 1041049; } }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.CanBeginAction( typeof( GnarledStaff ) ) || FactionItemState == null )
				return;

			if ( Parent == from )
			{
				if ( m_Charges > 0 )
				{
					if ( FactionItemState != null )
					{
						PlayerState state = PlayerState.Find( from );

						if ( state.Faction == FactionItemState.Faction )
							OnStaffUse( from );
					}
				}
				else
					from.SendLocalizedMessage( 1019073 ); // This item is out of charges.
			}
			else
				from.SendLocalizedMessage( 502641 ); // You must equip this item to use it.
		}

		public void Cast( Spell spell )
		{
			bool clone = Movable;
			Movable = false;
			spell.Cast();
			Movable = clone;
		}

		public void OnStaffUse( Mobile from )
		{
			switch ( Effect )
			{
				case StaffEffect.Healing:
				{
					Cast( new HealSpell( from, this ) );
					break;
				}
				case StaffEffect.Harm:
				{
					Cast( new HarmSpell( from, this ) );
					break;
				}
				case StaffEffect.Fireball:
				{
					Cast( new FireballSpell( from, this ) );
					break;
				}
				case StaffEffect.GreaterHealing:
				{
					Cast( new GreaterHealSpell( from, this ) );
					break;
				}
				case StaffEffect.Lightning:
				{
					Cast( new LightningSpell( from, this ) );
					break;
				}
			}
		}

		public void DoStaffTarget( Mobile from, object target )
		{
			if ( Deleted || Charges <= 0 || Parent != from || target is StaticTarget || target is LandTarget )
				return;

			if ( OnStaffTarget( from, target ) )
				ConsumeCharge( from );
		}

		public bool OnStaffTarget( Mobile from, object target )
		{
			return true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)1 ); // version
			writer.Write( (int)m_Effect );
			writer.Write( (int)m_Charges );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_Effect = (StaffEffect)reader.ReadInt();
					m_Charges = (int)reader.ReadInt();
					break;
				}
			}
		}

		private class GnarledStaffTarget : Target
		{
			private GnarledStaff m_Staff;

			public GnarledStaffTarget( GnarledStaff staff ) : base ( 6, false, TargetFlags.None )
			{
				m_Staff = staff;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				m_Staff.DoStaffTarget( from, targeted );
			}
		}
	}
}