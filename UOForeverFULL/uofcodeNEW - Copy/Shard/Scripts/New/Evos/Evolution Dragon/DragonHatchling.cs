    /////////////////////////////////////////
   //			                  //
  //      Scripted by Raelis             //
 //Re-edited for 2.0 by Timeinabottle   //
//		             	       //
////////////////////////////////////////
using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a dragon hatchling" )]
	public class DragonHatchling : BaseCreature
	{
		[Constructable]
		public DragonHatchling() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a dragon hatchling";
			Body = 52;
			Hue = Utility.RandomList( 1157, 1175, 1172, 1171, 1170, 1169, 1168, 1167, 1166, 1165 );
			BaseSoundID = 0xDB;

			SetStr( 296, 325 );
			SetDex( 56, 75 );
			SetInt( 76, 96 );

			SetHits( 200, 250 );

			SetDamage( 11, 17 );

			

			

			SetSkill( SkillName.Magery, 50.1, 70.0 );
			SetSkill( SkillName.Meditation, 50.1, 70.0 );
			SetSkill( SkillName.EvalInt, 50.1, 70.0 );
			SetSkill( SkillName.MagicResist, 15.1, 20.0 );
			SetSkill( SkillName.Tactics, 19.3, 34.0 );
			SetSkill( SkillName.Wrestling, 19.3, 34.0 );
			SetSkill( SkillName.Anatomy, 19.3, 34.0 );

			Fame = 300;
			Karma = -300;

			VirtualArmor = 30;

			PackGold( 200, 400 );
			PackMagicItems( 2, 2, 0.95, 0.95 );

			if ( Utility.RandomDouble() <= 0.5 )
			{
				int amount = Utility.RandomMinMax( 1, 5 );

				PackItem( new DragonDust(amount) );
			}
		}

		public DragonHatchling(Serial serial) : base(serial)
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

			this.MovingParticles( m, 0x1FA8, 1, 0, false, true, ( this.Hue - 1 ), 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );
		}

		private class BreatheTimer : Timer
		{
			private DragonHatchling d;
			private Mobile m_Mobile;

			public BreatheTimer( Mobile m, DragonHatchling owner ) : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
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
				m_Mobile.Damage( Utility.RandomMinMax( damagemin, damagemax ));
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
		}
	}
}