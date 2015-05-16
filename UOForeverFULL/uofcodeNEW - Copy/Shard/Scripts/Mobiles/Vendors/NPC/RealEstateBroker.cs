#region References
using System;
using System.Collections.Generic;

using Server.Engines.XmlSpawner2;
using Server.Factions;
using Server.Items;
using Server.Misc;
using Server.Multis.Deeds;
using Server.Network;
using Server.Targeting;
#endregion

namespace Server.Mobiles
{
	public class RealEstateBroker : BaseVendor
	{
		private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

		[Constructable]
		public RealEstateBroker()
			: base("the real estate broker")
		{ }

		public override bool HandlesOnSpeech(Mobile from)
		{
			if (from.Alive && from.InRange(this, 3))
			{
				return true;
			}

			return base.HandlesOnSpeech(from);
		}

		private DateTime m_NextCheckPack;

		public override void OnMovement(Mobile m, Point3D oldLocation)
		{
			if (!TestCenter.Enabled && DateTime.UtcNow > m_NextCheckPack && InRange(m, 4) && !InRange(oldLocation, 4) && m.Player)
			{
				Container pack = m.Backpack;

				if (pack != null)
				{
					m_NextCheckPack = DateTime.UtcNow + TimeSpan.FromSeconds(2.0);

					Item deed = pack.FindItemByType(typeof(HouseDeed), false);

					if (deed != null)
					{
						// If you have a deed, I can appraise it or buy it from you...
						PrivateOverheadMessage(MessageType.Regular, 0x3B2, 500605, m.NetState);

						// Simply hand me a deed to sell it.
						PrivateOverheadMessage(MessageType.Regular, 0x3B2, 500606, m.NetState);
					}
				}
			}

			base.OnMovement(m, oldLocation);
		}

		public override void OnSpeech(SpeechEventArgs e)
		{
			if (XmlScript.HasTrigger(this, TriggerName.onSpeech) &&
				UberScriptTriggers.Trigger(this, e.Mobile, TriggerName.onSpeech, null, e.Speech))
			{
				return;
			}

			Mobile m = e.Mobile;
			Faction fact = Faction.Find(m);

			if (!e.Handled && m.Alive && e.HasKeyword(0x38)) // *appraise*
			{
				if (FactionAllegiance != null && fact != null && FactionAllegiance != fact)
				{
					Say("I will not do business with the enemy!");
				}
				else if (!TestCenter.Enabled)
				{
					PublicOverheadMessage(MessageType.Regular, 0x3B2, 500608); // Which deed would you like appraised?
					m.BeginTarget(12, false, TargetFlags.None, Appraise_OnTarget);
					e.Handled = true;
				}
			}

			base.OnSpeech(e);
		}

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			// trigger returns true if returnoverride
			if (XmlScript.HasTrigger(this, TriggerName.onDragDrop) &&
				UberScriptTriggers.Trigger(this, from, TriggerName.onDragDrop, dropped))
			{
				return true;
			}

			if (!TestCenter.Enabled && dropped is HouseDeed)
			{
				Faction fact = Faction.Find(from);
				
				if (FactionAllegiance != null && fact != null && FactionAllegiance != fact)
				{
					Say("I will not do business with the enemy!");
				}
				else
				{
					var deed = (HouseDeed)dropped;
					int price = ComputePriceFor(deed);

					if (price > 0)
					{
						if (Banker.Deposit(from, TypeOfCurrency, price))
						{
							Say("For the deed I have placed {0} in your bank box : {1:#,0}", TypeOfCurrency.Name, price);

							deed.Delete();
							return true;
						}
						
						Say(500390); // Your bank box is full.
						return false;
					}
					
					Say(500607); // I'm not interested in that.
					return false;
				}
			}

			return base.OnDragDrop(from, dropped);
		}

		public void Appraise_OnTarget(Mobile from, object obj)
		{
			if (obj is HouseDeed)
			{
				var deed = (HouseDeed)obj;
				int price = ComputePriceFor(deed);

				if (price > 0)
				{
					Say("I will pay you {0} for this deed : {1:#,0}", TypeOfCurrency.Name, price);
					
					Say(500610); // Simply hand me the deed if you wish to sell it.
				}
				else
				{
					Say(500607); // I'm not interested in that.
				}
			}
			else
			{
				Say(500609); // I can't appraise things I know nothing about...
			}
		}

		public int ComputePriceFor(HouseDeed deed)
		{
			int price = 0;

			if (deed is SmallBrickHouseDeed || deed is StonePlasterHouseDeed || deed is FieldStoneHouseDeed ||
				deed is WoodHouseDeed || deed is WoodPlasterHouseDeed || deed is ThatchedRoofCottageDeed)
			{
				price = 43800;
			}
			else if (deed is BrickHouseDeed)
			{
				price = 144500;
			}
			else if (deed is TwoStoryWoodPlasterHouseDeed || deed is TwoStoryStonePlasterHouseDeed)
			{
				price = 132400;
			}
			else if (deed is TowerDeed)
			{
				price = 433200;
			}
			else if (deed is KeepDeed)
			{
				price = 2665200;
			}
			else if (deed is CastleDeed)
			{
				price = 1022800;
			}
			else if (deed is LargePatioDeed)
			{
				price = 152800;
			}
			else if (deed is LargeMarbleDeed)
			{
				price = 192800;
			}
			else if (deed is SmallTowerDeed)
			{
				price = 88500;
			}
			else if (deed is LogCabinDeed)
			{
				price = 97800;
			}
			else if (deed is SandstonePatioDeed)
			{
				price = 90900;
			}
			else if (deed is VillaDeed)
			{
				price = 136500;
			}
			else if (deed is StoneWorkshopDeed)
			{
				price = 60600;
			}
			else if (deed is MarbleWorkshopDeed)
			{
				price = 60300;
			}

			return AOS.Scale(price, 50); // refunds 50% of the purchase price
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add(new SBRealEstateBroker());
		}

		public RealEstateBroker(Serial serial)
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