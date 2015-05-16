#region References
using Server.Items;
#endregion

namespace Server.Mobiles
{
	[TypeAlias("Server.Mobiles.RidableWolf")]
	[CorpseName("a wolf corpse")]
	public class WolfMount : BaseMount
	{
		public override string DefaultName { get { return "a rideable wolf"; } }

		public override int InternalItemItemID { get { return 0x3E91; } }

		public override bool CanHeal { get { return false; } }
		public override bool CanHealOwner { get { return false; } }
		public override FoodType FavoriteFood { get { return FoodType.Meat; } }
		public override bool CanAngerOnTame { get { return false; } }
		public override bool StatLossAfterTame { get { return true; } }
		public override int Hides { get { return 3; } }
		public override int Meat { get { return 3; } }

		[Constructable]
		public WolfMount()
			: base(277, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
		{
			double chance = Utility.RandomDouble() * 23301;

			if (chance <= 1)
			{
				Hue = 0x9C2;
			}
			else if (chance < 50)
			{
				Hue = Utility.RandomList(0x657, 0x515, 0x4B1, 0x481, 0x482, 0x455);
			}
			else if (chance < 500)
			{
				Hue = Utility.RandomList(0x97A, 0x978, 0x901, 0x8AC, 0x5A7, 0x527);
			}

			SetStr(150, 180);
			SetDex(92, 100);
			SetInt(25, 35);

			SetHits(120, 150);

			SetDamage(1, 6);

			SetSkill(SkillName.Wrestling, 24.1, 27.8);
			SetSkill(SkillName.Tactics, 11.3, 33.3);
			SetSkill(SkillName.MagicResist, 15.3, 17.0);
			SetSkill(SkillName.Anatomy, 22.5, 22.4);
			SetSkill(SkillName.Healing, 0);

			Fame = 1000; //Guessing here
			Karma = 1000; //Guessing here

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 26.5;

			//if (Utility.RandomDouble() < 0.2)
		//	{
			//	PackItem(new TreasureMap(5, Map.Trammel));
		//	}

			//if (Utility.RandomDouble() < 0.1)
		//	{
		//		PackItem(new ParrotItem());
		//	}

			PackGold(25, 67);

			// TODO 0-2 spellweaving scroll
		}

		public WolfMount(Serial serial)
			: base(serial)
		{ }

		public override int GetIdleSound()
		{
			return 0x577;
		}

		public override int GetAttackSound()
		{
			return 0x576;
		}

		public override int GetAngerSound()
		{
			return 0x578;
		}

		public override int GetHurtSound()
		{
			return 0x576;
		}

		public override int GetDeathSound()
		{
			return 0x579;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			if (version < 1 && Name == "a rideable wolf")
			{
				Name = "a rideable wolf";
			}
		}
	}
}