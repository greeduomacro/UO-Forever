//Script Transformed By: Cherokee/Mule II aka. HotShot

using System;
using Server;
using Server.Items; 

namespace Server.Mobiles
{
	[CorpseName( "an ancient daemon corpse" )]
	public class AncientDaemon : BaseCreature
	{
		[Constructable]
		public AncientDaemon () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an ancient daemon";
			Body = 9;
			Hue = Utility.RandomList( 1157, 1175, 1172, 1171, 1170, 1169, 1168, 1167, 1166, 1165 );
			BaseSoundID = 357;

			SetStr( 1196, 1285 );
			SetDex( 90, 185 );
			SetInt( 706, 726 );

			SetHits( 1000, 1250 );

			SetDamage( 29, 35 );

			
			
			
			
			

			
			
			
			
			

			SetSkill( SkillName.Magery, 110.1, 120.0 );
			SetSkill( SkillName.Meditation, 110.1, 120.0 );
			SetSkill( SkillName.EvalInt, 110.1, 120.0 );
			SetSkill( SkillName.MagicResist, 115.1, 120.0 );
			SetSkill( SkillName.Tactics, 109.3, 120.0 );
			SetSkill( SkillName.Wrestling, 109.3, 120.0 );
			SetSkill( SkillName.Anatomy, 109.3, 120.0 );

			Fame = 22500;
			Karma = -22500;

			VirtualArmor = 90;

			PackGold( 3000, 3500 );
			PackMagicItems( 5, 5, 0.95, 0.95 );
			PackMagicItems( 5, 5, 0.80, 0.65 );
			PackMagicItems( 5, 5, 0.80, 0.65 );
			PackMagicItems( 6, 6, 0.80, 0.65 );

			if ( Utility.RandomDouble() <= 0.65 )
			{
				int amount = Utility.RandomMinMax( 1, 5 );

				PackItem( new DaemonDust(amount) );
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

		public AncientDaemon( Serial serial ) : base( serial )
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

			this.MovingParticles( m, 0x1FBE, 1, 0, false, true, ( this.Hue - 1 ), 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );
		}

		private class BreatheTimer : Timer
		{
			private AncientDaemon d;
			private Mobile m_Mobile;

			public BreatheTimer( Mobile m, AncientDaemon owner ) : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
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

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}