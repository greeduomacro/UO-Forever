using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "pirate crew corpse" )]
	public class ArcherCrew1 : BaseCreature
	{
		[Constructable]
		public ArcherCrew1() : base( AIType.AI_Archer, FightMode.Strongest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName("male"); 
			Title = "the Crewman";
			Body = 0x190;


			SetStr( 190, 210 );
			SetDex( 70, 80 );
			SetInt( 250 );
			SetHits( 190, 250 );
			SetMana( 250 );

			SetDamage( 8, 18 );

			SetSkill( SkillName.Fencing, 96.0, 99.5 );
			SetSkill( SkillName.Macing, 89.0, 97.5 );
			SetSkill( SkillName.MagicResist, 57.0, 69.5 );
			SetSkill( SkillName.Swords, 95.0, 97.5 );
			SetSkill( SkillName.Tactics, 95.0, 97.5 );
			SetSkill( SkillName.Wrestling, 95.0, 97.5 );
			SetSkill( SkillName.Archery, 95.0, 97.5 );


			Fame = 2000;
			Karma = -2000;

			VirtualArmor = 40;

			HairItemID = Utility.RandomList(0x203B, 0x2049, 0x2048, 0x204A);
			HairHue = Utility.RandomNondyedHue();
			FacialHairItemID = Utility.RandomList(0x203E, 0x203F, 0x2040, 0x2041, 0x204B, 0x204C, 0x204D);
			FacialHairHue = Utility.RandomNondyedHue();

			AddItem( new SkullCap() );
			AddItem( NotCorpseCont( new ShortPants( 2052 ) ) );
			AddItem( new FancyShirt() );
			AddItem( new ThighBoots() );
			AddItem( new BodySash() );
			AddItem( new HeavyCrossbow() );
			AddItem( new Bolt( 50 ) );



		}

		public override bool AutoDispel{ get{ return false; } }
		public override bool BardImmune{ get{ return true; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }
		public override Poison HitPoison{ get{ return Poison.Greater; } }

		public ArcherCrew1( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
    }
}
