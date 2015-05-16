#region References
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Server.Mobiles
{
	[CorpseName("blood of the hydra")]
	public class BloodoftheHydra : BaseCreature
	{
		public HydraMotM Hydra { get; private set; }

		public override bool Unprovokable { get { return true; } }
		public override bool Uncalmable { get { return true; } }

		[Constructable]
		public BloodoftheHydra(HydraMotM hydra)
			: base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.3, 0.3)
		{
			Hydra = hydra;

			if (Hydra != null)
			{
				Hydra.AddBlood(this);
			}

			Name = "Blood of the Hydra";
			Body = 0x33;
			BaseSoundID = 1006;
			Hue = 0x58B;

			SetStr(496, 520);
			SetDex(181, 205);
			SetInt(136, 260);

            SetHits(100000, 100000);

			SetDamage(1, 1);

			SetSkill(SkillName.Anatomy, 90.0);
			SetSkill(SkillName.MagicResist, 70.0);
			SetSkill(SkillName.Tactics, 90.0);
			SetSkill(SkillName.Wrestling, 90.0);

			Fame = 0;
			Karma = 0;

			VirtualArmor = 500;
		}

		public BloodoftheHydra(Serial serial)
			: base(serial)
		{ }

		public override void Attack(Mobile m)
		{
			if (m is EnergyVortex)
			{
				m.Damage(50, this);
			}

			BeginFlee(TimeSpan.FromSeconds(30));

			base.Attack(m);
		}

		public override void OnThink()
		{
			base.OnThink();
		
			if (Hydra != null && (Hydra.Deleted || !Hydra.Alive))
			{
				Hydra = null;
			}
		}

		public override void OnMovement(Mobile m, Point3D oldLocation)
		{
			BeginFlee(TimeSpan.FromSeconds(30));

			foreach (BaseCreature pet in
				AcquireAllTargets(Location, 50).OfType<BaseCreature>().Where(c => c.GetMaster<PlayerMobile>() != null))
			{
				pet.Attack(this);
			}

			base.OnMovement(m, oldLocation);
		}

		public override bool OnMoveOver(Mobile m)
		{
			if (m is PlayerMobile)
			{
				m.SendMessage(2966, "You stepped on a Blood of the Hydra!");
				m.ApplyPoison(this, Poison.Lethal);
				m.Damage(25);

				Kill();
			}

			return base.OnMoveOver(m);
		}

		public override void OnDamage(int amount, Mobile from, bool willKill)
		{
			BeginFlee(TimeSpan.FromSeconds(30));

			base.OnDamage(amount, from, willKill);
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			if (Hydra == null)
			{
				return;
			}

			Hydra.RemoveBlood(this);
			Hydra = null;
		}

		public List<Mobile> AcquireAllTargets(Point3D p, int range)
		{
			return
				p.GetMobilesInRange(Map, range)
				 .Where(
					 m =>
					 m != null && !m.Deleted && m.Alive &&
					 (m.Player || (m is BaseCreature && ((BaseCreature)m).GetMaster<PlayerMobile>() != null && !m.IsDeadBondedPet)))
				 .ToList();
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
		}
	}
}