using System;
using Server.Misc;
using Server.Network;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	public class KhaldunSummoner : BaseCreature
	{
		public override bool ClickTitle{ get{ return false; } }
		public override bool ShowFameTitle{ get{ return false; } }
		public override string DefaultName{ get{ return "a cultist of Khaldun"; } }

		[Constructable]
		public KhaldunSummoner():base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 0x190;
			Title = "the Summoner";

            SetStr(1254, 1381);
            SetDex(93, 135);
            SetInt(745, 810);

            SetHits(694, 875);

            SetDamage(12, 20);

			
			

			
			
			
			
			

			SetSkill( SkillName.Wrestling, 90.1, 100.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.MagicResist, 90.1, 100.0 );
			SetSkill( SkillName.Magery, 90.1, 100.0 );
			SetSkill( SkillName.EvalInt, 100.0 );
			SetSkill( SkillName.Meditation, 120.1, 130.0 );

			VirtualArmor = 36;
			Fame = 10000;
			Karma = -10000;

			LeatherGloves gloves = new LeatherGloves();
			gloves.Hue = 0x66D;
            gloves.Identified = true;
			AddItem( Immovable(gloves) );

			BoneHelm helm = new BoneHelm();
			helm.Hue = 0x835;
            helm.Identified = true;
			AddItem( Immovable(helm) );

			Necklace necklace = new Necklace();
			necklace.Hue = 0x66D;
			AddItem( Immovable(necklace) );

			Cloak cloak = new Cloak();
			cloak.Hue = 0x66D;
			AddItem( Immovable(cloak) );

			Kilt kilt = new Kilt();
			kilt.Hue = 0x66D;
			AddItem( Immovable(kilt) );

			Sandals sandals = new Sandals();
			sandals.Hue = 0x66D;
			AddItem( Immovable(sandals) );
		}

		public override int GetIdleSound()
		{
			return 0x184;
		}

		public override int GetAngerSound()
		{
			return 0x286;
		}

		public override int GetDeathSound()
		{
			return 0x288;
		}

		public override int GetHurtSound()
		{
			return 0x19F;
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public KhaldunSummoner( Serial serial ) : base( serial )
		{
		}

		public override bool OnBeforeDeath()
		{
            DemonicSkeletonMage rm = new DemonicSkeletonMage();

			Effects.PlaySound(this, Map, GetDeathSound());
			Effects.SendLocationEffect( Location, Map, 0x3709, 30, 10, 0x835, 0 );
			rm.MoveToWorld( Location, Map );

			Delete();
			return false;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}