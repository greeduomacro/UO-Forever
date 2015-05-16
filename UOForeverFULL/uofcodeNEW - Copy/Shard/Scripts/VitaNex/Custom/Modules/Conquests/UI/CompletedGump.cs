#region References
using System;
using System.Drawing;
using System.Linq;

using Server.Mobiles;

using VitaNex;
using VitaNex.SuperGumps;
#endregion

namespace Server.Engines.Conquests
{
	public abstract class ConquestCompletedGump : SuperGump
	{
		public static ConquestCompletedGump Acquire(ConquestState state)
		{
			var gumps = GetInstances<ConquestCompletedGump>(state.User, true);
			var type = Conquests.ConquestCompleteGumpTypes.FirstOrDefault(t => gumps.All(g => !g.TypeEquals(t)));

			var cg = type.CreateInstanceSafe<ConquestCompletedGump>(state.User, state);

			if (cg != null)
			{
				const int xPos = 100;
				const int yPos = 80;

				cg.X = xPos;
				cg.Y = yPos;

				if (gumps.Length > 0)
				{
					const int w = 350;
					const int h = 150;

					const int rows = 2;
					const int cols = 3;

					bool escape = false;

					for (int x = 0; x < cols; x++)
					{
						int realX = xPos + (x * w);

						for (int y = 0; y < rows; y++)
						{
							int realY = yPos + (y * h);

							if (gumps.Any(g => g.X == realX && g.Y == realY))
							{
								continue;
							}

							cg.X = realX;
							cg.Y = realY;

							escape = true;
							break;
						}

						if (escape)
						{
							break;
						}
					}
				}
			}

			return cg;
		}

		public sealed class Sub0 : ConquestCompletedGump
		{
			public Sub0(PlayerMobile user, ConquestState state)
				: base(user, state)
			{ }
		}

		public sealed class Sub1 : ConquestCompletedGump
		{
			public Sub1(PlayerMobile user, ConquestState state)
				: base(user, state)
			{ }
		}

		public sealed class Sub2 : ConquestCompletedGump
		{
			public Sub2(PlayerMobile user, ConquestState state)
				: base(user, state)
			{ }
		}

		public sealed class Sub3 : ConquestCompletedGump
		{
			public Sub3(PlayerMobile user, ConquestState state)
				: base(user, state)
			{ }
		}

		public sealed class Sub4 : ConquestCompletedGump
		{
			public Sub4(PlayerMobile user, ConquestState state)
				: base(user, state)
			{ }
		}

		public sealed class Sub5 : ConquestCompletedGump
		{
			public Sub5(PlayerMobile user, ConquestState state)
				: base(user, state)
			{ }
		}

		public static TimeSpan DefaultAnimDuration = TimeSpan.FromMilliseconds(1500.0);
		public static TimeSpan DefaultPauseDuration = TimeSpan.FromSeconds(5.0);

		public ConquestState State { get; set; }

		public TimeSpan AnimDuration { get; set; }
		public TimeSpan PauseDuration { get; set; }

		public bool AutoClose { get; set; }

		public bool? AnimState { get; private set; }

		public int FrameIndex { get; private set; }
		public int FrameCount { get { return (int)Math.Ceiling(Math.Max(100.0, AnimDuration.TotalMilliseconds) / 100.0); } }

		public override bool InitPolling { get { return true; } }

		private bool _Sound1, _Sound2;

		public ConquestCompletedGump(PlayerMobile user, ConquestState state)
			: base(user)
		{
			AnimDuration = DefaultAnimDuration;
			PauseDuration = DefaultPauseDuration;

			State = state;

			Modal = false;

			AutoClose = true;

			FrameIndex = 0;
			AnimState = null;

			CanClose = true;
			CanDispose = true;
			CanMove = false;
			CanResize = false;

			ForceRecompile = true;
			AutoRefresh = true;
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			double fp = FrameIndex / (double)FrameCount;

			layout.Add(
				"conquest/bg",
				() =>
				{
					const int x = 35, y = 64;

					int w = 315, h = 80;

					if (fp <= 0.75)
					{
						w = (int)Math.Ceiling((fp / 0.75) * w);
					}

					if (w > 0 && h > 0)
					{
						AddBackground(x, y, w, h, 2620);
					}

					if (w > 10 && h > 10)
					{
						AddAlphaRegion(x + 5, y + 5, w - 10, h - 10);
					}
				});

			layout.Add(
				"conquest/dragon",
				() =>
				{
					if (AnimState == true)
					{
						var p = new Point2D(117, 54);

						int itemID;

						if (fp <= 0.80)
						{
							if (!_Sound1)
							{
								User.PlaySound(551);
								_Sound1 = true;
							}
							
							itemID = 14078 + (int)Math.Floor(10 * (fp / 0.80)); //fireball

							p = p.Lerp2D(310, 104, fp);
						}
						else
						{
							if (!_Sound2)
							{
								User.PlaySound(520);
								_Sound2 = true;
							}
							
							itemID = 14002 + (int)Math.Floor(30 * ((fp - 0.80) / 0.20)); //explosion

							p = p.Lerp2D(272, 80, fp);
						}

						AddItem(p.X, p.Y, itemID); 
					}

					AddImage(0, 0, 10400); //dragon
				});

			layout.Add(
				"conquest/badge",
				() =>
				{
					AddImage(20, 65, 1417); //plate

					if (AnimState == true && fp <= 0.50)
					{
						AddItem(30, 80, 14002 + (int)Math.Floor(30 * (fp / 0.50)));
					}
					else if (State.Conquest.Hue > 0)
					{
						AddItem(35, 85, State.Conquest.ItemID, State.Conquest.Hue);
					}
					else
					{
						AddItem(35, 85, State.Conquest.ItemID);
					}
				});

			layout.Add(
				"conquest/title",
				() =>
				{
					if (fp >= 0.80)
					{
						AddLabelCropped(105, 70, 170, 20, HighlightHue, "CONQUEST UNLOCKED!");
					}
				});

			layout.Add(
				"conquest/name",
				() =>
				{
					if (fp >= 0.85)
					{
						AddLabelCropped(105, 93, 170, 20, TextHue, State.Name);
					}
				});

			layout.Add(
				"conquest/tier",
				() =>
				{
					if (fp < 0.90)
					{
						return;
					}

					string tier = String.Format("Tier: {0:#,0} / {1:#,0}", State.Tier, State.TierMax);
					
					AddLabelCropped(105, 116, 170, 20, HighlightHue, tier);
				});

			layout.Add(
				"state/icon",
				() =>
				{
					if (fp < 0.95)
					{
						return;
					}

					if (State.WorldFirst)
					{
						//globe
						AddButton(
							282,
							75,
							5608,
							5609,
							b =>
							{
								Refresh();
								Send(new ConquestStateGump(User, State));
							});
					}
					else
					{
						//uo icon
						AddButton(
							280,
							74,
							5545,
							5546,
							b =>
							{
								Refresh();
								Send(new ConquestStateGump(User, State));
							});
					}
				});

			layout.Add(
				"conquest/points",
				() =>
				{
					if (fp < 0.95)
					{
						return;
					}

					string points = State.Conquest.Points.ToString("#,0") + " CP";

					points = points.WrapUOHtmlTag("big");
					points = points.WrapUOHtmlTag("center");
					points = points.WrapUOHtmlColor(Color.SkyBlue);

					//AddImageTiled(285, 95, 50, 20, 2624);
					AddAlphaRegion(285, 95, 50, 20);
					AddHtml(285, 95, 50, 40, points, false, false); // points
				});
		}

		protected virtual Color GetTierColor(ConquestState entry)
		{
			if (entry == null || !entry.ConquestExists)
			{
				return Color.Yellow;
			}

			double p = entry.Tier / entry.TierMax;

			if (p < 0.50)
			{
				return Color.OrangeRed.Interpolate(Color.Yellow, p / 0.50);
			}

			if (p > 0.50)
			{
				return Color.Yellow.Interpolate(Color.LawnGreen, (p - 0.50) / 0.50);
			}

			return Color.Yellow;
		}

		protected override bool CanAutoRefresh()
		{
			return State == null && FrameIndex > 0 ? AutoClose && base.CanAutoRefresh() : base.CanAutoRefresh();
		}

		protected override void OnAutoRefresh()
		{
			base.OnAutoRefresh();

			AnimateList();

			switch (AnimState)
			{
				case true:
					{
						if (FrameIndex++ >= FrameCount)
						{
							AutoRefreshRate = PauseDuration;
							AnimState = null;
							FrameIndex = FrameCount;
						}
					}
					break;
				case false:
					{
						if (FrameIndex-- <= 0)
						{
							AutoRefreshRate = TimeSpan.FromMilliseconds(100.0);
							AnimState = null;
							FrameIndex = 0;
							Close(true);
						}
					}
					break;
				case null:
					{
						AutoRefreshRate = TimeSpan.FromMilliseconds(100.0);
						AnimState = FrameIndex <= 0 ? true : AutoClose ? false : (bool?)null;
					}
					break;
			}
		}

		public override void Close(bool all = false)
		{
			if (all)
			{
				base.Close(true);
			}
			else
			{
				AutoRefreshRate = TimeSpan.FromMilliseconds(100.0);
				AutoClose = true;
			}
		}

		private void AnimateList()
		{
			VitaNexCore.TryCatch(
				() =>
				{
					ConquestCompletedGump p = this;

					foreach (ConquestCompletedGump g in
						GetInstances<ConquestCompletedGump>(User, true)
							.Where(g => g != this && g.IsOpen && !g.IsDisposed && g.X >= p.X)
							.OrderBy(g => g.X))
					{
						g.X = p.X + p.OuterWidth;
						p = g;

						if (g.AnimState != null)
						{
							return;
						}

						DateTime lr = g.LastAutoRefresh;
						g.Refresh(true);
						g.LastAutoRefresh = lr;
					}
				},
				e => e.ToConsole());
		}
	}
}