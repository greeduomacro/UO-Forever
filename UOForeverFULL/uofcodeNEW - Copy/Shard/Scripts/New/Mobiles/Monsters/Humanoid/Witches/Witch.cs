using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Misc;

namespace Server.Mobiles
{
	[TypeAlias("Server.Mobiles.Warlock")]
	public class Witch : BaseCreature
	{
		[Constructable]
		public Witch() : base( AIType.AI_Mage, FightMode.Closest, 10, 8, 0.25, 0.25 )
		{
			if ( Female = Utility.RandomBool() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
				Title = "the witch";

				AddItem( new Skirt( 1 )  );
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
				Title = "the warlock";

				AddItem( new Robe( 1 )  );
			}
			
			Hue = 0x599;
			
			SpeechHue = Utility.RandomDyedHue();
			HairItemID = 0x203C;
			HairHue = Utility.RandomGreyHue();

			SetSkill( SkillName.Magery, 55.0, 75.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.MagicResist, 65.0, 90.0 );
			SetSkill( SkillName.Parry, 55.0, 75.0 );
			SetSkill( SkillName.Wrestling, 55.0, 75.0 );

			Fame = 4500;
			Karma = -4500;

			SetStr( 425, 478 );
			SetDex( 75, 96 );
			SetInt( 126, 168 );

			SetHits( 375, 445 );

			SetDamage( 15, 28 );

			
			
			
			
			

			
			

			VirtualArmor = 38;

			AddItem( new FancyShirt() );
			AddItem( new WizardsHat( 1 ) );

			if ( 0.001 > Utility.RandomDouble() )
				AddItem( new Sandals( 1 ) ) ;
			else
				AddItem( new Boots( 1 )  );

			AddItem( Immovable( new BlackBookOfSpells( 0.05 > Utility.RandomDouble() ? ulong.MaxValue : 0ul ) ) );
		}

		private int m_BreathPoison = Utility.Random( 50, 35 );

		public override bool ClickTitle{ get{ return false; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }
		public override bool ReacquireOnMovement{ get{ return true; } }
		public override bool HasBreath{ get{ return true; } } // it breaths poison?
		public override double BreathMinDelay{ get{ return 20.0; } }
		public override double BreathMaxDelay{ get{ return 25.0; } }
		public override double BreathStallTime{ get{ return 0.0; } }
		public override double BreathEffectDelay{ get{ return 0.65; } }
		public override int BreathFireDamage{ get{ return 0; } }
		public override int BreathPoisonDamage{ get{ return m_BreathPoison; } }
		public override int BreathEnergyDamage{ get{ return 100 - m_BreathPoison; } }

		public override LootCollection GenerateLootCollection()
		{
			LootCollection coll = new LootCollection("5d10+50");

			coll.AddLoot( new HighMagicAWSTemplate() );

			coll.AddLoot( new MedScrollLootSet( 1 ), 25 );
			coll.AddLoot( new LesserOrRegularPotionLootSet( 0, 3 ), 25 );
			coll.AddLoot( new GemLootSet( 1, 4 ), 0.4, 100 );

			return coll;
		}

		public override void GenerateLoot( bool spawning )
		{
			Container pack = Backpack;

			if ( pack == null )
			{
				pack = new Backpack();
				pack.Movable = false;
				AddItem( pack );
			}

			base.GenerateLoot( spawning );

			LootCollection coll = LootSystem.GetCollection( this.GetType() );

			if ( spawning )
				coll.GenerateGold( this, pack );
			else
				coll.GenerateLoot( this, pack );
		}

		public override bool OnBeforeDeath()
		{
			if ( 0.10 > Utility.RandomDouble() )
				PackItem( new BallOfReputation() );

			return base.OnBeforeDeath();
		}

		public static Item MakeWitch( Item item )
		{
			if ( 0.99 > Utility.RandomDouble() )
				item.SetSavedFlag( 0x01, true );

			return item;
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			if ( from != null && from.Map != null )
			{
				int amt = 0;

				Mobile target = this;

				int rand = Utility.Random( 1, 100 );

				if ( willKill )
					amt = ((( rand % 5 ) >> 2 ) + 3);

				if ( ( Hits < 300 ) && ( rand < 21 ) )
				{
					target = ( rand % 2 ) < 1 ? this : from;
					amt++;
				}

				if ( amt > 0 )
				{
					SpillAcid( target, amt );
					from.SendMessage( "The creature spills a pool of acidic blood." );
				}
			}

			base.OnDamage( amount, from, willKill );
		}

		public override Item NewHarmfulItem()
		{
			AcidSlime slime = new AcidSlime( TimeSpan.FromSeconds( 15 ), 10, 15 );
			slime.Name = "witch's blood";
			slime.Hue = 1175;
			return slime;
		}

		public Witch( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}