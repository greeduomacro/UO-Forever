// Created by: Wolfen & FullMetalNecro
// Rewriten by: Sexy-vampire
using System;
using Server;
using Server.Misc;
using System.Collections;
using Server.Network;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a guardian wolf corpse" )]
	public class GuardianWolf : BaseCreature
	{
		public Timer m_DeathTimer;

		[Constructable]
		public GuardianWolf () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a guardian Wolf";
			Body = 277;
			BaseSoundID =  0xE5;
			Hue = Utility.RandomList(  1157, 1175, 1172, 1170, 2703, 2473, 2643, 1156, 2704, 2734, 2669, 2621, 2859, 2716, 2791, 2927, 2974, 1161, 2717, 2652, 2821, 2818, 2730, 2670, 2678, 2630, 2641, 2644, 2592, 2543, 2526, 2338, 868, 689, 1579, 1393, 1292, 2339, 1793, 1980, 1983);

			SetStr( 1000, 1500 );
			SetDex( 200, 300 );
			SetInt( 906, 1026 );

			SetHits( 15000, 20500 );

			SetDamage( 34, 40 );

			
			
			
			
			

			
			
			
			
			

			SetSkill( SkillName.EvalInt, 125.1, 130.0 );
			SetSkill( SkillName.Magery, 125.1, 130.0 );
			SetSkill( SkillName.Meditation, 125.1, 130.0 );
			SetSkill( SkillName.MagicResist, 140.5, 170.0 );
			SetSkill( SkillName.Tactics, 125.1, 130.0 );
			SetSkill( SkillName.Wrestling, 125.1, 130.0 );

			Fame = 50000;
			Karma = -50000;

			VirtualArmor = 100;

			PackGem();
			PackGem();
			PackPotion();
			PackGold( 30000, 40000 );
			PackScroll( 2, 8 );
			PackMagicItems( 3, 5, 0.95, 0.95 );
			PackMagicItems( 4, 5, 0.80, 0.65 );
			PackMagicItems( 4, 5, 0.80, 0.65 );
			PackSlayer();

			if ( Utility.RandomDouble() <= 0.30 )
			{
				PackItem( new  NytesWolfEgg() );
			}
		}

		public override int GetIdleSound()
		{
			return 0x2D3;
		}

		public override int GetHurtSound()
		{
			return 0x2D1;
		}

		public override bool AutoDispel{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

		public override WeaponAbility GetWeaponAbility()
		{
			switch ( Utility.Random( 3 ) )
			{
				default:
				case 1: return WeaponAbility.BleedAttack;
				case 2: return WeaponAbility.MortalStrike; 
				case 3: return WeaponAbility.CrushingBlow;
			}
		}

		public GuardianWolf( Serial serial ) : base( serial )
		{
		}

		private DateTime m_NextBreathe;

		public override void OnActionCombat()
		{
			Mobile combatant = Combatant;

			if ( combatant == null || combatant.Deleted || combatant.Map != Map || !InRange( combatant, 12 ) || !CanBeHarmful( combatant ) || !InLOS( combatant ) )
				return;

			if ( DateTime.UtcNow >= m_NextBreathe )
			{
				Breathe( combatant );

				m_NextBreathe = DateTime.UtcNow + TimeSpan.FromSeconds( 12.0 + (3.0 * Utility.RandomDouble()) ); // 12-15 seconds
			}
		}

		public void Breathe( Mobile m )
		{
			DoHarmful( m );

			new BreatheTimer( m, this ).Start();

			this.Frozen = true;

			this.MovingParticles( m, 0x1FBE, 1, 0, false, true, Utility.RandomList( 1157, 1175, 1172, 1171, 1170, 1169, 1168, 1167, 1166, 1165 ), 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );
		}

		private class BreatheTimer : Timer
		{
			private GuardianWolf d;
			private Mobile m_Mobile;

			public BreatheTimer( Mobile m, GuardianWolf owner ) : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
			{
				d = owner;
				m_Mobile = m;
			}

			protected override void OnTick()
			{
				d.Frozen = false;

				m_Mobile.PlaySound( 0x11D );
				m_Mobile.Damage( Utility.RandomMinMax( 250, 300 ));
				Stop();
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}