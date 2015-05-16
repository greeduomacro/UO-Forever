using System;
using Server;
using Server.Network;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an imp corpse" )]
	public class BurningImp : BaseCreature
	{
		public override string DefaultName{ get{ return "a burning imp"; } }

		[Constructable]
		public BurningImp() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 74;
			BaseSoundID = 422;
			Hue = 1257;

			SetStr( 145, 165 );
			SetDex( 61, 80 );
			SetInt( 106, 125 );

			SetHits( 105, 170 );

			SetDamage( 13, 17 );

			
			
			

			
			
			
			
			

			SetSkill( SkillName.EvalInt, 50.1, 60.0 );
			SetSkill( SkillName.Magery, 65.1, 95.0 );
			SetSkill( SkillName.MagicResist, 65.1, 85.0 );
			SetSkill( SkillName.Tactics, 92.1, 100.0 );
			SetSkill( SkillName.Wrestling, 90.1, 94.0 );

			Fame = 4500;
			Karma = -4500;

			VirtualArmor = 33;

			Tamable = true;
			ControlSlots = 2;
			MinTameSkill = 93.1;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager, 3 );
			AddLoot( LootPack.MedScrolls, 2 );

            if (0.0025 > Utility.RandomDouble()) // 6% chance
            { SkillScroll scroll = new SkillScroll(); scroll.Randomize(); PackItem(scroll); }
		}

		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override int Meat{ get{ return 2; } }
		public override int Hides{ get{ return 3; } }
		public override HideType HideType{ get{ return HideType.Spined; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Daemon; } }
		public override bool CanRummageCorpses{ get{ return true; } }

		public BurningImp( Serial serial ) : base( serial )
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

		public override bool HasAura{ get{ return true; } }
	}
}