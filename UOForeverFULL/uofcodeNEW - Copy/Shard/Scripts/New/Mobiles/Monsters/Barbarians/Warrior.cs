using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Misc;

namespace Server.Mobiles
{
	public class BarbarianWarrior : BaseBarbarian
	{
		[Constructable]
		public BarbarianWarrior() : this( Utility.RandomBool() )
		{
		}

		[Constructable]
		public BarbarianWarrior( bool female ) : base( female, AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.4 )
		{
			if ( this.Female )
			{
				Title = "a barbarian warrioress";
				AddItem( new ThighBoots( 0x1bb ) );
				AddItem( new FemaleStuddedChest() );
				AddItem( new BattleAxe() );
			}
			else
			{
				Title = "a barbarian warrior";
				AddItem( new Kilt( 0x1bb ) );
				AddItem( new LeatherChest() );
				AddItem( new Boots( 0x1bb ) );
				AddItem( new DoubleAxe() );
				FacialHairItemID = 0x204B;
				FacialHairHue = HairHue;
			}

			AddItem( new DeerMask() );

			SetSkill( SkillName.Swords, 100.0 );
			SetSkill( SkillName.Macing, 80.0, 95.0 );
			SetSkill( SkillName.Tactics, 80.0, 95.0 );
			SetSkill( SkillName.MagicResist, 20.0, 35.0 );
			SetSkill( SkillName.Parry, 75.0, 90.0 );
			SetSkill( SkillName.Wrestling, 80.0, 90.0 );

			Fame = 4000;
			Karma = -4000;

			SetStr( 200, 225 );
			SetDex( 88, 105 );
			SetInt( 48, 50 );

			SetHits( 200, 285 );

			SetDamage( 18, 23 );

			VirtualArmor = 30;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Rich );
		}

		public override bool ClickTitle{ get{ return false; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }

		public BarbarianWarrior( Serial serial ) : base( serial )
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