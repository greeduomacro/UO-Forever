using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Server.Mobiles;
using System.IO;

namespace Server.Engines.XmlSpawner2
{
    public class ArgumentNode : UberNode
    {
        public ArgumentNode(UberNode parent, string scriptInput) : base(parent, scriptInput) { }
    }
}