#region References
using System.Collections.Generic;

using Server.Network;
#endregion

namespace Server.Mobiles
{
	public class Architect : BaseVendor
	{
		private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

		public override NpcGuild NpcGuild { get { return NpcGuild.TinkersGuild; } }

		[Constructable]
		public Architect()
			: base("the architect")
		{ }

		public override void InitSBInfo()
		{
			m_SBInfos.Add(new SBArchitect(Expansion));
			m_SBInfos.Add(new SBHouseDeed());
		}

		public Architect(Serial serial)
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

			int version = reader.ReadInt();
		}

		public override void VendorBuy(Mobile buyer)
		{
			base.VendorBuy(buyer);
			buyer.LocalOverheadMessage(
				MessageType.Regular,
				38,
				false,
				"WARNING! House deeds can only be sold back to real-estate broker NPCs for a fraction of the buying cost! Deeds are NOT blessed. House placement tools are HIGHLY recommended!");
		}
	}
}