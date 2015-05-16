#region References
using System;

using Server.Mobiles;
#endregion

namespace Server.Items
{
	public enum HeadType
	{
		Regular,
		Duel,
		Tournament
	}

	public interface IBodyPart
	{ }

	public class Head : Item, IBodyPart
	{
		private PlayerMobile _Player;
		private string _HeadName;
		private HeadType _TypeOf;

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

		public override string DefaultName { get { return GetName(); } }

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
				return "a head";
			}

			string name = String.Format("the head of {0}", _HeadName);

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
				name = "a head";
			}

			return name;
		}

		[Constructable]
		public Head()
			: this(String.Empty)
		{ }

		[Constructable]
		public Head(string name)
			: base(7584)
		{
			Weight = 2.0;
			HeadName = name;
		}

		public Head(PlayerMobile m)
			: base(7584)
		{
			Weight = 2.0;
			Player = m;
		}

		public Head(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(2); // version

			//v2
			writer.Write(_Player);

			//v1
			writer.Write(_HeadName);
			writer.WriteEncodedInt((int)_TypeOf);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 2:
					_Player = reader.ReadMobile<PlayerMobile>();
					goto case 1;
				case 1:
					{
						_HeadName = reader.ReadString();
						_TypeOf = (HeadType)reader.ReadEncodedInt();
					}
					goto case 0;
				case 0:
					break;
			}
		}
	}
}