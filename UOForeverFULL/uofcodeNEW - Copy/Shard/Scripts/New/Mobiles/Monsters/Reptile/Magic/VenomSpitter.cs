using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	[TypeAlias( "Server.Mobiles.Venomspitter" )]
	[CorpseName( "a venomous corpse" )]
	public class VenomSpitter : BaseCreature
	{
		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.DoubleStrike;
		}

		public override string DefaultName{ get{ return "a venom spitter"; } }

		[Constructable]
		public VenomSpitter() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 249;

			SetStr( 425, 450 );
			SetDex( 171, 190 );
			SetInt( 56, 63 );

			SetHits( 1090, 1250 );

			SetDamage( 19, 35 );

			
			
			

			
			
			
			
			

			SetSkill( SkillName.Anatomy, 115.1, 130.0 );
			SetSkill( SkillName.MagicResist, 98.6, 115.5 );
			SetSkill( SkillName.Poisoning, 125.1, 135.0 );
			SetSkill( SkillName.Tactics, 125.1, 140.0 );
			SetSkill( SkillName.Wrestling, 110.6, 120.5 );

			Fame = 17500;
			Karma = -17500;

			if ( 0.25 > Utility.RandomDouble() )
				PackItem( Engines.Plants.Seed.RandomPeculiarSeed( Utility.Random( 4 ) ) );

			PackItem( new Eggs( 2 ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 2 );
			AddLoot( LootPack.Average, 2 );
			AddLoot( LootPack.Gems, 4 );
			AddLoot( LootPack.MedScrolls, 2 );
			AddLoot( LootPack.RichProvisions );
		}

		public override void GenerateLoot( bool spawning )
		{
			base.GenerateLoot( spawning );

			if ( !spawning )
				PackBagofRegs( Utility.RandomMinMax( 35, 45 ) );
		}

		public override bool ReacquireOnMovement{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override Poison HitPoison{ get{ return Utility.RandomBool() ? Poison.Lesser : Poison.Regular; } }
		public override int TreasureMapLevel{ get{ return 3; } }
		public override int Hides{ get{ return 15; } }

		public override void OnDamagedBySpell( Mobile attacker )
		{
			base.OnDamagedBySpell( attacker );

			DoCounter( attacker );
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			DoCounter( attacker );
		}

		public override void AlterMeleeDamageTo( Mobile to, ref int damage )
		{
			if ( to is BaseCreature )
				damage *= 2;
		}

		public override void CheckReflect( Mobile caster, ref bool reflect )
		{
			if ( caster.Player && 0.50 > Utility.RandomDouble() )
				reflect = true;
		}

		private void DoCounter( Mobile attacker )
		{
			if( this.Map == null )
				return;

			if ( attacker is BaseCreature && ((BaseCreature)attacker).BardProvoked )
				return;

			if ( 0.2 > Utility.RandomDouble() )
			{
				/* Counterattack with Hit Poison Area
				 * 20-25 damage, unresistable
				 * Lethal poison, 100% of the time
				 * Particle effect: Type: "2" From: "0x4061A107" To: "0x0" ItemId: "0x36BD" ItemIdName: "explosion" FromLocation: "(296 615, 17)" ToLocation: "(296 615, 17)" Speed: "1" Duration: "10" FixedDirection: "True" Explode: "False" Hue: "0xA6" RenderMode: "0x0" Effect: "0x1F78" ExplodeEffect: "0x1" ExplodeSound: "0x0" Serial: "0x4061A107" Layer: "255" Unknown: "0x0"
				 * Doesn't work on provoked monsters
				 */

				Mobile target = null;

				if ( attacker is BaseCreature )
				{
					Mobile m = ((BaseCreature)attacker).GetMaster();

					if( m != null )
						target = m;
				}

				if ( target == null || !target.InRange( this, 18 ) )
					target = attacker;

				this.Animate( 10, 4, 1, true, false, 0 );

				List<Mobile> targets = new List<Mobile>();

				foreach ( Mobile m in target.GetMobilesInRange( 8 ) )
				{
					if ( m == this || !CanBeHarmful( m ) )
						continue;

					if ( m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != this.Team) )
						targets.Add( m );
					else if ( m.Player && m.Alive )
						targets.Add( m );
				}

				for ( int i = 0; i < targets.Count; ++i )
				{
					Mobile m = targets[i];

					DoHarmful( m );

					//AOS.Damage( m, this, Utility.RandomMinMax( 20, 25 ), true, 0, 0, 0, 100, 0 );

					m.FixedParticles( 0x36BD, 1, 10, 0x1F78, 0xA6, 0, (EffectLayer)255 );
					m.ApplyPoison( this, Poison.Deadly );
				}
			}
		}

		public override int GetAttackSound()
		{
			return 1260;
		}

		public override int GetAngerSound()
		{
			return 1262;
		}

		public override int GetDeathSound()
		{
			return 1259; //Other Death sound is 1258... One for Yamadon, one for Serado?
		}

		public override int GetHurtSound()
		{
			return 1263;
		}

		public override int GetIdleSound()
		{
			return 1261;
		}


		public VenomSpitter( Serial serial ) : base( serial )
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