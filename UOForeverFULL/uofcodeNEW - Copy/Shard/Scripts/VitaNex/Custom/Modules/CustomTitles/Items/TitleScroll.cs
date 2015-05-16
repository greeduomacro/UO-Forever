#region References
using System;

using Server.Commands;
using Server.Mobiles;
using Server.Network;

using VitaNex.SuperGumps;
#endregion

namespace Server.Engines.CustomTitles
{
	public class TitleScroll : Item
	{
		public static Title Resolve(string title)
		{
			Title t;
			CustomTitles.TryGetTitle(title, out t);
			return t;
		}

		private ScrollConfirmGump _Gump;
		private Title _CachedTitle;

		[CommandProperty(AccessLevel.GameMaster)]
		public string RawTitle { get; set; }

		[CommandProperty(AccessLevel.GameMaster, true)]
		public Title Title
		{
			get
			{
				if (Deleted)
				{
					_CachedTitle = null;
					RawTitle = String.Empty;
					return null;
				}

				if (_CachedTitle != null && _CachedTitle.Match(RawTitle))
				{
					return _CachedTitle;
				}

				return _CachedTitle = Resolve(RawTitle);
			}
			set
			{
				if (Deleted || value == null)
				{
					_CachedTitle = null;
					RawTitle = String.Empty;
					return;
				}

				_CachedTitle = value;
				RawTitle = _CachedTitle.ToString();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile BoundToMobile { get; set; }

		public override bool ExpansionChangeAllowed { get { return true; } }

		[Constructable]
		public TitleScroll()
			: base(0x2831)
		{
			Name = "a title scroll";
			Weight = 2;
			Hue = 1195;
		}

		[Constructable]
		public TitleScroll(string title)
			: this(Resolve(title))
		{ }

		public TitleScroll(Title title)
			: base(0x2831)
		{
			Title = title;

			Name = "a title scroll";
			Weight = 2;
			Hue = 1195;
		}

		public TitleScroll(Serial serial)
			: base(serial)
		{ }

		public override void OnSingleClick(Mobile m)
		{
			if (Title == null)
			{
				base.OnSingleClick(m);
				return;
			}

			LabelToExpansion(m);

			LabelTo(m, this.ResolveName(m), Title.GetRarityHue());
			LabelTo(m, Title.GetRarityHue(), 1049644, "Title: " + Title.ToString(m.Female));

			if (BoundToMobile != null)
			{
				LabelTo(m, "Bound to: " + BoundToMobile.Name, Title.GetRarityHue());
			}
		}

		public override void OnAfterDuped(Item newItem)
		{
			var titlescroll = newItem as TitleScroll;

			if (titlescroll != null)
			{
				titlescroll.RawTitle = RawTitle;
			}

			base.OnAfterDuped(newItem);
		}

		public bool CanConsume(PlayerMobile pm, bool message)
		{
			if (Title == null)
			{
				return false;
			}

			if (BoundToMobile != null && BoundToMobile != pm)
			{
				if (message)
				{
					pm.SendMessage(54, "This scroll is bound to another entity.");
				}

				return false;
			}

			TitleProfile p = CustomTitles.EnsureProfile(pm);

			if (p == null)
			{
				return false;
			}

			if (p.Contains(Title))
			{
				if (message)
				{
					pm.SendMessage(0x22, "You already own this title.");
				}

				return false;
			}

			return true;
		}

		public bool AutoConsume(PlayerMobile m)
		{
			if (m == null || m.Deleted)
			{
				return false;
			}

			if (Title == null)
			{
				m.SendMessage(0x22, "This ancient scroll has lost whatever power it once had.");
				m.PrivateOverheadMessage(MessageType.Label, 1287, true, "*The scroll disintegrates in your hands*", m.NetState);

				LoggingCustom.Log("TitleScrollsInvalid.txt", String.Format("{0} -> {1} ({2})", m, this, Title));

				Delete();

				return false;
			}

			if (!CanConsume(m, false))
			{
				return false;
			}

			TitleProfile p = CustomTitles.EnsureProfile(m);

			if (p == null)
			{
				return false;
			}

			GrantTitle(m, p);
			return true;
		}

		public override void OnDoubleClick(Mobile m)
		{
			if (!this.CheckDoubleClick(m, true, false, 2, true) || !(m is PlayerMobile))
			{
				return;
			}

			if (Title == null)
			{
				m.SendMessage(0x22, "This ancient scroll has lost whatever power it once had.");
				m.PrivateOverheadMessage(MessageType.Label, 1287, true, "*The scroll disintegrates in your hands*", m.NetState);

				LoggingCustom.Log("TitleScrollsInvalid.txt", String.Format("{0} -> {1} ({2})", m, this, Title));

				Delete();

				return;
			}

			if (!CanConsume((PlayerMobile)m, true))
			{
				return;
			}

			var pm = (PlayerMobile)m;
			
			TitleProfile p = CustomTitles.EnsureProfile(pm);

			if (p == null)
			{
				return;
			}

			if (_Gump != null)
			{
				_Gump.Close(true);
				_Gump = null;
			}

			_Gump = SuperGump.Send(
				new ScrollConfirmGump(
					pm,
					null,
					Title,
					b => GrantTitle(pm, p),
					b =>
					{
						pm.SendMessage(0x55, "You choose to not use the title scroll.");
						_Gump = null;
					}));
		}

		private void GrantTitle(PlayerMobile pm, TitleProfile p)
		{
			if (Title == null || pm == null || p == null)
			{
				return;
			}

			if (p.Contains(Title))
			{
				pm.SendMessage(0x22, "You already own this title.");
				return;
			}

			p.Add(Title);

			pm.PrivateOverheadMessage(
				MessageType.Label, 1287, true, "*The scroll crumbles in your hands as you absorb its power*", pm.NetState);

			LoggingCustom.Log("TitleScrollsUsed.txt", String.Format("{0} -> {1} ({2})", pm, this, Title));
			
			Delete();
		}

		public override void Delete()
		{
			if (_Gump != null)
			{
				_Gump.Close(true);
				_Gump = null;
			}

			Title = null;

			base.Delete();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
					writer.WriteMobile(BoundToMobile);
					goto case 0;
				case 0:
					writer.Write(RawTitle);
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
					BoundToMobile = reader.ReadMobile();
					goto case 0;
				case 0:
					RawTitle = reader.ReadString();
					break;
			}
		}
	}
}