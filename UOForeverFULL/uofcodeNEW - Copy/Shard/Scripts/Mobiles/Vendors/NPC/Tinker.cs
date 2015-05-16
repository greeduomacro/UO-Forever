#region References
using System.Collections.Generic;

using Server.ContextMenus;
using Server.Items;
#endregion

namespace Server.Mobiles
{
	public class Tinker : BaseVendor
	{
		private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

		public override NpcGuild NpcGuild { get { return NpcGuild.TinkersGuild; } }

		[Constructable]
		public Tinker()
			: base("the tinker")
		{
			SetSkill(SkillName.Lockpicking, 60.0, 83.0);
			SetSkill(SkillName.RemoveTrap, 75.0, 98.0);
			SetSkill(SkillName.Tinkering, 64.0, 100.0);
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add(new SBTinker());
		}

		public Tinker(Serial serial)
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

		#region Veteran Rewards
		public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
		{
			if (from.Alive && EraML)
			{
				var entry = new RechargeEntry(from, this);

				if (WeaponEngravingTool.Find(from) == null)
				{
					entry.Enabled = false;
				}

				list.Add(entry);
			}

			base.AddCustomContextEntries(from, list);
		}

		private class RechargeEntry : ContextMenuEntry
		{
			private readonly Mobile m_From;
			private readonly BaseVendor m_Vendor;

			public RechargeEntry(Mobile from, BaseVendor vendor)
				: base(6271, 6)
			{
				m_From = from;
				m_Vendor = vendor;
			}

			public override void OnClick()
			{
				if (m_Vendor == null || m_Vendor.Deleted)
				{
					return;
				}

				WeaponEngravingTool tool = WeaponEngravingTool.Find(m_From);

				if (tool != null && tool.UsesRemaining <= 0)
				{
					if (Banker.GetBalance(m_From, m_Vendor.TypeOfCurrency) >= 100000)
					{
						m_From.SendGump(new WeaponEngravingTool.ConfirmGump(tool, m_Vendor));
					}
					else
					{
						m_Vendor.Say(1076167); // You need a 100,000 gold and a blue diamond to recharge the weapon engraver.
					}
				}
				else
				{
					m_Vendor.Say(1076164); // I can only help with this if you are carrying an engraving tool that needs repair.
				}
			}
		}
		#endregion
	}
}