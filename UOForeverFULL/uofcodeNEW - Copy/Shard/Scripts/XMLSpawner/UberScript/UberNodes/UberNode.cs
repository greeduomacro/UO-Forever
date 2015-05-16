using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Server.Mobiles;
using System.IO;

namespace Server.Engines.XmlSpawner2
{
    public enum ProcessResult
    {
        None,
        FailedIf,
        SucceedIf,
        Return,
        ReturnOverride,
        EndOfSequence,
        Pause,
        Break,
        Continue
    }

    public class UberNode
    {
        public UberNode Parent;
        public List<UberNode> Children = new List<UberNode>();
        public int LineNumber = -1;
        public bool Paused = false;

        public RootNode GetRootNode
        {
            get
            {
                UberNode current = this.Parent;
                if (current == null)
                {
                    if (this is RootNode) return this as RootNode;
                    else return null;
                }
                while (current != null)
                {
                    if (current is RootNode) return current as RootNode;
                    current = current.Parent;
                }
                return null;
            }
        }

        private string m_ScriptString;
        virtual public string ScriptString { get { return m_ScriptString; } set { m_ScriptString = value; } }

        public UberNode(UberNode parent, string scriptInput)
        {
            if (parent != null)
            {
                this.Parent = parent;
            }
            ScriptString = scriptInput;
        }

        public UberNode(UberNode parent, string scriptInput, int lineNumber)
        {
            if (parent != null)
            {
                this.Parent = parent;
            }
            LineNumber = lineNumber;
            ScriptString = scriptInput;
        }

        public UberNode PrevSibling()
        {
            if (Parent == null) return null;
            UberNode prevSibling = null;
            foreach (UberNode sibling in Parent.Children)
            {
                if (sibling == this)
                    return prevSibling;
                prevSibling = sibling;
            }
            return null;
        }

        public UberNode NextSibling()
        {
            if (Parent == null) return null;
            bool foundThis = false;
            foreach (UberNode sibling in Parent.Children)
            {
                if (foundThis) // we're on the next node after this
                {
                    return sibling;
                }
                if (sibling == this)
                    foundThis = true;
            }
            return null;
        }

        virtual public ProcessResult Process(ProcessResult lastSiblingResult, TriggerObject trigObject) // lastSiblingResult used for conditionals
        {
            throw new NotImplementedException();
        }

        protected ProcessResult ProcessChildren(TriggerObject trigObject)
        {
            ProcessResult lastResult = ProcessResult.None;
            PauseNode newPause = null;
            
            if (trigObject.PausedNodeChain != null)
            {
                 if (trigObject.PausedNodeChain.Peek() != this)
                     return ProcessResult.None;

                 trigObject.PausedNodeChain.Dequeue();
            }

            trigObject.CurrentNodeExecutionChain.Add(this);

            foreach (UberNode child in Children)
            {
                if (trigObject.PausedNodeChain != null)
                {
                    if (trigObject.PausedNodeChain.Peek() != child)
                        continue;
                    if (!(child is ConditionalNode)) // Conditional node handles it's own processChildren
                        trigObject.PausedNodeChain.Dequeue();
                    
                    if (trigObject.PausedNodeChain.Count == 0)
                    {
                        // if we got here, we found the node that we need to start execution on
                        trigObject.PausedNodeChain = null;
                    }
                }

                if (newPause != null) // just passed a pause node, time to stop execution
                {
                    trigObject.CurrentNodeExecutionChain.Add(child);
                    trigObject.PausedNodeChain = new Queue<UberNode>();
                    foreach (UberNode node in trigObject.CurrentNodeExecutionChain)
                    {
                        trigObject.PausedNodeChain.Enqueue(node);
                    }
                    trigObject.CurrentNodeExecutionChain = new List<UberNode>();
                    PausedUberScript paused = new PausedUberScript(trigObject, (trigObject != null && trigObject.TrigName != TriggerName.NoTrigger), newPause.PauseMS);
                    return ProcessResult.Return;
                }
                if (child is PauseNode)
                {
                    newPause = child as PauseNode;
                    continue;
                }
                if (child is BreakNode)
                {
                    trigObject.CurrentNodeExecutionChain.Remove(this);
                    return ProcessResult.Break;
                }
                if (child is ContinueNode)
                {
                    trigObject.CurrentNodeExecutionChain.Remove(this);
                    return ProcessResult.Continue;
                }
                if (trigObject.Script.ProceedToNextStage)
                {
                    // only execute the proper sequence node
                    if (child is SequenceNode)
                    {
                        SequenceNode sequenceNode = child as SequenceNode;
                        if (trigObject.Script.Stage < sequenceNode.Stage)
                        {
                            // found the next stage
                            trigObject.Script.Stage = sequenceNode.Stage;
                            trigObject.Script.ProceedToNextStage = false;
                            lastResult = child.Process(lastResult, trigObject);
                            trigObject.CurrentNodeExecutionChain.Remove(this);
                            return lastResult;
                        }
                    }
                    continue;
                }
                else
                {
                    if (child is TriggerNode) continue; // triggernodes execute only in response to events
                    else if (child is ConditionalNode && trigObject.PausedNodeChain == null) // if PausedNodeChain not null and it reached here, then it is the right node, don't depend on past conditionals--execute it
                    {
                        ConditionalNode conditionalNode = child as ConditionalNode;
                        if (conditionalNode.IfType == ConditionalNode.ConditionalType.Elif || conditionalNode.IfType == ConditionalNode.ConditionalType.Else)
                        {
                            if (lastResult == ProcessResult.FailedIf)
                            {
                                lastResult = child.Process(lastResult, trigObject);
                                if (lastResult == ProcessResult.Return || lastResult == ProcessResult.ReturnOverride)
                                {
                                    trigObject.CurrentNodeExecutionChain.Remove(this);
                                    return lastResult;
                                }
                            }
                            continue;
                        }
                    }
                }
                
                lastResult = child.Process(lastResult, trigObject);
                if (lastResult == ProcessResult.Return || lastResult == ProcessResult.ReturnOverride || lastResult == ProcessResult.Break || lastResult == ProcessResult.Continue)
                {
                    trigObject.CurrentNodeExecutionChain.Remove(this);
                    return lastResult;
                }
            }
            if (trigObject.Script.ProceedToNextStage)
            {
                lastResult = ProcessResult.EndOfSequence;
                trigObject.Script.ProceedToNextStage = false;
            }
            trigObject.CurrentNodeExecutionChain.Remove(this);
            if (lastResult == ProcessResult.FailedIf || lastResult == ProcessResult.SucceedIf && !(this is ConditionalNode))
            {
                lastResult = ProcessResult.None; // don't percolate the result up
            }
            return lastResult;
        }
    }
}