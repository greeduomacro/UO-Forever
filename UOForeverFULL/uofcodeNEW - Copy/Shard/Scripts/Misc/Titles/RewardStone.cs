#region References
using System;
using System.Collections.Generic;

using Server.Commands;
using Server.Mobiles;
using Server.Targeting;
#endregion

namespace Server.Scripts.Engines.ValorSystem
{
	[TypeAlias("Server.Scripts.Misc.Titles.RewardController")]
	public class ValorRewardController : Item
	{
		private static ValorRewardController _mInstance;

		public static ValorRewardController Instance { get { return _mInstance; } }

		public static TitleCollection MTitle;
		private ValorItemCollection _mValorItems;
		public Dictionary<int, string> Categories { get; private set; }

		[Constructable]
		public ValorRewardController()
			: base(0xEDC)
		{
			Name = "Rewards Controller";
			Movable = false;
			Visible = false;
			MTitle = new TitleCollection();
			_mValorItems = new ValorItemCollection();
			Categories = new Dictionary<int, string>
			{
				{0, "Titles"}
			};

			if (_mInstance != null)
			{
				// there can only be one RewardController game stone in the world
				_mInstance.Location = Location;
				CommandHandlers.BroadcastMessage(
					AccessLevel.GameMaster, 0x489, "Existing Title Controller has been moved to this location (DON'T DELETE IT!).");
				Timer.DelayCall(TimeSpan.FromSeconds(1), UpdateInstancePosition, this);
			}
			else
			{
				_mInstance = this;
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (from.AccessLevel >= AccessLevel.GameMaster && from is PlayerMobile)
			{
				from.SendGump(new ValorTitleRewardAdmin(this, null, 0, 0, null, null));
			}
			else
			{
				from.SendMessage("Sorry, but you don't have permission access this.");
			}
			base.OnDoubleClick(from);
		}

		public class AddRewardTarget : Target
		{
			private readonly ValorRewardController _IValorRewardController;

			public AddRewardTarget()
				: base(2, false, TargetFlags.None)
			{ }

			public AddRewardTarget(ValorRewardController valorRewardController)
				: this()
			{
				_IValorRewardController = valorRewardController;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				var i = targeted as Item;

				if (_IValorRewardController == null || i == null)
				{
					return;
				}

				from.SendGump(new ValorTitleItemEdit(_IValorRewardController, null, new ValorItem(i), true));
			}
		}

		public static void UpdateInstancePosition(ValorRewardController attemptedConstruct)
		{
			if (attemptedConstruct == null)
			{
				return;
			}
			if (_mInstance == null) // should never happen, but if it does, make this the instance
			{
				_mInstance = attemptedConstruct;
			}
			else if (attemptedConstruct.Location != new Point3D(0, 0, 0))
				// move the instance to it's location and delete it
			{
				_mInstance.Location = attemptedConstruct.Location;
				attemptedConstruct.Delete();
			}
		}

		public ValorRewardController(Serial serial)
			: base(serial)
		{ }

		public override void Delete()
		{
			// can't delete it!
		}

		public class TitleCollection : List<ValorTitleItem>
		{ }

		public int TitleCount { get { return MTitle.Count; } }

		public TitleCollection GetTitles { get { return MTitle; } }

		public void AddTitle(ValorTitleItem r)
		{
			MTitle.Add(r);
		}

		public class ValorItemCollection : List<ValorItem>
		{ }

		public ValorItemCollection GetValorItems { get { return _mValorItems; } set { _mValorItems = value; } }

		public void AddValorItem(ValorItem r, Mobile from)
		{
			_mValorItems.Add(r);
		}

		public int ValorItemCount { get { return _mValorItems.Count; } }

		#region Serialize/Deserialize
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(2); //version

			if (Categories == null)
			{
				writer.Write(0);
			}
			else
			{
				writer.Write(Categories.Count);

				foreach (KeyValuePair<int, string> kvp in Categories)
				{
					writer.Write(kvp.Key);
					writer.Write(kvp.Value);
				}
			}
			int count = _mValorItems.Count;
			writer.Write(count);
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					writer.WriteItem(_mValorItems[i]);
				}
			}
			count = MTitle.Count;
			writer.Write(count);
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					writer.WriteItem(MTitle[i]);
				}
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			MTitle = new TitleCollection();
			_mValorItems = new ValorItemCollection();
			switch (version)
			{
				case 2:
					{
						int categoryCount = reader.ReadInt();

						if (categoryCount > 0)
						{
							Categories = new Dictionary<int, string>();

							for (int i = 0; i < categoryCount; i++)
							{
								int num = reader.ReadInt();
								string title = reader.ReadString();
								Categories.Add(num, title);
							}
						}
						goto case 1;
					}
				case 1:
					{
						int count = reader.ReadInt();
						if (count > 0)
						{
							for (int i = 0; i < count; i++)
							{
								var r = (ValorItem)reader.ReadItem();
								_mValorItems.Add(r);
							}
						}
						goto case 0;
					}
				case 0:
					{
						int count = reader.ReadInt();
						if (count > 0)
						{
							for (int i = 0; i < count; i++)
							{
								var r = (ValorTitleItem)reader.ReadItem();
								MTitle.Add(r);
							}
						}
					}
					_mInstance = this;
					break;
			}
		}
		#endregion
	}
}