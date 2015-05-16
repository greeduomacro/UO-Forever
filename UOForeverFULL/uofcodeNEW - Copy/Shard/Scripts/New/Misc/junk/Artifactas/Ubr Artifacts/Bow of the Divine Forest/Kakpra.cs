using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a corpse of Kakpra" )]
	public class Kakpra : BaseCreature
	{
		[Constructable]
		public Kakpra() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Kakpra";
			Body = 47;
			Hue = 1881;
			BaseSoundID = 442;

			SetStr( 66, 215 );
			SetDex( 66, 75 );
			SetInt( 101, 250 );

			SetHits( 40, 129 );
			SetStam( 0 );

			SetDamage( 9, 11 );

			
			

			
			
			
			
			

			SetSkill( SkillName.EvalInt, 90.1, 100.0 );
			SetSkill( SkillName.Magery, 90.1, 100.0 );
			SetSkill( SkillName.MagicResist, 100.1, 125.0 );
			SetSkill( SkillName.Tactics, 45.1, 60.0 );
			SetSkill( SkillName.Wrestling, 50.1, 60.0 );

			Fame = 3500;
			Karma = -3500;

			VirtualArmor = 40;

			PackItem( new Log( 10 ) );
			PackItem( new MandrakeRoot( 5 ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
		}




   public override bool OnBeforeDeath()
        {
              switch (Utility.Random(10))  
              {
                  case 0: PackItem(new BowOfTheDivineForest());
                      break;
              }
              return base.OnBeforeDeath();
               }







		public override Poison PoisonImmune{ get{ return Poison.Greater; } }
		public override int TreasureMapLevel{ get{ return 2; } }
		public override bool DisallowAllMoves{ get{ return true; } }

		public Kakpra( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}