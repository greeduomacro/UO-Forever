#region References
using System;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

using VitaNex.FX;
#endregion

namespace VitaNex.Modules.AutoPvP.Battles
{
	public sealed class BoWBrazier : Item
	{
		private DateTime _LastClick = DateTime.UtcNow;
		private DateTime _LastFX = DateTime.UtcNow;

		private BoWTeam _Controller;

		[CommandProperty(AutoPvP.Access)]
		public BoWBattle Battle { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public int CurrentHealth { get; set; }

		public BoWBrazier(BoWBattle battle)
			: base(3633)
		{
			Name = "a magical brazier";
			Movable = false;
			Battle = battle;
		}

		public BoWBrazier(Serial serial)
			: base(serial)
		{ }

		public override void OnSingleClick(Mobile from)
		{
			LabelToExpansion(from);

			OwnedMessage();
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (Battle == null || Battle.Deleted || !this.CheckDoubleClick(from, true, false, 2) || !(from is PlayerMobile))
			{
				return;
			}

			var pm = (PlayerMobile)from;

			if (!Battle.IsParticipant(pm))
			{
				pm.SendMessage("You must be a participant to perform that action.");
				return;
			}

			if (_LastClick > _LastClick + TimeSpan.FromSeconds(2))
			{
				pm.SendMessage("You cannot attack that again so soon!");
				return;
			}

			if (pm.FindItemOnLayer(Layer.TwoHanded) is BaseWeapon || pm.FindItemOnLayer(Layer.OneHanded) is BaseWeapon)
			{
				Calcdamage(pm, 2);

				pm.Direction = Direction;

				SwingAnimation(pm);

				_LastClick = DateTime.UtcNow;
			}
			else
			{
				pm.SendMessage("If you want to assault the brazier, you must have a weapon equipped!");
			}
		}

		public void OwnedMessage()
		{
			if (_Controller == null)
			{
				PublicOverheadMessage(MessageType.Regular, 2049, false, "Unowned: ");
			}
			else
			{
				switch (_Controller.Name)
				{
					case "The Council of Mages":
						PublicOverheadMessage(MessageType.Regular, 1325, false, "The Council of Mages: ");
						break;
					case "Minax":
						PublicOverheadMessage(MessageType.Regular, 1645, false, "Minax: ");
						break;
					case "The True Britannians":
						PublicOverheadMessage(MessageType.Regular, 1287, false, "The True Britannians: ");
						break;
					case "The Shadowlords":
						PublicOverheadMessage(MessageType.Regular, 1109, false, "The Shadowlords: ");
						break;
				}
			}

			PublicOverheadMessage(MessageType.Regular, 33, false, CurrentHealth + "/10");
		}

		public void Calcdamage(Mobile m, int damage)
		{
			if (Battle == null || Battle.Deleted)
			{
				return;
			}

			if (!(m is PlayerMobile))
			{
				return;
			}

			var pm = (PlayerMobile)m;
			var team = Battle.FindTeam(pm) as BoWTeam;

			if (team == null)
			{
				return;
			}

			if (!Battle.IsParticipant(pm))
			{
				pm.SendMessage("You must be a participant to perform that action.");
				return;
			}

			if (_Controller != team)
			{
				if (CurrentHealth - damage > 0)
				{
					CurrentHealth -= damage;
				}
				else if (CurrentHealth - damage == 0)
				{
					CurrentHealth = 0;
					Hue = 0;

					if (_Controller.Multi > 1)
					{
						_Controller.Multi--;
					}

					_Controller = null;
				}
				else if (CurrentHealth - damage < 0)
				{
					Battle.LocalBroadcast("{0} have taken control of a magical brazier!", team.Name);
					Hue = team.Color;
					CurrentHealth = damage - CurrentHealth;

					Battle.EnsureStatistics(pm)["Brazier Captures"]++;

					if (_Controller != null && _Controller.Multi > 1)
					{
						_Controller.Multi--;
					}

					_Controller = team;
					_Controller.Multi++;
				}

				OwnedMessage();
			}
			else if (_Controller == team)
			{
				if (CurrentHealth + damage < 10)
				{
					CurrentHealth += damage;
				}
				else
				{
					CurrentHealth = 10;

					if (_LastFX + TimeSpan.FromSeconds(20) <= DateTime.UtcNow)
					{
						BaseSpecialEffect e = SpecialFX.FirePentagram.CreateInstance(
							Location, Map, 5, 0, TimeSpan.FromMilliseconds(1000 - ((10 - 1) * 100)));
						e.Send();
						_LastFX = DateTime.UtcNow;
					}
				}

				OwnedMessage();
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
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
					break;
			}
		}

		#region SwingAnimation
		public void SwingAnimation(Mobile m)
		{
			if (m == null || m.Weapon == null || !(m.Weapon is BaseWeapon))
			{
				return;
			}

			int action;
			WeaponAnimation animation = ((BaseWeapon)m.Weapon).Animation;

			switch (m.Body.Type)
			{
				case BodyType.Sea:
				case BodyType.Animal:
					action = Utility.Random(5, 2);
					break;
				case BodyType.Monster:
					{
						switch (animation)
						{
							default:
								action = Utility.Random(4, 3);
								break;
						}
					}
					break;
				case BodyType.Human:
					{
						if (!m.Mounted)
						{
							action = (int)animation;
						}
						else
						{
							switch (animation)
							{
								default:
									action = 26;
									break;
								case WeaponAnimation.Bash2H:
								case WeaponAnimation.Pierce2H:
								case WeaponAnimation.Slash2H:
									action = 29;
									break;
								case WeaponAnimation.ShootBow:
									action = 27;
									break;
								case WeaponAnimation.ShootXBow:
									action = 28;
									break;
							}
						}
					}
					break;
				default:
					return;
			}

			m.Animate(action, 7, 1, true, false, 0);
		}
		#endregion
	}
}