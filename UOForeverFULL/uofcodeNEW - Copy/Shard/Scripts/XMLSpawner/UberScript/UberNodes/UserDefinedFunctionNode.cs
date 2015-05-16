using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Server.Mobiles;
using System.IO;

namespace Server.Engines.XmlSpawner2
{
    public class UserDefinedFunctionNode : UberNode
    {
        public override string ScriptString
        {
            get
            {
                return base.ScriptString;
            }
            set
            {
                if (value == null)
                {
                    throw new UberScriptException("Line " + LineNumber + ": " + value + "\nLine Conditional node string was null!");
                }
                value = value.Trim();
                if (value.StartsWith("function"))
                {
                    base.ScriptString = value.Substring(8).ToLower();
                }
                else
                {
                    throw new UberScriptException("Line " + LineNumber + ": " + value + "\nUser-defined function node did not start with 'function'!");
                }
            }
        }
        public UserDefinedFunctionNode(UberNode parent, string scriptInput) : base(parent, scriptInput) { }
        public UserDefinedFunctionNode(UberNode parent, string scriptInput, int lineNumber) : base(parent, scriptInput, lineNumber) { }

        public ProcessResult Execute(TriggerObject trigObject)
        {
            return ProcessChildren(trigObject);
        }

        override public ProcessResult Process(ProcessResult lastSiblingResult, TriggerObject trigObject) // lastSiblingResult used for conditionals
        {
            return ProcessResult.None;
        }

        public static UserDefinedFunctionNode GetUserDefinedFunctionString(string input, RootNode root)
        {
            string potentialMatch = input.ToLower().Trim();
            foreach (UserDefinedFunctionNode node in root.UserDefinedFunctionNodes)
            {
                if (node.ScriptString == potentialMatch)
                {
                    return node;
                }
            }
            return null;
        }
    }

        
}