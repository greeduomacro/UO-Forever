using Server.Mobiles;
using VitaNex;
using VitaNex.Notify;

namespace Server.Scripts.New.Adam
{
    public static partial class NotifyIgnore
    {
        public static void Configure()
        {
            CommandUtility.Register("StaffMessages", AccessLevel.Player, e => StaffMessages(e.Mobile as PlayerMobile));
        }

        private static void StaffMessages(PlayerMobile user)
        {
            if (user == null)
            {
                return;
            }

            if (user.EventMsgFlag)
            {
                user.SendMessage(54, "Currently Disabled.");
                //user.EventMsgFlag = false;
                //user.SendMessage(54, "You have chosen to opt-out of staff notifies and event pop-ups. Type [staffmessages to opt back in.");
            }
            else if (!user.EventMsgFlag)
            {
                user.SendMessage(54, "Currently Disabled.");
                //user.EventMsgFlag = true;
                //user.SendMessage(54, "You have chosen to opt-in to staff notifies and event pop-ups. Type [staffmessages to opt back out.");               
            }
        }
    }
}
