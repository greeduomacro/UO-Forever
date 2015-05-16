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
    public class MathTree : UberNode
    {
        public MathTree(UberNode parent, string scriptInput) : base(parent, scriptInput) { }
        public MathTree(UberNode parent, string scriptInput, int lineNumber) : base(parent, scriptInput, lineNumber) { }

        public override string ScriptString
        {
            get
            {
                return base.ScriptString;
            }
            set
            {
                base.ScriptString = value;
                ParseMath(value);
            }
        }

        public bool IsEmpty() { return ScriptString == ""; }

        public Object Calculate(TriggerObject trigObj)
        {
            if (Children.Count == 0 || Children.Count > 1) { throw new Exception("Bad number of children in MathTree!:" + Children.Count + " ScriptString: " + ScriptString); }
            return MathNode.Calculate(trigObj, (MathNode)Children[0]);
        }

        public static MathNode currentRoot = null;
        public static bool isNegative = false;
        public void ParseMath(string input)
        {
            int currentIndex = 0;
            Stack<MathNode> prevRootNodes = new Stack<MathNode>();
            currentRoot = null;
            isNegative = false;
            int numOpenParensInFunctionCallString = 0; // should be 0 if we are not in a function call, never less than 0
            int totalOpenParens = 0; // should never be less than 0
            string operatorsSupportingUnaryMinus = "+-*/^%(<=>";
            bool insideQuotation = false;
            int totalOpenSquareBracket = 0;

            if (input == "")
            {
                currentRoot = new MathNode(null, null, OpType.String, TreePriority.Operand, String.Empty);
                Children.Add(currentRoot);
                return;
            }

            // if it is an array, there can be nothing more in the math tree
            if (input.StartsWith("[") && input.EndsWith("]"))
            {
                MathNode arrayNode = new MathNode(null, input, OpType.Array, TreePriority.Infinite);
                string inArray = input.Substring(1, input.Length - 2);
                List<string> elements = new List<string>();
                for (int i = 0; i < inArray.Length; i++)
                {
                    char c = inArray[i];
                    if (insideQuotation && c != '"')
                    {
                        continue;
                    }

                    if (c == '(')
                    {
                        totalOpenParens += 1;
                    }
                    else if (c == ')')
                    {
                        totalOpenParens -= 1;
                        if (totalOpenParens < 0) { throw new Exception("Unbalanced () characters in math string!"); }
                    }
                    else if (c == '[')
                    {
                        totalOpenSquareBracket += 1;
                    }
                    else if (c == ']')
                    {
                        totalOpenSquareBracket -= 1;
                        if (totalOpenSquareBracket < 0) { throw new Exception("Unbalanced [] characters in math string!"); }
                    }
                    else
                    {
                        if (totalOpenParens > 0 || totalOpenSquareBracket > 0)
                            continue;

                        if (c == '"')
                        {
                            
                            /* I'm not sure why I thought I need to do this here; it's taken care of in the MathTree parsing
                            if (i > 0 && inArray[i-1] == '\\')
                            {
                                // not super efficient, but oh well, it's only run once
                                inArray = inArray.Substring(0, i - 1) + inArray.Substring(i); // remove the \, keep the "
                                i--;
                            }
                            else
                            {
                                inArray = inArray.Substring(0, i) + inArray.Substring(i+1); // remove the "
                                i--;
                             */
                                insideQuotation = !insideQuotation;
                            //}
                        }
                        else if (c == ',')
                        {
                            elements.Add(inArray.Substring(0, i));
                            inArray = inArray.Substring(i+1);
                            i = -1;
                        }
                    }
                }
                if (inArray.Length > 0) { elements.Add(inArray); } // add final element

                ArrayList arrayList = new ArrayList(elements.Count + 10);

                foreach (string element in elements)
                {
                    arrayList.Add(new MathTree(null, element));
                }
                arrayNode.Value = arrayList;
                Children.Add(arrayNode);
                return;
            }

            bool justEscapedBackslash = false;
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                string arg;
                
                if (insideQuotation && c != '"')
                {
                    if (c == '\\')
                    {
                        if (i > 0 && input[i - 1] == '\\' && !justEscapedBackslash)
                        {
                            justEscapedBackslash = true;
                            continue;
                        }
                    }
                    justEscapedBackslash = false;
                    continue;
                }

                if (c == '"')
                {
                    if (i > 0 && input[i - 1] == '\\' && !justEscapedBackslash)
                    {
                        continue; // it's an escaped \" character
                    }
                    else
                    {
                        // --> I'm putting this in the ParseOperand area
                        //input = input.Substring(0, i) + input.Substring(i+1); // remove the "
                        //i--;
                        insideQuotation = !insideQuotation;
                    }
                }


                if (totalOpenSquareBracket > 0 && c != ']')
                {
                   if (c == '[')
                    {
                        totalOpenSquareBracket++;
                    } 
                    continue;
                }
                else if (c == '[')
                {
                    totalOpenSquareBracket++;
                }
                else if (c == ']')
                {
                    if (totalOpenSquareBracket > 0)
                    {
                        totalOpenSquareBracket--;
                    }
                    else
                    {
                        throw new Exception("Unbalanced [] characters in list accessing string!");
                    }
                }

                if (numOpenParensInFunctionCallString > 0 && c != '(' && c != ')')
                {
                    // don't do anything in the middle of a function string
                    continue;
                }

                if (c == '|')
                {
                    if (input[i + 1] == '|')
                    {
                        AddOperationNode(ref input, OpType.Or, TreePriority.Or, i, ref currentIndex);
                        i++;
                    }
                    else
                    {
                        throw new UberScriptException("Single | encountered!  Use || for logical 'or' operator. Note that the bitwise '|' operator currently not supported!");
                        //AddOperationNode(ref input, OpType.BitOr, TreePriority.BitAnd, i, ref currentIndex);
                    }
                }
                else if (c == '&')
                {
                    if (input[i + 1] == '&')
                    {
                        AddOperationNode(ref input, OpType.And, TreePriority.And, i, ref currentIndex);
                        i++;
                    }
                    else
                    {
                        throw new UberScriptException("Single & encountered!  Use && for logical 'and' operator. Note that the bitwise '&' operator currently not supported!");
                        //AddOperationNode(ref input, OpType.BitAnd, TreePriority.BitAnd, i, ref currentIndex);
                    }
                }
                else if (c == '>')
                {
                    if (input[i+1] == '=')
                    {
                        AddOperationNode(ref input, OpType.GThanOrEqualTo, TreePriority.Comparison, i, ref currentIndex);
                        i++;
                    }
                    else
                    {
                        AddOperationNode(ref input, OpType.GThan, TreePriority.Comparison, i, ref currentIndex);
                    }
                }
                else if (c == '<')
                {
                    if (input[i+1] == '=')
                    {
                        AddOperationNode(ref input, OpType.LThanOrEqualTo, TreePriority.Comparison, i, ref currentIndex);
                        i++;
                    }
                    else
                    {
                        AddOperationNode(ref input, OpType.LThan, TreePriority.Comparison, i, ref currentIndex);
                    }
                }
                else if (c == '!')
                {
                    if (input[i + 1] == '=')
                    {
                        AddOperationNode(ref input, OpType.NotEqualTo, TreePriority.Comparison, i, ref currentIndex);
                        i++;
                    }
                    else
                    {
                        throw new UberScriptException("Cannot have a single = sign!");
                    }
                }
                else if (c == '=')
                {
                    if (input[i+1] == '=')
                    {
                        AddOperationNode(ref input, OpType.EqualTo, TreePriority.Comparison, i, ref currentIndex);
                        i++;
                    }
                    else
                    {
                        throw new UberScriptException("Cannot have a single = sign!");
                    }
                }
                else if (c == '+')
                {
                    AddOperationNode(ref input, OpType.Add, TreePriority.AddSub, i, ref currentIndex);
                }
                else if (c == '-')
                {
                    // i == 0 means a statment is STARTING with -, e.g. -6
                    // otherwise there is something like this ...+-6 or ...*-6, etc
                    if (i == 0 || (i > 0 && (operatorsSupportingUnaryMinus.Contains(input[i - 1]))))
                    {
                        isNegative = !isNegative;
                        currentIndex = i + 1;
                    }
                    else
                    {
                        AddOperationNode(ref input, OpType.Sub, TreePriority.AddSub, i, ref currentIndex);
                    }
                }
                else if (c == '*')
                {
                    AddOperationNode(ref input, OpType.Mul, TreePriority.MultDiv, i, ref currentIndex);
                }
                else if (c == '/')
                {
                    AddOperationNode(ref input, OpType.Div, TreePriority.MultDiv, i, ref currentIndex);
                }
                else if (c == '^')
                {
                    AddOperationNode(ref input, OpType.Pow, TreePriority.Power, i, ref currentIndex);
                }
                else if (c == '%')
                {
                    AddOperationNode(ref input, OpType.Mod, TreePriority.MultDiv, i, ref currentIndex);
                }
                else if (c == '(')
                {
                    totalOpenParens++;
                    // must check if it is a function call
                    if (numOpenParensInFunctionCallString == 0)
                    {
                        if (UberScriptFunctions.IsFunctionString(input.Substring(currentIndex, i - currentIndex).Trim()))
                        {
                            numOpenParensInFunctionCallString++;
                            continue;
                        }
                        // it's not a function call, it's an order of operations open parenthesis
                        prevRootNodes.Push(currentRoot);
                        MathNode openParenPlaceholder = new MathNode(null, null, OpType.OpenParenthesis, TreePriority.Operand);
                        InsertIntoTree(currentRoot, openParenPlaceholder);
                        currentRoot = openParenPlaceholder;
                        currentIndex = i + 1;
                    }
                    else
                    {
                        numOpenParensInFunctionCallString++; // keep incrementing it
                    }
                }
                else if (c == ')')
                {
                    totalOpenParens--;
                    if (totalOpenParens < 0) { throw new Exception("Unbalanced () characters in math string!"); }
                    if (numOpenParensInFunctionCallString > 0)
                    {
                        numOpenParensInFunctionCallString--;
                        if (numOpenParensInFunctionCallString < 0) { throw new Exception("Unbalanced () characters in function call string!"); }
                    }
                    else
                    {
                        // should be an operand or ) right before this )
                        arg = input.Substring(currentIndex, i - currentIndex).Trim();
                        if (arg == "" && !(i > 0 && input[i - 1] == ')')) { throw new UberScriptException("Math parse error: no operand before operator for input=" + input); }
                        if (currentRoot == null) { throw new UberScriptException("Math parse error: can't have ) without a ( currentRoot! input=" + input); } // should be impossible

                        if (arg != "") InsertIntoTree(currentRoot, ParseOperand(arg));
                        currentIndex = i + 1;
                        MathNode prevRootNode = prevRootNodes.Pop();
                        currentRoot.Priority = TreePriority.Infinite;
                        if (prevRootNode != null)
                        {
                            currentRoot = prevRootNode; // return up a level
                        }
                        // else the first character in the input was (, so there was no root node initially, so keep the currentNode
                    }
                }
            }

            if (insideQuotation) { throw new UberScriptException("Unbalanced \" characters! You must terminate strings with \""); };
            if (totalOpenParens > 0) { throw new UberScriptException("Did not have matching ()!"); }
            if (totalOpenSquareBracket > 0) { throw new UberScriptException("Did not have matching []!"); }

            if (currentIndex < input.Length)
            {
                if (currentRoot == null)
                {
                    currentRoot = ParseOperand(input.Substring(currentIndex, input.Length - currentIndex));
                }
                else
                {
                    InsertIntoTree(currentRoot, ParseOperand(input.Substring(currentIndex, input.Length - currentIndex)));
                }
            }
            Children.Add(currentRoot);
        }

        /// <summary>
        /// Replaces the node in the tree, returns the replaced node
        /// </summary>
        /// <param name="nodeToReplace"></param>
        /// <param name="newNode"></param>
        /// <param name="replaceCurrent"></param>
        /// <returns></returns>
        public static MathNode ReplaceNode(MathNode nodeToReplace, MathNode newNode, bool replaceCurrent)
        {
            if (nodeToReplace.Parent == null)
            {
                currentRoot = newNode; // we're at the top root
                newNode.Left = nodeToReplace.Left;
                newNode.Right = nodeToReplace.Right;
                return nodeToReplace;
            } // in case it was the only thing in the tree
            if (((MathNode)nodeToReplace.Parent).Right == nodeToReplace) { ((MathNode)nodeToReplace.Parent).Right = newNode; }
            else { ((MathNode)nodeToReplace.Parent).Left = newNode; }
            newNode.Left = nodeToReplace.Left;
            newNode.Right = nodeToReplace.Right;
            if (replaceCurrent)
            {
                currentRoot = newNode;
            }
            return nodeToReplace;
        }

        public static void InsertIntoTree(MathNode nodeToCheck, MathNode newNode)
        {
            if (nodeToCheck == null) { currentRoot = newNode; return; }
            try
            {
                if (newNode.Priority == TreePriority.Unassigned) { throw new Exception("Attempted to insert MathNode with unassigned priority--Not allowed!"); }
                if (newNode.Priority == TreePriority.Operand)
                {
                    if (nodeToCheck.OpTypeVal == OpType.OpenParenthesis && newNode.OpTypeVal != OpType.OpenParenthesis)
                    {
                        // should never be more than 1 open parenthesis in a row (b/c currentRootNode is replaced)...

                        // remove the Open Parenthesis node--it was just a placerholder
                        ReplaceNode(nodeToCheck, newNode, true); // becomes the new Current root until the next closing parenthesis
                        return;
                    }
                    // should never happen that two operands land on each other in this function
                    // EXCEPT if you get 2 open parentheses in a row
                    if (nodeToCheck.Priority > TreePriority.Operand
                        || (nodeToCheck.OpTypeVal == OpType.OpenParenthesis && newNode.OpTypeVal == OpType.OpenParenthesis))
                    {
                        if (nodeToCheck.Left == null) { nodeToCheck.Left = newNode; }
                        else if (nodeToCheck.Left.OpTypeVal == OpType.OpenParenthesis) { InsertIntoTree(nodeToCheck.Left, newNode); } // the only time operand can go left
                        else if (nodeToCheck.Right == null) { nodeToCheck.Right = newNode; }
                        else InsertIntoTree(nodeToCheck.Right, newNode);
                    }
                    else { throw new Exception("Cannot have operand land on operand in InsertIntoTree method!"); }
                }
                else // priority is > Operand... this will either push a branch down or replace an operator somewhere
                {
                    if (nodeToCheck.OpTypeVal == OpType.OpenParenthesis)
                    {
                        // something like ((5+5)*3), the * would have hit this case

                        // remove the Open Parenthesis node--it was just a placerholder
                        ReplaceNode(nodeToCheck, newNode, true); // becomes the new Current root until the next closing parenthesis
                    }
                    else if (nodeToCheck.Priority >= newNode.Priority) // push the branch down
                    {
                        if (nodeToCheck.Parent != null)
                        {
                            if (((MathNode)nodeToCheck.Parent).Right == nodeToCheck) { ((MathNode)nodeToCheck.Parent).Right = newNode; }
                            else { ((MathNode)nodeToCheck.Parent).Left = newNode; }
                        }
                        else
                        {
                            currentRoot = newNode;
                        }
                        if (currentRoot == nodeToCheck) currentRoot = newNode;
                        newNode.Left = nodeToCheck;
                    }
                    else // (nodeToCheck.Priority < newNode.Priority) // get underneath it, push operand down
                    {
                        if (nodeToCheck.Left == null)
                        { // DON"T throw new Exception("InsertIntoTree Higher priority operator found null left node!"); }
                            // actually this can happen in a case like (4+10), where the ( is replaced with the
                            // 4 and then the + tries to follow suit, but 4 is now the root node.
                            if (nodeToCheck.Priority == TreePriority.Operand)
                            {
                                if (nodeToCheck.Parent != null)
                                {
                                    if (((MathNode)nodeToCheck.Parent).Right == nodeToCheck) { ((MathNode)nodeToCheck.Parent).Right = newNode; }
                                    else { ((MathNode)nodeToCheck.Parent).Left = newNode; }
                                    if (currentRoot == nodeToCheck) currentRoot = newNode;
                                }
                                else
                                {
                                    currentRoot = newNode;
                                }
                                newNode.Left = nodeToCheck;
                            }
                        }
                        else if (nodeToCheck.Right == null) { throw new Exception("Right node should not be null here!"); }
                        else if (nodeToCheck.Right.Priority == TreePriority.Operand)
                        {
                            MathNode oldRight = ReplaceNode(nodeToCheck.Right, newNode, false);
                            newNode.Left = oldRight;
                        }
                        else InsertIntoTree(nodeToCheck.Right, newNode);
                    }
                }
            }
            catch (Exception e)
            {
                throw new UberScriptException("InsertIntoTree error: ", e);
            }
        }

        public static void AddOperationNode(ref string input, OpType opType, TreePriority priority, int i, ref int currentIndex)
        {
            string operand = input.Substring(currentIndex, i - currentIndex);
            MathNode newNode = new MathNode(null, null, opType, priority);
            char prevChar = i > 0 ? input[i-1] : '\0'; 
            bool twoCharOperator = false;
            char nextChar;
            if (opType == OpType.And || opType == OpType.Or || opType == OpType.NotEqualTo 
                || opType == OpType.EqualTo || opType == OpType.GThanOrEqualTo || opType == OpType.LThanOrEqualTo)
            {
                nextChar = input.Length > (i+2) ? input[i+2] : '\0';
                twoCharOperator = true;
            }
            else
            {
                nextChar = input.Length > (i+1) ? input[i+1] : '\0';
            }
            if (operand == "")
            {
                // the only character possible before an operator is )
                if (i > 0 && input[i - 1] == ')')
                {
                    bool hasFollowingOpenParen = false; // special case--requires decreased priority when inserting this node
                    if (nextChar == '(')
                    {
                        hasFollowingOpenParen = true;
                    }
                    if (hasFollowingOpenParen)
                    {
                        TreePriority prevPriority = newNode.Priority;
                        newNode.Priority = TreePriority.AddSub; // SHOULD THIS BE CHANGED TO JUST DECREASING BY ONE? OR ALWAYS ADD SUB?
                        InsertIntoTree(currentRoot, newNode);
                        newNode.Priority = prevPriority;
                    }
                    else InsertIntoTree(currentRoot, newNode);
                }
                else throw new UberScriptException("Math parse error: no operand before + for input= " + input);
            }
            else
            {
                if (currentRoot == null) { currentRoot = newNode; InsertIntoTree(currentRoot, ParseOperand(operand)); }
                else { InsertIntoTree(currentRoot, ParseOperand(operand)); InsertIntoTree(currentRoot, newNode); }
            }
            if (twoCharOperator)
                currentIndex = i + 2;
            else
                currentIndex = i + 1;
        }

        // returns the added operand node
        public static MathNode ParseOperand(string input)
        {
            if (input == null) { throw new UberScriptException("AddOperand input was null!"); }
            if (input == "") { throw new UberScriptException("AddOperand input was an empty string!"); }
            Object operand = input;
            OpType opType = OpType.Unassigned;
            try
            {
                int numCommas = 0;
                for (int i = 0; i < input.Length; i++)
                {
                    if (input[i] == ',') numCommas++;
                }
                if (input.StartsWith("0x"))
                {
                    try
                    {
                        if (isNegative) // might fail here
                        {
                            operand = Convert.ToInt32(input.Substring(2), 16);
                            opType = OpType.Int;
                        }
                        else
                        {
                            operand = Convert.ToUInt64(input.Substring(2), 16);
                            // try to use int if at all possible
                            if ((UInt64)operand < 2147483647) // 2^31 - 1
                            {
                                operand = Convert.ToInt32(input.Substring(2), 16);
                                opType = OpType.Int;
                            }
                            else
                                opType = OpType.UInt64;
                        }
                    }
                    catch // (InvalidCastException e)
                    {
                        // must be a string
                        operand = input;
                        opType = OpType.String;
                    }
                }
                else if (char.IsNumber(input[0]) || input[0] == '.')
                {
                    double doubleVal;
                    bool result = double.TryParse(input, out doubleVal);
                    if (result == false)
                    {
                        operand = input; // must be something like 016hello, so just treat it as a string
                        opType = OpType.String;
                    }
                    else
                    {
                        if (isNegative) { doubleVal = -doubleVal; isNegative = false; }
                        if (!input.Contains('.') && doubleVal % 1 < 0.0009765625) // 2^-10
                        {
                            // close enough to an integer value
                            operand = (int)doubleVal;
                            opType = OpType.Int;
                        }
                        else
                        {
                            operand = doubleVal;
                            opType = OpType.Double;
                        }
                    }
                }
                else if (input == "null")
                {
                    opType = OpType.Null;
                    operand = null;
                }
                else if (input.StartsWith("\""))
                {
                    // inside a quotation--> parse out the innards and set it to a string
                    if (!input.EndsWith("\"") || input.Length == 1)
                    {
                        throw new UberScriptException("ParseOperand input started with \" but did not end with \"... it must be balanced!");
                    }
                    input = input.Substring(1, input.Length - 2); // remove the front and back ""
                    bool justEscapedBackslash = false;
                    for (int ix = 0; ix < input.Length; ix++)
                    {
                        if (ix > 0 && input[ix - 1] == '\\')
                        {
                            // not super efficient, but oh well, it's only run once
                            if (!justEscapedBackslash)
                            {
                                if (input[ix] == '"')
                                {
                                    input = input.Substring(0, ix - 1) + input.Substring(ix); // remove the \, keep the "
                                    ix--;
                                }
                                else if (input[ix] == 't')
                                {
                                    input = input.Substring(0, ix - 1) + "\t" + input.Substring(ix + 1); // remove the \ and add a tab
                                    ix--;
                                }
                                else if (input[ix] == 'n')
                                {
                                    input = input.Substring(0, ix - 1) + "\n" + input.Substring(ix + 1); // remove the \ and add a new line
                                    ix--;
                                }
                                else if (input[ix] == 'r')
                                {
                                    input = input.Substring(0, ix - 1) + "\r" + input.Substring(ix + 1); // remove the \ and add a carriage return
                                    ix--;
                                }
                                else if (input[ix] == '\\')
                                {
                                    input = input.Substring(0, ix - 1) + input.Substring(ix); // keep only one of the \ characters 
                                    justEscapedBackslash = true;
                                    continue;
                                }
                            }
                            // otherwise ignore it
                        }
                        justEscapedBackslash = false;
                    }
                    operand = input;
                    opType = OpType.String;
                }
                else if (UberScriptFunctions.IsFunctionString(input))
                {
                    MathNode latestRootNode = currentRoot; // currentRoot is replaced when FunctionNode adds a mathtree for each arg
                    currentRoot = null;
                    operand = new FunctionNode(null, input); // can't add LineNumber here :(
                    if (isNegative) { ((FunctionNode)operand).NegateOutput = true; isNegative = false; }
                    opType = OpType.Func;
                    currentRoot = latestRootNode;
                }
                // NOTE: While the performance is not as good (perhaps), I'm going to rely on parsing list accession (e.g. objs.mobs[objs.mobs.count - 1]) 
                // on the fly (as is done in the PropertySetters and PropertyGetters functions) rather than pre-parsing it.
                /*
                else if (UberScriptFunctions.IsArrayAccessor(input))
                {
                    MathNode latestRootNode = currentRoot; // currentRoot is replaced when FunctionNode adds a mathtree for each arg
                    currentRoot = null;
                    operand = new ListAccessNode(null, input); // can't add LineNumber here :(
                    if (isNegative) { ((ListAccessNode)operand).NegateOutput = true; isNegative = false; }
                    opType = OpType.ListAccess;
                    currentRoot = latestRootNode;
                }*/

                /* // I'm not going to handle literal points e.g. (1,2,3)... use the LOC function instead (so you can do math in it)
                else if (numCommas > 1) // it must be a location if it is not a function and has 2 commas in it
                {
                    operand = "(" + input + ")";
                    opType = OpType.String;
                }*/
                else
                {
                    operand = input;
                    opType = OpType.String;
                }


                MathNode newOperandNode = new MathNode(null, null, opType, TreePriority.Operand, operand); // assigned into tree in constructor!
                return newOperandNode;
            }
            catch (Exception e)
            {
                Console.WriteLine("AddOperand error: " + e.Message + "\n" + e.StackTrace);
                throw new UberScriptException("AddOperand error for " + input, e);
            }
        }
    }

    public enum TreePriority : int
    {
        Unassigned = -1,
        Operand = 0, // Lowest, NOTE: includes (, since that just reassigns the root node
        Or = 1,
        And = 2,
        
        //BitOr = 3,
        //BitAnd = 4,
        //EqualTo = 8,  // Just use comparison!
        Comparison = 9,
        AddSub = 10, // Medium
        MultDiv = 11, // High
        Power = 12, // Highest
        OpenParen = 13,
        Infinite = 14
    }

    public enum OpType : byte
    {
        Func,
        ListAccess,
        Array,
        Bool,
        UInt64,
        Int,
        Double,
        BitAnd,
        BitOr,
        And,
        Or,
        GThan,
        LThan,
        NotEqualTo,
        EqualTo,
        GThanOrEqualTo,
        LThanOrEqualTo,
        Add,
        Sub,
        Mul,
        Div,
        Mod, // %
        Pow,
        String,
        Null,
        OpenParenthesis,
        Unassigned
    }
}