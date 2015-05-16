using System;
using Server.Misc;
using Server.Network;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	public class KhaldunZealot : BaseCreature
	{
		public override bool ClickTitle{ get{ return false; } }
		public override bool ShowFameTitle{ get{ return false; } }
		public override string DefaultName{ get{ return "a cultist of Khaldun"; } }

		[Constructable]
		public KhaldunZealot(): base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 0x190;
			Title = "the Knight";
			Hue = 0;

            SetStr(767, 945);
            SetDex(66, 75);
            SetInt(46, 70);

            SetHits(476, 552);

            SetDamage(20, 25);

			SetSkill( SkillName.Wrestling, 70.1, 80.0 );
			SetSkill( SkillName.Swords, 120.1, 130.0 );
			SetSkill( SkillName.Anatomy, 120.1, 130.0 );
            SetSkill(SkillName.MagicResist, 125.1, 140.0);
			SetSkill( SkillName.Tactics, 90.1, 100.0 );

			Fame = 10000;
			Karma = -10000;
			VirtualArmor = 40;

			VikingSword weapon = new VikingSword();
			weapon.Hue = 0x835;
            weapon.Identified = true;
			weapon.Movable = false;
            AddItem(Immovable(weapon));

			MetalShield shield = new MetalShield();
			shield.Hue = 0x835;
            shield.Identified = true;
			shield.Movable = false;
			AddItem( Immovable(shield) );

			BoneHelm helm = new BoneHelm();
			helm.Hue = 0x835;
            helm.Identified = true;
			AddItem( Immovable(helm ));

			BoneArms arms = new BoneArms();
			arms.Hue = 0x835;
            arms.Identified = true;
			AddItem( Immovable(arms) );

			BoneGloves gloves = new BoneGloves();
			gloves.Hue = 0x835;
            gloves.Identified = true;
			AddItem( Immovable(gloves) );

			BoneChest tunic = new BoneChest();
			tunic.Hue = 0x835;
            tunic.Identified = true;
			AddItem( Immovable(tunic) );

			BoneLegs legs = new BoneLegs();
			legs.Hue = 0x835;
            legs.Identified = true;
			AddItem( Immovable(legs) );

			AddItem( new Boots() );
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
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }

		public KhaldunZealot( Serial serial ) : base( serial )
		{
		}

		public override bool OnBeforeDeath()
		{
            DemonicSkeleton rm = new DemonicSkeleton();

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