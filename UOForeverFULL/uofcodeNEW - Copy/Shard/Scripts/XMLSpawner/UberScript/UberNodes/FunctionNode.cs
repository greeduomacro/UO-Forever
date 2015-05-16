#region References
using System;
using System.Collections.Generic;
#endregion

namespace Server.Engines.XmlSpawner2
{
	public class FunctionNode : UberNode
	{
		public bool NegateOutput = false;

		public string Property;

		public string OriginalString { get; protected set; }

		public override string ScriptString { get { return base.ScriptString; } set { Parse(value); } }

		public FunctionNode(UberNode parent, string scriptInput)
			: base(parent, scriptInput)
		{ }

		public FunctionNode(UberNode parent, string scriptInput, int lineNumber)
			: base(parent, scriptInput, lineNumber)
		{ }

		protected void Parse(string value)
		{
			if (value == null)
			{
				throw new UberScriptException(
					String.Format("Parse: ScriptString value is null at line {0}:\n{1}", LineNumber, OriginalString));
			}

			try
			{
				OriginalString = value;
				value = value.Trim();

				int index = 0;

				// first get the function name
				for (int i = 0; i < value.Length; i++)
				{
					if (value[i] != '(')
					{
						continue;
					}

					base.ScriptString = value.Substring(index, i - index);
					value = value.Substring(i);
					break;
				}
				
				index = 0;

				// next, process each argument individually as a MathTree
				int numOpenParens = 0;
				
				string dotOperator = "";
				bool insideQuotation = false;
				bool justEscapedBackslash = false;
				
				for (int i = 0; i < value.Length; i++)
				{
					char c = value[i];
				
					if (insideQuotation && c != '"')
					{
						if (value[i] == '\\')
						{
							if (i > 0 && value[i - 1] == '\\' && !justEscapedBackslash)
							{
								justEscapedBackslash = true;
							}
						}
						else
						{
							justEscapedBackslash = false;
						}

						continue;
					}
					
					if (i > 0 && numOpenParens == 0)
					{
						// should be outside the function and only adding . operators
						dotOperator += c;
						continue;
					}

					switch (c)
					{
						case '(':
							{
								numOpenParens++;
								
								if (i == 0)
								{
									index = 1;
								} // only move the index after the first openParen
							}
							break;
						case ')':
							{
								numOpenParens--;
								
								if (numOpenParens < 0)
								{
									throw new UberScriptException(
										String.Format("Parse: parenthesis mismatch detected at line {0}:\n{1}", LineNumber, OriginalString));
								}
								
								if (numOpenParens == 0) // final parenthesis
								{
									// process final argument
									MathTree mathTree = new MathTree(this, value.Substring(index, i - index), LineNumber);
									Children.Add(mathTree);
								}
							}
							break;
						case '"':
							{
								if (i > 0 && value[i - 1] == '\\')
								{
									// leave it in as-is-- MathTree will take care of it
									if (justEscapedBackslash)
									{
										insideQuotation = !insideQuotation;
									}
								}
								else
								{
									// leave it in as-is-- MathTree will take care of is
									// just need to indicate we are inside quotation so it doesn't
									// do anything else (e.g. with commas in "hey, what are you doing?")
									insideQuotation = !insideQuotation;
								}
							}
							break;
						case ',':
							{
								if (numOpenParens == 1) // within the main function still
								{
									// we can process the argument into a MathTree
									try
									{
										MathTree mathTree = new MathTree(this, value.Substring(index, i - index), LineNumber);
										Children.Add(mathTree);
										index = i + 1;
									}
									catch (Exception x)
									{
										throw new UberScriptException(
											String.Format(
												"Parse: MathTree parse failed for '{0}' at line {1}:\n{2}",
												value.Substring(index, i - index),
												LineNumber,
												OriginalString),
											x);
									}
								}
							}
							break;
					}
				}

				if (numOpenParens > 0)
				{
					throw new UberScriptException(
						String.Format("Parse: parenthesis mismatch detected at line {0}:\n{1}", LineNumber, OriginalString));
				}

				if (insideQuotation)
				{
					throw new UberScriptException(
						String.Format("Parse: quotation mismatch detected at line {0}:\n{1}", LineNumber, OriginalString));
				}

				if (String.IsNullOrWhiteSpace(dotOperator))
				{
					return;
				}

				dotOperator = dotOperator.Trim();

				if (!dotOperator.StartsWith(".") || dotOperator.Length < 2)
				{
					throw new UberScriptException(
						String.Format("Parse: unexpected access token '{0}' at line {1}:\n{2}", dotOperator, LineNumber, OriginalString));
				}

				Property = dotOperator.Substring(1);
			}
			catch (Exception x)
			{
				throw new UberScriptException(
					String.Format("Parse: an exception was thrown at line {0}:\n{1}", LineNumber, OriginalString), x);
			}
		}

		public Object Execute(TriggerObject trigObject, bool tryReturnObject = false)
		{
			if (trigObject == null)
			{
				throw new UberScriptException("Execute: trigObject reference is null");
			}

			var args = new List<Object> {
				trigObject
			};

			// the args of the function are actually
			// stored in nodes--either function or argument... e.g.
			// EFFECT(14000,25, THIS().x, THIS().y, THIS().z)
			// has 2 argument nodes and 3 function nodes

			// each child of a FunctionNode is an argument represented by a MathTree
			foreach (UberNode child in Children)
			{
				if (!(child is MathTree))
				{
					throw new UberScriptException(
						String.Format("Execute: MathTree child expected at line {0}:\n{1}", LineNumber, OriginalString));
				}

				MathTree mathTree = child as MathTree;
				
				if (!mathTree.IsEmpty())
				{
					args.Add(mathTree.Calculate(trigObject));
				}
			}
			
			Object obj;
			
			try
			{
				// FunctionNode scriptstring contains function name
				obj = UberScriptFunctions.Invoke(ScriptString, args.ToArray());
			}
			catch (Exception x)
			{
				throw new UberScriptException(
					String.Format("Execute: an exception was thrown during invocation at line {0}:\n{1}", LineNumber, OriginalString),
					x);
			}

			if (Property == null || tryReturnObject)
			{
				return obj;
			}
			
			Type ptype;

			try
			{
				obj = PropertyGetters.GetObject(trigObject, obj, Property, out ptype);

				if (obj == null)
				{
					return null; // it's ok to be null here
				}
			}
			catch (Exception x)
			{
				throw new UberScriptException(
					String.Format(
						"Execute: value could not be set on function '{0}' output object at line {1}:\n{2}",
						ScriptString,
						LineNumber,
						OriginalString),
					x);
			}

			if (ptype == null)
			{
				throw new UberScriptException(
					String.Format(
						"Execute: property '{0}' does not exist on function '{1}' output object at line {2}:\n{3}",
						Property,
						ScriptString,
						LineNumber,
						OriginalString));
			}
			
			if (NegateOutput)
			{
				if (obj is sbyte)
				{
					obj = -(sbyte)obj;
				}
				else if (obj is short)
				{
					obj = -(short)obj;
				}
				else if (obj is int)
				{
					obj = -(int)obj;
				}
				else if (obj is long)
				{
					obj = -(long)obj;
				}
				else
				{
					throw new UberScriptException(
						String.Format(
							"Execute: output negation failed on function '{0}' output object type '{1}' at line {2}:\n{3}",
							ScriptString,
							obj.GetType(),
							LineNumber,
							OriginalString));
				}
			}

			return obj;
		}

		public override ProcessResult Process(ProcessResult lastSiblingResult, TriggerObject trigObject)
		{
			Execute(trigObject);
			return ProcessResult.None;
		}
	}
}