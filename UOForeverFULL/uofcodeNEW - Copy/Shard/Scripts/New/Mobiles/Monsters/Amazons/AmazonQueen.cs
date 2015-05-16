using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class AmazonQueen : BaseAmazon
	{
		[Constructable]
		public AmazonQueen() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.3 )
		{
			Title = "the Amazon Queen";

			SetSkill( SkillName.Parry, 120.0 );
			SetSkill( SkillName.Swords, 120.0 );
			SetSkill( SkillName.Macing, 120.0 );
			SetSkill( SkillName.Tactics, 120.0 );
			SetSkill( SkillName.MagicResist, 120.0 );

			SetSkill( SkillName.Magery, 100.0, 110.0 );
			SetSkill( SkillName.EvalInt, 120.0 );
			SetSkill( SkillName.Meditation, 120.0 );

			AddItem( MakeAmazonArmor( new LeatherChest() ) );
			AddItem( MakeAmazonArmor( new LeatherGloves() ) );
			AddItem( MakeAmazonArmor( new Boots() ) );
			AddItem( new AmazonSpellbook() );

			if ( Utility.Random( 450 ) == 0 )
				AddItem( new AmazonJadeNecklace() );

			if ( Utility.Random( 450 ) == 0 )
				AddRingOfPower();

			
			
			
			
			

			Fame = 15000;
			Karma = -10000;

			SetStr( 575, 750 );
			SetDex( 100 );
			SetInt( 250, 300 );

			SetHits( 1200 );

			SetDamage( 25, 35 );

			
			

			VirtualArmor = 100;
		}

	//	public override bool BardImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

		public AmazonQueen( Serial serial ) : base( serial )
		{
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average, 2 );
			AddLoot( LootPack.Rich, 2 );
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.UltraRich );

			AddLoot( LootPack.Potions, 4 );
			AddLoot( LootPack.Gems, 4 );
			AddLoot( LootPack.HighScrolls, 4 );
			AddLoot( LootPack.MedScrolls, 3 );

			if ( 0.25 > Utility.RandomDouble() )
				AddLoot( LootPack.UltraRich );

			if ( 0.05 > Utility.RandomDouble() )
				PackBagofRegs( Utility.RandomMinMax( 25, 45 ) );
				//PackItem( new BagOfReagents( 75 ) );

			if ( 0.25 > Utility.RandomDouble() )
				PackBagofRegs( Utility.RandomMinMax( 10, 20 ) );
				//PackItem( new BagOfReagents( 30 ) );
		}

		public override void AlterDamageScalarFrom( Mobile caster, ref double scalar )
		{
			if ( caster is BaseCreature )
			{
				BaseCreature creat = (BaseCreature)caster;
				if ( (creat.Controlled || creat.Summoned) && creat.ControlMaster != null )
					scalar = 0.50;
			}
		}

		public override void AlterDamageScalarTo( Mobile target, ref double scalar )
		{
			if ( target is BaseCreature )
			{
				BaseCreature creat = (BaseCreature)target;
				if ( (creat.Controlled || creat.Summoned) && creat.ControlMaster != null )
					scalar += 2.0;
			}
		}

		public override void AlterMeleeDamageFrom( Mobile from, ref int damage )
		{
			if ( from is BaseCreature )
			{
				BaseCreature creat = (BaseCreature)from;
				if ( (creat.Controlled || creat.Summoned) && creat.ControlMaster != null )
					damage /= 2;
			}
		}

		public override void AlterMeleeDamageTo( Mobile to, ref int damage )
		{
			if ( to is BaseCreature )
			{
				BaseCreature creat = (BaseCreature)to;
				if ( (creat.Controlled || creat.Summoned) && creat.ControlMaster != null )
					damage *= 2;
			}
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