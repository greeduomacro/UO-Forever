using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using Server.Commands;
using System.Xml;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.XmlSpawner2
{
    public class UberGumpEditor
    {
        public static void Initialize()
        {
            CommandSystem.Register("UberGumpEdit", AccessLevel.GameMaster, new CommandEventHandler(EditGump_Command));
        }

        public static void EditGump_Command(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (e.Arguments.Length == 0) 
            { 
                from.SendMessage("You must provide a file name to save open / save to. Note that \"GUMP_\" is prepended and \".xml\" is appended to the file name you enter (e.g. '[editgump test' would open and/or create the 'GUMP_test.xml' file).  Also, you can specify the name of an XmlScript (and trigger name if you want) attached to your character that you want to execute before the gump is displayed, e.g. [ubergumpedit testScript onUse ... this allows for the objs in that script to be available to the gump.");
                return;
            }
            
            try
            {
                
                XmlDocument parsedXml = null;
                // first, always clear out deleted scripts
                string fileName = "GUMP_" + e.Arguments[0].ToLower() + ".xml";
                string fileNameLower = fileName.ToLower();
                string path = null;
                UberGumpBase parsedGump = null;

                ParsedGumps.GumpFileMap.TryGetValue(fileNameLower, out path);
                if (path == null)
                {
                    path = Path.GetFullPath(Path.Combine(Core.BaseDirectory, ParsedScripts.UberScriptDirectory, fileName));
                }
    
                if (!File.Exists(path))
                {
                    // create a new file
                        
                    from.SendMessage("A new gump file was created at " + path);
                    string initialText = @"<Gump>
<VBox padding=""10"" backgroundID=""9250"">
    <Label>""Replace me!""</Label>
</VBox>
</Gump>";
                    LoggingCustom.Log(path, initialText);
                    ParsedGumps.GumpFileMap.Add(fileNameLower, path);
                    parsedXml = new XmlDocument(); //* create an xml document object.
                    parsedXml.Load(path); //* load the XML document from the specified file.
                    //if (parsedXml == null) throw new Exception("Xml Document parsed to null!");
                    parsedGump = new UberGumpBase(parsedXml.FirstChild);
                    ParsedGumps.Gumps.Add(path, parsedGump);
                }
                else
                {
                    // always reload and parse the file
                    parsedXml = new XmlDocument(); //* create an xml document object.
                    parsedXml.Load(path); //* load the XML document from the specified file.
                    //if (parsedXml == null) throw new Exception("Gump file not parsed correctly...");
                    parsedGump = new UberGumpBase(parsedXml.FirstChild);
                    ParsedGumps.Gumps[path] = parsedGump;
                    ParsedGumps.GumpFileMap[fileNameLower] = path;
                }
                from.CloseGump(typeof(UberGumpEditorGump));
                from.CloseGump(typeof(UberScriptGump));
                TriggerObject trigObject = new TriggerObject();
                trigObject.TrigMob = from;
                trigObject.This = from;
                XmlScript scriptToUse = null;
                TriggerName triggerToUse = TriggerName.NoTrigger;
                if (e.Arguments.Length > 1)
                {
                    List<XmlScript> scripts = XmlAttach.GetScripts(from);
                    foreach (XmlScript script in scripts)
                    {
                        if (script.Name == e.Arguments[1])
                        {
                            scriptToUse = script;
                            if (e.Arguments.Length > 2)
                            {
                                try
                                {
                                    triggerToUse = (TriggerName)Enum.Parse(typeof(TriggerName), e.Arguments[2].ToLower(), true); 
                                }
                                catch { }
                            }
                            break;
                        }
                    }
                }

                if (scriptToUse == null) // just send the gump
                {
                    from.SendGump(new UberScriptGump(from, trigObject, parsedGump, fileName));
                    from.CloseGump(typeof(UberGumpTree));
                    from.SendGump(new UberGumpTree(from, parsedXml.FirstChild, parsedXml, fileName));
                    from.SendGump(new UberGumpEditorGump(from, parsedXml.FirstChild, parsedXml, fileName));
                }
                else
                {
                    // if there is a script, it is expected that the script will send this gump
                    // using the SENDGUMP command
                    trigObject.Script = scriptToUse;
                    if (triggerToUse == TriggerName.NoTrigger)
                    {
                        scriptToUse.Execute(trigObject, false);
                    }
                    else
                    {
                        trigObject.TrigName = triggerToUse;
                        scriptToUse.Execute(trigObject, true); 
                    }
                    from.CloseGump(typeof(UberGumpTree));
                    from.SendGump(new UberGumpTree(from, parsedXml.FirstChild, parsedXml, fileName, scriptToUse, triggerToUse));
                    from.SendGump(new UberGumpEditorGump(from, parsedXml.FirstChild, parsedXml, fileName, scriptToUse, triggerToUse));
                }
            }
            catch (Exception except)
            {
                from.SendMessage("Exception caught: " + except.Message);
            }
        }
    }

    public class UberGumpEditorGump : Gump
    {

        private Mobile m_Owner;
        private XmlNode m_CurrentNode;
        private XmlDocument m_ParentDocument;
        private string m_FileName;
        private XmlScript m_Script;
        private TriggerName m_TriggerName = TriggerName.NoTrigger;

        public UberGumpEditorGump(Mobile owner, XmlNode currentNode, XmlDocument xmlDocument, string fileName,
                                    XmlScript script = null, TriggerName triggerName = TriggerName.NoTrigger)
            : base(300, 50)
		{
			m_Owner = owner;
            m_CurrentNode = currentNode;
            m_ParentDocument = xmlDocument;
            m_FileName = fileName;
            m_Script = script;
            m_TriggerName = triggerName;
            Closable = true;
            Dragable = true;

			InitializeGump();
		}

        public enum BUTTON_IDS : int
        {
            Closed = 0,
            DeleteElement = 1,
            UpdateElement = 2,
            NavigateParent = 3,
            NavigateNextSibling = 4,
            NavigatePrevSibling = 5,
            NavigateChild = 6,
            MoveToParent = 7,
            MoveToNextSibling = 8,
            MoveToChild = 9,
            MoveToPrevSibling = 10,
            AddHBox = 11,
            AddVBox = 12,
            AddBox = 13,
            AddButton = 14,
            AddCheckBox = 15,
            AddRadioButton = 16,
            AddTextInput = 17,
            AddLabel = 18,
            AddHTML = 19,
            AddImage = 20,
            AddItem = 21,
            AddSpacer = 22,
            AddPaperdoll = 23,
            BackgroundHelp = 24
        }

        private const int WIDTH = 500;
        private const int LEFT_EDGE = 10;
        private const int TOP_EDGE = 10;

        public void InitializeGump()
        {
            int penX = LEFT_EDGE;
            int penY = TOP_EDGE;

            UberGumpElementDefinition[] definitions = null;
            string currentLowerName = m_CurrentNode.LocalName.ToLower();
            UberGumpElementDefinition.UberGumpElementDictionary.TryGetValue(currentLowerName, out definitions);
            int gumpHeight = 225;
            if (definitions != null)
            {
                gumpHeight += definitions.Length * 26;
            }
            if (currentLowerName == "box" || currentLowerName == "gump" || currentLowerName == "vbox" || currentLowerName == "hbox")
            {
                gumpHeight += 275; // for the add child menu
            }

            AddBackground(0, 0, WIDTH, gumpHeight, 5054);


            // NAVIGATION ==========================================
            penY += 22;
            AddLabel(penX, penY, 0x38, "Navigation:");
            
            //21 x 21

            penX += 90;
            if (!(m_CurrentNode.ParentNode == m_ParentDocument || m_CurrentNode.ParentNode == null))
            {
                AddButton(penX, penY, 9909, 9911, (int)BUTTON_IDS.NavigateParent, GumpButtonType.Reply, 0);
            }


            penX += 22;
            penY -= 22;
            if (m_CurrentNode.PreviousSibling != null)
            {
                AddButton(penX, penY, 9900, 9902, (int)BUTTON_IDS.NavigatePrevSibling, GumpButtonType.Reply, 0);
            }

            
            penY += 44;
            if (m_CurrentNode.NextSibling != null)
            {
                AddButton(penX, penY, 9906, 9908, (int)BUTTON_IDS.NavigateNextSibling, GumpButtonType.Reply, 0);
            }

            penX += 22;
            penY -= 22;

            if (m_CurrentNode.ChildNodes.Count != 0)
            {
                AddButton(penX, penY, 9903, 9905, (int)BUTTON_IDS.NavigateChild, GumpButtonType.Reply, 0);
            }

            if (!(m_CurrentNode.ParentNode == null || m_CurrentNode.ParentNode == m_ParentDocument)) // can't delete the root gump node
            {
                penX += 45;
                AddButton(penX, penY, 4017, 4019, (int)BUTTON_IDS.DeleteElement, GumpButtonType.Reply, 0);
                penX += 31;
                AddLabel(penX, penY, 0x38, "Delete Element");
            }

            penY += 70;
            penX = LEFT_EDGE;


            // MOVE GUMP ELEMENT =====================================
            AddLabel(penX, penY, 0x38, "Move Gump Element:");


            //21 x 21

            penX += 140;
            if (!(m_CurrentNode.ParentNode == null || m_CurrentNode.ParentNode == m_ParentDocument || m_CurrentNode.ParentNode == m_ParentDocument.FirstChild))
            {
                AddButton(penX, penY, 9909, 9911, (int)BUTTON_IDS.MoveToParent, GumpButtonType.Reply, 0);
            }

            penX += 22;
            penY -= 22;
            if (m_CurrentNode.PreviousSibling != null)
            {
                AddButton(penX, penY, 9900, 9902, (int)BUTTON_IDS.MoveToPrevSibling, GumpButtonType.Reply, 0);
            }


            penY += 44;
            if (m_CurrentNode.NextSibling != null)
            {
                AddButton(penX, penY, 9906, 9908, (int)BUTTON_IDS.MoveToNextSibling, GumpButtonType.Reply, 0);
            }

            penX += 22;
            penY -= 22;
            if (m_CurrentNode.PreviousSibling != null)
            {
                string prevSiblingNameLower = m_CurrentNode.PreviousSibling.LocalName.ToLower();
                if (prevSiblingNameLower == "gump" || prevSiblingNameLower == "box" || prevSiblingNameLower == "vbox" || prevSiblingNameLower == "hbox")
                {
                    AddButton(penX, penY, 9903, 9905, (int)BUTTON_IDS.MoveToChild, GumpButtonType.Reply, 0);
                }
            }

            penX = LEFT_EDGE;
            penY += 70;

            AddLabel(penX, penY, 0x38, "==== Currently Selected: ====");
            penY += 22;
            AddLabel(penX, penY, 1153, m_CurrentNode.LocalName);
            penX += 140;
            AddButton(penX, penY, 4014, 4016, (int)BUTTON_IDS.UpdateElement, GumpButtonType.Reply, 0);
            penX += 31;
            AddLabel(penX, penY, 1153, "Update Element");

            penX = LEFT_EDGE + 10;
            penY += 26;

            if (definitions != null)
            {
                int entryID = 1000;
                int switchID = 2000;
                foreach (UberGumpElementDefinition definition in definitions)
                {
                    AddLabel(penX, penY, 0x38, definition.Name + ":");
                    penX += 80;

                    XmlNode currentValue = m_CurrentNode.Attributes.GetNamedItem(definition.Name);
                    if (currentValue != null)
                    {
                        if (definition.AttributeType == UberGumpElementDefinition.UberGumpElementAttributeType.String)
                        {
                            AddTextEntry(penX, penY, 100, 22, 0, entryID, currentValue.InnerText);
                            entryID += 1;
                        }
                        else // boolean
                        {
                            AddCheck(penX, penY, 2151, 2153, currentValue.Value == "true", switchID);
                            switchID += 1;
                        }
                    }
                    else
                    {
                        if (definition.AttributeType == UberGumpElementDefinition.UberGumpElementAttributeType.String)
                        {
                            if (definition.Name == "text" && (currentLowerName == "label" || currentLowerName == "html" )) 
                            {
                                AddTextEntry(penX, penY, 400, 200, 0, entryID, m_CurrentNode.InnerXml == "" ? definition.DefaultState : m_CurrentNode.InnerXml);
                                penY += 38;
                            }
                            else
                            {
                                AddTextEntry(penX, penY, 100, 22, 0, entryID, definition.DefaultState);
                            }
                            entryID += 1;
                        }
                        else // boolean
                        {
                            AddCheck(penX, penY, 2151, 2153, definition.DefaultState == "true", switchID);
                            switchID += 1;
                        }
                    }

                    if (definition.Name == "backgroundID")
                    {
                        // add a help button
                        penX += 150;
                        AddButton(penX, penY, 4014, 4016, (int)BUTTON_IDS.BackgroundHelp, GumpButtonType.Reply, 0);
                        penX += 31;
                        AddLabel(penX, penY, 1153, "Background Help");
                    }
                    penX = LEFT_EDGE + 10;
                    penY += 22;

                    
                }
            }

            // need this in the onResponse function
            //if (currentLowerName == "label") // has an extra text box
            //{
             //   penX = LEFT_EDGE;
              //      penY += 24;
           // }

            // if it is a container, let them add child elements
            if (currentLowerName == "hbox" || currentLowerName == "vbox" || currentLowerName == "box" || currentLowerName == "gump")
            {
                penY += 35;
                AddLabel(penX, penY, 0x38, "Add Child Element:");
                penY += 24;

                int addElementStartingY = penY;
                AddLabel(penX, penY, 0x38, "Containers:");
                penY += 22;

                AddButton(penX, penY, 4005, 4007, (int)BUTTON_IDS.AddHBox, GumpButtonType.Reply, 0);
                AddLabel(penX + 31, penY, 0x38, "HBox");
                penY += 24;
                AddButton(penX, penY, 4005, 4007, (int)BUTTON_IDS.AddVBox, GumpButtonType.Reply, 0);
                AddLabel(penX + 31, penY, 0x38, "VBox");
                penY += 24;
                AddButton(penX, penY, 4005, 4007, (int)BUTTON_IDS.AddBox, GumpButtonType.Reply, 0);
                AddLabel(penX + 31, penY, 0x38, "Box");

                penY += 30;
                AddLabel(penX, penY, 0x38, "Input:");
                penY += 22;

                AddButton(penX, penY, 4005, 4007, (int)BUTTON_IDS.AddButton, GumpButtonType.Reply, 0);
                AddLabel(penX + 31, penY, 0x38, "Button");
                penY += 24;
                AddButton(penX, penY, 4005, 4007, (int)BUTTON_IDS.AddCheckBox, GumpButtonType.Reply, 0);
                AddLabel(penX + 31, penY, 0x38, "CheckBox");
                penY += 24;
                AddButton(penX, penY, 4005, 4007, (int)BUTTON_IDS.AddRadioButton, GumpButtonType.Reply, 0);
                AddLabel(penX + 31, penY, 0x38, "RadioButton");
                penY += 24;
                AddButton(penX, penY, 4005, 4007, (int)BUTTON_IDS.AddTextInput, GumpButtonType.Reply, 0);
                AddLabel(penX + 31, penY, 0x38, "TextEntry");

                penY = addElementStartingY;
                penX += 150;


                AddLabel(penX, penY, 0x38, "Text:");
                penY += 22;

                AddButton(penX, penY, 4005, 4007, (int)BUTTON_IDS.AddLabel, GumpButtonType.Reply, 0);
                AddLabel(penX + 31, penY, 0x38, "Label");
                penY += 24;
                AddButton(penX, penY, 4005, 4007, (int)BUTTON_IDS.AddHTML, GumpButtonType.Reply, 0);
                AddLabel(penX + 31, penY, 0x38, "HTML");

                penY += 30;
                AddLabel(penX, penY, 0x38, "Artwork:");
                penY += 22;

                AddButton(penX, penY, 4005, 4007, (int)BUTTON_IDS.AddImage, GumpButtonType.Reply, 0);
                AddLabel(penX + 31, penY, 0x38, "Image (Gump)");
                penY += 24;
                AddButton(penX, penY, 4005, 4007, (int)BUTTON_IDS.AddItem, GumpButtonType.Reply, 0);
                AddLabel(penX + 31, penY, 0x38, "Item");
                penY += 24;
                AddButton(penX, penY, 4005, 4007, (int)BUTTON_IDS.AddPaperdoll, GumpButtonType.Reply, 0);
                AddLabel(penX + 31, penY, 0x38, "Paperdoll");

                penY += 30;
                AddLabel(penX, penY, 0x38, "Formatting:");
                penY += 22;

                AddButton(penX, penY, 4005, 4007, (int)BUTTON_IDS.AddSpacer, GumpButtonType.Reply, 0);
                AddLabel(penX + 31, penY, 0x38, "Spacer");
                penY += 24;
            }
        }



        public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
            string toAdd = null;
            bool updateFileRequired = false;
            XmlDocument backup = null;
            List<int> backupCurrentNodeLocation = null;

			switch ( (BUTTON_IDS)info.ButtonID )
			{
                case BUTTON_IDS.Closed:
                    return;
                case BUTTON_IDS.AddBox: toAdd = UberGumpElementDefinition.NewXmlElement("Box"); break;
                case BUTTON_IDS.AddButton: toAdd = UberGumpElementDefinition.NewXmlElement("Button"); break;
                case BUTTON_IDS.AddCheckBox: toAdd = UberGumpElementDefinition.NewXmlElement("Check"); break;
                case BUTTON_IDS.AddHBox: toAdd = UberGumpElementDefinition.NewXmlElement("HBox"); break;
                case BUTTON_IDS.AddHTML: toAdd = UberGumpElementDefinition.NewXmlElement("HTML"); break;
                case BUTTON_IDS.AddImage: toAdd = UberGumpElementDefinition.NewXmlElement("Image"); break;
                case BUTTON_IDS.AddItem: toAdd = UberGumpElementDefinition.NewXmlElement("Item"); break;
                case BUTTON_IDS.AddLabel: toAdd = UberGumpElementDefinition.NewXmlElement("Label"); break;
                case BUTTON_IDS.AddPaperdoll: toAdd = UberGumpElementDefinition.NewXmlElement("Paperdoll"); break;
                case BUTTON_IDS.AddRadioButton: toAdd = UberGumpElementDefinition.NewXmlElement("Radio"); break;
                case BUTTON_IDS.AddSpacer: toAdd = UberGumpElementDefinition.NewXmlElement("Spacer"); break;
                case BUTTON_IDS.AddTextInput: toAdd = UberGumpElementDefinition.NewXmlElement("TextEntry"); break;
                case BUTTON_IDS.AddVBox: toAdd = UberGumpElementDefinition.NewXmlElement("VBox"); break;
                case BUTTON_IDS.DeleteElement: 
                    if (!(m_CurrentNode.ParentNode == null || m_CurrentNode.ParentNode == m_ParentDocument)) // can't delete the root gump node
                    {
                        XmlNode newCurrent = null;
                        if (m_CurrentNode.PreviousSibling != null)
                        {
                            newCurrent = m_CurrentNode.PreviousSibling;
                        }
                        else if (m_CurrentNode.NextSibling != null)
                        {
                            newCurrent = m_CurrentNode.NextSibling;
                        }
                        else
                        {
                            newCurrent = m_CurrentNode.ParentNode;
                        }

                        m_CurrentNode.ParentNode.RemoveChild(m_CurrentNode);
                        m_CurrentNode = newCurrent;
                        updateFileRequired = true;
                    }
                    break;
                case BUTTON_IDS.MoveToChild:
                    if (m_CurrentNode.PreviousSibling != null)
                    {
                        string prevSiblingNameLower = m_CurrentNode.PreviousSibling.LocalName.ToLower();
                        if (prevSiblingNameLower == "gump" || prevSiblingNameLower == "box" || prevSiblingNameLower == "vbox" || prevSiblingNameLower == "hbox")
                        {
                            // previous sibling is a container element, so you can move it to it
                            XmlNode prevSibling = m_CurrentNode.PreviousSibling;
                            m_CurrentNode.ParentNode.RemoveChild(m_CurrentNode);
                            prevSibling.AppendChild(m_CurrentNode);
                            updateFileRequired = true;
                        }
                    }
                    break;
                case BUTTON_IDS.MoveToNextSibling:
                    if (m_CurrentNode.NextSibling != null)
                    {
                        XmlNode nextSibling = m_CurrentNode.NextSibling;
                        XmlNode parent = m_CurrentNode.ParentNode;
                        parent.RemoveChild(m_CurrentNode);
                        parent.InsertAfter(m_CurrentNode, nextSibling);
                        updateFileRequired = true;
                    }
                    break;
                case BUTTON_IDS.MoveToParent:
                    if (!(m_CurrentNode.ParentNode == null || m_CurrentNode.ParentNode == m_ParentDocument || m_CurrentNode.ParentNode == m_ParentDocument.FirstChild))
                    {
                        XmlNode parent = m_CurrentNode.ParentNode;
                        XmlNode grandparent = parent.ParentNode;
                        parent.RemoveChild(m_CurrentNode);
                        grandparent.InsertAfter(m_CurrentNode, parent);
                        updateFileRequired = true;
                    }
                    break;
                case BUTTON_IDS.MoveToPrevSibling:
                    if (m_CurrentNode.PreviousSibling != null)
                    {
                        XmlNode prevSibling = m_CurrentNode.PreviousSibling;
                        XmlNode parent = m_CurrentNode.ParentNode;
                        parent.RemoveChild(m_CurrentNode);
                        parent.InsertBefore(m_CurrentNode, prevSibling);
                        updateFileRequired = true;
                    }
                    break;
                case BUTTON_IDS.NavigateChild:
                    if (m_CurrentNode.ChildNodes.Count > 0)
                    {
                        m_CurrentNode = m_CurrentNode.ChildNodes[0];
                    }
                    break;
                case BUTTON_IDS.NavigateParent:
                    if (!(m_CurrentNode.ParentNode == null || m_CurrentNode.ParentNode == m_ParentDocument)) // can't move up past gump node
                    {
                        m_CurrentNode = m_CurrentNode.ParentNode;
                    }
                    break;
                case BUTTON_IDS.NavigatePrevSibling:
                    if (m_CurrentNode.PreviousSibling != null)
                    {
                        m_CurrentNode = m_CurrentNode.PreviousSibling;
                    }
                    break;
                case BUTTON_IDS.NavigateNextSibling:
                    if (m_CurrentNode.NextSibling!= null)
                    {
                        m_CurrentNode = m_CurrentNode.NextSibling;
                    }
                    break;
                case BUTTON_IDS.BackgroundHelp:
                    m_Owner.CloseGump(typeof(Backgrounds));
                    m_Owner.SendGump(new Backgrounds());
                    break;
                case BUTTON_IDS.UpdateElement:
                    // make a backup
                    //string path = Path.GetFullPath(Path.Combine(Core.BaseDirectory, ParsedScripts.UberScriptDirectory, "GUMPEDITOR_BACKUP.xml"));
                    //using (var file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                   // {
                    //    m_ParentDocument.Save(file);
                    //}
                    backup = (XmlDocument)m_ParentDocument.CloneNode(true);
                    backupCurrentNodeLocation = GetMapToNode(m_CurrentNode);

                    updateFileRequired = true;
                    UberGumpElementDefinition[] definitions = null;
                    UberGumpElementDefinition.UberGumpElementDictionary.TryGetValue(m_CurrentNode.LocalName.ToLower(), out definitions);
                    int switchCount = 0;
                    int textEntryCount = 0;

                    if (definitions != null)
                    {
                        for (int i = 0; i < definitions.Length; i++) //info.TextEntries.Length; i++)
                        {
                            if (definitions[i].AttributeType == UberGumpElementDefinition.UberGumpElementAttributeType.Boolean)
                            {
                                bool trueSwitch = false;
                                for (int j = 0; j < info.Switches.Length; j++)
                                {
                                    if ((info.Switches[j] - 2000) == switchCount)
                                    {
                                        // it's true
                                        XmlNode attribute = m_CurrentNode.Attributes.GetNamedItem(definitions[i].Name);
                                        if (attribute == null)
                                        {
                                            if (m_CurrentNode is XmlElement)
                                            {
                                                ((XmlElement)m_CurrentNode).SetAttribute(definitions[i].Name, "true");
                                            }
                                            else
                                            {
                                                Console.WriteLine("Current Node: " + m_CurrentNode.OuterXml + " was not an xmlElement!");
                                            }
                                        }
                                        else
                                        {
                                            attribute.Value = "true";
                                        }
                                        trueSwitch = true;
                                    }
                                }
                                if (!trueSwitch)
                                {
                                    XmlNode attribute = m_CurrentNode.Attributes.GetNamedItem(definitions[i].Name);
                                    if (attribute == null)
                                    {
                                        if (m_CurrentNode is XmlElement)
                                        {
                                            ((XmlElement)m_CurrentNode).SetAttribute(definitions[i].Name, "false");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Current Node: " + m_CurrentNode.OuterXml + " was not an xmlElement!");
                                        }
                                    }
                                    else
                                    {
                                        attribute.Value = "false";
                                    }
                                }
                                switchCount += 1;
                            }
                            else // it's a String attribute
                            {
                                // should match up the lengths perfectly, except if it is a label
                                string currentLowerName = m_CurrentNode.LocalName.ToLower();
                                XmlNode attribute = m_CurrentNode.Attributes.GetNamedItem(definitions[i].Name);
                                if (attribute == null)
                                {
                                    if (definitions[i].Name == "text" && (currentLowerName == "label" || currentLowerName == "html" ))
                                    {
                                        if (currentLowerName == "html")
                                        {
                                            try
                                            {
                                                while (m_CurrentNode.ChildNodes.Count > 0)
                                                {
                                                    m_CurrentNode.RemoveChild(m_CurrentNode.ChildNodes[0]);
                                                }
                                                
                                                XmlDocumentFragment fragment = m_ParentDocument.CreateDocumentFragment();
                                                fragment.InnerXml = info.TextEntries[textEntryCount].Text;
                                                // if we got here, then it is formatted correctly
                                                m_CurrentNode.AppendChild(fragment);
                                            }
                                            catch
                                            {
                                                m_CurrentNode.InnerText = info.TextEntries[textEntryCount].Text;
                                            }
                                        }
                                        else
                                        {
                                            m_CurrentNode.InnerText = info.TextEntries[textEntryCount].Text;
                                        }
                                    }
                                    else
                                    {
                                        if (m_CurrentNode is XmlElement)
                                        {
                                            ((XmlElement)m_CurrentNode).SetAttribute(definitions[i].Name, info.TextEntries[textEntryCount].Text);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Current Node: " + m_CurrentNode.OuterXml + " was not an xmlElement!");
                                        }
                                    }
                                }
                                else
                                {
                                    attribute.Value = info.TextEntries[textEntryCount].Text;
                                }
                                textEntryCount += 1;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Current Node: " + m_CurrentNode.LocalName + " does not have definition!");
                    }
                    break;
            }

            if (toAdd != null)
            {
                backup = (XmlDocument)m_ParentDocument.CloneNode(true);
                backupCurrentNodeLocation = GetMapToNode(m_CurrentNode);
                XmlDocumentFragment fragment = m_ParentDocument.CreateDocumentFragment();
                fragment.InnerXml = toAdd;
                m_CurrentNode.AppendChild(fragment);
                updateFileRequired = true;
            }

            XmlDocument parsedXml = null;

            if (updateFileRequired)
            {
                try
                {
                    // serialize xml to the file
                    string path = Path.GetFullPath(Path.Combine(Core.BaseDirectory, ParsedScripts.UberScriptDirectory, m_FileName));
                    //Console.WriteLine("STARTED Updating xml gump file: " + path);
                    using (var file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        m_ParentDocument.Save(file);
                    }
                    Console.WriteLine("COMPLETED updating xml gump file: " + path);

                    // need to reload the gump!

                    // unfortunately I think the XmlDocumentFragments become an issue, so I'm going to
                    // reload the gump from the xml file such that they are recast into XmlElements...
                    parsedXml = new XmlDocument();
                    parsedXml.Load(path); //* load the XML document from the specified file.
                    if (parsedXml == null) throw new Exception("Gump file not parsed correctly...");

                    UberGumpBase parsedGump = new UberGumpBase(parsedXml.FirstChild);
                    ParsedGumps.Gumps[path] = parsedGump;
                    ParsedGumps.GumpFileMap[m_FileName.ToLower()] = path;
                    m_Owner.CloseGump(typeof(UberGumpEditorGump));
                    m_Owner.CloseGump(typeof(UberScriptGump));
                    TriggerObject trigObject = new TriggerObject();
                    trigObject.TrigMob = m_Owner;
                    trigObject.This = m_Owner;

                    // need to create a map back down to the m_CurrentNode for the newly parsed xml tree
                    List<int> mapToCurrentNode = GetMapToNode(m_CurrentNode);
                    XmlNode current = parsedXml;
                    foreach (int childIndex in mapToCurrentNode)
                    {
                        current = current.ChildNodes[childIndex];
                    }

                    if (m_Script != null)
                    {
                        trigObject.Script = m_Script;
                        if (m_TriggerName == TriggerName.NoTrigger)
                        {
                            m_Script.Execute(trigObject, false);
                        }
                        else
                        {
                            trigObject.TrigName = m_TriggerName;
                            m_Script.Execute(trigObject, true);
                        }
                        // it is expected that the script will SENDGUMP 
                        m_Owner.CloseGump(typeof(UberGumpTree));
                        m_Owner.SendGump(new UberGumpTree(m_Owner, current, parsedXml, m_FileName, m_Script, m_TriggerName));

                        m_Owner.SendGump(new UberGumpEditorGump(m_Owner, current, parsedXml, m_FileName, m_Script, m_TriggerName));
                    }
                    else
                    {
                        m_Owner.SendGump(new UberScriptGump(m_Owner, trigObject, parsedGump, m_FileName));
                        m_Owner.CloseGump(typeof(UberGumpTree));
                        m_Owner.SendGump(new UberGumpTree(m_Owner, current, parsedXml, m_FileName));

                        m_Owner.SendGump(new UberGumpEditorGump(m_Owner, current, parsedXml, m_FileName));
                    }
                }
                catch (Exception e)
                {
                    m_Owner.SendMessage(38, "Gump update error: " + e.Message + "          " + e.StackTrace);

                    try
                    {
                        string path = Path.GetFullPath(Path.Combine(Core.BaseDirectory, ParsedScripts.UberScriptDirectory, m_FileName));
                        //Console.WriteLine("STARTED Updating xml gump file: " + path);
                        using (var file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            backup.Save(file);
                        }
                        Console.WriteLine("COMPLETED updating xml gump file: " + path);

                        // need to reload the gump!

                        // unfortunately I think the XmlDocumentFragments become an issue, so I'm going to
                        // reload the gump from the xml file such that they are recast into XmlElements...
                        parsedXml = new XmlDocument();
                        parsedXml.Load(path); //* load the XML document from the specified file.
                        if (parsedXml == null) throw new Exception("Gump file not parsed correctly...");

                        UberGumpBase parsedGump = new UberGumpBase(parsedXml.FirstChild);
                        ParsedGumps.Gumps[path] = parsedGump;
                        ParsedGumps.GumpFileMap[m_FileName.ToLower()] = path;
                        m_Owner.CloseGump(typeof(UberGumpEditorGump));
                        m_Owner.CloseGump(typeof(UberScriptGump));
                        TriggerObject trigObject = new TriggerObject();
                        trigObject.TrigMob = m_Owner;
                        trigObject.This = m_Owner;

                        // need to create a map back down to the m_CurrentNode for the newly parsed xml tree
                        XmlNode current = parsedXml;
                        foreach (int childIndex in backupCurrentNodeLocation)
                        {
                            current = current.ChildNodes[childIndex];
                        }

                        if (m_Script != null)
                        {
                            trigObject.Script = m_Script;
                            if (m_TriggerName == TriggerName.NoTrigger)
                            {
                                m_Script.Execute(trigObject, false);
                            }
                            else
                            {
                                trigObject.TrigName = m_TriggerName;
                                m_Script.Execute(trigObject, true);
                            }
                            // it is expected that the script will SENDGUMP 
                            m_Owner.CloseGump(typeof(UberGumpTree));
                            m_Owner.SendGump(new UberGumpTree(m_Owner, current, parsedXml, m_FileName, m_Script, m_TriggerName));

                            m_Owner.SendGump(new UberGumpEditorGump(m_Owner, current, parsedXml, m_FileName, m_Script, m_TriggerName));
                        }
                        else
                        {
                            m_Owner.SendGump(new UberScriptGump(m_Owner, trigObject, parsedGump, m_FileName));
                            m_Owner.CloseGump(typeof(UberGumpTree));
                            m_Owner.SendGump(new UberGumpTree(m_Owner, current, parsedXml, m_FileName));

                            m_Owner.SendGump(new UberGumpEditorGump(m_Owner, current, parsedXml, m_FileName));
                        }
                    }
                    catch (Exception e2)
                    {
                        m_Owner.SendMessage(100, "Gump recovery failed!: " + e2.Message + "          " + e2.StackTrace);
                    }
                }
            }
            else
            {
                m_Owner.CloseGump(typeof(UberGumpTree));
                m_Owner.SendGump(new UberGumpTree(m_Owner, m_CurrentNode, m_ParentDocument, m_FileName, m_Script, m_TriggerName));
                m_Owner.SendGump(new UberGumpEditorGump(m_Owner, m_CurrentNode, m_ParentDocument, m_FileName, m_Script, m_TriggerName));
            }
        }

        public List<int> GetMapToNode(XmlNode currentNode)
        {
            List<int> output = new List<int>();
            while (currentNode != null && currentNode.ParentNode != null) // will stop when Document is reached
            {
                for (int i = 0; i < currentNode.ParentNode.ChildNodes.Count; i++)
                {
                    if (currentNode.ParentNode.ChildNodes[i] == currentNode)
                    {
                        output.Insert(0, i);
                        break;
                    }
                }
                currentNode = currentNode.ParentNode;
            }
            return output;
        }
    }
        

    public class UberGumpElementDefinition
    {
        public static Dictionary<string, UberGumpElementDefinition[]> UberGumpElementDictionary = new Dictionary<string, UberGumpElementDefinition[]>();
            
        public static void Initialize()
        {
            UberGumpElementDictionary["gump"] = new UberGumpElementDefinition[] 
            {  
                new UberGumpElementDefinition("closable", UberGumpElementAttributeType.Boolean, "true"),
                new UberGumpElementDefinition("disposable", UberGumpElementAttributeType.Boolean, "true"),
                new UberGumpElementDefinition("dragable", UberGumpElementAttributeType.Boolean, "false"),
                new UberGumpElementDefinition("resizable", UberGumpElementAttributeType.Boolean, "true"),
            };
            UberGumpElementDictionary["box"] = new UberGumpElementDefinition[] 
            {   
                new UberGumpElementDefinition("fitToContents", UberGumpElementAttributeType.Boolean, "false"),
                new UberGumpElementDefinition("x", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("y", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("width", UberGumpElementAttributeType.String, "200"),
                new UberGumpElementDefinition("height", UberGumpElementAttributeType.String, "200"),
                new UberGumpElementDefinition("backgroundID", UberGumpElementAttributeType.String, "3500"),
                new UberGumpElementDefinition("padding", UberGumpElementAttributeType.String, "20"),
            };
            UberGumpElementDictionary["vbox"] = new UberGumpElementDefinition[] 
            {  
                new UberGumpElementDefinition("fitToContents", UberGumpElementAttributeType.Boolean, "true"),
                new UberGumpElementDefinition("x", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("y", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("width", UberGumpElementAttributeType.String, "44"),
                new UberGumpElementDefinition("height", UberGumpElementAttributeType.String, "44"),
                new UberGumpElementDefinition("backgroundID", UberGumpElementAttributeType.String, "3500"),
                new UberGumpElementDefinition("verticalGap", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("padding", UberGumpElementAttributeType.String, "20")
            };
            UberGumpElementDictionary["hbox"] = new UberGumpElementDefinition[] 
            {  
                new UberGumpElementDefinition("fitToContents", UberGumpElementAttributeType.Boolean, "true"),
                new UberGumpElementDefinition("x", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("y", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("width", UberGumpElementAttributeType.String, "44"),
                new UberGumpElementDefinition("height", UberGumpElementAttributeType.String, "44"),
                new UberGumpElementDefinition("backgroundID", UberGumpElementAttributeType.String, "3500"),
                new UberGumpElementDefinition("horizontalGap", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("padding", UberGumpElementAttributeType.String, "20")
            };
            UberGumpElementDictionary["label"] = new UberGumpElementDefinition[] 
            {  
                new UberGumpElementDefinition("x", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("y", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("hue", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("text", UberGumpElementAttributeType.String, "")
            };
            UberGumpElementDictionary["button"] = new UberGumpElementDefinition[] 
            {  
                new UberGumpElementDefinition("x", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("y", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("name", UberGumpElementAttributeType.String, ""),
                new UberGumpElementDefinition("normalID", UberGumpElementAttributeType.String, "2074"),
                new UberGumpElementDefinition("pressedID", UberGumpElementAttributeType.String, "2075")
            };
            UberGumpElementDictionary["check"] = new UberGumpElementDefinition[] 
            {  
                new UberGumpElementDefinition("x", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("y", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("name", UberGumpElementAttributeType.String, ""),
                new UberGumpElementDefinition("inactiveID", UberGumpElementAttributeType.String, "210"),
                new UberGumpElementDefinition("activeID", UberGumpElementAttributeType.String, "211"),
                new UberGumpElementDefinition("default", UberGumpElementAttributeType.Boolean, "false")
            };
            UberGumpElementDictionary["radio"] = new UberGumpElementDefinition[] 
            {  
                new UberGumpElementDefinition("x", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("y", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("name", UberGumpElementAttributeType.String, ""),
                new UberGumpElementDefinition("inactiveID", UberGumpElementAttributeType.String, "10850"),
                new UberGumpElementDefinition("activeID", UberGumpElementAttributeType.String, "10830"),
                new UberGumpElementDefinition("default", UberGumpElementAttributeType.Boolean, "false")
            };
            UberGumpElementDictionary["textentry"] = new UberGumpElementDefinition[] 
            {  
                new UberGumpElementDefinition("x", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("y", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("name", UberGumpElementAttributeType.String, ""),
                new UberGumpElementDefinition("size", UberGumpElementAttributeType.String, "230"),
                new UberGumpElementDefinition("width", UberGumpElementAttributeType.String, "44"),
                new UberGumpElementDefinition("height", UberGumpElementAttributeType.String, "44"),
                new UberGumpElementDefinition("default", UberGumpElementAttributeType.String, "")
            };
            UberGumpElementDictionary["image"] = new UberGumpElementDefinition[] 
            {  
                new UberGumpElementDefinition("x", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("y", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("gumpID", UberGumpElementAttributeType.String, "106"), // hand
                new UberGumpElementDefinition("hue", UberGumpElementAttributeType.String, "0")
            };
            UberGumpElementDictionary["item"] = new UberGumpElementDefinition[] 
            {  
                new UberGumpElementDefinition("x", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("y", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("itemID", UberGumpElementAttributeType.String, "130"), // wall of stone
                new UberGumpElementDefinition("width", UberGumpElementAttributeType.String, "44"),
                new UberGumpElementDefinition("height", UberGumpElementAttributeType.String, "44"),
                new UberGumpElementDefinition("hue", UberGumpElementAttributeType.String, "0")
            };
            UberGumpElementDictionary["html"] = new UberGumpElementDefinition[] 
            {  
                new UberGumpElementDefinition("x", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("y", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("width", UberGumpElementAttributeType.String, "300"),
                new UberGumpElementDefinition("height", UberGumpElementAttributeType.String, "100"),
                new UberGumpElementDefinition("scrollbar", UberGumpElementAttributeType.Boolean, "true"),
                new UberGumpElementDefinition("hasbackground", UberGumpElementAttributeType.Boolean, "true"),
                new UberGumpElementDefinition("text", UberGumpElementAttributeType.String, "")
            };
            UberGumpElementDictionary["list"] = new UberGumpElementDefinition[] 
            {  
                new UberGumpElementDefinition("x", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("y", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("list", UberGumpElementAttributeType.String, "NEWLIST()"),
                new UberGumpElementDefinition("loopfunction", UberGumpElementAttributeType.String, ""),
                new UberGumpElementDefinition("objs", UberGumpElementAttributeType.String, "")
            };
            UberGumpElementDictionary["paperdoll"] = new UberGumpElementDefinition[] 
            {  
                new UberGumpElementDefinition("x", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("y", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("mob", UberGumpElementAttributeType.String, "TRIGMOB()")
            };
            UberGumpElementDictionary["spacer"] = new UberGumpElementDefinition[] 
            {  
                new UberGumpElementDefinition("x", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("y", UberGumpElementAttributeType.String, "0"),
                new UberGumpElementDefinition("width", UberGumpElementAttributeType.String, "10"),
                new UberGumpElementDefinition("height", UberGumpElementAttributeType.String, "10")
            };
        }
            
        public enum UberGumpElementAttributeType
        {
            Boolean,
            String
        }

        public UberGumpElementAttributeType AttributeType = UberGumpElementAttributeType.String;
        public string Name;

        public string DefaultState;

        public UberGumpElementDefinition(string name, UberGumpElementAttributeType type, string defaultState)
        {
            AttributeType = type;
            Name = name;
            DefaultState = defaultState;
        }

        public string ToXML()
        {
            return " " + Name + "=\"" + DefaultState +"\"";
        }

        public static string NewXmlElement(string elementType)
        {
            string output = "";
            UberGumpElementDefinition[] definitions = null;
            string elementTypeLower = elementType.ToLower();
            UberGumpElementDictionary.TryGetValue(elementTypeLower, out definitions);
            if (definitions != null)
            {
                output = "<" + elementType;
                foreach (UberGumpElementDefinition definition in definitions)
                {
                    if (definition.Name == "text" && (elementType == "Label" || elementType == "HTML")) // never add this attribute since it is the innerText
                        continue;
                    output += definition.ToXML();
                }
                output += "/>";
            }
            else
            {
                Console.WriteLine("elementType: " + elementType + " not found!");
                return null;
            }
            return output;
        }
    }
}
