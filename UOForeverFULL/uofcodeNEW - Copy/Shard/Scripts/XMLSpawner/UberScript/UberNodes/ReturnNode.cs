using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Server.Mobiles;
using System.IO;

namespace Server.Engines.XmlSpawner2
{
    public class ReturnNode : UberNode
    {
        public bool Override = false;
        
        public ReturnNode(UberNode parent, string scriptInput) : base(parent, scriptInput) { }
        public ReturnNode(UberNode parent, string scriptInput, int lineNumber) : base(parent, scriptInput, lineNumber) { }

        public override ProcessResult Process(ProcessResult lastSiblingResult, TriggerObject trigObject)
        {
            if (Override) return ProcessResult.ReturnOverride;
            return ProcessResult.Return;
        }
    }
}