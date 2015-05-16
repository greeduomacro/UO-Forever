#region References
using System;
using System.Collections.Generic;

using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
#endregion

namespace Server.Items
{
	public class HouseTeleporter : Item, ISecurable
	{
		[CommandProperty(AccessLevel.GameMaster)]
		public Item Target { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public SecureLevel Level { get; set; }

		[Constructable]
		public HouseTeleporter(int itemID)
			: this(itemID, null)
		{ }

		public HouseTeleporter(int itemID, Item target)
			: base(itemID)
		{
			Movable = false;

			Target = target;
			Level = SecureLevel.Anyone;
		}

		public HouseTeleporter(Serial serial)
			: base(serial)
		{ }

		public bool CheckAccess(Mobile m)
		{
			BaseHouse house = BaseHouse.FindHouseAt(this);

			if (house == null)
			{
				return false;
			}

			if (house.Public ? house.IsBanned(m) : !house.HasAccess(m))
			{
				return false;
			}

			return house.HasSecureAccess(m, Level);
		}

		public override bool OnMoveOver(Mobile m)
		{
			if (Target != null && !Target.Deleted)
			{
				if (CheckAccess(m))
				{
					if (!m.Hidden || m.AccessLevel < AccessLevel.Counselor)
					{
						new EffectTimer(Location, Map, 2023, 0x1F0, TimeSpan.FromSeconds(0.4)).Start();
					}

					new DelayTimer(this, m).Start();
				}
				else
				{
					m.SendLocalizedMessage(1061637); // You are not allowed to access this.
				}
			}

			return true;
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);

			SetSecureLevelEntry.AddTo(from, this, list);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version

			writer.Write((int)Level);

			writer.Write(Target);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 1:
					Level = (SecureLevel)reader.ReadInt();
					goto case 0;
				case 0:
					{
						Target = reader.ReadItem();

						if (version < 0)
						{
							Level = SecureLevel.Anyone;
						}
					}
					break;
			}
		}

		private class EffectTimer : Timer
		{
			private readonly Point3D _Location;
			private readonly Map _Map;
			private readonly int _EffectID;
			private readonly int _SoundID;

			public EffectTimer(Point3D p, Map map, int effectID, int soundID, TimeSpan delay)
				: base(delay)
			{
				_Location = p;
				_Map = map;
				_EffectID = effectID;
				_SoundID = soundID;
			}

			protected override void OnTick()
			{
				Effects.SendLocationParticles(
					EffectItem.Create(_Location, _Map, EffectItem.DefaultDuration), 0x3728, 10, 10, _EffectID, 0);

				if (_SoundID != -1)
				{
					Effects.PlaySound(_Location, _Map, _SoundID);
				}
			}
		}

		private class DelayTimer : Timer
		{
			private readonly HouseTeleporter _Teleporter;
			private readonly Mobile _Mobile;

			public DelayTimer(HouseTeleporter tp, Mobile m)
				: base(TimeSpan.FromSeconds(1.0))
			{
				_Teleporter = tp;
				_Mobile = m;
			}

			protected override void OnTick()
			{
				Item target = _Teleporter.Target;

				if (_Teleporter.Target == null || _Teleporter.Target.Deleted || _Mobile.Location != _Teleporter.Location ||
					_Mobile.Map != _Teleporter.Map)
				{
					return;
				}

				Point3D p = target.GetWorldTop();
				Map map = target.Map;

				BaseCreature.TeleportPets(_Mobile, p, map);

				_Mobile.MoveToWorld(p, map);

				if (!_Mobile.Hidden || _Mobile.AccessLevel < AccessLevel.Counselor)
				{
					Effects.PlaySound(target.Location, target.Map, 0x1FE);

					Effects.SendLocationParticles(
						EffectItem.Create(_Teleporter.Location, _Teleporter.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023, 0);
					Effects.SendLocationParticles(
						EffectItem.Create(target.Location, target.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023, 0);

					new EffectTimer(target.Location, target.Map, 2023, -1, TimeSpan.FromSeconds(0.4)).Start();
				}
			}
		}
	}
}