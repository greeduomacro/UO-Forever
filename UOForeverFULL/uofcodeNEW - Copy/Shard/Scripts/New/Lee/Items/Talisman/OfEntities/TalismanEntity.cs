using System;

using Server.Mobiles;
using Server.Network;

using VitaNex.FX;

namespace Server.Items
{
	[CorpseName("Remains of a Talisman Entity")]
	public class TalismanEntity : VanityCreature
	{
		[CommandProperty(AccessLevel.GameMaster, true)]
		public TalismanOfEntities Link { get; private set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Linked
		{
			get
			{
				if (Link == null)
				{
					return false;
				}

				if (Link.Deleted)
				{
					Link = null;
					return false;
				}

				return true;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				if (base.Name == value)
				{
					return;
				}

				base.Name = value;

				if (Linked)
				{
					Link.EntityName = base.Name;
				}
			}
		}

		[Hue, CommandProperty(AccessLevel.GameMaster)]
		public override int Hue
		{
			get
			{
				return base.Hue;
			}
			set
			{
				if (base.Hue == value)
				{
					return;
				}

				base.Hue = value;

				if (Linked)
				{
					Link.EntityHue = base.Hue;
				}
			}
		}

		[Body, CommandProperty(AccessLevel.GameMaster)]
		public override int BodyValue
		{
			get
			{
				return base.BodyValue;
			}
			set
			{
				if (base.BodyValue == value)
				{
					return;
				}

				base.BodyValue = value;

				if (Linked)
				{
					Link.EntityBody = base.BodyValue;
				}
			}
		}

		public TalismanEntity(Mobile owner, TalismanOfEntities talisman)
			: base(AIType.AI_Animal)
		{
			if (talisman == null && owner != null)
			{
				talisman = owner.FindItemOnLayer<TalismanOfEntities>(Layer.Talisman) ??
						   owner.Backpack.FindItemByType<TalismanOfEntities>(true, i => !i.Linked);
			}

			if (owner == null && talisman != null)
			{
				owner = talisman.RootParent as Mobile;
			}

			if (talisman != null)
			{
				Link = talisman;

				Name = talisman.EntityName;
				BodyValue = talisman.EntityBody;
				Hue = talisman.EntityHue;
			}
			else
			{
				var eType = TalismanOfEntities.RandomEntity();

				Name = eType.ToString().SpaceWords().ToLower();
				BodyValue = (int)eType;
				Hue = Utility.RandomAnimalHue();
			}

			if (owner == null)
			{
				return;
			}

			SetControlMaster(owner);

			IsBonded = true;

			ControlTarget = owner;
			ControlOrder = OrderType.Come;

			Female = owner.Female;
			Fame = owner.Fame;
			Karma = owner.Karma;
		}
		
		public TalismanEntity(Serial serial)
			: base(serial)
		{ }

		public override void OnThink()
		{
			base.OnThink();

			if (Deleted)
			{
				return;
			}

			if (!Linked || IsStabled || ControlMaster == null || ControlMaster.Map == null || ControlMaster.Map == Map.Internal ||
				ControlMaster.NetState == null)
			{
				Delete();
				return;
			}

			Map = ControlMaster.Map;

			if (!InRange(ControlMaster, RangePerception))
			{
				SetLocation(ControlMaster.Location, true);
			}
		}

		public override void OnAosSingleClick(Mobile m)
		{
			OnSingleClick(m);
		}

		public override void OnSingleClick(Mobile m)
		{
			base.OnSingleClick(m);

			if (m == null || m.Deleted || m.NetState == null)
			{
				return;
			}

			if (Linked && ControlMaster != null && !ControlMaster.Deleted)
			{
				PrivateOverheadMessage(MessageType.Label, 85, true, "Master: " + ControlMaster.RawName, m.NetState);
			}
		}

		public override void Delete()
		{
			if (!Deleted && Map != null && Map != Map.Internal)
			{
				PlaySound(491);

				new EnergyExplodeEffect(Location, Map, 2).Send();
			}

			Link = null;

			base.Delete();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			//writer.Write(Link);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			//Link = reader.ReadItem<TalismanOfEntities>();

			Delete();
		}
	}
}