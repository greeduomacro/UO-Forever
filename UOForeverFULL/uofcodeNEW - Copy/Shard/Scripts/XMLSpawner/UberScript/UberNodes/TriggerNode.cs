using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Server.Mobiles;
using System.IO;

namespace Server.Engines.XmlSpawner2
{
    public class TriggerNode : UberNode
    {
        public TriggerName TrigName = TriggerName.NoTrigger;
        override public string ScriptString { get { return base.ScriptString; } set { TrigName = (TriggerName)Enum.Parse(typeof(TriggerName), value, true); base.ScriptString = value; } }

        public TriggerNode(UberNode parent, string scriptInput) : base(parent, scriptInput) { }

        public override ProcessResult Process(ProcessResult lastSiblingResult, TriggerObject trigObject)
        {
            return ProcessChildren(trigObject);
            //return ProcessResult.None;
        }
    }
}