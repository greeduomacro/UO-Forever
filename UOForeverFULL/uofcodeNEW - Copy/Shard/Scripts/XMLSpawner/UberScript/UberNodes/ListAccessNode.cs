using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Server.Mobiles;
using System.IO;

namespace Server.Engines.XmlSpawner2
{
    // NOTE: THIS CLASS IS NOT ACTUALLY USED AT THIS TIME (I am parsing list accession on the fly right now, which might not be as good performance, but oh well)
    public class ListAccessNode : UberNode
    {
        public bool NegateOutput = false;
        
        public ListAccessNode(UberNode parent, string scriptInput) : base(parent, scriptInput) { }
        public ListAccessNode(UberNode parent, string scriptInput, int lineNumber) : base(parent, scriptInput, lineNumber) { }

        public string Property;

        public string OriginalString = null;

        public MathTree ListToAccess = null;
        public MathTree Index = null;

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
                    throw new UberScriptException("Line " + LineNumber + ": " + OriginalString + "\nFunction node string was null!");
                }
                try
                {
                    string originalInput = value;
                    OriginalString = value;
                    value = value.Trim();
                    Stack<string> balanceStack = new Stack<string>();
                    int currentIndex = 0;
                    //string functionName;
                    int i;
                    // first get the function name
                    for (i = 0; i < value.Length; i++)
                    {
                        if (value[i] == '[')
                        {
                            base.ScriptString = value.Substring(currentIndex, i - currentIndex);
                            value = value.Substring(i);
                            break;
                        }
                    }
                    ListToAccess = new MathTree(null, base.ScriptString); // this should eventually evaluate to a list of some kind (e.g. in objs.mobs[test] it would be objs.mobs)
                    // next, process the argument individually as a MathTree
                    int numOpenParens = 0;
                    currentIndex = 0;
                    string dotOperator = "";
                    bool insideQuotation = false;
                    for (i = 0; i < value.Length; i++)
                    {
                        char c = value[i];
                        if (insideQuotation && c != '"')
                        {
                            continue;
                        }
                        if (i > 0 && numOpenParens == 0)
                        {
                            // should be outside the function and only adding . operators
                            dotOperator += c;
                            continue;
                        }

                        if (c == '[')
                        {
                            numOpenParens++;
                            if (i == 0) { currentIndex = 1; } // only move the index after the first openParen
                        }
                        else if (c == ']')
                        {
                            numOpenParens--;
                            if (numOpenParens < 0)
                            {
                                throw new UberScriptException("Line " + LineNumber + ": " + OriginalString + "\nListAccessNode for " + originalInput + " did not have balanced ()!");
                            }
                            if (numOpenParens == 0) // final parenthesis
                            {
                                // process final argument
                                Index = new MathTree(this, value.Substring(currentIndex, i - currentIndex), LineNumber);
                            }
                        }
                        else if (c == '"')
                        {
                            if (i > 0 && value[i-1] == '\\')
                            {
                                // leave it in as-is-- MathTree will take care of it
                            }
                            else
                            {
                                // leave it in as-is-- MathTree will take care of is
                                // just need to indicate we are inside quotation so it doesn't
                                // do anything else (e.g. with commas in "hey, what are you doing?")
                                insideQuotation = !insideQuotation;
                            }
                        }
                    }

                    if (numOpenParens > 0)
                    {
                        throw new UberScriptException("Line " + LineNumber + ": " + OriginalString + "\nListAccessNode for " + originalInput + " did not have balanced ()!");
                    }
                    if (insideQuotation)
                    {
                        throw new UberScriptException("Line " + LineNumber + ": " + OriginalString + "\nUnbalanced \" characters! You must terminate strings with \"");
                    }
                    if (dotOperator != "")
                    {
                        if (dotOperator.StartsWith(".") == false)
                        {
                            throw new UberScriptException("Line " + LineNumber + ": " + OriginalString + "\nListAccessNode for " + originalInput + " had \"" + dotOperator + "\" following it.  This should be a dot operator only (e.g. \".name\").  NOTE that semicolons are NOT used at the end of statements for uberscript.");
                        }
                        if (dotOperator.Length < 2)
                        {
                            // can't have a dot operator without something after the dot
                            throw new UberScriptException("Line " + LineNumber + ": " + OriginalString + "\nListAccessNode for " + originalInput + " had a dot operator without a property after!");
                        }
                        this.Property = dotOperator.Substring(1); // used for debugging
                        string property = Property; // this is a temp variable that changes as it is parsed

                        // now process the dot properties (there might be another list accessor in there)... have to do it this way b/c there might be .'s INSIDE of a list accession, e.g. objs.moblist[objs.moblist.count - 1]
                        // and also need to handle stuff like objs.groups[1].team.mobiles[2]
                        for (i = 0; i < property.Length; i++)
                        {
                            if (numOpenParens > 0 && property[i] != '[' && property[i] != ']')
                            {
                                continue;
                            }
                            if (property[i] == '.')
                            {
                                // it's just a simple property (e.g. "hits" in objs.mobslist[1].hits)
                                Children.Add(new ArgumentNode(this, property.Substring(0, i)));
                                property = property.Substring(i + 1);
                                i = 0;
                            }
                            else if (property[i] == '[')
                            {
                                numOpenParens++;
                            }
                            else if (property[i] == ']')
                            {
                                numOpenParens--;
                                if (numOpenParens < 0)
                                {
                                    throw new UberScriptException("Line " + LineNumber + ": " + OriginalString + "\nListAccessNode for " + originalInput + " did not have balanced ()!");
                                }
                                if (numOpenParens == 0)
                                {
                                    // add a child ListAccessNode
                                    Children.Add(new ListAccessNode(this, property));
                                    property = string.Empty; // it is all handled in the child ListAccessNode
                                }
                            }
                        }
                        // should process the final property
                        if (property != string.Empty)
                        {
                            Children.Add(new ArgumentNode(this, property));
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new UberScriptException("Line " + LineNumber + ": " + OriginalString + "\nListAccessNode Parse error!", e);
                }
                if (Index == null)
                {
                    throw new UberScriptException("Line " + LineNumber + ": " + OriginalString + "\nListAccessNode had no index... missing something between the []!");
                }
            }
        }

        public Object Execute(TriggerObject trigObject, bool tryReturnObject = false)
        {
            
            List<Object> args = new List<Object>();
            args.Add(trigObject); // every function takes this as a parameter
            // the args of the function are actually
            // stored in nodes--either function or argument... e.g.
            // EFFECT(14000,25, THIS().x, THIS().y, THIS().z)
            // has 2 argument nodes and 3 function nodes
            
            // each child of a ListAccessNode is an argument represented by a MathTree
            foreach (UberNode child in Children)
            {
                if (child is MathTree)
                {
                    MathTree mathTree = child as MathTree;
                    if (!mathTree.IsEmpty())
                    {
                        args.Add(mathTree.Calculate(trigObject));
                    }
                    
                     //Execute(trigObject, tryReturnObject = false));
                }
                else
                {
                    throw new UberScriptException("Line " + LineNumber + ": " + OriginalString + "\nListAccessNode had children other than MathTree... something is broken!");
                }
            }
            object[] reflectionArgs = new object[args.Count];
            int count = 0;
            foreach (Object arg in args)
            {
                reflectionArgs[count] = arg;
                count++;
            }
            Object outputObject;

            /*
            if (ScriptString == "THIS") { outputObject = trigObject.This; }
            else if (ScriptString == "TRIGMOB") { outputObject = trigObject.TrigMob; }
            else if (ScriptString == "GIVENTOTHIS") { outputObject = trigObject.DroppedOnThis; }
            else if (ScriptString == "GIVENBYTHIS") { outputObject = trigObject.DroppedOnThis; }
            else if (ScriptString == "TARGETTEDBY") { outputObject = trigObject.TargettedBy; }
            else if (ScriptString == "TARGETTED") { outputObject = trigObject.Targetted; }
            else if (ScriptString == "DAMAGE") { outputObject = trigObject.Damage; }
            else*/
            try
            {
                outputObject = UberScriptFunctions.Invoke(this.ScriptString, reflectionArgs); // ListAccessNode scriptstring contains function name
            }
            catch (Exception e)
            {
                throw new UberScriptException("Line " + LineNumber + ": " + OriginalString + "\nError invoking function:", e);
            }

            if (Property == null || tryReturnObject)
                return outputObject;
            Type ptype;
            try
            {
                outputObject = PropertyGetters.GetObject(trigObject, outputObject, Property, out ptype);
                if (outputObject == null) return null; // it's ok to be null here
            }
            catch (Exception e)
            {
                throw new UberScriptException("Line " + LineNumber + ": " + OriginalString + "\nError setting value:", e);
            }
            if (ptype == null)
            {
                throw new UberScriptException("Line " + LineNumber + ": " + OriginalString + "\nListAccessNode function " + ScriptString + " output object did not have property: " + Property);
            }
            if (NegateOutput)
            {
                Type outputType = outputObject.GetType();
                if (outputType == typeof(SByte)) outputObject = -((SByte)outputObject);
                else if (outputType == typeof(Int16)) outputObject = -((Int16)outputObject);
                else if (outputType == typeof(Int32)) outputObject = -((Int32)outputObject);
                else if (outputType == typeof(Int64)) outputObject = -((Int64)outputObject);
                else throw new UberScriptException("Line " + LineNumber + ": " + OriginalString + "\nCould not negate output of " + this.ScriptString + " output object of type: " + outputType);
            }
            return outputObject;
        }

        public override ProcessResult Process(ProcessResult lastSiblingResult, TriggerObject trigObject)
        {
            Execute(trigObject);
            return ProcessResult.None;
        }
    }
}