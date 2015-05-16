using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class DreadGuard : BaseCreature, IRangedMelee
	{
		private BaseWeapon m_RangedWeapon;
		private BaseWeapon m_MeleeWeapon;
		private BaseShield m_MeleeShield;

		public BaseWeapon RangedWeapon{ get{ return m_RangedWeapon; } }
		public BaseWeapon MeleeWeapon{ get{ return m_MeleeWeapon; } }
		public BaseShield MeleeShield{ get{ return m_MeleeShield; } }

		[Constructable]
		public DreadGuard() : base( AIType.AI_RangedMelee, FightMode.Closest, 10, 1, 0.1, 0.1 )
		{
			Body = 0x190;
			Name = NameList.RandomName( "male" );
			SpeechHue = Utility.RandomDyedHue();
			Title = "the dread guard";

			Hue = Utility.RandomSkinHue();

			Utility.AssignRandomHair( this, true );
			if ( Utility.RandomBool() )
				Utility.AssignRandomFacialHair( this, HairHue );

			SetSkill( SkillName.Swords, 100.0 );
			SetSkill( SkillName.Macing, 100.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.MagicResist, 85.0, 90.0 );
			SetSkill( SkillName.Parry, 100.0, 120.0 );
			SetSkill( SkillName.Wrestling, 120.0 );
			SetSkill( SkillName.Archery, 120.0 );
			SetSkill( SkillName.Anatomy, 120.0 );

			Fame = 9000;
			Karma = -9000;

			SetStr( 425, 500 );
			SetDex( 120, 135 );
			SetInt( 48, 50 );

			SetHits( 650, 700 );

			SetDamage( 16, 19 );

			
			
			

			VirtualArmor = 15;

			AddItem( Identify( Rehued( new PlateChest(), 2407 ) ) );
			AddItem( Identify( Rehued( new PlateArms(), 2407 ) ) );
			AddItem( Identify( Rehued( new PlateHelm(), 2407 ) ) );
			AddItem( Identify( Rehued( new PlateGloves(), 2407 ) ) );
			AddItem( Identify( Rehued( new PlateLegs(), 2407 ) ) );
			AddItem( Identify( Rehued( new PlateGorget(), 2407 ) ) );

			m_RangedWeapon = new Crossbow();
			AddItem( Identify( NotCorpseCont( Resourced( m_RangedWeapon, CraftResource.Bronze ) ) ) );

			if ( 0.75 > Utility.RandomDouble() )
				m_MeleeWeapon = new ExecutionersAxe();
			else
			{
				m_MeleeWeapon = new Longsword();
				m_MeleeShield = new HeaterShield();
				PackItem( Identify( Rehued( m_MeleeShield, 2407 ) ) );
			}

			PackItem( Identify( Rehued( m_MeleeWeapon, 2407 ) ) );
			AddItem( new BodySash( 1636 ) );
			AddItem( new Cloak( 1636 ) );

			//PackItem( new Gold( Utility.Random( 150, 200 ) ) );
			PackItem( new Bolt( 50 ) );
		}

		public override bool ClickTitle{ get{ return false; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Gems, 6 );
			AddLoot( LootPack.Rich, 2 );
			AddLoot( LootPack.Potions, Utility.Random( 3 ) );
			AddLoot( LootPack.HighScrolls, Utility.Random( 1 ) );
		}

/*
		public override void AddNameProperties( ObjectPropertyList list )
		{
			list.Add( 1050045, "Dreaded Lord \t{0}\t {1}", Name, ApplyNameSuffix( String.Empty ) ); // ~1_PREFIX~~2_NAME~~3_SUFFIX~
		}
*/
		public DreadGuard( Serial serial ) : base( serial )
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