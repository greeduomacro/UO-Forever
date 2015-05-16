#region References
using System.Collections.Generic;
using System.Linq;

using Mat = Server.Engines.BulkOrders.BulkMaterialType;
#endregion

namespace Server.Engines.BulkOrders
{
	public class LargeTailorBOD : LargeBOD
	{
		public static double[] m_TailoringMaterialChances = new[]
		{
			0.857421875, // None
			0.125000000, // Spined
			0.015625000, // Horned
			0.001953125 // Barbed
		};

		public override int ComputeFame()
		{
			return TailorRewardCalculator.Instance.ComputeFame(this);
		}

		public override int ComputeGold()
		{
			return TailorRewardCalculator.Instance.ComputeGold(this);
		}

		[Constructable]
		public LargeTailorBOD()
		{
			LargeBulkEntry[] entries;
			bool useMaterials = false;

			switch (Utility.Random(14))
			{
				default:
				//case 0:
					entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Farmer);
					break;
				case 1:
					entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.FemaleLeatherSet);
					useMaterials = true;
					break;
				case 2:
					entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.FisherGirl);
					break;
				case 3:
					entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Gypsy);
					break;
				case 4:
					entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.HatSet);
					break;
				case 5:
					entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Jester);
					break;
				case 6:
					entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Lady);
					break;
				case 7:
					entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.MaleLeatherSet);
					useMaterials = true;
					break;
				case 8:
					entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Pirate);
					break;
				case 9:
					entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.ShoeSet);
					break;
				case 10:
					entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.StuddedSet);
					useMaterials = true;
					break;
				case 11:
					entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.TownCrier);
					break;
				case 12:
					entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Wizard);
					break;
				case 13:
					entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.BoneSet);
					useMaterials = true;
					break;
			}

			const int hue = 0x483;
			int amountMax = Utility.RandomList(10, 15, 20, 20);
			bool reqExceptional = (0.825 > Utility.RandomDouble());

			BulkMaterialType material = useMaterials
											? GetRandomMaterial(BulkMaterialType.Spined, m_TailoringMaterialChances)
											: BulkMaterialType.None;

			Hue = hue;
			AmountMax = amountMax;
			Entries = entries;
			RequireExceptional = reqExceptional;
			Material = material;
		}

		public LargeTailorBOD(int amountMax, bool reqExceptional, BulkMaterialType mat, LargeBulkEntry[] entries)
		{
			Hue = 0x483;
			AmountMax = amountMax;
			Entries = entries;
			RequireExceptional = reqExceptional;
			Material = mat;
		}

		protected override void OnExpansionChanged(Expansion old)
		{
			base.OnExpansionChanged(old);

			if (EraML && Material == Mat.None)
			{
				Material = GetRandomMaterial(BulkMaterialType.Spined, m_TailoringMaterialChances);
			}
		}

		public override List<Item> ComputeRewards(bool full)
		{
			var list = new List<Item>();

			RewardGroup rewardGroup =
				TailorRewardCalculator.Instance.LookupRewards(TailorRewardCalculator.Instance.ComputePoints(this));

			if (rewardGroup != null)
			{
				if (full)
				{
					list.AddRange(rewardGroup.Items.Select(t => t.Construct()).Where(item => item != null));
				}
				else
				{
					RewardItem rewardItem = rewardGroup.AcquireItem();

					if (rewardItem != null)
					{
						Item item = rewardItem.Construct();

						if (item != null)
						{
							list.Add(item);
						}
					}
				}
			}

			return list;
		}

		public LargeTailorBOD(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}
}