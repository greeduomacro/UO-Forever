using System;
using System.Collections;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class UndeadExecutioner : BaseCreature
	{
		[Constructable]
		public UndeadExecutioner() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			SpeechHue = Utility.RandomDyedHue();
			Title = "the undead executioner";
			//Hue = Utility.RandomSkinHue();
			Hue = 2403;
            Alignment = Alignment.Undead;
			Body = 0x190;
			Name = NameList.RandomName( "male" );

			SetStr( 486, 550 );
			SetDex( 151, 165 );
			SetInt( 161, 175 );

			SetDamage( 10, 14 );

			SetSkill( SkillName.Anatomy, 125.0 );
			SetSkill( SkillName.Fencing, 76.0, 86.5 );
			SetSkill( SkillName.Macing, 75.0, 107.5 );
			SetSkill( SkillName.Poisoning, 100.0 );
			SetSkill( SkillName.MagicResist, 93.5, 102.5 );
			SetSkill( SkillName.Swords, 125.0 );
			SetSkill( SkillName.Tactics, 125.0 );
			SetSkill( SkillName.Lumberjacking, 125.0 );

			Fame = 5000;
			Karma = -5000;

			VirtualArmor = 55;

			AddItem( Immovable(new HoodedShroudOfShadows( 1175 )) );
			AddItem( Immovable(new ExecutionersAxe()));

			Utility.AssignRandomHair( this );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Meager );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public UndeadExecutioner( Serial serial ) : base( serial )
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