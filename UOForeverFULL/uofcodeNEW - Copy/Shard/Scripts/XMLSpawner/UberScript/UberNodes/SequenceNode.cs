using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Server.Mobiles;
using System.IO;

namespace Server.Engines.XmlSpawner2
{
    public class SequenceNode : UberNode
    {
        public int Stage = -1;
        
        public SequenceNode(UberNode parent, string scriptInput, int stage) : base(parent, scriptInput) 
        {
            Stage = stage;
        }

        public override ProcessResult Process(ProcessResult lastSiblingResult, TriggerObject trigObject)
        {
            return ProcessChildren(trigObject);
            //return ProcessResult.None;
        }
    }
}