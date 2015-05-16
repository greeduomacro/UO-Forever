#region References
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Server.Engines.CustomTitles;
using Server.Gumps;
using Server.Mobiles;

using VitaNex.SuperGumps;
#endregion

namespace Server.Engines.Conquests
{
	public class ConquestStatesGump : SuperGumpList<ConquestState>
	{
		private int _CategoryScroll;

		public int CategoryScrollMax { get; protected set; }

		public virtual int CategoryScroll { get { return _CategoryScroll; } set { _CategoryScroll = Math.Max(0, Math.Min(CategoryScrollMax, value)); } }

		public string Title { get; private set; }

		public ConquestCategory RootCategory { get; private set; }

		public List<ConquestCategory> Categories { get; private set; }
		
		public ConquestCategory Category { get; set; }

		public ConquestProfile Profile { get; set; }

		public ConquestStatesGump(
			PlayerMobile user, Gump parent = null, ConquestProfile profile = null, ConquestCategory category = null)
			: base(user, parent)
		{
			Title = "Conquests";
			RootCategory = String.Empty;

			Profile = profile ?? Conquests.EnsureProfile(user);

			Category = category ?? String.Empty;

			Categories = new List<ConquestCategory>();

			ForceRecompile = true;

			CanMove = true;

			Sorted = true;

			EntriesPerPage = 4;
		}

		protected override void CompileList(List<ConquestState> list)
		{
			list.Clear();

            list.TrimExcess();

			if (Profile != null)
			{
				list.AddRange(
					Profile.Where(CanDisplay)
						   .Where(s => (Category.IsEmpty && s.Completed) || (!Category.IsEmpty && Category.Equals(s.Conquest.Category))));
			}

			base.CompileList(list);

			CompileCategories();
		}

		public void CompileCategories()
		{
			Categories.Clear();

            Categories.TrimExcess();


			Categories.Add(RootCategory);

			if (Category.HasParent)
			{
				Categories.AddOrReplace(Category.Parent);
			}

			foreach (ConquestCategory c in
				Profile.Where(CanDisplay).Select(s => new ConquestCategory(s.Conquest.Category ?? ConquestCategory.Default)))
			{
				Categories.AddOrReplace(c);

				foreach (var p in c.GetParents())
				{
					Categories.AddOrReplace(p);
				}
			}

			var selectedParents = Category.GetParents().ToArray();
			
			Categories.RemoveAll(
				c =>
				{
					var parents = c.GetParents().ToArray();

					if (parents.Length > 0)
					{
						if (parents.Length <= selectedParents.Length && c != Category && !parents.Contains(Category) &&
							!selectedParents.Any(p => p == c || c.Parent == p))
						{
							return true;
						}

						if (parents.Length > selectedParents.Length && c.Parent != Category)
						{
							return true;
						}
					}

					return false;
				});

			Categories.Sort((l, r) => Insensitive.Compare(l.FullName, r.FullName));

			CategoryScrollMax = Math.Max(0, Categories.Count - 14);
			CategoryScroll = Math.Max(0, Math.Min(CategoryScrollMax, CategoryScroll));
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.Add(
				"frame/main",
				() =>
				{
					AddBackground(0, 40, 775, 525, 2600);
					AddImage(269, 18, 1419);
					AddImage(346, 0, 1417);
					AddImage(355, 9, 9012);
				});

			layout.Add(
				"frame/inner",
				() =>
				{
					AddBackground(299, 100, 420, 413, 9270);
					AddBackground(55, 100, 250, 413, 9270);
				});

			layout.Add(
				"conquest/scrollbar",
				() =>
				{
					AddBackground(480, 114, 212, 20, 9200);

					AddScrollbarH(
						480,
						114,
						PageCount,
						Page,
						PreviousPage,
						NextPage,
						new Rectangle2D(23, 4, 166, 13),
						new Rectangle2D(2, 3, 16, 16),
						new Rectangle2D(192, 3, 16, 16),
						Tuple.Create(9354, 9304), // track, handle
						Tuple.Create(5603, 5607, 5607), // normal, pressed, inactive
						Tuple.Create(5601, 5605, 5605)); // normal, pressed, inactive
				});

			CompileCategoryLayout(layout);

			Dictionary<int, ConquestState> range = GetListRange();

			layout.Add(
				"frame/topsides",
				() =>
				{
					AddImageTiled(716, 134, 17, 389, 10154);
					AddImageTiled(59, 509, 659, 15, 10154);
					AddImageTiled(42, 134, 17, 389, 10154);
					AddImageTiled(58, 88, 659, 15, 10154);
				});

			layout.Add(
				"frame/dogsheads",
				() =>
				{
					AddImage(295, 86, 10452, 1102);
					AddImage(696, 84, 10450);
					AddImage(42, 84, 10450);
				});

			layout.Add(
				"frame/owner",
				() =>
				{
					if (Profile.Owner.Account == User.Account)
					{
						AddHtml(90, 115, 197, 40, "Your Conquests".WrapUOHtmlColor(Color.Gold, false), false, false);
					}
					else
					{
						AddHtml(
							90,
							115,
							197,
							40,
							String.Format("Conquests of {0}", Profile.Owner.RawName.WrapUOHtmlColor(User.GetNotorietyColor(Profile.Owner)))
								  .WrapUOHtmlColor(Color.Gold, false),
							false,
							false);
					}
				});

			layout.Add(
				"frame/points",
				() =>
				AddHtml(
					300,
					90,
					170,
					60,
					String.Format("Conquest Points\n{0:#,0}", Profile.GetPointsTotal())
						  .WrapUOHtmlTag("BIG")
						  .WrapUOHtmlTag("CENTER")
						  .WrapUOHtmlColor(Color.Gold, false),
					false,
					false));

            if (range.Count > 0)
            {
                CompileConquestLayout(layout, range);
            }
		}

		private void CompileCategoryLayout(SuperGumpLayout layout)
		{
			int cIndex = 0;
			
			layout.Add(
				"category/scrollbar",
				() =>
				{
					AddBackground(70, 140, 20, 360, 9200);

					AddScrollbarV(
						70,
						140,
						CategoryScrollMax,
						CategoryScroll,
						b =>
						{
							--CategoryScroll;
							Refresh(true);
						},
						b =>
						{
							++CategoryScroll;
							Refresh(true);
						},
						new Rectangle2D(3, 25, 13, 310),
						new Rectangle2D(2, 5, 16, 16),
						new Rectangle2D(2, 340, 16, 16),
						Tuple.Create(9354, 9304), // track, handle
						Tuple.Create(5600, 5604, 5604), // normal, pressed, inactive
						Tuple.Create(5602, 5606, 5606)); // normal, pressed, inactive
				});

			var range = Categories.Skip(CategoryScroll).Take(15);

			const int catSpacing = 24;

			foreach (ConquestCategory c in range)
			{
				int index = cIndex;
				var parents = c.GetParents().ToArray();

				// Insert the button before the backgrounds that were added earlier
				layout.AddBefore(
					"frame/inner",
					"category/button/" + cIndex,
					() => AddButton(
						90,
						142 + (catSpacing * index),
						1122,
						1124,
						btn =>
						{
							Category = Category != c ? c : Category.HasParent ? Category.Parent : RootCategory;
							Refresh(true);
						}));

				layout.Add(
					"category/entry/" + cIndex,
					() =>
					{
						int offset = parents.Length * 10;

						AddBackground(95 + offset, 140 + (catSpacing * index), 195 - offset, 20, 9200);

						AddLabelCropped(
							100 + offset,
							140 + (catSpacing * index),
							190 - offset,
							20,
							Category == c || Category.IsChildOf(c) ? 1258 : 2049,
							c == RootCategory ? "Recently Completed" : c.Name);
					});

				++cIndex;
			}
		}

		protected virtual void CompileConquestLayout(SuperGumpLayout layout, Dictionary<int, ConquestState> range)
		{
			range.For((i, kv) => CompileConquestLayout(layout, range.Count, kv.Key, i, 138 + (i * 91), kv.Value));
		}

		protected virtual void CompileConquestLayout(
			SuperGumpLayout layout, int length, int index, int pIndex, int yOffset, ConquestState entry)
		{
			layout.Add(
				"conquest/entry/" + index,
				() =>
				{
					AddBackground(313, yOffset, 394, 87, 9200);

					AddItem(
						328 + entry.Conquest.ItemIDOffset.X,
						yOffset + 6 + entry.Conquest.ItemIDOffset.Y,
						entry.Conquest.ItemID,
						entry.Conquest.Hue);

					if (entry.TierMax > 1)
					{
						AddLabel(333, yOffset + 55, 2049, "Tier");
						AddLabel(336, yOffset + 67, 2049, entry.Tier + "/" + entry.TierMax);
					}

					if (entry.Completed)
					{
						AddItem(619, yOffset + 69, 4655); // Blood
					}

					AddImage(627, yOffset + 4, 1417); // Disc

					if (entry.Completed)
					{
						AddLabel(550, yOffset - 1, entry.Completed ? 1258 : 2049, entry.CompletedDate.ToShortDateString());
					}

					ConquestRewardInfo[] rewards =
						entry.Conquest.Rewards.Select(t => ConquestRewardInfo.EnsureInfo(entry.Conquest, t))
							 .Where(r => r != null)
							 .ToArray();
					ConquestRewardInfo displayReward =
						rewards.FirstOrDefault(r => r.TypeOf.IsEqualOrChildOf<Item>() || r.TypeOf.IsEqualOrChildOf<Mobile>());

					if (displayReward != null)
					{
						AddHtml(
							637,
							yOffset + 19,
							60,
							40,
							String.Format("{0:#,0}", entry.Conquest.Points)
								  .WrapUOHtmlTag("CENTER")
								  .WrapUOHtmlColor(entry.Completed ? Color.OrangeRed : Color.White),
							false,
							false);

						if (displayReward.ItemID > 0)
						{
							AddItem(645, yOffset + 39, displayReward.ItemID, displayReward.Hue);
						}

						if (String.IsNullOrWhiteSpace(displayReward.Name) && displayReward.Label > 0)
						{
							AddTooltip(displayReward.Label);
						}

						if (displayReward.Amount > 1)
						{
							AddHtml(
								637,
								yOffset + 49,
								60,
								40,
								String.Format("{0:#,0} x", displayReward.Amount)
									  .WrapUOHtmlTag("CENTER")
									  .WrapUOHtmlColor(entry.Completed ? Color.Yellow : Color.White),
								false,
								false);
						}
					}
					else
					{
						AddHtml(
							637,
							yOffset + 34,
							60,
							40,
							String.Format("{0:#,0}", entry.Conquest.Points)
								  .WrapUOHtmlTag("CENTER")
								  .WrapUOHtmlColor(entry.Completed ? Color.OrangeRed : Color.White),
							false,
							false);
					}

					string desc = entry.Conquest.Desc ?? String.Empty;

				    if (!entry.Completed)
				    {
                        if (!String.IsNullOrWhiteSpace(desc))
                        {
                            desc += "\n";
                        }

                        desc += "Current Progress: " + entry.Progress + "/" + entry.ProgressMax;				        
				    }

					if (rewards.Length > 0)
					{
						if (!String.IsNullOrWhiteSpace(desc))
						{
							desc += "\n\n";
						}

						desc += "Rewards:\n";
						desc += String.Join("\n", rewards.Select(r => r.ToString()));
					}

                    if (entry.WorldFirst)
                    {
                        if (!String.IsNullOrWhiteSpace(desc))
                        {
                            desc += "\n\n";
                        }

                        desc += "*****WORLD FIRST*****";
                    }

					if (!String.IsNullOrWhiteSpace(desc))
					{
						AddHtml(389, yOffset + 28, 229, 53, desc, true, true);
					}

					if (!String.IsNullOrWhiteSpace(entry.Conquest.Name))
					{
						AddLabel(389, yOffset + 12, entry.Completed ? 1258 : 2049, entry.Conquest.Name);
					}
				});

			layout.Add(
				"conquest/entry/progress/" + index,
				() =>
				{
					const int height = 77;

					int hOffset = (int)Math.Ceiling(height * (entry.Progress / (double)entry.ProgressMax));

					if (hOffset >= height)
					{
						AddImageTiled(316, yOffset + 5, 8, height, 9742);
					}
					else
					{
						AddImageTiled(316, yOffset + 5, 8, height, 9740);

						if (hOffset > 0)
						{
							AddImageTiled(316, (yOffset + 5) + (height - hOffset), 8, hOffset, 9743);

							if (hOffset < height)
							{
								AddImageTiled(316, (yOffset + 5) + (height - hOffset), 8, 2, 9742); // Borderline
							}
						}
					}

					// Markers for 25%, 50% and 75%
					/*
					AddImageTiled(316, yOffset + 23, 4, 2, 10722);
					AddImageTiled(316, yOffset + 43, 4, 2, 10722);
					AddImageTiled(316, yOffset + 63, 4, 2, 10722);
					*/
				});

			if (entry.TierMax > 1)
			{
				layout.Add(
					"conquest/entry/tier/" + index,
					() =>
					{
						const int height = 77;

						var hOffset = (int)Math.Ceiling(height * (entry.Tier / (double)entry.TierMax));

						if (hOffset >= height)
						{
							AddImageTiled(326, yOffset + 5, 8, height, 9742);
						}
						else
						{
							AddImageTiled(326, yOffset + 5, 8, height, 9740);

							if (hOffset > 0)
							{
								AddImageTiled(326, (yOffset + 5) + (height - hOffset), 8, hOffset, 9741);

								if (hOffset < height)
								{
									AddImageTiled(326, (yOffset + 5) + (height - hOffset), 8, 2, 9742); // Borderline
								}
							}
						}

						// Markers for 25%, 50% and 75%
						/*
						AddImageTiled(326, yOffset + 23, 4, 2, 10722);
						AddImageTiled(326, yOffset + 43, 4, 2, 10722);
						AddImageTiled(326, yOffset + 63, 4, 2, 10722);
						*/
					});
			}
		}

		public override int SortCompare(ConquestState a, ConquestState b)
		{
			int result = 0;

			if (a.CompareNull(b, ref result))
			{
				return result;
			}

			if (a.Completed && b.Completed)
			{
				if (a.CompletedDate.Year > b.CompletedDate.Year)
				{
					return -1;
				}

				if (a.CompletedDate.Year < b.CompletedDate.Year)
				{
					return 1;
				}

				if (a.CompletedDate.Month > b.CompletedDate.Month)
				{
					return -1;
				}

				if (a.CompletedDate.Month < b.CompletedDate.Month)
				{
					return 1;
				}

				if (a.CompletedDate.Day > b.CompletedDate.Day)
				{
					return -1;
				}

				if (a.CompletedDate.Day < b.CompletedDate.Day)
				{
					return 1;
				}

				return Insensitive.Compare(a.Name, b.Name);
			}

			if (a.Completed)
			{
				return -1;
			}

			if (b.Completed)
			{
				return 1;
			}

			double aT = a.Tier / (double)a.TierMax;
			double bT = b.Tier / (double)b.TierMax;

			if (aT > bT)
			{
				return -1;
			}

			if (aT < bT)
			{
				return 1;
			}

			double aP = a.Progress / (double)a.ProgressMax;
			double bP = b.Progress / (double)b.ProgressMax;

			if (aP > bP)
			{
				return -1;
			}

			if (aP < bP)
			{
				return 1;
			}

			return Insensitive.Compare(a.Name, b.Name);
		}

		protected bool CanDisplay(ConquestState state)
		{
			return state != null && state.ConquestExists && state.Conquest.Enabled &&
				   (!state.Conquest.Secret || state.Tier > 0 || User.AccessLevel >= Conquests.Access);
		}
	}
}