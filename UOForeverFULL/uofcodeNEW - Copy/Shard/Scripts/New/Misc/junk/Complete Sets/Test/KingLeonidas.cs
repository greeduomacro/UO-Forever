using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "King Leonidas corpse" )]
	public class KingLeonidas : BaseCreature
	{
		[Constructable]
		public KingLeonidas() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "King Leonidas";
			Body = 400;
			Hue = 1020;

			SetStr( 3020, 4000 );
			SetDex( 800 );
			SetInt( 160, 200 );

			SetHits( 900, 1000 );

			SetDamage( 25, 35 );

			
			

			
			
			
			
			

			SetSkill( SkillName.MagicResist, 200.0 );
			SetSkill( SkillName.Tactics, 200.0 );
			SetSkill( SkillName.Wrestling, 200.0 );
			
			new Horse().Rider = this;

			
			VikingSword weapon = new VikingSword();
			weapon.Hue = 2125;
			weapon.Movable = false;
			AddItem( weapon );

			WoodenKiteShield shield = new WoodenKiteShield();
			shield.Hue = 2125;
			shield.Movable = false;
			AddItem( shield );

			Circlet helm = new Circlet();
			helm.Hue = 2125;
			helm.Movable = false;
			AddItem( helm );

			PlateArms arms = new PlateArms();
			arms.Hue = 2125;
			AddItem( arms );

			PlateGloves gloves = new PlateGloves();
			gloves.Hue = 2125;
			AddItem( gloves );

			ChainChest tunic = new ChainChest();
			tunic.Hue = 2125;
			AddItem( tunic );

			ChainLegs legs = new ChainLegs();
			legs.Hue = 2125;
			AddItem( legs );

			AddItem( new Boots() );

			HairItemID = 0x203C; // Short Hair
			HairHue = 742;



			Fame = 0;
			Karma = 0;

			VirtualArmor = 90;
			PackArmor( 1, 20 );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Gems, Utility.Random( 1, 3 ) );
			// TODO: dungeon chest, healthy gland
		}

		public override bool ReacquireOnMovement{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override Poison HitPoison{ get{ return Utility.RandomBool() ? Poison.Deadly : Poison.Lethal; } } 
		public override int TreasureMapLevel{ get{ return 5; } }
		public override int Hides{ get{ return 20; } }

		private void DoCounter( Mobile attacker )
		{
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

				ArrayList targets = new ArrayList();

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
					Mobile m = (Mobile)targets[i];

					DoHarmful( m );

					m.Damage( Utility.RandomMinMax( 20, 25 ), this);

					m.FixedParticles( 0x36BD, 1, 10, 0x1F78, 0xA6, 0, (EffectLayer)255 );
					m.ApplyPoison( this, Poison.Lethal );
				}
			}
		}


		// TODO: Poison attack

		public override void OnDamagedBySpell( Mobile caster )
		{
			if ( caster != this && 0.15 > Utility.RandomDouble() )
			{
				BaseCreature spawn = new Spartan( this );

				spawn.Team = this.Team;
				spawn.MoveToWorld( this.Location, this.Map );
				spawn.Combatant = caster;
				this.Say( true, "Show them no mercy!!" );

				DoCounter( caster );
			}

			base.OnDamagedBySpell( caster );
		}

		public override bool AutoDispel{ get{ return true; } }

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			if ( attacker != this && 0.15 > Utility.RandomDouble() )
			{
				BaseCreature spawn = new Spartan( this );

				spawn.Team = this.Team;
				spawn.MoveToWorld( this.Location, this.Map );
				spawn.Combatant = attacker;
				DoCounter( attacker );

				this.Say( true, "You have wounded me! Guards!" );
			}

			base.OnGotMeleeAttack( attacker );
		}

		public KingLeonidas( Serial serial ) : base( serial )
		{
		}

		public override int GetIdleSound()
		{
			return 0x1BF;
		}

		public override int GetAttackSound()
		{
			return 0x1C0;
		}

		public override int GetHurtSound()
		{
			return 0x1C1;
		}

		public override int GetDeathSound()
		{
			return 0x1C2;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}