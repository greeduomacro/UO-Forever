using System;
using Server;
using System.Xml;
using Server.Network;
using Server.Engines.XmlSpawner2;
using System.Collections.Generic;

namespace Server.Gumps
{
    public class UberGumpTree : Gump
    {
        private Mobile m_Owner;
        private XmlNode m_CurrentNode;
        private XmlDocument m_ParentDocument;
        private string m_FileName;
        private XmlScript m_Script;
        private TriggerName m_TriggerName = TriggerName.NoTrigger;
        
        private const int ENTRY_HEIGHT = 18;

        public int CalculateHeight(XmlNode current)
        {
            string name = current.LocalName.ToLower();
            if (name == "label" || name == "html")
            {
                return ENTRY_HEIGHT; // don't go for the children (since their innertext is technically a child, but don't count it
            }
            int output = ENTRY_HEIGHT; // height of this node
            foreach (XmlNode child in current.ChildNodes)
            {
                output += CalculateHeight(child);
            }
            return output;
        }

        public void Traverse(XmlNode current, int penX, ref int penY, ref int buttonID)
        {
            penY += ENTRY_HEIGHT;
            AddButton(penX, penY, 0x15E1, 0x15E5, buttonID, GumpButtonType.Reply, 0);
            buttonID += 1;

            if (m_CurrentNode == current)
                AddLabel(penX + 20, penY, 1153, current.LocalName);
            else
                AddLabel(penX + 20, penY, 0, current.LocalName);

            string name = current.LocalName.ToLower();
            if (name == "label" || name == "html")
            {
                return; // don't process text child node
            }
            foreach (XmlNode child in current.ChildNodes)
            {
                Traverse(child, penX + 15, ref penY, ref buttonID);
            }
        }

        public UberGumpTree(Mobile owner, XmlNode current, XmlDocument document, string fileName, XmlScript script = null, TriggerName triggerName = TriggerName.NoTrigger)
            : base(150, 50)
        {
            m_CurrentNode = current;
            m_ParentDocument = document;
            m_Owner = owner;
            m_FileName = fileName;
            m_Script = script;
            m_TriggerName = triggerName;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;
            AddPage(0);

            int height = CalculateHeight(document.FirstChild) + 40;
            AddBackground(0, 0, 168, height, 9300);
            AddLabel(10, 10, 38, "UberGump Tree Elements");
            int penY = 10;
            int buttonID = 0;
            Traverse(document.FirstChild, 10, ref penY, ref buttonID);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            int buttonID = info.ButtonID;
            if (buttonID == 0) return;

            int entryCount = 0;
            XmlNode gumpNode = m_ParentDocument.FirstChild;
            XmlNode current = gumpNode; // start from the gumpNode
            List<XmlNode> visited = new List<XmlNode>();
            // this is the kind of hacky non-recursive approach to find the buttonid corresponding to the right element in the xml tree
            while (entryCount < 1000)
            {
                if (visited.Contains(current))
                {
                    if (current.NextSibling == null)
                    {
                        if (current == gumpNode) // we can't do any more
                        {
                            break;
                        }
                        else
                        {
                            // go back up to the parent
                            current = current.ParentNode;
                            continue;
                        }
                    }
                    else
                    {
                        // go on to the next sibling
                        current = current.NextSibling;
                        continue;
                    }
                }

                visited.Add(current);

                if (buttonID == entryCount) { break; } 
                entryCount += 1;

                if (current.ChildNodes.Count > 0 && (current.LocalName.ToLower() != "label" && current.LocalName.ToLower() != "html"))
                {
                    current = current.FirstChild;
                    continue;
                }
                else
                {
                    if (current.NextSibling == null)
                    {
                        if (current == gumpNode) // we can't do any more
                        {
                            break;
                        }
                        else
                        {
                            // go back up to the parent
                            current = current.ParentNode;
                            continue;
                        }
                    }
                    else
                    {
                        current = current.NextSibling;
                        continue;
                    }
                }
            }
            m_CurrentNode = current;

            m_Owner.CloseGump(typeof(UberGumpTree));
            m_Owner.SendGump(new UberGumpTree(m_Owner, m_CurrentNode, m_ParentDocument, m_FileName, m_Script, m_TriggerName));
            m_Owner.CloseGump(typeof(UberGumpEditorGump));
            m_Owner.SendGump(new UberGumpEditorGump(m_Owner, m_CurrentNode, m_ParentDocument, m_FileName, m_Script, m_TriggerName));
        }
    }
}