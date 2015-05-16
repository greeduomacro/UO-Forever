using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Server.Mobiles;
using System.IO;

namespace Server.Engines.XmlSpawner2
{
    public class SpawnEntryNode : UberNode
    {
        public MathTree ConditionalMathTree = null;

        public SpawnEntryNode(UberNode parent, string scriptInput) : base(parent, scriptInput) { }
        public SpawnEntryNode(UberNode parent, string scriptInput, int lineNumber) : base(parent, scriptInput, lineNumber) { }

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
                    throw new UberScriptException("Line " + LineNumber + ": " + value + "\nLine SpawnEntry node string was null!");
                }
                value = value.Trim();
                
                base.ScriptString = GetSpawnEntryString(value, LineNumber);

            }
        }

        public static string GetSpawnEntryString(string input, int lineNumber)
        {
            if (input[input.Length - 1] != ')')
                throw new UberScriptException("Line " + lineNumber + ": " + input + "\nSpawnEntry node did not end in )... parentheses are required!: " + input);

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '(')
                {
                    return input.Substring(i + 1, input.Length - (i + 2)); // take off the final )
                }
            }
            throw new UberScriptException("Line " + lineNumber + ": " + input + "\nSpawnEntry node did not have (... parentheses are required!: " + input);
        }

        public override ProcessResult Process(ProcessResult lastSiblingResult, TriggerObject trigObject)
        {
            ProcessResult lastResult = ProcessResult.None;
            // NOTE: I commented out the try here because
            if (trigObject.PausedNodeChain != null)
            {
                if (trigObject.PausedNodeChain.Count == 1 && trigObject.PausedNodeChain.Peek() == this)
                {
                    trigObject.PausedNodeChain = null;
                }
                else
                {
                    // it was paused inside of the spawnentry statement, so just keep going
                    return ProcessChildren(trigObject);
                }
            }

            lastResult = ProcessChildren(trigObject);
            if (lastResult == ProcessResult.Return || lastResult == ProcessResult.ReturnOverride || lastResult == ProcessResult.Break || lastResult == ProcessResult.Continue)
            {
                return lastResult;
            }
            return ProcessResult.None;
        }
    }
}