using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class DreadLord : BaseCreature, IRangedMelee
	{
		private BaseWeapon m_RangedWeapon;
		private BaseWeapon m_MeleeWeapon;
		private BaseShield m_MeleeShield;

		public BaseWeapon RangedWeapon{ get{ return m_RangedWeapon; } }
		public BaseWeapon MeleeWeapon{ get{ return m_MeleeWeapon; } }
		public BaseShield MeleeShield{ get{ return m_MeleeShield; } }

		[Constructable]
		public DreadLord() : base( AIType.AI_RangedMelee, FightMode.Closest, 10, 1, 0.1, 0.1 )
		{
			Body = 0x190;
			Name = NameList.RandomName( "male" );
			SpeechHue = Utility.RandomDyedHue();
			Title = "the dread lord";

			Hue = Utility.RandomSkinHue();

			Utility.AssignRandomHair( this, true );
			if ( Utility.RandomBool() )
				Utility.AssignRandomFacialHair( this, HairHue );

			SetSkill( SkillName.Swords, 120.0 );
			SetSkill( SkillName.Macing, 120.0 );
			SetSkill( SkillName.Tactics, 120.0 );
			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Parry, 120.0 );
			SetSkill( SkillName.Wrestling, 120.0 );
			SetSkill( SkillName.Archery, 120.0 );
			SetSkill( SkillName.Anatomy, 120.0 );

			Fame = 9000;
			Karma = -9000;

			SetStr( 650, 700 );
			SetDex( 120, 135 );
			SetInt( 48, 50 );

			SetHits( 690, 750 );

			SetDamage( 20, 25 );

			
			
			

			VirtualArmor = 100;

			AddItem( NotCorpseCont( Immovable(new RoyalPlateChest()) ) );
			AddItem( NotCorpseCont( Immovable(new RoyalPlateArms()) ) );
			AddItem( NotCorpseCont( Immovable(new RoyalPlateHelm()) ) );
			AddItem( NotCorpseCont( Immovable(new RoyalPlateGloves()) ) );
			AddItem( NotCorpseCont( Immovable(new RoyalPlateLegs()) ) );
			AddItem( NotCorpseCont( Immovable(new RoyalPlateGorget()) ) );
			AddItem( NotCorpseCont( Immovable(new RoyalPlateBoots()) ) );
			m_RangedWeapon = (BaseWeapon)Resourced( new HeavyCrossbow(), CraftResource.Bronze );
			AddItem( Identify( NotCorpseCont( m_RangedWeapon ) ) );
			if ( 0.75 > Utility.RandomDouble() )
				m_MeleeWeapon = new OrnateWarAxe();
			else
			{
				m_MeleeWeapon = new RadiantWarSword();
				m_MeleeShield = new RoyalShield();
				PackItem( NotCorpseCont( m_MeleeShield ) );
			}

			PackItem( NotCorpseCont( m_MeleeWeapon ) );
			AddItem( new BodySash( 1636 ) );
			AddItem( NotCorpseCont( new RoyalCloak() ) );

			//PackItem( new Gold( Utility.Random( 300, 500 ) ) );
			PackItem( new Bolt( 50 ) );
		}

		public override bool ClickTitle{ get{ return false; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Gems, 6 );
			if ( Utility.RandomBool() )
				AddLoot( LootPack.FilthyRich, 2 );
			else
				AddLoot( LootPack.UltraRich );
			AddLoot( LootPack.Potions, Utility.Random( 3 ) );
			AddLoot( LootPack.HighScrolls, Utility.Random( 1 ) );
		}

/*
		public override void AddNameProperties( ObjectPropertyList list )
		{
			list.Add( 1050045, "Dreaded Lord \t{0}\t {1}", Name, ApplyNameSuffix( String.Empty ) ); // ~1_PREFIX~~2_NAME~~3_SUFFIX~
		}
*/
		public DreadLord( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version

			writer.Write( m_RangedWeapon );
			writer.Write( m_MeleeWeapon );
			writer.Write( m_MeleeShield );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_RangedWeapon = reader.ReadItem() as BaseWeapon;
			m_MeleeWeapon = reader.ReadItem() as BaseWeapon;
			m_MeleeShield = reader.ReadItem() as BaseShield;
		}
	}
}