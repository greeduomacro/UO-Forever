using System;
using Server.Items;

namespace Server.Mobiles
{
	public class UndeadKnight : BaseCreature
	{
		public override string DefaultName{ get{ return "a undead knight"; } }

		[Constructable]
		public UndeadKnight() : base( AIType.AI_Mage, FightMode.Weakest, 10, 1, 0.2, 0.4 )
		{
			Hue = Utility.RandomSkinHue();
			Body = 400;
			Hue = 253;
			BaseSoundID = 0;

            Alignment = Alignment.Undead;

			SetStr( 275, 375 );
			SetDex( 90, 120 );
			SetInt( 300, 350 );

			SetHits( 730, 775 );
			SetMana( 300, 350 );

			SetDamage( 20, 25 );

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

			
			EquipItem( Immovable(MakeBloodArmor( new DragonLegs() ))) ;
            EquipItem(Immovable(MakeBloodArmor(new DragonChest())));
			EquipItem( Immovable(MakeBloodArmor( new DragonGloves() )) );
			EquipItem( Immovable(MakeBloodArmor( new DragonHelm() )) );
			EquipItem( Immovable(MakeBloodArmor( new PlateGorget() )) );
			EquipItem( Immovable(MakeBloodArmor( new DragonArms() )) );

			EquipItem( Immovable(Identify( NotCorpseCont( Rehued( new Sandals(), 1157 ) ) )) );

			HairItemID = 0x203B;
			HairHue = 1109;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 2 );
		}

		public static BaseArmor MakeBloodArmor( BaseArmor armor )
		{
			if ( 0.995 > Utility.RandomDouble() )
				armor.SetSavedFlag( 0x01, true );

			armor.Resource = CraftResource.BloodScales;
			armor.Identified = true;

			return armor;
		}
public override void OnDeath(Container c)
		{
			base.OnDeath( c );	

			double rand = Utility.RandomDouble();

			if ( 0.0020 > rand ) 
				c.AddItem( new BloodyBandage(2) );
			else if ( 0.030 > rand ) 
				c.AddItem( new BloodyBandage() );

		}
		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool CanRummageCorpses{ get{ return true; } }



		public UndeadKnight( Serial serial ) : base( serial )
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