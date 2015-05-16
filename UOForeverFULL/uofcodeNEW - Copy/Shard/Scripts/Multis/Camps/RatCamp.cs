#region References
using System;

using Server.Items;
using Server.Mobiles;
#endregion

namespace Server.Multis
{
	public class RatCamp : BaseCamp
	{
		public virtual Mobile Ratmen { get { return new Ratman(); } }

		private BaseCreature m_Prisoner;
		private LockableContainer m_Chest;
		private LockableContainer m_Crate;
		private bool m_Filled;

		[Constructable]
		public RatCamp()
			: base(0x10ee) // dummy garbage at center
		{ }

		public override void AddComponents()
		{
			Visible = false;
			DecayDelay = TimeSpan.FromMinutes(5.0);

			AddItem(new Static(0x10ee), 0, 0, 0);
			AddItem(new Static(0xfac), 0, 6, 0);

			switch (Utility.Random(3))
			{
				case 0:
					{
						AddItem(new Item(0xDE3), 0, 6, 0); // Campfire
						AddItem(new Item(0x974), 0, 6, 1); // Cauldron
					}
					break;
				case 1:
					AddItem(new Item(0x1E95), 0, 6, 1); // Rabbit on a spit
					break;
				default:
					AddItem(new Item(0x1E94), 0, 6, 1); // Chicken on a spit
					break;
			}

			AddItem(new Item(0x41F), 5, 5, 0); // Gruesome Standart South

			AddCampChests();

			for (int i = 0; i < 4; i ++)
			{
				AddMobile(Ratmen, 6, Utility.RandomMinMax(-7, 7), Utility.RandomMinMax(-7, 7), 0);
			}

			switch (Utility.Random(2))
			{
				case 0:
					m_Prisoner = new Noble();
					break;
				default:
					m_Prisoner = new SeekerOfAdventure();
					break;
			}

			m_Prisoner.IsPrisoner = true;
			m_Prisoner.CantWalk = true;

			m_Prisoner.YellHue = Utility.RandomList(0x57, 0x67, 0x77, 0x87, 0x117);
			AddMobile(m_Prisoner, 2, Utility.RandomMinMax(-2, 2), Utility.RandomMinMax(-2, 2), 0);
		}

		private void AddCampChests()
		{
			switch (Utility.Random(3))
			{
				case 0:
					m_Chest = new MetalChest();
					break;
				case 1:
					m_Chest = new MetalGoldenChest();
					break;
				default:
					m_Chest = new WoodenChest();
					break;
			}

			m_Chest.LiftOverride = true;

			AddItem(m_Chest, -2, -2, 0);

			switch (Utility.Random(4))
			{
				case 0:
					m_Crate = new SmallCrate();
					break;
				case 1:
					m_Crate = new MediumCrate();
					break;
				case 2:
					m_Crate = new LargeCrate();
					break;
				default:
					m_Crate = new LockableBarrel();
					break;
			}

			m_Crate.TrapType = TrapType.ExplosionTrap;
			m_Crate.TrapPower = Utility.RandomMinMax(30, 40);
			m_Crate.TrapLevel = 2;

			m_Crate.RequiredSkill = 76;
			m_Crate.LockLevel = 66;
			m_Crate.MaxLockLevel = 116;
			m_Crate.Locked = true;

			m_Crate.DropItem(new Gold(Utility.RandomMinMax(100, 400)));
			m_Crate.DropItem(new Arrow(10));
			m_Crate.DropItem(new Bolt(10));

			m_Crate.LiftOverride = true;

			if (Utility.RandomDouble() < 0.8)
			{
				switch (Utility.Random(4))
				{
					case 0:
						m_Crate.DropItem(new LesserCurePotion());
						break;
					case 1:
						m_Crate.DropItem(new LesserExplosionPotion());
						break;
					case 2:
						m_Crate.DropItem(new LesserHealPotion());
						break;
					default:
						m_Crate.DropItem(new LesserPoisonPotion());
						break;
				}
			}

			AddItem(m_Crate, 2, 2, 0);
		}

		protected override void OnExpansionChanged(Expansion old)
		{
			base.OnExpansionChanged(old);

			if (m_Chest == null || m_Filled)
			{
				return;
			}

			TreasureMapChest.Fill(m_Chest, 1, Expansion);
			m_Filled = true;
		}

		// Don't refresh decay timer
		public override void OnEnter(Mobile m)
		{
			if (!m.Player || m_Prisoner == null || !m_Prisoner.CantWalk)
			{
				return;
			}

			int number;

			switch (Utility.Random(8))
			{
				case 0:
					number = 502261;
					break; // HELP!
				case 1:
					number = 502262;
					break; // Help me!
				case 2:
					number = 502263;
					break; // Canst thou aid me?!
				case 3:
					number = 502264;
					break; // Help a poor prisoner!
				case 4:
					number = 502265;
					break; // Help! Please!
				case 5:
					number = 502266;
					break; // Aaah! Help me!
				case 6:
					number = 502267;
					break; // Go and get some help!
				default:
					number = 502268;
					break; // Quickly, I beg thee! Unlock my chains! If thou dost look at me close thou canst see them.
			}

			m_Prisoner.Yell(number);
		}

		// Don't refresh decay timer
		public override void OnExit(Mobile m)
		{ }

		public RatCamp(Serial serial)
			: base(serial)
		{ }

		public override void AddItem(Item item, int xOffset, int yOffset, int zOffset)
		{
			if (item != null)
			{
				item.Movable = false;
			}

			base.AddItem(item, xOffset, yOffset, zOffset);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(2); // version

			// 2
			writer.Write(m_Chest);
			writer.Write(m_Crate);
			writer.Write(m_Filled);

			// 1
			writer.Write(m_Prisoner);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 2:
					{
						m_Chest = reader.ReadItem<LockableContainer>();
						m_Crate = reader.ReadItem<LockableContainer>();
						m_Filled = reader.ReadBool();
					}
					goto case 1;
				case 1:
					m_Prisoner = reader.ReadMobile<BaseCreature>();
					goto case 0;
				case 0:
					break;
			}
		}
	}
}