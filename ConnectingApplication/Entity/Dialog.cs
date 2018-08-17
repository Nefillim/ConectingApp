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
using UnityEngine;

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
            List<DialogueNode> newNodes = CoreController.DialogueManager.GetNodesForDialogue(Id, currentBlock, EGetDialogueNodeType.next, currentNode.Id);
            if (newNodes.Count > 0)
            {
                selectableNodes = newNodes;
                return newNodes;
            }
            else
            {
                switch (currentBlock)
                {
                    case Core.Dialogues.DialogueBlock.BlockType.hi:
                        currentBlock++;
                        return TakeNextNodes(-1);

                    case Core.Dialogues.DialogueBlock.BlockType.body:
                        if (ConnectingAppManager.DialogManager.ActiveDialogs.Count == 1)
                            currentBlock++;
                        else
                            currentBlock = Core.Dialogues.DialogueBlock.BlockType.next;
                        return TakeNextNodes(-1);

                    case Core.Dialogues.DialogueBlock.BlockType.bye:
                    case Core.Dialogues.DialogueBlock.BlockType.next:
                        break;

                    default:
                        break;
                }
                return newNodes;
            }
        }

        /// <summary>
        /// Если nodeId==-1, то запрашиваем первые ноды для диалога.
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public List<DialogueNode> TakeNextNodes(int nodeId)
        {
            if (nodeId == -1)
            {
                selectableNodes = CoreController.DialogueManager.GetNodesForDialogue(Id, currentBlock, EGetDialogueNodeType.firstNodes);
                return selectableNodes;
            }
            else
            {
                currentNode = selectableNodes.Find(n => n.Id == nodeId);
                return TakeNextNodes();
            }
        }
    }
}
