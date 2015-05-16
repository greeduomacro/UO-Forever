using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Server.Mobiles;
using System.IO;

namespace Server.Engines.XmlSpawner2
{
    public class ConditionalNode : UberNode
    {
        public ConditionalType IfType = ConditionalType.If;
        public MathTree ConditionalMathTree = null;

        public ConditionalNode(UberNode parent, string scriptInput) : base(parent, scriptInput) { }
        public ConditionalNode(UberNode parent, string scriptInput, int lineNumber) : base(parent, scriptInput, lineNumber) { }

        public enum ConditionalType
        {
            If,
            Elif,
            Else
        }
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
                if (value.StartsWith("if"))
                {
                    IfType = ConditionalType.If;
                    base.ScriptString = GetConditionalString(value, LineNumber);
                    try
                    {
                        ConditionalMathTree = new MathTree(null, base.ScriptString, LineNumber);
                    }
                    catch (Exception e)
                    {
                        throw new UberScriptException("Line " + LineNumber + ": " + value + "\nConditional MathTree parse error:", e);
                    }
                }
                else if (value.StartsWith("elif") || value.StartsWith("elseif"))
                {
                    IfType = ConditionalType.Elif;
                    base.ScriptString = GetConditionalString(value, LineNumber);
                    try
                    {
                        ConditionalMathTree = new MathTree(null, base.ScriptString, LineNumber);
                    }
                    catch (Exception e)
                    {
                        throw new UberScriptException("Line " + LineNumber + ": " + value + "\nConditional MathTree parse error:", e);
                    }
                }
                else if (value.StartsWith("else"))
                {
                    IfType = ConditionalType.Else;
                }
                else
                {
                    throw new UberScriptException("Line " + LineNumber + ": " + value + "\nConditional node did not start with if/elif/else!");
                }
            }
        }

        public static string GetConditionalString(string input, int lineNumber)
        {
            if (input[input.Length - 1] != ')')
                throw new UberScriptException("Line " + lineNumber + ": " + input + "\nConditional node did not end in )... parentheses are required!: " + input);

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '(')
                {
                    return input.Substring(i + 1, input.Length - (i + 2)); // take off the final )
                }
            }
            throw new UberScriptException("Line " + lineNumber + ": " + input + "\nConditional node did not have (... parentheses are required!: " + input);
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
                    // it was paused inside of the if statement, so just keep going
                    return ProcessChildren(trigObject);
                }
            }

            Object result;
            if (this.IfType == ConditionalType.If)
            {
                try
                {
                    result = ConditionalMathTree.Calculate(trigObject);
                }
                catch (Exception e)
                {
                    throw new UberScriptException("Line " + LineNumber + ": " + ScriptString + "\nConditionalNode Error:", e);
                }
                if (result is bool && (bool)result)
                {
                    lastResult = ProcessChildren(trigObject);
                    if (lastResult == ProcessResult.Return || lastResult == ProcessResult.ReturnOverride || lastResult == ProcessResult.Break || lastResult == ProcessResult.Continue)
                    {
                        return lastResult;
                    }
                    return ProcessResult.SucceedIf;
                }
                else
                {
                    return ProcessResult.FailedIf;
                }
            }
            else if (this.IfType == ConditionalType.Elif)
            {
                try
                {
                    result = ConditionalMathTree.Calculate(trigObject);
                }
                catch (Exception e)
                {
                    throw new UberScriptException("Line " + LineNumber + ": " + ScriptString + "\nConditionalNode Error:", e);
                }

                if (result is bool && (bool)result)
                {
                    lastResult = ProcessChildren(trigObject);
                    if (lastResult == ProcessResult.Return || lastResult == ProcessResult.ReturnOverride || lastResult == ProcessResult.Break || lastResult == ProcessResult.Continue)
                    {
                        return lastResult;
                    }
                    return ProcessResult.SucceedIf;
                }
                else
                {
                    return ProcessResult.FailedIf;
                }
            }
            else // this.IfType == ConditionalType.Else
            {
                lastResult = ProcessChildren(trigObject);
                if (lastResult == ProcessResult.Return || lastResult == ProcessResult.ReturnOverride || lastResult == ProcessResult.Break || lastResult == ProcessResult.Continue)
                {
                    return lastResult;
                }
            }
            return ProcessResult.None;
        }
    }
}