#region References
using Server;
using Server.Mobiles;
using Server.Network;
#endregion

namespace VitaNex.Modules.AutoPvP.Battles
{
	public class BoWWinPortal : Item
	{
		[CommandProperty(AutoPvP.Access)]
		public BoWBattle Battle { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public string Controller { get; set; }

		public BoWWinPortal(BoWBattle battle)
			: base(3948)
		{
			Name = "a dimensional gateway";
			Movable = false;
			Battle = battle;
			Hue = 1266;
			DoesNotDecay = true;
		}

		public BoWWinPortal(Serial serial)
			: base(serial)
		{ }

		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);

			if (Controller == null)
			{
				PublicOverheadMessage(MessageType.Regular, 2049, false, "Unowned");
			}
			else
			{
				switch (Controller)
				{
					case "The Council of Mages":
						PublicOverheadMessage(MessageType.Regular, 1325, false, "Owned by: The Council of Mages ");
						break;
					case "Minax":
						PublicOverheadMessage(MessageType.Regular, 1645, false, "Owned by: Minax ");
						break;
					case "The True Britannians":
						PublicOverheadMessage(MessageType.Regular, 1287, false, "Owned by: The True Britannians ");
						break;
					case "The Shadowlords":
						PublicOverheadMessage(MessageType.Regular, 1109, false, "Owned by: The Shadowlords ");
						break;
				}
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			SendCheck(from);
		}

		public override bool OnMoveOver(Mobile from)
		{
			SendCheck(from);
			return true;
		}

		public void SendCheck(Mobile m)
		{
			var pm = m as PlayerMobile;

			if (pm != null && pm.FactionName != null)
			{
				if (pm.FactionName == "LORD BRITISH" && Controller != "The True Britannians")
				{
					PublicOverheadMessage(MessageType.Regular, 2049, false, "You must join: " + Controller + " to use this portal.");
				}
				else if (pm.FactionName == "Minax" && Controller != "MINAX")
				{
					PublicOverheadMessage(MessageType.Regular, 2049, false, "You must join: " + Controller + " to use this portal.");
				}
				else if (pm.FactionName == "COUNCIL OF MAGES" && Controller != "The Council of Mages")
				{
					PublicOverheadMessage(MessageType.Regular, 2049, false, "You must join: " + Controller + " to use this portal.");
				}
				else if (pm.FactionName == "SHADOWLORDS" && Controller != "The Shadowlords")
				{
					PublicOverheadMessage(MessageType.Regular, 2049, false, "You must join: " + Controller + " to use this portal.");
				}
				else
				{
					Effects.SendIndividualFlashEffect(m, (FlashType)2);
					m.MoveToWorld(new Point3D(1363, 1105, -26), Map.Ilshenar);
				}
			}
			else if (pm != null && pm.FactionName == null)
			{
				PublicOverheadMessage(MessageType.Regular, 2049, false, "You must join: " + Controller + " to use this portal.");
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.Write(Controller);
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
				case 0:
					Controller = reader.ReadString();
					break;
			}
		}
	}
}