using System;
using Server.Mobiles;
using Server.Items;
using Server.Engines.Quests.Collector;

namespace Server.Mobiles
{
	[CorpseName( "a giant rat corpse" )]
	public class LightningRat : BaseCreature
	{
		public override string DefaultName{ get{ return "an electrically charged rat"; } }

		[Constructable]
		public LightningRat() : base( AIType.AI_Melee, FightMode.Weakest, 10, 1, 0.2, 0.4 )
		{
			Body = 0xD7;
			BaseSoundID = 0x188;

			SetStr( 72, 104 );
			SetDex( 76, 95 );

			SetHits( 103, 175 );

			SetDamage( 8, 12 );

			SetSkill( SkillName.MagicResist, 75.1, 90.0 );
			SetSkill( SkillName.Tactics, 79.3, 84.0 );
			SetSkill( SkillName.Wrestling, 89.3, 94.0 );

			Fame = 5500;
			Karma = -5500;

			VirtualArmor = 25;

			PackItem( new BlackPearl( 5 ) );

			if ( Utility.Random( 250 ) == 0 )
				PackItem( new EnchantedWood() );

			if ( 0.005 > Utility.RandomDouble() )
				PackItem( new ObsidianStatue() );
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

			m.FixedParticles( 0x37CC, 10, 25, 9540, 1155, 0, EffectLayer.Waist );

			new InternalTimer( m, this ).Start();
		}

		private class InternalTimer : Timer
		{
			private Mobile m_Mobile, m_From;

			public InternalTimer( Mobile m, Mobile from ) : base( TimeSpan.FromSeconds( 0 ) )
			{
				m_Mobile = m;
				m_From = from;
				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
				m_Mobile.PlaySound( 41 );
                m_Mobile.Damage(Utility.RandomMinMax(1, 20), m_From);
			}
		}

		public override int Meat{ get{ return 3; } }
		public override int Hides{ get{ return 8; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

		public LightningRat(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}
}