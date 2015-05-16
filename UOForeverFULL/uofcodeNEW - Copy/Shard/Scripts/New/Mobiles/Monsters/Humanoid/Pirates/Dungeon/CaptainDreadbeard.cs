using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "pirate captain corpse" )]
	public class CaptainDreadbeard : BaseCreature
	{
		public override string DefaultName{ get{ return "Captain Dreadbeard"; } }

		[Constructable]
		public CaptainDreadbeard() : base( AIType.AI_Archer, FightMode.Strongest, 10, 1, 0.2, 0.4 )
		{
			Body = 0x190;

			SetStr( 1250, 1400 );
			SetDex( 155, 250 );
			SetInt( 250 );
			SetHits( 2500 );
			SetMana( 250 );

			SetDamage( 20, 30 );

			SetSkill( SkillName.EvalInt, 120.0 );
			SetSkill( SkillName.Magery, 120.0 );
			SetSkill( SkillName.Meditation, 120.0 );
			SetSkill( SkillName.MagicResist, 250.0 );
			SetSkill( SkillName.Tactics, 120.0 );
			SetSkill( SkillName.Anatomy, 120.0 );
			SetSkill( SkillName.Healing, 120.0 );
			SetSkill( SkillName.Swords, 120.0 );
			SetSkill( SkillName.Wrestling, 120.0 );
			SetSkill( SkillName.Poisoning, 120.0 );

			Fame = 10000;
			Karma = -10000;

			VirtualArmor = 60;

			HairItemID = 0x203C;
			HairHue = 2052;
			FacialHairItemID = 0x204C;
			FacialHairHue = 2052;

			AddItem( NotCorpseCont( new TricorneHat( 2052 ) ) );
			AddItem( NotCorpseCont( Rehued( Renamed( new LeafChest(), "a vest" ), 2053 ) ) );
			AddItem( NotCorpseCont( new ShortPants( 2052 ) ) );
			AddItem( NotCorpseCont( Rehued( new GoldRing(), 1150 ) ) );
			AddItem( new FancyShirt() );
			AddItem( new ThighBoots() );
			AddItem( new Arrow( 500 ) );

			AddItem( NotCorpseCont( new LongBow() ) );
		}

		public override bool AutoDispel{ get{ return true; } }
		public override bool BardImmune{ get{ return true; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override Poison HitPoison{ get{ return Poison.Lethal; } }
		public override int TreasureMapLevel{ get{ return 5; } }

		public CaptainDreadbeard( Serial serial ) : base( serial )
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