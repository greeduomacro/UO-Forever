using System;
using System.Collections.Generic;
using Server.Items;
using Server.Engines.Quests.Collector;

namespace Server.Mobiles
{
	[CorpseName( "a wind summoner corpse" )]
	public class WindSummoner : BaseCreature
	{
		public override bool ShowFameTitle{ get{ return false; } }

		[Constructable]
		public WindSummoner() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.15, 0.2 )
		{
			Title = "the Wind Summoner";

			Hue = Utility.RandomSkinHue();
			Body = 0x190;
			Name = NameList.RandomName( "male" );
			BaseSoundID = 0;

			Item hat = new WizardsHat();
			hat.Movable = false;
			hat.Hue = 1154;
			EquipItem( hat );

			Item shirt = new Shirt();
			shirt.Movable = false;
			shirt.Hue = 1154;
			EquipItem( shirt );

			Item skirt = new Skirt();
			skirt.Movable = false;
			skirt.Hue = 1154;
			EquipItem( skirt );

			Item Sandals = new Sandals();
			Sandals.Movable = false;
			Sandals.Hue = 1154;
			EquipItem( Sandals );

			SetStr( 110, 120 );
			SetDex( 86, 95 );
			SetInt( 161, 170 );

			SetHits( 120, 130 );

			SetDamage( 5, 13 );

			SetSkill( SkillName.Wrestling, 70.3, 77.8 );
			SetSkill( SkillName.Tactics, 80.5, 87.0 );
			SetSkill( SkillName.MagicResist, 90.6, 92.8);
			SetSkill( SkillName.Magery, 94.7, 96.0 );
			SetSkill( SkillName.EvalInt, 40.1, 44.1 );
			SetSkill( SkillName.Meditation, 21.1, 30.1 );

			Fame = 9000;
			Karma = -9000;

			VirtualArmor = 45;

			PackPotion();
			PackItem( new Bandage( Utility.RandomMinMax( 5, 10 ) ) );

			if ( 0.005 > Utility.RandomDouble() )
				PackItem( new ObsidianStatue() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
		}

		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool CanRummageCorpses{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lesser; } }
		public override int TreasureMapLevel{ get{ return 2; } }

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			if ( 0.3 >= Utility.RandomDouble() ) // 30% chance to drop or throw a WindSummoner Gate
				AddWindGate( defender, 0.25 );
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			if ( 0.1 >= Utility.RandomDouble() ) // 10% chance to drop or throw a WindSummoner Gate
				AddWindGate( attacker, 0.25 );
		}

		public override void AlterDamageScalarFrom( Mobile caster, ref double scalar )
		{
			base.AlterDamageScalarFrom( caster, ref scalar );

			if ( 0.1 >= Utility.RandomDouble() ) // 10% chance to throw a WindSummoner Gate
				AddWindGate( caster, 1.0 );
		}

		private List<AirElemental> m_Summons = new List<AirElemental>();
		public List<AirElemental> Summons{ get{ return m_Summons; } }

		public void AddWindGate( Mobile target, double chanceToThrow )
		{
			if ( chanceToThrow >= Utility.RandomDouble() )
			{
				Direction = GetDirectionTo( target );
				MovingEffect( target, 0xF7E, 10, 1, true, false, 0x496, 0 );
				new DelayTimer( this, target ).Start();
			}
			else
				new WindGate( this ).MoveToWorld( Location, Map );
		}

		public override void OnDelete()
		{
			for ( int i = m_Summons.Count-1;i >= 0; i-- )
				m_Summons[i].Delete();

			base.OnDelete();
		}

		private class DelayTimer : Timer
		{
			private WindSummoner m_Mobile;
			private Mobile m_Target;

			public DelayTimer( WindSummoner m, Mobile target ) : base( TimeSpan.FromSeconds( 1.0 ) )
			{
				m_Mobile = m;
				m_Target = target;
			}

			protected override void OnTick()
			{
				if ( m_Mobile.CanBeHarmful( m_Target ) )
				{
					m_Mobile.DoHarmful( m_Target );
                    m_Target.Damage(Utility.RandomMinMax(10, 20), m_Mobile);
					new WindGate( m_Mobile ).MoveToWorld( m_Target.Location, m_Target.Map );
				}
			}
		}

		public WindSummoner( Serial serial ) : base( serial )
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
	}
}