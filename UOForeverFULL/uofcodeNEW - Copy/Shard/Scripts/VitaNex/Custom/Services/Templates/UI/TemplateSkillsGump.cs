#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Gumps;
using Server.Mobiles;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace Server.PvPTemplates
{
	public sealed class TemplateSkillsGump : SkillSelectionGump
	{
		private static readonly SkillName[] _AcceptedSkills = new[]
		{
			SkillName.Alchemy, SkillName.Anatomy, SkillName.Archery, SkillName.EvalInt, SkillName.Fencing, SkillName.Healing,
			SkillName.Inscribe, SkillName.Lumberjacking, SkillName.Macing, SkillName.Magery, SkillName.MagicResist,
			SkillName.Meditation, SkillName.Parry, SkillName.Poisoning, SkillName.Stealing, SkillName.Swords, SkillName.Tactics,
			SkillName.Wrestling
		};

		private static readonly SkillName[] _InternalIgnoredSkills =
			((SkillName)0).GetValues<SkillName>().Not(_AcceptedSkills.Contains).ToArray();

		public TemplateSkillsGump(
			PlayerMobile user,
			Gump parent = null,
			int limit = 1,
			Action<GumpButton> onAccept = null,
			Action<GumpButton> onCancel = null,
			Action<SkillName[]> callback = null,
			params SkillName[] selected)
			: base(user, parent, limit, onAccept, onCancel, callback, _InternalIgnoredSkills)
		{
			if(selected != null)
			{
				SelectedSkills = new List<SkillName>(selected.Take(limit));
			}
			
			Modal = true;
			ForceRecompile = true;
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			if(Minimized)
			{
				return;
			}

			layout.AddReplace(
				"background/body/base",
				() =>
				{
					AddBackground(0, 55, 420, 210, 9270);
					AddImageTiled(10, 65, 400, 190, 2624);
				});
		}

		protected override void CompileEntryLayout(
			SuperGumpLayout layout, int length, int index, int pIndex, int yOffset, SkillName entry)
		{
			int xOffset = 0;

			if(pIndex < 6)
			{
				xOffset = 10;
			}
			else if(pIndex < 12)
			{
				xOffset = 145;
				yOffset = 70 + (pIndex - 6) * 30;
			}
			else if(pIndex < EntriesPerPage)
			{
				xOffset = 280;
				yOffset = 70 + (pIndex - 12) * 30;
			}

			layout.AddReplace(
				"check/list/select/" + index,
				() => AddButton(
					xOffset,
					yOffset,
					5033,
					5033,
					b =>
					{
						if(SelectedSkills.Contains(entry))
						{
							SelectedSkills.Remove(entry);
						}
						else
						{
							if(SelectedSkills.Count < Limit)
							{
								SelectedSkills.Add(entry);
							}
							else
							{
								new NoticeDialogGump(
									User,
									Refresh(true),
									title: "Limit Reached",
									html:
										"You have selected the maximum of " + Limit +
										" skills.\nIf you are happy with your selection, click the 'Okay' button.").Send();
								return;
							}
						}

						Refresh(true);
					}));

			if(SelectedSkills.Contains(entry))
			{
				layout.Add(
					"imagetiled/list/entry/" + index,
					() =>
					{
						AddImageTiled(xOffset, yOffset, 128, 28, 3004);
						AddImageTiled(4 + xOffset, 4 + yOffset, 120, 20, 2624);
					});
			}
			else
			{
				layout.Add("imagetiled/list/entry/" + index, () => AddImageTiled(xOffset, yOffset, 128, 28, 2624));
			}

			layout.Add(
				"html/list/entry/" + index,
				() =>
				AddHtml(
					4 + xOffset,
					4 + yOffset,
					120,
					20,
					GetLabelText(index, pIndex, entry).WrapUOHtmlTag("center").WrapUOHtmlColor(DefaultHtmlColor),
					false,
					false));
		}
	}
}