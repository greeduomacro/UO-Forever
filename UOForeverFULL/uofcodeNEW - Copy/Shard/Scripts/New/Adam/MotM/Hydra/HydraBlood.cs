#region References
using System;

using Server.Mobiles;
#endregion

namespace Server.Items
{
	public class HydraBlood : Item
	{
		private Timer _Timer;

		public HydraMotM Hydra { get; private set; }

		public override bool HandlesOnMovement { get { return true; } }

		[Constructable]
		public HydraBlood(HydraMotM hydra)
			: base(4655)
		{
			Hydra = hydra;

			Movable = false;
			Name = "poisonous hydra blood";

			Hue = 1167;

			_Timer = Timer.DelayCall(TimeSpan.FromSeconds(30), Coagulate);
		}

		public HydraBlood(Serial serial)
			: base(serial)
		{ }

		private void Coagulate()
		{
			if (!Deleted && Hydra != null && !Hydra.Deleted && Hydra.Alive)
			{
				new BloodoftheHydra(Hydra).MoveToWorld(Location, Map);
			}

			Delete();
		}

		public override void OnMovement(Mobile m, Point3D oldLocation)
		{
			base.OnMovement(m, oldLocation);

			if (m == null || m.Deleted || m == Hydra || !m.Alive || !m.InRange3D(this, 1, -5, 5) ||
				(Hydra != null && !Hydra.CanBeHarmful(m)))
			{
				return;
			}

			if (Hydra != null)
			{
				Hydra.DoHarmful(m, true);
			}

			if (m is BaseCreature && m.IsControlled())
			{
				m.Damage(25);
			}
			else if (m is PlayerMobile)
			{
				m.Damage(1);
			}

			if (m.Poisoned)
			{
				return;
			}

			if (m is PlayerMobile)
			{
				m.ApplyPoison(Hydra ?? m, Poison.Lesser);
			}
			else if (m is BaseCreature)
			{
				m.ApplyPoison(Hydra ?? m, Poison.Lethal);
			}
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			Hydra = null;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(1);
			
			switch (version)
			{
				case 1:
					writer.Write(Hydra);
					goto case 0;
				case 0:
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 1:
					Hydra = reader.ReadMobile<HydraMotM>();
					goto case 0;
				case 0:
					break;
			}

			_Timer = Timer.DelayCall(TimeSpan.FromSeconds(30), Coagulate);
		}
	}
}