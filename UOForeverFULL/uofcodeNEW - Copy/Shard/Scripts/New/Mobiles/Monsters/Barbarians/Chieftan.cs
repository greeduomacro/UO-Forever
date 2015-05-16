using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Misc;

namespace Server.Mobiles
{
	public class BarbarianChieftan : BaseBarbarian
	{
		[Constructable]
		public BarbarianChieftan() : base( false, AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.2 )
		{
			Title = "a barbarian chieftan";
			AddItem( new Kilt( 0x1bb ) );
			AddItem( new LeatherChest() );
			AddItem( new Boots( 0x1bb ) );
			AddItem( new LargeBattleAxe() );
			FacialHairItemID = 0x204C;
			FacialHairHue = HairHue;
			AddItem( new BearMask() );

			SetSkill( SkillName.Swords, 100.0 );
			SetSkill( SkillName.Macing, 90.0, 100.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.MagicResist, 35.0, 45.0 );
			SetSkill( SkillName.Parry, 90.0, 100.0 );
			SetSkill( SkillName.Wrestling, 90.0, 100.0 );

			Fame = 6000;
			Karma = -6000;

			SetStr( 225, 260 );
			SetDex( 99, 105 );
			SetInt( 48, 50 );

			SetHits( 275, 350 );

			SetDamage( 20, 26 );

			//AddItem( new Gold( 75, 150 ) );

			VirtualArmor = 50;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average, 2 );
			AddLoot( LootPack.Rich );
		}

		public override bool ClickTitle{ get{ return false; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }

		public BarbarianChieftan( Serial serial ) : base( serial )
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