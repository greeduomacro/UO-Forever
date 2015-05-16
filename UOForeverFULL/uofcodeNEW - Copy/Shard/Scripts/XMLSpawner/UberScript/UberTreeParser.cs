#region References
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
#endregion

namespace Server.Engines.XmlSpawner2
{
	public class CleanLine
	{
		public int LineNumber = -1;
		public string Value = null;

		public CleanLine(int lineNumber, string value)
		{
			LineNumber = lineNumber;
			Value = value;
		}
	}

	public class UberTreeParser
	{
		public static Dictionary<string, int> global_ints = new Dictionary<string, int>();
		public static Dictionary<string, string> global_strings = new Dictionary<string, string>();
		public static Dictionary<string, double> global_doubles = new Dictionary<string, double>();
		public static Dictionary<string, object> global_objs = new Dictionary<string, object>();

		public static string CurrentFileBeingParsed = null;

		public static RootNode ParseFile(string fileName)
		{
			return ParseFileContents(File.ReadAllLines(fileName), fileName);
		}

		public static RootNode ParseFileContents(string[] lines, string fileName)
		{
			CurrentFileBeingParsed = fileName;
			
			// ERROR HANDLING OUTSIDE OF THIS FUNCTION
			var balanceStack = new Stack<UberNode>();
			RootNode root = new RootNode(null, null);
			RootNode currentRoot = root; // spawn nodes are root nodes
			UberNode current = root;
			UberNode prev = null;
			int currentLineNumber = 0;
			int openCurlyBraces = 0;

			var cleanedStrings = new List<CleanLine>();
			
			foreach (string line in lines)
			{
				//Console.WriteLine(line);
				currentLineNumber++;

				StringBuilder sb = new StringBuilder();
				string cleanedLine = line.Trim();
				
				if (cleanedLine.StartsWith("//"))
				{
					continue;
				}

				bool insideQuotation = false;
				bool justEscapedBackslash = false;
				bool possibleComment = false;
				int forLoopRemainingSemiColons = 0;

				for (int i = 0; i < cleanedLine.Length; i++)
				{
					if (cleanedLine[i] == '\t')
					{
						continue; // always remove tabs... not allowed even in strings..
					}

					if (cleanedLine[i] == '\\')
					{
						if (!insideQuotation)
						{
							throw new UberScriptException(
								String.Format(
									"Parse: unexpected escape token '\\' in '{0}' at line {1}:\n{2}", fileName, currentLineNumber, cleanedLine));
						}

						if (!justEscapedBackslash)
						{
							if (i > 0 && cleanedLine[i - 1] == '\\')
							{
								justEscapedBackslash = true;
							}
						}
						else
						{
							justEscapedBackslash = false;
						}

						sb.Append('\\'); // keep the double backslash in there regardless (it is handled in MathTree.ParseOperand)
						continue;
					}

					if (cleanedLine[i] == '"')
					{
						if (!(i > 0 && cleanedLine[i - 1] == '\\' && !justEscapedBackslash))
						{
							insideQuotation = !insideQuotation;
						}
						
						sb.Append("\""); // add \" to the string
						continue;
					}
					
					justEscapedBackslash = false; // make sure to reset this if you have gotten to this point

					if (insideQuotation)
					{
						sb.Append(cleanedLine[i]);
						continue;
					}

					if (cleanedLine[i] == '/')
					{
						if (possibleComment)
						{
							sb.Remove(sb.Length - 1, 1); // remove the previously added / character
							break;
						}
						
						possibleComment = true;
						sb.Append('/');
						continue;
					}
				
					possibleComment = false;

					if (cleanedLine[i] == ' ' || cleanedLine[i] == '\t')
					{
						continue;
					}

					// handle lines with multiple delimiters (e.g. orc { name = "Bob"; } )
					if (cleanedLine[i] == '{' || cleanedLine[i] == '}')
					{
						// check if unbalanced
						if (cleanedLine[i] == '{')
						{
							openCurlyBraces++;
						}
						else
						{
							openCurlyBraces--;
						}

						if (openCurlyBraces < 0)
						{
							throw new UberScriptException(
								String.Format(
									"Parse: brace mismatch detected in '{0}' at line {1}:\n{2}", fileName, currentLineNumber, cleanedLine));
						}

						// add everything before the { or } (if there is anything)
						if (i > 0)
						{
							cleanedStrings.Add(new CleanLine(currentLineNumber, sb.ToString()));
						}
						
						cleanedStrings.Add(new CleanLine(currentLineNumber, cleanedLine[i].ToString(CultureInfo.InvariantCulture)));
						
						// add { or } on separate "line" (it's processed as a line)
						cleanedLine = cleanedLine.Substring(i + 1);
						i = -1;
						sb.Clear();

						continue;
					}
					
					if (cleanedLine[i] == ';')
					{
						if (cleanedLine.Length == i + 1)
						{
							break;
						} // we are at the end of the loop, ignore lines ending in semicolon

						string stringPreviousToSemiColon = sb.ToString();
						
						if (stringPreviousToSemiColon.Contains("for"))
						{
							if (stringPreviousToSemiColon.Contains("foreach("))
							{
								forLoopRemainingSemiColons += 1;
							}
							else if (stringPreviousToSemiColon.Contains("for(")) // for loop has 2 semicolons
							{
								forLoopRemainingSemiColons += 2;
							}
						}
						
						if (forLoopRemainingSemiColons == 0)
						{
							// we have encountered a statement-separating semi-colon!
							// add everything before the { or } (if there is anything)
							if (i > 0)
							{
								cleanedStrings.Add(new CleanLine(currentLineNumber, sb.ToString()));
							}
							
							cleanedLine = cleanedLine.Substring(i + 1);
							i = -1;
							sb.Clear();

							continue;
						}
						
						forLoopRemainingSemiColons--;
					}
					
					sb.Append(cleanedLine[i]);
				}
				
				cleanedLine = sb.ToString();

				if (!String.IsNullOrWhiteSpace(cleanedLine) && !cleanedLine.StartsWith("//"))
				{
					cleanedStrings.Add(new CleanLine(currentLineNumber, sb.ToString()));
				}
			}

			foreach (CleanLine cleanedString in cleanedStrings)
			{
				// i have to use this CleanLine object to keep track of line numbers
				// in spite of {, }, and ; characters splitting the lines up!
				currentLineNumber = cleanedString.LineNumber;
				
				string cleanedLine = cleanedString.Value;
				
				if (cleanedLine == "" || cleanedLine.StartsWith("//"))
				{
					continue;
				}

				if (cleanedLine == "{")
				{
					balanceStack.Push(current);
					
					if (prev != null)
					{
						current = prev;
					}
					
					continue;
				}

				if (cleanedLine == "}")
				{
					if (balanceStack.Count > 0)
					{
						current = balanceStack.Pop();
						
						if (current is RootNode)
						{
							// likely we have finished procesinto a spawn nodes (ChildRoots)
							// and therefore need to go back to the spawn node's root
							// so triggers are correctly assigned to it
							currentRoot = current.GetRootNode;
						}

						continue;
					}

					throw new UberScriptException(
						String.Format(
							"Parse: brace mismatch detected in '{0}' at line {1}:\n{2}", fileName, currentLineNumber, cleanedLine));
				}
				// remove ending ";" characters (doesn't matter if you end with ; or not)
				// CANNOT do more than one statement on a line, though.                

				// determine which type of node it is
				string lowerCase = cleanedLine.ToLower();
				
				if (UberScriptTriggers.HasTrigger(lowerCase))
				{
					prev = new TriggerNode(current, lowerCase);
					
					//Console.WriteLine(current + "->" + prev);
					
					currentRoot.TriggerNodes.Add(prev as TriggerNode);
				}
				else if (char.IsDigit(cleanedLine[0])) // must be a sequence node
				{
					int stage;

					if (!Int32.TryParse(cleanedLine, out stage))
					{
						throw new UberScriptException(
							String.Format(
								"Parse: incorrect sequence format, integer expected at line {0}:\n{1}", currentLineNumber, cleanedLine));
					}

					prev = new SequenceNode(current, cleanedLine, stage);
				}
				else if (cleanedLine.StartsWith("foreach"))
				{
					/*
					objs.moblist = GETNEARBYMOBS(TRIGMOB(),5)
					
					foreach(objs.mob ; objs.moblist)
					{
					}
					*/

					prev = new ForEachNode(current, cleanedLine, currentLineNumber);
				}
				else if (cleanedLine.StartsWith("for"))
				{	
					/*
					for(ints.i = 0; ints.i < TRIGMOB().hits; ints.i = ints.i + 1)
					{
					}
					*/
				
					prev = new ForLoopNode(current, cleanedLine, currentLineNumber);
				}
				else if (cleanedLine.StartsWith("if") || cleanedLine.StartsWith("elif") || cleanedLine.StartsWith("else") ||
						 cleanedLine.StartsWith("elseif"))
				{
					prev = new ConditionalNode(current, cleanedLine, currentLineNumber);
					
					//Console.WriteLine(current + "->" + prev);
				}
				else if (cleanedLine.StartsWith("return"))
				{
					prev = new ReturnNode(current, cleanedLine);
					
					if (cleanedLine == "returnoverride")
					{
						((ReturnNode)prev).Override = true;
					}
				}
				else switch (cleanedLine)
				{
					case "break":
						{
							// check whether it has a foreach / for ancestor
							UberNode testParent = current;
							bool foundForNodeAncestor = false;
							
							while (testParent != null)
							{
								if (testParent is ForEachNode || testParent is ForLoopNode)
								{
									foundForNodeAncestor = true;
									break;
								}
								testParent = testParent.Parent;
							}

							if (!foundForNodeAncestor)
							{
								throw new UberScriptException(
									String.Format("Parse: unexpected 'break' statement at line {0}:\n{1}", currentLineNumber, cleanedLine));
							}

							prev = new BreakNode(current, cleanedLine);
						}
						break;
					case "continue":
						{
							// check whether it has a foreach / for ancestor
							UberNode testParent = current;
							bool foundForNodeAncestor = false;
							
							while (testParent != null)
							{
								if (testParent is ForEachNode || testParent is ForLoopNode)
								{
									foundForNodeAncestor = true;
									break;
								}
								
								testParent = testParent.Parent;
							}

							if (!foundForNodeAncestor)
							{
								throw new UberScriptException(
									String.Format("Parse: unexpected 'continue' statement at line {0}:\n{1}", currentLineNumber, cleanedLine));
							}

							prev = new ContinueNode(current, cleanedLine);
						}
						break;
					default:
						{
							if (cleanedLine.StartsWith("pause"))
							{
								double pauseDuration;

								if (!Double.TryParse(cleanedLine.Substring(5), out pauseDuration))
								{
									pauseDuration = 0;
								}

								prev = new PauseNode(current, cleanedLine, pauseDuration);
							}
							else if (cleanedLine.StartsWith("function"))
							{
								prev = new UserDefinedFunctionNode(current, lowerCase);
								//Console.WriteLine(current + "->" + prev);
								root.UserDefinedFunctionNodes.Add(prev as UserDefinedFunctionNode);
							}
							/*
							else if (cleanedLine.StartsWith("spawngroup"))
							{
								prev = new SpawnGroupNode
							}
							else if (cleanedLine.StartsWith("spawnregion"))
							{

							}
							else if (cleanedLine.StartsWith("spawnentry"))
							{

							}
							*/
							else
							{
								int equalCount = 0;
								//char beforeChar = '\0';
								int numOpenParentheses = 0;
								bool inQuotes = false;

								for (int i = 0; i < cleanedLine.Length; i++)
								{
									if (!inQuotes)
									{
										if (cleanedLine[i] == '=' && numOpenParentheses == 0)
										{
											equalCount++;
										}
										else
											switch (cleanedLine[i])
											{
												case '(':
													numOpenParentheses++;
													break;
												case ')':
													{
														numOpenParentheses--;

														if (numOpenParentheses < 0)
														{
															throw new UberScriptException(
																String.Format(
																	"Parse: parentheses mismatch detected in '{0}' at line {1}:\n{2}",
																	fileName,
																	currentLineNumber,
																	cleanedLine));
														}
													}
													break;
											}
									}

									if (cleanedLine[i] != '"')
									{
										continue;
									}

									inQuotes = !inQuotes; // assume that it's not an escaped quotation

									// determine if it is an escaped quotation (and that there aren't escaped backslashes preceding it)
									for (int ix = i - 1; ix >= 0 && cleanedLine[ix] == '\\'; ix--)
									{
										inQuotes = !inQuotes; // for each backslash behind, unescape or escape accordingly
									}
								}

								if (equalCount == 1) // it's a statement node
								{
									prev = new StatementNode(current, cleanedLine, currentLineNumber);
									//Console.WriteLine(current + "->" + prev);

									if (((StatementNode)prev).ContainsSpawnNode)
									{
										// global_objs.test = orc
										// {
										//      name = goober
										// }
										prev.LineNumber = currentLineNumber; // we "continue" here so we set this later
										current.Children.Add(prev); // we "continue" here so we don't add it later
										//Console.WriteLine(current + "->" + prev + "... adding spawn node to statement node!");

										prev = prev.Children[1]; // should contain a spawn node
										currentRoot.ChildRoots.Add(prev as RootNode);
										currentRoot = prev as RootNode; // spawn node become the new root node
										continue;
									}
								}
								else if (UberScriptFunctions.IsFunctionString(cleanedLine))
								{
									prev = new FunctionNode(current, cleanedLine, currentLineNumber);
									//Console.WriteLine(current + "->" + prev);
								}
								else if (UserDefinedFunctionNode.GetUserDefinedFunctionString(cleanedLine, root) != null)
								{
									prev = new UserDefinedFunctionExectueNode(current, cleanedLine, currentLineNumber);

									((UserDefinedFunctionExectueNode)prev).UserDefinedFunction =
										UserDefinedFunctionNode.GetUserDefinedFunctionString(cleanedLine, root);
								}
								else
								{
									// the string might have commas (indicating a spawn type with a constructor accepting arguments
									// such as "static,100"
									int commaIndex = cleanedLine.IndexOf(',');

									Type spawnType =
										ScriptCompiler.FindTypeByName(commaIndex > 0 ? cleanedLine.Substring(0, commaIndex) : cleanedLine);

									if (spawnType != null && typeof(Mobile).IsAssignableFrom(spawnType) || typeof(Item).IsAssignableFrom(spawnType))
									{
										prev = new RootNode(current, cleanedLine);
										// keep a list of child root nodes so that spawned creatures that have triggers
										// specific to them can correctly point to the right place in the script e.g.
										// troll
										// {
										//      onDeath 
										//      {
										//          orc
										//          {
										//              onDeath
										//              {
										//                  ...
										//              }
										//          }
										//      }
										// }
										// --> trigger nodes are only added to their closest root node
										currentRoot.ChildRoots.Add(prev as RootNode);
										//Console.WriteLine(current + "->" + prev);
										currentRoot = prev as RootNode; // spawn node become the new root node
									}
									else
									{
										throw new UberScriptException(
											String.Format("Parse: could not parse node at line {0}:\n{1}", currentLineNumber, cleanedLine));
									}
								}
							}
						}
						break;
				}

				current.Children.Add(prev);
				prev.LineNumber = currentLineNumber; // used for debugging purposes
			}

			if (balanceStack.Count > 0)
			{
				throw new UberScriptException(
					String.Format("Parse: brace mismatch detected in '{0}' at line {1}", fileName, currentLineNumber));
			}

			return root;
		}
	}

	[Serializable]
	public class UberScriptException : Exception
	{
		public UberScriptException(string message)
			: base(message)
		{ }

		public UberScriptException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		protected UberScriptException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}