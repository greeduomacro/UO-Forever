using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Server.Mobiles;
using System.IO;

namespace Server.Engines.XmlSpawner2
{
    public class ContinueNode : UberNode
    {
        public ContinueNode(UberNode parent, string scriptInput) : base(parent, scriptInput) { }

        override public ProcessResult Process(ProcessResult lastSiblingResult, TriggerObject trigObject) // lastSiblingResult used for conditionals
        {
            return ProcessResult.Continue;
        }
    }
}