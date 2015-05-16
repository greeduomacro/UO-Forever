using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Server.Mobiles;
using System.IO;

namespace Server.Engines.XmlSpawner2
{
    public class PauseNode : UberNode
    {
        public double PauseMS;

        public PauseNode(UberNode parent, string scriptInput, double pause) : base(parent, scriptInput) { PauseMS = pause; }

        override public ProcessResult Process(ProcessResult lastSiblingResult, TriggerObject trigObject) // lastSiblingResult used for conditionals
        {
            return ProcessResult.Pause;
        }
    }
}