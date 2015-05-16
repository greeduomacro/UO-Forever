#region References
using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using VitaNex.FX;

#endregion

namespace Server.Items
{
	public class BeetleDung : Item
	{
		private Timer _Timer;

        public IrradiatedBeetlePortal Beetle { get; private set; }

		public override bool HandlesOnMovement { get { return true; } }

		[Constructable]
		public BeetleDung(IrradiatedBeetlePortal beetle)
			: base(4655)
		{
            Beetle = beetle;

			Movable = false;
			Name = "beetle dung";

			Hue = 1167;

			_Timer = Timer.DelayCall(TimeSpan.FromSeconds(30), Explode);
		}

        public BeetleDung(Serial serial)
			: base(serial)
		{ }

		private void Explode()
		{
            if (!Deleted && Beetle != null && !Beetle.Deleted && Beetle.Alive)
			{
                BaseExplodeEffect e = ExplodeFX.Fire.CreateInstance(
                    Location, Map, 2, 0, TimeSpan.FromMilliseconds(1000 - ((10) * 100)), null, () =>
                    {
                        foreach (Mobile mobile in AcquireAllTargets(Location, 2))
                        {
                            if (mobile is BaseCreature && mobile.IsControlled())
                            {
                                mobile.Damage(500);
                            }
                            else if (mobile is PlayerMobile)
                            {
                                mobile.Damage(40);
                            }
                        }
                    });
                e.Send();
			}

			Delete();
		}

		public override void OnMovement(Mobile m, Point3D oldLocation)
		{
			base.OnMovement(m, oldLocation);

            if (m == null || m.Deleted || m == Beetle || !m.Alive || !m.InRange3D(this, 1, -5, 5) ||
                (Beetle != null && !Beetle.CanBeHarmful(m)))
			{
				return;
			}

            if (Beetle != null)
			{
                Beetle.DoHarmful(m, true);
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
                m.ApplyPoison(Beetle ?? m, Poison.Deadly);
			}
			else if (m is BaseCreature)
			{
                m.ApplyPoison(Beetle ?? m, Poison.Lethal);
			}
		}

        public List<Mobile> AcquireAllTargets(Point3D p, int range)
        {
            return
                p.GetMobilesInRange(Map, range)
                    .Where(
                        m =>
                            m != null && !m.Deleted && m.Alive &&
                            (m.Player || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile && !m.IsDeadBondedPet)))
                    .ToList();
        }

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

            Beetle = null;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(1);
			
			switch (version)
			{
				case 1:
					writer.Write(Beetle);
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
                    Beetle = reader.ReadMobile<IrradiatedBeetlePortal>();
					goto case 0;
				case 0:
					break;
			}

			_Timer = Timer.DelayCall(TimeSpan.FromSeconds(30), Explode);
		}
	}
}