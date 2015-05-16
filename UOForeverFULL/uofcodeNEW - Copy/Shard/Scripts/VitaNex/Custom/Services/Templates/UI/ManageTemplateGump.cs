#region References

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Server.Gumps;
using Server.Mobiles;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.PvPTemplates
{
    public class ManageTemplateGump : DialogGump
    {
        public TemplateProfile Profile { get; set; }

        public string TemplateName { get; set; }
        public string TemplateNotes { get; set; }

        public SkillName[] TemplateSkills { get; set; }

        public int TemplateStr { get; set; }
        public int TemplateDex { get; set; }
        public int TemplateInt { get; set; }

        public Template Template { get; set; }

        public bool EditMode
        {
            get { return Template != null; }
        }

        public bool LockMode
        {
            get
            {
                return Profile == null || Profile.Deleted || Profile.Owner == null || Profile.Owner.Deleted ||
                       (EditMode && !Profile.Contains(Template));
            }
        }

        public ManageTemplateGump(PlayerMobile user, Gump parent = null, TemplateProfile profile = null,
            Template edit = null)
            : base(user, parent, null, null, null, null, 7020)
        {
            Profile = profile ?? PvPTemplates.FetchProfile(user);

            Modal = true;
            ForceRecompile = true;

            if(LockMode)
            {
                return;
            }

            if(edit == null || !Profile.Contains(edit))
            {
                Title = "Create Template";

                TemplateName = "New Template";
                TemplateNotes = "Description/Notes";

                TemplateSkills = new SkillName[0];

                TemplateStr = TemplateDex = TemplateInt = 0;
            }
            else
            {
                Title = "Edit Template";

                Template = edit;

                TemplateName = Template.Name;
                TemplateNotes = Template.Notes;

                TemplateSkills = Template.GetActiveSkills().ToArray();

                TemplateStr = Template.Stats[StatType.Str];
                TemplateDex = Template.Stats[StatType.Dex];
                TemplateInt = Template.Stats[StatType.Int];
            }
        }

        protected override void Compile()
        {
            if(LockMode)
            {
                Title = "Profile Unavailable";

                TemplateName = "N/A";
                TemplateNotes = "N/A";

                TemplateStr = TemplateDex = TemplateInt = 0;

                TemplateSkills = new SkillName[0];
            }

            base.Compile();
        }

        protected override void CompileLayout(SuperGumpLayout layout)
        {
            base.CompileLayout(layout);

            layout.Remove("html/body/info");

            //Name
            layout.Add(
                "name",
                () =>
                {
                    AddLabel(100, 70, HighlightHue, "Name:");
                    AddBackground(100, 90, 150, 30, 9350);

                    if(LockMode)
                    {
                        AddLabelCropped(102, 100, 150, 20, ErrorHue, TemplateName);
                    }
                    else
                    {
                        AddTextEntryLimited(102, 100, 150, 20, TextHue, TemplateName, 20, (b, t) => TemplateName = t);
                    }
                });

            //Notes
            layout.Add(
                "notes",
                () =>
                {
                    AddLabel(25, 140, HighlightHue, "Notes:");
                    AddBackground(25, 160, 270, 125, 9350);

                    if(LockMode)
                    {
                        AddLabelCropped(27, 165, 265, 125, ErrorHue, TemplateName);
                    }
                    else
                    {
                        AddTextEntryLimited(27, 165, 265, 125, TextHue, TemplateNotes, 1000, (b, t) => TemplateNotes = t);
                    }
                });

            //Str
            layout.Add(
                "stat/str",
                () =>
                {
                    AddLabel(300, 100, HighlightHue, "Str:");
                    AddBackground(328, 100, 40, 20, 9350);

                    if(LockMode)
                    {
                        AddLabelCropped(330, 100, Width - 120, 20, ErrorHue, TemplateName);
                    }
                    else
                    {
                        AddTextEntryLimited(
                            330,
                            100,
                            Width - 120,
                            20,
                            TextHue,
                            TemplateStr.ToString(CultureInfo.InvariantCulture),
                            3,
                            (b, t) =>
                            {
                                int sStr;

                                if(Int32.TryParse(t, out sStr))
                                {
                                    TemplateStr = Math.Max(0, Math.Min(100, sStr));
                                }
                            });
                    }
                });

            //Dex
            layout.Add(
                "stat/dex",
                () =>
                {
                    AddLabel(300, 120, HighlightHue, "Dex:");
                    AddBackground(328, 120, 40, 20, 9350);

                    if(LockMode)
                    {
                        AddLabelCropped(330, 120, Width - 120, 20, ErrorHue, TemplateName);
                    }
                    else
                    {
                        AddTextEntryLimited(
                            330,
                            120,
                            Width - 120,
                            20,
                            TextHue,
                            TemplateDex.ToString(CultureInfo.InvariantCulture),
                            3,
                            (b, t) =>
                            {
                                int sDex;

                                if(Int32.TryParse(t, out sDex))
                                {
                                    TemplateDex = Math.Max(0, Math.Min(100, sDex));
                                }
                            });
                    }
                });

            //Int
            layout.Add(
                "stat/int",
                () =>
                {
                    AddLabel(300, 140, HighlightHue, "Int:");
                    AddBackground(328, 140, 40, 20, 9350);

                    if(LockMode)
                    {
                        AddLabelCropped(330, 140, Width - 120, 20, ErrorHue, TemplateName);
                    }
                    else
                    {
                        AddTextEntryLimited(
                            330,
                            140,
                            Width - 120,
                            20,
                            TextHue,
                            TemplateInt.ToString(CultureInfo.InvariantCulture),
                            3,
                            (b, t) =>
                            {
                                int sInt;

                                if(Int32.TryParse(t, out sInt))
                                {
                                    TemplateInt = Math.Max(0, Math.Min(100, sInt));
                                }
                            });
                    }
                });

            //menu
            layout.Add(
                "skills",
                () =>
                {
                    if(LockMode)
                    {
                        AddImage(300, 70, 2017);
                    }
                    else
                    {
                        AddButton(
                            300,
                            70,
                            2017,
                            2016,
                            button => Send(
                                new TemplateSkillsGump(User, this, 7, selected: TemplateSkills)
                                {
                                    Callback = skills =>
                                    {
                                        TemplateSkills = skills;
                                        Refresh(true);
                                    }
                                }));
                    }
                });
        }

        protected override void OnAccept(GumpButton button)
        {
            base.OnAccept(button);

            if(LockMode)
            {
                Close();
                return;
            }

            if(String.IsNullOrWhiteSpace(TemplateName))
            {
                Send(
                    new NoticeDialogGump(User, Hide(true))
                    {
                        Title = "Choose a Template Name",
                        Html = "Your template must have a name!",
                        AcceptHandler = b => Refresh(true)
                    });
                return;
            }

            if(TemplateSkills.Length < 7)
            {
                Send(
                    new NoticeDialogGump(User, Hide(true))
                    {
                        Title = "Choose 7 Skills",
                        Html = "You must choose 7 unique skills to create a template.",
                        AcceptHandler = b => Refresh(true)
                    });
                return;
            }

            if(TemplateStr <= 0 || TemplateDex <= 0 || TemplateInt <= 0)
            {
                Send(
                    new NoticeDialogGump(User, Hide(true))
                    {
                        Title = "Choose 3 Stats",
                        Html = "You must specify your three character stats.",
                        AcceptHandler = b => Refresh(true)
                    });
                return;
            }

            if(TemplateStr + TemplateDex + TemplateInt > 225)
            {
                Send(
                    new NoticeDialogGump(User, Hide(true))
                    {
                        Title = "Stat Failure",
                        Html =
                            "Either one of your stats is above the 100 limit, " +
                            "or you have exceeded the overall limit of 225 stat points with your current allocation.",
                        AcceptHandler = b => Refresh(true)
                    });
                return;
            }

            Dictionary<SkillName, double> skills = Profile.Owner.Skills.Not(sk => sk == null)
                .ToDictionary(sk => sk.SkillName, sk => TemplateSkills.Contains(sk.SkillName) ? 100.0 : 0.0);
            var stats = new Dictionary<StatType, int>
            {
                {
                    StatType.Str, TemplateStr
                },
                {
                    StatType.Dex, TemplateDex
                },
                {
                    StatType.Int, TemplateInt
                }
            };

            if(EditMode)
            {
                Template.Name = TemplateName;
                Template.Notes = TemplateNotes;
                Template.Skills = skills;
                Template.Stats = stats;

                Profile.Add(Template);
            }
            else
            {
                Template = Profile.Create(TemplateName, TemplateNotes, skills, stats);
                if(Profile.Selected == null)
                    Profile.Select(Template);
            }
            Close();
        }

        protected override void OnCancel(GumpButton button)
        {
            base.OnCancel(button);

            if(LockMode)
            {
                Close();
            }
        }
    }
}