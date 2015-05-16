#region References
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using Server.Accounting;
using Server.Mobiles;
using Server.Network;

using VitaNex.FX;
using VitaNex.IO;
using VitaNex.Modules.AutoPvP;
using VitaNex.Modules.AutoPvP.Battles;
using VitaNex.Network;
using VitaNex.Targets;
#endregion

namespace Server.Engines.CustomTitles
{
	public static partial class CustomTitles
	{
		public const AccessLevel Access = AccessLevel.Administrator;

		private static TitlesOptions _CMOptions = new TitlesOptions();

		public static TitlesOptions CMOptions { get { return _CMOptions ?? (_CMOptions = new TitlesOptions()); } }

		public static BinaryDataStore<TitleObjectSerial, Title> TitleRegistry { get; private set; }
		public static BinaryDataStore<TitleObjectSerial, TitleHue> HueRegistry { get; private set; }

		public static BinaryDataStore<PlayerMobile, TitleProfile> Profiles { get; private set; }

		public static TitleRarity[] Rarities { get; private set; }

		public static PacketHandler LookReqParent { get; private set; }

		public static bool GetTitle(PlayerMobile target, out Title t)
		{
			t = null;

			if (target == null || target.Deleted)
			{
				return false;
			}

			TitleProfile p;

			if (!Profiles.TryGetValue(target, out p) || p == null || p.Owner != target)
			{
				return false;
			}

			t = p.SelectedTitle;
			return true;
		}

		public static bool GetTitleHue(PlayerMobile target, out TitleHue h)
		{
			h = null;

			if (target == null || target.Deleted)
			{
				return false;
			}

			TitleProfile p;

			if (!Profiles.TryGetValue(target, out p) || p == null || p.Owner != target)
			{
				return false;
			}

			h = p.SelectedHue;
			return true;
		}

		public static bool GetTitleAndHue(PlayerMobile target, out Title t, out TitleHue h)
		{
			t = null;
			h = null;

			if (target == null || target.Deleted)
			{
				return false;
			}

			TitleProfile p;

			if (!Profiles.TryGetValue(target, out p) || p == null || p.Owner != target)
			{
				return false;
			}

			t = p.SelectedTitle;
			h = p.SelectedHue;
			return true;
		}

		public static bool DisplayTitle(PlayerMobile viewer, PlayerMobile target)
		{
			if (!CMOptions.ModuleEnabled || viewer == null || viewer.Deleted || target == null || target.Deleted)
			{
				return false;
			}

			Title t;
			TitleHue h;

			if (!GetTitleAndHue(target, out t, out h) || t == null)
			{
				return false;
			}

			string title = t.ToString(target.Female);
			int hue = h != null ? h.Hue : CMOptions.DefaultTitleHue;

			if (String.IsNullOrWhiteSpace(title))
			{
				return false;
			}

			target.PrivateOverheadMessage(MessageType.Label, hue, true, title, viewer.NetState);
			return true;
		}

		private static void OnSingleClick(NetState state, PacketReader pvSrc)
		{
			if (state == null || pvSrc == null)
			{
				return;
			}

			PlayerMobile viewer = null;
			PlayerMobile target = null;
			TitleDisplay? d = null;

			if (CMOptions.ModuleEnabled && state.Mobile is PlayerMobile)
			{
				viewer = (PlayerMobile)state.Mobile;

				int pos = pvSrc.Seek(0, SeekOrigin.Current);

				pvSrc.Seek(1, SeekOrigin.Begin);

				Serial s = pvSrc.ReadInt32();

				pvSrc.Seek(pos, SeekOrigin.Begin);

				if (s.IsMobile)
				{
					target = World.FindMobile(s) as PlayerMobile;

					if (target != null && viewer.CanSee(target) && Utility.InUpdateRange(viewer, target))
					{
						Title t;

						if (GetTitle(target, out t) && t != null)
						{
							d = t.Display;
						}
					}
				}
			}

            var battle = AutoPvP.FindBattle(target) as UOF_PvPBattle;

			if (d != null && d.Value == TitleDisplay.BeforeName && (battle == null || !battle.IncognitoMode))
			{
				DisplayTitle(viewer, target);
			}

			if (LookReqParent != null)
			{
				LookReqParent.OnReceive(state, pvSrc);
			}
			else
			{
				PacketHandlers.LookReq(state, pvSrc);
			}

			if (d != null && d.Value == TitleDisplay.AfterName && (battle == null || !battle.IncognitoMode))
			{
				DisplayTitle(viewer, target);
			}
		}

		/// <summary>
		///     Extension method, can be called with an enum instance like Rarity.Fabled.AsColor()
		/// </summary>
		public static Color AsColor(this TitleRarity rarity)
		{
			switch (rarity)
			{
				case TitleRarity.Common:
					return Color.White;
				case TitleRarity.Uncommon:
					return Color.LimeGreen;
				case TitleRarity.Rare:
					return Color.SkyBlue;
				case TitleRarity.Epic:
					return Color.MediumVioletRed;
				case TitleRarity.Legendary:
					return Color.Orange;
				case TitleRarity.Fabled:
					return Color.PaleGoldenrod;
				default:
					return Color.White;
			}
		}

		/// <summary>
		///     Extension method, can be called with an enum instance like Rarity.Fabled.AsHue()
		/// </summary>
		public static int AsHue(this TitleRarity rarity)
		{
			switch (rarity)
			{
				case TitleRarity.Common:
					return 1301;
				case TitleRarity.Uncommon:
					return 63;
				case TitleRarity.Rare:
					return 99;
				case TitleRarity.Epic:
					return 113;
				case TitleRarity.Legendary:
					return 1258;
				case TitleRarity.Fabled:
					return 137;
				default:
					return 0;
			}
		}

		public static TitleProfile EnsureProfile(PlayerMobile pm)
		{
			if (!CMOptions.ModuleEnabled || pm == null)
			{
				return null;
			}

			TitleProfile p;

			if (!Profiles.TryGetValue(pm, out p))
			{
				Profiles.Add(pm, p = new TitleProfile(pm));
			}
			else if (p == null)
			{
				Profiles[pm] = p = new TitleProfile(pm);
			}

			return p;
		}

		public static Title CreateTitle(
			string maleValue, string femaleValue, TitleRarity rarity, TitleDisplay display, out string result)
		{
			if (String.IsNullOrWhiteSpace(maleValue))
			{
				result = "The male title can not be whitespace.";
				return null;
			}

			if (String.IsNullOrWhiteSpace(femaleValue))
			{
				femaleValue = maleValue;
				//result = "The female title can not be whitespace.";
				//return null;
			}

			//Allow female and male titles to be the same to specify gender neutral titles
			/*if (maleValue == femaleValue)
			{
				result = "The male and female titles must differ.";
				return null;
			}*/

			if (TitleRegistry.Any(x => x.Value.MaleTitle == maleValue))
			{
				result = "The male title '" + maleValue + "' already exists in the registry.";
				return null;
			}

			if (TitleRegistry.Any(x => x.Value.FemaleTitle == femaleValue))
			{
				result = "The female title '" + femaleValue + "' already exists in the registry.";
				return null;
			}

			var title = new Title(maleValue, femaleValue, rarity, display);

			TitleRegistry.Add(title.UID, title);

			foreach (TitleProfile p in
				Profiles.Values.AsParallel()
						.Where(p => p != null && p.Owner != null && p.Owner.AccessLevel >= AccessLevel.GameMaster))
			{
				p.Add(title);
			}

			result = "The title '" + title + "' was successfully added to the registry.";
			return title;
		}

		public static TitleHue CreateHue(int value, TitleRarity rarity, out string result)
		{
			if (HueRegistry.Values.Any(x => x.Hue == value))
			{
				result = "The hue '" + value + "' already exists in the registry.";
				return null;
			}

			var hue = new TitleHue(value, rarity);

			HueRegistry.Add(hue.UID, hue);

			foreach (TitleProfile p in
				Profiles.Values.AsParallel()
						.Where(p => p != null && p.Owner != null && p.Owner.AccessLevel >= AccessLevel.GameMaster))
			{
				p.Add(hue);
			}

			result = "The Hue: '" + hue + "' was successfully added to the registry.";
			return hue;
		}

		public static int GetOwnerCount(this Title title)
		{
			return title != null ? Profiles.Values.Count(p => p.Contains(title)) : 0;
		}

		public static int GetOwnerCount(this TitleHue hue)
		{
			return hue != null ? Profiles.Values.Count(p => p.Contains(hue)) : 0;
		}

		public static bool TryGetTitle(string title, out Title value)
		{
			return (value = TitleRegistry.Values.FirstOrDefault(t => t.Match(title))) != null;
		}

		public static bool TryGetHue(int hue, out TitleHue value)
		{
			return (value = HueRegistry.Values.FirstOrDefault(h => h.Match(hue))) != null;
		}

		public static bool PurgeTitle(Title title, out string result)
		{
			if (title == null)
			{
				result = "Title can not be null.";
				return false;
			}

			foreach (TitleProfile p in Profiles.Values)
			{
				p.Remove(title);
			}

			if (!TitleRegistry.Remove(title.UID))
			{
				result = "The title '" + title + "' did not exist in the registry.";
				return false;
			}

			result = "The title '" + title + "' has been successfully deleted.";

			return true;
		}

		public static bool PurgeHue(TitleHue hue, out string result)
		{
			if (hue == null)
			{
				result = "Hue can not be null.";
				return false;
			}

			foreach (TitleProfile p in Profiles.Values)
			{
				p.Remove(hue);
			}

			if (!HueRegistry.Remove(hue.UID))
			{
				result = "The hue '" + hue + "' did not exist in the registry.";
				return false;
			}

			result = "The hue '" + hue + "' has been successfully deleted.";
			return true;
		}

		public static void GrantTitlesTarget(PlayerMobile pm)
		{
			MobileSelectTarget<PlayerMobile>.Begin(pm, (m, t) => GrantTitles(pm, t), null);
		}

		public static void GrantTitles(PlayerMobile pm, PlayerMobile target)
		{
			if (pm == null || pm.Deleted || target == null || target.Deleted)
			{
				return;
			}

			GrantTitles(target);

			target.SendMessage("{0} has granted you every available title that you did not already own.", pm.RawName);
			pm.SendMessage("You have granted {0} every available title they did not already own.", target.RawName);
		}

		public static void GrantTitles(PlayerMobile pm)
		{
			if (pm == null || pm.Deleted)
			{
				return;
			}

			TitleProfile p = EnsureProfile(pm);

			if (p != null)
			{
				p.Titles.AddRange(TitleRegistry.Values.Not(p.Titles.Contains));
			}
		}

		public static void RevokeTitlesTarget(PlayerMobile pm)
		{
			MobileSelectTarget<PlayerMobile>.Begin(pm, (m, t) => RevokeTitles(pm, t), null);
		}

		public static void RevokeTitles(PlayerMobile pm, PlayerMobile target)
		{
			if (pm == null || pm.Deleted || target == null || target.Deleted)
			{
				return;
			}

			RevokeTitles(target);

			target.SendMessage("{0} has revoked all of your titles!", pm.RawName);
			pm.SendMessage("You have revoked every title {0} owned.", target.RawName);
		}

		public static void RevokeTitles(PlayerMobile pm)
		{
			if (pm == null || pm.Deleted)
			{
				return;
			}

			TitleProfile p = EnsureProfile(pm);

			if (p != null)
			{
				p.Titles.Clear();
			}
		}

		public static void SendTitlesGump(PlayerMobile pm)
		{
			if (pm == null || pm.Deleted)
			{
				return;
			}

			TitleProfile p = EnsureProfile(pm);

			if (p != null)
			{
				new CustomTitlesGump(pm, p).Send();
			}
		}

		public static void BeginTarget(Mobile pm)
		{
			if (pm != null && !pm.Deleted)
			{
				pm.Target = new MobileSelectTarget<Mobile>((m, t) => SendFlameLine(pm, t), m => { });
			}
		}

		public static void DoFlash(Mobile pm)
		{
			List<Mobile> list = pm.GetMobilesInRange(pm.Map, 30).Where(m => m != null && !m.Deleted && m.Player).ToList();
			foreach (Mobile player in list)
			{
				Effects.SendIndividualFlashEffect(player, (FlashType)2);
			}
		}

        public static void UpgradeAccounts()
        {
            int charslots = 0;
            foreach (Account acc in Accounts.GetAccounts())
            {
                charslots = acc.GetCharSlots();
                if (charslots < 7)
                {
                    charslots++;
                    acc.SetTag("maxChars", charslots.ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        public static void LockedDownFix()
        {
            var players = World.Mobiles.Values.Where(x => x is PlayerMobile);
            foreach (var player in players)
            {
                var items = player.Backpack.Items;
                foreach (var item in items.Where(item => item.IsLockedDown))
                {
                    item.IsLockedDown = false;
                }
            }
        }

        public static void UpgradeAccountsNew()
        {
            foreach (Account acc in Accounts.GetAccounts())
            {
                int chars = Convert.ToInt32(acc.GetTag("maxChars"));
                if (chars <= 3)
                {
                    chars = 4;
                    acc.SetTag("maxChars", chars.ToString(CultureInfo.InvariantCulture));
                }
            }
        }

		public static void SendFlameLine(Mobile from, Mobile target)
		{
			target.Frozen = true;
			var queue = new EffectQueue
			{
				Deferred = false
			};

			Point3D endpoint = target.Location;
			Point3D[] line = from.Location.GetLine3D(endpoint, from.Map);

			int n = 0;
			foreach (Point3D p in line)
			{
				n += 20;
				//Point3D p1 = p;
				queue.Add(
					new EffectInfo(p, from.Map, 14089, 0, 10, 30, EffectRender.Normal, TimeSpan.FromMilliseconds(n), () => { }));
			}
			queue.Process();
			Timer.DelayCall(TimeSpan.FromSeconds(1), SendSpiral, target);
		}

		public static void SendSpiral(Mobile target)
		{
			var queue = new EffectQueue
			{
				Deferred = false
			};

			var points = new List<Point3D>();
			double d;
			double r = 1;
			int newx;
			int newy;
			points.Add(target.Location);
			//calculate spiral vector
			for (d = 0; d < 4 * Math.PI; d += 0.01)
			{
				newx = (int)Math.Floor(target.X + (Math.Sin(d) * d) * r);
				newy = (int)Math.Floor(target.Y + (Math.Sin(d + (Math.PI / 2)) * (d + (Math.PI / 2))) * r);
				var to = new Point3D(newx, newy, target.Z);
				if (!points.Contains(to))
				{
					points.Add(to);
				}
			}
			int n = 0;
			//Build the queue based on the points in the line.
			foreach (Point3D p in points)
			{
				n += 20;
				queue.Add(
					new EffectInfo(p, target.Map, 14089, 0, 10, 30, EffectRender.Normal, TimeSpan.FromMilliseconds(n), () => { }));
			}
			n += 400; //used to offset when the spiral reverses so it doesn't overlap
			foreach (Point3D p in points.AsEnumerable().Reverse())
			{
				n += 20;
				queue.Add(
					new EffectInfo(p, target.Map, 14089, 0, 10, 30, EffectRender.Normal, TimeSpan.FromMilliseconds(n), () => { }));
			}
			queue.Process();
			target.Frozen = false;
		}
	}
}