//Script Transformed By: Cherokee/Mule II aka. HotShot

using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a mystic daemon hatchling corpse" )]
	public class MysticDaemonHatchling : BaseCreature
	{
		[Constructable]
		public MysticDaemonHatchling() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a mystic daemon hatchling";
			Body = 74;
			Hue = Utility.RandomList( 1157, 1175, 1172, 1171, 1170, 1169, 1168, 1167, 1166, 1165 );
			BaseSoundID = 422;

			SetStr( 496, 525 );
			SetDex( 76, 95 );
			SetInt( 106, 126 );

			SetHits( 500, 750 );

			SetDamage( 12, 18 );

			
			
			
			
			

			
			
			
			
			

			SetSkill( SkillName.Magery, 60.1, 70.0 );
			SetSkill( SkillName.Meditation, 60.1, 70.0 );
			SetSkill( SkillName.EvalInt, 60.1, 70.0 );
			SetSkill( SkillName.MagicResist, 65.1, 70.0 );
			SetSkill( SkillName.Tactics, 39.3, 54.0 );
			SetSkill( SkillName.Wrestling, 39.3, 54.0 );
			SetSkill( SkillName.Anatomy, 39.3, 54.0 );

			Fame = 2500;
			Karma = -2500;

			VirtualArmor = 40;

			PackGold( 500, 800 );
			PackMagicItems( 2, 3, 0.95, 0.95 );
			PackMagicItems( 2, 3, 0.80, 0.65 );

			if ( Utility.RandomDouble() <= 0.15 )
			{
				int amount = Utility.RandomMinMax( 1, 5 );

				PackItem( new DaemonDust(amount) );
			}
		}

		public MysticDaemonHatchling(Serial serial) : base(serial)
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

			this.MovingParticles( m, 0x1FA9, 1, 0, false, true, ( this.Hue - 1 ), 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );
		}

		private class BreatheTimer : Timer
		{
			private MysticDaemonHatchling d;
			private Mobile m_Mobile;

			public BreatheTimer( Mobile m, MysticDaemonHatchling owner ) : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
			{
				d = owner;
				m_Mobile = m;
			}

			protected override void OnTick()
			{
				int damagemin = d.Hits / 20;
				int damagemax = d.Hits / 25;
				d.Frozen = false;

				m_Mobile.PlaySound( 0x11D );
                m_Mobile.Damage(Utility.RandomMinMax(damagemin, damagemax));
				Stop();
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			if ( BaseSoundID == -1 )
				BaseSoundID = 422;
		}
	}
}