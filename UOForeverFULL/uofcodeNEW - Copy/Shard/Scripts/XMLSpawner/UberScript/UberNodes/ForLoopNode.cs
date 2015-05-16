using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Server.Mobiles;
using System.IO;

namespace Server.Engines.XmlSpawner2
{
    public class ForLoopNode : UberNode
    {
        public MathTree ConditionalMathTree = null;
        public StatementNode InitialStatement = null;
        public StatementNode RepeatedStatement = null;
        public bool InfiniteLoopRisk = false;

        public ForLoopNode(UberNode parent, string scriptInput) : base(parent, scriptInput) { }
        public ForLoopNode(UberNode parent, string scriptInput, int lineNumber) : base(parent, scriptInput, lineNumber) { }
        

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
                    throw new UberScriptException("Line " + LineNumber + ": ForLoop node string was null!");
                }
                base.ScriptString = value;
                value = value.Trim();
                value = ConditionalNode.GetConditionalString(value, LineNumber);
                string[] splitString = value.Split(';');
                if (splitString.Length != 3)
                {
                    throw new UberScriptException("Line " + LineNumber + ": " + base.ScriptString + "\nForLoop node did not have 3 parts to it separated by semi-colons!");
                }
                if (splitString[0].Length > 0)
                {
                    try
                    {
                        InitialStatement = new StatementNode(null, splitString[0], LineNumber); // could throw error
                    }
                    catch (Exception e)
                    {
                        throw new UberScriptException("Line " + LineNumber + ": " + splitString[0] + "\nForLoop Initial Statement Parse Error:", e);
                    }
                }
                if (splitString[1].Length > 0)
                {
                    try
                    {
                        ConditionalMathTree = new MathTree(null, splitString[1], LineNumber);
                    }
                    catch (Exception e)
                    {
                        throw new UberScriptException("Line " + LineNumber + ": " + splitString[1] + "\nForLoop Condition Parse Error:", e);
                    }
                }
                else
                {
                    throw new UberScriptException("Line " + LineNumber + ": " + base.ScriptString + "\nForLoop node had no conditional statement (the middle part)!");
                }
                if (splitString[2].Length > 0)
                {
                    try
                    {
                        RepeatedStatement = new StatementNode(null, splitString[2], LineNumber);
                    }
                    catch (Exception e)
                    {
                        throw new UberScriptException("Line " + LineNumber + ": " + splitString[2] + "\nForLoop Repeated Statement Parse Error:", e);
                    }
                }
            }
        }

        public override ProcessResult Process(ProcessResult lastSiblingResult, TriggerObject trigObject)
        {
            if (InfiniteLoopRisk == true)
                throw new UberScriptException("Line " + LineNumber + ": " + base.ScriptString + "\nAttempted to execute ForLoop with InfiniteLoop risk!  Skipping for loop!");

            int maxLoopNumber = 10000;
            int loopCount = 0;

            // first try to execute the initial statement
            if (InitialStatement != null)
            {
                InitialStatement.Process(ProcessResult.None, trigObject);
            }

            Object result = ConditionalMathTree.Calculate(trigObject);
            while (result is bool && (bool)result)
            {
                //execute the child code
                ProcessResult lastResult = ProcessChildren(trigObject);
                if (lastResult == ProcessResult.Break) // exit out of this for loop
                    return ProcessResult.None;
                if (lastResult == ProcessResult.Return || lastResult == ProcessResult.ReturnOverride)
                {
                    return lastResult;
                }
                // ProcessResult.Continue--just keep going

                //execute the next part of the loop (often ints.i++)
                try
                {
                    RepeatedStatement.Process(ProcessResult.None, trigObject);
                }
                catch (Exception e)
                {
                    throw new UberScriptException("Line " + LineNumber + ": " + base.ScriptString + "\nForLoop Repeated Statement execution error: ", e);
                }
                try
                {
                    // see whether we still meet our condition
                    result = ConditionalMathTree.Calculate(trigObject);
                }
                catch (Exception e)
                {
                    throw new UberScriptException("Line " + LineNumber + ": " + base.ScriptString + "\nForLoop Conditional Statement execution error: ", e);
                }
                
                loopCount++;
                if (loopCount > maxLoopNumber)
                {
                    InfiniteLoopRisk = true;
                    throw new UberScriptException("Attempted to execute ForLoop with InfiniteLoop risk!  Skipping for loop!");
                }
            }

            return ProcessResult.None;
        }
    }
}