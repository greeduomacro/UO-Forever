#region References
using System;
using System.Collections.Generic;

using Server.Engines.BulkOrders;
using Server.Factions;
using Server.Games;
using Server.Items;
#endregion

namespace Server.Mobiles
{
	public class Blacksmith : BaseVendor
	{
		private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

		public override NpcGuild NpcGuild { get { return NpcGuild.BlacksmithsGuild; } }

		[Constructable]
		public Blacksmith()
			: base("the blacksmith")
		{
			SetSkill(SkillName.ArmsLore, 36.0, 68.0);
			SetSkill(SkillName.Blacksmith, 65.0, 88.0);
			SetSkill(SkillName.Fencing, 60.0, 83.0);
			SetSkill(SkillName.Macing, 61.0, 93.0);
			SetSkill(SkillName.Swords, 60.0, 83.0);
			SetSkill(SkillName.Tactics, 60.0, 83.0);
			SetSkill(SkillName.Parry, 61.0, 93.0);
		}

		public override void InitSBInfo()
		{
			/*m_SBInfos.Add( new SBSmithTools() );

			m_SBInfos.Add( new SBMetalShields() );
			m_SBInfos.Add( new SBWoodenShields() );

			m_SBInfos.Add( new SBPlateArmor() );

			m_SBInfos.Add( new SBHelmetArmor() );
			m_SBInfos.Add( new SBChainmailArmor() );
			m_SBInfos.Add( new SBRingmailArmor() );
			m_SBInfos.Add( new SBAxeWeapon() );
			m_SBInfos.Add( new SBPoleArmWeapon() );
			m_SBInfos.Add( new SBRangedWeapon() );

			m_SBInfos.Add( new SBKnifeWeapon() );
			m_SBInfos.Add( new SBMaceWeapon() );
			m_SBInfos.Add( new SBSpearForkWeapon() );
			m_SBInfos.Add( new SBSwordWeapon() );*/

			m_SBInfos.Add(new SBBlacksmith(Expansion));
			if (IsTokunoVendor)
			{
				m_SBInfos.Add(new SBSEArmor());
				m_SBInfos.Add(new SBSEWeapons());
			}
		}

		public override VendorShoeType ShoeType { get { return VendorShoeType.None; } }

		public override void InitOutfit()
		{
			base.InitOutfit();

			Item item = (Utility.RandomBool() ? null : new RingmailChest());

			if (item != null && !EquipItem(item))
			{
				item.Delete();
				item = null;
			}

			if (item == null)
			{
				AddItem(new FullApron());
			}

			AddItem(new Bascinet());
			AddItem(new SmithHammerWeapon());
		}

		#region Bulk Orders
		public override Item CreateBulkOrder(Mobile from, bool fromContextMenu)
		{
			var pm = from as PlayerMobile;

			if (pm != null && pm.NextSmithBulkOrder <= TimeSpan.Zero && (fromContextMenu || 0.2 > Utility.RandomDouble()))
			{
				double theirSkill = pm.Skills[SkillName.Blacksmith].Base;

				if (theirSkill >= 120.0)
					pm.NextSmithBulkOrder = TimeSpan.FromHours(5.0);
				else
					pm.NextSmithBulkOrder = TimeSpan.FromHours(6.0);

				/*if ( theirSkill >= 85.1 )
					pm.NextSmithBulkOrder = TimeSpan.FromHours( 6.0 );
				else if ( theirSkill >= 70.1 )
					pm.NextSmithBulkOrder = TimeSpan.FromHours( 3.0 );
				else
					pm.NextSmithBulkOrder = TimeSpan.FromHours( 1.5 );*/
				
				if (theirSkill >= 75.1 && ((theirSkill - 75.0) / 250.0) > Utility.RandomDouble())
				{
					return new LargeSmithBOD();
				}

				return SmallSmithBOD.CreateRandomFor(from);
			}

			return null;
		}

		public override bool IsValidBulkOrder(Item item)
		{
			return (item is SmallSmithBOD || item is LargeSmithBOD);
		}

		public override bool SupportsBulkOrders(Mobile from)
		{
			Faction fact = Faction.Find(from);
			return (FactionAllegiance == null || fact == null || FactionAllegiance == fact) &&
				   (from is PlayerMobile && from.Skills[SkillName.Blacksmith].Base > 55.0);
		}

		public override TimeSpan GetNextBulkOrder(Mobile from)
		{
			if (from is PlayerMobile)
			{
				return ((PlayerMobile)from).NextSmithBulkOrder;
			}

			return TimeSpan.Zero;
		}

		public override void OnSuccessfulBulkOrderReceive(Mobile from, Item dropped)
		{
			var pm = from as PlayerMobile;

			if (pm != null)
			{
				//if ( /*Core.AOS ||*/ dropped is LargeSmithBOD )
				//always reset
				pm.NextSmithBulkOrder = TimeSpan.FromHours(PseudoSeerStone.HoursBetweenBODs);
				/*else
				{
					pm.NextSmithBulkOrder -= TimeSpan.FromHours( 2.0 );
					if ( pm.NextSmithBulkOrder < TimeSpan.Zero )
						pm.NextSmithBulkOrder = TimeSpan.Zero;
				}*/
			}
		}
		#endregion

		public Blacksmith(Serial serial)
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
	}
}