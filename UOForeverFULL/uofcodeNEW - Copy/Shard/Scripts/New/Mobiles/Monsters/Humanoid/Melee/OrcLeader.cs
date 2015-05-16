using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "an orcish corpse" )]
	public class OrcLeader : BaseCreature
	{
		public override string DefaultName{ get{ return "an orcish leader"; } }

		[Constructable]
		public OrcLeader() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 189;
			BaseSoundID = 0x45A;

			SetStr( 1500, 1681 );
			SetDex( 66, 400 );
			SetInt( 425, 500 );

			Hue = 1123;

			SetHits( 1700, 1860 );

			SetDamage( 14, 25 );



            Alignment = Alignment.Orc;
			
			
			
			

			SetSkill( SkillName.Macing, 115.1, 125.0 );
			SetSkill( SkillName.MagicResist, 125.1, 140.0 );
			SetSkill( SkillName.Tactics, 115.1, 125.0 );
			SetSkill( SkillName.Wrestling, 115.1, 125.0 );

			Fame = 15000;
			Karma = -15000;

			VirtualArmor = 40;

			PackItem( new ShadowIronOre( 25 ) );
			PackItem( new IronIngot( 10 ) );

			if ( 0.05 > Utility.RandomDouble() )
				PackItem( new OrcishKinMask() );

			if ( 0.2 > Utility.RandomDouble() )
                PackItem(new OrcishKinMask());
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Rich, 3 );

			if ( Utility.Random( 250 ) == 0 ) //If this becomes blessed, move to OnAfterDeath
				PackItem( new DecoOreMiningCartDeed() );
		}

//		public override bool BardImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int Meat{ get{ return 2; } }

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

		public override bool CanRummageCorpses{ get{ return true; } }
		public override bool AutoDispel{ get{ return true; } }
		public override int DefaultBloodHue{ get{ return -2; } }
		public override int BloodHueTemplate{ get{ return Utility.RandomBlackHue(); } }

		public override void OnDamagedBySpell( Mobile caster )
		{
			if ( caster == this || Region != null && Region.Name != "Orc Cave")
				return;

			SpawnOrcLord( caster );
		}

		public void SpawnOrcLord( Mobile target )
		{
			Map map = target.Map;

			if ( map == null )
				return;

			int orcs = 0;

			foreach ( Mobile m in this.GetMobilesInRange( 10 ) )
			{
				if ( m is OrcishLord )
					++orcs;
			}

			if ( orcs < 10 )
			{
				BaseCreature orc = new SpawnedOrcishLord();

				orc.Team = this.Team;

				Point3D loc = target.Location;
				bool validLocation = false;

				for ( int j = 0; !validLocation && j < 10; ++j )
				{
					int x = target.X + Utility.Random( 3 ) - 1;
					int y = target.Y + Utility.Random( 3 ) - 1;
					int z = map.GetAverageZ( x, y );

					if ( validLocation = map.CanFit( x, y, this.Z, 16, false, false ) )
						loc = new Point3D( x, y, Z );
					else if ( validLocation = map.CanFit( x, y, z, 16, false, false ) )
						loc = new Point3D( x, y, z );
				}

				orc.MoveToWorld( loc, map );

				orc.Combatant = target;
			}
		}

		public OrcLeader( Serial serial ) : base( serial )
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