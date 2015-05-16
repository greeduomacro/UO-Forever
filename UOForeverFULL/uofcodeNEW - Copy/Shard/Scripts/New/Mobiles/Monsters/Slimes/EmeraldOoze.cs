using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Engines.Quests.Collector;

namespace Server.Mobiles
{
	[CorpseName( "an emerald ooze corpse" )]
	public class EmeraldOoze : BaseCreature
	{
		public override string DefaultName{ get{ return "an emerald ooze"; } }

		[Constructable]
		public EmeraldOoze() : base( AIType.AI_Mage, FightMode.Weakest, 10, 1, 0.06, 0.06 )
		{
			Body = 51;
			BaseSoundID = 456;
			Hue = Utility.Random( 1267, 6 ) + ( Utility.RandomBool() ? 100 : 0 );

			SetStr( 95, 105 );
			SetDex( 90, 100 );
			SetInt( 190, 200 );

			SetHits( 105, 175 );

			SetDamage( 5, 10 );

			SetSkill( SkillName.MagicResist, 88.1, 95.0 );
			SetSkill( SkillName.Magery, 94.3, 96.5 );
			SetSkill( SkillName.EvalInt, 56.3, 75.0 );
			SetSkill( SkillName.Meditation, 56.3, 75.0 );
			SetSkill( SkillName.Wrestling, 68.4, 81.7);
			SetSkill( SkillName.Tactics, 125.7, 130.4);

			Fame = 8500;
			Karma = -8500;

			VirtualArmor = 55;

			if ( Utility.Random( 250 ) == 0 )
				PackItem( new EnchantedWood() );

			PackGold( 150, 200 );

			if ( 0.005 > Utility.RandomDouble() )
				PackItem( new ObsidianStatue() );
		}

		public override void GenerateLoot()
		{
			if ( Utility.RandomBool() )
				AddLoot( LootPack.Average, 2 );
			else
				AddLoot( LootPack.Rich );
		}

		private DateTime m_NextAttack;

		public override void OnActionCombat()
		{
			Mobile combatant = Combatant;

			if ( combatant == null || combatant.Deleted || combatant.Map != Map || !InRange( combatant, 15 ) || !CanBeHarmful( combatant ) || !InLOS( combatant ) )
				return;

			if ( DateTime.UtcNow >= m_NextAttack )
			{
				SandAttack( combatant );
				m_NextAttack = DateTime.UtcNow + TimeSpan.FromSeconds( 5.0 + (5.0 * Utility.RandomDouble()) );
			}
		}

		public void SandAttack( Mobile m )
		{
			DoHarmful( m );

			m.FixedParticles( 0x37C4, 10, 25, 9540, 1271, 0, EffectLayer.Waist );

			new InternalTimer( m, this ).Start();
		}

		private class InternalTimer : Timer
		{
			private Mobile m_Mobile, m_From;

			public InternalTimer( Mobile m, Mobile from ) : base( TimeSpan.FromSeconds( 0 ) )
			{
				m_Mobile = m;
				m_From = from;
			}

			protected override void OnTick()
			{
				m_Mobile.PlaySound( 288 );
                m_Mobile.Damage(Utility.RandomMinMax(1, 15), m_From);
			}
		}

		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override Poison HitPoison{ get{ return Poison.Greater; } }
		public override double HitPoisonChance{ get{ return 0.75; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override int BloodHueTemplate{ get{ if ( Hue == 0 ) return 1072; else return Hue; } }
		public override int DefaultBloodHue{ get{ return -2; } }

		public override void AlterMeleeDamageTo( Mobile to, ref int damage )
		{
			if ( to is BaseCreature )
				damage *= 10;
		}


		public EmeraldOoze( Serial serial ) : base( serial )
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