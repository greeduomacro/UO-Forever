using System;
using System.Collections.Generic;
using Server.Items;
using Server.Engines.Quests.Collector;

namespace Server.Mobiles
{
	[CorpseName( "an earth summoner corpse" )]
	public class EarthSummoner : BaseCreature
	{
		public override bool ShowFameTitle{ get{ return false; } }

		[Constructable]
		public EarthSummoner() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.15, 0.2 )
		{
			Title = "the Earth Summoner";

			Hue = Utility.RandomSkinHue();
			Body = 0x190;
			Name = NameList.RandomName( "male" );
			BaseSoundID = 0;

			Item StrawHat = new StrawHat();
			StrawHat.Movable = false;
			StrawHat.Hue = 1021;
			EquipItem( StrawHat );

			Item Robe = new Robe();
			Robe.Movable = false;
			Robe.Hue = 1021;
			EquipItem( Robe );

			Item Sandals = new Sandals();
			Sandals.Movable = false;
			Sandals.Hue = 1021;
			EquipItem( Sandals );

			SetStr( 200, 220 );
			SetDex( 136, 145 );

			SetDamage( 10, 17 );

			SetSkill( SkillName.Wrestling, 95.3, 98.8 );
			SetSkill( SkillName.Tactics, 93.5, 97.0 );
			SetSkill( SkillName.MagicResist, 96.6, 99.8);

			Fame = 12500;
			Karma = -12500;

			VirtualArmor = 50;

			PackGold( 500, 800 );
			PackPotion();
			PackItem( new Bandage( Utility.RandomMinMax( 5, 10 ) ) );
			PackArmor( 0, 5 );
			PackWeapon( 0, 5 );
			PackSlayer();

			if ( 0.005 > Utility.RandomDouble() )
				PackItem( new ObsidianStatue() );
		}

		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool CanRummageCorpses{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lesser; } }
		public override int TreasureMapLevel{ get{ return 2; } }

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			if ( 0.3 >= Utility.RandomDouble() ) // 30% chance to drop or throw a EarthSummoner Gate
				AddEarthGate( defender, 0.25 );
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			if ( 0.1 >= Utility.RandomDouble() ) // 10% chance to drop or throw a EarthSummoner Gate
				AddEarthGate( attacker, 0.25 );
		}

		public override void AlterDamageScalarFrom( Mobile caster, ref double scalar )
		{
			base.AlterDamageScalarFrom( caster, ref scalar );

			if ( 0.1 >= Utility.RandomDouble() ) // 10% chance to throw a EarthSummoner Gate
				AddEarthGate( caster, 1.0 );
		}

		private List<EarthElemental> m_Summons = new List<EarthElemental>();
		public List<EarthElemental> Summons{ get{ return m_Summons; } }

		public void AddEarthGate( Mobile target, double chanceToThrow )
		{
			if ( chanceToThrow >= Utility.RandomDouble() )
			{
				Direction = GetDirectionTo( target );
				MovingEffect( target, 0xF7E, 10, 1, true, false, 0x496, 0 );
				new DelayTimer( this, target ).Start();
			}
			else
				new EarthGate( this ).MoveToWorld( Location, Map );
		}

		public override void OnDelete()
		{
			for ( int i = m_Summons.Count-1;i >= 0; i-- )
				m_Summons[i].Delete();

			base.OnDelete();
		}

		private class DelayTimer : Timer
		{
			private EarthSummoner m_Mobile;
			private Mobile m_Target;

			public DelayTimer( EarthSummoner m, Mobile target ) : base( TimeSpan.FromSeconds( 1.0 ) )
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
					new EarthGate( m_Mobile ).MoveToWorld( m_Target.Location, m_Target.Map );
				}
			}
		}

		public EarthSummoner( Serial serial ) : base( serial )
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