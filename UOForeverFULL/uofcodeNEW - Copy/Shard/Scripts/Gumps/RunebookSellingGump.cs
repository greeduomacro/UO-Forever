using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Network;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Seventh;
using Server.Prompts;
using Server.Engines.XmlSpawner2;

namespace Server.Gumps
{
    public class RunebookSellingGump : Gump
    {   
        public RunebookSellingGump(Runebook book)
            : base(0, 0)
        {
            this.Dragable = false;
            this.Closable = true;

            if (book == null) return;

            AddPage(0);
            this.AddBackground(0, 0,520, 420, 5170); 
            int penY = 25;
            int penX = 125;
            int textHeight = 20;
            AddLabel(penX, penY, 38, "Runes in this Runebook");
            penY += textHeight + 5;
            penX = 30;

            if (book.Entries == null || book.Entries.Count == 0)
            {
                AddLabel(penX, penY, 0, "This is an empty runebook...");
                return;
            }

            AddLabel(penX, penY, 38, "  Map      Region        Coordinates            Description");
            penY += textHeight;
            foreach (RunebookEntry entry in book.Entries)
            {
                if (entry.Map == null) continue;

                Region region = Region.Find(entry.Location, entry.Map);
                AddLabel(penX, penY, 0, entry.Map.Name);
                penX = 95;
                if (region != null)  AddLabel(penX, penY, 0,  region.Name);
                penX = 200;
                AddLabel(penX, penY, 0, UberScriptFunctions.Methods.SEXTANTCOORDS(null, entry.Location, entry.Map));
                penX = 340;
                AddLabel(penX, penY, 0, entry.Description);

                penY += textHeight;
                penX = 30;
            }
        }
        /* This is the enum of the Buttonss. */
        /* Gump Studio uses this form when creating the scripts of the gumps.
         * I find it (in most cases) easier to keep track of the buttons by using this method
         * so I can give each button a name. */
        /* One thing GumpStudio does not do is add in the 'Exit' button.
         * The first button in this list below is going to have an int value of 0, being the Exit button.
         * GumpStudio does not take that into account, so I just add in a new entry of 'Exit' */
        public enum Buttonss
        {
            Exit,
            Page2Button,
            ReplyButton,
        }
    }
}