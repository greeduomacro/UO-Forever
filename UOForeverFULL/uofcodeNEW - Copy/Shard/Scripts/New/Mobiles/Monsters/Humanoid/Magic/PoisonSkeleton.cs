using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a poisonous skeletal corpse" )]
	public class PoisonSkeleton : BaseCreature
	{
		public override string DefaultName{ get{ return "a poisonous skeleton"; } }

		[Constructable]
		public PoisonSkeleton() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 3.0, 5.0 )
		{
			Body = 50;
			BaseSoundID = 0x48D;
			Hue = 1164;

			SetStr( 500, 600 );
			SetDex( 200, 250 );
			SetInt( 10, 10 );

			SetHits( 575, 675 );

			SetDamage( 25, 38 );





            Alignment = Alignment.Undead;
			
			

			SetSkill( SkillName.MagicResist, 85.1, 90.3 );
			SetSkill( SkillName.Tactics, 100.0, 100.0 );
			SetSkill( SkillName.Wrestling, 100.0, 125.0 );
			SetSkill( SkillName.Anatomy, 100.0, 100.0 );

			Fame = 5000;
			Karma = -5000;

			VirtualArmor = 35;

			if ( Utility.Random( 300 ) == 0 )
				PackItem( new RareSwampTile() );

			PackMagicItems( 1, 5, 0.80, 0.75 );
			PackMagicItems( 3, 5, 0.60, 0.45 );
		}

		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
	//	public override bool BardImmune{ get{ return true; } }
		public override bool AlwaysMurderer{ get{ return true; } }

		public override Poison HitPoison{ get{ return Poison.Deadly; } }
		public override double HitPoisonChance{ get{ return 0.75; } }

		private DateTime m_NextAttack;

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			Mobile combatant = Combatant;

			if ( combatant == null || combatant.Deleted || combatant.Map != Map || !InRange( combatant, 18 ) || !CanBeHarmful( combatant ) || !InLOS( combatant ) )
				return;

			if ( DateTime.UtcNow >= m_NextAttack )
			{
				SandAttack( combatant );
				m_NextAttack = DateTime.UtcNow + TimeSpan.FromSeconds( 10.0 + (10.0 * Utility.RandomDouble()) );
			}
		}

		public void SandAttack( Mobile m )
		{
			DoHarmful( m );

			m.FixedParticles( 0x36B0, 10, 25, 9540, 567, 0, EffectLayer.Waist );

			new InternalTimer( m, this ).Start();
		}

		private class InternalTimer : Timer
		{
			private Mobile m_Mobile, m_From;

			public InternalTimer( Mobile m, Mobile from ) : base( TimeSpan.FromSeconds( 1.0 ) )
			{
				m_Mobile = m;
				m_From = from;
				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
				m_Mobile.PlaySound( 0x4CF );
			    m_Mobile.Damage( Utility.RandomMinMax( 1, 10 ), m_From);
			}
		}

		public PoisonSkeleton( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}
}