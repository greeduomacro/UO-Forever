#region References
using System;
using System.Linq;

using Server.Misc;
using Server.Mobiles;
using Server.Network;

using VitaNex.FX;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace Server.Items
{
	// Name = Body
	public enum TalismanEntityType
	{
		None = 0,
		Ant = 28,
		Ape = 29,
		Bull = 233,
		Bunny = 205,
		Cat = 201,
		Centaur = 101,
		Chicken = 208,
		Cow = 231,
		Daemon = 9,
		Deer = 237,
		Dog = 217,
		Dragon = 59,
		Drake = 61,
		Goat = 209,
		Imp = 74,
		Minotaur = 299,
		MongBat = 39,
		Ogre = 1,
		Orc = 17,
		Panther = 214,
		Pig = 203,
		Pixie = 128,
		Rat = 238,
		Scorpion = 48,
		Sheep = 207,
		Slime = 51,
		Snake = 52,
		Spider = 11,
		Succubus = 149,
		Walrus = 221,
		Wisp = 58,

		Wolf = 225
	}

	public class TalismanOfEntities : DecoTalisman
	{
		private static readonly TalismanEntityType[] _EntityTypeTypes =
			TalismanEntityType.None.GetValues<TalismanEntityType>().Not(t => t == TalismanEntityType.None).ToArray();

		public static TalismanEntityType RandomEntity()
		{
			return _EntityTypeTypes.GetRandom();
		}

		[CommandProperty(AccessLevel.GameMaster, true)]
		public TalismanEntity Link { get; private set; }

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

		private InputDialogGump _NameInput;

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Renaming { get { return _NameInput != null && _NameInput.IsOpen; } }

		private string _EntityName;

		[CommandProperty(AccessLevel.GameMaster)]
		public string EntityName
		{
			get { return _EntityName; }
			set
			{
				if (_EntityName == value)
				{
					return;
				}

				_EntityName = value;

				if (Linked)
				{
					Link.Name = _EntityName;
				}
			}
		}

		private int _EntityBody;

		[Body, CommandProperty(AccessLevel.GameMaster)]
		public int EntityBody
		{
			get { return _EntityBody; }
			set
			{
				if (_EntityBody == value)
				{
					return;
				}

				_EntityBody = value;

				if (Linked)
				{
					Link.Body = _EntityBody;
				}
			}
		}

		private int _EntityHue;

		[Hue, CommandProperty(AccessLevel.GameMaster)]
		public int EntityHue
		{
			get { return _EntityHue; }
			set
			{
				if (_EntityHue == value)
				{
					return;
				}

				_EntityHue = value;

				if (Linked)
				{
					Link.Hue = _EntityHue;
				}
			}
		}

		public override string DefaultName { get { return "Talisman Of Entities"; } }

		[Constructable]
		public TalismanOfEntities()
			: this(RandomEntity())
		{ }

		[Constructable]
		public TalismanOfEntities(TalismanEntityType type)
			: this((int)type)
		{ }

		[Constructable]
		public TalismanOfEntities(int entityBody)
			: base(12120)
		{
			EntityBody = entityBody;
		}

		public TalismanOfEntities(Serial serial)
			: base(serial)
		{ }

		public override bool CheckLift(Mobile m, Item item, ref LRReason reject)
		{
			if (m == null)
			{
				return false;
			}

			if (m.AccessLevel <= AccessLevel.Counselor && Renaming)
			{
				m.SendMessage(0x22, "You can not move this talisman while the entity is being named.");
				reject = LRReason.CannotLift;
				return false;
			}

			return base.CheckLift(m, item, ref reject);
		}

		public override bool OnDragLift(Mobile player)
		{
			if (player == null)
			{
				return false;
			}

			if (player.AccessLevel <= AccessLevel.Counselor && Renaming)
			{
				player.SendMessage(0x22, "You can not move this talisman while the entity is being named.");
				return false;
			}

			return true;
		}

		public override void OnParentDeleted(object parent)
		{
			DismissEntity();

			base.OnParentDeleted(parent);
		}

		public override DeathMoveResult OnParentDeath(Mobile parent)
		{
			DismissEntity();

			return base.OnParentDeath(parent);
		}

		public override void OnMapChange()
		{
			base.OnMapChange();

			if (Map == null || Map == Map.Internal)
			{
				DismissEntity();
			}
		}

		public override void OnAdded(object parent)
		{
			base.OnAdded(parent);

			if (!Linked)
			{
				return;
			}

			if (!(RootParent is Mobile))
			{
				DismissEntity();
				return;
			}

			if (Link.ControlMaster == RootParent)
			{
				return;
			}

			Link.ControlMaster = Link.ControlTarget = (Mobile)RootParent;
			Link.ControlOrder = OrderType.Come;
		}

		public override void OnRemoved(object parent)
		{
			base.OnRemoved(parent);

			if (!Linked)
			{
				return;
			}

			if (!(RootParent is Mobile))
			{
				DismissEntity();
				return;
			}

			if (Link.ControlMaster == RootParent)
			{
				return;
			}

			Link.ControlMaster = Link.ControlTarget = (Mobile)RootParent;
			Link.ControlOrder = OrderType.Come;
		}

		public override void OnSingleClick(Mobile m)
		{
			base.OnSingleClick(m);

			if (EntityBody <= 0)
			{
				EntityBody = (int)RandomEntity();
			}

			LabelTo(
				m, 85, "Use: {0} {1}", Linked ? "Dismiss" : "Summon", !String.IsNullOrWhiteSpace(EntityName) ? EntityName : "Entity");
		}

		public override void OnDoubleClick(Mobile m)
		{
			if (!this.CheckDoubleClick(m, true, false, -1) || !(m is PlayerMobile))
			{
				return;
			}

			if (RootParent != m)
			{
				m.SendMessage(0x22, "You must own this talisman before you can summon its entity.");
				return;
			}

			if (Renaming)
			{
				m.SendMessage(0x22, "You can not use this talisman while the entity is being named.");
				return;
			}

			if (Linked)
			{
				DismissEntity();
				return;
			}

			if (EntityBody <= 0)
			{
				EntityBody = (int)RandomEntity();
			}

			if (String.IsNullOrWhiteSpace(EntityName))
			{
				BeginRename((PlayerMobile)m);
			}
			else
			{
				SummonEntity((PlayerMobile)m);
			}
		}

		protected void BeginRename(PlayerMobile m)
		{
			if (m == null || m.Deleted)
			{
				return;
			}

			_NameInput = SuperGump.Send(
				new InputDialogGump(m)
				{
					Limit = 16,
					Title = "Name Your Entity",
					Html =
						"Give your talisman entity a name!\n\n" +
						"Name length must be between 1 and 16 letters and contain no invalid characters, numbers, or white-space.\n\n" +
						"When you're done, click OK to summon your entity!",
					AcceptHandler = b => _NameInput = null,
					CancelHandler = b =>
					{
						_NameInput = null;

						m.SendMessage("You decide not to name your entity.");
					},
					Callback = (b, t) =>
					{
						if (VerifyName(m, t, true) != NameResultMessage.Allowed)
						{
							return;
						}

						EntityName = t;

						SummonEntity(m);
					}
				});
		}

		protected NameResultMessage VerifyName(PlayerMobile m, string name, bool message)
		{
			if (m == null || m.Deleted)
			{
				return NameResultMessage.NotAllowed;
			}

			if (String.IsNullOrWhiteSpace(name))
			{
				if (message)
				{
					m.SendMessage("The name you chose is too short.");
				}

				return NameResultMessage.TooFewCharacters;
			}

			string kw;

			if (AntiAdverts.Detect(name, out kw))
			{
				if (message)
				{
					m.SendMessage("The name you chose is not allowed.");
				}

				return NameResultMessage.NotAllowed;
			}

			NameResultMessage res = NameVerification.ValidatePlayerName(
				name,
				1,
				16,
				true,
				false,
				true,
				0,
				NameVerification.Empty,
				NameVerification.DisallowedWords,
				NameVerification.StartDisallowed,
				NameVerification.DisallowedAnywhere);

			switch (res)
			{
				case NameResultMessage.InvalidCharacter:
					{
						if (message)
						{
							m.SendMessage("The name you chose contains invalid characters.");
						}
					}
					return res;
				case NameResultMessage.NotAllowed:
					{
						if (message)
						{
							m.SendMessage("The name you chose is not allowed.");
						}
					}
					return res;
				case NameResultMessage.TooFewCharacters:
					{
						if (message)
						{
							m.SendMessage("The name you chose is too short.");
						}
					}
					return res;
				case NameResultMessage.TooManyCharacters:
					{
						if (message)
						{
							m.SendMessage("The name you chose is too long.");
						}
					}
					return res;
			}

			if (ProfanityProtection.DisallowedWords.Any(t => name.IndexOf(t, StringComparison.OrdinalIgnoreCase) != -1))
			{
				// That name isn't very polite.
				m.SendLocalizedMessage(1072622);

				return NameResultMessage.NotAllowed;
			}

			return NameResultMessage.Allowed;
		}

		public void SummonEntity(PlayerMobile m)
		{
			DismissEntity();

			Link = new TalismanEntity(m, this);

			new EnergyExplodeEffect(m.Location, m.Map, 2).Send();

			Link.MoveToWorld(m.Location, m.Map);
			Link.PlaySound(491);
		}

		public void DismissEntity()
		{
			if (!Linked)
			{
				return;
			}

			Link.Delete();
			Link = null;
		}

		public override void Delete()
		{
			if (!Deleted)
			{
				DismissEntity();
			}

			base.Delete();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.Write(Link);
			writer.Write(EntityName);
			writer.Write(EntityBody);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			Link = reader.ReadMobile<TalismanEntity>();
			EntityName = reader.ReadString();
			EntityBody = reader.ReadInt();
		}
	}
}