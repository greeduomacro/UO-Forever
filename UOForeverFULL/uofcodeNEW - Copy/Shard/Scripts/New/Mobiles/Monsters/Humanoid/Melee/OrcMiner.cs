using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "an orcish corpse" )]
	public class OrcMiner : BaseCreature
	{
		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Orc; } }

		[Constructable]
		public OrcMiner() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "orc" );
			Body = 17;
			BaseSoundID = 0x45A;
			Hue = Utility.Random( 2102, 6 );

			SetStr( 175, 225 );
			SetDex( 81, 105 );
			SetInt( 65, 90 );

			SetHits( 100, 137 );

			SetDamage( 7, 9 );





            Alignment = Alignment.Orc;
			
			

			SetSkill( SkillName.MagicResist, 60.1, 85.0 );
			SetSkill( SkillName.Tactics, 75.1, 100.0 );
			SetSkill( SkillName.Wrestling, 70.1, 90.0 );

			Fame = 3000;
			Karma = -3000;

			VirtualArmor = 30;

			switch ( Utility.Random( 20 ) )
			{
				case 0: PackItem( new Scimitar() ); break;
				case 1: PackItem( new Katana() ); break;
				case 2: PackItem( new WarMace() ); break;
				case 3: PackItem( new WarHammer() ); break;
				case 4: PackItem( new Kryss() ); break;
				case 5: PackItem( new Pitchfork() ); break;
			}

			PackItem( new ThighBoots() );

			switch ( Utility.Random( 4 ) )
			{
				case 0: PackItem( new Ribs() ); break;
				case 1: PackItem( new Shaft() ); break;
				case 2: PackItem( new Candle() ); break;
				case 3: PackItem( new Torch() ); break;
			}

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
			if ( Utility.Random( 250 ) == 0 )
				PackItem( new OrcishPickaxe() );
			else
				PackItem( new Shovel() );

            if (0.02 >= Utility.RandomDouble())
            { SkillScroll scroll = new SkillScroll(); scroll.Randomize(); PackItem(scroll); }
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 3; } }
		public override int Meat{ get{ return 2; } }
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

		public OrcMiner( Serial serial ) : base( serial )
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