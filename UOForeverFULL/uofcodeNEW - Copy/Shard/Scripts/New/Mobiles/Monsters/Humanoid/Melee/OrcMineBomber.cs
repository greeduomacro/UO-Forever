using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "an orcish corpse" )]
	public class OrcMineBomber : BaseCreature
	{
		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Orc; } }
		public override string DefaultName{ get{ return "an orcish dynamite expert"; } }

		[Constructable]
		public OrcMineBomber() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 182;
			Hue = Utility.Random( 2107, 6 );
			BaseSoundID = 0x45A;

			SetStr( 195, 270 );
			SetDex( 91, 115 );
			SetInt( 81, 105 );

			SetHits( 225, 296 );

			SetDamage( 3, 9 );






            Alignment = Alignment.Orc;
			
			

			SetSkill( SkillName.MagicResist, 75.1, 95.0 );
			SetSkill( SkillName.Swords, 75.1, 100.0 );
			SetSkill( SkillName.Tactics, 80.1, 95.0 );
			SetSkill( SkillName.Wrestling, 75.1, 100.0 );

			Fame = 4500;
			Karma = -4500;

			VirtualArmor = 33;

			PackItem( new SulfurousAsh( Utility.RandomMinMax( 10, 18 ) ) );
			PackItem( new MandrakeRoot( Utility.RandomMinMax( 10, 18 ) ) );
			PackItem( new BlackPearl( Utility.RandomMinMax( 8, 14 ) ) );
			PackItem( new MortarPestle() );
			PackItem( new LesserExplosionPotion() );

			if ( 0.2 > Utility.RandomDouble() )
				PackItem( new OrcishKinMask() );

			if ( 0.75 > Utility.RandomDouble() )
				PackItem( new IronOre( 10 ) );
			else
				PackItem( new IronIngot( 5 ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average, 2 );
			AddLoot( LootPack.Meager, 2 );
			if ( Utility.Random( 250 ) == 0 )
				PackItem( new OrcishPickaxe() );
			else
				PackItem( new ExplosionPotion() );

            if (0.02 >= Utility.RandomDouble())
            { SkillScroll scroll = new SkillScroll(); scroll.Randomize(); PackItem(scroll); }
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int DefaultBloodHue{ get{ return -2; } }
		public override int BloodHueTemplate{ get{ return Utility.RandomBlackHue(); } }

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.SavagesAndOrcs; }
		}

		public override bool IsEnemy( Mobile m )
		{
			if ( m.Player && m.FindItemOnLayer( Layer.Helm ) is OrcishKinMask )
				return false;

			return base.IsEnemy( m );
		}

        public override void AggressiveAction(Mobile aggressor, bool criminal)
        {
            base.AggressiveAction(aggressor, criminal);

            Item item = aggressor.FindItemOnLayer(Layer.Helm);

            if (item is OrcishKinMask)
            {
                aggressor.Damage(50);
                item.Delete();
                aggressor.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
                aggressor.PlaySound(0x307);
            }
        }

		private DateTime m_NextBomb;
		private int m_Thrown;

		public override void OnActionCombat()
		{
			Mobile combatant = Combatant;

			if ( combatant == null || combatant.Deleted || combatant.Map != Map || !InRange( combatant, 12 ) || !CanBeHarmful( combatant ) || !InLOS( combatant ) )
				return;

			if ( DateTime.UtcNow >= m_NextBomb )
			{
				ThrowBomb( combatant );

				m_Thrown++;

				if ( 0.75 >= Utility.RandomDouble() && (m_Thrown % 2) == 1 ) // 75% chance to quickly throw another bomb
					m_NextBomb = DateTime.UtcNow + TimeSpan.FromSeconds( 3.0 );
				else
					m_NextBomb = DateTime.UtcNow + TimeSpan.FromSeconds( 5.0 + (10.0 * Utility.RandomDouble()) ); // 5-15 seconds
			}
		}

		public void ThrowBomb( Mobile m )
		{
			DoHarmful( m );

			this.MovingParticles( m, 0x1C19, 1, 0, false, true, 0, 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );

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
				m_Mobile.PlaySound( 0x11D );
				m_Mobile.Damage( Utility.RandomMinMax( 12, 25 ), m_From);
			}
		}

		public OrcMineBomber( Serial serial ) : base( serial )
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