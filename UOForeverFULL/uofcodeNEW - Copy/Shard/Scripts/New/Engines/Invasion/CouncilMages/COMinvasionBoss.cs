using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Commands;

namespace Server.Engines.Invasion
{
	[CorpseName( "the corpse of a creature from the void" )]
	public class COMInvasionBoss : BaseCreature
	{
		private Timer m_Timer;

		[Constructable]
		public COMInvasionBoss() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "shadow knight" );
			Title = "Lord of the Undead";
			Body = 308;
			Hue = 2403;
			BaseSoundID = 0x48D;

			SetStr( 3500 );
			SetDex( 151, 175 );
			SetInt( 471, 520 );

			SetHits( 48000 );

			SetDamage( 34, 36 );

			
			

			
			
			
			
			

			SetSkill( SkillName.DetectHidden, 100.0 );
			SetSkill( SkillName.EvalInt, 125.0 );
			SetSkill( SkillName.Magery, 200.0 );
			SetSkill( SkillName.Meditation, 125.0 );
			SetSkill( SkillName.MagicResist, 135.0 );
			SetSkill( SkillName.Tactics, 125.0 );
			SetSkill( SkillName.Wrestling, 125.0 );

			Fame = 20000;
			Karma = -20000;

			VirtualArmor = 68;

			m_Timer = new RepeatTimer( this );
		}

		private class RepeatTimer : Timer
		{
			private COMInvasionBoss m_Boss;

			public RepeatTimer( COMInvasionBoss boss ) : base( TimeSpan.Zero, TimeSpan.FromMinutes( 5.0 ) )
			{
				m_Boss = boss;

				Priority = TimerPriority.FiveSeconds;
			}

			protected override void OnTick()
			{
				if ( m_Boss != null && !m_Boss.Deleted )
					CommandHandlers.BroadcastMessage( AccessLevel.Player, 38, "You will all perish before the Lord of the Undead!  I will burn Skara Brae to the ground!" );
				else
					Stop();
			}
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 1 );
			AddLoot( LootPack.UltraRich, 2 );
			AddLoot( LootPack.SuperBoss, 2 );
		}

		public override bool OnBeforeDeath()
		{
			if ( !NoKillAwards )
			{
				GivePrizes();
				PackItem( new SpellbookBlessDeed() );
				PackItem( new SpellbookBlessDeed() );
				PackItem( new NameChangeDeed() );
				PackItem( new NameChangeDeed() );
				PackItem( new CandleSkull() );
				PackItem( new CandelabraStand() );
				PackItem( new DecoMandrakeRoot() );
				PackItem( new DecoGinsengRoot2() );
			}
			
			if ( m_Timer != null )
				m_Timer.Stop();

			return base.OnBeforeDeath();
		}

		public void GivePrizes()
		{
			List<DamageStore> rights = BaseCreature.GetLootingRights( this.DamageEntries, this.HitsMax );
			List<Mobile> toGive = new List<Mobile>();

			for ( int i = rights.Count - 1; i >= 0; --i )
			{
				DamageStore ds = rights[i];

				if ( ds.m_Damage >= 300 )
					toGive.Add( ds.m_Mobile );
			}

			for ( int i = 0; i < toGive.Count; i++ )
				GivePrize( toGive[i] );
		}

		public void GivePrize( Mobile from )
		{
			from.SendMessage( "You grab fragements from the realm of the undead and place them in your backpack." );
			from.PlaceInBackpack( new AltRealmFragment() );
		}

		public override bool Unprovokable { get { return true; } }
		public override bool AreaPeaceImmune { get { return true; } }
		public override bool Uncalmable{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int TreasureMapLevel{ get{ return 5; } }
		public override bool BleedImmune{ get{ return true; } }
		public override double BonusPetDamageScalar{ get{ return 0.5; } }
		public override double DispelDifficulty{ get{ return 125.0; } }
		public override double DispelFocus{ get{ return 45.0; } }
		public override Poison HitPoison{ get{ return Poison.Lethal; } }
		public override double HitPoisonChance{ get{ return 0.01; } }

		public override bool HasAura { get { return true; } }
		public override double AuraMinInterval { get { return 10.0; } }
		public override double AuraMaxInterval { get { return 20.0; } }
		public override int AuraRange { get { return 3; } }

		public override int AuraMinDamage { get { return 1; } }
		public override int AuraMaxDamage { get { return 2; } }

		public override void CheckReflect( Mobile caster, ref bool reflect )
		{
			if ( 0.10 > Utility.RandomDouble() )
				reflect = true; // Every spell is reflected back to the caster
		}

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			if ( 0.10 > Utility.RandomDouble() )
				FlameStrike();
		}

		public void FlameStrike()
		{
			Map map = this.Map;

			if ( map == null )
				return;

			List<Mobile> targets = new List<Mobile>();

			foreach ( Mobile m in this.GetMobilesInRange( 10 ) )
			{
				if ( m == this || !CanBeHarmful( m ) )
					continue;

				if ( m.Player )
					targets.Add( m ) ;
			}

			if ( targets.Count > 0 )
			{
				Mobile targ = targets[Utility.Random( targets.Count )];

				targ.PlaySound( 0x208 );
				targ.FixedParticles( 0x3709, 10, 30, 5052, EffectLayer.LeftFoot );

				double damage = targ.HitsMax * 0.3;

				if ( damage < 10.0 )
					damage = 10.0;
				else if ( damage > 30.0 )
					damage = 30.0;

				DoHarmful( targ );

				targ.Damage( (int)damage, this);

				if ( targ.Alive && targ.Body.IsHuman && !targ.Mounted )
					targ.Animate( 20, 7, 1, true, false, 0 ); // take hit
			}
		}

		public COMInvasionBoss( Serial serial ) : base( serial )
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