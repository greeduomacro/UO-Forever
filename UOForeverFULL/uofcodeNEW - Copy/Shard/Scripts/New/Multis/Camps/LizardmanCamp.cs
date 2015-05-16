#region References
using Server.Items;
using Server.Mobiles;
#endregion

namespace Server.Multis
{
	public class LizardmanCamp : BaseCamp
	{
		private Mobile m_Prisoner;
		private BaseDoor m_Gate;
		private MetalChest m_Chest;
		private bool m_Filled;

		[Constructable]
		public LizardmanCamp()
			: base(0x1D4C)
		{ }

		public override void AddComponents()
		{
			m_Gate = new IronGate(DoorFacing.EastCCW) {
				KeyValue = Key.RandomValue(),
				Locked = true
			};

			AddItem(m_Gate, -2, 1, 0);

			m_Chest = new MetalChest {
				ItemID = 0xE7C,
				LiftOverride = true
			};

			m_Chest.DropItem(new Key(KeyType.Iron, m_Gate.KeyValue));

			AddItem(m_Chest, 4, 4, 0);

			AddMobile(new Lizardman(), 15, 0, -2, 0);
			AddMobile(new Lizardman(), 15, 0, -2, 0);
			AddMobile(new Lizardman(), 15, 0, -2, 0);
			AddMobile(new LizardWarrior(), 15, 0, 1, 0);
			AddMobile(new LizardMage(), 15, 0, -1, 0);
			AddMobile(new LizardMage(), 15, 0, -1, 0);
			AddMobile(new LizardArcher(), 15, 0, 0, 0);
			AddMobile(new LizardArcher(), 15, 0, 0, 0);

			switch (Utility.Random(2))
			{
				case 0:
					m_Prisoner = new Noble();
					break;
				case 1:
					m_Prisoner = new SeekerOfAdventure();
					break;
			}

			m_Prisoner.YellHue = Utility.RandomList(0x57, 0x67, 0x77, 0x87, 0x117);

			AddMobile(m_Prisoner, 2, -2, 0, 0);
		}

		protected override void OnExpansionChanged(Expansion old)
		{
			base.OnExpansionChanged(old);

			if (m_Chest == null || m_Filled)
			{
				return;
			}

			TreasureMapChest.Fill(m_Chest, 2, Expansion);
			m_Filled = true;
		}

		public override void OnEnter(Mobile m)
		{
			base.OnEnter(m);

			if (!m.Player || m_Prisoner == null || m_Gate == null || !m_Gate.Locked)
			{
				return;
			}

			int number;

			switch (Utility.Random(4))
			{
				default:
					number = 502264;
					break; // Help a poor prisoner!
				case 1:
					number = 502266;
					break; // Aaah! Help me!
				case 2:
					number = 1046000;
					break; // Help! These savages wish to end my life!
				case 3:
					number = 1046003;
					break; // Quickly! Kill them for me! HELP!!
			}

			m_Prisoner.Yell(number);
		}

		public LizardmanCamp(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt(1); // version

			writer.Write(m_Prisoner);
			writer.Write(m_Gate);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();

			switch (version)
			{
				case 1:
					m_Chest = reader.ReadItem<MetalChest>();
					m_Filled = reader.ReadBool();
					goto case 0;
				case 0:
					{
						m_Prisoner = reader.ReadMobile();
						m_Gate = reader.ReadItem<BaseDoor>();
					}
					break;
			}
		}
	}
}