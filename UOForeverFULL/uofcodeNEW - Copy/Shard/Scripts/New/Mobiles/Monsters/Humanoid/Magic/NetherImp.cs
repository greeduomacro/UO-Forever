using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an imp corpse" )]
	public class NetherImp : BaseCreature
	{
		public override string DefaultName{ get{ return "a nether imp"; } }

		[Constructable]
		public NetherImp() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 74;
			BaseSoundID = 422;
			Hue = 0x4001;

			SetStr( 182, 230 );
			SetDex( 100, 115 );
			SetInt( 125, 130 );

			SetHits( 125, 145 );

			SetDamage( 13, 17 );

			
			
			

			
			
			
			
			

			SetSkill( SkillName.EvalInt, 60.1, 70.0 );
			SetSkill( SkillName.Magery, 100.0 );
			SetSkill( SkillName.MagicResist, 70.1, 90.0 );
			SetSkill( SkillName.Tactics, 82.1, 90.0 );
			SetSkill( SkillName.Wrestling, 80.1, 84.0 );

			Fame = 4000;
			Karma = -4000;

			VirtualArmor = 35;

			Tamable = true;
			ControlSlots = 3;
			MinTameSkill = 99.1;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.LowScrolls, 2 );
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public override int Meat{ get{ return 0; } }
		public override int Hides{ get{ return 6; } }

		public override HideType HideType
		{
			get
			{
				switch ( Utility.Random( 3 ) )
				{
					default:
					case 0: return HideType.Spined;
					case 1: return HideType.Horned;
					case 2: return HideType.Barbed;
				}
			}
		}

		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Daemon; } }
		public override int DefaultBloodHue{ get{ return -1; } }

		public NetherImp( Serial serial ) : base( serial )
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