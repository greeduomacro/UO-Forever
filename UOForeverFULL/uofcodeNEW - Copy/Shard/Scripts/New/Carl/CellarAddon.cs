#region References
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Regions;

using VitaNex;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
using VitaNex.Targets;
#endregion

namespace Server.Items
{
	//Initially separated by a factor of 100 to leave insertion spaces, feel free to use any slots in between.
	//After adding a new entry, create the style info for it below and register it like the others.
	public enum CellarStyle
	{
		None = 0,
		Grass = 100,
		Dirt = 200,
		FlagStone = 300,
		SandFlagStone = 301,
		Stone = 400,
		DarkStone = 500,
		SandStone = 501,
		Marble = 600,
		Brick = 601,
		Timber = 700,
		Crystal = 701,
		Jade = 702,
		Dungeon = 900,
		Blood = 1000
	}

	public static class CellarStyles
	{
		public static List<CellarStyleInfo> Styles { get; private set; }

		public static CellarStyleInfo GetInfo(this CellarStyle style)
		{
			return Styles.FirstOrDefault(s => s.Style == style);
		}

		static CellarStyles()
		{
			Styles = new List<CellarStyleInfo>();

			CellarStyleInfo dirt = new CellarStyleInfo(CellarStyle.Dirt, "Dirt");
			dirt.SetFloor(12788, 12789, 12790, 12791, 12792, 12793, 12794, 12795);
			dirt.SetWall(3215, 3216, 3217, 3218);
			dirt.SetStairs(17621, 2212);
			Styles.Add(dirt);

			CellarStyleInfo grass = new CellarStyleInfo(CellarStyle.Grass, "Grass");
			grass.SetFloor(6013, 6014, 6015, 6016, 6017);
			grass.SetWall(3215, 3216, 3217, 3218);
			grass.SetStairs(17621, 2213);
			Styles.Add(grass);

			CellarStyleInfo dungeon = new CellarStyleInfo(CellarStyle.Dungeon, "Dungeon");
			dungeon.SetFloor(1339, 1340, 1341, 1342, 1343);
			dungeon.SetWall(1955);
			dungeon.SetStairs(17621, 1956);
			Styles.Add(dungeon);

			CellarStyleInfo flags = new CellarStyleInfo(CellarStyle.FlagStone, "Flagstone");
			flags.SetFloor(1276, 1277, 1278, 1279);
			flags.SetWall(1872);
			flags.SetStairs(17621, 1873);
			Styles.Add(flags);

			CellarStyleInfo sandFlags = new CellarStyleInfo(CellarStyle.SandFlagStone, "Sand Flagstone");
			sandFlags.SetFloor(1327, 1328, 1329, 1330);
			sandFlags.SetWall(1900);
			sandFlags.SetStairs(17621, 1901);
			Styles.Add(sandFlags);

			CellarStyleInfo sand = new CellarStyleInfo(CellarStyle.SandStone, "Sand Stone");
			sand.SetFloor(1181, 1182, 1183, 1184);
			sand.SetWall(1900);
			sand.SetStairs(17621, 1901);
			Styles.Add(sand);

			CellarStyleInfo stone = new CellarStyleInfo(CellarStyle.Stone, "Stone");
			stone.SetFloor(1305, 1306, 1307, 1308);
			stone.SetWall(1822);
			stone.SetStairs(17621, 1823);
			Styles.Add(stone);

			CellarStyleInfo darkStone = new CellarStyleInfo(CellarStyle.DarkStone, "Dark Stone");
			darkStone.SetFloor(1313, 1314, 1315, 1316);
			darkStone.SetWall(1928);
			darkStone.SetStairs(17621, 1929);
			Styles.Add(darkStone);

			CellarStyleInfo marble = new CellarStyleInfo(CellarStyle.Marble, "Marble");
			marble.SetFloor(1297, 1298, 1299, 1300);
			marble.SetWall(1801);
			marble.SetStairs(17621, 1802);
			Styles.Add(marble);

			CellarStyleInfo brick = new CellarStyleInfo(CellarStyle.Brick, "Brick");
			brick.SetFloor(1250, 1251, 1252, 1253, 1254, 1255, 1256, 1257);
			brick.SetWall(1822);
			brick.SetStairs(17621, 1823);
			Styles.Add(brick);

			CellarStyleInfo timber = new CellarStyleInfo(CellarStyle.Timber, "Timber");
			timber.SetFloor(1193, 1194, 1195, 1196);
			timber.SetWall(1848);
			timber.SetStairs(17621, 1849);
			Styles.Add(timber);

			CellarStyleInfo crystal = new CellarStyleInfo(CellarStyle.Crystal, "Crystal");
			crystal.SetFloor(13751, 13752, 13753, 13754, 13755, 13756, 13757);
			crystal.SetWall(13778);
			crystal.SetStairs(17621, 13780);
			Styles.Add(crystal);

			CellarStyleInfo jade = new CellarStyleInfo(CellarStyle.Jade, "Jade");
			jade.SetFloor(16815, 16815, 16816, 16817);
			jade.SetWall(19207);
			jade.SetStairs(17621, 19205);
			Styles.Add(jade);

			CellarStyleInfo blood = new CellarStyleInfo(CellarStyle.Blood, "Blood");
			blood.SetFloor(2760);
			blood.SetWall(8700);
			blood.SetStairs(17621, 1979);
			Styles.Add(blood);
		}

		public static CellarStyle ResolveOldID(int itemID)
		{
			switch (itemID)
			{
				case 0x177D:
					return CellarStyle.Grass;
				case 0x31F4:
					return CellarStyle.Dirt;
				case 0x04FF:
					return CellarStyle.FlagStone;
				case 0x051A:
					return CellarStyle.Stone;
				case 0x0521:
					return CellarStyle.DarkStone;
				case 0x04B8:
					return CellarStyle.Timber;
				default:
					return CellarStyle.None;
			}
		}
	}

	public sealed class CellarStyleInfo
	{
		public CellarStyle Style { get; private set; }
		public string Name { get; private set; }

		public int[] FloorTiles { get; set; }
		public int[] WallTiles { get; set; }

		public int StairsDown { get; set; }
		public int StairsUp { get; set; }

		public CellarStyleInfo(CellarStyle style, string name)
		{
			Style = style;
			Name = name;

			FloorTiles = new int[0];
			WallTiles = new int[0];
		}

		public void SetStairs(int down, int up)
		{
			StairsDown = down;
			StairsUp = up;
		}

		public void SetFloor(params int[] itemIDs)
		{
			FloorTiles = itemIDs ?? new int[0];
		}

		public void SetWall(params int[] itemIDs)
		{
			WallTiles = itemIDs ?? new int[0];
		}
	}

	public interface ICellarComponent
	{
		bool Deleted { get; }
		bool CommandDelete { get; set; }

		Point3D Offset { get; set; }

		Point3D Location { get; set; }
		Map Map { get; set; }

		void Delete();
	}

	[TypeAlias("Server.Items.CellarTeleporter3", "Server.Items.CellarTeleporter4")]
	public sealed class CellarStairs : AddonComponent, ICellarComponent, ISecurable
	{
		private SecureLevel _Level = SecureLevel.Anyone;

		[CommandProperty(AccessLevel.Counselor, true)]
		public CellarStairs Link { get; set; }

		[CommandProperty(AccessLevel.Counselor)]
		public SecureLevel Level
		{
			get
			{
				if (Link != null && _Level != Link._Level)
				{
					_Level = Link._Level;
				}

				return _Level;
			}
			set
			{
				if (_Level == value)
				{
					return;
				}

				_Level = value;

				if (Link != null)
				{
					Link._Level = _Level;
				}
			}
		}

		public CellarStairs(int itemID)
			: base(itemID)
		{
			Movable = false;
			CommandDelete = false;
			UpdateItemData();
		}

		public CellarStairs(Serial serial)
			: base(serial)
		{
			UpdateItemData();
		}

		private void UpdateItemData()
		{
			var data = ItemData;
			var flags = data.Flags;

			flags &= ~TileFlag.Impassable;
			flags &= ~TileFlag.Wall;
			flags |= TileFlag.Surface;
			flags |= TileFlag.Bridge;
			flags |= TileFlag.NoShoot;

			ItemData = new ItemData(Name, flags, data.Weight, data.Quality, data.Quantity, data.Value, data.Height);
		}

		public void LinkWith(CellarStairs other)
		{
			if (Link == other || other == null || other == this)
			{
				return;
			}

			other.Link = this;
			Link = other;
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);

			SetSecureLevelEntry.AddTo(from, this, list);
		}

		public override void OnSingleClick(Mobile m)
		{
			base.OnSingleClick(m);

			var house = BaseHouse.FindHouseAt(this);

			if (house == null || !house.IsOwner(m))
			{
				return;
			}

			m.CloseGump(typeof(SetSecureLevelGump));
			m.SendGump(new SetSecureLevelGump(m, this, house));
		}

		public override void OnDoubleClick(Mobile m)
		{
            var house = BaseHouse.FindHouseAt(this);

            if (house == null || !house.HasSecureAccess(m, Level))
            {
                return;
            }

			if (Link != null && !Link.Deleted && Link.Map != null && Link.Map != Map.Internal && Link.Location != Point3D.Zero &&
				m.Region.IsPartOf(Region.Find(GetWorldLocation(), Map)) && this.CheckDoubleClick(m, true, true, 1))
			{
				m.MoveToWorld(Link.Location, Link.Map);
			}
		}

		public override bool OnMoveOver(Mobile m)
		{
			var house = BaseHouse.FindHouseAt(this);

            if (house == null || !house.HasSecureAccess(m, Level))
            {
                return false;
            }

			if (Link != null && !Link.Deleted && Link.Map != null && Link.Map != Map.Internal && Link.Location != Point3D.Zero &&
				m != null && !m.Deleted)
			{
				m.MoveToWorld(Link.Location, Link.Map);
			}

			return false;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(3);

			switch (version)
			{
				case 3:
				case 2:
					writer.WriteFlag(_Level);
					goto case 1;
				case 1:
					writer.Write(Link);
					goto case 0;
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
				case 3:
					{
						if (version < 3)
						{
							CommandDelete = false;
						}
					}
					goto case 2;
				case 2:
					_Level = reader.ReadFlag<SecureLevel>();
					goto case 1;
				case 1:
					{
						var link = reader.ReadItem<CellarStairs>();

						if (Link == null || Link.Deleted)
						{
							Link = link;
						}
					}
					goto case 0;
				case 0:
					break;
			}
		}
	}

	public sealed class CellarFloor : AddonComponent, ICellarComponent
	{
		private CellarAddon Cellar { get; set; }

		public CellarFloor(int itemID)
			: base(itemID)
		{
			Name = "Cellar Floor";
			Movable = false;
			CommandDelete = false;
			UpdateItemData();
		}

		public CellarFloor(Serial serial)
			: base(serial)
		{
			UpdateItemData();
		}

		private void UpdateItemData()
		{
			var data = ItemData;
			var flags = data.Flags;

			flags &= ~TileFlag.Impassable;
			flags &= ~TileFlag.Wall;
			flags |= TileFlag.Surface;

			ItemData = new ItemData(Name, flags, data.Weight, data.Quality, data.Quantity, data.Value, data.Height);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(1);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			if (version < 1)
			{
				CommandDelete = false;
			}
		}
	}

	public sealed class CellarWall : AddonComponent, ICellarComponent
	{
		private CellarAddon Cellar { get; set; }

		public CellarWall(int itemID)
			: base(itemID)
		{
			Name = "Cellar Wall";
			Movable = false;
			CommandDelete = false;
			UpdateItemData();
		}

		public CellarWall(Serial serial)
			: base(serial)
		{
			UpdateItemData();
		}

		private void UpdateItemData()
		{
			var data = ItemData;
			var flags = data.Flags;

			flags &= ~TileFlag.Surface;
			flags |= TileFlag.Impassable;
			flags |= TileFlag.NoShoot;
			flags |= TileFlag.Wall;

			ItemData = new ItemData(Name, flags, data.Weight, data.Quality, data.Quantity, data.Value, data.Height);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(1);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			
			int version = reader.GetVersion();

			if (version < 1)
			{
				CommandDelete = false;
			}
		}
	}

	[TypeAlias("Server.Items.CellarAddon2")]
	public class CellarAddon : BaseAddon
	{
		protected sealed class CellarPlaceholder : AddonComponent, ICellarComponent
		{
			public CellarPlaceholder(int itemID)
				: base(itemID)
			{
				Name = "Cellar Placeholder";
				Movable = false;
			}

			public CellarPlaceholder(Serial serial)
				: base(serial)
			{ }

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
		
		public static bool CanPlace(Mobile m, Point3D p, out BaseHouse house, bool message = true)
		{
			house = null;

			if (m == null || p == Point3D.Zero)
			{
				return false;
			}

			HouseRegion r = m.Region.GetRegion<HouseRegion>();

			if (r == null || r.House == null || r.House.Deleted || r.House.Map == null || r.House.Map == Map.Internal)
			{
				if (message)
				{
					m.SendMessage(0x22, "You must be inside a house to build a cellar.");
				}

				return false;
			}

			house = r.House;

			if (house.Addons.OfType<CellarAddon>().Any())
			{
				if (message)
				{
					m.SendMessage(0x22, "There is already a cellar in this house.");
				}

				return false;
			}

			if (!house.Region.Contains(p))
			{
				if (message)
				{
					m.SendMessage(0x22, "You may only build a cellar inside a house.");
				}

				return false;
			}

			// 10z allows for the ground floor tile height, house foundation surface tops are typically z + 7
			if (Math.Abs(house.Z - p.Z) > 10)
			{
				if (message)
				{
					m.SendMessage(0x22, "You may only build a cellar on the first floor of a house.");
				}

				return false;
			}

			if (m.AccessLevel >= AccessLevel.GameMaster)
			{
				return true;
			}

			if (!house.IsOwner(m))
			{
				if (message)
				{
					m.SendMessage(0x22, "Only the house owner may build a cellar.");
				}

				return false;
			}

			if (!house.Map.CanFit(p.X, p.Y, p.Z, 20, true, false, true))
			{
				if (message)
				{
					m.SendMessage(0x22, "The cellar can't be built here because something is blocking the location.");
				}

				return false;
			}

			return true;
		}

		public override BaseAddonDeed Deed { get { return new CellarDeed(_Style); } }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public CellarStyle Style
		{
			get { return _Style; }
			set
			{
				if (_Style == value)
				{
					return;
				}

				_Style = value;

				if (_Style != CellarStyle.None)
				{
					UpdateComponents();
				}
			}
		}

		public void UpdateComponents()
		{
			var info = _Style.GetInfo();

			if (info == null)
			{
				return;
			}

			if (_FloorTiles.Count == 0 || _WallTiles.Count == 0)
			{
				if (House == null)
				{
					House = BaseHouse.FindHouseAt(_Placeholder as Item ?? this);
				}

				if (House != null)
				{
					ClearComponents(true);
					GenerateComponents();
				}
			}

			_FloorTiles.Where(s => s != null && !s.Deleted).ForEach(s => s.ItemID = info.FloorTiles.GetRandom());
			_WallTiles.Where(s => s != null && !s.Deleted).ForEach(s => s.ItemID = info.WallTiles.GetRandom());

			if (StairsDown != null)
			{
				StairsDown.ItemID = info.StairsDown;
			}

			if (StairsUp != null)
			{
				StairsUp.ItemID = info.StairsUp;
			}
		}

		[CommandProperty(AccessLevel.Counselor, true)]
		public BaseHouse House { get; private set; }

		[CommandProperty(AccessLevel.Counselor, true)]
		public CellarStairs StairsDown { get; private set; }

		[CommandProperty(AccessLevel.Counselor, true)]
		public CellarStairs StairsUp { get; private set; }

		private CellarPlaceholder _Placeholder;

		private List<CellarFloor> _FloorTiles;
		private List<CellarWall> _WallTiles;
		private CellarStyle _Style;

		private bool _Chopping;

		[Constructable]
		public CellarAddon()
			: this(CellarStyle.Dirt)
		{ }

		[Constructable]
		public CellarAddon(CellarStyle style)
		{
			_Style = style;
			_FloorTiles = new List<CellarFloor>();
			_WallTiles = new List<CellarWall>();
			_Placeholder = AddComponent(new CellarPlaceholder(0xF39), Point3D.Zero);
		}

		public CellarAddon(Serial serial)
			: base(serial)
		{ }

		public override void OnChop(Mobile m)
		{
			if (House == null || House.Deleted)
			{
				House = BaseHouse.FindHouseAt(this);
			}

			if (House == null || House.Deleted || _Chopping || !(m is PlayerMobile))
			{
				return;
			}

			if (!House.IsOwner(m))
			{
				m.SendMessage(0x22, "Only the house owner may demolish the cellar.");
				return;
			}

			_Chopping = true;

			ConfirmDialogGump g = null;

			g = new ConfirmDialogGump((PlayerMobile)m)
			{
				Title = "Demolish Cellar?",
				Html =
					String.Format(
						"Demolishing this cellar will cause all contents to be moved to {0}." + "\nClick OK to to demolish this cellar.",
						House is HouseFoundation ? "the moving crate" : "the cellar entrance").WrapUOHtmlColor(Color.OrangeRed),
				AcceptHandler = b =>
				{
					g = null;
					_Chopping = false;
					Demolish(m);
				},
				CancelHandler = b => _Chopping = false
			};

			g.Send();

			Timer.DelayCall(
				TimeSpan.FromSeconds(10.0),
				() =>
				{
					if (g == null || !g.IsOpen)
					{
						return;
					}

					m.SendMessage(0x22, "Your time to select an option has expired and the demolition has been cancelled.");
					g.Close(true);
					_Chopping = false;
				});
		}

		public override void Delete()
		{
			Demolish(null);
		}

		public override void OnAfterDelete()
		{ }

		public void Demolish(Mobile m)
		{
			if (House == null || House.Deleted)
			{
				House = BaseHouse.FindHouseAt(this);
			}

			Point3D loc = StairsDown.Location;
			Map map = StairsDown.Map;

			if (House != null)
			{
				House.Region.Area.Select(r => new Rectangle3D(r.Start.ToPoint3D(House.Z - 40), r.End.ToPoint3D(House.Z)))
					 .ForEach(
						 r =>
						 {
							 var entities = r.GetEntities(House.Map).Where(e => e != null && e.Z < House.Z).ToList();

							 entities.RemoveAll(e => e is ICellarComponent);

							 entities.OfType<Mobile>().ForEach(
								 e =>
								 {
									 e.MoveToWorld(House.BanLocation, House.Map);
									 entities.Remove(e);
								 });

							 entities.OfType<IAddon>().Not(e => e == this).ForEach(
								 e =>
								 {
									 var d = e.Deed;

									 if (d != null)
									 {
										 ((IEntity)e).Delete();
										 entities.Add(d);
									 }

									 entities.Remove((IEntity)e);
								 });

							 entities.OfType<Item>().ForEach(
								 e =>
								 {
									 House.Release(m, e);

									 if (House is HouseFoundation)
									 {
										 House.DropToMovingCrate(e);
									 }
									 else
									 {
										 e.MoveToWorld(loc, map);
									 }

									 entities.Remove(e);
								 });
						 });
			}

			ClearComponents(false);

			if (m != null)
			{
				base.OnChop(m);
			}
			else
			{
				base.Delete();
			}

			if (House != null)
			{
				House.Addons.Remove(this);
			}
		}

		public override void OnMapChange()
		{
			base.OnMapChange();

			GenerateComponents();
		}

		public override void OnLocationChange(Point3D oldLoc)
		{
			base.OnLocationChange(oldLoc);

			GenerateComponents();
		}

		public void ClearComponents(bool holdPlace)
		{
			var comp = Components.ToArray();

			_FloorTiles.Clear();
			_WallTiles.Clear();
			Components.Clear();

			foreach (var c in comp)
			{
				c.Addon = null;
				c.Delete();
			}

			if (holdPlace)
			{
				_Placeholder = AddComponent(new CellarPlaceholder(0xF39), Point3D.Zero);
			}
		}

		protected TComponent AddComponent<TComponent>(TComponent c, Point3D o) where TComponent : AddonComponent
		{
			AddComponent(c, o.X, o.Y, o.Z);
			return c;
		}

		public void GenerateComponents()
		{
			if (Deleted || Map == null || Map == Map.Internal || Location == Point3D.Zero)
			{
				return;
			}

			if (House == null || House.Deleted)
			{
				House = BaseHouse.FindHouseAt(this);
			}

			if (House == null || House.Deleted || House.Area == null || House.Area.Length == 0 || _Placeholder == null ||
				_Placeholder.Deleted)
			{
				House = null;
				return;
			}

			if (_Style == CellarStyle.None)
			{
				_Style = CellarStyle.Dirt;
			}

			Point3D p = _Placeholder.Location;
			Point3D o = _Placeholder.Offset;
			Point3D t = House.Location;

			Components.Remove(_Placeholder);

			_Placeholder.Addon = null;
			_Placeholder.Delete();
			_Placeholder = null;

			var s = Style.GetInfo();

			AddComponent(StairsDown = new CellarStairs(s.StairsDown), o);

			var floorPoints = new List<Point2D>();
			var wallPoints = new List<Point2D>();

			foreach (var r2d in House.Region.Area.Select(r3d => new Rectangle2D(r3d.Start, r3d.End)))
			{
				floorPoints.AddRange(r2d.EnumeratePoints().Not(floorPoints.Contains));
			}

			floorPoints.ForEach(
				f =>
				{
					var points = new[]
					{
						f.Clone2D(-1, -1), //nw
						f.Clone2D(0, -1), //n
						f.Clone2D(1, -1), //ne
						f.Clone2D(1), //e
						f.Clone2D(1, 1), //se
						f.Clone2D(0, 1), //s
						f.Clone2D(-1, 1), //sw
						f.Clone2D(-1) //w
					};

					wallPoints.AddRange(points.Not(floorPoints.Contains).Not(wallPoints.Contains));
				});

			foreach (var p3d in floorPoints.Select(p2d => p2d.ToPoint3D(t.Z - 40).Clone3D(-p.X, -p.Y, -p.Z)))
			{
				_FloorTiles.Add(AddComponent(new CellarFloor(s.FloorTiles.GetRandom()), p3d));
			}

			foreach (var p3d in wallPoints.Select(p2d => p2d.ToPoint3D(t.Z - 40).Clone3D(-p.X, -p.Y, -p.Z)))
			{
				_WallTiles.Add(AddComponent(new CellarWall(s.WallTiles.GetRandom()), p3d));
			}

			AddComponent(StairsUp = new CellarStairs(s.StairsUp), p.ToPoint3D(t.Z - 40).Clone3D(-p.X, -p.Y, -p.Z));

			StairsDown.LinkWith(StairsUp);

			if (!House.Addons.Contains(this))
			{
				House.Addons.Add(this);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
					{
						writer.Write(_Placeholder);
						writer.WriteItemList(_FloorTiles);
						writer.WriteItemList(_WallTiles);
						writer.WriteFlag(_Style);
						writer.Write(House);
						writer.Write(StairsDown);
						writer.Write(StairsUp);
					}
					goto case 0;
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
				case 1:
					{
						_Placeholder = reader.ReadItem<CellarPlaceholder>();
						_FloorTiles = reader.ReadStrongItemList<CellarFloor>();
						_WallTiles = reader.ReadStrongItemList<CellarWall>();
						_Style = reader.ReadFlag<CellarStyle>();
						House = reader.ReadItem<BaseHouse>();
						StairsDown = reader.ReadItem<CellarStairs>();
						StairsUp = reader.ReadItem<CellarStairs>();
					}
					goto case 0;
				case 0:
					{
						if (version < 1)
						{
							_FloorTiles = new List<CellarFloor>();
							_WallTiles = new List<CellarWall>();
						}
					}
					break;
			}
		}

		public static void Configure()
		{
			EventSink.WorldLoad += () =>
			{
				foreach (var c in World.Items.Values.OfType<CellarAddon>().ToArray())
				{
					c.InvalidateComponents();
				}
			};

			CommandUtility.Register(
				"Cellar",
				AccessLevel.Counselor,
				e =>
				{
					BaseHouse house = BaseHouse.FindHouseAt(e.Mobile);

					if (house == null)
					{
						e.Mobile.SendMessage("You must be inside a house to use that command.");
						return;
					}

					var cellar = house.Addons.OfType<CellarAddon>().FirstOrDefault();

					if (cellar == null)
					{
						e.Mobile.SendMessage("This house does not have a cellar.");
						return;
					}

					e.Mobile.SendGump(new PropertiesGump(e.Mobile, cellar));
				});
		}

		private void InvalidateComponents()
		{
			if (StairsDown == null || StairsUp == null)
			{
				var stairs = Components.OfType<CellarStairs>().OrderByDescending(s => s.Z).ToArray();

				if (stairs.Length > 1)
				{
					StairsDown = stairs.First();
					StairsUp = stairs.Last();

					StairsDown.Link = StairsUp;
					StairsUp.Link = StairsDown;
				}
			}
			else if (StairsDown.Link == null || StairsUp.Link == null)
			{
				StairsDown.Link = StairsUp;
				StairsUp.Link = StairsDown;
			}

			if (_Style == CellarStyle.None)
			{
				var tiles = new Queue<AddonComponent>(Components);

				while (_Style == CellarStyle.None && tiles.Count > 0)
				{
					var floor = tiles.Dequeue();

					if (floor != null)
					{
						_Style = CellarStyles.ResolveOldID(floor.ItemID);
					}
				}

				tiles.Clear();
			}

			if (_Style == CellarStyle.None)
			{
				_Style = CellarStyle.Dirt;
			}

			UpdateComponents();
		}
	}

	[TypeAlias("Server.Items.CellarDeed2")]
	public class CellarDeed : BaseAddonDeed
	{
		public override BaseAddon Addon { get { return new CellarAddon(Style); } }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public CellarStyle Style { get; set; }

		[Constructable]
		public CellarDeed()
			: this(CellarStyle.None)
		{ }

		[Constructable]
		public CellarDeed(CellarStyle style)
		{
			Style = style;

			Name = "Cellar Deed";
			LootType = LootType.Blessed;
		}

		public CellarDeed(Serial serial)
			: base(serial)
		{ }

		public override void OnSingleClick(Mobile m)
		{
			base.OnSingleClick(m);

			if (Style != CellarStyle.None)
			{
				LabelTo(m, "[Floor: {0}]", Style.ToString().SpaceWords());
			}
		}

		public override void OnDoubleClick(Mobile m)
		{
			if (!this.CheckDoubleClick(m, true, false, 2, true) || !(m is PlayerMobile))
			{
				return;
			}

			if (Style == CellarStyle.None)
			{
				new CellarFloorSelector((PlayerMobile)m, this).Send();
			}
			else
			{
				BeginTarget(m, Style);
			}
		}

		public void BeginTarget(Mobile m, CellarStyle style)
		{
			if (m == null || m.Deleted || !this.CheckDoubleClick(m, false, false, 2, true)) //soft dclick check
			{
				return;
			}

			m.SendMessage("Pick a place for your cellar stairs.");
			GenericSelectTarget<IPoint3D>.Begin(m, (tm, to) => EndTarget(tm, style, to), OnTargetFail, -1, true);
		}

		public void EndTarget(Mobile m, CellarStyle style, IPoint3D p)
		{
			BaseHouse house;
			Point3D loc = p.Clone3D();

			if (!CellarAddon.CanPlace(m, loc, out house))
			{
				return;
			}

			Style = style;

			var addon = Addon;

			addon.MoveToWorld(loc, m.Map);

			Delete();
		}

		protected void OnTargetFail(Mobile m)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(3);

			switch(version)
			{
				case 3:
				case 2:
					writer.WriteFlag(Style);
					break;
				case 1:
					writer.Write((int)Style);
					break;
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
				case 3:
					LootType = LootType.Blessed;
					goto case 2;
				case 2:
					Style = reader.ReadFlag<CellarStyle>();
					break;
				case 1:
					Style = (CellarStyle)reader.ReadInt();
					break;
				case 0:
					break;
			}
		}
	}

	public sealed class CellarFloorSelector : MenuGump
	{
		public CellarDeed Deed { get; set; }

		public CellarFloorSelector(PlayerMobile user, CellarDeed deed)
			: base(user)
		{
			Deed = deed;

			Modal = true;
			CanClose = true;
			CanDispose = true;
		}

		protected override void CompileOptions(MenuGumpOptions list)
		{
			CellarStyles.Styles.ForEach(s => list.AppendEntry(new ListGumpEntry(s.Name, b => Select(s), HighlightHue)));

			base.CompileOptions(list);
		}

		public override void CompileEntryLayout(
			SuperGumpLayout layout, int length, int index, int pIndex, int yOffset, ListGumpEntry entry)
		{
			base.CompileEntryLayout(layout, length, index, pIndex, yOffset, entry);

			layout.AddReplace(
				"button/list/select/" + index,
				() =>
				{
					CellarStyleInfo info = CellarStyles.Styles.FirstOrDefault(s => s.Name == entry.Label);

					if (info != null && info.Style != CellarStyle.None)
					{
						AddButton(20, yOffset + 5, 1895, 1896, b => SelectEntry(b, entry));
						AddItem(10, yOffset - 5, info.FloorTiles.GetRandom());
					}
					else
					{
						AddButton(15, yOffset, 4006, 4007, b => SelectEntry(b, entry));
					}
				});
		}

		public void Select(CellarStyleInfo info)
		{
			if (info != null)
			{
				User.SendMessage("You have chosen {0}.", info.Name);

				if (Deed != null && info.Style != CellarStyle.None)
				{
					Deed.BeginTarget(User, info.Style);
				}
			}

			Close();
		}
	}
}