using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Server.Mobiles;
using System.IO;

namespace Server.Engines.XmlSpawner2
{
    public class UserDefinedFunctionExectueNode : UberNode
    {
        public UserDefinedFunctionExectueNode(UberNode parent, string scriptInput) : base(parent, scriptInput) { }
        public UserDefinedFunctionExectueNode(UberNode parent, string scriptInput, int lineNumber) : base(parent, scriptInput, lineNumber) { }

        public UserDefinedFunctionNode UserDefinedFunction;

        override public ProcessResult Process(ProcessResult lastSiblingResult, TriggerObject trigObject)
        {
            trigObject.CurrentNodeExecutionChain.Add(this);

            ProcessResult result = UserDefinedFunction.Execute(trigObject);

            trigObject.CurrentNodeExecutionChain.Remove(this);
            return result;
        }

        public static UserDefinedFunctionNode GetUserDefinedFunctionString(string input, RootNode root)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '(')
                {
                    string potentialMatch = input.Substring(0, i).Trim().ToLower();
                    foreach (UserDefinedFunctionNode node in root.UserDefinedFunctionNodes)
                    {
                        if (node.ScriptString == potentialMatch)
                        {
                            return node;
                        }   
                    }
                }
            }
            return null;
        }
    }

        
}