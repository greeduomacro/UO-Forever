#region References
using System;
using System.Linq;

using Server;
using Server.Mobiles;

using VitaNex.FX;
#endregion

namespace VitaNex.Modules.AutoPvP.Battles
{
	public class UOF_CTFPodium : Item
	{
		[CommandProperty(AutoPvP.Access)]
		public virtual UOF_CTFTeam Team { get; set; }

		public override bool HandlesOnMovement { get { return true; } }

		public UOF_CTFPodium(UOF_CTFTeam team)
			: base(16144)
		{
			Team = team;

			Name = Team.Name;
			Hue = Team.Color;
			Movable = false;
		}

		public UOF_CTFPodium(Serial serial)
			: base(serial)
		{ }

		public override void OnDoubleClick(Mobile from)
		{
			if (this.CheckDoubleClick(from, true, false, 2))
			{
				CheckCapture(from as PlayerMobile);
			}
		}

		public override void OnMovement(Mobile m, Point3D oldLocation)
		{
			base.OnMovement(m, oldLocation);

			CheckCapture(m as PlayerMobile);
		}

		public virtual void CheckCapture(PlayerMobile attacker)
		{
			if (attacker != null && !attacker.Deleted && attacker.InRange3D(this, 1, -5, 5) && Team != null && !Team.Deleted &&
				Team.Flag != null && !Team.Flag.Deleted && Team.Flag.Carrier == null && Team.IsMember(attacker))
			{
				Team.Battle.Teams.OfType<UOF_CTFTeam>()
					.Where(
						t =>
						t != Team && !t.Deleted && t.Flag != null && !t.Flag.Deleted && t.Flag.Carrier != null &&
						t.Flag.Carrier == attacker)
					.ForEach(
						t =>
						{
							t.Flag.Capture(attacker);
							ExplodeFX.Random.CreateInstance(this, Map, 3, 0, null, e => e.Hue = t.Color).Send();
						});
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}
}