using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Engines.CannedEvil;

namespace Server.Mobiles
{
	public class Serado : BaseChampion
	{
		public override ChampionSkullType SkullType{ get{ return ChampionSkullType.Power; } }

		[Constructable]
		public Serado() : base( AIType.AI_Melee )
		{
			Name = "Serado";
			Title = "the awakened";

			Body = 249;
			Hue = 0x96C;

			SetStr( 1000, 1250 );
			SetDex( 100, 175 );
			SetInt( 300, 450 );

			SetHits( 9000, 12750 );
			SetMana( 300 );

			SetDamage( 33, 50 );
            SetStam(80, 100);

			SetSkill( SkillName.MagicResist, 120.0 );
			SetSkill( SkillName.Tactics, 120.0 );
			SetSkill( SkillName.Wrestling, 100.0 );
			SetSkill( SkillName.Poisoning, 150.0 );

			Fame = 22500;
			Karma = -22500;

            VirtualArmor = 30;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 4 );
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Gems, 6 );
		}

		public override int TreasureMapLevel{ get{ return 5; } }

		public override Poison HitPoison{ get{ return Poison.Lethal; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override double HitPoisonChance{	get{ return 0.8; } }

		public override int Feathers{ get{ return 30; } }

		public override bool ShowFameTitle{ get{ return false; } }
		public override bool ClickTitle{ get{ return false; } }

		// TODO: Hit Lightning Area

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

		private void DoCounter( Mobile attacker )
		{
			if ( this.Map == null || ( attacker is BaseCreature && ((BaseCreature)attacker).BardProvoked ) )
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

					if ( m != null )
						target = m;
				}

				if ( target == null || !target.InRange( this, 25 ) )
					target = attacker;

				this.Animate( 10, 4, 1, true, false, 0 );

				ArrayList targets = new ArrayList();

				foreach ( Mobile m in target.GetMobilesInRange( 8 ) )
				{
					if ( m == this || !CanBeHarmful( m ) )
						continue;

					if ( m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != this.Team) )
						targets.Add( m );
					else if ( m.Player )
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

		public Serado( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}