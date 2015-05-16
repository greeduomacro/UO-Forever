#region References
using System;
#endregion

namespace Server.Engines.XmlSpawner2
{
	public class StatementNode : UberNode
	{
		public StatementNode(UberNode parent, string scriptInput)
			: base(parent, scriptInput)
		{ }

		public StatementNode(UberNode parent, string scriptInput, int lineNumber)
			: base(parent, scriptInput, lineNumber)
		{ }

		public string prop;
		public bool ContainsSpawnNode = false;
		public AssignmentType AssignType = AssignmentType.Regular;

		public enum AssignmentType
		{
			Regular,
			Plus,
			Minus,
			Mult,
			Divide
		}

		public override string ScriptString
		{
			get { return base.ScriptString; }
			set
			{
				if (value == null)
				{
					throw new UberScriptException("Line " + LineNumber + ": " + ScriptString + "\nStatement node string was null!");
				}

				value = value.Trim();
				base.ScriptString = value;

				// first find the = sign (assuming there is one)
				var args = value.Split('=');

				if (args.Length != 2)
				{
					throw new UberScriptException(
						"Line " + LineNumber + ": " + ScriptString + "\nStatement node did not have a single = sign!:" + value);
				}

				if (args[0].EndsWith("+"))
				{
					AssignType = AssignmentType.Plus;
					args[0] = args[0].Substring(0, args[0].Length - 1);
				}
				else if (args[0].EndsWith("-"))
				{
					AssignType = AssignmentType.Minus;
					args[0] = args[0].Substring(0, args[0].Length - 1);
				}
				else if (args[0].EndsWith("*"))
				{
					AssignType = AssignmentType.Mult;
					args[0] = args[0].Substring(0, args[0].Length - 1);
				}
				else if (args[0].EndsWith("/"))
				{
					AssignType = AssignmentType.Divide;
					args[0] = args[0].Substring(0, args[0].Length - 1);
				}

				// left hand side of the =
				// check if args[0] is a keyword like TRIGMOB or MOB or
				// check if it is a [] variable like TRIGMOB[counter]
				args[0] = args[0].Trim();

				if (UberScriptFunctions.IsFunctionString(args[0]))
				{
					Children.Add(new FunctionNode(this, args[0], LineNumber));
				}
				else
				{
					Children.Add(new ArgumentNode(this, args[0]));
				}

				// right hand side of the =
				args[1] = args[1].Trim();
				// check whether it's a spawn call
				// the string might have commas (indicating a spawn type with a constructor accepting arguments
				// such as "static,100"
				int commaIndex = args[1].IndexOf(',');

				Type spawnType = ScriptCompiler.FindTypeByName(commaIndex > 0 ? args[1].Substring(0, commaIndex) : args[1]);

				if (spawnType != null && typeof(Mobile).IsAssignableFrom(spawnType) || typeof(Item).IsAssignableFrom(spawnType))
				{
					Children.Add(new RootNode(this, args[1]));
					ContainsSpawnNode = true;
				}
				else
				{
					try
					{
						MathTree mathTree = new MathTree(this, args[1], LineNumber);
						Children.Add(mathTree);
					}
					catch (Exception e)
					{
						throw new UberScriptException(
							"Line " + LineNumber + ": " + ScriptString + "\nStatementNode Mathtree Parse Error: ", e);
					}
				}

				//if (UberScriptFunctions.IsFunctionString(args[1])) { Children.Add(new FunctionNode(this, args[1])); }
				//else { Children.Add(new ArgumentNode(this, args[1])); }
			}
		}

		public override ProcessResult Process(ProcessResult lastSiblingResult, TriggerObject trigObject)
		{
			// will always have 2 children, either function nodes or MathTree nodes
			Object o = null;
			string propname = null;
			Object value;
			string globalObjsString;
			string localObjsString;

			// 1st child is the property that will be set
			if (Children[0] is FunctionNode)
			{
				// this presumes that it is a function like THIS() or TRIGMOB()
				FunctionNode fn = Children[0] as FunctionNode;
				o = fn.Execute(trigObject, true);
				propname = fn.Property;
			}
			else // Children[0] is argument node
			{
				string scriptString = Children[0].ScriptString;
				int dotIndex = scriptString.IndexOf('.');

				if (dotIndex > -1)
				{
					if (scriptString.StartsWith("global_"))
					{
						if (scriptString.StartsWith("global_ints."))
						{
							UberTreeParser.global_ints[scriptString.Substring(12)] = (int)((MathTree)Children[1]).Calculate(trigObject);

							value = ((MathTree)Children[1]).Calculate(trigObject);

							switch (AssignType)
							{
								case AssignmentType.Plus:
									UberTreeParser.global_ints[scriptString.Substring(12)] =
										UberTreeParser.global_ints[scriptString.Substring(12)] + Convert.ToInt32(value);
									break;
								case AssignmentType.Minus:
									UberTreeParser.global_ints[scriptString.Substring(12)] =
										UberTreeParser.global_ints[scriptString.Substring(12)] - Convert.ToInt32(value);
									break;
								case AssignmentType.Mult:
									UberTreeParser.global_ints[scriptString.Substring(12)] =
										UberTreeParser.global_ints[scriptString.Substring(12)] * Convert.ToInt32(value);
									break;
								case AssignmentType.Divide:
									UberTreeParser.global_ints[scriptString.Substring(12)] =
										Convert.ToInt32(UberTreeParser.global_ints[scriptString.Substring(12)] / Convert.ToDouble(value));
									break;
								default:
									UberTreeParser.global_ints[scriptString.Substring(12)] = Convert.ToInt32(value);
									break;
							}

							return ProcessResult.None;
						}

						if (scriptString.StartsWith("global_strings."))
						{
							value = GetReturnObject(Children[1], trigObject);

							if (value != null)
							{
								value = value.ToString();

								switch (AssignType)
								{
									case AssignmentType.Plus:
										UberTreeParser.global_strings[scriptString.Substring(15)] =
											UberTreeParser.global_strings[scriptString.Substring(15)] + (string)value;
										break;
									case AssignmentType.Regular:
										UberTreeParser.global_strings[scriptString.Substring(15)] = (string)value;
										break;
								}

								throw new UberScriptException(
									"Line " + LineNumber + ": " + ScriptString + "\nTried to use +=, -=, *=, or /= but this is only supported for " +
									value.GetType() + "! It is only supported for int, double, and UInt64");
								//return ProcessResult.None;
							}

							throw new UberScriptException(
								"Line " + LineNumber + ": " + ScriptString +
								"\nTried to add null value to strings dictionary... this is not allowed!");
						}

						if (scriptString.StartsWith("global_doubles."))
						{
							value = ((MathTree)Children[1]).Calculate(trigObject);

							switch (AssignType)
							{
								case AssignmentType.Plus:
									UberTreeParser.global_doubles[scriptString.Substring(15)] =
										UberTreeParser.global_doubles[scriptString.Substring(15)] + Convert.ToDouble(value);
									break;
								case AssignmentType.Minus:
									UberTreeParser.global_doubles[scriptString.Substring(15)] =
										UberTreeParser.global_doubles[scriptString.Substring(15)] - Convert.ToDouble(value);
									break;
								case AssignmentType.Mult:
									UberTreeParser.global_doubles[scriptString.Substring(15)] =
										UberTreeParser.global_doubles[scriptString.Substring(15)] * Convert.ToDouble(value);
									break;
								case AssignmentType.Divide:
									UberTreeParser.global_doubles[scriptString.Substring(15)] =
										UberTreeParser.global_doubles[scriptString.Substring(15)] / Convert.ToDouble(value);
									break;
								default:
									UberTreeParser.global_doubles[scriptString.Substring(15)] = Convert.ToDouble(value);
									break;
							}

							return ProcessResult.None;
						}

						if (scriptString.StartsWith("global_objs."))
						{
							globalObjsString = scriptString.Substring(12);

							int secondDotIndex = globalObjsString.IndexOf('.');

							if (secondDotIndex > -1) // we are trying to set a property on an existing object
							{
								// will throw error if it doesn't exist yet
								o = UberTreeParser.global_objs[globalObjsString.Substring(0, secondDotIndex)];
								propname = globalObjsString.Substring(secondDotIndex + 1);
							}
							else
							{
								UberTreeParser.global_objs[globalObjsString] = GetReturnObject(Children[1], trigObject);
								return ProcessResult.None;
							}
						}
					}
					else if (scriptString.StartsWith("ints."))
					{
						value = ((MathTree)Children[1]).Calculate(trigObject);

						switch (AssignType)
						{
							case AssignmentType.Plus:
								trigObject.ints[scriptString.Substring(5)] = trigObject.ints[scriptString.Substring(5)] + Convert.ToInt32(value);
								break;
							case AssignmentType.Minus:
								trigObject.ints[scriptString.Substring(5)] = trigObject.ints[scriptString.Substring(5)] - Convert.ToInt32(value);
								break;
							case AssignmentType.Mult:
								trigObject.ints[scriptString.Substring(5)] = trigObject.ints[scriptString.Substring(5)] * Convert.ToInt32(value);
								break;
							case AssignmentType.Divide:
								trigObject.ints[scriptString.Substring(5)] =
									Convert.ToInt32(trigObject.ints[scriptString.Substring(5)] / Convert.ToDouble(value));
								break;
							default:
								trigObject.ints[scriptString.Substring(5)] = Convert.ToInt32(value);
								break;
						}

						return ProcessResult.None;
					}
					else if (scriptString.StartsWith("strings."))
					{
						value = GetReturnObject(Children[1], trigObject);

						if (value != null)
						{
							value = value.ToString();

							switch (AssignType)
							{
								case AssignmentType.Plus:
									trigObject.strings[scriptString.Substring(8)] = trigObject.strings[scriptString.Substring(8)] + (string)value;
									break;
								case AssignmentType.Regular:
									trigObject.strings[scriptString.Substring(8)] = (string)value;
									break;
								default:
									throw new UberScriptException(
										"Line " + LineNumber + ": " + ScriptString +
										"\nTried to use +=, -=, *=, or /= but this is only supported for " + value.GetType() +
										"! It is only supported for int, double, and UInt64");
							}
							return ProcessResult.None;
						}

						throw new UberScriptException(
							"Line " + LineNumber + ": " + ScriptString +
							"\nTried to add null value to strings dictionary... this is not allowed!");
					}
					else if (scriptString.StartsWith("doubles."))
					{
						value = ((MathTree)Children[1]).Calculate(trigObject);

						switch (AssignType)
						{
							case AssignmentType.Plus:
								trigObject.doubles[scriptString.Substring(8)] = trigObject.doubles[scriptString.Substring(8)] +
																				Convert.ToDouble(value);
								break;
							case AssignmentType.Minus:
								trigObject.doubles[scriptString.Substring(8)] = trigObject.doubles[scriptString.Substring(8)] -
																				Convert.ToDouble(value);
								break;
							case AssignmentType.Mult:
								trigObject.doubles[scriptString.Substring(8)] = trigObject.doubles[scriptString.Substring(8)] *
																				Convert.ToDouble(value);
								break;
							case AssignmentType.Divide:
								trigObject.doubles[scriptString.Substring(8)] = trigObject.doubles[scriptString.Substring(8)] /
																				Convert.ToDouble(value);
								break;
							default:
								trigObject.doubles[scriptString.Substring(8)] = Convert.ToDouble(value);
								break;
						}

						return ProcessResult.None;
					}
					else if (scriptString.StartsWith("objs."))
					{
						localObjsString = scriptString.Substring(5);

						int secondDotIndex = localObjsString.IndexOf('.');

						if (secondDotIndex > -1) // we are trying to set a property on an existing object
						{
							if (!trigObject.objs.TryGetValue(localObjsString.Substring(0, secondDotIndex), out o))
							{
								// will throw error if it doesn't exist yet
								throw new UberScriptException(
									"local objs dictionary did not have entry: " + localObjsString.Substring(0, secondDotIndex));
							}

							propname = localObjsString.Substring(secondDotIndex + 1);
						}
						else
						{
							trigObject.objs[localObjsString] = GetReturnObject(Children[1], trigObject);
							return ProcessResult.None;
						}
					}
				}

				// only makes it here if we aren't editing global variables directly 
				// (or if the we are returning a global object, e.g. global_objs.goober.hits
				if (o == null)
				{
					o = trigObject.Spawn ?? trigObject.This;
					propname = scriptString;
				}
			}

			// 2nd child is the value, but could have an operation in it and needs to be parsed
			try
			{
				if (AssignType == AssignmentType.Regular)
				{
					value = GetReturnObject(Children[1], trigObject);
				}
				else
				{
					Type ptype;

					value = PropertyGetters.GetObject(trigObject, o, propname, out ptype);

					if (value is int)
					{
						switch (AssignType)
						{
							case AssignmentType.Plus:
								value = (int)value + Convert.ToInt32(GetReturnObject(Children[1], trigObject));
								break;
							case AssignmentType.Minus:
								value = (int)value - Convert.ToInt32(GetReturnObject(Children[1], trigObject));
								break;
							case AssignmentType.Mult:
								value = Convert.ToInt32((int)value * Convert.ToDouble(GetReturnObject(Children[1], trigObject)));
								break;
							case AssignmentType.Divide:
								value = Convert.ToInt32((int)value / Convert.ToDouble(GetReturnObject(Children[1], trigObject)));
								break;
						}
					}
					else if (value is double)
					{
						switch (AssignType)
						{
							case AssignmentType.Plus:
								value = (double)value + Convert.ToDouble(GetReturnObject(Children[1], trigObject));
								break;
							case AssignmentType.Minus:
								value = (double)value - Convert.ToDouble(GetReturnObject(Children[1], trigObject));
								break;
							case AssignmentType.Mult:
								value = (double)value * Convert.ToDouble(GetReturnObject(Children[1], trigObject));
								break;
							case AssignmentType.Divide:
								value = (double)value / Convert.ToDouble(GetReturnObject(Children[1], trigObject));
								break;
						}
					}
					else if (value is UInt64)
					{
						switch (AssignType)
						{
							case AssignmentType.Plus:
								value = (UInt64)value + Convert.ToUInt64(GetReturnObject(Children[1], trigObject));
								break;
							case AssignmentType.Minus:
								value = (UInt64)value - Convert.ToUInt64(GetReturnObject(Children[1], trigObject));
								break;
							case AssignmentType.Mult:
								value = Convert.ToUInt64((UInt64)value * Convert.ToDouble(GetReturnObject(Children[1], trigObject)));
								break;
							case AssignmentType.Divide:
								value = Convert.ToUInt64((UInt64)value / Convert.ToDouble(GetReturnObject(Children[1], trigObject)));
								break;
						}
					}
					else if (value is string)
					{
						if (AssignType == AssignmentType.Plus)
						{
							value = (string)value + GetReturnObject(Children[1], trigObject);
						}
						else
						{
							throw new UberScriptException(
								"Tried to use +=, -=, *=, or /= but this is only supported for " + value.GetType() +
								"! It is only supported for int, double, and UInt64");
						}
					}
					else
					{
						//throw new UberScriptException("Tried to use +=, -=, *=, or /= on a non-numeric left-hand side!");
						throw new UberScriptException(
							"Tried to use +=, -=, *=, or /= but this is only supported for " + value.GetType() +
							"! It is only supported for int, double, and UInt64");
					}
				}
			}
			catch (Exception e)
			{
				throw new UberScriptException("Line " + LineNumber + ": " + ScriptString + "\nGetReturnObject Error:", e);
			}

			try
			{
				PropertySetters.SetPropertyValue(trigObject, o, propname, value);
			}
			catch (Exception e)
			{
				throw new UberScriptException("Line " + LineNumber + ": " + ScriptString + "\nError setting value:", e);
			}

			return ProcessResult.None;
		}

		private Object GetReturnObject(UberNode node, TriggerObject trigObject)
		{
			if (node is RootNode)
			{
				// it's a spawn node, execute it
				return ((RootNode)Children[1]).SpawnAndReturnObject(ProcessResult.None, trigObject);
			}

			if (node is MathTree)
			{
				return ((MathTree)Children[1]).Calculate(trigObject);
			}

			throw new UberScriptException(
				"Line " + LineNumber + ": " + ScriptString + "\nStatement node did not have MathTree second child!");
		}
	}
}