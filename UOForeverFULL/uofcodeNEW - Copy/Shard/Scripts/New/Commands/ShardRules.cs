#region References



#endregion

namespace Server.Commands
{
    public class ShardRules
    {
        public static void Initialize()
        {
            CommandSystem.Register("ShardRules", AccessLevel.Player, ShardRules_OnCommand);
        }

        [Usage("ShardRules")]
        [Description("Displays the shard rules page.")]
        public static void ShardRules_OnCommand(CommandEventArgs e)
        {
            e.Mobile.NetState.LaunchBrowser("http://uoforever.com/rules.php");
        }
    }
}