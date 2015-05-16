using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Server.Mobiles;
using System.IO;
using System.Collections;

namespace Server.Engines.XmlSpawner2
{
    public class ForEachNode : UberNode
    {
        public string ObjectLookup = null;
        public MathTree ListObject = null;
        public bool InfiniteLoopRisk = false;

        public ForEachNode(UberNode parent, string scriptInput) : base(parent, scriptInput) { }
        public ForEachNode(UberNode parent, string scriptInput, int lineNumber) : base(parent, scriptInput, lineNumber) { }
        

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
                    throw new UberScriptException("ForLoop node string was null!");
                }
                base.ScriptString = value;
                value = value.Trim();
                value = ConditionalNode.GetConditionalString(value, LineNumber);
                string[] splitString = value.Split(';');
                if (splitString.Length != 2)
                {
                    throw new UberScriptException("ForEach node did not have 2 parts to it separated by semi-colons!  It should be something like foreach (objs.mob; objs.moblist)");
                }
                if (splitString[0].Length > 0 && splitString[0].StartsWith("objs."))
                {
                    ObjectLookup = splitString[0].Substring(5);
                }
                else
                {
                    throw new UberScriptException("Line " + LineNumber + ": " + base.ScriptString + "\nForeachLoop Error: You must specify an objs object to put each item in the list into!  e.g. foreach (objs.mob; objs.moblist)");
                }
                if (splitString[1].Length > 0)
                {
                    try
                    {
                        ListObject = new MathTree(null, splitString[1], LineNumber);
                    }
                    catch (Exception e)
                    {
                        throw new UberScriptException("Line " + LineNumber + ": " + base.ScriptString + "\nForeachLoop MathTree parse error: ", e);
                    }
                }
                else
                {
                    throw new UberScriptException("Line " + LineNumber + ": " + base.ScriptString + "\nForEach node had no reference to a possible List object!");
                }
            }
        }

        public override ProcessResult Process(ProcessResult lastSiblingResult, TriggerObject trigObject)
        {
            if (InfiniteLoopRisk == true)
                throw new UberScriptException("Attempted to execute ForLoop with InfiniteLoop risk!  Skipping for loop!");

            int maxLoopNumber = 1000000;
            int loopCount = 0;

            Object list = ListObject.Calculate(trigObject);
            // currently only supports these 3 types of lists
            IPooledEnumerable pooledEnum = list as IPooledEnumerable;
            List<Mobile> mobList = list as List<Mobile>;
            List<Item> itemList = list as List<Item>;
            ArrayList arrayList = list as ArrayList;

            if (pooledEnum !=  null)
            {
                foreach (Object obj in pooledEnum)
                {
                    trigObject.objs[ObjectLookup] = obj;
                    // execute the child nodes
                    ProcessResult lastResult = ProcessChildren(trigObject);
                    if (lastResult == ProcessResult.Break) // exit out of this for loop
                    {
                        trigObject.CurrentNodeExecutionChain.Remove(this);
                        return ProcessResult.None;
                    }
                    if (lastResult == ProcessResult.Return || lastResult == ProcessResult.ReturnOverride)
                    {
                        return lastResult;
                    }
                    loopCount++;
                    if (loopCount > maxLoopNumber)
                    {
                        InfiniteLoopRisk = true;
                        throw new UberScriptException("Line " + LineNumber + ": " + base.ScriptString + "\nAttempted to execute ForLoop with InfiniteLoop risk!  Skipping for loop!");
                    }
                    if (lastResult == ProcessResult.Continue)
                    {
                        continue;
                    }
                }
                pooledEnum.Free();    
            }
            else if (mobList != null)
            {
                foreach (Mobile mob in mobList)
                {
                    trigObject.objs[ObjectLookup] = mob;
                    // execute the child nodes
                    ProcessResult lastResult = ProcessChildren(trigObject);
                    if (lastResult == ProcessResult.Break) // exit out of this for loop
                    {
                        trigObject.CurrentNodeExecutionChain.Remove(this);
                        return ProcessResult.None;
                    }
                    if (lastResult == ProcessResult.Return || lastResult == ProcessResult.ReturnOverride)
                    {
                        return lastResult;
                    }
                    loopCount++;
                    if (loopCount > maxLoopNumber)
                    {
                        InfiniteLoopRisk = true;
                        throw new UberScriptException("Line " + LineNumber + ": " + base.ScriptString + "\nAttempted to execute ForLoop with InfiniteLoop risk!  Skipping for loop!");
                    }
                    if (lastResult == ProcessResult.Continue)
                    {
                        continue;
                    }
                }
            }
            else if (itemList != null)
            {
                foreach (Item item in itemList)
                {
                    trigObject.objs[ObjectLookup] = item;
                    // execute the child nodes
                    ProcessResult lastResult = ProcessChildren(trigObject);
                    if (lastResult == ProcessResult.Break) // exit out of this for loop
                    {
                        trigObject.CurrentNodeExecutionChain.Remove(this);
                        return ProcessResult.None;
                    }
                    if (lastResult == ProcessResult.Return || lastResult == ProcessResult.ReturnOverride)
                    {
                        return lastResult;
                    }
                    loopCount++;
                    if (loopCount > maxLoopNumber)
                    {
                        InfiniteLoopRisk = true;
                        throw new UberScriptException("Line " + LineNumber + ": " + base.ScriptString + "\nAttempted to execute ForLoop with InfiniteLoop risk!  Skipping for loop!");
                    }
                    if (lastResult == ProcessResult.Continue)
                    {
                        continue;
                    }
                }
            }
            else if (arrayList != null)
            {
                foreach (Object obj in arrayList)
                {
                    trigObject.objs[ObjectLookup] = obj;
                    // execute the child nodes
                    ProcessResult lastResult = ProcessChildren(trigObject);
                    if (lastResult == ProcessResult.Break) // exit out of this for loop
                    {
                        trigObject.CurrentNodeExecutionChain.Remove(this);
                        return ProcessResult.None;
                    }
                    if (lastResult == ProcessResult.Return || lastResult == ProcessResult.ReturnOverride)
                    {
                        return lastResult;
                    }
                    loopCount++;
                    if (loopCount > maxLoopNumber)
                    {
                        InfiniteLoopRisk = true;
                        throw new UberScriptException("Line " + LineNumber + ": " + base.ScriptString + "\nAttempted to execute ForLoop with InfiniteLoop risk!  Skipping for loop!");
                    }
                    if (lastResult == ProcessResult.Continue)
                    {
                        continue;
                    }
                }
            }
            else
                throw new UberScriptException("Line " + LineNumber + ": " + base.ScriptString + "\nDid not have IPooledEnumerable, Mobile List, or Item List to iterate over!");
            return ProcessResult.None;
        }
    }
}