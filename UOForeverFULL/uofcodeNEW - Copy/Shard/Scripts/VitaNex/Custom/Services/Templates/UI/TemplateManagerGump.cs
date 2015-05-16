#region References

using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Mobiles;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
using VitaNex.Text;

#endregion

namespace Server.PvPTemplates
{
    public sealed class TemplateManagerGump : ListGump<Template>
    {
        public TemplateProfile Profile { get; set; }

        public TemplateManagerGump(PlayerMobile user, Gump parent = null, TemplateProfile profile = null)
            : base(user, parent, null, 100, emptyText: "No Templates to Display.", title: "Template Manager")
        {
            Profile = profile ?? PvPTemplates.FetchProfile(user);

            EntriesPerPage = 2;

            Modal = true;
            ForceRecompile = true;
        }

        protected override void Compile()
        {
            if(Profile == null)
            {
                Profile = PvPTemplates.FetchProfile(User);
            }

            base.Compile();
        }

        protected override void CompileList(List<Template> list)
        {
            list.Clear();
            list.TrimExcess();

            if(Profile != null)
            {
                list.AddRange(Profile);
            }

            base.CompileList(list);
        }

        protected override void CompileMenuOptions(MenuGumpOptions list)
        {
            if(Profile != null)
            {
                list.AppendEntry(
                    new ListGumpEntry("Create Template", b => Send(new ManageTemplateGump(User, Hide(true), Profile)
                    {
                        AcceptHandler = a => Refresh(true),
                        CancelHandler = c => Refresh(true)
                    }),
                        HighlightHue));

                list.AppendEntry(
                    new ListGumpEntry(
                        "Delete All",
                        b => Send(
                            new ConfirmDialogGump(User, Hide(true))
                            {
                                Title = "Delete All Templates",
                                Html =
                                    "You are about to delete all templates.\nThis action can not be undone!\nClick OK to continue.",
                                AcceptHandler = ba =>
                                {
                                    Profile.Clear();
                                    Refresh(true);
                                }
                            }),
                        ErrorHue));
            }

            base.CompileMenuOptions(list);
        }

        protected override void CompileLayout(SuperGumpLayout layout)
        {
            base.CompileLayout(layout);

            layout.Add(
                "tile/body/header/1",
                () =>
                    AddLabelCropped(
                        220, 16, 165, 50, 77,
                        String.Format("[{0}]", Profile.Selected != null ? Profile.Selected.Name : "N/A")));

            if(Minimized)
            {
                return;
            }

            Dictionary<int, Template> range = GetListRange();

            if(range.Count == 0)
            {
                layout.AddReplace(
                    "background/body/base",
                    () =>
                    {
                        AddBackground(0, 55, 420, 300, 9270);
                        AddImageTiled(10, 65, 400, 280, 2624);
                    });

                layout.Remove("imagetiled/body/vsep/0");
            }
            else
            {
                layout.AddReplace(
                    "background/body/base",
                    () =>
                    {
                        AddBackground(0, 55, 420, 20 + (range.Count * 220), 9270);
                        AddImageTiled(10, 65, 400, (range.Count * 220), 2624);
                    });

                layout.Remove("imagetiled/body/vsep/0");
            }
        }

        protected override void CompileEntryLayout(
            SuperGumpLayout layout, int length, int index, int pIndex, int yOffset, Template entry)
        {
            if(Profile == null)
            {
                return;
            }

            yOffset = yOffset + pIndex * 192;

            base.CompileEntryLayout(layout, length, index, pIndex, yOffset, entry);

            layout.Add("imagetiled/entry/vtit/" + index, () => AddImageTiled(100, yOffset - 4, 5, 25, 9275));
            layout.Add("imagetiled/entry/htit/" + index, () => AddImageTiled(10, yOffset + 20, 93, 5, 9277));

            //Deletes a template
            layout.Add("button/list/delete/" + index,
                () => AddButton(379, yOffset - 3, 4017, 4018, b => DeleteEntry(entry)));

            layout.Remove("button/list/select/" + index);
            layout.AddReplace(
                "label/list/entry/" + index,
                () =>
                    AddLabelCropped(15, yOffset, 75, 50, GetLabelHue(index, pIndex, entry),
                        GetLabelText(index, pIndex, entry)));

            //START STATS
            layout.Add(
                "label/list/entry/stats1" + index,
                () => AddLabelCropped(110, yOffset, 75, 50, 33, "Str: " + entry.Stats[StatType.Str]));

            layout.Add(
                "label/list/entry/stats2" + index,
                () => AddLabelCropped(180, yOffset, 75, 50, 33, "Dex: " + entry.Stats[StatType.Dex]));

            layout.Add(
                "label/list/entry/stats3" + index,
                () => AddLabelCropped(250, yOffset, 75, 50, 33, "Int: " + entry.Stats[StatType.Int]));
            //END STATS

            layout.Add(
                "html/body/info" + index,
                () => AddHtml(103, yOffset + 22, 190, 185, entry.Notes.ParseBBCode(DefaultHtmlColor, 3), true, true));

            layout.Add("image/list/entry/skills" + index, () => AddImage(310, yOffset, 2105));

            int iconIndex = 0;
            int idx = 0;

            foreach(SkillName skill in entry.GetActiveSkills())
            {
                int subIndex = ++idx;
                string skillName = Profile.Owner != null
                    ? Profile.Owner.Skills[skill].Name
                    : skill.ToString().SpaceWords();

                layout.Add(
                    "label/list/entry/skill/" + index + "/" + subIndex,
                    () =>
                        AddLabelCropped(320, yOffset + (23 * subIndex), 80, 50, GetLabelHue(index, pIndex, entry),
                            skillName));

                if((iconIndex != 0 && iconIndex != 1) ||
                   (skill != SkillName.Macing && skill != SkillName.Archery && skill != SkillName.Fencing &&
                    skill != SkillName.Swords && skill != SkillName.Magery))
                {
                    continue;
                }

                int ySubOffset = 0;

                switch(++iconIndex)
                {
                    case 1:
                        ySubOffset = yOffset + 23;
                        break;
                    case 2:
                        ySubOffset = yOffset + 88;
                        break;
                }

                layout.Add(
                    "image/list/entry/skillicon/" + index + "/" + iconIndex,
                    () => AddImage(11, ySubOffset, GetIcon(skill), 0));
            }

            layout.Add(
                "button/list/choose/" + index,
                () => AddButton(15, yOffset + 165, 2124, 2122, button => SelectEntry(button, entry)));


            layout.Add(
                "button/list/choose/edit" + index,
                () =>
                    AddButton(18, yOffset + 190, 4024, 4025,
                        b => Send(new ManageTemplateGump(User, Hide(true), Profile, entry)
                        {
                            AcceptHandler = a => Refresh(true),
                            CancelHandler = c => Refresh(true)
                        })));

            layout.Add("label/list/entry/edit" + index, () => AddLabelCropped(52, yOffset + 192, 75, 50, 33, "Edit"));

            if(pIndex < (length))
            {
                layout.AddReplace("imagetiled/body/hsep/" + index, () => AddImageTiled(10, yOffset - 8, 400, 5, 9277));
            }
        }

        protected override int GetLabelHue(int index, int pageIndex, Template entry)
        {
            return entry != null ? (entry == Profile.Selected ? HighlightHue : TextHue) : ErrorHue;
        }

        protected override string GetLabelText(int index, int pageIndex, Template entry)
        {
            return entry != null ? entry.Name : "N/A";
        }

        public override string GetSearchKeyFor(Template key)
        {
            return key != null ? key.Name : base.GetSearchKeyFor(null);
        }

        protected override void SelectEntry(GumpButton button, Template entry)
        {
            base.SelectEntry(button, entry);

            if(Profile != null)
            {
                Profile.Select(entry);
            }

            Refresh(true);
        }

        private void DeleteEntry(Template entry)
        {
            if(Profile == null)
            {
                return;
            }

            if(entry == null || !Profile.Contains(entry))
            {
                if(Selected == entry)
                {
                    Selected = null;
                }

                Refresh(true);
                return;
            }

            Send(
                new ConfirmDialogGump(User, this)
                {
                    Title = String.Format("Delete The Template: {0}?", entry.Name),
                    Html = "Deleting this template is permanent.\nDo you wish to continue?",
                    AcceptHandler = b =>
                    {
                        Profile.Remove(entry);

                        if(Selected == entry)
                        {
                            Selected = null;
                        }

                        Refresh(true);
                    },
                    CancelHandler = c => Refresh()
                });
        }

        private static int GetIcon(SkillName skill)
        {
            switch(skill)
            {
                case SkillName.Swords:
                    return 1007;
                case SkillName.Archery:
                    return 1029;
                case SkillName.Fencing:
                    return 1008;
                case SkillName.Macing:
                    return 1009;
                case SkillName.Magery:
                    return 1006;
            }

            return 0;
        }
    }
}