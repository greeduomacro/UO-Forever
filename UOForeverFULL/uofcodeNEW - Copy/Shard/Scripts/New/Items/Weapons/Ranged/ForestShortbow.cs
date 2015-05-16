////////////////////
//Created by Darky//
////////////////////

using System;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;

namespace Server.Items
{
	[FlipableAttribute( 0x2D1F, 0x2D2B )]
	public class ForestShortBow : BaseRanged
	{
		private BaseCreature m_Summon;

		public override int EffectID{ get{ return 0xF42; } }
		public override Type AmmoType{ get{ return typeof( Arrow ); } }
		public override Item Ammo{ get{ return new Arrow(); } }

		public override int OldStrengthReq{ get{ return 50; } }
		public override int OldDexterityReq{ get{ return 75; } }
		public override int NewMinDamage{ get{ return 30; } }
		public override int NewMaxDamage{ get{ return 45; } }
		//public override int DiceDamage { get { return Utility.Dice( 5, 4, 25 ); } } // 5d4 + 25 (30-45)
		public override int OldSpeed{ get{ return 35; } }

		public override int DefMaxRange{ get{ return 9; } }

		public override int InitMinHits{ get{ return 125; } }
		public override int InitMaxHits{ get{ return 150; } }

		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.ShootBow; } }

		public override string DefaultName{ get{ return "a bow of the forest"; } }

		[Constructable]
		public ForestShortBow() : base( 0x2D1F )
		{
			Weight = 10.0;
			Layer = Layer.TwoHanded;
		}

		public ForestShortBow( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );
			LabelTo( from, "10% chance to summon creature" );
		}

		public override void OnRemoved( object parent )
		{
			base.OnRemoved( parent );

			if ( parent is Mobile )
			{
				Mobile m = (Mobile)parent;
				m.RemoveStatMod( Serial.ToString() + "Forest-Dex" );
				if ( m_Summon != null && !m_Summon.Deleted )
				{
					m_Summon.FixedParticles( 0x3728, 1, 10, 9910, EffectLayer.Head );
					m.SendMessage( "The summoned creature fades as you release the bow from your hands." );
					m_Summon.Delete();
				}
			}
		}

		public override bool OnEquip( Mobile from )
		{
			from.AddStatMod( new StatMod( StatType.Dex, Serial.ToString() + "Forest-Dex", 5, TimeSpan.Zero ) );
			return base.OnEquip( from );
		}

		public override void OnHit( Mobile attacker, Mobile defender, double damageBonus )
		{
/*
			if ( defender is PlayerMobile )
			{
				AOS.Damage( attacker, 50, 0, 100, 0, 0, 0 );
				Delete();
				attacker.FixedParticles( 0x36BD, 20, 10, 5044, EffectLayer.Head );
				attacker.PlaySound( 0x307 );
			}
*/

			if ( defender != null && !defender.Player && attacker.Alive && ( m_Summon == null || !m_Summon.Alive ) )
			{
				if ( damageBonus <= 1.0 && 0.1 > Utility.RandomDouble() )
				{
						switch ( Utility.Random( 7 ) )
						{
							case 0: m_Summon = new GrizzlyBear(); break;
							case 1: m_Summon = new BlackBear(); break;
							case 2: m_Summon = new BrownBear(); break;
							case 3: m_Summon = new Cougar(); break;
							case 4: m_Summon = new DireWolf(); break;
							case 5: m_Summon = new Panther(); break;
							case 6: m_Summon = new TimberWolf(); break;
						}

						m_Summon.ControlSlots = 1;
						SpellHelper.Summon( m_Summon, attacker, 0x215, TimeSpan.FromMinutes( 2.0 ), false, false );
						m_Summon.Combatant = defender;
						attacker.FixedParticles( 0x3709, 10, 30, 5052, EffectLayer.LeftFoot );
						attacker.BoltEffect( 0 );
						attacker.PlaySound( 1481 );
						attacker.SendMessage( "A creature has been summoned to your aid!" );
				}
			}

			base.OnHit( attacker, defender, damageBonus );
		}
	}
}