#region References

using System;
using System.Linq;
using Server.Accounting;
using Server.Commands;
using Server.Network;

#endregion

namespace Server.Gumps
{
    public class PasswordChanger : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("password", AccessLevel.Player, Password_OnCommand);
        }

        public static void Password_OnCommand(CommandEventArgs e)
        {
            e.Mobile.CloseGump(typeof(PasswordChanger));
            e.Mobile.SendGump(new PasswordChanger());
        }

        public PasswordChanger() : base(0, 0)
        {
            Closable = true;
            Disposable = true;
            Dragable = false;
            Resizable = false;
            AddPage(0);

            AddBackground(133, 77, 414, 206, 3600);

            AddLabel(279, 97, 1178, @"Password Changer");
            AddLabel(160, 130, 1299, @"Current Password:");
            AddLabel(160, 160, 1299, @"New Password:");
            AddLabel(160, 190, 1299, @"Confirm New Password:");

            AddButton(306, 230, 247, 248, 1, GumpButtonType.Reply, 0);

            AddAlphaRegion(321, 190, 200, 20);
            AddAlphaRegion(321, 160, 200, 20);
            AddAlphaRegion(321, 130, 200, 20);

            AddTextEntry(321, 130, 200, 20, 1175, 1, "", 16);
            AddTextEntry(321, 160, 200, 20, 1175, 2, "", 16);
            AddTextEntry(321, 190, 200, 20, 1175, 3, "", 16);
        }

        private string GetString(RelayInfo info, int id)
        {
            TextRelay t = info.GetTextEntry(id);
            return (t == null ? null : t.Text.Trim());
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile m = sender.Mobile;

            if (m == null)
                return;
            switch (info.ButtonID)
            {
                case 0:
                {
                    m.SendMessage(1300, "Your password has not changed.");
                    return;
                }
                case 1:
                {
                    string origPass = GetString(info, 1);
                    string newPass = GetString(info, 2);
                    string confirmNewPass = GetString(info, 3);

                    if (String.IsNullOrWhiteSpace(origPass) || String.IsNullOrWhiteSpace(newPass) ||
                        String.IsNullOrWhiteSpace(confirmNewPass))
                        return;

                    if (newPass != confirmNewPass) //Two "New password" fields do not match
                    {
                        m.SendMessage(37,
                            "The 'Confirm New Password' value does not match the 'New Password'. Remember it is cAsE sEnSaTiVe. ");
                        m.CloseGump(typeof(PasswordChanger));
                        m.SendGump(new PasswordChanger());
                        return;
                    }

                    if (newPass.Any(t => !(char.IsLetterOrDigit(t))))
                    {
                        m.SendMessage(37, "Passwords may only consist of letters (A - Z) and Digits (0 - 9).");
                        return;
                    }

                    var a = m.Account as Account;

                    if (a != null && !(a.CheckPassword(origPass))) //"Current Password" value is incorrect
                    {
                        m.SendMessage(37, "The 'Current Password' value is incorrect. [ " + origPass + " ].");
                        m.CloseGump(typeof(PasswordChanger));
                        m.SendGump(new PasswordChanger());
                        return;
                    }

                    if (a != null && ((a.CheckPassword(origPass)) && (newPass == confirmNewPass)))
                        //Current password is correct, and 2 "New Password" fields match.
                    {
                        a.SetPassword(newPass);
                        m.SendMessage(68,
                            "Your account ( " + a.Username + " ) password has been changed to '" + newPass +
                            "'. Remember this!");
                    }
                    break;
                }
            }
        }
    }
}