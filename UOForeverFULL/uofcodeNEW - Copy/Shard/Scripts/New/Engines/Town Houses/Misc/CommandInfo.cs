#region References

using Server;

#endregion

namespace Knives.TownHouses
{
    public delegate void TownHouseCommandHandler(CommandInfo info);

    public class CommandInfo
    {
        private readonly Mobile c_Mobile;
        private readonly string c_Command;
        private readonly string c_ArgString;
        private readonly string[] c_Arguments;

        public Mobile Mobile
        {
            get { return c_Mobile; }
        }

        public string Command
        {
            get { return c_Command; }
        }

        public string ArgString
        {
            get { return c_ArgString; }
        }

        public string[] Arguments
        {
            get { return c_Arguments; }
        }

        public CommandInfo(Mobile m, string com, string args, string[] arglist)
        {
            c_Mobile = m;
            c_Command = com;
            c_ArgString = args;
            c_Arguments = arglist;
        }

        public string GetString(int num)
        {
            return c_Arguments.Length > num ? c_Arguments[num] : "";
        }
    }
}