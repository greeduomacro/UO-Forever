using System;
using Server.Items;

namespace Server.Mobiles
{
	public class BloodKnight : BaseCreature
	{
		public override string DefaultName{ get{ return "a blood knight"; } }

		[Constructable]
		public BloodKnight() : base( AIType.AI_Mage, FightMode.Weakest, 10, 1, 0.2, 0.4 )
		{
			Hue = Utility.RandomSkinHue();
			Body = 400;
			Hue = 1175;
			BaseSoundID = 0;

			SetStr( 275, 375 );
			SetDex( 40, 75 );
			SetInt( 300, 350 );

			SetHits( 230, 375 );
			SetMana( 300, 350 );

			SetDamage( 10, 15 );

			
			

			
			
			
			

			SetSkill( SkillName.Wrestling, 100.2, 100.6 );
			SetSkill( SkillName.Tactics, 100.7, 100.4 );
			SetSkill( SkillName.Anatomy, 100.5, 100.3 );
			SetSkill( SkillName.MagicResist, 110.4, 110.7 );
			SetSkill( SkillName.Magery, 120.4, 120.7 );
			SetSkill( SkillName.Swords, 130.4, 130.7 );
			SetSkill( SkillName.EvalInt, 130.4, 130.7 );

			Fame = 6000;
			Karma = -10000;

			VirtualArmor = 45;

			BaseWeapon sword = new PaladinSword();
			sword.Slayer = SlayerName.DragonSlaying;

			EquipItem( Identify( NotCorpseCont( sword ) ) );

			EquipItem( MakeBloodArmor( new DragonLegs() ) );
			EquipItem( MakeBloodArmor( new DragonChest() ) );
			EquipItem( MakeBloodArmor( new DragonGloves() ) );
			EquipItem( MakeBloodArmor( new DragonHelm() ) );
			EquipItem( MakeBloodArmor( new PlateGorget() ) );
			EquipItem( MakeBloodArmor( new DragonArms() ) );

			EquipItem( Identify( NotCorpseCont( Rehued( new Sandals(), 1157 ) ) ) );

			HairItemID = 0x203B;
			HairHue = 1109;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager, 2 );
		}

		public static BaseArmor MakeBloodArmor( BaseArmor armor )
		{
			if ( 0.995 > Utility.RandomDouble() )
				armor.SetSavedFlag( 0x01, true );

			armor.Resource = CraftResource.BloodScales;
			armor.Identified = true;

			return armor;
		}

		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool CanRummageCorpses{ get{ return true; } }

		public BloodKnight( Serial serial ) : base( serial )
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