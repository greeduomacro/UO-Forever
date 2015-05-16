#region References
using System;

using Server.Commands;
using Server.Mobiles;
using Server.Network;

using VitaNex.SuperGumps;
#endregion

namespace Server.Engines.CustomTitles
{
	public class HueScroll : Item
	{
		public static TitleHue Resolve(int hue)
		{
			TitleHue h;
			CustomTitles.TryGetHue(hue, out h);
			return h;
		}

		private ScrollConfirmGump _Gump;
		private TitleHue _CachedHue;

		[CommandProperty(AccessLevel.GameMaster)]
		public int RawTitleHue { get; set; }

		[CommandProperty(AccessLevel.GameMaster, true)]
		public TitleHue TitleHue
		{
			get
			{
				if (Deleted)
				{
					_CachedHue = null;
					RawTitleHue = -1;
					return null;
				}

				if (_CachedHue != null && _CachedHue.Hue == RawTitleHue)
				{
					return _CachedHue;
				}

				return _CachedHue = Resolve(RawTitleHue);
			}
			set
			{
				if (Deleted || value == null)
				{
					_CachedHue = null;
					RawTitleHue = -1;
					return;
				}

				_CachedHue = value;
				RawTitleHue = _CachedHue.Hue;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile BoundToMobile { get; set; }

		public override bool ExpansionChangeAllowed { get { return true; } }

		[Constructable]
		public HueScroll()
			: base(0x2831)
		{
			Name = "a title hue scroll";
			Weight = 2;
			Hue = 1195;
		}

		[Constructable]
		public HueScroll(int hue)
			: this(Resolve(hue))
		{ }

		public HueScroll(TitleHue hue)
			: base(0x2831)
		{
			TitleHue = hue;

			Name = "a title hue scroll";
			Weight = 2;
			Hue = 1195;
		}

		public HueScroll(Serial serial)
			: base(serial)
		{ }

		public override void OnSingleClick(Mobile m)
		{
			if (TitleHue == null)
			{
				base.OnSingleClick(m);
				return;
			}

			LabelToExpansion(m);

			LabelTo(m, this.ResolveName(m), TitleHue.GetRarityHue());
			LabelTo(m, TitleHue.Hue, 1049644, "Hue: " + TitleHue.Hue);

			if (BoundToMobile != null)
			{
				LabelTo(m, "Bound to: " + BoundToMobile.Name, TitleHue.GetRarityHue());
			}
		}

		public bool CanConsume(PlayerMobile pm, bool message)
		{
			if (TitleHue == null)
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

			if (p.Contains(TitleHue))
			{
				if (message)
				{
					pm.SendMessage(0x22, "You already own this title hue.");
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

			if (TitleHue == null)
			{
				m.SendMessage(0x22, "This ancient scroll has lost whatever power it once had.");
				m.PrivateOverheadMessage(MessageType.Label, 1287, true, "*The scroll disintegrates in your hands*", m.NetState);

				LoggingCustom.Log("HueScrollsInvalid.txt", String.Format("{0} -> {1} ({2})", m, this, TitleHue));

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

			GrantHue(m, p);
			return true;
		}

		public override void OnDoubleClick(Mobile m)
		{
			if (!this.CheckDoubleClick(m, true, false, 2, true) || !(m is PlayerMobile))
			{
				return;
			}

			if (TitleHue == null)
			{
				m.SendMessage(0x22, "This ancient scroll has lost whatever power it once had.");
				m.PrivateOverheadMessage(MessageType.Label, 1287, true, "*The scroll disintegrates in your hands*", m.NetState);

				LoggingCustom.Log("HueScrollsInvalid.txt", String.Format("{0} -> {1} ({2})", m, this, TitleHue));

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
					TitleHue,
					b => GrantHue(pm, p),
					b =>
					{
						pm.SendMessage(0x55, "You choose to not use the title hue scroll.");
						_Gump = null;
					}));
		}

		private void GrantHue(PlayerMobile pm, TitleProfile p)
		{
			if (TitleHue == null || pm == null || p == null)
			{
				return;
			}

			if (p.Contains(TitleHue))
			{
				pm.SendMessage(0x22, "You already own this title hue.");
				return;
			}

			p.Add(TitleHue);

			pm.PrivateOverheadMessage(
				MessageType.Label, 1287, true, "*The scroll crumbles in your hands as you absorb its power*", pm.NetState);

			LoggingCustom.Log("HueScrollsUsed.txt", String.Format("{0} -> {1} ({2})", pm, this, TitleHue));

			Delete();
		}

		public override void OnAfterDuped(Item newItem)
		{
			var huescroll = newItem as HueScroll;

			if (huescroll != null)
			{
				huescroll.RawTitleHue = RawTitleHue;
			}

			base.OnAfterDuped(newItem);
		}

		public override void Delete()
		{
			if (_Gump != null)
			{
				_Gump.Close(true);
				_Gump = null;
			}

			TitleHue = null;

			base.Delete();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
					writer.Write(BoundToMobile);
					goto case 0;
				case 0:
					writer.Write(RawTitleHue);
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
					RawTitleHue = reader.ReadInt();
					break;
			}
		}
	}
}