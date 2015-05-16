#region References
using System;
using System.Collections;
#endregion

namespace Server.Engines.XmlSpawner2
{
	public class MathNode : UberNode
	{
		public Object Value = null;
		public TreePriority Priority = TreePriority.Unassigned;
		public OpType OpTypeVal = OpType.Unassigned;

		// should form a binary tree, can't have more than 2 children
		public MathNode(UberNode parent, string scriptInput, OpType opType)
			: base(parent, scriptInput)
		{
			Children.Add(null);
			Children.Add(null);
			OpTypeVal = opType;
		}

		public MathNode(UberNode parent, string scriptInput, OpType opType, TreePriority priority)
			: this(parent, scriptInput, opType)
		{
			Priority = priority;
		}

		public MathNode(UberNode parent, string scriptInput, OpType opType, TreePriority priority, Object value)
			: this(parent, scriptInput, opType, priority)
		{
			Value = value;
		}

		public MathNode Left
		{
			get { return Children[0] as MathNode; }
			set
			{
				Children[0] = value;
				if (value != null)
				{
					value.Parent = this;
				}
			}
		}

		public MathNode Right
		{
			get { return Children[1] as MathNode; }
			set
			{
				Children[1] = value;
				if (value != null)
				{
					value.Parent = this;
				}
			}
		}

		public static object CheckDictionaries(TriggerObject trigObj, string nodeValueString, out bool isDictionaryLookup)
		{
			//string nodeValueStringLower = nodeValueString.ToLower();
			var arglist = PropertyGetters.PropSplitter(nodeValueString);
			//NEED TO fixed INDEXING ISSUE.. DO IT JUST LIKE A FUNCTION CALL??? (should make a function for it too)
			Type ptype;

			if (arglist[0] == "ints")
			{
				isDictionaryLookup = true;
				if (arglist.Count > 2)
				{
					throw new UberScriptException("ints lookup cannot have a dot operator after it!");
				}
				if (!trigObj.ints.ContainsKey(arglist[1]))
				{
					return null;
				}
				int toReturn = trigObj.ints[arglist[1]];
				return toReturn;
			}
			else if (arglist[0] == "doubles")
			{
				isDictionaryLookup = true;
				if (arglist.Count > 2)
				{
					throw new UberScriptException("doubles lookup cannot have a dot operator after it!");
				}
				if (!trigObj.doubles.ContainsKey(arglist[1]))
				{
					return null;
				}
				double toReturn = trigObj.doubles[arglist[1]];
				return toReturn;
			}
			else if (arglist[0] == "strings")
			{
				isDictionaryLookup = true;
				string toReturn;
				trigObj.strings.TryGetValue(arglist[1], out toReturn);
				if (toReturn != null && arglist.Count > 2)
				{
					return PropertyGetters.GetObject(trigObj, toReturn, arglist[2], out ptype);
				}
				return toReturn;
			}
			else if (arglist[0] == "objs")
			{
				isDictionaryLookup = true;
				object toReturn;
				// since the object might be a list of some kind, need to check for the [] indexing first
				string objName = arglist[1];
				int openBracketIndex = objName.IndexOf('[');
				int listindex = -1; // for lists / arrays
				if (openBracketIndex > 0)
				{
					int closingBracketIndex = objName.IndexOf(']');
					if (closingBracketIndex < openBracketIndex + 1)
					{
						throw new UberScriptException(
							"xmlobjs." + objName + " [] indexing error: must have at least 1 character between the []");
					}

					listindex =
						(int)
						(new MathTree(null, objName.Substring(openBracketIndex + 1, closingBracketIndex - openBracketIndex - 1)))
							.Calculate(trigObj);
					objName = objName.Substring(0, openBracketIndex);
				}
				trigObj.objs.TryGetValue(objName, out toReturn);

				if (toReturn != null)
				{
					if (listindex >= 0)
					{
						ptype = toReturn.GetType();
						if (ptype.GetInterface("IList") != null)
						{
							try
							{
								toReturn = ((IList)toReturn)[listindex];
							}
							catch (Exception e)
							{
								throw new UberScriptException("Get Array value error!", e);
							}
						}
						else
						{
							throw new UberScriptException("xmlobj." + objName + " was indexed with [] indexer, but did not contain a list!");
						}
					}

					if (arglist.Count > 2)
					{
						string propLookup = arglist[2]; // add this first so later additions all prepended with '.'
						for (int i = 3; i < arglist.Count; i++)
						{
							propLookup += "." + arglist[i];
						}
						toReturn = PropertyGetters.GetObject(trigObj, toReturn, propLookup, out ptype);
					}
				}
				return toReturn;
			}
			else if (nodeValueString.StartsWith("global_"))
			{
				if (nodeValueString.StartsWith("global_ints."))
				{
					isDictionaryLookup = true;
					if (arglist.Count > 2)
					{
						throw new UberScriptException("global_ints lookup cannot have a dot operator after it!");
					}
					if (!UberTreeParser.global_ints.ContainsKey(arglist[1]))
					{
						return null;
					}
					int toReturn = UberTreeParser.global_ints[arglist[1]];
					return toReturn;
				}
				else if (nodeValueString.StartsWith("global_doubles."))
				{
					isDictionaryLookup = true;
					if (arglist.Count > 2)
					{
						throw new UberScriptException("global_doubles lookup cannot have a dot operator after it!");
					}
					if (!UberTreeParser.global_doubles.ContainsKey(arglist[1]))
					{
						return null;
					}
					double toReturn;
					UberTreeParser.global_doubles.TryGetValue(arglist[1], out toReturn);
					if (arglist.Count > 2)
					{
						return PropertyGetters.GetObject(trigObj, toReturn, arglist[2], out ptype);
					}
					return toReturn;
				}
				else if (nodeValueString.StartsWith("global_strings."))
				{
					isDictionaryLookup = true;
					string toReturn;
					UberTreeParser.global_strings.TryGetValue(arglist[1], out toReturn);
					if (toReturn != null && arglist.Count > 2)
					{
						return PropertyGetters.GetObject(trigObj, toReturn, arglist[2], out ptype);
					}
					return toReturn;
				}
				else if (nodeValueString.StartsWith("global_objs."))
				{
					isDictionaryLookup = true;
					object toReturn;
					// since the object might be a list of some kind, need to check for the [] indexing first
					string objName = arglist[1];
					int openBracketIndex = objName.IndexOf('[');
					int listindex = -1; // for lists / arrays
					if (openBracketIndex > 0)
					{
						int closingBracketIndex = objName.IndexOf(']');
						if (closingBracketIndex < openBracketIndex + 1)
						{
							throw new UberScriptException(
								"xmlobjs." + objName + " [] indexing error: must have at least 1 character between the []");
						}

						listindex =
							(int)
							(new MathTree(null, objName.Substring(openBracketIndex + 1, closingBracketIndex - openBracketIndex - 1)))
								.Calculate(trigObj);
						objName = objName.Substring(0, openBracketIndex);
					}
					UberTreeParser.global_objs.TryGetValue(objName, out toReturn);

					if (toReturn != null)
					{
						if (listindex >= 0)
						{
							ptype = toReturn.GetType();
							if (ptype.GetInterface("IList") != null)
							{
								try
								{
									toReturn = ((IList)toReturn)[listindex];
								}
								catch (Exception e)
								{
									throw new UberScriptException("Get Array value error!", e);
								}
							}
							else
							{
								throw new UberScriptException("xmlobj." + objName + " was indexed with [] indexer, but did not contain a list!");
							}
						}

						if (arglist.Count > 2)
						{
							string propLookup = arglist[2]; // add this first so later additions all prepended with '.'
							for (int i = 3; i < arglist.Count; i++)
							{
								propLookup += "." + arglist[i];
							}
							toReturn = PropertyGetters.GetObject(trigObj, toReturn, propLookup, out ptype);
						}
					}
					return toReturn;
				}
			}
			else if (nodeValueString.StartsWith("xml"))
			{
				// get the nearest "this" (either the closest spawn node or whatever the script is attached to)
				IEntity toCheck = ((trigObj.Spawn == null) ? trigObj.This : trigObj.Spawn) as IEntity;
				if (toCheck != null && arglist.Count > 1)
				{
					if (arglist[0].ToLower() == "xmlints")
					{
						isDictionaryLookup = true;
						// check for existing xmlValue attachment or create a new one
						XmlValue xmlValue = XmlAttach.GetValueAttachment(toCheck, arglist[1]);
						if (xmlValue == null)
						{
							return null; //throw new UberScriptException("Could not find XmlValue named " + name + " on " + o);
						}
						if (arglist.Count > 2) // must be trying to get a property on the xmlValue (e.g. xmlints.test.expiration)
						{
							return PropertyGetters.GetObject(trigObj, xmlValue, arglist[2], out ptype);
						}
						return xmlValue.Value;
					}
					else if (arglist[0].ToLower() == "xmlstrings")
					{
						isDictionaryLookup = true;
						XmlLocalVariable xmlLocalVariable = XmlAttach.GetStringAttachment(toCheck, arglist[1]);
						if (xmlLocalVariable == null)
						{
							return null; // throw new UberScriptException("Could not find XmlLocalVariable named " + name + " on " + o);
						}
						if (arglist.Count > 2) // must be trying to get a property on the xmlValue (e.g. xmlints.test.expiration)
						{
							return PropertyGetters.GetObject(trigObj, xmlLocalVariable, arglist[2], out ptype);
						}
						return xmlLocalVariable.Data;
					}
					else if (arglist[0].ToLower() == "xmldoubles")
					{
						isDictionaryLookup = true;
						XmlDouble xmlDouble = XmlAttach.GetDoubleAttachment(toCheck, arglist[1]);
						if (xmlDouble == null)
						{
							return null; //  throw new UberScriptException("Could not find XmlDouble named " + name + " on " + o);
						}
						if (arglist.Count > 2) // must be trying to get a property on the xmlValue (e.g. xmlints.test.expiration)
						{
							return PropertyGetters.GetObject(trigObj, xmlDouble, arglist[2], out ptype);
						}
						return xmlDouble.Value;
					}
					else if (arglist[0].ToLower() == "xmlobjs")
					{
						isDictionaryLookup = true;

						// since the object might be a list of some kind, need to check for the [] indexing first
						string objName = arglist[1];
						int openBracketIndex = objName.IndexOf('[');
						int listindex = -1; // for lists / arrays
						if (openBracketIndex > 0)
						{
							int closingBracketIndex = objName.IndexOf(']');
							if (closingBracketIndex < openBracketIndex + 1)
							{
								throw new UberScriptException(
									"xmlobjs." + objName + " [] indexing error: must have at least 1 character between the []");
							}

							listindex =
								(int)
								(new MathTree(null, objName.Substring(openBracketIndex + 1, closingBracketIndex - openBracketIndex - 1)))
									.Calculate(trigObj);
							objName = objName.Substring(0, openBracketIndex);
						}

						XmlObject xmlObject = XmlAttach.GetObjectAttachment(toCheck, objName);
						if (xmlObject == null)
						{
							return null; // throw new UberScriptException("Could not find XmlObject named " + name + " on " + o);
						}
						//if (arglist.Count > 2) // must be trying to get a property on the xmlValue (e.g. xmlints.test.expiration)
						//    return GetObject(trigObject, xmlObject, arglist[2], out ptype);

						//=====
						if (arglist.Count > 2)
						{
							// XmlObject only contains a few properties that
							// can be accessed through statements like THIS().xmlobjs.test._____
							// since there is a potential conflict between the developer wanting access
							// to the properties on the object contained in the XmlObject.Value (most likely)
							// or the properties on the XmlObject itself (far less likely)
							string testPropName = arglist[2].ToLower();
							// to access properties on the xmlobject itself (e.g. expiration), one must do this:
							//  THIS().xmlobjs.test.xmlobject.expiration
							if (testPropName == "xmlobject")
							{
								if (arglist.Count < 4)
								{
									return xmlObject;
								}

								string propLookup = arglist[3]; // add this first so later additions all prepended with '.'
								for (int i = 4; i < arglist.Count; i++)
								{
									propLookup += "." + arglist[i];
								}
								return PropertyGetters.GetObject(trigObj, xmlObject, propLookup, out ptype);
							}
							else
							{
								if (xmlObject.Value == null)
								{
									return null;
								}
								string propLookup = arglist[2]; // add this first so later additions all prepended with '.'
								for (int i = 3; i < arglist.Count; i++)
								{
									propLookup += "." + arglist[i];
								}

								if (listindex >= 0)
								{
									ptype = xmlObject.Value.GetType();
									if (ptype.GetInterface("IList") != null)
									{
										try
										{
											return PropertyGetters.GetObject(trigObj, ((IList)xmlObject.Value)[listindex], propLookup, out ptype);
										}
										catch (Exception e)
										{
											throw new UberScriptException("Get Array value error!", e);
										}
									}
									else
									{
										throw new UberScriptException(
											"xmlobj." + objName + " was indexed with [] indexer, but did not contain a list!");
									}
								}

								return PropertyGetters.GetObject(trigObj, xmlObject.Value, propLookup, out ptype);
							}
						}
						else if (listindex >= 0)
						{
							// no properties afterward but has a list index (e.g. xmlobjs.spawnedPlatforms[0])
							ptype = xmlObject.Value.GetType();
							if (ptype.GetInterface("IList") != null)
							{
								try
								{
									return ((IList)xmlObject.Value)[listindex];
								}
								catch (Exception e)
								{
									throw new UberScriptException("Get Array value error!", e);
								}
							}
							else
							{
								throw new UberScriptException("xmlobj." + objName + " was indexed with [] indexer, but did not contain a list!");
							}
						}

						//====
						return xmlObject.Value;
					}
				}
			}
			isDictionaryLookup = false;
			return null;
		}

		/// <summary>
		///     Ugly function for taking Objects from the MathTree and adding them together...
		///     Not sure if there might be a better way (without calling up the C# compiler
		///     to sort things out which I think would be terrible performance)
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static Object Calculate(TriggerObject trigObj, MathNode node)
		{
			// ordered in the most probable order, (in my opinion)
			bool isInDictionary = false;
			if (node == null)
			{
				return null;
			}
			if (node.Left == null && node.Right == null)
			{
				if (node.OpTypeVal == OpType.Func)
				{
					return ((FunctionNode)node.Value).Execute(trigObj, false); // true???
				}
				else if (node.Value is ArrayList)
				{
					ArrayList original = (ArrayList)node.Value;
					if (original.Count > 0 && original[0] is MathTree)
					{
						// if 1 element is a mathtree, then they are all mathtrees
						// ... created using this syntax: ["hello", "heh" + " hah"]
						ArrayList outputList = new ArrayList(original.Count);
						for (int i = 0; i < original.Count; i++)
						{
							outputList.Add(((MathTree)original[i]).Calculate(trigObj));
						}
						return outputList;
					}
					// otherwise it's a regular arraylist--just return it
					return original;
				}
				else if (node.Value is string && (string)node.Value != string.Empty)
				{
					string nodeValueString = (string)node.Value;
					string nodeValueStringLower = nodeValueString.ToLower();
					if (nodeValueStringLower == "false")
					{
						return false;
					}
					else if (nodeValueStringLower == "true")
					{
						return true;
					}
						// check if it is one of the local or global function ints, strings, doubles, or objs
					else
					{
						object possibleReturnValue = CheckDictionaries(trigObj, nodeValueString, out isInDictionary);
						if (isInDictionary)
						{
							return possibleReturnValue;
						}
					}
				}
				return node.Value;
			}
			Object operand1;
			Object operand2;
			if (node.Left.OpTypeVal == OpType.Int || node.Left.OpTypeVal == OpType.Double || node.Left.OpTypeVal == OpType.String ||
				node.Left.OpTypeVal == OpType.UInt64 || node.Left.OpTypeVal == OpType.Bool)
			{
				operand1 = node.Left.Value;
				if (operand1 is string && (string)operand1 != "")
				{
					// check if it is one of the local or global function ints, strings, doubles, or objs
					string operand1String = (string)operand1;
					string operand1StringLower = operand1String.ToLower();
					if (operand1StringLower == "false")
					{
						operand1 = false;
					}
					else if (operand1StringLower == "true")
					{
						operand1 = true;
					}

					object possibleReturnValue = CheckDictionaries(trigObj, operand1String, out isInDictionary);
					if (isInDictionary)
					{
						operand1 = possibleReturnValue;
					}
				}
			}
			else
			{
				operand1 = Calculate(trigObj, node.Left);
			}

			// hacky solution to extraneous parenthesis
			if (node.OpTypeVal == OpType.OpenParenthesis)
			{
				return operand1;
			}

			if (node.OpTypeVal == OpType.And)
			{
				// check whether we even need to find operand 2 (since if the
				// first argument is false, then arg1 && arg2 is false
				if (operand1 is bool)
				{
					if ((bool)operand1 == false)
					{
						return false;
					}
				}
				else
				{
					throw new UberScriptException("Cannot use && operator between non boolean: " + operand1);
				}
			}
			else if (node.OpTypeVal == OpType.Or)
			{
				// check whether we even need to find operand 2 (since if the
				// first argument is true, then arg1 || arg2 is true
				if (operand1 is bool)
				{
					if ((bool)operand1)
					{
						return true;
					}
				}
				else
				{
					throw new UberScriptException("Cannot use || operator between non boolean: " + operand1);
				}
			}

			if (node.Right.OpTypeVal == OpType.Int || node.Right.OpTypeVal == OpType.Double ||
				node.Right.OpTypeVal == OpType.String || node.Right.OpTypeVal == OpType.UInt64 || node.Left.OpTypeVal == OpType.Bool)
			{
				operand2 = node.Right.Value;
				if (operand2 is string && (string)operand2 != "")
				{
					// check if it is one of the local or global function ints, strings, doubles, or objs
					string operand2String = (string)operand2;
					string operand2StringLower = operand2String.ToLower();
					if (operand2StringLower == "false")
					{
						operand2 = false;
					}
					else if (operand2StringLower == "true")
					{
						operand2 = true;
					}

					object possibleReturnValue = CheckDictionaries(trigObj, operand2String, out isInDictionary);
					if (isInDictionary)
					{
						operand2 = possibleReturnValue;
					}
				}
			}
			else
			{
				operand2 = Calculate(trigObj, node.Right);
			}

            switch (node.OpTypeVal)
            {
                case OpType.Add:
                    if (operand1 is int)
                    {
                        if (operand2 is int)
                        {
                            return (int)operand1 + (int)operand2;
                        }
                        else if (operand2 is double)
                        {
                            return (int)operand1 + (double)operand2;
                        }
                        else if (operand2 is string)
                        {
                            return operand1 + (string)operand2;
                        }
                        else if (operand2 is UInt64)
                        {
                            return Convert.ToUInt64(operand1) + (UInt64)operand2;
                        }
                    }
                    else if (operand1 is double)
                    {
                        if (operand2 is int)
                        {
                            return (double)operand1 + (int)operand2;
                        }
                        else if (operand2 is double)
                        {
                            return (double)operand1 + (double)operand2;
                        }
                        else if (operand2 is string)
                        {
                            return operand1 + (string)operand2;
                        }
                        else if (operand2 is UInt64)
                        {
                            return (double)operand1 + (UInt64)operand2;
                        }
                    }
                    else if (operand1 is string)
                    {
                        return (string)operand1 + operand2;
                    }
                    else if (operand1 is UInt64)
                    {
                        if (operand2 is int)
                        {
                            return (UInt64)operand1 + (UInt64)operand2;
                        }
                        else if (operand2 is double)
                        {
                            return (UInt64)operand1 + (double)operand2;
                        }
                        else if (operand2 is string)
                        {
                            return operand1 + (string)operand2;
                        }
                        else if (operand2 is UInt64)
                        {
                            return (UInt64)operand1 + (UInt64)operand2;
                        }
                    }
                    else if (operand2 is string)
                    {
                        return operand1 + (string)operand2;
                    }
                    else if (operand1 is DateTime)
                    {
                        if (operand2 is TimeSpan)
                        {
                            return (DateTime)operand1 + (TimeSpan)operand2;
                        }
                    }
                    break;
                
                
                case OpType.Func:
                    // evaluate the function, if it isn't one of the 4 "primitives", then call return ToString on it.
				    Object output = ((FunctionNode)node.Value).Execute(trigObj, true);
				    if (output is int || output is double || output is string || output is UInt64)
				    {
					    return output;
				    }
				    //if (output is Mobile) return ((Mobile)output).Serial;
				    //if (output is Item) return ((Item)output).Serial;
				    return output.ToString();
                
                case OpType.Sub:
                    if (operand1 is string)
				    {
					    throw new UberScriptException("attempted to subtract using a string!:" + operand1);
				    }
				    if (operand2 is string)
				    {
					    throw new UberScriptException("attempted to subtract using a string!:" + operand2);
				    }
				    if (operand1 is int)
				    {
					    if (operand2 is int)
					    {
						    return (int)operand1 - (int)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (int)operand1 - (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return Convert.ToUInt64(operand1) - (UInt64)operand2;
					    }
				    }
				    else if (operand1 is double)
				    {
					    if (operand2 is int)
					    {
						    return (double)operand1 - (int)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (double)operand1 - (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return (double)operand1 - (UInt64)operand2;
					    }
				    }
				    else if (operand1 is UInt64)
				    {
					    if (operand2 is int)
					    {
						    return (UInt64)operand1 - (UInt64)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (UInt64)operand1 - (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return (UInt64)operand1 - (UInt64)operand2;
					    }
				    }
				    else if (operand1 is DateTime)
				    {
					    if (operand2 is DateTime)
					    {
						    return (DateTime)operand1 - (DateTime)operand2;
					    }
					    else if (operand2 is TimeSpan)
					    {
						    return (DateTime)operand1 - (TimeSpan)operand2;
					    }
				    }
                    break;
                
                
                case OpType.Mul:
                    if (operand1 is string)
				    {
					    throw new UberScriptException("attempted to multiply using a string!:" + operand1);
				    }
				    if (operand2 is string)
				    {
					    throw new UberScriptException("attempted to multiply using a string!:" + operand2);
				    }
				    if (operand1 is int)
				    {
					    if (operand2 is int)
					    {
						    return (int)operand1 * (int)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (int)operand1 * (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return Convert.ToUInt64(operand1) * (UInt64)operand2;
					    }
				    }
				    else if (operand1 is double)
				    {
					    if (operand2 is int)
					    {
						    return (double)operand1 * (int)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (double)operand1 * (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return (double)operand1 * (UInt64)operand2;
					    }
				    }
				    else if (operand1 is UInt64)
				    {
					    if (operand2 is int)
					    {
						    return (UInt64)operand1 * (UInt64)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (UInt64)operand1 * (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return (UInt64)operand1 * (UInt64)operand2;
					    }
				    }
                    break;


                case OpType.GThan:
                    if (operand1 == null || operand2 == null)
				    {
					    throw new UberScriptException("GThan had null argument!");
				    }
				    if (operand1 is string)
				    {
					    throw new UberScriptException("attempted to GThan using a string!:" + operand1);
				    }
				    if (operand2 is string)
				    {
					    throw new UberScriptException("attempted to GThan using a string!:" + operand2);
				    }
				    if (operand1 is int)
				    {
					    if (operand2 is int)
					    {
						    return (int)operand1 > (int)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (int)operand1 > (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return Convert.ToUInt64(operand1) > (UInt64)operand2;
					    }
				    }
				    else if (operand1 is double)
				    {
					    if (operand2 is int)
					    {
						    return (double)operand1 > (int)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (double)operand1 > (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return (double)operand1 > (UInt64)operand2;
					    }
				    }
				    else if (operand1 is UInt64)
				    {
					    if (operand2 is int)
					    {
						    return (UInt64)operand1 > (UInt64)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (UInt64)operand1 > (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return (UInt64)operand1 > (UInt64)operand2;
					    }
				    }
				    else if (operand1 is DateTime)
				    {
					    if (operand2 is DateTime)
					    {
						    return (DateTime)operand1 > (DateTime)operand2;
					    }
				    }
                    break;


                case OpType.LThan:
                    if (operand1 == null || operand2 == null)
				    {
					    throw new UberScriptException("LThan had null argument!");
				    }
				    if (operand1 is string)
				    {
					    throw new UberScriptException("attempted to LThan using a string!:" + operand1);
				    }
				    if (operand2 is string)
				    {
					    throw new UberScriptException("attempted to LThan using a string!:" + operand2);
				    }
				    if (operand1 is int)
				    {
					    if (operand2 is int)
					    {
						    return (int)operand1 < (int)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (int)operand1 < (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return Convert.ToUInt64(operand1) < (UInt64)operand2;
					    }
				    }
				    else if (operand1 is double)
				    {
					    if (operand2 is int)
					    {
						    return (double)operand1 < (int)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (double)operand1 < (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return (double)operand1 < (UInt64)operand2;
					    }
				    }
				    else if (operand1 is UInt64)
				    {
					    if (operand2 is int)
					    {
						    return (UInt64)operand1 < (UInt64)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (UInt64)operand1 < (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return (UInt64)operand1 < (UInt64)operand2;
					    }
				    }
				    else if (operand1 is DateTime)
				    {
					    if (operand2 is DateTime)
					    {
						    return (DateTime)operand1 < (DateTime)operand2;
					    }
				    }
                    break;


                case OpType.GThanOrEqualTo:
                    if (operand1 == null || operand2 == null)
				    {
					    throw new UberScriptException("GThanOrEqualTo had null argument!");
				    }
				    if (operand1 is string)
				    {
					    throw new UberScriptException("attempted to GThanOrEqualTo using a string!:" + operand1);
				    }
				    if (operand2 is string)
				    {
					    throw new UberScriptException("attempted to GThanOrEqualTo using a string!:" + operand2);
				    }
				    if (operand1 is int)
				    {
					    if (operand2 is int)
					    {
						    return (int)operand1 >= (int)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (int)operand1 >= (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return Convert.ToUInt64(operand1) >= (UInt64)operand2;
					    }
				    }
				    else if (operand1 is double)
				    {
					    if (operand2 is int)
					    {
						    return (double)operand1 >= (int)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (double)operand1 >= (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return (double)operand1 >= (UInt64)operand2;
					    }
				    }
				    else if (operand1 is UInt64)
				    {
					    if (operand2 is int)
					    {
						    return (UInt64)operand1 >= (UInt64)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (UInt64)operand1 >= (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return (UInt64)operand1 >= (UInt64)operand2;
					    }
				    }
				    else if (operand1 is DateTime)
				    {
					    if (operand2 is DateTime)
					    {
						    return (DateTime)operand1 >= (DateTime)operand2;
					    }
				    }
                    break;


                case OpType.LThanOrEqualTo:
                    if (operand1 == null || operand2 == null)
				    {
					    throw new UberScriptException("LThanOrEqualTo had null argument!");
				    }
				    if (operand1 is string)
				    {
					    throw new UberScriptException("attempted to LThanOrEqualTo using a string!:" + operand1);
				    }
				    if (operand2 is string)
				    {
					    throw new UberScriptException("attempted to LThanOrEqualTo using a string!:" + operand2);
				    }
				    if (operand1 is int)
				    {
					    if (operand2 is int)
					    {
						    return (int)operand1 <= (int)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (int)operand1 <= (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return Convert.ToUInt64(operand1) <= (UInt64)operand2;
					    }
				    }
				    else if (operand1 is double)
				    {
					    if (operand2 is int)
					    {
						    return (double)operand1 <= (int)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (double)operand1 <= (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return (double)operand1 <= (UInt64)operand2;
					    }
				    }
				    else if (operand1 is UInt64)
				    {
					    if (operand2 is int)
					    {
						    return (UInt64)operand1 <= (UInt64)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (UInt64)operand1 <= (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return (UInt64)operand1 <= (UInt64)operand2;
					    }
				    }
				    else if (operand1 is DateTime)
				    {
					    if (operand2 is DateTime)
					    {
						    return (DateTime)operand1 <= (DateTime)operand2;
					    }
				    }
                    break;


                case OpType.EqualTo:
                    if (operand1 == operand2)
				    {
					    return true;
				    }
				    if (operand1 != null && operand2 == null)
				    {
					    return false;
				    }
				    if (operand1 == null && operand2 != null)
				    {
					    return false;
				    }
				    if (operand1 is string && !(operand2 is string))
				    {
					    return ((string)operand1).ToLower() == (operand2 == null ? "null" : operand2.ToString().ToLower());
				    }
				    if (!(operand1 is string) && operand2 is string)
				    {
					    return (operand1 == null ? "null" : operand1.ToString().ToLower()) == ((string)operand2).ToLower();
				    }
				    if (operand1 is string && operand2 is string)
				    {
					    return ((string)operand1).ToLower() == ((string)operand2).ToLower();
				    }
				    if (operand1 is int)
				    {
					    if (operand2 is int)
					    {
						    return (int)operand1 == (int)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (int)operand1 == (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return Convert.ToUInt64(operand1) == (UInt64)operand2;
					    }
				    }
				    else if (operand1 is double)
				    {
					    if (operand2 is int)
					    {
						    return (double)operand1 == (int)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (double)operand1 == (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return (double)operand1 == (UInt64)operand2;
					    }
				    }
				    else if (operand1 is UInt64)
				    {
					    if (operand2 is int)
					    {
						    return (UInt64)operand1 == (UInt64)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (UInt64)operand1 == (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return (UInt64)operand1 == (UInt64)operand2;
					    }
				    }
				    else if (operand1 is DateTime)
				    {
					    if (operand2 is DateTime)
					    {
						    return (DateTime)operand1 == (DateTime)operand2;
					    }
				    }
				    else
				    {
					    return operand1.Equals(operand2); // could be objects
				    }
                    break;


                case OpType.NotEqualTo:
				    if (operand1 == operand2)
				    {
					    return false;
				    }
				    if (operand1 != null && operand2 == null)
				    {
					    return true;
				    }
				    if (operand1 == null && operand2 != null)
				    {
					    return true;
				    }
				    if (operand1 is string && !(operand2 is string))
				    {
					    return ((string)operand1).ToLower() == (operand2 == null ? "null" : operand2.ToString().ToLower());
				    }
				    if (!(operand1 is string) && operand2 is string)
				    {
					    return (operand1 == null ? "null" : operand1.ToString().ToLower()) == ((string)operand2).ToLower();
				    }
				    if (operand1 is string && operand2 is string)
				    {
					    return ((string)operand1).ToLower() != ((string)operand2).ToLower();
				    }
				    if (operand1 is int)
				    {
					    if (operand2 is int)
					    {
						    return (int)operand1 != (int)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (int)operand1 != (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return Convert.ToUInt64(operand1) != (UInt64)operand2;
					    }
				    }
				    else if (operand1 is double)
				    {
					    if (operand2 is int)
					    {
						    return (double)operand1 != (int)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (double)operand1 != (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return (double)operand1 != (UInt64)operand2;
					    }
				    }
				    else if (operand1 is UInt64)
				    {
					    if (operand2 is int)
					    {
						    return (UInt64)operand1 != (UInt64)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (UInt64)operand1 != (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return (UInt64)operand1 != (UInt64)operand2;
					    }
				    }
				    else if (operand1 is DateTime)
				    {
					    if (operand2 is DateTime)
					    {
						    return (DateTime)operand1 != (DateTime)operand2;
					    }
				    }
				    else
				    {
					    return !operand1.Equals(operand2); // could be objects
				    }
                    break;
                
                
                case OpType.And:
                    // if we have reached this point, then we already know that operand1 is true!
				    // (because right after finding the first operand we check whether it's false or not)
				    if (!(operand2 is bool))
				    {
					    throw new UberScriptException("Cannot use && operator between non booleans: " + operand1 + " " + operand2);
				    }
				    return operand2;
                
                
                case OpType.Or:
                    // if we have reached this point, then we already know that operand1 is false!
				    // (because right after finding the first operand we check whether it's true or not)
				    if (!(operand2 is bool))
				    {
					    throw new UberScriptException("Cannot use || operator between non booleans: " + operand1 + " " + operand2);
				    }
				    return (bool)operand2;

                    
                case OpType.Div:
                    if (operand1 is string)
				    {
					    throw new UberScriptException("attempted to multiply using a string!:" + operand1);
				    }
				    if (operand2 is string)
				    {
					    throw new UberScriptException("attempted to multiply using a string!:" + operand2);
				    }
				    if (operand1 is int)
				    {
					    if (operand2 is int)
					    {
						    return (int)operand1 / (int)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (int)operand1 / (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return Convert.ToUInt64(operand1) / (UInt64)operand2;
					    }
				    }
				    else if (operand1 is double)
				    {
					    if (operand2 is int)
					    {
						    return (double)operand1 / (int)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (double)operand1 / (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return (double)operand1 / (UInt64)operand2;
					    }
				    }
				    else if (operand1 is UInt64)
				    {
					    if (operand2 is int)
					    {
						    return (UInt64)operand1 / (UInt64)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (UInt64)operand1 / (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return (UInt64)operand1 / (UInt64)operand2;
					    }
				    }
                    break;

                case OpType.Pow:
                    if (operand1 is string)
				    {
					    throw new UberScriptException("attempted to multiply using a string!:" + operand1);
				    }
				    if (operand2 is string)
				    {
					    throw new UberScriptException("attempted to multiply using a string!:" + operand2);
				    }
				    if (operand1 is int)
				    {
					    if (operand2 is int)
					    {
						    return Math.Pow((int)operand1, (int)operand2);
					    }
					    else if (operand2 is double)
					    {
						    return Math.Pow((int)operand1, (double)operand2);
					    }
					    else if (operand2 is UInt64)
					    {
						    return Math.Pow(Convert.ToUInt64(operand1), (UInt64)operand2);
					    }
				    }
				    else if (operand1 is double)
				    {
					    if (operand2 is int)
					    {
						    return Math.Pow((double)operand1, (int)operand2);
					    }
					    else if (operand2 is double)
					    {
						    return Math.Pow((double)operand1, (double)operand2);
					    }
					    else if (operand2 is UInt64)
					    {
						    return Math.Pow((double)operand1, (UInt64)operand2);
					    }
				    }
				    else if (operand1 is UInt64)
				    {
					    if (operand2 is int)
					    {
						    return Math.Pow((UInt64)operand1, (UInt64)operand2);
					    }
					    else if (operand2 is double)
					    {
						    return Math.Pow((UInt64)operand1, (double)operand2);
					    }
					    else if (operand2 is UInt64)
					    {
						    return Math.Pow((UInt64)operand1, (UInt64)operand2);
					    }
				    }
                    break;
                case OpType.Mod:
                    if (operand1 is string)
				    {
					    throw new UberScriptException("attempted to multiply using a string!:" + operand1);
				    }
				    if (operand2 is string)
				    {
					    throw new UberScriptException("attempted to multiply using a string!:" + operand2);
				    }
				    if (operand1 is int)
				    {
					    if (operand2 is int)
					    {
						    return (int)operand1 % (int)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (int)operand1 % (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return Convert.ToUInt64(operand1) % (UInt64)operand2;
					    }
				    }
				    else if (operand1 is double)
				    {
					    if (operand2 is int)
					    {
						    return (double)operand1 % (int)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (double)operand1 % (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return (double)operand1 % (UInt64)operand2;
					    }
				    }
				    else if (operand1 is UInt64)
				    {
					    if (operand2 is int)
					    {
						    return (UInt64)operand1 % (UInt64)operand2;
					    }
					    else if (operand2 is double)
					    {
						    return (UInt64)operand1 % (double)operand2;
					    }
					    else if (operand2 is UInt64)
					    {
						    return (UInt64)operand1 % (UInt64)operand2;
					    }
				    }
                    break;
            }

			throw new UberScriptException("Calculate MathNode did not have an assigned OpType!: value=" + node.Value);
		}
	}
}