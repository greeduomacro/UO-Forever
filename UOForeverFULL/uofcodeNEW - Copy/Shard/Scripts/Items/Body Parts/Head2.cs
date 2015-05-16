#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Engines.MurderSystem;
using Server.Factions;
using Server.Mobiles;
using Server.Regions;
using VitaNex;
#endregion

namespace Server.Items
{
	public class Head2 : Item, IBodyPart
	{
		private static readonly List<Head2> _AllHeads;
		private static readonly PollTimer[] _ExpireTimers;
		private static bool _Processing;

		public static List<Head2> AllHeads { get { return _AllHeads; } }

		static Head2()
		{
			_AllHeads = new List<Head2>();
			_ExpireTimers = new PollTimer[10];
		}

		public static void Initialize()
		{
			if (_ExpireTimers.All(t => t == null))
			{
				_ExpireTimers.SetAll(i => PollTimer.FromMinutes(1.0, () => ProcessHeads(i), () => AllHeads.Count > i * 5000));
			}
			else
			{
				_ExpireTimers.For(i => _ExpireTimers[i].Running = true);
			}
		}

		private static void ProcessHeads(int i)
		{
			if (_Processing)
			{
				return;
			}

			_Processing = true;

			VitaNexCore.TryCatch(
				() =>
				{
					DateTime now = DateTime.UtcNow;

					int skip = i * 5000;
					int take = i == _ExpireTimers.Length - 1 ? AllHeads.Count - skip : (1 + i) * 5000;

					foreach (var h in AllHeads.AsParallel().Skip(skip).Take(take).Where(h => h.ExpireDate <= now).ToArray())
					{
						h.Expire();
						AllHeads.Remove(h);
					}

					AllHeads.RemoveAll(h => h == null || h.Deleted);
				},
				x => x.ToConsole(true));

			_Processing = false;
		}

		private PlayerMobile _Owner;
		private PlayerMobile _Player;
		private HeadType _TypeOf;
		private string _HeadName;

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastOffer { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public PlayerMobile Owner
		{
			get { return _Owner; }
			set
			{
				if (_Owner == value)
				{
					return;
				}

				_Owner = value;

				if (_Owner == null || Expired)
				{
					AllHeads.Remove(this);
				}
				else if (!AllHeads.Contains(this))
				{
					AllHeads.Add(this);
				}

				InvalidateName();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public PlayerMobile Player
		{
			get { return _Player; }
			set
			{
				_Player = value;
				InvalidateName();
			}
		}

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsTown { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public string HeadName
		{
			get { return _HeadName; }
			set
			{
				_HeadName = value;
				InvalidateName();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public HeadType TypeOf
		{
			get { return _TypeOf; }
			set
			{
				_TypeOf = value;
				InvalidateName();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime ExpireDate
		{
			get { return _ExpireDate; }
			set
			{
				_ExpireDate = value;

				if (_ExpireDate <= DateTime.UtcNow)
				{
					return;
				}

				_ExpireProcessed = false;
				InvalidateName();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan ExpireTime { get { return TimeSpan.FromMilliseconds(Math.Max(0.0, (ExpireDate - DateTime.UtcNow).TotalMilliseconds)); } }

		[CommandProperty(AccessLevel.GameMaster)]
        public bool Expired { get
        {
            return ExpireDate <= DateTime.UtcNow;
        } }

		private bool _ExpireProcessed;
		private DateTime _ExpireDate;

		public override string DefaultName { get { return GetName(); } }

		[Constructable]
		public Head2()
			: this(String.Empty)
		{ }

		[Constructable]
		public Head2(string name)
			: base(7393)
		{
			Weight = 2.0;
			HeadName = name;
			ExpireDate = DateTime.UtcNow + TimeSpan.FromHours(10.0);
		}

		public Head2(PlayerMobile m, Region obtained)
			: base(7393)
		{
			Weight = 2.0;
			Player = m;
			ExpireDate = DateTime.UtcNow + TimeSpan.FromHours(10.0);
		    if (obtained is TownRegion)
		    {
		        IsTown = true;
		    }
		}

		public Head2(Serial serial)
			: base(serial)
		{ }

		public void InvalidateName()
		{
			Name = GetName();
		}

		public string GetName()
		{
			if (String.IsNullOrWhiteSpace(_HeadName) && _Player != null)
			{
				_HeadName = _Player.RawName;
			}

			if (String.IsNullOrWhiteSpace(_HeadName))
			{
				return String.Format("a{0}head", Expired ? " decayed " : " ");
			}

			string name = String.Format("the{0}head of {1}", Expired ? " decayed " : " ", _HeadName);

			switch (TypeOf)
			{
				case HeadType.Duel:
					name = String.Format("{0}, taken in a duel", name);
					break;
				case HeadType.Tournament:
					name = String.Format("{0}, taken in a tournament", name);
					break;
			}

			if (String.IsNullOrWhiteSpace(name))
			{
				name = String.Format("a{0}head", Expired ? " decayed " : " ");
			}

			return name;
		}

		public void Expire()
		{
			if (_ExpireProcessed)
			{
				return;
			}

			_ExpireProcessed = true;

			AllHeads.Remove(this);

			Hue = 568;
			InvalidateName();
		}

		public override void OnAfterDelete()
		{
			if (!_Processing)
			{
				AllHeads.Remove(this);
			}

			base.OnAfterDelete();
		}

		public override void OnSingleClick(Mobile m)
		{
			if (m == null || m.Deleted || !m.CanSee(this) || _Player == null || _Player.Deleted || !(m is PlayerMobile))
			{
				base.OnSingleClick(m);
				return;
			}

			LabelToExpansion(m);

			if (Expired)
			{
				Expire();
				LabelTo(m, Name);
			}
			else
			{
				TimeSpan time = ExpireTime;

				LabelTo(m, Name);
				LabelTo(
					m,
					"Decaying: {0} Hour{1}, {2} Minute{3}",
					time.Hours,
					time.Hours != 1 ? "s" : String.Empty,
					time.Minutes,
					time.Minutes != 1 ? "s" : String.Empty);
			}
		}

		public override void OnDoubleClick(Mobile m)
		{
			if (!this.CheckDoubleClick(m, true, false, 2, true) || _Player == null || _Player.Deleted || !(m is PlayerMobile))
			{
				base.OnDoubleClick(m);
				return;
			}

			if (Expired)
			{
				Expire();
				m.SendMessage("This head is so decomposed that you cannot discern the owners' identity.");
				return;
			}

			if (m.InCombat())
			{
				m.SendMessage("You cannot make an offer while in the heat of combat.");
				return;
			}

			if (_Player == m)
			{
				m.SendMessage("You cannot negotiate with yourself.");
				return;
			}

			if (_Player.InCombat())
			{
				m.SendMessage("That person is currently trying to keep their current head, maybe later...");
				return;
			}

			if (!_Player.IsOnline())
			{
				m.SendMessage("That person is currently not online.");
				return;
			}

			if (_Player.Kills < 5)
			{
				m.SendMessage("You can't negotiate with an innocent person for their head.");
				return;
			}

			DateTime now = DateTime.UtcNow;

			if ((now - LastOffer).TotalMinutes < 5.0)
			{
				m.SendMessage("You must wait a moment before you can make another offer.");
				return;
			}

			LastOffer = now;

			m.CloseGump(typeof(HeadNegotiateGump));
			_Player.CloseGump(typeof(HeadNegotiateGump));

			new KillerSellHeadGump((PlayerMobile)m, _Player, this).Send();
		}

		public override void OnAdded(object parent)
		{
			base.OnAdded(parent);

			Owner = RootParent as PlayerMobile;
		}

		public override void OnRemoved(object parent)
		{
			base.OnRemoved(parent);

			Owner = RootParent as PlayerMobile;
		}

		public override bool OnDragLift(Mobile m)
		{
			bool allow = base.OnDragLift(m);

			if (allow)
			{
				Owner = m as PlayerMobile;
			}

			return allow;
		}

		public override void OnSecureTrade(Mobile from, Mobile to, Mobile owner, bool accepted)
		{
			if (accepted)
			{
				Owner = owner as PlayerMobile;
			}

			base.OnSecureTrade(from, to, owner, accepted);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(6);

            // Version 6
            writer.Write(IsTown);
			// Version 5
			writer.Write(_Owner);

			// Version 4
			writer.Write(_ExpireDate);

			// Version 3
			writer.Write(_ExpireProcessed);
			writer.Write(_Player);
			writer.Write(_HeadName);
			writer.WriteEncodedInt((int)_TypeOf);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
                case 6:
			        IsTown = reader.ReadBool();
                    goto case 5;
				case 5:
					_Owner = reader.ReadMobile<PlayerMobile>();
					goto case 4;
				case 4:
					_ExpireDate = reader.ReadDateTime();
					goto case 3;
				case 3:
					{
						_ExpireProcessed = reader.ReadBool();
						_Player = reader.ReadMobile<PlayerMobile>();
						_HeadName = reader.ReadString();
						_TypeOf = (HeadType)reader.ReadEncodedInt();
					}
					goto case 2;
				case 2:
				case 1:
				case 0:
					break;
			}

			if (_Player != null && TypeOf == HeadType.Regular && !Expired)
			{
				AllHeads.Add(this);
			}
		}
	}
}