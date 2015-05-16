using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a spider corpse" )]
	public class BlackWidow : BaseCreature
	{
		public override string DefaultName{ get{ return "a black widow"; } }

		[Constructable]
		public BlackWidow() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 11;
			Hue = 1109;
			BaseSoundID = 1170;

			SetStr( 196, 220 );
			SetDex( 126, 145 );
			SetInt( 286, 310 );

			SetHits( 600, 800 );

			SetDamage( 10, 17 );

			SetSkill( SkillName.EvalInt, 65.1, 80.0 );
			SetSkill( SkillName.Magery, 65.1, 80.0 );
			SetSkill( SkillName.Meditation, 65.1, 80.0 );
			SetSkill( SkillName.MagicResist, 45.1, 60.0 );
			SetSkill( SkillName.Tactics, 55.1, 70.0 );
			SetSkill( SkillName.Wrestling, 60.1, 75.0 );

			Fame = 5000;
			Karma = -5000;

			VirtualArmor = 36;

			PackItem( new SpidersSilk( 8 ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
		}

		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override Poison HitPoison{ get{ return Poison.Lethal; } }

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			if ( from != null && from.Map != null )
			{
				int amt = 0;

				Mobile target = this;

				if ( willKill )
					amt = 3 + ((Utility.Random( 6 ) % 5) >> 2); // 20% = 1, 20% = 2, 60% = 0

				if ( Hits < 550 )
				{
					double rand = Utility.RandomDouble();

					if ( 0.10 > rand )
						target = from;

					if ( 0.20 > rand )
						amt++;
				}

				if ( amt > 0 )
				{
					SpillAcid( target, amt );

					if ( willKill )
						from.SendMessage( "Your body explodes into a pile of venom!" );
					else
						from.SendMessage( "The creature spits venom at you!" );
				}
			}

			base.OnDamage( amount, from, willKill );
		}

		public override void OnDeath(Container c)
		{
			base.OnDeath( c );	

			double rand = Utility.RandomDouble();

			if ( 0.005 > rand ) //Large Web
				c.AddItem( new LargeWebEast() );
			else if ( 0.01 > rand ) //Large Web
				c.AddItem( new LargeWebSouth() );
		}

		public BlackWidow( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}
