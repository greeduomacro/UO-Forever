#region References
using Server.Engines.CannedEvil;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Regions;
using Server.Targeting;
#endregion

namespace Server.Multis
{
	public abstract class BaseBoatDeed : Item
	{
		[CommandProperty(AccessLevel.GameMaster)]
		public int MultiID { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D Offset { get; set; }

		public BaseBoatDeed(int id, Point3D offset)
			: base(0x14F2)
		{
			Weight = 1.0;

			MultiID = id;
			Offset = offset;
			LootType = LootType.Blessed;
		}

		public BaseBoatDeed(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version

			writer.Write(MultiID);
			writer.Write(Offset);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						MultiID = reader.ReadInt();
						Offset = reader.ReadPoint3D();

						break;
					}
			}

			if (Weight == 0.0)
			{
				Weight = 1.0;
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (!IsChildOf(from.Backpack))
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
			}
			else if (from.AccessLevel < AccessLevel.GameMaster && (from.Map == Map.Ilshenar || from.Map == Map.Malas))
			{
				from.SendLocalizedMessage(1010567, null, 0x25); // You may not place a boat from this location.
			}
			else
			{
				if (EraSE)
				{
					from.SendLocalizedMessage(502482); // Where do you wish to place the ship?
				}
				else
				{
					from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502482); // Where do you wish to place the ship?
				}

				from.Target = new InternalTarget(this);
			}
		}

		public abstract BaseBoat Boat { get; }

		public void OnPlacement(Mobile from, Point3D p)
		{
			if (Deleted)
			{
				return;
			}

			if (!IsChildOf(from.Backpack))
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
			}
			else
			{
				Map map = from.Map;

				if (map == null)
				{
					return;
				}

				if (from.AccessLevel < AccessLevel.GameMaster && (map == Map.Ilshenar || map == Map.Malas))
				{
					from.SendLocalizedMessage(1043284); // A ship can not be created here.
					return;
				}

				if (from.Region.IsPartOf(typeof(HouseRegion)) || BaseBoat.FindBoatAt(from, from.Map) != null)
				{
					// You may not place a ship while on another ship or inside a house.
					from.SendLocalizedMessage(1010568, null, 0x25);
					return;
				}

				BaseBoat boat = Boat;

				if (boat == null)
				{
					return;
				}

				p = new Point3D(p.X - Offset.X, p.Y - Offset.Y, p.Z - Offset.Z);

				if (BaseBoat.IsValidLocation(p, map) && boat.CanFit(p, map, boat.ItemID))
				{
					Delete();

					boat.Owner = from;
					boat.Anchored = true;

					uint keyValue = boat.CreateKeys(from);

					if (boat.PPlank != null)
					{
						boat.PPlank.KeyValue = keyValue;
					}

					if (boat.SPlank != null)
					{
						boat.SPlank.KeyValue = keyValue;
					}

					foreach (BoatComponent component in boat.BoatComponents)
					{
						if (component is BoatRope)
						{
							((BoatRope)component).KeyValue = keyValue;
						}
					}

					boat.MoveToWorld(p, map);
					from.SendGump(
						new WarningGump(
							1060637,
							30720,
							"!!! WARNING WARNING WARNING WARNING !!!\nBoats are SINKABLE by using cannons. When a boat has sustained significant damage, it will also be BOARDABLE (the planks or ropes will open to anybody). This means anything that you have on the boat is at risk! The owner of a sunken boat can (through a deed that appears in their bank) either pay a ransom to those who destroyed the boat OR risk venturing out to repair the boat themselves, in which case they have a certain time window to do so or else it can be repaired and captured by anybody else (and therefore LOST TO THEM). Boat deeds and dry-docked boats ARE blessed.",
							0xFFC000,
							320,
							240,
							null,
							null));
				}
				else
				{
					boat.Delete();
					from.SendLocalizedMessage(1043284); // A ship can not be created here.
				}
			}
		}

		private class InternalTarget : MultiTarget
		{
			private readonly BaseBoatDeed m_Deed;

			public InternalTarget(BaseBoatDeed deed)
				: base(deed.MultiID, deed.Offset, 14, true, TargetFlags.None)
			{
				m_Deed = deed;
			}

			protected override void OnTarget(Mobile from, object o)
			{
				var ip = o as IPoint3D;

				if (ip != null)
				{
					if (ip is Item)
					{
						ip = ((Item)ip).GetWorldTop();
					}

					var p = new Point3D(ip);

					Region region = Region.Find(p, from.Map);

						m_Deed.OnPlacement(from, p);
					
				}
			}
		}
	}
}