using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a blood vortex corpse" )]
	public class BloodVortex : BaseCreature
	{
		public override string DefaultName{ get{ return "a blood vortex"; } }

		[Constructable]
		public BloodVortex() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 790;
			Hue = 1157;
			BaseSoundID = 263;

			SetStr( 156, 190 );
			SetDex( 171, 195 );
			SetInt( 106, 130 );

			SetHits( 1051, 1062 );

			SetDamage( 23, 26 );

			
			

			
			
			
			
			

			SetSkill( SkillName.MagicResist, 150.0 );
			SetSkill( SkillName.Tactics, 90.0 );
			SetSkill( SkillName.Wrestling, 100.0 );
		    SetSkill( SkillName.Poisoning, 90.0 );
			//SetSkill( SkillName.EvalInt, 100.0 );
			//SetSkill( SkillName.MagicResist, 100.0, 110.0 );
			SetSkill( SkillName.Magery, 120.0, 130.0 );
			SetSkill( SkillName.EvalInt, 100.0, 110.0 );
			SetSkill( SkillName.Meditation, 100.0, 110.0 );

			Fame = 14500;
			Karma = -14500;

			VirtualArmor = 68;
			PackItem( new Bone() );
		}
		public override bool AutoDispel{ get{ return true; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 7 );
		}
	public override void GenerateLoot( bool spawning )
		{
			base.GenerateLoot( spawning );

			if ( !spawning )
			{
				double rand = Utility.RandomDouble();
				if ( 0.025 > rand )
					PackItem( new Sandals(1157) );
				else if ( 0.125 > rand )
					PackItem( new StatueEast(1157) );
			}
		}
		private DateTime m_NextAttack;

		public override void OnActionCombat()
		{
			Mobile combatant = Combatant;

			if ( combatant == null || combatant.Deleted || combatant.Map != Map || !InRange( combatant, 12 ) || !CanBeHarmful( combatant ) || !InLOS( combatant ) )
				return;

			if ( DateTime.UtcNow >= m_NextAttack )
			{
				BloodAttack( combatant );
				m_NextAttack = DateTime.UtcNow + TimeSpan.FromSeconds( 10.0 + (10.0 * Utility.RandomDouble()) );
			}
		}

		public void BloodAttack( Mobile m )
		{
			DoHarmful( m );

			m.FixedParticles( 0x36B0, 10, 25, 9540, 2413, 0, EffectLayer.Waist );

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
                m_Mobile.Damage(Utility.RandomMinMax(1, 40), m_From);
			}
		}

		public BloodVortex( Serial serial ) : base( serial )
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
}