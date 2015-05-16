#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using Server.Commands;
using Server.Gumps;
using Server.Network;
#endregion

namespace Server.Engines.XmlSpawner2
{
	public class ListIDMapping
	{
		public ArrayList List { get; set; }
		public object m_Obj = null;
		public object Obj { get { return m_Obj; } set { m_Obj = value; } }
		public List<int> m_IDs = new List<int>();
		public List<int> IDs { get { return m_IDs; } set { m_IDs = value; } }

		public ListIDMapping(ArrayList list, object obj)
		{
			if (list == null || obj == null)
			{
				throw new UberScriptException("ListIDMapping had null list or obj-this should not be possible!");
			}

			List = list;
			Obj = obj;
		}

		public void AddID(int newId)
		{
			if (!IDs.Contains(newId))
			{
				IDs.Add(newId);
			}
		}
	}

	public class UberScriptGump : Gump
	{
		public ArrayList CurrentList = null;
		public object CurrentListObject = null;
		// ListIDMap used to return the items from the list
		public List<ListIDMapping> ListIDMap = new List<ListIDMapping>();

		public void AddListIDMapping(ArrayList list, object obj, int id)
		{
			// one instance of each list
			bool foundList = false;
			bool foundObject = false;

			foreach (ListIDMapping mapping in ListIDMap.Where(mapping => mapping.List == list))
			{
				foundList = true;

				if (mapping.Obj != obj)
				{
					continue;
				}

				foundObject = true;
				mapping.AddID(id);
				break;
			}

			if (foundList && foundObject)
			{
				return;
			}

			ListIDMapping newMapping = new ListIDMapping(list, obj);

			newMapping.AddID(id);

			ListIDMap.Add(newMapping);
		}

		public ListIDMapping GetListIDMapping(int id)
		{
			return ListIDMap.FirstOrDefault(mapping => mapping.IDs.Any(mappedID => mappedID == id));
		}

		public List<int> UsedButtonIDs = new List<int>();
		public Dictionary<int, string> ButtonMap = new Dictionary<int, string>();
		public const int MaxRandomID = 60000;
		public TriggerObject TrigObject = null;

		public string GumpSource = "";

		public int GetRandomButtonID()
		{
			int roll = Utility.RandomMinMax(1, MaxRandomID);

			while (UsedButtonIDs.Contains(roll))
			{
				roll = Utility.RandomMinMax(1, MaxRandomID);
			}

			UsedButtonIDs.Add(roll);

			return roll;
		}

		public Mobile Target;

		public UberScriptGump(Mobile target, TriggerObject trigObject, UberGumpBase parsedGump, string gumpSource)
			: base(0, 0)
		{
			Target = target;
			TrigObject = trigObject;
			GumpSource = gumpSource;
			parsedGump.GenerateGump(trigObject, this, 0, 0);
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			try
			{
				if (TrigObject.Script == null)
				{
					return; // can't do anything with a script that doesn't exist
				}

				TrigObject = TrigObject.Dupe();
				// just in case there are pauses and stuff, don't want to screw up the trigger object
				TrigObject.TrigName = TriggerName.onGumpResponse;
				TrigObject.TrigMob = state.Mobile;
				TrigObject.GumpID = GumpSource;

				ListIDMapping matchingMap;
				ArrayList gumpMatchingListObjects = new ArrayList();

				// figure out all the "selected" things
				if (ButtonMap.ContainsKey(info.ButtonID))
				{
					//TrigObject.objs["gumpMatchingListObjects"] = new ArrayList();

					TrigObject.strings["buttonPressed"] = ButtonMap[info.ButtonID];
					TrigObject.ints[ButtonMap[info.ButtonID]] = 1;
					matchingMap = GetListIDMapping(info.ButtonID);

					if (matchingMap != null)
					{
						gumpMatchingListObjects.Add(matchingMap);
					}

					foreach (int switchID in info.Switches.Where(switchID => ButtonMap.ContainsKey(switchID)))
					{
						TrigObject.ints[ButtonMap[switchID]] = 1;
						matchingMap = GetListIDMapping(switchID);

						if (matchingMap != null)
						{
							gumpMatchingListObjects.Add(matchingMap);
						}
					}

					foreach (TextRelay textRelay in info.TextEntries.Where(textRelay => ButtonMap.ContainsKey(textRelay.EntryID)))
					{
						TrigObject.strings[ButtonMap[textRelay.EntryID]] = textRelay.Text;
						matchingMap = GetListIDMapping(textRelay.EntryID);

						if (matchingMap != null)
						{
							gumpMatchingListObjects.Add(matchingMap);
						}
					}
				}
				else if (info.ButtonID == 0) // they just closed the gump
				{
					foreach (int switchID in info.Switches.Where(switchID => ButtonMap.ContainsKey(switchID)))
					{
						TrigObject.ints[ButtonMap[switchID]] = 1;
						matchingMap = GetListIDMapping(switchID);

						if (matchingMap != null)
						{
							gumpMatchingListObjects.Add(matchingMap);
						}
					}

					foreach (TextRelay textRelay in info.TextEntries.Where(textRelay => ButtonMap.ContainsKey(textRelay.EntryID)))
					{
						TrigObject.strings[ButtonMap[textRelay.EntryID]] = textRelay.Text;
						matchingMap = GetListIDMapping(textRelay.EntryID);

						if (matchingMap != null)
						{
							gumpMatchingListObjects.Add(matchingMap);
						}
					}

					TrigObject.strings["buttonPressed"] = "gumpclosed";
					TrigObject.ints["gumpclosed"] = 1;
				}

				TrigObject.objs["gumpMatchingListObjects"] = gumpMatchingListObjects;
				TrigObject.Script.Execute(TrigObject, true);
			}
			catch (Exception e)
			{
				LoggingCustom.Log(
					"ERROR_UberScriptGumpResponse.txt", DateTime.Now + "\t" + e.Message + "\n" + e.StackTrace);
			}
		}

		public string Center(string text)
		{
			return String.Format("<CENTER>{0}</CENTER>", text);
		}

		public string Color(string text, int color)
		{
			return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
		}
	}

	public class UberGumpElement
	{
		public virtual int GetWidth(TriggerObject trigObject)
		{
			return 0;
		}

		public virtual int GetHeight(TriggerObject trigObject)
		{
			return 0;
		}

		protected MathTree m_Condition = null;

		public bool GetCondition(TriggerObject trigObject)
		{
			object output = m_Condition.Calculate(trigObject);

			if (!(output is bool))
			{
				throw new UberScriptException(
					GetType() + " GetCondition did not resolve to an bool: " + output + " from " + m_Condition.ScriptString);
			}

			return (bool)output;
		}

		protected MathTree m_X = new MathTree(null, "0");

		public int GetX(TriggerObject trigObject)
		{
			object output = m_X.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetX did not resolve to an integer: " + output + " from " + m_X.ScriptString);
			}

			return (int)output;
		}

		protected MathTree m_Y = new MathTree(null, "0");

		public int GetY(TriggerObject trigObject)
		{
			object output = m_Y.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetY did not resolve to an integer: " + output + " from " + m_Y.ScriptString);
			}

			return (int)output;
		}

		public List<UberGumpElement> Children = new List<UberGumpElement>();

		public UberGumpElement(XmlNode fromNode)
		{
			XmlNode LastAttributeSetAttempt = null;

			try
			{
				if (fromNode.Attributes != null)
				{
					foreach (XmlAttribute attribute in fromNode.Attributes)
					{
						LastAttributeSetAttempt = attribute;

						string lowerCaseAttribute = attribute.LocalName.ToLower();

						switch (lowerCaseAttribute)
						{
							case "x":
								m_X = new MathTree(null, attribute.Value);
								break;
							case "y":
								m_Y = new MathTree(null, attribute.Value);
								break;
							case "condition":
								m_Condition = new MathTree(null, attribute.Value);
								break;
						}
					}
				}

				LastAttributeSetAttempt = null;
			}
			catch (Exception e)
			{
				if (LastAttributeSetAttempt != null)
				{
					throw new UberScriptException(
						"UberGumpElement Error processing node: " + fromNode.LocalName + "... AttributeError on attribute " +
						LastAttributeSetAttempt.LocalName,
						e);
				}

				throw new UberScriptException("UberGumpElement Error processing node: " + fromNode.LocalName + "...", e);
			}
		}

		// returns the width and height of the element
		public virtual Rectangle2D GenerateGump(TriggerObject trigObject, UberScriptGump gump, int originX, int originY)
		{
			throw new NotImplementedException();
		}

		public virtual void ParseChildren(XmlNode fromNode)
		{
			XmlNode NodeBeingProcessed = null;

			try
			{
				foreach (XmlNode child in fromNode.ChildNodes.Cast<XmlNode>().Where(child => !(child is XmlComment)))
				{
					NodeBeingProcessed = child;

					switch (child.LocalName.ToLower())
					{
						case "box":
							AddChild(new UberGumpBox(child));
							break;
						case "hbox":
							AddChild(new UberGumpHBox(child));
							break;
						case "vbox":
							AddChild(new UberGumpVBox(child));
							break;
						case "label":
							AddChild(new UberGumpLabel(child));
							break;
						case "button":
							AddChild(new UberGumpButton(child));
							break;
						case "item":
							AddChild(new UberGumpItem(child));
							break;
						case "image":
							AddChild(new UberGumpImage(child));
							break;
						case "html":
							AddChild(new UberGumpHTML(child));
							break;
						case "textentry":
							AddChild(new UberGumpTextEntry(child));
							break;
						case "radio":
							AddChild(new UberGumpRadio(child));
							break;
						case "check":
							AddChild(new UberGumpCheck(child));
							break;
						case "list":
							AddChild(new UberGumpList(child));
							break;
						case "spacer":
							AddChild(new UberGumpSpacer(child));
							break;
						case "paperdoll":
							AddChild(new UberGumpPaperDoll(child));
							break;
						default:
							throw new UberScriptException("UberScriptGump node " + child.LocalName + " was not recognized!");
					}
				}
			}
			catch (Exception e)
			{
				if (NodeBeingProcessed != null)
				{
					throw new UberScriptException(
						"UberScriptGump Error processing node: " + NodeBeingProcessed.LocalName + "... ChildError:", e);
				}
			}
		}

		public virtual void AddChild(UberGumpElement child)
		{
			Children.Add(child);
		}
	}

	public class UberGumpBase : UberGumpElement
	{
		public bool Closable = true;
		public bool Disposable = true;
		public bool Dragable = true;
		public bool Resizable = false;

		public UberGumpBase(XmlNode fromNode)
			: base(fromNode)
		{
			// for debugging: the
			XmlAttribute LastAttributeSetAttempt = null;

			try
			{
				if (fromNode.Attributes != null)
				{
					foreach (XmlAttribute attribute in fromNode.Attributes)
					{
						LastAttributeSetAttempt = attribute;

						string lowerCaseAttribute = attribute.LocalName.ToLower();

						switch (lowerCaseAttribute)
						{
							case "closable":
								Closable = Convert.ToBoolean(attribute.Value);
								break;
							case "disposable":
								Disposable = Convert.ToBoolean(attribute.Value);
								break;
							case "dragable":
								Dragable = Convert.ToBoolean(attribute.Value);
								break;
							case "resizable":
								Resizable = Convert.ToBoolean(attribute.Value);
								break;
							default:
								{
									if (lowerCaseAttribute != "x" && lowerCaseAttribute != "y" && lowerCaseAttribute != "condition")
									{
										throw new UberScriptException(attribute.LocalName + " is not a valid attribute for this node!");
									}
								}
								break;
						}
					}
				}

				LastAttributeSetAttempt = null;
			}
			catch (Exception e)
			{
				if (LastAttributeSetAttempt != null)
				{
					throw new UberScriptException(
						"UberGumpBase Error processing node: " + fromNode.LocalName + "... AttributeError on attribute " +
						LastAttributeSetAttempt.LocalName,
						e);
				}

				throw new UberScriptException("UberGumpBase Error processing node: " + fromNode.LocalName + "...", e);
			}

			ParseChildren(fromNode);
		}

		public override Rectangle2D GenerateGump(TriggerObject trigObject, UberScriptGump gump, int originX, int originY)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return new Rectangle2D();
			}

			gump.X = GetX(trigObject);
			gump.Y = GetY(trigObject);
			gump.Closable = Closable;
			gump.Disposable = Disposable;
			gump.Dragable = Dragable;
			gump.Resizable = Resizable;

			gump.AddPage(0);

			foreach (UberGumpElement child in Children)
			{
				child.GenerateGump(trigObject, gump, gump.X, gump.Y);
			}

			return new Rectangle2D();
		}
	}

	public class UberGumpHTML : UberGumpElement
	{
		protected MathTree m_Width = new MathTree(null, "300");

		public override int GetWidth(TriggerObject trigObject)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return 0;
			}

			object output = m_Width.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetWidth did not resolve to an integer: " + output + " from " + m_Width.ScriptString);
			}

			return (int)output;
		}

		protected MathTree m_Height = new MathTree(null, "100");

		public override int GetHeight(TriggerObject trigObject)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return 0;
			}

			object output = m_Height.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetHeight did not resolve to an integer: " + output + " from " + m_Height.ScriptString);
			}

			return (int)output;
		}

		public bool ScrollBar = false;
		public bool Background = false;
		public string HTMLString = "";
		public MathTree HTMLMath = null;

		public UberGumpHTML(XmlNode fromNode)
			: base(fromNode)
		{
			// for debugging: the
			XmlAttribute LastAttributeSetAttempt = null;

			try
			{
				if (fromNode.Attributes != null)
				{
					foreach (XmlAttribute attribute in fromNode.Attributes)
					{
						LastAttributeSetAttempt = attribute;

						string lowerCaseAttribute = attribute.LocalName.ToLower();

						switch (lowerCaseAttribute)
						{
							case "width":
								m_Width = new MathTree(null, attribute.Value);
								break;
							case "height":
								m_Height = new MathTree(null, attribute.Value);
								break;
							case "scrollbar":
								ScrollBar = Boolean.Parse(attribute.Value);
								break;
							case "hasbackground":
								Background = Boolean.Parse(attribute.Value);
								break;
							default:
								{
									if (lowerCaseAttribute != "x" && lowerCaseAttribute != "y" && lowerCaseAttribute != "condition")
									{
										throw new UberScriptException(attribute.LocalName + " is not a valid attribute for this node!");
									}
								}
								break;
						}
					}
				}

				LastAttributeSetAttempt = null;
			}
			catch (Exception e)
			{
				if (LastAttributeSetAttempt != null)
				{
					throw new UberScriptException(
						"UberGumpBox Error processing node: " + fromNode.LocalName + "... AttributeError on attribute " +
						LastAttributeSetAttempt.LocalName,
						e);
				}

				throw new UberScriptException("UberGumpBox Error processing node: " + fromNode.LocalName + "...", e);
			}

			//we don't parse the children, but the
			try
			{
				if (fromNode.ChildNodes.Count > 0) // use the xml as html
				{
					HTMLString = "";

					foreach (XmlNode child in fromNode.ChildNodes)
					{
						HTMLString += child.OuterXml;
					}
				}
				else
				{
					HTMLString = fromNode.InnerText;
				}

				if (HTMLString.StartsWith("strings.") || HTMLString.Contains("xmlstrings.") ||
					HTMLString.StartsWith("global_strings."))
				{
					// presume it is supposed to be calculated from the string
					HTMLMath = new MathTree(null, HTMLString);
				}
			}
			catch (Exception e)
			{
				throw new UberScriptException("UberGumpLabel Error processing node: " + fromNode.LocalName + "...", e);
			}
		}

		public override Rectangle2D GenerateGump(TriggerObject trigObject, UberScriptGump gump, int originX, int originY)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return new Rectangle2D();
			}

			int x = originX + GetX(trigObject);
			int y = originY + GetY(trigObject);
			int width = GetWidth(trigObject);
			int height = GetHeight(trigObject);

			if (HTMLMath != null)
			{
				object toSend = HTMLMath.Calculate(trigObject);

				gump.AddHtml(x, y, width, height, toSend == null ? HTMLString : toSend.ToString(), Background, ScrollBar);
			}
			else
			{
				gump.AddHtml(x, y, width, height, HTMLString, Background, ScrollBar);
			}

			return new Rectangle2D(GetX(trigObject), GetY(trigObject), GetWidth(trigObject), GetHeight(trigObject));
		}
	}

	public class UberGumpSpacer : UberGumpElement
	{
		protected MathTree m_Width = new MathTree(null, "10"); // 44 is default tile size

		public override int GetWidth(TriggerObject trigObject)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return 0;
			}

			object output = m_Width.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetWidth did not resolve to an integer: " + output + " from " + m_Width.ScriptString);
			}

			return (int)output;
		}

		protected MathTree m_Height = new MathTree(null, "10"); // 44 is default tile size

		public override int GetHeight(TriggerObject trigObject)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return 0;
			}

			object output = m_Height.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetHeight did not resolve to an integer: " + output + " from " + m_Height.ScriptString);
			}

			return (int)output;
		}

		public UberGumpSpacer(XmlNode fromNode)
			: base(fromNode)
		{
			// for debugging: the
			XmlAttribute LastAttributeSetAttempt = null;

			try
			{
				if (fromNode.Attributes != null)
				{
					foreach (XmlAttribute attribute in fromNode.Attributes)
					{
						LastAttributeSetAttempt = attribute;
						string lowerCaseAttribute = attribute.LocalName.ToLower();
						if (lowerCaseAttribute == "width")
						{
							m_Width = new MathTree(null, attribute.Value);
						}
						else if (lowerCaseAttribute == "height")
						{
							m_Height = new MathTree(null, attribute.Value);
						}
						else if (lowerCaseAttribute != "x" && lowerCaseAttribute != "y" && lowerCaseAttribute != "condition")
						{
							throw new UberScriptException(attribute.LocalName + " is not a valid attribute for this node!");
						}
					}
				}

				LastAttributeSetAttempt = null;
			}
			catch (Exception e)
			{
				if (LastAttributeSetAttempt != null)
				{
					throw new UberScriptException(
						"UberGumpSpacer Error processing node: " + fromNode.LocalName + "... AttributeError on attribute " +
						LastAttributeSetAttempt.LocalName,
						e);
				}

				throw new UberScriptException("UberGumpSpacer Error processing node: " + fromNode.LocalName + "...", e);
			}
		}

		public override void AddChild(UberGumpElement child)
		{
			throw new NotImplementedException();
		}

		public override Rectangle2D GenerateGump(TriggerObject trigObject, UberScriptGump gump, int originX, int originY)
		{
			return m_Condition != null && GetCondition(trigObject) == false
					   ? new Rectangle2D()
					   : new Rectangle2D(GetX(trigObject), GetY(trigObject), GetWidth(trigObject), GetHeight(trigObject));
		}
	}

	public class UberGumpLabel : UberGumpElement
	{
		protected MathTree m_Hue = new MathTree(null, "0");

		public int GetHue(TriggerObject trigObject)
		{
			object output = m_Hue.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetHue did not resolve to an integer: " + output + " from " + m_Hue.ScriptString);
			}

			return (int)output;
		}

		protected MathTree m_Label = new MathTree(null, "null");

		public string GetLabel(TriggerObject trigObject)
		{
			object output = m_Label.Calculate(trigObject);

			if (output == null)
			{
				return "null";
			}

			return output.ToString();
		}

		public override int GetWidth(TriggerObject trigObject)
		{
			return m_Condition != null && GetCondition(trigObject) == false ? 0 : GetWidthFromString(GetLabel(trigObject));
		}

		public override int GetHeight(TriggerObject trigObject)
		{
			return m_Condition != null && GetCondition(trigObject) == false ? 0 : 20;
		}

		public int GetWidthFromString(string value)
		{
			return value.Length * 8;
		}

		public UberGumpLabel(XmlNode fromNode)
			: base(fromNode)
		{
			// for debugging: the
			XmlAttribute LastAttributeSetAttempt = null;

			try
			{
				if (fromNode.Attributes != null)
				{
					foreach (XmlAttribute attribute in fromNode.Attributes)
					{
						LastAttributeSetAttempt = attribute;

						string lowerCaseAttribute = attribute.LocalName.ToLower();

						switch (lowerCaseAttribute)
						{
							case "hue":
								m_Hue = new MathTree(null, attribute.Value);
								break;
							case "font":
								break;
							default:
								{
									if (lowerCaseAttribute != "x" && lowerCaseAttribute != "y" && lowerCaseAttribute != "condition")
									{
										throw new UberScriptException(attribute.LocalName + " is not a valid attribute for this node!");
									}
								}
								break;
						}
					}
				}

				LastAttributeSetAttempt = null;
			}
			catch (Exception e)
			{
				if (LastAttributeSetAttempt != null)
				{
					throw new UberScriptException(
						"UberGumpBase Error processing node: " + fromNode.LocalName + "... AttributeError on attribute " +
						LastAttributeSetAttempt.LocalName,
						e);
				}

				throw new UberScriptException("UberGumpBase Error processing node: " + fromNode.LocalName + "...", e);
			}

			try
			{
				m_Label = new MathTree(null, fromNode.InnerText);
			}
			catch (Exception e)
			{
				throw new UberScriptException("UberGumpLabel Error processing node: " + fromNode.LocalName + "...", e);
			}
			// don't process any children (Label can't have any)
			//ParseChildren(fromNode);
		}

		public override void AddChild(UberGumpElement child)
		{
			throw new NotImplementedException();
		}

		public override Rectangle2D GenerateGump(TriggerObject trigObject, UberScriptGump gump, int originX, int originY)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return new Rectangle2D();
			}

			gump.AddLabel(originX + GetX(trigObject), originY + GetY(trigObject), GetHue(trigObject), GetLabel(trigObject));

			return new Rectangle2D(GetX(trigObject), GetY(trigObject), GetWidth(trigObject), GetHeight(trigObject));
		}
	}

	public class UberGumpButton : UberGumpElement
	{
		//public void AddButton( int x, int y, int normalID, int pressedID, int buttonID, GumpButtonType type, int param )
		protected MathTree m_NormalID = new MathTree(null, "2074");

		public int GetNormalID(TriggerObject trigObject)
		{
			object output = m_NormalID.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetNormalID did not resolve to an integer: " + output + " from " + m_NormalID.ScriptString);
			}

			return (int)output;
		}

		protected MathTree m_PressedID = new MathTree(null, "2075");

		public int GetPressedID(TriggerObject trigObject)
		{
			object output = m_PressedID.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetPressedID did not resolve to an integer: " + output + " from " + m_PressedID.ScriptString);
			}

			return (int)output;
		}

		// basically Name is used to pass the result back to uberscript
		// by using the TriggerObject ints dictionary and the Name
		// e.g. if Name = "hello" for this button and the user pressed it, then
		// ints.hello would be 1 if it was pressed (so in uberscript you could use
		//     if (ints.hello == 1) { // do something
		protected MathTree m_Name = new MathTree(null, "buttonResponse");

		public string GetName(TriggerObject trigObject)
		{
			object output = m_Name.Calculate(trigObject);

			if (output == null)
			{
				return "null";
			}

			return output.ToString();
		}

		public override int GetWidth(TriggerObject trigObject)
		{
			return m_Condition != null && GetCondition(trigObject) == false
					   ? 0
					   : GumpIndexData.GetGumpData(GetNormalID(trigObject)).Width;
		}

		public override int GetHeight(TriggerObject trigObject)
		{
			return m_Condition != null && GetCondition(trigObject) == false
					   ? 0
					   : GumpIndexData.GetGumpData(GetNormalID(trigObject)).Height;
		}

		//public void AddButton( int x, int y, int normalID, int pressedID, int buttonID, GumpButtonType type, int param )
		public UberGumpButton(XmlNode fromNode)
			: base(fromNode)
		{
			// for debugging: the
			XmlAttribute LastAttributeSetAttempt = null;

			try
			{
				if (fromNode.Attributes != null)
				{
					foreach (XmlAttribute attribute in fromNode.Attributes)
					{
						LastAttributeSetAttempt = attribute;

						string lowerCaseAttribute = attribute.LocalName.ToLower();

						switch (lowerCaseAttribute)
						{
							case "normalid":
								m_NormalID = new MathTree(null, attribute.Value);
								break;
							case "pressedid":
								m_PressedID = new MathTree(null, attribute.Value);
								break;
							case "name":
								m_Name = new MathTree(null, attribute.Value);
								break;
							default:
								{
									if (lowerCaseAttribute != "x" && lowerCaseAttribute != "y" && lowerCaseAttribute != "condition")
									{
										throw new UberScriptException(attribute.LocalName + " is not a valid attribute for this node!");
									}
								}
								break;
						}
					}
				}

				LastAttributeSetAttempt = null;
			}
			catch (Exception e)
			{
				if (LastAttributeSetAttempt != null)
				{
					throw new UberScriptException(
						"UberGumpBase Error processing node: " + fromNode.LocalName + "... AttributeError on attribute " +
						LastAttributeSetAttempt.LocalName,
						e);
				}

				throw new UberScriptException("UberGumpBase Error processing node: " + fromNode.LocalName + "...", e);
			}

			// don't process any children (Button can't have any)
			//ParseChildren(fromNode);
		}

		public override void AddChild(UberGumpElement child)
		{
			throw new NotImplementedException();
		}

		public override Rectangle2D GenerateGump(TriggerObject trigObject, UberScriptGump gump, int originX, int originY)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return new Rectangle2D();
			}

			int randomButtonID = gump.GetRandomButtonID();

			gump.ButtonMap.Add(randomButtonID, GetName(trigObject));
			gump.AddButton(
				originX + GetX(trigObject),
				originY + GetY(trigObject),
				GetNormalID(trigObject),
				GetPressedID(trigObject),
				randomButtonID,
				GumpButtonType.Reply,
				0);

			if (gump.CurrentList != null && gump.CurrentListObject != null)
			{
				gump.AddListIDMapping(gump.CurrentList, gump.CurrentListObject, randomButtonID);
			}

			return new Rectangle2D(GetX(trigObject), GetY(trigObject), GetWidth(trigObject), GetHeight(trigObject));
		}
	}

	public class UberGumpCheck : UberGumpElement
	{
		//public void AddButton( int x, int y, int normalID, int pressedID, int buttonID, GumpButtonType type, int param )
		protected MathTree m_InactiveID = new MathTree(null, "210");

		public int GetInactiveID(TriggerObject trigObject)
		{
			object output = m_InactiveID.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetInactiveID did not resolve to an integer: " + output + " from " + m_InactiveID.ScriptString);
			}

			return (int)output;
		}

		protected MathTree m_ActiveID = new MathTree(null, "211");

		public int GetActiveID(TriggerObject trigObject)
		{
			object output = m_ActiveID.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetActiveID did not resolve to an integer: " + output + " from " + m_ActiveID.ScriptString);
			}

			return (int)output;
		}

		protected MathTree m_Default = new MathTree(null, "false");

		public bool GetDefault(TriggerObject trigObject)
		{
			object output = m_Default.Calculate(trigObject);

			if (!(output is bool))
			{
				throw new UberScriptException(
					GetType() + " GetDefault did not resolve to an boolean: " + output + " from " + m_Default.ScriptString);
			}

			return (bool)output;
		}

		// basically Name is used to pass the result back to uberscript
		// by using the TriggerObject ints dictionary and the Name
		// e.g. if Name = "hello" for this button and the user Active it, then
		// ints.hello would be 1 if it was Active (so in uberscript you could use
		//     if (ints.hello == 1) { // do something
		protected MathTree m_Name = new MathTree(null, "checkResponse");

		public string GetName(TriggerObject trigObject)
		{
			object output = m_Name.Calculate(trigObject);

			if (output == null)
			{
				return "null";
			}

			return output.ToString();
		}

		public override int GetWidth(TriggerObject trigObject)
		{
			return m_Condition != null && GetCondition(trigObject) == false
					   ? 0
					   : GumpIndexData.GetGumpData(GetInactiveID(trigObject)).Width;
		}

		public override int GetHeight(TriggerObject trigObject)
		{
			return m_Condition != null && GetCondition(trigObject) == false
					   ? 0
					   : GumpIndexData.GetGumpData(GetInactiveID(trigObject)).Height;
		}

		//public void AddButton( int x, int y, int InactiveID, int ActiveID, int buttonID, GumpButtonType type, int param )
		public UberGumpCheck(XmlNode fromNode)
			: base(fromNode)
		{
			// for debugging: the
			XmlAttribute LastAttributeSetAttempt = null;
			try
			{
				if (fromNode.Attributes != null)
				{
					foreach (XmlAttribute attribute in fromNode.Attributes)
					{
						LastAttributeSetAttempt = attribute;

						string lowerCaseAttribute = attribute.LocalName.ToLower();

						switch (lowerCaseAttribute)
						{
							case "inactiveid":
								m_InactiveID = new MathTree(null, attribute.Value);
								break;
							case "activeid":
								m_ActiveID = new MathTree(null, attribute.Value);
								break;
							case "name":
								m_Name = new MathTree(null, attribute.Value);
								break;
							case "default":
								m_Default = new MathTree(null, attribute.Value);
								break;
							default:
								{
									if (lowerCaseAttribute != "x" && lowerCaseAttribute != "y" && lowerCaseAttribute != "condition")
									{
										throw new UberScriptException(attribute.LocalName + " is not a valid attribute for this node!");
									}
								}
								break;
						}
					}
				}

				LastAttributeSetAttempt = null;
			}
			catch (Exception e)
			{
				if (LastAttributeSetAttempt != null)
				{
					throw new UberScriptException(
						"UberGumpBase Error processing node: " + fromNode.LocalName + "... AttributeError on attribute " +
						LastAttributeSetAttempt.LocalName,
						e);
				}

				throw new UberScriptException("UberGumpBase Error processing node: " + fromNode.LocalName + "...", e);
			}

			// don't process any children (Button can't have any)
			//ParseChildren(fromNode);
		}

		public override void AddChild(UberGumpElement child)
		{
			throw new NotImplementedException();
		}

		public override Rectangle2D GenerateGump(TriggerObject trigObject, UberScriptGump gump, int originX, int originY)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return new Rectangle2D();
			}

			int randomButtonID = gump.GetRandomButtonID();

			gump.ButtonMap.Add(randomButtonID, GetName(trigObject));
			gump.AddCheck(
				originX + GetX(trigObject),
				originY + GetY(trigObject),
				GetInactiveID(trigObject),
				GetActiveID(trigObject),
				GetDefault(trigObject),
				randomButtonID);

			if (gump.CurrentList != null && gump.CurrentListObject != null)
			{
				gump.AddListIDMapping(gump.CurrentList, gump.CurrentListObject, randomButtonID);
			}

			return new Rectangle2D(GetX(trigObject), GetY(trigObject), GetWidth(trigObject), GetHeight(trigObject));
		}
	}

	public class UberGumpRadio : UberGumpElement
	{
		protected MathTree m_InactiveID = new MathTree(null, "208"); // 10850

		public int GetInactiveID(TriggerObject trigObject)
		{
			object output = m_InactiveID.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetInactiveID did not resolve to an integer: " + output + " from " + m_InactiveID.ScriptString);
			}

			return (int)output;
		}

		protected MathTree m_ActiveID = new MathTree(null, "209"); // 10830

		public int GetActiveID(TriggerObject trigObject)
		{
			object output = m_ActiveID.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetActiveID did not resolve to an integer: " + output + " from " + m_ActiveID.ScriptString);
			}

			return (int)output;
		}

		protected MathTree m_Default = new MathTree(null, "false");

		public bool GetDefault(TriggerObject trigObject)
		{
			object output = m_Default.Calculate(trigObject);

			if (!(output is bool))
			{
				throw new UberScriptException(
					GetType() + " GetDefault did not resolve to an boolean: " + output + " from " + m_Default.ScriptString);
			}

			return (bool)output;
		}

		// basically Name is used to pass the result back to uberscript
		// by using the TriggerObject ints dictionary and the Name
		// e.g. if Name = "hello" for this button and the user Active it, then
		// ints.hello would be 1 if it was Active (so in uberscript you could use
		//     if (ints.hello == 1) { // do something
		protected MathTree m_Name = new MathTree(null, "checkResponse");

		public string GetName(TriggerObject trigObject)
		{
			object output = m_Name.Calculate(trigObject);

			if (output == null)
			{
				return "null";
			}

			return output.ToString();
		}

		public override int GetWidth(TriggerObject trigObject)
		{
			return m_Condition != null && GetCondition(trigObject) == false
					   ? 0
					   : GumpIndexData.GetGumpData(GetInactiveID(trigObject)).Width;
		}

		public override int GetHeight(TriggerObject trigObject)
		{
			return m_Condition != null && GetCondition(trigObject) == false
					   ? 0
					   : GumpIndexData.GetGumpData(GetInactiveID(trigObject)).Height;
		}

		//public void AddButton( int x, int y, int InactiveID, int ActiveID, int buttonID, GumpButtonType type, int param )
		public UberGumpRadio(XmlNode fromNode)
			: base(fromNode)
		{
			// for debugging: the
			XmlAttribute LastAttributeSetAttempt = null;

			try
			{
				if (fromNode.Attributes != null)
				{
					foreach (XmlAttribute attribute in fromNode.Attributes)
					{
						LastAttributeSetAttempt = attribute;

						string lowerCaseAttribute = attribute.LocalName.ToLower();

						switch (lowerCaseAttribute)
						{
							case "inactiveid":
								m_InactiveID = new MathTree(null, attribute.Value);
								break;
							case "activeid":
								m_ActiveID = new MathTree(null, attribute.Value);
								break;
							case "name":
								m_Name = new MathTree(null, attribute.Value);
								break;
							case "default":
								m_Default = new MathTree(null, attribute.Value);
								break;
							default:
								{
									if (lowerCaseAttribute != "x" && lowerCaseAttribute != "y" && lowerCaseAttribute != "condition")
									{
										throw new UberScriptException(attribute.LocalName + " is not a valid attribute for this node!");
									}
								}
								break;
						}
					}
				}

				LastAttributeSetAttempt = null;
			}
			catch (Exception e)
			{
				if (LastAttributeSetAttempt != null)
				{
					throw new UberScriptException(
						"UberGumpBase Error processing node: " + fromNode.LocalName + "... AttributeError on attribute " +
						LastAttributeSetAttempt.LocalName,
						e);
				}

				throw new UberScriptException("UberGumpBase Error processing node: " + fromNode.LocalName + "...", e);
			}

			// don't process any children (Button can't have any)
			//ParseChildren(fromNode);
		}

		public override void AddChild(UberGumpElement child)
		{
			throw new NotImplementedException();
		}

		public override Rectangle2D GenerateGump(TriggerObject trigObject, UberScriptGump gump, int originX, int originY)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return new Rectangle2D();
			}

			int randomButtonID = gump.GetRandomButtonID();

			gump.ButtonMap.Add(randomButtonID, GetName(trigObject));
			gump.AddRadio(
				originX + GetX(trigObject),
				originY + GetY(trigObject),
				GetInactiveID(trigObject),
				GetActiveID(trigObject),
				GetDefault(trigObject),
				randomButtonID);

			if (gump.CurrentList != null && gump.CurrentListObject != null)
			{
				gump.AddListIDMapping(gump.CurrentList, gump.CurrentListObject, randomButtonID);
			}

			return new Rectangle2D(GetX(trigObject), GetY(trigObject), GetWidth(trigObject), GetHeight(trigObject));
		}
	}

	public class UberGumpTextEntry : UberGumpElement
	{
		protected MathTree m_Hue = new MathTree(null, "0");

		public int GetHue(TriggerObject trigObject)
		{
			object output = m_Hue.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetHue did not resolve to an integer: " + output + " from " + m_Hue.ScriptString);
			}

			return (int)output;
		}

		protected MathTree m_Width = new MathTree(null, "44"); // 44 is default tile size

		public override int GetWidth(TriggerObject trigObject)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return 0;
			}

			object output = m_Width.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetWidth did not resolve to an integer: " + output + " from " + m_Width.ScriptString);
			}

			return (int)output;
		}

		protected MathTree m_Height = new MathTree(null, "44"); // 44 is default tile size

		public override int GetHeight(TriggerObject trigObject)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return 0;
			}

			object output = m_Height.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetHeight did not resolve to an integer: " + output + " from " + m_Height.ScriptString);
			}

			return (int)output;
		}

		protected MathTree m_Size = new MathTree(null, "-1"); // if > 

		public int GetSize(TriggerObject trigObject)
		{
			object output = m_Size.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " m_Size did not resolve to an integer: " + output + " from " + m_Height.ScriptString);
			}

			return (int)output;
		}

		// basically Name is used to pass the result back to uberscript
		// by using the TriggerObject ints dictionary and the Name
		// e.g. if Name = "hello" for this button and the user pressed it, then
		// ints.hello would be 1 if it was pressed (so in uberscript you could use
		//     if (ints.hello == 1) { // do something
		protected MathTree m_Name = new MathTree(null, "textEntryResponse");

		public string GetName(TriggerObject trigObject)
		{
			object output = m_Name.Calculate(trigObject);

			if (output == null)
			{
				return "null";
			}

			return output.ToString();
		}

		protected MathTree m_Default = new MathTree(null, "");

		public string GetDefault(TriggerObject trigObject)
		{
			object output = m_Default.Calculate(trigObject);

			if (output == null)
			{
				return "null";
			}

			return output.ToString();
		}

		public UberGumpTextEntry(XmlNode fromNode)
			: base(fromNode)
		{
			// for debugging: the
			XmlAttribute LastAttributeSetAttempt = null;

			try
			{
				if (fromNode.Attributes != null)
				{
					foreach (XmlAttribute attribute in fromNode.Attributes)
					{
						LastAttributeSetAttempt = attribute;
						string lowerCaseAttribute = attribute.LocalName.ToLower();
						switch (lowerCaseAttribute)
						{
							case "hue":
								m_Hue = new MathTree(null, attribute.Value);
								break;
							case "width":
								m_Width = new MathTree(null, attribute.Value);
								break;
							case "height":
								m_Height = new MathTree(null, attribute.Value);
								break;
							case "name":
								m_Name = new MathTree(null, attribute.Value);
								break;
							case "default":
								m_Default = new MathTree(null, attribute.Value);
								break;
							case "size":
								m_Size = new MathTree(null, attribute.Value);
								break;
							default:
								{
									if (lowerCaseAttribute != "x" && lowerCaseAttribute != "y" && lowerCaseAttribute != "condition")
									{
										throw new UberScriptException(attribute.LocalName + " is not a valid attribute for this node!");
									}
								}
								break;
						}
					}
				}

				LastAttributeSetAttempt = null;
			}
			catch (Exception e)
			{
				if (LastAttributeSetAttempt != null)
				{
					throw new UberScriptException(
						"UberGumpBase Error processing node: " + fromNode.LocalName + "... AttributeError on attribute " +
						LastAttributeSetAttempt.LocalName,
						e);
				}

				throw new UberScriptException("UberGumpBase Error processing node: " + fromNode.LocalName + "...", e);
			}

			// don't process any children (Button can't have any)
			//ParseChildren(fromNode);
		}

		public override void AddChild(UberGumpElement child)
		{
			throw new NotImplementedException();
		}

		public override Rectangle2D GenerateGump(TriggerObject trigObject, UberScriptGump gump, int originX, int originY)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return new Rectangle2D();
			}

			int randomButtonID = gump.GetRandomButtonID();
			int size = GetSize(trigObject);

			gump.ButtonMap.Add(randomButtonID, GetName(trigObject));

			if (size > -1)
			{
				gump.AddTextEntry(
					originX + GetX(trigObject),
					originY + GetY(trigObject),
					GetWidth(trigObject),
					GetHeight(trigObject),
					GetHue(trigObject),
					randomButtonID,
					GetDefault(trigObject),
					size);
			}
			else
			{
				gump.AddTextEntry(
					originX + GetX(trigObject),
					originY + GetY(trigObject),
					GetWidth(trigObject),
					GetHeight(trigObject),
					GetHue(trigObject),
					randomButtonID,
					GetDefault(trigObject));
			}

			if (gump.CurrentList != null && gump.CurrentListObject != null)
			{
				gump.AddListIDMapping(gump.CurrentList, gump.CurrentListObject, randomButtonID);
			}

			return new Rectangle2D(GetX(trigObject), GetY(trigObject), GetWidth(trigObject), GetHeight(trigObject));
		}
	}

	public class UberGumpImage : UberGumpElement
	{
		protected MathTree m_Hue = new MathTree(null, "0");

		public int GetHue(TriggerObject trigObject)
		{
			object output = m_Hue.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetHue did not resolve to an integer: " + output + " from " + m_Hue.ScriptString);
			}

			return (int)output;
		}

		protected MathTree m_GumpID = new MathTree(null, "0");

		public int GetGumpID(TriggerObject trigObject)
		{
			object output = m_GumpID.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetGumpID did not resolve to an integer: " + output + " from " + m_GumpID.ScriptString);
			}

			return (int)output;
		}

		public override int GetWidth(TriggerObject trigObject)
		{
			return m_Condition != null && GetCondition(trigObject) == false
					   ? 0
					   : GumpIndexData.GetGumpData(GetGumpID(trigObject)).Width;
		}

		public override int GetHeight(TriggerObject trigObject)
		{
			return m_Condition != null && GetCondition(trigObject) == false
					   ? 0
					   : GumpIndexData.GetGumpData(GetGumpID(trigObject)).Height;
		}

		public UberGumpImage(XmlNode fromNode)
			: base(fromNode)
		{
			// for debugging: the
			XmlAttribute LastAttributeSetAttempt = null;

			try
			{
				if (fromNode.Attributes != null)
				{
					foreach (XmlAttribute attribute in fromNode.Attributes)
					{
						LastAttributeSetAttempt = attribute;

						string lowerCaseAttribute = attribute.LocalName.ToLower();

						switch (lowerCaseAttribute)
						{
							case "hue":
								m_Hue = new MathTree(null, attribute.Value);
								break;
							case "gumpid":
								m_GumpID = new MathTree(null, attribute.Value);
								break;
							default:
								{
									if (lowerCaseAttribute != "x" && lowerCaseAttribute != "y" && lowerCaseAttribute != "condition")
									{
										throw new UberScriptException(attribute.LocalName + " is not a valid attribute for this node!");
									}
								}
								break;
						}
					}
				}

				LastAttributeSetAttempt = null;
			}
			catch (Exception e)
			{
				if (LastAttributeSetAttempt != null)
				{
					throw new UberScriptException(
						"UberGumpBase Error processing node: " + fromNode.LocalName + "... AttributeError on attribute " +
						LastAttributeSetAttempt.LocalName,
						e);
				}

				throw new UberScriptException("UberGumpBase Error processing node: " + fromNode.LocalName + "...", e);
			}

			// don't process any children (Label can't have any)
			//ParseChildren(fromNode);
		}

		public override void AddChild(UberGumpElement child)
		{
			throw new NotImplementedException();
		}

		public override Rectangle2D GenerateGump(TriggerObject trigObject, UberScriptGump gump, int originX, int originY)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return new Rectangle2D();
			}

			gump.AddImage(originX + GetX(trigObject), originY + GetY(trigObject), GetGumpID(trigObject), GetHue(trigObject));

			return new Rectangle2D(GetX(trigObject), GetY(trigObject), GetWidth(trigObject), GetHeight(trigObject));
		}
	}

	public class UberGumpPaperDoll : UberGumpElement
	{
		protected MathTree m_Mobile = new MathTree(null, "null");

		public Mobile GetMobile(TriggerObject trigObject)
		{
			object output = m_Mobile.Calculate(trigObject);

			if (!(output is Mobile))
			{
				throw new UberScriptException(
					GetType() + " GetMobile did not resolve to a Mobile: " + output + " from " + m_Mobile.ScriptString);
			}

			return (Mobile)output;
		}

		public override int GetWidth(TriggerObject trigObject)
		{
			return m_Condition != null && GetCondition(trigObject) == false ? 0 : 260;
		}

		// hard coded gump art sizes
		public override int GetHeight(TriggerObject trigObject)
		{
			return m_Condition != null && GetCondition(trigObject) == false ? 0 : 237;
		}

		// hard coded gump art sizes

		public UberGumpPaperDoll(XmlNode fromNode)
			: base(fromNode)
		{
			// for debugging: the
			XmlAttribute LastAttributeSetAttempt = null;

			try
			{
				if (fromNode.Attributes != null)
				{
					foreach (XmlAttribute attribute in fromNode.Attributes)
					{
						LastAttributeSetAttempt = attribute;
						string lowerCaseAttribute = attribute.LocalName.ToLower();
						if (lowerCaseAttribute == "mob")
						{
							m_Mobile = new MathTree(null, attribute.Value);
						}
						else if (lowerCaseAttribute != "x" && lowerCaseAttribute != "y" && lowerCaseAttribute != "condition")
						{
							throw new UberScriptException(attribute.LocalName + " is not a valid attribute for this node!");
						}
					}
				}

				LastAttributeSetAttempt = null;
			}
			catch (Exception e)
			{
				if (LastAttributeSetAttempt != null)
				{
					throw new UberScriptException(
						"UberGumpPaperDoll Error processing node: " + fromNode.LocalName + "... AttributeError on attribute " +
						LastAttributeSetAttempt.LocalName,
						e);
				}

				throw new UberScriptException("UberGumpPaperDoll Error processing node: " + fromNode.LocalName + "...", e);
			}

			// don't process any children (Label can't have any)
			//ParseChildren(fromNode);
		}

		public override void AddChild(UberGumpElement child)
		{
			throw new NotImplementedException();
		}

		public static int[] LayerMapping = new /*int*/[/*0x18*/]
		{
			0, //    Invalid		 = 0x00,
			19, // FirstValid   = 0x01, OneHanded	 = 0x01,
			18, // TwoHanded	 = 0x02,
			3, // Shoes		 = 0x03,
			2, // Pants		 = 0x04,
			5, // Shirt		 = 0x05,
			15, // Helm		 = 0x06,
			10, // Gloves		 = 0x07,
			9, // Ring		 = 0x08,
			20, // Talisman	 = 0x09,
			12, // Neck		 = 0x0A,
			13, // Hair		 = 0x0B,
			17, // Waist		 = 0x0C,
			6, // InnerTorso	 = 0x0D,
			9, // Bracelet	 = 0x0E,
			0, // Unused_xF	 = 0x0F,
			14, // FacialHair	 = 0x10,
			8, // MiddleTorso	 = 0x11,
			15, // Earrings	 = 0x12,
			7, // Arms		 = 0x13,
			0, // Cloak		 = 0x14,
			11, // OuterTorso	 = 0x16,
			10, // OuterLegs	 = 0x17,
			2 // InnerLegs	 = 0x18,
		};

		public const int NumLayers = 21;

		public override Rectangle2D GenerateGump(TriggerObject trigObject, UberScriptGump gump, int originX, int originY)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return new Rectangle2D();
			}

			Mobile mob = GetMobile(trigObject);

			if (mob == null)
			{
				return new Rectangle2D(GetX(trigObject), GetY(trigObject), GetWidth(trigObject), GetHeight(trigObject));
			}

			var layers = new Item[NumLayers];

			foreach (Item item in mob.Items)
			{
				byte byteLayer = (byte)item.Layer;

				if (byteLayer < 0x19)
				{
					layers[LayerMapping[byteLayer]] = item;
				}
			}

			if (layers[2] != null && (layers[2].ItemID == 0x1411 || layers[2].ItemID == 0x141a)) // plate legs go over shoes
			{
				layers[3] = null; // remove shoe layer
			}

			int x = originX + GetX(trigObject);
			int y = originY + GetY(trigObject);
			int mobHue = mob.Hue & 0x0FFF;
			// somehow 0x8000 bit is added to indicate it is a skin hue... but this doesn't work as a gumpart hue... weird :P

			if (mobHue > 0)
			{
				mobHue = mobHue - 1;
				// for some reason gump hues skip hue = 1 (pitch black), so anything above 0 needs to be offset by one
			}

			if (mob.BodyValue == 400) // human male
			{
				gump.AddImage(x, y, 0xC, mobHue);
			}
			else if (mob.BodyValue == 401) // human male
			{
				gump.AddImage(x, y, 0xD, mobHue);
			}
			else if (mob.BodyValue == 605) // elf male
			{
				gump.AddImage(x, y, 0xE, mobHue);
			}
			else if (mob.BodyValue == 606) // elf female
			{
				gump.AddImage(x, y, 0xF, mobHue);
			}
			else if (mob.BodyValue == 183 || mob.BodyValue == 185) // savage male?
			{
				gump.AddImage(x, y, 0x79, mobHue);
			}
			else if (mob.BodyValue == 184 || mob.BodyValue == 186) // savage female?
			{
				gump.AddImage(x, y, 0x78, mobHue);
			}
			else if (mob.BodyValue == 666) // gargoyle male
			{
				gump.AddImage(x, y, 0x29A, mobHue);
			}
			else if (mob.BodyValue == 667) // gargoyle female
			{
				gump.AddImage(x, y, 0x299, mobHue);
			}
			else if (mob.BodyValue == 990) // lord british
			{
				gump.AddImage(x, y, 0x3DE, mobHue);
			}
			else if (mob.BodyValue == 991) // lord blackthorn
			{
				gump.AddImage(x, y, 0x3DF, mobHue);
			}
			else if (mob.BodyValue == 994) // dupre
			{
				gump.AddImage(x, y, 0xC732, mobHue);
			}
			else
			{
				return new Rectangle2D(GetX(trigObject), GetY(trigObject), GetWidth(trigObject), GetHeight(trigObject));
				// can't make a paperdoll
			}

			for (int i = 0; i < NumLayers; i++)
			{
				int mappedGumpID = 0;

				if (i == 13) // hair layer
				{
					PaperDollLookup.ItemIDtoGumpID.TryGetValue(mob.HairItemID, out mappedGumpID);

					if (mappedGumpID != 0)
					{
						if (mob.Female && HasFemaleOffset(mappedGumpID))
						{
							mappedGumpID += 10000; // female offset
						}

						gump.AddImage(x, y, mappedGumpID, mob.HairHue == 0 ? 0 : mob.HairHue - 1);
						// weird gump art hue offset (skips jet black)
					}

					PaperDollLookup.ItemIDtoGumpID.TryGetValue(mob.FacialHairItemID, out mappedGumpID);

					if (mappedGumpID != 0)
					{
						if (mob.Female && HasFemaleOffset(mappedGumpID))
						{
							mappedGumpID += 10000; // female offset
						}

						gump.AddImage(x, y, mappedGumpID, mob.FacialHairHue == 0 ? 0 : mob.FacialHairHue - 1);
						// weird gump art hue offset (skips jet black)
					}
				}
				else
				{
					if (layers[i] == null)
					{
						continue;
					}

					PaperDollLookup.ItemIDtoGumpID.TryGetValue(layers[i].ItemID, out mappedGumpID);

					if (mappedGumpID == 0)
					{
						continue; // not in the dictionary
					}

					if (mob.Female && HasFemaleOffset(mappedGumpID))
					{
						mappedGumpID += 10000; // female offset
					}

					gump.AddImage(x, y, mappedGumpID, layers[i].Hue == 0 ? 0 : layers[i].Hue - 1);
					// weird gump art hue offset (skips jet black)
				}
			}

			return new Rectangle2D(GetX(trigObject), GetY(trigObject), GetWidth(trigObject), GetHeight(trigObject));
		}

		private bool HasFemaleOffset(int mappedGumpID)
		{
			mappedGumpID += 10000;

			if (GumpIndexData.ParsedGumpData.Count <= mappedGumpID)
			{
				return false;
			}

			return ((GumpData)GumpIndexData.ParsedGumpData[mappedGumpID]).Width != 0;
		}
	}

	public class UberGumpItem : UberGumpElement
	{
		protected MathTree m_Hue = new MathTree(null, "0");

		public int GetHue(TriggerObject trigObject)
		{
			object output = m_Hue.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetHue did not resolve to an integer: " + output + " from " + m_Hue.ScriptString);
			}

			return (int)output;
		}

		protected MathTree m_ItemID = new MathTree(null, "0");

		public int GetItemID(TriggerObject trigObject)
		{
			object output = m_ItemID.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetItemID did not resolve to an integer: " + output + " from " + m_ItemID.ScriptString);
			}

			return (int)output;
		}

		protected MathTree m_Width = new MathTree(null, "44"); // 44 is default tile size

		public override int GetWidth(TriggerObject trigObject)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return 0;
			}

			object output = m_Width.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetWidth did not resolve to an integer: " + output + " from " + m_Width.ScriptString);
			}

			return (int)output;
		}

		protected MathTree m_Height = new MathTree(null, "44"); // 44 is default tile size

		public override int GetHeight(TriggerObject trigObject)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return 0;
			}

			object output = m_Height.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetHeight did not resolve to an integer: " + output + " from " + m_Height.ScriptString);
			}

			return (int)output;
		}

		public UberGumpItem(XmlNode fromNode)
			: base(fromNode)
		{
			// for debugging: the
			XmlAttribute LastAttributeSetAttempt = null;

			try
			{
				if (fromNode.Attributes != null)
				{
					foreach (XmlAttribute attribute in fromNode.Attributes)
					{
						LastAttributeSetAttempt = attribute;

						string lowerCaseAttribute = attribute.LocalName.ToLower();

						switch (lowerCaseAttribute)
						{
							case "width":
								m_Width = new MathTree(null, attribute.Value);
								break;
							case "height":
								m_Height = new MathTree(null, attribute.Value);
								break;
							case "hue":
								m_Hue = new MathTree(null, attribute.Value);
								break;
							case "itemid":
								m_ItemID = new MathTree(null, attribute.Value);
								break;
							default:
								{
									if (lowerCaseAttribute != "x" && lowerCaseAttribute != "y" && lowerCaseAttribute != "condition")
									{
										throw new UberScriptException(attribute.LocalName + " is not a valid attribute for this node!");
									}
								}
								break;
						}
					}
				}

				LastAttributeSetAttempt = null;
			}
			catch (Exception e)
			{
				if (LastAttributeSetAttempt != null)
				{
					throw new UberScriptException(
						"UberGumpBase Error processing node: " + fromNode.LocalName + "... AttributeError on attribute " +
						LastAttributeSetAttempt.LocalName,
						e);
				}

				throw new UberScriptException("UberGumpBase Error processing node: " + fromNode.LocalName + "...", e);
			}

			// don't process any children (Label can't have any)
			//ParseChildren(fromNode);
		}

		public override void AddChild(UberGumpElement child)
		{
			throw new NotImplementedException();
		}

		public override Rectangle2D GenerateGump(TriggerObject trigObject, UberScriptGump gump, int originX, int originY)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return new Rectangle2D();
			}

			gump.AddItem(originX + GetX(trigObject), originY + GetY(trigObject), GetItemID(trigObject), GetHue(trigObject));

			return new Rectangle2D(GetX(trigObject), GetY(trigObject), GetWidth(trigObject), GetHeight(trigObject));
		}
	}

	/// <summary>
	///     UberGumpList basically iterates through a list and re-executes its children each time
	/// </summary>
	public class UberGumpList : UberGumpElement
	{
		protected MathTree m_ListSource = new MathTree(null, "null");

		public ArrayList GetListSource(TriggerObject trigObject)
		{
			return m_ListSource.Calculate(trigObject) as ArrayList;
		}

		protected MathTree m_LoopFunction = new MathTree(null, "null");

		public string GetLoopFunction(TriggerObject trigObject)
		{
			object output = m_LoopFunction.Calculate(trigObject);

			if (output == null)
			{
				return null;
			}

			return output.ToString();
		}

		protected MathTree m_ObjsVarName = new MathTree(null, "uberGumpListObj");

		public string GetObjsVarName(TriggerObject trigObject)
		{
			object output = m_ObjsVarName.Calculate(trigObject);

			if (output == null)
			{
				return "null";
			}

			return output.ToString();
		}

		public override int GetWidth(TriggerObject trigObject)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return 0;
			}

			int maxWidth = 0;

			foreach (UberGumpElement child in Children)
			{
				/*
				// int labelWidthModifier = 0;
                // I was thinking of automatically calculating label widths but i don't think it's worth it 
				// (the gump maker is on the hook to make it the right width)
                if (child is UberGumpLabel)
                {
                    // test to see if we can estimate the width based on the elements
                    UberGumpLabel label = (UberGumpLabel)child;
                    ArrayList actualList = GetListSource(trigObject);
                    if (actualList != null && actualList.Count > 1)
                    {
                        string objsVarName = GetObjsVarName(trigObject);
                        trigObject.objs[objsVarName] = actualList[0];
                        // now that we have the item in the list, get the label width as normal (but add some extra space)
                        labelWidthModifier = 10;
                    }
                }
                */

				int childWidth = 0;
				int childX = 0;

				// have to go through the elements and "test" them with real list data to see what their 
				// actual widths will be (also this allows for conditions on objects in the list)
				ArrayList actualList = GetListSource(trigObject);

				if (actualList != null)
				{
					string key = GetObjsVarName(trigObject);
					//string functionName = GetLoopFunction(trigObject); 
					// don't execute this function in between; it's too dangerous in the width calc!

					foreach (object element in actualList)
					{
						trigObject.objs[key] = element;

						int testX = child.GetX(trigObject);
						int testWidth = child.GetWidth(trigObject); // + labelWidthModifier;

						if (testWidth + testX <= childWidth + childX)
						{
							continue;
						}

						childWidth = testWidth;
						childX = testX;
					}
				}

				if (childWidth + childX > maxWidth)
				{
					maxWidth = childWidth;
				}
			}

			return maxWidth;
		}

		public override int GetHeight(TriggerObject trigObject)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return 0;
			}

			int maxHeight = 0;

			foreach (UberGumpElement child in Children)
			{
				int childHeight = 0;
				int childY = 0;

				// have to go through the elements and "test" them with real list data to see what their actual widths will be (also this allows for conditions on objects in the list)
				ArrayList actualList = GetListSource(trigObject);
				if (actualList != null)
				{
					string key = GetObjsVarName(trigObject);

					//string functionName = GetLoopFunction(trigObject); // don't execute this function in between; it's too dangerous in the width calc!
					foreach (object element in actualList)
					{
						trigObject.objs[key] = element;

						int testY = child.GetY(trigObject);
						int testHeight = child.GetHeight(trigObject); // + labelWidthModifier;

						if (testHeight + testY <= childHeight + childY)
						{
							continue;
						}

						childHeight = testHeight;
						childY = testY;
					}
				}

				if (childHeight + childY > maxHeight)
				{
					maxHeight = childHeight;
				}
			}

			return maxHeight;
		}

		public UberGumpList(XmlNode fromNode)
			: base(fromNode)
		{
			// for debugging: the
			XmlAttribute LastAttributeSetAttempt = null;

			try
			{
				if (fromNode.Attributes != null)
				{
					foreach (XmlAttribute attribute in fromNode.Attributes)
					{
						LastAttributeSetAttempt = attribute;

						string lowerCaseAttribute = attribute.LocalName.ToLower();

						switch (lowerCaseAttribute)
						{
							case "list":
								m_ListSource = new MathTree(null, attribute.Value);
								break;
							case "loopfunction":
								m_LoopFunction = new MathTree(null, attribute.Value);
								break;
							case "objs":
								m_ObjsVarName = new MathTree(null, attribute.Value);
								break;
							default:
								{
									if (lowerCaseAttribute != "x" && lowerCaseAttribute != "y" && lowerCaseAttribute != "condition")
									{
										throw new UberScriptException(attribute.LocalName + " is not a valid attribute for this node!");
									}
								}
								break;
						}
					}
				}

				LastAttributeSetAttempt = null;
			}
			catch (Exception e)
			{
				if (LastAttributeSetAttempt != null)
				{
					throw new UberScriptException(
						"UberGumpBase Error processing node: " + fromNode.LocalName + "... AttributeError on attribute " +
						LastAttributeSetAttempt.LocalName,
						e);
				}

				throw new UberScriptException("UberGumpBase Error processing node: " + fromNode.LocalName + "...", e);
			}

			ParseChildren(fromNode);
		}

		public override Rectangle2D GenerateGump(TriggerObject trigObject, UberScriptGump gump, int originX, int originY)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return new Rectangle2D();
			}

			int x = originX + GetX(trigObject);
			int y = originY + GetY(trigObject);

			gump.CurrentList = GetListSource(trigObject);

			string objName = GetObjsVarName(trigObject);
			ArrayList prevList = gump.CurrentList;
			object prevListObject = gump.CurrentListObject;

			gump.CurrentListObject = trigObject.objs.ContainsKey(objName) ? trigObject.objs[objName] : null;

			foreach (UberGumpElement child in Children)
			{
				child.GenerateGump(trigObject, gump, x, y);
			}

			gump.CurrentList = prevList;
			gump.CurrentListObject = prevListObject;

			return new Rectangle2D(GetX(trigObject), GetY(trigObject), GetWidth(trigObject), GetHeight(trigObject));
		}
	}

	public class UberGumpBox : UberGumpElement
	{
		protected MathTree m_Width = new MathTree(null, "0");

		public override int GetWidth(TriggerObject trigObject)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return 0;
			}

			if (FitToContents)
			{
				int padding = GetPadding(trigObject);
				int width = padding * 2;

				foreach (UberGumpElement child in Children)
				{
					int childX = child.GetX(trigObject);
					int childWidth = childX + child.GetWidth(trigObject) + padding * 2;

					if (childWidth > width)
					{
						width = childWidth;
					}
				}

				return width;
			}

			object output = m_Width.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetWidth did not resolve to an integer: " + output + " from " + m_Width.ScriptString);
			}

			return (int)output;
		}

		protected MathTree m_Height = new MathTree(null, "0");

		public override int GetHeight(TriggerObject trigObject)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return 0;
			}

			if (FitToContents)
			{
				int padding = GetPadding(trigObject);
				int height = padding * 2;

				foreach (UberGumpElement child in Children)
				{
					int childY = child.GetY(trigObject);
					int childHeight = childY + child.GetHeight(trigObject) + padding * 2;

					if (childHeight > height)
					{
						height = childHeight;
					}
				}

				return height;
			}

			object output = m_Height.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetHeight did not resolve to an integer: " + output + " from " + m_Height.ScriptString);
			}

			return (int)output;
		}

		protected MathTree m_Padding = new MathTree(null, "0");

		public int GetPadding(TriggerObject trigObject)
		{
			object output = m_Padding.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetPadding did not resolve to an integer: " + output + " from " + m_Padding.ScriptString);
			}

			return (int)output;
		}

		protected MathTree m_BackgroundID = new MathTree(null, "0");

		public int GetBackgroundID(TriggerObject trigObject)
		{
			object output = m_BackgroundID.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetBackgroundID did not resolve to an integer: " + output + " from " + m_BackgroundID.ScriptString);
			}

			return (int)output;
		}

		public bool FitToContents = true;

		public UberGumpBox(XmlNode fromNode)
			: base(fromNode)
		{
			// for debugging: the
			XmlAttribute LastAttributeSetAttempt = null;

			try
			{
				if (fromNode.Attributes != null)
				{
					foreach (XmlAttribute attribute in fromNode.Attributes)
					{
						LastAttributeSetAttempt = attribute;

						string lowerCaseAttribute = attribute.LocalName.ToLower();

						switch (lowerCaseAttribute)
						{
							case "width":
								m_Width = new MathTree(null, attribute.Value);
								break;
							case "height":
								m_Height = new MathTree(null, attribute.Value);
								break;
							case "padding":
								m_Padding = new MathTree(null, attribute.Value);
								break;
							case "backgroundid":
								m_BackgroundID = new MathTree(null, attribute.Value);
								break;
							case "fittocontents":
								FitToContents = Boolean.Parse(attribute.Value);
								break;
							default:
								{
									if (lowerCaseAttribute != "horizontalgap" && lowerCaseAttribute != "verticalgap" && lowerCaseAttribute != "x" &&
										lowerCaseAttribute != "y" && lowerCaseAttribute != "condition")
									{
										throw new UberScriptException(attribute.LocalName + " is not a valid attribute for this node!");
									}
								}
								break;
						}
					}
				}

				LastAttributeSetAttempt = null;
			}
			catch (Exception e)
			{
				if (LastAttributeSetAttempt != null)
				{
					throw new UberScriptException(
						"UberGumpBox Error processing node: " + fromNode.LocalName + "... AttributeError on attribute " +
						LastAttributeSetAttempt.LocalName,
						e);
				}

				throw new UberScriptException("UberGumpBox Error processing node: " + fromNode.LocalName + "...", e);
			}

			// don't parseChildren if we inherit from UberGumpBox
			if (GetType() == typeof(UberGumpBox))
			{
				ParseChildren(fromNode);
			}
		}

		public override Rectangle2D GenerateGump(TriggerObject trigObject, UberScriptGump gump, int originX, int originY)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return new Rectangle2D();
			}

			int backgroundID = GetBackgroundID(trigObject);
			int padding = GetPadding(trigObject);
			int x = originX + GetX(trigObject);
			int y = originY + GetY(trigObject);
			int width = GetWidth(trigObject);
			int height = GetHeight(trigObject);

			if (backgroundID != 0)
			{
				gump.AddBackground(x, y, width, height, backgroundID);
			}

			x += padding;
			y += padding;

			foreach (UberGumpElement child in Children)
			{
				child.GenerateGump(trigObject, gump, x, y);
			}

			return new Rectangle2D(GetX(trigObject), GetY(trigObject), GetWidth(trigObject), GetHeight(trigObject));
		}
	}

	public class UberGumpHBox : UberGumpBox
	{
		protected MathTree m_HorizontalGap = new MathTree(null, "0");

		public int GetHorizontalGap(TriggerObject trigObject)
		{
			object output = m_HorizontalGap.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetHorizontalGap did not resolve to an integer: " + output + " from " + m_HorizontalGap.ScriptString);
			}

			return (int)output;
		}

		public override int GetWidth(TriggerObject trigObject)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return 0;
			}

			if (FitToContents)
			{
				int padding = GetPadding(trigObject);
				int width = 0;
				int horizontalGap = GetHorizontalGap(trigObject);

				int count = 0;

				foreach (UberGumpElement child in Children)
				{
					int childWidth = child.GetWidth(trigObject);
					int childX = child.GetX(trigObject);

					// list--assume they are all as wide as the first one
					if (child is UberGumpList)
					{
						UberGumpList list = (UberGumpList)child;
						ArrayList actualList = list.GetListSource(trigObject);

						if (actualList != null)
						{
							if (actualList.Count > 1)
							{
								width += (childWidth) * (actualList.Count - 1);
							}
						}
					}

					width += childWidth + childX;
					count += 1;
				}

				return width + padding * 2 + ((count - 1) * horizontalGap);
			}

			object output = m_Width.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetWidth did not resolve to an integer: " + output + " from " + m_Width.ScriptString);
			}

			return (int)output;
		}

		public UberGumpHBox(XmlNode fromNode)
			: base(fromNode)
		{
			// for debugging: the
			XmlAttribute LastAttributeSetAttempt = null;

			try
			{
				if (fromNode.Attributes != null)
				{
					foreach (XmlAttribute attribute in fromNode.Attributes)
					{
						LastAttributeSetAttempt = attribute;
						string lowerCaseAttribute = attribute.LocalName.ToLower();
						if (lowerCaseAttribute == "horizontalgap")
						{
							m_HorizontalGap = new MathTree(null, attribute.Value);
						}
						else if (lowerCaseAttribute != "width" && lowerCaseAttribute != "height" && lowerCaseAttribute != "x" &&
								 lowerCaseAttribute != "y" && lowerCaseAttribute != "padding" && lowerCaseAttribute != "backgroundid" &&
								 lowerCaseAttribute != "fittocontents" && lowerCaseAttribute != "condition")
						{
							throw new UberScriptException(attribute.LocalName + " is not a valid attribute for this node!");
						}
					}
				}

				LastAttributeSetAttempt = null;
			}
			catch (Exception e)
			{
				if (LastAttributeSetAttempt != null)
				{
					throw new UberScriptException(
						"UberGumpHBox Error processing node: " + fromNode.LocalName + "... AttributeError on attribute " +
						LastAttributeSetAttempt.LocalName,
						e);
				}

				throw new UberScriptException("UberGumpHBox Error processing node: " + fromNode.LocalName + "...", e);
			}

			ParseChildren(fromNode);
		}

		public override Rectangle2D GenerateGump(TriggerObject trigObject, UberScriptGump gump, int originX, int originY)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return new Rectangle2D();
			}

			int backgroundID = GetBackgroundID(trigObject);
			int padding = GetPadding(trigObject);
			int x = originX + GetX(trigObject);
			int y = originY + GetY(trigObject);
			int width = GetWidth(trigObject);
			int height = GetHeight(trigObject);
			int horizontalGap = GetHorizontalGap(trigObject);

			if (backgroundID != 0)
			{
				gump.AddBackground(x, y, width, height, backgroundID);
			}

			x += padding;
			y += padding;

			foreach (UberGumpElement child in Children)
			{
				child.GenerateGump(trigObject, gump, x, y);
				x += child.GetWidth(trigObject) + horizontalGap;
			}

			return new Rectangle2D(GetX(trigObject), GetY(trigObject), GetWidth(trigObject), GetHeight(trigObject));
		}
	}

	public class UberGumpVBox : UberGumpBox
	{
		protected MathTree m_VerticalGap = new MathTree(null, "0");

		public int GetVerticalGap(TriggerObject trigObject)
		{
			object output = m_VerticalGap.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetVerticalGap did not resolve to an integer: " + output + " from " + m_VerticalGap.ScriptString);
			}

			return (int)output;
		}

		public override int GetHeight(TriggerObject trigObject)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return 0;
			}

			if (FitToContents)
			{
				int padding = GetPadding(trigObject);
				int height = 0;
				int verticalGap = GetVerticalGap(trigObject);

				int count = 0;

				foreach (UberGumpElement child in Children)
				{
					int childHeight = child.GetHeight(trigObject);
					int childY = child.GetY(trigObject);

					// list--assume they are all as tall as the first one
					if (child is UberGumpList)
					{
						UberGumpList list = (UberGumpList)child;
						ArrayList actualList = list.GetListSource(trigObject);

						if (actualList != null)
						{
							if (actualList.Count > 1)
							{
								height += (childHeight) * (actualList.Count - 1);
							}
						}
					}

					height += childHeight + childY;
					count += 1;
				}

				return height + padding * 2 + ((count - 1) * verticalGap);
			}

			object output = m_Height.Calculate(trigObject);

			if (!(output is int))
			{
				throw new UberScriptException(
					GetType() + " GetHeight did not resolve to an integer: " + output + " from " + m_Width.ScriptString);
			}

			return (int)output;
		}

		public UberGumpVBox(XmlNode fromNode)
			: base(fromNode)
		{
			// for debugging: the
			XmlAttribute LastAttributeSetAttempt = null;

			try
			{
				if (fromNode.Attributes != null)
				{
					foreach (XmlAttribute attribute in fromNode.Attributes)
					{
						LastAttributeSetAttempt = attribute;

						string lowerCaseAttribute = attribute.LocalName.ToLower();

						if (lowerCaseAttribute == "verticalgap")
						{
							m_VerticalGap = new MathTree(null, attribute.Value);
						}
						else if (lowerCaseAttribute != "width" && lowerCaseAttribute != "height" && lowerCaseAttribute != "x" &&
								 lowerCaseAttribute != "y" && lowerCaseAttribute != "padding" && lowerCaseAttribute != "backgroundid" &&
								 lowerCaseAttribute != "fittocontents" && lowerCaseAttribute != "condition")
						{
							throw new UberScriptException(attribute.LocalName + " is not a valid attribute for this node!");
						}
					}
				}

				LastAttributeSetAttempt = null;
			}
			catch (Exception e)
			{
				if (LastAttributeSetAttempt != null)
				{
					throw new UberScriptException(
						"UberGumpHBox Error processing node: " + fromNode.LocalName + "... AttributeError on attribute " +
						LastAttributeSetAttempt.LocalName,
						e);
				}

				throw new UberScriptException("UberGumpHBox Error processing node: " + fromNode.LocalName + "...", e);
			}

			ParseChildren(fromNode);
		}

		public override Rectangle2D GenerateGump(TriggerObject trigObject, UberScriptGump gump, int originX, int originY)
		{
			if (m_Condition != null && GetCondition(trigObject) == false)
			{
				return new Rectangle2D();
			}

			int backgroundID = GetBackgroundID(trigObject);
			int padding = GetPadding(trigObject);
			int x = originX + GetX(trigObject);
			int y = originY + GetY(trigObject);
			int width = GetWidth(trigObject);
			int height = GetHeight(trigObject);
			int verticalGap = GetVerticalGap(trigObject);

			if (backgroundID != 0)
			{
				gump.AddBackground(x, y, width, height, backgroundID);
			}

			x += padding;
			y += padding;

			foreach (UberGumpElement child in Children)
			{
				if (child is UberGumpList)
				{
					UberGumpList list = (UberGumpList)child;
					ArrayList actualList = list.GetListSource(trigObject);

					if (actualList != null)
					{
						string key = list.GetObjsVarName(trigObject);
						string functionName = list.GetLoopFunction(trigObject);

						foreach (object element in actualList)
						{
							trigObject.objs[key] = element;
							child.GenerateGump(trigObject, gump, x, y);
							y += child.GetHeight(trigObject) + verticalGap;

							// check the script to execute in between
							if (functionName == null)
							{
								continue;
							}

							RootNode scriptNode = trigObject.Script.ScriptRootNode;

							if (scriptNode != null)
							{
								foreach (UserDefinedFunctionNode node in
									scriptNode.UserDefinedFunctionNodes.Where(node => node.ScriptString == functionName))
								{
									node.Execute(trigObject);
									break;
								}
							}
						}
					}
				}
				else
				{
					child.GenerateGump(trigObject, gump, x, y);
					y += child.GetHeight(trigObject) + verticalGap;
				}
			}
			return new Rectangle2D(GetX(trigObject), GetY(trigObject), GetWidth(trigObject), GetHeight(trigObject));
		}
	}

	//public void AddButton( int x, int y, int normalID, int pressedID, int buttonID, GumpButtonType type, int param )
	//public void AddCheck( int x, int y, int inactiveID, int activeID, bool initialState, int switchID )
	//public void AddRadio( int x, int y, int inactiveID, int activeID, bool initialState, int switchID )

	public class TutorialGump : Gump
	{
		public TutorialGump()
			: base(0, 0)
		{
			// =========== ITEM ===========
			// x, y, itemid, hue
			AddItem(100, 100, 130);

			// =========== BACKGROUNDS =============
			/* AddBackground( X, Y, SizeX, SizeY, GumpID ); 
			 * GumpID is the start of 9 consequtive ID's that form the Background
			 * Look in Gump Studio and see how it looks 
			 */
			AddBackground(300, 10, 100, 100, 83);
			AddLabel(339, 52, 0, "83");

			// =========== LABELS ==============
			// AddLabel( X, Y, Hue, string Message );
			AddLabel(20, 210, 0, "I am a plain label");
			// You can change the hue of the label by changing the Hue to any hue number. 

			AddLabel(20, 225, 2, "I am a hued label");
			// You can pass in other strings into the Message part......|           |

			// =========== IMAGES =============
			/* AddImage( X, Y, GumpID );
			 * Shows a single GumpID
			 */
			AddImage(285, 0, 1211);
			AddLabel(325, 10, 1153, "Single Image");

			/* AddImageTiled( X, Y, SizeX, SizeY, GumpID );
             * Shows multiple 'copies' of a GumpID
			 * Will cut the picture if it is bigger than the tiled size 
			 */
			AddImageTiled(0, 235, 111, 117, 1211);
			AddLabel(10, 212, 1153, "Tiled Image (Cut)");
			// Just an example that if will tile both ways, right/left and up/down
			AddImageTiled(135, 235, 142, 271, 1211);
			AddLabel(145, 212, 1153, "Tiled Image (Full)");

			// ============ ALPHA ==============
			/* AddAlphaRegion( X, Y, SizeX, SizeY ); 
			 * Notice the order of adding these two.
			 * The first adds the alpha region 'over' (after) the background.
             * And 'under' (before) the label/button 
			 */
			AddBackground(0, 185, 100, 100, 9270);
			AddAlphaRegion(10, 195, 80, 80);
			AddLabel(12, 200, 1153, "Alpha Under");
			AddButton(18, 235, 247, 248, 1, GumpButtonType.Reply, 0);
			/* This one adds the alpha region 'over' (after) both the background AND the label/button.
             * See the difference in game? 
			 */
			AddBackground(100, 185, 100, 100, 9270);
			AddLabel(115, 200, 1153, "Alpha Over");
			AddButton(118, 235, 247, 248, 1, GumpButtonType.Reply, 0);
			AddAlphaRegion(110, 195, 80, 80);
			// Notice you can still press the button with the alpha region 'over' it.

			// ============ BUTTONS =============
			/* Up untill now, these AddPage(0)'s have not been looked at
			 * So I will describe them now...
             * Page 0 is ALWAYS show in a gump.  This means that if you change pages with a page button,
             * the stuff that is on Page 0 is shown as well. 
			 */
			AddPage(0);

			// That means these two background will show all the time, or untill the gump is closed
			AddBackground(0, 185, 100, 100, 9270);
			AddBackground(100, 185, 100, 100, 9270);

			/* Page 1 is the first page to be displayed. (On top of 0)
             * If there is no Page 1, you will only see Page 0, with nothing else. 
			 */
			AddPage(1);
			AddLabel(28, 200, 0, "Page 1");

			// AddButton( X, Y, UnpressedGumpID, PressedGumpID, ButtonID, GumpButtonType, Page );
			AddButton(18, 235, 247, 248, (int)Buttonss.Page2Button, GumpButtonType.Page, 2);
			/* You can see here that I am using this |^^^^^^^^^^^^^^^^^^^^^^^| form of ButtonID.
             * The enum Buttonss keeps track of what ButtonID each button has, and it's easier to keep track of which buttons are which.(IMO)
             * You can also simply put an int there, like 2, 4, 92, etc.
             * ButtonID 0 is the Exit button.  This means if you right click on the gump to close, you are 'pressing' a button with ButtonID=0.
             * In a Page button, (to my knowledge) the ButtonID does not matter.
			 * The last number in the call is which page to open.  In this case, it is page 2, so when the button is pushed
             * the gump jumps to where you see AddPage(2) and runs the following code. 
			 */

			// This is where Page 2 code starts
			AddPage(2);
			// Added a new background, as you can see in game
			AddBackground(1, 285, 100, 100, 9270);

			/* Also added a Page button to go back to Page 1.
             * When this button is pressed, the Gump will show Page 1 again.
             * Notice I used a 3 for the ButtonID.  You can use any int you want for this.
             * Notice how the other stuff is gone that showed up when Page 2 was opened 
			 */
			AddButton(18, 235, 247, 248, 3, GumpButtonType.Page, 3);
			AddLabel(28, 300, 0, "Page 2");

			/* This is a Reply Button.  It has the same call as the Page button, but it uses the GumpButtonType.Reply.
             * In the Reply buttons, the ButtonID matters.  You will use this ID to check which button was pressed in OnResponse.
             * In Reply buttons, the Page (Last number in the call) does not matter.  (That I know of)
             * When a Reply button is pressed, the gump closes and runs the OnResponse method.
             * That method checks to see what each ButtonID is assigned to do. (More on this below)
			 */
			AddButton(118, 235, 247, 248, (int)Buttonss.ReplyButton, GumpButtonType.Reply, 0);
			AddLabel(132, 200, 0, "Reply");

			// =========== CHECKBOXES / RADIOBUTTONS =================
			AddBackground(0, 0, 100, 100, 9270);
			AddLabel(25, 5, 1153, "Radios");

			//AddButton( X, Y, UnCheckedGumpID, CheckedGumpID, StartChecked?, SwitchID );
			AddRadio(15, 30, 208, 209, false, 1);
			AddRadio(60, 30, 208, 209, false, 2);
			AddRadio(15, 60, 208, 209, false, 3);
			AddRadio(60, 60, 208, 209, false, 4);

			// Out of the four Radio buttons above, you can only select one.
			// When you select one, the other will un-select (if it was selected)
			AddLabel(20, 30, 1153, "1");
			AddLabel(65, 30, 1153, "2");
			AddLabel(20, 60, 1153, "3");
			AddLabel(65, 60, 1153, "4");

			AddBackground(100, 0, 100, 100, 9270);
			AddLabel(125, 5, 1153, "Checks");

			// AddCheck( X, Y, UnCheckedGumpID, CheckedGumpID, StartChecked?, SwitchID );
			AddCheck(115, 30, 208, 209, false, 10);
			AddCheck(160, 30, 208, 209, false, 11);
			AddCheck(115, 60, 208, 209, false, 12);
			AddCheck(160, 60, 208, 209, false, 13);

			// Out of the four Check buttons above, you can select as many as you want.
			AddLabel(120, 30, 1153, "1");
			AddLabel(165, 30, 1153, "2");
			AddLabel(120, 60, 1153, "3");
			AddLabel(165, 60, 1153, "4");

			AddBackground(0, 100, 100, 100, 9270);
			AddLabel(30, 105, 1153, "Reply");
			AddButton(17, 140, 247, 248, 20, GumpButtonType.Reply, 0);

			/* from pseudoseergump
            AddBackground(0, 0, WIDTH, HEIGHT, 5054);
            AddAlphaRegion(LEFT_EDGE, TOP_EDGE, WIDTH - 2 * LEFT_EDGE, HEIGHT - 2 * TOP_EDGE);

            AddHtml(penX, penY, WIDTH - 2 * LEFT_EDGE, lineHeight, Color(Center("<u>Pseudoseer Control Panel</u>"), LabelColor32), false, false);
            penY += lineHeight;

            AddButton(penX, penY, ArrowButtonID1, ArrowButtonID2, (int)BUTTON_IDS.AddPseudoSeer, GumpButtonType.Reply, 0);
            penX += NextWidth;
            AddLabel(penX, penY, TextHue, "Add Pseudoseer / Update Pseudoseer Permissions");
            penX = LEFT_EDGE;
            penY += lineHeight;

            AddButton(penX, penY, ArrowButtonID1, ArrowButtonID2, (int)BUTTON_IDS.RemovePseudoSeer, GumpButtonType.Reply, 0);
            penX += NextWidth;
            AddLabel(penX, penY, TextHue, "Remove Pseudoseer");
            penX = LEFT_EDGE + WIDTH / 2;
            AddButton(penX, penY, ArrowButtonID1, ArrowButtonID2, (int)BUTTON_IDS.RemoveAllPseudoSeers, GumpButtonType.Reply, 0);
            penX += NextWidth;
            AddLabel(penX, penY, TextHue, "Remove ALL Pseudoseers");
            penY += lineHeight;
            */
		}

		/* This is the enum of the Buttonss.
		 *
		 * Gump Studio uses this form when creating the scripts of the gumps.
		 * I find it (in most cases) easier to keep track of the buttons by using this method
		 * so I can give each button a name.
		 *
		 * One thing GumpStudio does not do is add in the 'Exit' button.
		 * The first button in this list below is going to have an int value of 0, being the Exit button.
		 * GumpStudio does not take that into account, so I just add in a new entry of 'Exit'
		 */

		public enum Buttonss
		{
			Exit,
			Page2Button,
			ReplyButton,
		}
	}
}