using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
    public class SpawnHandlers
    {
        public static object Spawn(string spawnString, object caller)
        {
            if (spawnString == null || spawnString == "") return null;

            int commaIndex = spawnString.IndexOf(',');
            Type type;
            if (commaIndex != -1) type = ScriptCompiler.FindTypeByName(spawnString.Substring(0, commaIndex));
            else type = ScriptCompiler.FindTypeByName(spawnString);

            // do not allow mobiles to be spawned into item backpack
            if (type != null)
            {
                object o = XmlSpawner.CreateObject(type, spawnString);
                if (o == null)
                {
                    throw new UberScriptException("Could not create spawn: " + spawnString + "... should be impossible!");
                }
                return o;
            }
            return null;
        }
    }
}
