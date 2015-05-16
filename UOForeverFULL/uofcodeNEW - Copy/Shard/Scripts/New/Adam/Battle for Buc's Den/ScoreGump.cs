// **********
// RunUO Shard - ScoreGump.cs
// **********

#region References

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Server.Gumps;
using Server.Network;

#endregion

namespace Server.Scripts.New.Adam
{
    public class ScoreGumpBbd : Gump
    {
        public ScoreGumpBbd(List<KeyValuePair<Mobile, double>> myList)
            : base(300, 200)
        {
            Closable = true;
            Disposable = false;
            Dragable = true;
            Resizable = false;

            //container
            AddBackground(0, 0, 350, 650, 2600);
            AddLabel(103, 20, 2066, "Top 10 Damage Dealers");
            AddLabel(80, 45, 1287, "The Battle for Bucaneer's Den");
            AddItem(40, 40, 5185);
            AddItem(257, 40, 5185);
            //gold
            AddItem(32, 70, 0x1224, 1177);
            AddItem(266, 70, 0x1224, 1177);
            //silver
            AddItem(32, 120, 0x1224, 1150);
            AddItem(266, 120, 0x1224, 1150);
            //bronze
            AddItem(32, 170, 0x1224, 1126);
            AddItem(266, 170, 0x1224, 1126);

            var k = 0;
            foreach (var kvp in myList.Take(10))
            {
                k++;
                if (k == 1)
                {
                    AddLabel(75, 80, 2049, @k + ". " + kvp.Key.Name);
                    AddLabel(205, 80, 1194, kvp.Value.ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    AddLabel(75, (50*(k - 1)) + 80, 2049, @k + ". " + kvp.Key.Name);
                    AddLabel(205, (50*(k - 1)) + 80, 1194, kvp.Value.ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
        }
    }
}