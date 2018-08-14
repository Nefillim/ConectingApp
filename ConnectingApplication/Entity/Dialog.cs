using ConnectingApplication.Characters;
using ConnectingApplication.Entity.Characters;
using ConnectingApplication.Managers;
using Core;
using Core.Dialogues;
using Core.Dialogues.DialogueParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectingApplication.Entity
{
    public class Dialog : Dialogue
    {
        public List<DialogueNode> selectableNodes;
        public DialogueNode currentNode;
        public Core.Dialogues.DialogueBlock.BlockType currentBlock;


        public Dialog(Dialogue dialgoue) : base(dialgoue)
        {
            selectableNodes = new List<DialogueNode>();
        }

        public List<DialogueNode> TakeNextNodes()
        {
            List<DialogueNode> newNodes = CoreController.DialogueManager.GetNodesForDialogue(Id.ToString(), currentBlock, EGetDialogueNodeType.next, currentNode.Id);
            if (newNodes.Count > 0)
                return newNodes;
            else
            {
                switch (currentBlock)
                {
                    case Core.Dialogues.DialogueBlock.BlockType.hi:
                        currentBlock++;
                        return TakeNextNodes(0);

                    case Core.Dialogues.DialogueBlock.BlockType.body:
                        if (DialogManager.ActiveDialogs.Count == 1)
                            currentBlock++;
                        else
                            currentBlock = Core.Dialogues.DialogueBlock.BlockType.next;
                        return TakeNextNodes(0);

                    case Core.Dialogues.DialogueBlock.BlockType.bye:
                    case Core.Dialogues.DialogueBlock.BlockType.next:
                        break;

                    default:
                        break;
                }
                return null;
            }
        }

        public List<DialogueNode> TakeNextNodes(int nodeId)
        {
            currentNode = selectableNodes.Find(n => n.Id == nodeId);
            return TakeNextNodes();
        }
    }
}
